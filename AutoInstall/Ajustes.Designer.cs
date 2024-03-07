namespace AutoInstall
{
    partial class Ajustes
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Ajustes));
            label1 = new Label();
            pictureBox1 = new PictureBox();
            label8 = new Label();
            radioButton1 = new RadioButton();
            radioButton2 = new RadioButton();
            radioButton3 = new RadioButton();
            radioButton4 = new RadioButton();
            radioButton5 = new RadioButton();
            button1 = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Minecraft", 27.75F, FontStyle.Bold, GraphicsUnit.Point);
            label1.ForeColor = Color.White;
            label1.Location = new Point(90, 23);
            label1.Name = "label1";
            label1.Size = new Size(496, 37);
            label1.TabIndex = 1;
            label1.Text = "Opciones de Rendimiento";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.settings;
            pictureBox1.Location = new Point(12, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(72, 53);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 6;
            pictureBox1.TabStop = false;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Minecraft", 26.25F, FontStyle.Bold, GraphicsUnit.Point);
            label8.ForeColor = Color.White;
            label8.Location = new Point(12, 99);
            label8.Name = "label8";
            label8.Size = new Size(348, 35);
            label8.TabIndex = 13;
            label8.Text = "- Plantillas rapidas";
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Font = new Font("Minecraft", 12F, FontStyle.Bold, GraphicsUnit.Point);
            radioButton1.ForeColor = Color.Lime;
            radioButton1.Location = new Point(43, 154);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(505, 20);
            radioButton1.TabIndex = 14;
            radioButton1.TabStop = true;
            radioButton1.Text = "Calidad Alta - Recomendado para PC gama media-alta ";
            radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.Font = new Font("Minecraft", 12F, FontStyle.Bold, GraphicsUnit.Point);
            radioButton2.ForeColor = Color.Yellow;
            radioButton2.Location = new Point(43, 200);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(473, 20);
            radioButton2.TabIndex = 15;
            radioButton2.TabStop = true;
            radioButton2.Text = "Calidad Media - Recomendado para PC gama media";
            radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton3
            // 
            radioButton3.AutoSize = true;
            radioButton3.Font = new Font("Minecraft", 12F, FontStyle.Bold, GraphicsUnit.Point);
            radioButton3.ForeColor = Color.FromArgb(255, 128, 0);
            radioButton3.Location = new Point(43, 244);
            radioButton3.Name = "radioButton3";
            radioButton3.Size = new Size(461, 20);
            radioButton3.TabIndex = 16;
            radioButton3.TabStop = true;
            radioButton3.Text = "Calidad Baja - Recomendado para PC gama baja";
            radioButton3.UseVisualStyleBackColor = true;
            // 
            // radioButton4
            // 
            radioButton4.AutoSize = true;
            radioButton4.Font = new Font("Minecraft", 12F, FontStyle.Bold, GraphicsUnit.Point);
            radioButton4.ForeColor = Color.Red;
            radioButton4.Location = new Point(43, 290);
            radioButton4.Name = "radioButton4";
            radioButton4.Size = new Size(486, 20);
            radioButton4.TabIndex = 17;
            radioButton4.TabStop = true;
            radioButton4.Text = "Calidad Patata - Recomendado para PC del gobierno";
            radioButton4.UseVisualStyleBackColor = true;
            // 
            // radioButton5
            // 
            radioButton5.AutoSize = true;
            radioButton5.Font = new Font("Minecraft", 12F, FontStyle.Bold, GraphicsUnit.Point);
            radioButton5.ForeColor = Color.Fuchsia;
            radioButton5.Location = new Point(43, 334);
            radioButton5.Name = "radioButton5";
            radioButton5.Size = new Size(480, 20);
            radioButton5.TabIndex = 18;
            radioButton5.TabStop = true;
            radioButton5.Text = "Personalizado - Calidad personalizada por el usuario";
            radioButton5.UseVisualStyleBackColor = true;
            radioButton5.CheckedChanged += radioButton5_CheckedChanged;
            // 
            // button1
            // 
            button1.Location = new Point(529, 331);
            button1.Name = "button1";
            button1.Size = new Size(121, 23);
            button1.TabIndex = 19;
            button1.Text = "Customizar...";
            button1.UseVisualStyleBackColor = true;
            button1.Visible = false;
            button1.Click += button1_Click;
            // 
            // Ajustes
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            BackColor = Color.FromArgb(26, 26, 26);
            ClientSize = new Size(673, 383);
            Controls.Add(button1);
            Controls.Add(radioButton5);
            Controls.Add(radioButton4);
            Controls.Add(radioButton3);
            Controls.Add(radioButton2);
            Controls.Add(radioButton1);
            Controls.Add(label8);
            Controls.Add(pictureBox1);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Ajustes";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Opciones de rendimiento";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label label1;
        private PictureBox pictureBox1;
        private Label label8;
        private RadioButton radioButton1;
        private RadioButton radioButton2;
        private RadioButton radioButton3;
        private RadioButton radioButton4;
        private RadioButton radioButton5;
        private Button button1;
    }
}