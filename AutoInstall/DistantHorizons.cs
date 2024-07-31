using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoInstall
{
    public partial class DistantHorizons : Form
    {
        public delegate void TipoRenderizadoCambiadoEventHandler(string tipoRenderizado);
        public event TipoRenderizadoCambiadoEventHandler TipoRenderizadoCambiado;

        public string selectedPath { get; set; }

        public DistantHorizons(string selectedPath, string tipoRenderizado)
        {
            InitializeComponent();
            this.selectedPath = selectedPath ?? throw new ArgumentNullException(nameof(selectedPath));
            ActualizarColoresLabels(tipoRenderizado);

            // Inicializar el evento TipoRenderizadoCambiado
            TipoRenderizadoCambiado = delegate { };
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            label1.ForeColor = Color.Green;

            foreach (Control control in Controls)
            {
                if (control is Label label && label != label1)
                {
                    label.ForeColor = Color.White;
                }
            }
            TipoRenderizadoCambiado?.Invoke("Tipo Renderizado: Un bloque");
        }

        private void PictureBox1_MouseEnter(object sender, EventArgs e)
        {
            pictureBox1.Width = pictureBox1.Width + 10;
            pictureBox1.Height = pictureBox1.Height + 10;
        }

        private void PictureBox1_MouseLeave(object sender, EventArgs e)
        {
            pictureBox1.Width = pictureBox1.Width - 10;
            pictureBox1.Height = pictureBox1.Height - 10;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            label2.ForeColor = Color.Green;

            foreach (Control control in Controls)
            {
                if (control is Label label && label != label2)
                {
                    label.ForeColor = Color.White;
                }
            }
            TipoRenderizadoCambiado?.Invoke("Tipo Renderizado: 2 bloques");
        }

        private void PictureBox2_MouseEnter(object sender, EventArgs e)
        {
            pictureBox2.Width = pictureBox2.Width + 10;
            pictureBox2.Height = pictureBox2.Height + 10;
        }

        private void PictureBox2_MouseLeave(object sender, EventArgs e)
        {
            pictureBox2.Width = pictureBox2.Width - 10;
            pictureBox2.Height = pictureBox2.Height - 10;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            label3.ForeColor = Color.Green;

            foreach (Control control in Controls)
            {
                if (control is Label label && label != label3)
                {
                    label.ForeColor = Color.White;
                }
            }
            TipoRenderizadoCambiado?.Invoke("Tipo Renderizado: 4 bloques");
        }

        private void PictureBox3_MouseEnter(object sender, EventArgs e)
        {
            pictureBox3.Width = pictureBox3.Width + 10;
            pictureBox3.Height = pictureBox3.Height + 10;
        }

        private void PictureBox3_MouseLeave(object sender, EventArgs e)
        {
            pictureBox3.Width = pictureBox3.Width - 10;
            pictureBox3.Height = pictureBox3.Height - 10;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            label4.ForeColor = Color.Green;

            foreach (Control control in Controls)
            {
                if (control is Label label && label != label4)
                {
                    label.ForeColor = Color.White;
                }
            }
            TipoRenderizadoCambiado?.Invoke("DESACTIVADO");
        }

        private void PictureBox4_MouseEnter(object sender, EventArgs e)
        {
            pictureBox4.Width = pictureBox4.Width + 10;
            pictureBox4.Height = pictureBox4.Height + 10;
        }

        private void PictureBox4_MouseLeave(object sender, EventArgs e)
        {
            pictureBox4.Width = pictureBox4.Width - 10;
            pictureBox4.Height = pictureBox4.Height - 10;
        }



        public void ActualizarColoresLabels(string tipoRenderizado)
        {
            label1.ForeColor = Color.White;
            label2.ForeColor = Color.White;
            label3.ForeColor = Color.White;
            label4.ForeColor = Color.White;

            if (tipoRenderizado == "Tipo Renderizado: Un bloque")
            {
                label1.ForeColor = Color.Green;
            }
            else if (tipoRenderizado == "Tipo Renderizado: 2 bloques")
            {
                label2.ForeColor = Color.Green;
            }
            else if (tipoRenderizado == "Tipo Renderizado: 4 bloques")
            {
                label3.ForeColor = Color.Green;
            }
            else if (tipoRenderizado == "DESACTIVADO")
            {
                label4.ForeColor = Color.Green;
            }
        }


    }
}