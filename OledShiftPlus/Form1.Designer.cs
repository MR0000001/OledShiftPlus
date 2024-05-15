using System.Drawing;
using System.Windows.Forms;

namespace OledShiftPlus
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            button1 = new Button();
            button2 = new Button();
            label1 = new Label();
            label2 = new Label();
            timer1 = new Timer(components);
            button3 = new Button();
            label3 = new Label();
            textBox4 = new TextBox();
            textBox5 = new TextBox();
            groupBox1 = new GroupBox();
            label5 = new Label();
            listBox3 = new ListBox();
            label4 = new Label();
            listBox1 = new ListBox();
            listBox2 = new ListBox();
            listBox4 = new ListBox();
            label6 = new Label();
            label7 = new Label();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // button1
            // 
            button1.FlatAppearance.MouseOverBackColor = Color.Gold;
            button1.FlatStyle = FlatStyle.Flat;
            button1.ForeColor = Color.Crimson;
            button1.Location = new Point(12, 12);
            button1.Name = "button1";
            button1.Size = new Size(94, 61);
            button1.TabIndex = 0;
            button1.Text = "Move Windows";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.FlatAppearance.MouseOverBackColor = Color.Gold;
            button2.FlatStyle = FlatStyle.Flat;
            button2.ForeColor = Color.Crimson;
            button2.Location = new Point(112, 12);
            button2.Name = "button2";
            button2.Size = new Size(143, 61);
            button2.TabIndex = 1;
            button2.Text = "Automatic Move [ON]";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = Color.Crimson;
            label1.Location = new Point(311, 20);
            label1.Name = "label1";
            label1.Size = new Size(62, 20);
            label1.TabIndex = 3;
            label1.Text = "seconds";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = Color.Crimson;
            label2.Location = new Point(311, 46);
            label2.Name = "label2";
            label2.Size = new Size(47, 20);
            label2.TabIndex = 5;
            label2.Text = "pixels";
            // 
            // timer1
            // 
            timer1.Tick += timer1_Tick;
            // 
            // button3
            // 
            button3.FlatAppearance.MouseOverBackColor = Color.Gold;
            button3.FlatStyle = FlatStyle.Flat;
            button3.Location = new Point(6, 21);
            button3.Name = "button3";
            button3.Size = new Size(143, 29);
            button3.TabIndex = 6;
            button3.Text = "OverLay [ON]";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(209, 25);
            label3.Name = "label3";
            label3.Size = new Size(110, 20);
            label3.TabIndex = 8;
            label3.Text = "Black Pixel Size";
            // 
            // textBox4
            // 
            textBox4.BackColor = Color.Black;
            textBox4.ForeColor = Color.Crimson;
            textBox4.Location = new Point(10, 80);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(38, 27);
            textBox4.TabIndex = 9;
            textBox4.Text = "1";
            // 
            // textBox5
            // 
            textBox5.BackColor = Color.Black;
            textBox5.ForeColor = Color.Crimson;
            textBox5.Location = new Point(70, 80);
            textBox5.Name = "textBox5";
            textBox5.Size = new Size(38, 27);
            textBox5.TabIndex = 10;
            textBox5.Text = "100";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(listBox3);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(button3);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(textBox5);
            groupBox1.Controls.Add(textBox4);
            groupBox1.ForeColor = Color.Crimson;
            groupBox1.Location = new Point(12, 106);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(370, 110);
            groupBox1.TabIndex = 11;
            groupBox1.TabStop = false;
            groupBox1.Text = "OverLay Feature";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Impact", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.Location = new Point(53, 79);
            label5.Name = "label5";
            label5.Size = new Size(13, 19);
            label5.TabIndex = 15;
            label5.Text = ":";
            // 
            // listBox3
            // 
            listBox3.BackColor = Color.Black;
            listBox3.ForeColor = Color.Crimson;
            listBox3.FormattingEnabled = true;
            listBox3.Location = new Point(155, 23);
            listBox3.Name = "listBox3";
            listBox3.ScrollAlwaysVisible = true;
            listBox3.Size = new Size(48, 24);
            listBox3.TabIndex = 14;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(7, 57);
            label4.Name = "label4";
            label4.Size = new Size(124, 20);
            label4.TabIndex = 11;
            label4.Text = "Black Pixels Ratio";
            // 
            // listBox1
            // 
            listBox1.BackColor = Color.Black;
            listBox1.ForeColor = Color.Crimson;
            listBox1.FormattingEnabled = true;
            listBox1.Location = new Point(261, 17);
            listBox1.Name = "listBox1";
            listBox1.ScrollAlwaysVisible = true;
            listBox1.Size = new Size(49, 24);
            listBox1.TabIndex = 12;
            listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
            // 
            // listBox2
            // 
            listBox2.BackColor = Color.Black;
            listBox2.ForeColor = Color.Crimson;
            listBox2.FormattingEnabled = true;
            listBox2.Location = new Point(262, 45);
            listBox2.Name = "listBox2";
            listBox2.ScrollAlwaysVisible = true;
            listBox2.Size = new Size(48, 24);
            listBox2.TabIndex = 13;
            // 
            // listBox4
            // 
            listBox4.BackColor = Color.Black;
            listBox4.ForeColor = Color.Crimson;
            listBox4.FormattingEnabled = true;
            listBox4.Location = new Point(261, 76);
            listBox4.Name = "listBox4";
            listBox4.ScrollAlwaysVisible = true;
            listBox4.Size = new Size(49, 24);
            listBox4.TabIndex = 14;
            // 
            // label6
            // 
            label6.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label6.BackColor = SystemColors.ActiveCaptionText;
            label6.ForeColor = Color.Crimson;
            label6.Location = new Point(118, 79);
            label6.Name = "label6";
            label6.RightToLeft = RightToLeft.No;
            label6.Size = new Size(143, 16);
            label6.TabIndex = 15;
            label6.Text = "Restore position after";
            label6.TextAlign = ContentAlignment.TopRight;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.ForeColor = Color.Crimson;
            label7.Location = new Point(311, 78);
            label7.Name = "label7";
            label7.Size = new Size(86, 20);
            label7.TabIndex = 16;
            label7.Text = "movements";
            label7.Click += label7_Click;
            // 
            // Form1
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = SystemColors.ActiveCaptionText;
            ClientSize = new Size(394, 224);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(listBox4);
            Controls.Add(listBox2);
            Controls.Add(listBox1);
            Controls.Add(groupBox1);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(button2);
            Controls.Add(button1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            ShowIcon = false;
            Text = "OledShiftPlus";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Button button2;
        private Label label1;
        private Label label2;
        private System.Windows.Forms.Timer timer1;
        private Button button3;
        private Label label3;
        private TextBox textBox4;
        private TextBox textBox5;
        private GroupBox groupBox1;
        private ListBox listBox1;
        private ListBox listBox2;
        private ListBox listBox3;
        private Label label4;
        private Label label5;
        private ListBox listBox4;
        private Label label6;
        private Label label7;
    }
}