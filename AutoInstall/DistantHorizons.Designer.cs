namespace AutoInstall
{
    partial class DistantHorizons
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
            pictureBox1 = new PictureBox();
            pictureBox2 = new PictureBox();
            pictureBox3 = new PictureBox();
            pictureBox4 = new PictureBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Cursor = Cursors.Hand;
            pictureBox1.Image = Properties.Resources._1_Block_sq;
            pictureBox1.Location = new Point(12, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(445, 442);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            pictureBox1.MouseEnter += PictureBox1_MouseEnter;
            pictureBox1.MouseLeave += PictureBox1_MouseLeave;
            // 
            // pictureBox2
            // 
            pictureBox2.Cursor = Cursors.Hand;
            pictureBox2.Image = Properties.Resources._2_Blocks_sq;
            pictureBox2.Location = new Point(510, 12);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(445, 442);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 1;
            pictureBox2.TabStop = false;
            pictureBox2.Click += pictureBox2_Click;
            pictureBox2.MouseEnter += PictureBox2_MouseEnter;
            pictureBox2.MouseLeave += PictureBox2_MouseLeave;
            // 
            // pictureBox3
            // 
            pictureBox3.Cursor = Cursors.Hand;
            pictureBox3.Image = Properties.Resources._4_Blocks_sq;
            pictureBox3.Location = new Point(12, 515);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(445, 442);
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox3.TabIndex = 2;
            pictureBox3.TabStop = false;
            pictureBox3.Click += pictureBox3_Click;
            pictureBox3.MouseEnter += PictureBox3_MouseEnter;
            pictureBox3.MouseLeave += PictureBox3_MouseLeave;
            // 
            // pictureBox4
            // 
            pictureBox4.Cursor = Cursors.Hand;
            pictureBox4.Image = Properties.Resources.Off_sq;
            pictureBox4.Location = new Point(508, 515);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(445, 442);
            pictureBox4.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox4.TabIndex = 3;
            pictureBox4.TabStop = false;
            pictureBox4.Click += pictureBox4_Click;
            pictureBox4.MouseEnter += PictureBox4_MouseEnter;
            pictureBox4.MouseLeave += PictureBox4_MouseLeave;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Minecraft", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            label1.ForeColor = Color.White;
            label1.Location = new Point(113, 471);
            label1.Name = "label1";
            label1.Size = new Size(232, 19);
            label1.TabIndex = 4;
            label1.Text = "Renderizar un bloque";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Minecraft", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            label2.ForeColor = Color.White;
            label2.Location = new Point(604, 471);
            label2.Name = "label2";
            label2.Size = new Size(253, 19);
            label2.TabIndex = 5;
            label2.Text = "Renderizar dos bloques";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Minecraft", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            label3.ForeColor = Color.White;
            label3.Location = new Point(84, 976);
            label3.Name = "label3";
            label3.Size = new Size(287, 19);
            label3.TabIndex = 6;
            label3.Text = "Renderizar cuatro bloques";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Minecraft", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            label4.ForeColor = Color.White;
            label4.Location = new Point(628, 976);
            label4.Name = "label4";
            label4.Size = new Size(229, 19);
            label4.TabIndex = 7;
            label4.Text = "Desactivado (Vanilla)";
            // 
            // DistantHorizons
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            BackColor = Color.FromArgb(26, 26, 26);
            ClientSize = new Size(965, 1015);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(pictureBox4);
            Controls.Add(pictureBox3);
            Controls.Add(pictureBox2);
            Controls.Add(pictureBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DistantHorizons";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Seleccione una configuración para el Distant Horizons";
            TransparencyKey = SystemColors.ActiveBorder;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private PictureBox pictureBox4;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
    }
}