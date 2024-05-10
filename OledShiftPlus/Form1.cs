using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Threading;
using System.Reflection;

namespace OledShiftPlus
{
    public partial class Form1 : Form
    {
        // Dichiarazioni per l'overlay
        private OverlayForm overlayForm;
        private static OverlayForm staticOverlayForm;
        private static int movepx = 0;
        Boolean overlay = true;

        private string settingsFilePath = "settings.json";
        private Settings settings;


        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);


        // Definizione dei flag per SetWindowPos
        public const uint SWP_ASYNCWINDOWPOS = 0x4000;
        public const uint SWP_NOZORDER = 0x0004;
        public const uint SWP_NOACTIVATE = 0x0010;


        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        Boolean auto = true;

        bool writeoncewindowslog = true;

        // Codice esistente per il timer
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (overlay)
            {
                overlayForm.ReloadOverlay(overlay);
                overlayForm.Setpx(GetMovepx(listBox3.SelectedItem?.ToString()));
                overlayForm.Setratio(GetMovepx(textBox4.Text), GetMovepx(textBox5.Text));
            }
            
            if (auto){
                MoveAllWindows(GetMovepx(listBox2.Text), overlayForm, writeoncewindowslog);
            }
            
            writeoncewindowslog = false;
        }

        // Lista per memorizzare le posizioni delle finestre
        private static List<WindowPosition> userWndowPosition = new List<WindowPosition>();
        private static List<WindowPosition> movedWndowPosition = new List<WindowPosition>();


        // Contatore per tenere traccia dei movimenti consecutivi
        private static int consecutiveMoves = 0;

        // Funzione per riposizionare le finestre alle posizioni salvate
        public static void RestoreWindowPositions()
        {
            foreach (WindowPosition windowPosition in Form1.userWndowPosition)
            {
                Form1.SetWindowPos(windowPosition.Handle, IntPtr.Zero, windowPosition.Left, windowPosition.Top, windowPosition.Right - windowPosition.Left, windowPosition.Bottom - windowPosition.Top, Form1.SWP_ASYNCWINDOWPOS | Form1.SWP_NOZORDER | Form1.SWP_NOACTIVATE);
            }
        }

        private static void CheckIfWindowPositionChanged(IntPtr hWnd, RECT rect)
        {
            StringBuilder windowText = new StringBuilder(256);
            GetWindowText(hWnd, windowText, 256);
            if(windowText.ToString().Length > 2) { 
                // verifico che la finestra sia stata registrata
                WindowPosition windowPos = userWndowPosition.Find(wp => wp.Handle == hWnd);
                if (windowPos != null)
                {
                    // verifico che sia stato registrato un movimento di finestra
                    WindowPosition mwindowPos = movedWndowPosition.Find(wp => wp.Handle == hWnd);
                    if (mwindowPos != null)
                    {
                        // verifico se la finestra attualmente si trova in una posizione diversa da quella dell' ultimo movimento
                        if (mwindowPos.Left != rect.Left || mwindowPos.Top != rect.Top || mwindowPos.Right != rect.Right || mwindowPos.Bottom != rect.Bottom)
                        {
                            // aggiorno la registrazione posizione finestra con posizione definita dall'utente
                            windowPos.Left = rect.Left;
                            windowPos.Top = rect.Top;
                            windowPos.Right = rect.Right;
                            windowPos.Bottom = rect.Bottom;
                            Console.WriteLine("Finestra "+ windowText.ToString()+" mossa dall'utente, salvo");
                            Form1.consecutiveMoves = 0;
                        }
                    }
                }
                else
                {
                    // If the window is not present in the list, add it
                    userWndowPosition.Add(new WindowPosition
                    {
                        Handle = hWnd,
                        Left = rect.Left,
                        Right = rect.Right,
                        Top = rect.Top,
                        Bottom = rect.Bottom
                    });
                    Console.WriteLine("Finestra " + windowText.ToString() + " mai vista prima, salvo");
                }
            }
        }

        private static void UpdateWindowPosition(IntPtr hWnd, RECT rect)
        {
            // Update window position in the list
            int index = movedWndowPosition.FindIndex(wp => wp.Handle == hWnd);

            // If the element exists in the list, update it
            if (index != -1)
            {
                movedWndowPosition[index] = new WindowPosition
                {
                    Handle = hWnd,
                    Left = rect.Left,
                    Right = rect.Right,
                    Top = rect.Top,
                    Bottom = rect.Bottom
                };
            }
            else
            {
                // If the element doesn't exist, add it to the list
                movedWndowPosition.Add(new WindowPosition
                {
                    Handle = hWnd,
                    Left = rect.Left,
                    Right = rect.Right,
                    Top = rect.Top,
                    Bottom = rect.Bottom
                });
            }
        }

        private static bool MoveWindow(IntPtr hWnd, IntPtr lParam)
        {
            IntPtr hWndOVL = Form1.staticOverlayForm.Handle;
            string[] ignoredWindows = { "PowerToys", "Universal x86 Tuning Utilit", "Default IME", "DDE Server Window", "TouchPad", "MediaContextNotificationWindow", "PToyTrayIconWindow", "AMD:", "DDMExtensio", "OledShiftPlus", "DWM Notification Window" };

            // Controlla se la finestra è l'overlay, se lo è, passa alla prossima iterazione
            if (hWnd != hWndOVL && IsWindowVisible(hWnd) && !IsIconic(hWnd))
            {
                uint processId;
                GetWindowThreadProcessId(hWnd, out processId);
                StringBuilder windowText = new StringBuilder(256);
                GetWindowText(hWnd, windowText, 256);

                // Ottenere il processo associato alla finestra
                Process process = Process.GetProcessById((int)processId);

                string windowName = windowText.ToString();
                if (windowName.Length > 2) {
                    if (!Array.Exists(ignoredWindows, windowkeyname => windowName.Contains(windowkeyname)) && !Array.Exists(ignoredWindows, windowkeyname => process.ProcessName.Contains(windowkeyname)))
                    {
                        if (GetWindowRect(hWnd, out RECT rect)) // Ottieni il rettangolo della finestra hWnd
                        {
                            CheckIfWindowPositionChanged(hWnd, rect);
                            Random rand = new Random();

                            // Ottieni le dimensioni dello schermo
                            Rectangle screenBounds = Screen.GetBounds(Point.Empty);

                            // Calcola la nuova posizione in modo casuale all'interno dei limiti dello schermo
                            int newLeft = rect.Left + rand.Next(-movepx, movepx + 1);
                            int newTop = rect.Top + rand.Next(-movepx, movepx + 1);

                            // Assicurati che la finestra rimanga all'interno dello schermo
                            newLeft = Math.Max(screenBounds.Left, Math.Min(newLeft, screenBounds.Right - (rect.Right - rect.Left)));
                            newTop = Math.Max(screenBounds.Top, Math.Min(newTop, screenBounds.Bottom - (rect.Bottom - rect.Top)));


                            Console.WriteLine(windowName + " rect.left:" + rect.Left.ToString());
                            Form1.SetWindowPos(hWnd, IntPtr.Zero, newLeft, newTop, rect.Right - rect.Left, rect.Bottom - rect.Top, Form1.SWP_NOZORDER | Form1.SWP_NOACTIVATE);
                            if (GetWindowRect(hWnd, out rect)) // Ottieni il rettangolo della finestra hWnd
                            {
                                //Console.WriteLine(windowName + " rect.left:" + rect.Left.ToString());
                                //Console.WriteLine(windowName + " newleft:" + newLeft.ToString());
                                UpdateWindowPosition(hWnd, rect);
                            }
                        }
                    }
                }
            }

            return true; // Continua l'enumerazione
        }

        // Main loop
        public static void MoveAllWindows(int movepx, OverlayForm overlayForm, bool writeoncewindowslog)
        {
            if (Form1.consecutiveMoves >= 5)
            {
                //Console.WriteLine("Ripristino Posizione");
                Form1.RestoreWindowPositions();
                Form1.consecutiveMoves = 0;
            }
            else
            {
                Form1.consecutiveMoves++;
            }
            //Console.WriteLine(Form1.consecutiveMoves);

            Form1.movepx = movepx;
            Form1.staticOverlayForm = overlayForm;
            EnumWindows(MoveWindow, IntPtr.Zero);

        }

        public Form1()
        {
            InitializeComponent();
            settingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");
            // Inizializza l'overlay
            overlayForm = new OverlayForm();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            overlayForm.ReloadOverlay(overlay);
            overlayForm.Setpx(GetMovepx(listBox3.SelectedItem?.ToString()));
            overlayForm.Setratio(GetMovepx(textBox4.Text), GetMovepx(textBox5.Text));
            MoveAllWindows(GetMovepx(listBox2.Text), overlayForm, writeoncewindowslog);
            writeoncewindowslog = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PopolaListBox(listBox1, 120);
            PopolaListBox(listBox2, 50);
            PopolaListBox(listBox3, 15);

            // Rileva la lingua del sistema
            CultureInfo systemCulture = CultureInfo.CurrentCulture;

            // Imposta la lingua predefinita in inglese
            CultureInfo selectedCulture = new CultureInfo("en-US");

            // Verifica se la lingua del sistema è l'italiano
            if (systemCulture.Name.StartsWith("it"))
            {
                // Se la lingua del sistema è l'italiano, carica le risorse in italiano
                selectedCulture = new CultureInfo("it-IT");
                // Carica le risorse corrispondenti alla lingua selezionata
                ResourceManager rm = new ResourceManager("OledShiftPlus.it_IT", Assembly.GetExecutingAssembly());
                traduciUI(rm, selectedCulture);

            }

            // Imposta la cultura dell'applicazione
            Thread.CurrentThread.CurrentUICulture = selectedCulture;



            settings = Settings.Load(settingsFilePath);
            ApplySettings();


            if (listBox1.SelectedItem != null)
            {
                if (int.TryParse(listBox1.SelectedItem.ToString(), out int intervalInSeconds))
                {
                    // Converti i secondi in millisecondi per l'intervallo del timer
                    timer1.Interval = intervalInSeconds * 1000;
                    // Avvia il timer
                    timer1.Start();
                }
                else
                {
                    MessageBox.Show("Il valore selezionato non è un numero valido di secondi.");
                }
            }
            else
            {
                MessageBox.Show("Nessun valore selezionato.");
            }

            
        }

        private void traduciUI(ResourceManager rm, CultureInfo selectedCulture)
        {
            button1.Text = rm.GetString("button1", selectedCulture);
            button2.Text = rm.GetString("button2", selectedCulture);
            label1.Text = rm.GetString("label1", selectedCulture);
            label3.Text = rm.GetString("label3", selectedCulture);
            label4.Text = rm.GetString("label4", selectedCulture);


        }

        private void PopolaListBox(ListBox listbox, int maxint)
        {
            // Pulisci la ListBox
            listbox.Items.Clear();

            // Aggiungi numeri da 1 a 100 alla ListBox
            for (int i = 1; i <= maxint; i++)
            {
                listbox.Items.Add(i);
            }
        }

        private void ApplySettingsListBoxs(ListBox listbox, string value) {
            int index = listbox.FindStringExact(value);
            if (index != ListBox.NoMatches)
            {
                listbox.SelectedIndex = index;
            }
            else
            {
                MessageBox.Show("Valore non trovato nella ListBox.");
            }
        }

        private void ApplySettings()
        {
            // Applica le impostazioni ai controlli del form
            ApplySettingsListBoxs(listBox1, settings.IntervalInSeconds.ToString());
            ApplySettingsListBoxs(listBox2, settings.MovePixels.ToString());
            ApplySettingsListBoxs(listBox3, settings.pblacksize.ToString());

            textBox4.Text = settings.ptotal.ToString();
            textBox5.Text = settings.pwhite.ToString();
            auto = settings.AutoMode;
            overlay = settings.OverlayEnabled;
            if (auto)
            {
                button2.Text = button2.Text.Replace("[OFF]", "[ON]");
            }
            else
            {
                button2.Text = button2.Text.Replace("[ON]", "[OFF]");
            }

            if (overlay)
            {
                overlayForm.Show();
                button3.Text = "OverLay [ON]";
            }
            else
            {
                overlayForm.Hide();
                button3.Text = "OverLay [OFF]";
            }
        }

        private void SaveSettings()
        {
            // Salva le impostazioni
            settings.IntervalInSeconds = int.Parse(listBox1.SelectedItem?.ToString());
            settings.MovePixels = int.Parse(listBox2.SelectedItem?.ToString());
            settings.pblacksize = int.Parse(listBox3.SelectedItem?.ToString());
            settings.ptotal = int.Parse(textBox4.Text);
            settings.pwhite = int.Parse(textBox5.Text);
            settings.AutoMode = auto;
            settings.OverlayEnabled = overlay;
            settings.Save(settingsFilePath);
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (auto)
            {
                auto = false;
                button2.Text = button2.Text.Replace("[ON]", "[OFF]");
            }
            else
            {
                auto = true;
                button2.Text = button2.Text.Replace("[OFF]", "[ON]");
            }
        }

        public static int GetMovepx(string text)
        {
            return int.Parse(text);
        }


        private void button3_Click(object sender, EventArgs e)
        {
            if (overlay)
            {
                overlay = false;
                overlayForm.Hide();
                button3.Text = "OverLay [OFF]";
            }
            else
            {
                overlay = true;
                overlayForm.ReloadOverlay(overlay);
                overlayForm.Setpx(GetMovepx(listBox3.SelectedItem?.ToString()));
                overlayForm.Setratio(GetMovepx(textBox4.Text), GetMovepx(textBox5.Text));
                overlayForm.Show();
                button3.Text = "OverLay [ON]";
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (int.TryParse(listBox1.SelectedItem.ToString(), out int intervalInSeconds))
            {
                // Converti i secondi in millisecondi per l'intervallo del timer
                timer1.Interval = intervalInSeconds * 1000;
            }
            else
            {
                MessageBox.Show("Il valore selezionato non è un numero valido di secondi.");
            }
        }
    }

    // Classe per memorizzare la posizione delle finestre
    public class WindowPosition
    {
        public IntPtr Handle { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }
        public int Top { get; set; }
        public int Bottom { get; set; }
    }


    public class OverlayForm : Form
    {
        private const int WS_EX_LAYERED = 0x80000;
        private const int WS_EX_NOACTIVATE = 0x8000000;
        private const int GWL_EXSTYLE = -20;

        int blackpx = 2;
        int ptotal = 101;
        int pwhite = 100;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        public OverlayForm()
        {
            this.BackColor = Color.Black; // Imposta il colore di sfondo a nero
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;
            this.AutoScaleMode = AutoScaleMode.None;
            this.ControlBox = false;
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Opacity = 0.2; // Imposta l'opacit� a 0.2 per mantenere il nero come colore trasparente
            this.Size = Screen.PrimaryScreen.Bounds.Size;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = Screen.PrimaryScreen.Bounds.Location;
            this.Show();

            // Make the form click-through
            int exStyle = GetWindowLong(this.Handle, GWL_EXSTYLE);
            exStyle |= WS_EX_LAYERED | WS_EX_NOACTIVATE;
            SetWindowLong(this.Handle, GWL_EXSTYLE, exStyle);

            // Set the transparency color key
            SetLayeredWindowAttributes(this.Handle, 0, 0, 0x00000001);
        }

        public void ReloadOverlay(bool doit)
        {
            if (doit)
            {
                // Ridisegna l'overlay
                this.Refresh();
            }
        }

        public void Setpx(int px)
        {
            blackpx = px;
        }

        public void Setratio(int total, int white)
        {
            ptotal = total+white;
            pwhite = white;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            int cellSize = blackpx; // Dimensione di ogni cella dello scacchiere
            Random random = new Random(); // Generatore di numeri casuali

            for (int x = 0; x < ClientSize.Width; x += cellSize)
            {
                for (int y = 0; y < ClientSize.Height; y += cellSize)
                {
                    // Decide casualmente se la cella deve essere bianca o nera
                    // Utilizziamo un rapporto di 5:1 per i rettangoli bianchi
                    bool isBlack = random.Next(ptotal) < pwhite; // 100 probabilit� su 101 di essere bianco

                    // Imposta il colore in base alla decisione casuale
                    Color cellColor = isBlack ? Color.Black : Color.White;

                    // Calcola l'opacit� in base al colore
                    double opacity = isBlack ? 1.0 : 0.1; // Opacit� diversa per nero e bianco

                    // Disegna la cella con il colore scelto e l'opacit� corrispondente
                    using (SolidBrush brush = new SolidBrush(Color.FromArgb((int)(opacity * 255), cellColor)))
                    {
                        e.Graphics.FillRectangle(brush, x, y, cellSize, cellSize);
                    }
                }
            }
        }



    }

    public class Settings
    {
        public int IntervalInSeconds { get; set; }
        public int MovePixels { get; set; }
        public int pblacksize { get; set; }
        public int ptotal { get; set; }
        public int pwhite { get; set; }
        public bool AutoMode { get; set; }
        public bool OverlayEnabled { get; set; }

        public static Settings FromJson(string json)
        {
            return JsonSerializer.Deserialize<Settings>(json);
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        public static Settings Load(string filePath)
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return FromJson(json);
            }
            else
            {
                // File non trovato, restituisci un nuovo oggetto impostazioni
                return new Settings();
            }
        }

        public void Save(string filePath)
        {
            string json = ToJson();
            File.WriteAllText(filePath, json);
        }
    }

}
