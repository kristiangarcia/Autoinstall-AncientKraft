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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace AutoInstall
{
    public partial class Personalizado : Form
    {
        public string? selectedPath { get; set; }

        public Personalizado(string selectedPath)
        {
            InitializeComponent();
            this.selectedPath = selectedPath ?? throw new ArgumentNullException(nameof(selectedPath));
            trackBar1.ValueChanged += trackBar1_ValueChanged; // Asignar el evento ValueChanged al TrackBar
            LoadDrawDistanceFromConfig(); // Llamar explícitamente a la función LoadDrawDistanceFromConfig()
            LoadExtrasFromConfig();
            // Suscribirse al evento FormClosing
            this.FormClosing += Personalizado_FormClosing;


            //Comprobar extras
            string extrasFile = Path.Combine(selectedPath ?? string.Empty, "extras.txt");

            if (File.Exists(extrasFile))
            {
                string checkBoxValue = File.ReadAllText(extrasFile);

                if (checkBoxValue == "true")
                {
                    checkBox1.Checked = true;
                }
                else if (checkBoxValue == "false")
                {
                    checkBox1.Checked = false;
                }
            }
        }

        //Raton de 8 en 8
        private bool isDragging = false;

        private void TrackBar1_MouseDown(object sender, MouseEventArgs e)
        {
            isDragging = true;
        }

        private void TrackBar1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                int newValue = trackBar1.Minimum + (int)(((float)e.X / trackBar1.Width) * (trackBar1.Maximum - trackBar1.Minimum));
                newValue = (newValue / 8) * 8; // Ajustar el valor para que sea un múltiplo de 8
                trackBar1.Value = Math.Max(trackBar1.Minimum, Math.Min(trackBar1.Maximum, newValue));
            }
        }

        private void TrackBar1_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
        }

        private void trackBar1_ValueChanged(object? sender, EventArgs e)
        {
            // Actualizar la etiqueta con el valor actual del TrackBar
            int value = trackBar1.Value;
            label3.Text = "Distancia Renderizado: " + value.ToString();
        }

        private void trackBar2_ValueChanged(object? sender, EventArgs e)
        {
            // Actualizar la etiqueta con el valor actual del TrackBar
            int value = trackBar2.Value;
            label7.Text = "Distancia Entidades: " + value.ToString();
        }

        //Empesemoh
        private void LoadDrawDistanceFromConfig()
        {

            //Comprobar que mod esta habilitado
            string modsFolder = Path.Combine(selectedPath ?? string.Empty, "mods");
            string searchPattern = "DistantHorizons*.jar.disabled";
            string[] matchingFiles = Directory.GetFiles(modsFolder, searchPattern);
            if (matchingFiles.Length > 0)
            {
                label4.Text = "DESACTIVADO";
                trackBar1.Enabled = false;
            }

            else
            {
                trackBar1.Enabled = true;
                string configFile = Path.Combine(selectedPath ?? string.Empty, "config", "DistantHorizons.toml");

                if (File.Exists(configFile))
                {
                    try
                    {
                        string[] lines = File.ReadAllLines(configFile);

                        foreach (string line in lines)
                        {
                            if (line.Contains("lodChunkRenderDistance"))
                            {
                                string[] parts = line.Split('=');
                                if (parts.Length >= 2)
                                {
                                    string drawDistance = parts[1].Trim().Trim('"');
                                    int value;
                                    if (int.TryParse(drawDistance, out value))
                                    {
                                        trackBar1.Value = value;
                                        label3.Text = "Distancia Renderizado: " + value.ToString();
                                    }
                                }
                            }
                            else if (line.Contains("drawResolution"))
                            {
                                string[] parts = line.Split('=');
                                if (parts.Length >= 2)
                                {
                                    string drawResolution = parts[1].Trim().Trim('"');
                                    label4.Text = "Tipo Renderizado: " + GetRenderizadoText(drawResolution);
                                }
                            }
                        }
                    }
                    catch (IOException)
                    {
                        MessageBox.Show("Error al leer el archivo de configuración. Porfavor cierre el juego antes de tocar nada.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private string GetRenderizadoText(string drawResolution)
        {
            switch (drawResolution)
            {
                case "BLOCK":
                    return "Un bloque";
                case "TWO_BLOCKS":
                    return "2 bloques";
                case "FOUR_BLOCKS":
                    return "4 bloques";
                default:
                    return drawResolution;
            }
        }

        private void LoadExtrasFromConfig()
        {
            if (checkBox1.Checked)
            {
                trackBar2.Enabled = true;
                string configFile3 = Path.Combine(selectedPath ?? string.Empty, "config", "rubidium_extras.toml");

                if (File.Exists(configFile3))
                {
                    try
                    {
                        string[] lines = File.ReadAllLines(configFile3);

                        foreach (string line in lines)
                        {
                            if (line.Contains("(Entity) Max Horizontal Render Distance [Squared, Default 64^2]"))
                            {
                                string[] parts = line.Split('=');
                                if (parts.Length >= 2)
                                {
                                    string entityDistance = parts[1].Trim().Trim('"');
                                    double value;
                                    if (double.TryParse(entityDistance, out value))
                                    {
                                        double sqrtValue = Math.Sqrt(value);
                                        trackBar2.Value = (int)sqrtValue;
                                        label7.Text = "Distancia Entidades: " + sqrtValue.ToString();
                                    }
                                }
                            }
                        }
                    }
                    catch (IOException)
                    {
                        MessageBox.Show("Error al leer el archivo de configuración.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                trackBar2.Enabled = false;
            }


        }

        private void Personalizado_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {

                Principal principalForm = (Principal)Application.OpenForms["Principal"];
                principalForm.ajustesCheckBoxValue = checkBox1.Checked;
                principalForm.ajustesTrackBar1Value = trackBar1.Value;
                principalForm.ajustesTrackBar2Value = trackBar2.Value;
                principalForm.textoLabel4 = label4.Text;

                Ajustes ajustesForm = (Ajustes)Application.OpenForms["Ajustes"];
                if (ajustesForm != null)
                {
                    ajustesForm.ajustesCheckBoxValue = checkBox1.Checked;
                    ajustesForm.ajustesTrackBar1Value = trackBar1.Value;
                    ajustesForm.ajustesTrackBar2Value = trackBar2.Value;
                    ajustesForm.textoLabel4 = label4.Text;
                }
            }
        }





        private void button1_Click(object sender, EventArgs e)
        {
            if (selectedPath != null)
            {
                DistantHorizons form2 = new DistantHorizons(selectedPath, label4.Text); // Pasar el valor de label4.Text como tipoRenderizado
                form2.TipoRenderizadoCambiado += ActualizarTipoRenderizado;
                form2.ShowDialog();
                if (label4.Text == "DESACTIVADO")
                {
                    trackBar1.Enabled = false;
                    label3.Text = "Distancia Renderizado: ";
                }
                else
                {
                    trackBar1.Enabled = true;
                    string configFile = Path.Combine(selectedPath ?? string.Empty, "config", "DistantHorizons.toml");

                    if (File.Exists(configFile))
                    {
                        try
                        {
                            string[] lines = File.ReadAllLines(configFile);

                            foreach (string line in lines)
                            {
                                if (line.Contains("lodChunkRenderDistance"))
                                {
                                    string[] parts = line.Split('=');
                                    if (parts.Length >= 2)
                                    {
                                        string drawDistance = parts[1].Trim().Trim('"');
                                        int value;
                                        if (int.TryParse(drawDistance, out value))
                                        {
                                            trackBar1.Value = value;
                                            label3.Text = "Distancia Renderizado: " + value.ToString();
                                        }
                                    }
                                }
                            }
                        }
                        catch (IOException)
                        {
                            MessageBox.Show("Error al leer el archivo de configuración. Porfavor cierre el juego antes de tocar nada.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void ActualizarTipoRenderizado(string tipoRenderizado)
        {
            label4.Text = tipoRenderizado;
        }


        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                LoadExtrasFromConfig();
            }
            else
            {
                label7.Text = "Distancia Entidades: ";
                trackBar2.Enabled = false;
            }

        }

    }
}
