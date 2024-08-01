using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using static System.Windows.Forms.DataFormats;
using System.Reflection.Emit;
using System.Drawing.Drawing2D;

namespace AutoInstall
{
    public partial class Ajustes : Form
    {
        public string? selectedPath { get; set; }
        // Declarar variables para almacenar los valores
        public bool ajustesCheckBoxValue;
        public int ajustesTrackBar1Value;
        public int ajustesTrackBar2Value;
        public string? textoLabel4;

        private bool closingConfirmed = false; // Variable para controlar si se ha confirmado el cierre

        public Ajustes(string selectedPath, bool checkBox1Checked, int trackBar1Value, int trackBar2Value)
        {
            InitializeComponent();
            this.selectedPath = selectedPath ?? throw new ArgumentNullException(nameof(selectedPath));
            this.FormClosing += Ajustes_FormClosing;
            this.ajustesCheckBoxValue = checkBox1Checked; // Guardar el estado de checkBox1
            this.ajustesTrackBar1Value = trackBar1Value;
            this.ajustesTrackBar2Value = trackBar2Value;
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;

            //Comprobar preset
            string presetFile = Path.Combine(selectedPath ?? string.Empty, "preset.txt");

            if (File.Exists(presetFile))
            {
                string presetName = File.ReadAllText(presetFile);

                switch (presetName)
                {
                    case "High":
                        radioButton1.Checked = true;
                        break;
                    case "Medium":
                        radioButton2.Checked = true;
                        break;
                    case "Low":
                        radioButton3.Checked = true;
                        break;
                    case "Potato":
                        radioButton4.Checked = true;
                        break;
                    case "Custom":
                        radioButton5.Checked = true;
                        break;
                }
            }


        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            using (var brush = new LinearGradientBrush(
                ClientRectangle,
                Color.FromArgb(50, 50, 50),  // Color inicial (negro)
                Color.FromArgb(26, 26, 26), // Color final (gris)
                LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(brush, ClientRectangle);
            }
        }

        private void Ajustes_Load(object sender, EventArgs e)
        {
            foreach (Control control in Controls)
            {
                if (control is System.Windows.Forms.Label || control is RadioButton || control is TrackBar)
                {
                    control.BackColor = Color.Transparent;
                }
            }
        }


        //Cambios aplicados?
        private void Ajustes_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (!closingConfirmed) // Verificar si hay cambios sin guardar y si no se ha confirmado el cierre
            {
                DialogResult result = MessageBox.Show("¿Desea aplicar los cambios antes de cerrar?", "Cambios sin guardar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    closingConfirmed = true; // Confirmar el cierre

                    // Simular clic en el botón "Apply"
                    apply(this, EventArgs.Empty);

                    // Si deseas cerrar el formulario automáticamente después de aplicar los cambios, descomenta la línea siguiente:
                    // this.Close();
                }
                else
                {
                    closingConfirmed = true; // Confirmar el cierre
                }
            }
        }

        private bool UpdateEntityDistance(string configFile3, string entityDistance)
        {
            if (File.Exists(configFile3))
            {
                try
                {
                    string[] lines = File.ReadAllLines(configFile3);

                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (lines[i].Contains("(Entity) Max Horizontal Render Distance [Squared, Default 64^2]"))
                        {
                            lines[i] = "\"(Entity) Max Horizontal Render Distance [Squared, Default 64^2]\" = " + entityDistance + "";
                            File.WriteAllLines(configFile3, lines);
                            return true;
                        }
                    }

                    return false;
                }
                catch (IOException)
                {
                    return false;
                }
            }

            return false;
        }




        private void apply(object sender, EventArgs e)
        {
            string configFile = Path.Combine(selectedPath ?? string.Empty, "config", "DistantHorizons.toml");
            string drawDistance;
            string maxHorizontalResolution = ""; // Assign a default value

            if (radioButton5.Checked)
            {
                drawDistance = ajustesTrackBar1Value.ToString();
                if (!UpdateDrawDistance(configFile, drawDistance))
                {
                    MessageBox.Show("No se pudo encontrar el archivo de configuración DistantHorizons.toml.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else if (radioButton1.Checked)
            {
                drawDistance = "128";
                maxHorizontalResolution = "BLOCK";
            }
            else if (radioButton2.Checked)
            {
                drawDistance = "128";
                maxHorizontalResolution = "TWO_BLOCKS";
            }
            else if (radioButton3.Checked)
            {
                drawDistance = "64";
                maxHorizontalResolution = "FOUR_BLOCKS";
            }
            else
            {
                // Aplicar preset Potato
                ApplyPreset("Potato", 4, 4, 0, 3);
                radioButton4DrawDistance(this, EventArgs.Empty);
                MessageBox.Show("Se han aplicado los cambios correctamente", "Cambios aplicados", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Presets
            if (radioButton1.Checked)
            {
                // Aplicar preset High
                ApplyPreset("High", 8, 8, 1, 1);
                radioButton1DrawDistance(this, EventArgs.Empty);
            }
            else if (radioButton2.Checked)
            {
                // Aplicar preset Medium
                ApplyPreset("Medium", 8, 6, 1, 1);
                radioButton2DrawDistance(this, EventArgs.Empty);

            }
            else if (radioButton3.Checked)
            {
                // Aplicar preset Low
                ApplyPreset("Low", 6, 6, 0, 2);
                radioButton3DrawDistance(this, EventArgs.Empty);
            }
            else if (radioButton5.Checked)
            {
                SavePresetToFile("Custom");
            }

            if (textoLabel4 == "Tipo Renderizado: Un bloque")
            {
                string modsFolder = Path.Combine(selectedPath ?? string.Empty, "mods");
                string searchPattern = "DistantHorizons*.jar.disabled";
                string[] matchingFiles = Directory.GetFiles(modsFolder, searchPattern);
                if (matchingFiles.Length > 0)
                {
                    string disabledJarFilePath = matchingFiles[0];
                    string enabledJarFilePath = Path.ChangeExtension(disabledJarFilePath, null);
                    try
                    {
                        File.Move(disabledJarFilePath, enabledJarFilePath);
                    }
                    catch (IOException)
                    {
                        MessageBox.Show("No se pudo habilitar el mod DistantHorizons " + Path.GetFileName(enabledJarFilePath) + ".", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    string[] allFiles = Directory.GetFiles(modsFolder, "DistantHorizons*.jar");

                    if (allFiles.Length == 0)
                    {
                        MessageBox.Show("Porfavor instala primero el modpack (El mod DistantHorizons no está instalado)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Close();
                        return;
                    }
                }
                maxHorizontalResolution = "BLOCK";
            }
            else if (textoLabel4 == "Tipo Renderizado: 2 bloques")
            {
                string modsFolder = Path.Combine(selectedPath ?? string.Empty, "mods");
                string searchPattern = "DistantHorizons*.jar.disabled";
                string[] matchingFiles = Directory.GetFiles(modsFolder, searchPattern);
                if (matchingFiles.Length > 0)
                {
                    string disabledJarFilePath = matchingFiles[0];
                    string enabledJarFilePath = Path.ChangeExtension(disabledJarFilePath, null);
                    try
                    {
                        File.Move(disabledJarFilePath, enabledJarFilePath);
                    }
                    catch (IOException)
                    {
                        MessageBox.Show("No se pudo habilitar el mod DistantHorizons " + Path.GetFileName(enabledJarFilePath) + ".", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    string[] allFiles = Directory.GetFiles(modsFolder, "DistantHorizons*.jar");

                    if (allFiles.Length == 0)
                    {
                        MessageBox.Show("Porfavor instala primero el modpack (El mod DistantHorizons no está instalado)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Close();
                        return;
                    }
                }
                maxHorizontalResolution = "TWO_BLOCKS";
            }
            else if (textoLabel4 == "Tipo Renderizado: 4 bloques")
            {
                string modsFolder = Path.Combine(selectedPath ?? string.Empty, "mods");
                string searchPattern = "DistantHorizons*.jar.disabled";
                string[] matchingFiles = Directory.GetFiles(modsFolder, searchPattern);
                if (matchingFiles.Length > 0)
                {
                    string disabledJarFilePath = matchingFiles[0];
                    string enabledJarFilePath = Path.ChangeExtension(disabledJarFilePath, null);
                    try
                    {
                        File.Move(disabledJarFilePath, enabledJarFilePath);
                    }
                    catch (IOException)
                    {
                        MessageBox.Show("No se pudo habilitar el mod DistantHorizons " + Path.GetFileName(enabledJarFilePath) + ".", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    string[] allFiles = Directory.GetFiles(modsFolder, "DistantHorizons*.jar");

                    if (allFiles.Length == 0)
                    {
                        MessageBox.Show("Porfavor instala primero el modpack (El mod DistantHorizons no está instalado)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Close();
                        return;
                    }
                }
                maxHorizontalResolution = "FOUR_BLOCKS";
            }
            else if (textoLabel4 == "DESACTIVADO")
            {
                string modsFolder = Path.Combine(selectedPath ?? string.Empty, "mods");
                string searchPattern = "DistantHorizons*.jar";
                string[] matchingFiles = Directory.GetFiles(modsFolder, searchPattern);

                if (matchingFiles.Length > 0)
                {
                    string jarFilePath = matchingFiles[0];
                    string disabledJarFilePath = Path.ChangeExtension(jarFilePath, ".jar.disabled.");

                    try
                    {
                        File.Move(jarFilePath, disabledJarFilePath);
                    }
                    catch (IOException)
                    {
                        MessageBox.Show("No se pudo deshabilitar el archivo " + Path.GetFileName(jarFilePath) + ".", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            if (UpdateDrawDistance(configFile, drawDistance))
            {
                if (UpdatemaxHorizontalResolution(configFile, maxHorizontalResolution))
                {
                    MessageBox.Show("Se han aplicado los cambios correctamente", "Cambios aplicados", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No se pudo encontrar un archivo de configuración. Reinstala el Modpack", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("No se pudo encontrar un archivo de configuración. Reinstala el Modpack", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }



        }

        private void SavePresetToFile(string presetName)
        {
            string presetFile = Path.Combine(selectedPath ?? string.Empty, "preset.txt");

            try
            {
                File.WriteAllText(presetFile, presetName);
            }
            catch (IOException)
            {
                MessageBox.Show("Error al guardar el archivo de preset.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool UpdateDrawDistance(string configFile, string drawDistance)
        {
            if (File.Exists(configFile))
            {
                try
                {
                    string[] lines = File.ReadAllLines(configFile);

                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (lines[i].Contains("lodChunkRenderDistanceRadius"))
                        {
                            lines[i] = "lodChunkRenderDistanceRadius = " + drawDistance + "";
                            File.WriteAllLines(configFile, lines);
                            return true;
                        }
                    }

                    return false;
                }
                catch (IOException)
                {
                    return false;
                }
            }

            return false;
        }

        private void ApplyPreset(string presetName, int renderDistance, int simulationDistance, int graphicsMode, int qualityMode)
        {
            string configFile = Path.Combine(selectedPath ?? string.Empty, "options.txt");

            if (!File.Exists(configFile))
            {
                configFile = Path.Combine(selectedPath ?? string.Empty, "config", "defaultoptions", "options.txt");
                if (!File.Exists(configFile))
                {
                    MessageBox.Show("options.txt no encontrado en ninguna de las ubicaciones esperadas.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            string[] lines = File.ReadAllLines(configFile);

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("renderDistance:"))
                {
                    lines[i] = "renderDistance: " + renderDistance;
                }
                else if (lines[i].StartsWith("simulationDistance:"))
                {
                    lines[i] = "simulationDistance: " + simulationDistance;
                }
                else if (lines[i].StartsWith("graphicsMode:"))
                {
                    lines[i] = "graphicsMode: " + graphicsMode;
                }
            }

            File.WriteAllLines(configFile, lines);

            // Cambiar el valor de Quality Mode en dynamic_lights_reforged.toml
            string dynamicLightsConfigFile = Path.Combine(selectedPath ?? string.Empty, "config", "dynamic_lights_reforged.toml");
            string[] dynamicLightsLines = File.ReadAllLines(dynamicLightsConfigFile);

            for (int i = 0; i < dynamicLightsLines.Length; i++)
            {
                if (dynamicLightsLines[i].Contains("Quality Mode (OFF, SLOW, FAST, REALTIME)"))
                {
                    dynamicLightsLines[i] = "\"Quality Mode (OFF, SLOW, FAST, REALTIME)\" = \"" + GetQualityModeString(qualityMode) + "\"";
                    break; // Salir del bucle una vez que se ha encontrado y modificado la línea adecuada
                }
            }

            File.WriteAllLines(dynamicLightsConfigFile, dynamicLightsLines);

            SavePresetToFile(presetName);
            //}
            //catch (IOException)
            //{
            //MessageBox.Show("Error al actualizar los archivos de configuración. Inicia Minecraft al menos una vez o reinstala el Modpack", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }


        private string GetQualityModeString(int qualityMode)
        {
            switch (qualityMode)
            {
                case 1:
                    return "REALTIME";
                case 2:
                    return "FAST";
                case 3:
                    return "SLOW";
                default:
                    return "OFF";
            }
        }

        private void radioButton1DrawDistance(object sender, EventArgs e)
        {
            string modsFolder = Path.Combine(selectedPath ?? string.Empty, "mods");
            string searchPattern = "DistantHorizons*.jar.disabled";
            string[] matchingFiles = Directory.GetFiles(modsFolder, searchPattern);
            if (matchingFiles.Length > 0)
            {
                string disabledJarFilePath = matchingFiles[0];
                string enabledJarFilePath = Path.ChangeExtension(disabledJarFilePath, null);
                try
                {
                    File.Move(disabledJarFilePath, enabledJarFilePath);
                }
                catch (IOException)
                {
                    MessageBox.Show("No se pudo habilitar el mod DistantHorizons " + Path.GetFileName(enabledJarFilePath) + ".", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                string[] allFiles = Directory.GetFiles(modsFolder, "DistantHorizons*.jar");

                if (allFiles.Length == 0)
                {
                    MessageBox.Show("Porfavor instala primero el modpack (El mod DistantHorizons no está instalado)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }
            }


            string configFile = Path.Combine(selectedPath ?? string.Empty, "config", "DistantHorizons.toml");
            string maxHorizontalResolution = "BLOCK";

            if (UpdatemaxHorizontalResolution(configFile, maxHorizontalResolution))
            {

            }
            else
            {
                MessageBox.Show("No se pudo encontrar el archivo de configuración o el valor de maxHorizontalResolution en el archivo.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            string[] lines = File.ReadAllLines(configFile);

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("lodChunkRenderDistanceRadius"))
                {
                    lines[i] = "lodChunkRenderDistanceRadius = 128";
                    File.WriteAllLines(configFile, lines);

                }
            }
        }

        private void radioButton2DrawDistance(object sender, EventArgs e)
        {
            string modsFolder = Path.Combine(selectedPath ?? string.Empty, "mods");
            string searchPattern = "DistantHorizons*.jar.disabled";
            string[] matchingFiles = Directory.GetFiles(modsFolder, searchPattern);
            if (matchingFiles.Length > 0)
            {
                string disabledJarFilePath = matchingFiles[0];
                string enabledJarFilePath = Path.ChangeExtension(disabledJarFilePath, null);
                try
                {
                    File.Move(disabledJarFilePath, enabledJarFilePath);
                }
                catch (IOException)
                {
                    MessageBox.Show("No se pudo habilitar el mod DistantHorizons " + Path.GetFileName(enabledJarFilePath) + ".", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                string[] allFiles = Directory.GetFiles(modsFolder, "DistantHorizons*.jar");

                if (allFiles.Length == 0)
                {
                    MessageBox.Show("Porfavor instala primero el modpack (El mod DistantHorizons no está instalado)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }
            }


            string configFile = Path.Combine(selectedPath ?? string.Empty, "config", "DistantHorizons.toml");
            string maxHorizontalResolution = "TWO_BLOCKS";

            if (UpdatemaxHorizontalResolution(configFile, maxHorizontalResolution))
            {

            }
            else
            {
                MessageBox.Show("No se pudo encontrar el archivo de configuración o el valor de maxHorizontalResolution en el archivo.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            string[] lines = File.ReadAllLines(configFile);

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("lodChunkRenderDistanceRadius"))
                {
                    lines[i] = "lodChunkRenderDistanceRadius = 128";
                    File.WriteAllLines(configFile, lines);

                }
            }

        }

        private void radioButton3DrawDistance(object sender, EventArgs e)
        {
            string modsFolder = Path.Combine(selectedPath ?? string.Empty, "mods");
            string searchPattern = "DistantHorizons*.jar.disabled";
            string[] matchingFiles = Directory.GetFiles(modsFolder, searchPattern);
            if (matchingFiles.Length > 0)
            {
                string disabledJarFilePath = matchingFiles[0];
                string enabledJarFilePath = Path.ChangeExtension(disabledJarFilePath, null);
                try
                {
                    File.Move(disabledJarFilePath, enabledJarFilePath);
                }
                catch (IOException)
                {
                    MessageBox.Show("No se pudo habilitar el mod DistantHorizons " + Path.GetFileName(enabledJarFilePath) + ".", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                string[] allFiles = Directory.GetFiles(modsFolder, "DistantHorizons*.jar");

                if (allFiles.Length == 0)
                {
                    MessageBox.Show("Porfavor instala primero el modpack (El mod DistantHorizons no está instalado)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }
            }


            string configFile = Path.Combine(selectedPath ?? string.Empty, "config", "DistantHorizons.toml");
            string maxHorizontalResolution = "FOUR_BLOCKS";

            if (UpdatemaxHorizontalResolution(configFile, maxHorizontalResolution))
            {

            }
            else
            {
                MessageBox.Show("No se pudo encontrar el archivo de configuración o el valor de maxHorizontalResolution en el archivo.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            string[] lines = File.ReadAllLines(configFile);

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("lodChunkRenderDistanceRadius"))
                {
                    lines[i] = "lodChunkRenderDistanceRadius = 64";
                    File.WriteAllLines(configFile, lines);

                }
            }

        }


        private void radioButton4DrawDistance(object sender, EventArgs e)
        {
            string modsFolder = Path.Combine(selectedPath ?? string.Empty, "mods");
            string searchPattern = "DistantHorizons*.jar";
            string[] matchingFiles = Directory.GetFiles(modsFolder, searchPattern);

            if (matchingFiles.Length > 0)
            {
                string jarFilePath = matchingFiles[0];
                string disabledJarFilePath = Path.ChangeExtension(jarFilePath, ".jar.disabled.");

                try
                {
                    File.Move(jarFilePath, disabledJarFilePath);
                }
                catch (IOException)
                {
                    MessageBox.Show("No se pudo deshabilitar el archivo " + Path.GetFileName(jarFilePath) + ".", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private bool UpdatemaxHorizontalResolution(string configFile, string maxHorizontalResolution)
        {
            if (File.Exists(configFile))
            {
                try
                {
                    string[] lines = File.ReadAllLines(configFile);

                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (lines[i].Contains("maxHorizontalResolution"))
                        {
                            lines[i] = "maxHorizontalResolution = \"" + maxHorizontalResolution + "\"";
                            File.WriteAllLines(configFile, lines);
                            return true;
                        }
                    }

                    return false;
                }
                catch (IOException)
                {
                    return false;
                }
            }

            return false;
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton5.Checked)
            {
                button1.Visible = true;
            }
            else
            {
                button1.Visible = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (selectedPath != null)
            {
                Personalizado form2 = new Personalizado(selectedPath);
                form2.ShowDialog();
            }
        }


    }
}
