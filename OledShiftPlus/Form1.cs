using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace OledShiftPlus
{
    public partial class Form1 : Form
    {
        // Dichiarazioni per l'overlay
        private OverlayForm overlayForm;
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

        [DllImport("user32.dll")]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

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
        // Codice esistente per il timer
        private void timer1_Tick(object sender, EventArgs e)
        {
            overlayForm.ReloadOverlay(overlay);
            overlayForm.Setpx(GetMovepx(textBox3.Text));
            overlayForm.Setratio(GetMovepx(textBox4.Text), GetMovepx(textBox5.Text));
            MoveAllWindows(GetMovepx(textBox2.Text), overlayForm);
        }

        

        public static void MoveAllWindows(int movepx, OverlayForm overlayForm)
        {
            IntPtr hWnd = IntPtr.Zero;
            IntPtr hWndOVL = overlayForm.Handle;
            string[] ignoredWindows = { "PowerToys", "Universal x86 Tuning Utilit", "Default IME", "DDE Server Window", "TouchPad", "MediaContextNotificationWindow", "PToyTrayIconWindow", "AMD:", "DDMExtensio", "OledShiftPlus", "DWM Notification Window" };

            while ((hWnd = Form1.FindWindowEx(IntPtr.Zero, hWnd, null, null)) != IntPtr.Zero)
            {
                // Controlla se la finestra è l'overlay, se lo è, passa alla prossima iterazione
                if (hWnd == hWndOVL)
                    continue;

                if (!Form1.IsWindowMaximized(hWnd))
                {
                    StringBuilder windowText = new StringBuilder(256);
                    GetWindowText(hWnd, windowText, 256);
                    //Console.WriteLine(windowText.ToString());
                    string windowName = windowText.ToString();
                    if (Array.Exists(ignoredWindows, windowkeyname => windowName.Contains(windowkeyname)))
                    {
                        continue;
                    }
                        

                    Form1.RECT rect;
                    if (Form1.GetWindowRect(hWnd, out rect))
                    {
                        Random rand = new Random();
                        int offsetX = rand.Next(-movepx, movepx + 1);
                        int maxOffsetY = Screen.PrimaryScreen.Bounds.Height - (rect.Bottom - rect.Top);
                        int offsetY = rand.Next(-movepx, movepx + 1);

                        if (Math.Min(movepx, maxOffsetY) + 1 > -movepx) {
                            offsetY = rand.Next(-movepx, Math.Min(movepx, maxOffsetY) + 1);
                        }
                        else
                        {
                            offsetY = Math.Min(movepx, maxOffsetY) + 1;
                        }
                        

                        Form1.SetWindowPos(hWnd, IntPtr.Zero, rect.Left + offsetX, rect.Top + offsetY, rect.Right - rect.Left, rect.Bottom - rect.Top, Form1.SWP_ASYNCWINDOWPOS | Form1.SWP_NOZORDER | Form1.SWP_NOACTIVATE);
                    }
                }
            }
        }


        static bool IsWindowMaximized(IntPtr hWnd)
        {
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            placement.length = Marshal.SizeOf(placement);
            GetWindowPlacement(hWnd, ref placement);
            return placement.showCmd == SW_SHOWMAXIMIZED;
        }

        [DllImport("user32.dll")]
        static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        const int SW_SHOWMAXIMIZED = 3;

        [StructLayout(LayoutKind.Sequential)]
        struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public POINT minPosition;
            public POINT maxPosition;
            public RECT normalPosition;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct POINT
        {
            public int x;
            public int y;
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

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
            overlayForm.Setpx(GetMovepx(textBox3.Text));
            overlayForm.Setratio(GetMovepx(textBox4.Text), GetMovepx(textBox5.Text));
            MoveAllWindows(GetMovepx(textBox2.Text), overlayForm);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            settings = Settings.Load(settingsFilePath);
            ApplySettings();

            int intervalInSeconds;
            if (int.TryParse(textBox1.Text, out intervalInSeconds))
            {
                // Converti i secondi in millisecondi per l'intervallo del timer
                timer1.Interval = intervalInSeconds * 1000;
                // Avvia il timer
                timer1.Start();
            }
            else
            {
                MessageBox.Show("Inserisci un valore valido in secondi nella TextBox.");
            }
        }

        private void ApplySettings()
        {
            // Applica le impostazioni ai controlli del form
            textBox1.Text = settings.IntervalInSeconds.ToString();
            textBox2.Text = settings.MovePixels.ToString();
            textBox3.Text = settings.pblacksize.ToString();
            textBox4.Text = settings.ptotal.ToString();
            textBox5.Text = settings.pwhite.ToString();
            auto = settings.AutoMode;
            overlay = settings.OverlayEnabled;
            if (auto)
            {
                button2.Text = "Automatico[ON]";
                timer1.Start();
            }
            else
            {
                button2.Text = "Automatico[OFF]";
                timer1.Stop();
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
            settings.IntervalInSeconds = int.Parse(textBox1.Text);
            settings.MovePixels = int.Parse(textBox2.Text);
            settings.pblacksize = int.Parse(textBox3.Text);
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
                button2.Text = "Automatico[OFF]";
                timer1.Stop();
            }
            else
            {
                auto = true;
                button2.Text = "Automatico[ON]";
                int intervalInSeconds;
                if (int.TryParse(textBox1.Text, out intervalInSeconds))
                {
                    // Converti i secondi in millisecondi per l'intervallo del timer
                    timer1.Interval = intervalInSeconds * 1000;
                    // Avvia il timer
                    timer1.Start();
                }
                else
                {
                    MessageBox.Show("Inserisci un valore valido in secondi nella TextBox.");
                }
                timer1.Start();
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
                overlayForm.Show();
                button3.Text = "OverLay [ON]";
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
        }
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
            this.Opacity = 0.2; // Imposta l'opacità a 0.2 per mantenere il nero come colore trasparente
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
            ptotal = total;
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
                    bool isBlack = random.Next(ptotal) < pwhite; // 100 probabilità su 101 di essere bianco

                    // Imposta il colore in base alla decisione casuale
                    Color cellColor = isBlack ? Color.Black : Color.White;

                    // Calcola l'opacità in base al colore
                    double opacity = isBlack ? 1.0 : 0.1; // Opacità diversa per nero e bianco

                    // Disegna la cella con il colore scelto e l'opacità corrispondente
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
