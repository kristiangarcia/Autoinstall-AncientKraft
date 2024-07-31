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
            button1 = new Button();
            label2 = new Label();
            label4 = new Label();
            trackBar1 = new TrackBar();
            label3 = new Label();
            pictureBox0 = new PictureBox();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox0).BeginInit();
            SuspendLayout();
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
            // pictureBox0
            // 
            pictureBox0.Image = Properties.Resources.settings;
            pictureBox0.Location = new Point(12, 12);
            pictureBox0.Name = "pictureBox0";
            pictureBox0.Size = new Size(72, 53);
            pictureBox0.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox0.TabIndex = 24;
            pictureBox0.TabStop = false;
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
            ClientSize = new Size(746, 316);
            Controls.Add(pictureBox0);
            Controls.Add(label1);
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
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox0).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button button1;
        private Label label2;
        private Label label4;
        private TrackBar trackBar1;
        private Label label3;
        private PictureBox pictureBox0;
        private Label label1;
    }
}