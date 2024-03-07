namespace AutoInstall
{
    partial class Personalizado
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Personalizado));
            trackBar2 = new TrackBar();
            label7 = new Label();
            checkBox1 = new CheckBox();
            label6 = new Label();
            label5 = new Label();
            button1 = new Button();
            label2 = new Label();
            label4 = new Label();
            trackBar1 = new TrackBar();
            label3 = new Label();
            pictureBox1 = new PictureBox();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)trackBar2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // trackBar2
            // 
            trackBar2.Cursor = Cursors.SizeWE;
            trackBar2.LargeChange = 16;
            trackBar2.Location = new Point(365, 438);
            trackBar2.Maximum = 192;
            trackBar2.Minimum = 16;
            trackBar2.Name = "trackBar2";
            trackBar2.Size = new Size(348, 45);
            trackBar2.SmallChange = 8;
            trackBar2.TabIndex = 22;
            trackBar2.Tag = "";
            trackBar2.TickFrequency = 8;
            trackBar2.Value = 192;
            trackBar2.ValueChanged += trackBar2_ValueChanged;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Minecraft", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            label7.ForeColor = Color.White;
            label7.Location = new Point(43, 449);
            label7.Name = "label7";
            label7.Size = new Size(224, 19);
            label7.TabIndex = 21;
            label7.Text = "Distancia Entidades: ";
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            checkBox1.ForeColor = Color.White;
            checkBox1.Location = new Point(365, 370);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(211, 25);
            checkBox1.TabIndex = 20;
            checkBox1.Text = "(Para PC bajos recursos)";
            checkBox1.UseVisualStyleBackColor = true;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Minecraft", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            label6.ForeColor = Color.White;
            label6.Location = new Point(43, 374);
            label6.Name = "label6";
            label6.Size = new Size(294, 19);
            label6.TabIndex = 19;
            label6.Text = "Limitar distancia entidades:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Minecraft", 26.25F, FontStyle.Bold, GraphicsUnit.Point);
            label5.ForeColor = Color.White;
            label5.Location = new Point(12, 300);
            label5.Name = "label5";
            label5.Size = new Size(166, 35);
            label5.TabIndex = 18;
            label5.Text = "- Extras";
            // 
            // button1
            // 
            button1.Cursor = Cursors.Hand;
            button1.Location = new Point(456, 155);
            button1.Name = "button1";
            button1.Size = new Size(257, 23);
            button1.TabIndex = 17;
            button1.Text = "Seleccionar...";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Minecraft", 26.25F, FontStyle.Bold, GraphicsUnit.Point);
            label2.ForeColor = Color.White;
            label2.Location = new Point(12, 101);
            label2.Name = "label2";
            label2.Size = new Size(324, 35);
            label2.TabIndex = 16;
            label2.Text = "- DistantHorizons";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Minecraft", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            label4.ForeColor = Color.White;
            label4.Location = new Point(43, 159);
            label4.Name = "label4";
            label4.Size = new Size(203, 19);
            label4.TabIndex = 15;
            label4.Text = "Tipo Renderizado: ";
            // 
            // trackBar1
            // 
            trackBar1.Cursor = Cursors.SizeWE;
            trackBar1.LargeChange = 16;
            trackBar1.Location = new Point(365, 227);
            trackBar1.Maximum = 128;
            trackBar1.Minimum = 32;
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new Size(348, 45);
            trackBar1.SmallChange = 8;
            trackBar1.TabIndex = 14;
            trackBar1.Tag = "";
            trackBar1.TickFrequency = 8;
            trackBar1.Value = 128;
            trackBar1.ValueChanged += trackBar1_ValueChanged;
            trackBar1.MouseDown += TrackBar1_MouseDown;
            trackBar1.MouseMove += TrackBar1_MouseMove;
            trackBar1.MouseUp += TrackBar1_MouseUp;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Minecraft", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            label3.ForeColor = Color.White;
            label3.Location = new Point(43, 235);
            label3.Name = "label3";
            label3.Size = new Size(261, 19);
            label3.TabIndex = 13;
            label3.Text = "Distancia Renderizado:  ";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.settings;
            pictureBox1.Location = new Point(12, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(72, 53);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 24;
            pictureBox1.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Minecraft", 27.75F, FontStyle.Bold, GraphicsUnit.Point);
            label1.ForeColor = Color.White;
            label1.Location = new Point(90, 22);
            label1.Name = "label1";
            label1.Size = new Size(565, 37);
            label1.TabIndex = 23;
            label1.Text = "Configuracion personalizada";
            // 
            // Personalizado
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            BackColor = Color.FromArgb(26, 26, 26);
            ClientSize = new Size(761, 537);
            Controls.Add(pictureBox1);
            Controls.Add(label1);
            Controls.Add(trackBar2);
            Controls.Add(label7);
            Controls.Add(checkBox1);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(button1);
            Controls.Add(label2);
            Controls.Add(label4);
            Controls.Add(trackBar1);
            Controls.Add(label3);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Personalizado";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Configuración personalizada";
            ((System.ComponentModel.ISupportInitialize)trackBar2).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TrackBar trackBar2;
        private Label label7;
        private CheckBox checkBox1;
        private Label label6;
        private Label label5;
        private Button button1;
        private Label label2;
        private Label label4;
        private TrackBar trackBar1;
        private Label label3;
        private PictureBox pictureBox1;
        private Label label1;
    }
}