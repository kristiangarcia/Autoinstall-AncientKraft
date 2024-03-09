using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ionic.Zip;
using System.Drawing;
using TextBox = System.Windows.Forms.TextBox;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;
using System.Reflection.Metadata;
using System.Management;

namespace AutoInstall
{
    public partial class Principal : Form
    {
        // Declarar variables para almacenar los valores
        public bool ajustesCheckBoxValue;
        public int ajustesTrackBar1Value;
        public int ajustesTrackBar2Value;
        public string? textoLabel4;
        //Program version
        private string program_version = ""; // versión actual del programa
        private string program_pastebinUrl = "https://gist.github.com/Kristiansito/93a6940479e2fb1ecb9acc79dcae3ac0/raw"; // URL de pastebin del programa
        private string programUpdateUrl = ""; // URL de descarga de actualización
        private string newProgramVersion = "";

        //Modpack version
        private string modpack_version = ""; // versión actual del modpack
        private string modpack_pastebinUrl = "https://gist.github.com/Kristiansito/5d113c31c97f6a090c9c8f9f97ade2cf/raw"; // URL de pastebin del modpack
        private string modpackUpdateUrl = ""; // URL de descarga de actualización
        private string newModpackVersion = "";

        //Forge libraries + InfernoLand version
        private string libraries_Url = "https://www.dropbox.com/scl/fi/8c0c7sz9zoax19tmitq5h/forge-1.18.2-libraries-infernoland.zip?rlkey=2te2xdrntjqb43e0aweud29rx&dl=1"; // URL de dropbox de las libraries de Forge + Versions

        private string selectedPath = ""; // Variable para almacenar la ruta seleccionada por el usuario
        private System.Windows.Forms.Timer animationTimer;
        private int animationIndex;
        private readonly string[] animationSequence = { "Descargando", "Descargando.", "Descargando..", "Descargando..." };


        public Principal()
        {
            InitializeComponent();
            CheckForProgramUpdates();
            // Obtener la versión actual del título de la ventana principal
            program_version = ObtenerVersion(this.Text);
            SaveProgramVersion();
            InitializeProgressBar();
            LoadModpackVersion();
            animationTimer = new System.Windows.Forms.Timer();
            LoadCustomPath();


            // Crear el objeto ToolTip
            System.Windows.Forms.ToolTip toolTip = new System.Windows.Forms.ToolTip();
            toolTip.AutoPopDelay = 5000; // Tiempo en milisegundos que muestra el ToolTip
            toolTip.InitialDelay = 250; // Retardo inicial antes de mostrar el ToolTip
            toolTip.ReshowDelay = 100; // Retardo antes de mostrar nuevamente el ToolTip si el cursor se mueve sobre el control

            // Asociar el ToolTip al control PictureBox5
            toolTip.SetToolTip(pictureBox5, "Restablecer ruta...");


            // Asociar el ToolTip al control PictureBox6
            toolTip.SetToolTip(pictureBox6, "Ajustes...");
        }


        // Función para extraer los números de la versión
        private string ObtenerVersion(string titulo)
        {
            // Buscar el patrón "(vX.Y)" en el título
            int indiceInicio = titulo.IndexOf("(v") + 2;
            int indiceFin = titulo.IndexOf(")", indiceInicio);

            if (indiceInicio >= 0 && indiceFin > indiceInicio)
            {
                // Extraer los números de la versión entre los índices encontrados
                string version = titulo.Substring(indiceInicio, indiceFin - indiceInicio);
                return version;
            }

            return "";
        }

        private void InitializeProgressBar()
        {
            progressBarUI.Minimum = 0;
            progressBarUI.Maximum = 100;
            progressBarUI.Step = 1;
            progressBarUI.Visible = false;

            Controls.Add(progressBarUI);
        }

        private async void CheckForProgramUpdates()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Comprobar actualización del programa
                    string programPastebinInfo = await client.GetStringAsync(program_pastebinUrl);
                    string[] programLines = programPastebinInfo.Split('\n');
                    newProgramVersion = programLines[0].Trim();
                    programUpdateUrl = programLines[1].Trim();


                    if (program_version != newProgramVersion)
                    {
                        MessageBox.Show("Se ha encontrado una actualización del programa.\nA continuación se actualizará.", "Actualización (v" + newProgramVersion + ")", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        await DownloadAndUpdateProgram();
                        label1.Text = "Modpack: v" + modpack_version;
                        return;
                    }
                    else
                    {
                        CheckForModpackUpdates();
                    }


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un error al comprobar las actualizaciones:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void LoadCustomPath()
        {
            try
            {
                // Crear rutacompleta del archivo
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "custom_path.txt");

                // Comprobar que exista y leer ruta del archivo
                if (File.Exists(filePath))
                {
                    selectedPath = File.ReadAllText(filePath);
                    button2.Text = selectedPath;
                }
            }
            catch (FileNotFoundException)
            {
                // No hacer nada, se usará la ruta predeterminada
            }
        }

        private void SaveCustomPath()
        {
            // Crear rutacompleta del archivo
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "custom_path.txt");

            // Escribir ruta seleccionada en el archivo
            File.WriteAllText(filePath, selectedPath);
        }

        private async void LoadModpackVersion()
        {
            // Cargar versión del modpack
            string installedFilePath = Path.Combine(selectedPath, "modpack_version.txt");
            if (File.Exists(installedFilePath))
            {
                modpack_version = File.ReadAllText(installedFilePath);
                label1.Visible = true;
                label1.Text = "Modpack: v" + modpack_version;
            }
            else
            {
                button1.Text = "DESCARGAR";
                using (HttpClient client = new HttpClient())
                {
                    // Comprobar actualización del modpack
                    string modpackPastebinInfo = await client.GetStringAsync(modpack_pastebinUrl);
                    string[] modpackLines = modpackPastebinInfo.Split('\n');
                    newModpackVersion = modpackLines[0].Trim();
                    modpackUpdateUrl = modpackLines[1].Trim();
                }
            }

        }

        private async void CheckForModpackUpdates()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Comprobar actualización del modpack
                    string modpackPastebinInfo = await client.GetStringAsync(modpack_pastebinUrl);
                    string[] modpackLines = modpackPastebinInfo.Split('\n');
                    newModpackVersion = modpackLines[0].Trim();
                    modpackUpdateUrl = modpackLines[1].Trim();

                    if (button1.Text != "DESCARGAR")
                    {
                        if (modpack_version != newModpackVersion)
                        {
                            button1.Text = "ACTUALIZAR";
                            MessageBox.Show("Se ha encontrado una actualización del pack de mods", "Actualización (v" + newModpackVersion + ")", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            button1.Text = "REINSTALAR";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un error al comprobar las actualizaciones:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        private async Task DownloadAndUpdateProgram()
        {
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            pictureBox1.Enabled = false;
            pictureBox5.Enabled = false;
            pictureBox6.Enabled = false;
            this.ControlBox = false;
            Cursor = Cursors.WaitCursor;
            // Iniciar animacion Descargando...
            button1.Text = animationSequence[animationIndex];
            animationTimer.Interval = 500;
            animationTimer.Tick += (s, e) =>
            {
                animationIndex = (animationIndex + 1) % animationSequence.Length;
                button1.Text = animationSequence[animationIndex];
            };

            animationTimer.Start();
            try
            {
                // Descargar nuevo programa
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(programUpdateUrl);
                    response.EnsureSuccessStatusCode();
                    byte[] programBytes = await response.Content.ReadAsByteArrayAsync();

                    // Obtener la ruta de la carpeta de Descargas del usuario
                    string downloadsFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads";

                    // Crear el nombre del nuevo archivo
                    string newProgramFileName = $"Asistente de InfernoLand v{newProgramVersion}.exe";

                    // Guardar el nuevo programa en la carpeta de Descargas
                    string newProgramFilePath = Path.Combine(downloadsFolderPath, newProgramFileName);
                    File.WriteAllBytes(newProgramFilePath, programBytes);

                    // Mostrar mensaje de confirmación
                    MessageBox.Show("La actualización se ha descargado correctamente. El nuevo programa se encuentra en la carpeta de Descargas.", "Descarga completada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    animationTimer.Stop();

                    // Ejecutar el nuevo programa
                    Process.Start(newProgramFilePath);

                    // Cerrar la aplicación actual
                    Application.Exit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un error al descargar la actualización del programa:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            string url = "https://discord.gg/ctf8jmquPp";
            System.Diagnostics.Process.Start("cmd", "/c start " + url);
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            // Cambiar el tamaño de la imagen al entrar el cursor
            pictureBox1.Size = new Size(pictureBox1.Width + 10, pictureBox1.Height + 10);
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            // Restaurar el tamaño original de la imagen al salir el cursor
            pictureBox1.Size = new Size(pictureBox1.Width - 10, pictureBox1.Height - 10);
        }


        private void button1_MouseEnter(object sender, EventArgs e)
        {
            button1.BackgroundImage = Properties.Resources.half_widgetss2;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.BackgroundImage = Properties.Resources.half_widgets2;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedPath))
            {
                // Si el usuario no ha seleccionado una ruta, usa la ruta predeterminada
                selectedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "_infernoland");
            }
            if (!string.IsNullOrEmpty(selectedPath))
            {
                SaveCustomPath();
            }

            progressBarUI.Visible = true;
            progressBarUI.Value = 0;
            button1.Enabled = false;
            Cursor = Cursors.WaitCursor;

            // Descargar e instalar actualización
            await DownloadFile(modpackUpdateUrl);

            progressBarUI.Visible = false;
            Cursor = Cursors.Default;
            button1.Enabled = true;
            button2.Enabled = true;
            pictureBox1.Enabled = true;
            pictureBox5.Enabled = true;
            pictureBox6.Enabled = true;
            this.ControlBox = true;

            modpack_version = newModpackVersion; // actualizar versión del programa
            SaveModpackVersion(); // guardar versión en archivo

            Environment.Exit(0);
        }

        private async Task DownloadFile(string url)
        {
            button3.Text = "DESINSTALAR";
            button1.Enabled = false;
            button3.Enabled = false;
            button2.Enabled = false;
            pictureBox1.Enabled = false;
            pictureBox5.Enabled = false;
            pictureBox6.Enabled = false;
            this.ControlBox = false;
            Cursor = Cursors.WaitCursor;

            string optionsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "options.txt"); //Ruta por defecto
            string filePath = Path.Combine(selectedPath, "options.txt"); //Ruta a la que será copiado posteriormente
            string modsPath = Path.Combine(selectedPath, "mods" + Path.DirectorySeparatorChar);

            // Comprobar que el directorio mods existe
            //if (!Directory.Exists(modsPath))
            //{
            //    if (!File.Exists(optionsPath))
            //    {
            //        MessageBox.Show("Por favor, inicie su Launcher en cualquier versión almenos una vez y toque cualquier configuración gráfica, luego cierrelo e intentelo de nuevo...", "No se encontró archivo de configuración", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        Environment.Exit(0);
            //    }
            //    else
            //    {
            //        MessageBox.Show("Por favor, inicie su Launcher al menos una vez en cualquier versión antes de continuar...", "Eh eh eh.. detente!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        Environment.Exit(0);
            //    }
            //}

            //Comprobar que options.txt existe
            if (!File.Exists(optionsPath))
            {
                MessageBox.Show("Por favor, inicie su Launcher en cualquier versión almenos una vez y toque cualquier configuración gráfica, luego cierrelo e intentelo de nuevo...", "No se encontró archivo de configuración", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }


            // Iniciar animacion Descargando...
            button1.Text = animationSequence[animationIndex];
            animationTimer.Interval = 500;
            animationTimer.Tick += (s, e) =>
            {
                animationIndex = (animationIndex + 1) % animationSequence.Length;
                button1.Text = animationSequence[animationIndex];
            };

            animationTimer.Start();

            button2.Text = "Limpiando archivos antiguos...";
            await Task.Delay(1000);

            // Carpetas a limpiar
            string[] foldersToClean = { "mods", "XaeroWorldMap", "Distant_Horizons_server_data", "config/loadmyresources", "config/fancymenu", "config/drippyloadingscreen" };

            foreach (string folder in foldersToClean)
            {
                string folderPath = Path.Combine(selectedPath, folder);
                DirectoryInfo dir = new DirectoryInfo(folderPath);

                if (dir.Exists)
                {
                    // Eliminar archivos dentro de la carpeta
                    FileInfo[] files = dir.GetFiles();

                    foreach (FileInfo file in files)
                    {
                        file.Delete();
                    }

                    // Eliminar subcarpetas
                    DirectoryInfo[] subDirs = dir.GetDirectories();
                    foreach (DirectoryInfo subDir in subDirs)
                    {
                        DeleteDirectory(subDir);
                    }
                }
            }

            // Función recursiva para eliminar una carpeta y sus contenidos
            void DeleteDirectory(DirectoryInfo directory)
            {
                // Eliminar archivos dentro de la carpeta
                foreach (FileInfo file in directory.GetFiles())
                {
                    file.Delete();
                }

                // Eliminar subcarpetas
                foreach (DirectoryInfo subDir in directory.GetDirectories())
                {
                    DeleteDirectory(subDir);
                }

                // Eliminar la carpeta actual
                directory.Delete();
            }


            // Crear control TextBox para el log
            TextBox logTextBox = new TextBox();
            logTextBox.Multiline = true;
            logTextBox.ScrollBars = ScrollBars.Vertical;
            logTextBox.ReadOnly = true;
            logTextBox.Width = 400;
            logTextBox.Height = 200;
            logTextBox.Location = new Point(progressBarUI.Left, progressBarUI.Top - logTextBox.Height - 10);
            logTextBox.Visible = false; // Ocultar el log inicialmente
            Controls.Add(logTextBox);

            // Crear enlace "Mostrar más detalles"
            LinkLabel linkMoreDetails = new LinkLabel();
            linkMoreDetails.Text = "Mostrar más detalles (lento)";
            linkMoreDetails.LinkBehavior = LinkBehavior.HoverUnderline;
            linkMoreDetails.AutoSize = true;
            linkMoreDetails.LinkClicked += (s, e) =>
            {
                if (logTextBox.Visible)
                {
                    logTextBox.Hide();
                    linkMoreDetails.Text = "Mostrar más detalles (lento)";
                    linkMoreDetails.Location = new Point(progressBarUI.Left, progressBarUI.Top - linkMoreDetails.Height - 10);
                }
                else
                {
                    logTextBox.Show();
                    linkMoreDetails.Text = "Ocultar detalles (rápido)";
                    // Mover el botón "Ocultar detalles" encima del log
                    linkMoreDetails.Location = new Point(logTextBox.Left, logTextBox.Top - linkMoreDetails.Height - 10);
                }
            };
            linkMoreDetails.Location = new Point(progressBarUI.Left, progressBarUI.Top - linkMoreDetails.Height - 10);
            Controls.Add(linkMoreDetails);

            //Crear carpeta _infernoland en caso de que la ruta sea por defecto
            if (selectedPath == Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "_infernoland"))
            {
                // Verificar si la carpeta _infernoland ya existe
                if (!Directory.Exists(selectedPath))
                {
                    // Si no existe, crear la carpeta
                    Directory.CreateDirectory(selectedPath);
                }
            }


            button2.Text = "Descargando librerías...";

            // Descargar archivo zip librerias de dropbox
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(libraries_Url, HttpCompletionOption.ResponseHeadersRead))
            using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
            {
                string fileToWriteTo = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "librerias.zip");
                using (Stream streamToWriteTo = File.Open(fileToWriteTo, FileMode.Create))
                {
                    byte[] buffer = new byte[8192];
                    long totalBytes = response.Content.Headers.ContentLength ?? -1;
                    long downloadedBytes = 0;

                    while (true)
                    {
                        int bytesRead = await streamToReadFrom.ReadAsync(buffer, 0, buffer.Length);
                        if (bytesRead == 0)
                        {
                            break;
                        }

                        await streamToWriteTo.WriteAsync(buffer, 0, bytesRead);
                        downloadedBytes += bytesRead;

                        if (totalBytes > 0)
                        {
                            int progress = (int)(downloadedBytes * 100 / totalBytes);
                            progressBarUI.Value = progress;
                        }

                        // Registrar el progreso en el TextBox (si está visible)
                        if (logTextBox.Visible)
                        {
                            string message = $"Descargando: {downloadedBytes} / {totalBytes} bytes";
                            Invoke((MethodInvoker)(() =>
                            {
                                logTextBox.AppendText(message + Environment.NewLine);
                                logTextBox.ScrollToCaret();
                            }));
                        }
                    }
                }
            }


            button2.Text = "Descargando modpack (puede tardar un rato)...";


            // Descargar archivo zip modpack de dropbox
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
            using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
            {
                string fileToWriteTo = Path.Combine(selectedPath, "modpack.zip");
                using (Stream streamToWriteTo = File.Open(fileToWriteTo, FileMode.Create))
                {
                    byte[] buffer = new byte[8192];
                    long totalBytes = response.Content.Headers.ContentLength ?? -1;
                    long downloadedBytes = 0;

                    while (true)
                    {
                        int bytesRead = await streamToReadFrom.ReadAsync(buffer, 0, buffer.Length);
                        if (bytesRead == 0)
                        {
                            break;
                        }

                        await streamToWriteTo.WriteAsync(buffer, 0, bytesRead);
                        downloadedBytes += bytesRead;

                        if (totalBytes > 0)
                        {
                            int progress = (int)(downloadedBytes * 100 / totalBytes);
                            progressBarUI.Value = progress;
                        }

                        // Registrar el progreso en el TextBox (si está visible)
                        if (logTextBox.Visible)
                        {
                            string message = $"Descargando: {downloadedBytes} / {totalBytes} bytes";
                            Invoke((MethodInvoker)(() =>
                            {
                                logTextBox.AppendText(message + Environment.NewLine);
                                logTextBox.ScrollToCaret();
                            }));
                        }
                    }
                }
            }

            animationTimer.Stop();

            button1.Text = "Un segundo...";
            button2.Text = "Extrayendo librerías...";
            await Task.Delay(1500);

            // Extraer archivo librerias.zip
            using (ZipFile zip = ZipFile.Read(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft") + Path.DirectorySeparatorChar + "librerias.zip"))
            {
                zip.ExtractProgress += (sender, e) =>
                {
                    if (e.EventType == ZipProgressEventType.Extracting_AfterExtractEntry)
                    {
                        string message = $"Extrayendo: {e.CurrentEntry.FileName}";
                        Invoke((MethodInvoker)(() =>
                        {
                            logTextBox.AppendText(message + Environment.NewLine);
                            logTextBox.ScrollToCaret();
                        }));
                    }
                    else if (e.EventType == ZipProgressEventType.Extracting_EntryBytesWritten)
                    {
                        int progress = (int)(e.BytesTransferred * 100 / e.TotalBytesToTransfer);
                        Invoke((MethodInvoker)(() => progressBarUI.Value = progress));

                        // Registrar el progreso en el TextBox (si está visible)
                        if (logTextBox.Visible)
                        {
                            string message = $"Extrayendo: {e.BytesTransferred} / {e.TotalBytesToTransfer} bytes";
                            Invoke((MethodInvoker)(() =>
                            {
                                logTextBox.AppendText(message + Environment.NewLine);
                                logTextBox.ScrollToCaret();
                            }));
                        }
                    }
                };

                await Task.Run(() => zip.ExtractAll(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft"), ExtractExistingFileAction.OverwriteSilently));
            }

            button2.Text = "Extrayendo modpack...";

            // Extraer archivo modpack.zip
            using (ZipFile zip = ZipFile.Read(selectedPath + Path.DirectorySeparatorChar + "modpack.zip"))
            {
                zip.ExtractProgress += (sender, e) =>
                {
                    if (e.EventType == ZipProgressEventType.Extracting_AfterExtractEntry)
                    {
                        string message = $"Extrayendo: {e.CurrentEntry.FileName}";
                        Invoke((MethodInvoker)(() =>
                        {
                            logTextBox.AppendText(message + Environment.NewLine);
                            logTextBox.ScrollToCaret();
                        }));
                    }
                    else if (e.EventType == ZipProgressEventType.Extracting_EntryBytesWritten)
                    {
                        int progress = (int)(e.BytesTransferred * 100 / e.TotalBytesToTransfer);
                        Invoke((MethodInvoker)(() => progressBarUI.Value = progress));

                        // Registrar el progreso en el TextBox (si está visible)
                        if (logTextBox.Visible)
                        {
                            string message = $"Extrayendo: {e.BytesTransferred} / {e.TotalBytesToTransfer} bytes";
                            Invoke((MethodInvoker)(() =>
                            {
                                logTextBox.AppendText(message + Environment.NewLine);
                                logTextBox.ScrollToCaret();
                            }));
                        }
                    }
                };

                await Task.Run(() => zip.ExtractAll(selectedPath, ExtractExistingFileAction.OverwriteSilently));
            }

            ///////////////////////////////////////////////////////////////////
            //ESTO SE HACE SOLO SI options.txt no existe en la ruta de destino
            ///////////////////////////////////////////////////////////////////

            // Verificar si el archivo NO existe en la ruta de destino
            if (!File.Exists(filePath))
            {
                // Copiar el archivo options.txt de la ruta de origen a la ruta de destino
                File.Copy(optionsPath, filePath);

                //Deshabilitar resourcepacks activos
                string[] lines = File.ReadAllLines(filePath);

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("resourcePacks"))
                    {
                        lines[i] = "resourcePacks:[]";
                    }
                    else if (lines[i].Contains("incompatibleResourcePacks"))
                    {
                        lines[i] = "incompatibleResourcePacks:[]";
                    }
                }

                File.WriteAllLines(filePath, lines);


                // Leer todas las líneas del archivo
                string[] lines2 = File.ReadAllLines(filePath);

                bool foundBlock = false;
                bool foundMuteMicrophone = false;
                bool foundSoundCategoryMusic = false;

                // Recorrer todas las líneas
                for (int i = 0; i < lines2.Length; i++)
                {
                    // Buscar la línea que contiene "guiScale:0"
                    if (lines2[i].Contains("guiScale:0"))
                    {
                        // Reemplazar el valor "guiScale:0" con "guiScale:3"
                        lines2[i] = lines2[i].Replace("guiScale:0", "guiScale:3");

                        // Guardar los cambios en el archivo
                        File.WriteAllLines(filePath, lines2);
                    }

                    // Buscar el bloque de texto
                    if (lines2[i].Contains("key_key.zoomin:key.keyboard.?") &&
                        lines2[i + 1].Contains("key_key.centerzoom:key.keyboard.?") &&
                        lines2[i + 2].Contains("key_key.zoomout:key.keyboard.?") &&
                        lines2[i + 3].Contains("key_key.rollleft:key.keyboard.?") &&
                        lines2[i + 4].Contains("key_key.rollcenter:key.keyboard.?") &&
                        lines2[i + 5].Contains("key_key.rollright:key.keyboard.?") &&
                        lines2[i + 6].Contains("key_key.point:key.keyboard.?") &&
                        lines2[i + 7].Contains("key_key.startStop:key.keyboard.?") &&
                        lines2[i + 8].Contains("key_key.clearPoint:key.keyboard.?"))
                    {
                        foundBlock = true;

                        // Reemplazar el bloque con el nuevo texto
                        lines2[i] = "key_key.zoomin:key.keyboard.unknown";
                        lines2[i + 1] = "key_key.centerzoom:key.keyboard.unknown";
                        lines2[i + 2] = "key_key.zoomout:key.keyboard.unknown";
                        lines2[i + 3] = "key_key.rollleft:key.keyboard.unknown";
                        lines2[i + 4] = "key_key.rollcenter:key.keyboard.unknown";
                        lines2[i + 5] = "key_key.rollright:key.keyboard.unknown";
                        lines2[i + 6] = "key_key.point:key.keyboard.unknown";
                        lines2[i + 7] = "key_key.startStop:key.keyboard.unknown";
                        lines2[i + 8] = "key_key.clearPoint:key.keyboard.unknown";

                        // Guardar los cambios en el archivo
                        File.WriteAllLines(filePath, lines2);
                    }

                    // Buscar la línea "key_key.mute_microphone:key.keyboard.m"
                    if (lines2[i].Contains("key_key.mute_microphone:key.keyboard.m"))
                    {
                        foundMuteMicrophone = true;

                        // Reemplazar la línea con el nuevo texto
                        lines2[i] = "key_key.mute_microphone:key.keyboard.unknown";

                        // Guardar los cambios en el archivo
                        File.WriteAllLines(filePath, lines2);
                    }

                    // Buscar la línea "soundCategory_music:1.0"
                    if (lines2[i].Contains("soundCategory_music:1.0"))
                    {
                        foundSoundCategoryMusic = true;

                        // Reemplazar la línea con el nuevo texto
                        lines2[i] = "soundCategory_music:0.8";

                        // Guardar los cambios en el archivo
                        File.WriteAllLines(filePath, lines2);
                    }
                }

                // Si no se encontró el bloque, añadirlo al final del archivo
                if (!foundBlock)
                {
                    string[] newBlock = new string[]
                    {
            "key_key.zoomin:key.keyboard.unknown",
            "key_key.centerzoom:key.keyboard.unknown",
            "key_key.zoomout:key.keyboard.unknown",
            "key_key.rollleft:key.keyboard.unknown",
            "key_key.rollcenter:key.keyboard.unknown",
            "key_key.rollright:key.keyboard.unknown",
            "key_key.point:key.keyboard.unknown",
            "key_key.startStop:key.keyboard.unknown",
            "key_key.clearPoint:key.keyboard.unknown"
                    };

                    // Añadir el nuevo bloque al final del archivo
                    File.AppendAllLines(filePath, newBlock);
                }

                // Si no se encontró la línea "key_key.mute_microphone:key.keyboard.m", añadirla al final del archivo
                if (!foundMuteMicrophone)
                {
                    string newMuteMicrophoneLine = "key_key.mute_microphone:key.keyboard.unknown";

                    // Añadir la nueva línea al final del archivo
                    File.AppendAllText(filePath, newMuteMicrophoneLine + Environment.NewLine);
                }

                // Si no se encontró la línea "soundCategory_music:1.0", no hacer nada
                if (!foundSoundCategoryMusic)
                {
                    // No se encontró la línea, no se realiza ningún cambio adicional
                }

            }
            else
            {
                // Si el archivo ya existe en la ruta de destino, no se hace nada
            }






            // Guardar archivo "installed.txt"
            string installedFilePath = Path.Combine(selectedPath, "installed.txt");
            File.WriteAllText(installedFilePath, "OK");

            //Crear archivo preset.txt si no existe
            string presetPath = Path.Combine(selectedPath, "preset.txt");
            if (!File.Exists(presetPath))
            {
                File.Create(presetPath).Close();
                File.WriteAllText(presetPath, "High");
            }

            //Crear archivo dynamic_lights_reforged.toml si no existe
            string dynamiclightsPath = Path.Combine(selectedPath, "config", "dynamic_lights_reforged.toml");
            if (!File.Exists(dynamiclightsPath))
            {
                File.Create(dynamiclightsPath).Close();
                File.WriteAllText(dynamiclightsPath, "\r\n#Dynamic Lights Settings\r\n[Settings]\r\n\r\n\t[Settings.\"Lighting Settings\"]\r\n\t\t\"Dynamic TileEntity Lighting\" = true\r\n\t\t\"Only Update On Position Change\" = true\r\n\t\t#Allowed Values: OFF, SLOW, FAST, REALTIME\r\n\t\t\"Quality Mode (OFF, SLOW, FAST, REALTIME)\" = \"REALTIME\"\r\n\t\t\"Dynamic Entity Lighting\" = true\r\n\r\n");
            }

            //Crear archivo rubidium_extras.toml si no existe
            string extrasPath = Path.Combine(selectedPath, "config", "rubidium_extras.toml");
            if (!File.Exists(extrasPath))
            {
                File.Create(extrasPath).Close();
                File.WriteAllText(extrasPath, "\r\n#Dynamic Lights Settings\r\n[Settings]\r\n\r\n\t[Settings.Misc]\r\n\t\t\"Cloud Height [Raw, Default 256]\" = 256\r\n\t\t\"Chunk Fade In Quality (OFF, FAST, FANCY)\" = \"FANCY\"\r\n\t\t\"Render Fog\" = true\r\n\t\t\"Hide JEI Until Searching\" = true\r\n\t\t#Allowed Values: WINDOWED, BORDERLESS, FULLSCREEN\r\n\t\t\"Use Borderless Fullscreen\" = \"FULLSCREEN\"\r\n\r\n\t[Settings.\"FPS Counter\"]\r\n\t\t\"Display FPS Counter (OFF, SIMPLE, ADVANCED)\" = \"SIMPLE\"\r\n\t\t\"FPS Counter Distance\" = 12\r\n\r\n\t[Settings.\"Entity Distance\"]\r\n\t\t\"Enable Max Distance Checks\" = false\r\n\t\t\"(TileEntity) Max Horizontal Render Distance [Squared, Default 64^2]\" = 4096\r\n\t\t\"(TileEntity) Max Vertical Render Distance [Raw, Default 32]\" = 32\r\n\t\t\"(Entity) Max Horizontal Render Distance [Squared, Default 64^2]\" = 4096\r\n\t\t\"(Entity) Max Vertical Render Distance [Raw, Default 32]\" = 32\r\n\r\n\t[Settings.Zoom]\r\n\t\t\"Lower Zoom Sensitivity\" = true\r\n\t\t\"Zoom Scrolling Enabled\" = true\r\n\t\t\"Zoom Transition Mode (OFF, LINEAR, SMOOTH)\" = \"SMOOTH\"\r\n\t\t\"Zoom Transition Mode (TOGGLE, HOLD, PERSISTENT)\" = \"HOLD\"\r\n\t\t\"Cinematic Camera Mode (OFF, VANILLA, MULTIPLIED)\" = \"OFF\"\r\n\t\t\"Zoom Overlay?\" = true\r\n\r\n\t[Settings.\"True Darkness\"]\r\n\t\t\"Use True Darkness\" = true\r\n\t\t#Allowed Values: PITCH_BLACK, REALLY_DARK, DARK, DIM\r\n\t\t\"Darkness Setting (PITCH_BLACK, REALLY_DARK, DARK, DIM)\" = \"DARK\"\r\n\r\n\t\t[Settings.\"True Darkness\".Advanced]\r\n\t\t\t\"Only Effect Block Lighting\" = false\r\n\t\t\t\"Ignore Moon Light\" = false\r\n\t\t\t#Range: 0.0 ~ 1.0\r\n\t\t\t\"Minimum Moon Brightness (0->1)\" = 0.0\r\n\t\t\t#Range: 0.0 ~ 1.0\r\n\t\t\t\"Maximum Moon Brightness (0->1)\" = 0.25\r\n\r\n\t\t[Settings.\"True Darkness\".\"Dimension Settings\"]\r\n\t\t\t\"Dark Overworld?\" = true\r\n\t\t\t\"Dark By Default?\" = false\r\n\t\t\t\"Dark Nether?\" = false\r\n\t\t\t#Range: 0.0 ~ 1.0\r\n\t\t\t\"Dark Nether Fog Brightness (0->1)\" = 0.5\r\n\t\t\t\"Dark End?\" = false\r\n\t\t\t#Range: 0.0 ~ 1.0\r\n\t\t\t\"Dark End Fog Brightness (0->1)\" = 0.0\r\n\t\t\t\"Dark If No Skylight?\" = false\r\n\r\n");
            }

            //Crear archivo inventoryhud-client.toml si no existe
            string inventoryhudPath = Path.Combine(selectedPath, "config", "inventoryhud-client.toml");
            if (!File.Exists(inventoryhudPath))
            {
                File.Create(inventoryhudPath).Close();
                File.WriteAllText(inventoryhudPath, "\r\n#Settings for Inventory HUD\r\n[inventoryhud]\r\n\t#Inventory HUD mini mode\r\n\tinvMini = false\r\n\t#Inventory HUD vertical mode\r\n\tinvVert = false\r\n\t#Inventory HUD alpha\r\n\t#Range: 0 ~ 100\r\n\tinvAlpha = 0\r\n\t#Toggle on by default\r\n\tbyDefault = false\r\n\t#Animate recently picked up items\r\n\tanimatedInv = false\r\n\t#Hide background if inventory is empty\r\n\thideBackground = false\r\n\r\n#Settings for ArmorStatus HUD\r\n[armorhud]\r\n\t#Is Armor Damage HUD enabled\r\n\tArmorDamage = true\r\n\t#Hide if durability is above this (in percentage):\r\n\t#Range: 0 ~ 100\r\n\tarmAbove = 100\r\n\t#Show/Hide armor\r\n\tshowArmor = true\r\n\t#Show/Hide armor\r\n\tshowMain = true\r\n\t#Show/Hide armor\r\n\tshowOff = true\r\n\t#Show/Hide armor\r\n\tshowArrows = true\r\n\t#Show/Hide armor\r\n\tshowInv = true\r\n\t#Armor HUD durability view (PERCENTAGE, DAMAGE, DAMAGE LEFT)\r\n\t#Allowed Values: PERCENTAGE, DAMAGE, DAMAGE_LEFT, OFF\r\n\tarmView = \"PERCENTAGE\"\r\n\t#Show item durability bar\r\n\tarmBars = false\r\n\t#Move all items at once or each one\r\n\tmoveAll = true\r\n\t#Show/Hide empty slot icon\r\n\tshowEmpty = true\r\n\t#Show overall count of items in main/off hand\r\n\tshowCount = false\r\n\t#ArmorHUD scale in persentage from 50 to 150\r\n\t#Range: 50 ~ 150\r\n\tarmScale = 100\r\n\r\n#Settings for Potions HUD\r\n[potionshud]\r\n\t#Is Potions HUD enabled\r\n\tPotions = true\r\n\t#Potion HUD alpha\r\n\t#Range: 0 ~ 100\r\n\tpotAlpha = 100\r\n\t#Potion HUD gap\r\n\t#Range: -5 ~ 5\r\n\tpotGap = 0\r\n\t#Potion HUD mini mode\r\n\tpotMini = false\r\n\t#Potion HUD horizontal mode\r\n\tpotHor = false\r\n\t#Full bar duration\r\n\t#Range: > 1\r\n\tbarDuration = 300\r\n\r\n#DONT TOUCH THESE FIELDS!\r\n[positions]\r\n\t#Inventory HUD vertical align\r\n\t#Allowed Values: TOP, CENTER, BOTTOM\r\n\tinvValign = \"BOTTOM\"\r\n\t#Inventory HUD horizontal align\r\n\t#Allowed Values: LEFT, MIDDLE, RIGHT\r\n\tinvHalign = \"MIDDLE\"\r\n\t#Inventory HUD position (X)\r\n\t#Range: -9999 ~ 9999\r\n\txPos = 0\r\n\t#Inventory HUD position (Y)\r\n\t#Range: -9999 ~ 9999\r\n\tyPos = 150\r\n\t#Armor HUD vertical align\r\n\t#Allowed Values: TOP, CENTER, BOTTOM\r\n\tarmValign = \"BOTTOM\"\r\n\t#Armor HUD horizontal align\r\n\t#Allowed Values: LEFT, MIDDLE, RIGHT\r\n\tarmHalign = \"MIDDLE\"\r\n\t#Armor HUD position (X)\r\n\t#Range: -9999 ~ 9999\r\n\txArmPos = 0\r\n\t#Armor HUD position (Y)\r\n\t#Range: -9999 ~ 9999\r\n\tyArmPos = 70\r\n\t#Potion HUD vertical align\r\n\t#Allowed Values: TOP, CENTER, BOTTOM\r\n\tpotValign = \"TOP\"\r\n\t#Potion HUD horizontal align\r\n\t#Allowed Values: LEFT, MIDDLE, RIGHT\r\n\tpotHalign = \"LEFT\"\r\n\t#Potion HUD position (X)\r\n\t#Range: -9999 ~ 9999\r\n\txPotionPos = 20\r\n\t#Potion HUD position (Y)\r\n\t#Range: -9999 ~ 9999\r\n\tyPotionPos = 20\r\n\t#Helmet position (X)\r\n\t#Range: -9999 ~ 9999\r\n\thelmPosX = 103\r\n\t#Helmet position (Y)\r\n\t#Range: -9999 ~ 9999\r\n\thelmPosY = 54\r\n\t#Chestplate position (X)\r\n\t#Range: -9999 ~ 9999\r\n\tchestPosX = 103\r\n\t#Chestplate position (Y)\r\n\t#Range: -9999 ~ 9999\r\n\tchestPosY = 37\r\n\t#Leggings position (X)\r\n\t#Range: -9999 ~ 9999\r\n\tlegPosX = -103\r\n\t#Leggings position (Y)\r\n\t#Range: -9999 ~ 9999\r\n\tlegPosY = 54\r\n\t#Boots position (X)\r\n\t#Range: -9999 ~ 9999\r\n\tbootPosX = -103\r\n\t#Boots position (Y)\r\n\t#Range: -9999 ~ 9999\r\n\tbootPosY = 37\r\n\t#MainHand position (X)\r\n\t#Range: -9999 ~ 9999\r\n\tmainPosX = 103\r\n\t#MainHand position (Y)\r\n\t#Range: -9999 ~ 9999\r\n\tmainPosY = 71\r\n\t#OffHand position (X)\r\n\t#Range: -9999 ~ 9999\r\n\toffPosX = -103\r\n\t#OffHand position (Y)\r\n\t#Range: -9999 ~ 9999\r\n\toffPosY = 71\r\n\t#Arrows position (X)\r\n\t#Range: -9999 ~ 9999\r\n\tarrPosX = 103\r\n\t#Arrows position (Y)\r\n\t#Range: -9999 ~ 9999\r\n\tarrPosY = 20\r\n\t#InvIcon position (X)\r\n\t#Range: -9999 ~ 9999\r\n\tinvPosX = -103\r\n\t#InvIcon position (Y)\r\n\t#Range: -9999 ~ 9999\r\n\tinvPosY = 20\r\n\t#Helmet horizontal align\r\n\t#Allowed Values: LEFT, MIDDLE, RIGHT\r\n\thelmHal = \"MIDDLE\"\r\n\t#Helmet vertical align\r\n\t#Allowed Values: TOP, CENTER, BOTTOM\r\n\thelmVal = \"BOTTOM\"\r\n\t#Chestplate horizontal align\r\n\t#Allowed Values: LEFT, MIDDLE, RIGHT\r\n\tchestHal = \"MIDDLE\"\r\n\t#Chestplate vertical align\r\n\t#Allowed Values: TOP, CENTER, BOTTOM\r\n\tchestVal = \"BOTTOM\"\r\n\t#Leggings horizontal align\r\n\t#Allowed Values: LEFT, MIDDLE, RIGHT\r\n\tlegHal = \"MIDDLE\"\r\n\t#Leggings vertical align\r\n\t#Allowed Values: TOP, CENTER, BOTTOM\r\n\tlegVal = \"BOTTOM\"\r\n\t#Boots horizontal align\r\n\t#Allowed Values: LEFT, MIDDLE, RIGHT\r\n\tbootHal = \"MIDDLE\"\r\n\t#Boots vertical align\r\n\t#Allowed Values: TOP, CENTER, BOTTOM\r\n\tbootVal = \"BOTTOM\"\r\n\t#MainHand horizontal align\r\n\t#Allowed Values: LEFT, MIDDLE, RIGHT\r\n\tmainHal = \"MIDDLE\"\r\n\t#MainHand vertical align\r\n\t#Allowed Values: TOP, CENTER, BOTTOM\r\n\tmainVal = \"BOTTOM\"\r\n\t#OffHand horizontal align\r\n\t#Allowed Values: LEFT, MIDDLE, RIGHT\r\n\toffHal = \"MIDDLE\"\r\n\t#OffHand vertical align\r\n\t#Allowed Values: TOP, CENTER, BOTTOM\r\n\toffVal = \"BOTTOM\"\r\n\t#Arrows horizontal align\r\n\t#Allowed Values: LEFT, MIDDLE, RIGHT\r\n\tarrHal = \"MIDDLE\"\r\n\t#Arrows vertical align\r\n\t#Allowed Values: TOP, CENTER, BOTTOM\r\n\tarrVal = \"BOTTOM\"\r\n\t#InvIcon horizontal align\r\n\t#Allowed Values: LEFT, MIDDLE, RIGHT\r\n\tinvHal = \"MIDDLE\"\r\n\t#InvIcon vertical align\r\n\t#Allowed Values: TOP, CENTER, BOTTOM\r\n\tinvVal = \"BOTTOM\"\r\n\r\n");
            }

            //Crear archivo emotecraft.json si no existe
            string emotecraftPath = Path.Combine(selectedPath, "config", "emotecraft.json");
            if (!File.Exists(emotecraftPath))
            {
                File.Create(emotecraftPath).Close();
                File.WriteAllText(emotecraftPath, "{\n  \"config_version\": 4,\n  \"loadbuiltin\": true,\n  \"quark\": false,\n  \"dark\": false,\n  \"oldChooseWheel\": false,\n  \"perspective\": true,\n  \"default3rdPersonFront\": false,\n  \"showicon\": true,\n  \"enableNSFW\": false,\n  \"debug\": true,\n  \"validate\": false,\n  \"validationThreshold\": 8.0,\n  \"emotesFolderOnLogicalServer\": false,\n  \"emotesDirectory\": \"emotes\",\n  \"autoFixEmoteStop\": true,\n  \"alwaysValidateEmote\": false,\n  \"playersafety\": true,\n  \"stopthreshold\": 2980.9573,\n  \"yratio\": 0.75,\n  \"showHiddenConfig\": false,\n  \"neverRemoveBadIcon\": false,\n  \"exportBuiltin\": false,\n  \"hideWarning\": false,\n  \"fastmenu\": {\n    \"0\": {\n      \"0\": \"01a53a42-2fd2-418c-8e46-e4ee1ff9ee6a\",\n      \"1\": \"3045b335-12ca-4ddb-aca5-0aef450a5e4c\",\n      \"2\": \"931b2bda-a25e-4e55-957a-88c46f5e9a7b\",\n      \"3\": \"f9f6669d-f1b3-4170-8bc5-475a9a600439\",\n      \"4\": \"ebfb1e69-330a-4970-8bca-f5625c90681a\",\n      \"5\": \"96506a5e-a69c-4a18-9add-d43dfd272fa6\",\n      \"6\": \"a2969c42-520f-4c5c-a024-a7d0a2b7b4d1\",\n      \"7\": \"0de63fbe-2f20-44e0-8969-479b76ceeb6b\"\n    }\n  },\n  \"keys\": {}\n}");
            }

            //Crear archivo DistantHorizons.toml si no existe
            string distantPath = Path.Combine(selectedPath, "config", "DistantHorizons.toml");
            if (!File.Exists(distantPath))
            {
                File.Create(distantPath).Close();
                File.WriteAllText(distantPath, "_version = 1\r\n# Show the lod button in the options screen next to fov\r\noptionsButton = true\r\n\r\n[client]\r\n\r\n\t[client.graphics]\r\n\r\n\t\t[client.graphics.quality]\r\n\t\t\t# What is the maximum detail fake chunks should be drawn at? \r\n\t\t\t# This setting will only affect closer chunks.\r\n\t\t\t# Higher settings will increase memory and GPU usage. \r\n\t\t\t#\r\n\t\t\t# CHUNK: render 1 LOD for each Chunk. \r\n\t\t\t# HALF_CHUNK: render 4 LODs for each Chunk. \r\n\t\t\t# FOUR_BLOCKS: render 16 LODs for each Chunk. \r\n\t\t\t# TWO_BLOCKS: render 64 LODs for each Chunk. \r\n\t\t\t# BLOCK: render 256 LODs for each Chunk (width of one block). \r\n\t\t\t#\r\n\t\t\t# Lowest Quality: CHUNK Highest Quality: BLOCK\r\n\t\t\tdrawResolution = \"BLOCK\"\r\n\t\t\t# The radius of the mod's render distance. (measured in chunks) \r\n\t\t\t#\r\n\t\t\tlodChunkRenderDistance = 128\r\n\t\t\t# This indicates how detailed fake chunks will represent \r\n\t\t\t# overhangs, caves, floating islands, ect. \r\n\t\t\t# Higher options will make the world more accurate, but will increase memory and GPU usage. \r\n\t\t\t#\r\n\t\t\t# LOW: uses at max 2 columns per position. \r\n\t\t\t# MEDIUM: uses at max 4 columns per position. \r\n\t\t\t# HIGH: uses at max 8 columns per position. \r\n\t\t\t#\r\n\t\t\t# Lowest Quality: LOW Highest Quality: HIGH\r\n\t\t\tverticalQuality = \"MEDIUM\"\r\n\t\t\t# This indicates how quickly fake chunks decrease in quality the further away they are. \r\n\t\t\t# Higher settings will render higher quality fake chunks farther away, \r\n\t\t\t# but will increase memory and GPU usage.\r\n\t\t\thorizontalScale = 12\r\n\t\t\t# This indicates how quickly fake chunks decrease in quality the further away they are. \r\n\t\t\t# Higher settings will render higher quality fake chunks farther away, \r\n\t\t\t# but will increase memory and GPU usage.\r\n\t\t\thorizontalQuality = \"MEDIUM\"\r\n\t\t\t# This determines how lod level drop off will be done. \r\n\t\t\t#\r\n\t\t\t# SMOOTH_DROPOFF: \r\n\t\t\t#     The lod level is calculated for each point, making the drop off a smooth circle. \r\n\t\t\t# PERFORMANCE_FOCUSED: \r\n\t\t\t#     One detail level for an entire region. Minimize CPU usage and \r\n\t\t\t#     improve terrain refresh delay, especially for high Lod render distance. \r\n\t\t\t# AUTO: \r\n\t\t\t#     Use SMOOTH_DROPOFF for less then 128 Lod render distance, \r\n\t\t\t#     or PERFORMANCE_FOCUSED otherwise. \r\n\t\t\t#\r\n\t\t\tdropoffQuality = \"AUTO\"\r\n\t\t\t# This is the same as vanilla Biome Blending settings for Lod area. \r\n\t\t\t#     Note that anything other than '0' will greatly effect Lod building time \r\n\t\t\t#     and increase triangle count. The cost on chunk generation speed is also \r\n\t\t\t#     quite large if set too high.\r\n\t\t\t#\r\n\t\t\t#     '0' equals to Vanilla Biome Blending of '1x1' or 'OFF', \r\n\t\t\t#     '1' equals to Vanilla Biome Blending of '3x3', \r\n\t\t\t#     '2' equals to Vanilla Biome Blending of '5x5'... \r\n\t\t\t#\r\n\t\t\tlodBiomeBlending = 1\r\n\r\n\t\t[client.graphics.fogQuality]\r\n\t\t\t# At what distance should Fog be drawn on the fake chunks? \r\n\t\t\t#\r\n\t\t\t# This setting shouldn't affect performance.\r\n\t\t\tfogDistance = \"FAR\"\r\n\t\t\t# When should fog be drawn? \r\n\t\t\t#\r\n\t\t\t# USE_OPTIFINE_SETTING: Use whatever Fog setting Optifine is using.\r\n\t\t\t# If Optifine isn't installed this defaults to FOG_ENABLED. \r\n\t\t\t# FOG_ENABLED: Never draw fog on the LODs \r\n\t\t\t# FOG_DISABLED: Always draw fast fog on the LODs \r\n\t\t\t#\r\n\t\t\t# Disabling fog will improve GPU performance.\r\n\t\t\tfogDrawMode = \"FOG_ENABLED\"\r\n\t\t\t# What color should fog use? \r\n\t\t\t#\r\n\t\t\t# USE_WORLD_FOG_COLOR: Use the world's fog color. \r\n\t\t\t# USE_SKY_COLOR: Use the sky's color. \r\n\t\t\t#\r\n\t\t\t# This setting doesn't affect performance.\r\n\t\t\tfogColorMode = \"USE_WORLD_FOG_COLOR\"\r\n\t\t\t# If true disable Minecraft's fog. \r\n\t\t\t#\r\n\t\t\t# Experimental! Mod support is not guarantee.\r\n\t\t\tdisableVanillaFog = true\r\n\r\n\t\t\t[client.graphics.fogQuality.advancedFog]\r\n\t\t\t\t# Where should the far fog start? \r\n\t\t\t\t#\r\n\t\t\t\t#   '0.0': Fog start at player's position.\r\n\t\t\t\t#   '1.0': The fog-start's circle fit just in the lod render distance square.\r\n\t\t\t\t# '1.414': The lod render distance square fit just in the fog-start's circle.\r\n\t\t\t\t#\r\n\t\t\t\tfarFogStart = 0.0\r\n\t\t\t\t# Where should the far fog end? \r\n\t\t\t\t#\r\n\t\t\t\t#   '0.0': Fog end at player's position.\r\n\t\t\t\t#   '1.0': The fog-end's circle fit just in the lod render distance square.\r\n\t\t\t\t# '1.414': The lod render distance square fit just in the fog-end's circle.\r\n\t\t\t\t#\r\n\t\t\t\tfarFogEnd = 1.0\r\n\t\t\t\t# What is the minimum fog thickness? \r\n\t\t\t\t#\r\n\t\t\t\t#   '0.0': No fog at all.\r\n\t\t\t\t#   '1.0': Fully fog color.\r\n\t\t\t\t#\r\n\t\t\t\tfarFogMin = 0.0\r\n\t\t\t\t# What is the maximum fog thickness? \r\n\t\t\t\t#\r\n\t\t\t\t#   '0.0': No fog at all.\r\n\t\t\t\t#   '1.0': Fully fog color.\r\n\t\t\t\t#\r\n\t\t\t\tfarFogMax = 1.0\r\n\t\t\t\t# How the fog thickness should be calculated from distance? \r\n\t\t\t\t#\r\n\t\t\t\t# LINEAR: Linear based on distance (will ignore 'density')\r\n\t\t\t\t# EXPONENTIAL: 1/(e^(distance*density)) \r\n\t\t\t\t# EXPONENTIAL_SQUARED: 1/(e^((distance*density)^2) \r\n\t\t\t\t#\r\n\t\t\t\tfarFogType = \"EXPONENTIAL_SQUARED\"\r\n\t\t\t\t# What is the fog density? \r\n\t\t\t\t#\r\n\t\t\t\tfarFogDensity = 2.5\r\n\r\n\t\t\t\t[client.graphics.fogQuality.advancedFog.heightFog]\r\n\t\t\t\t\t# How the height should effect the fog thickness combined with the normal function? \r\n\t\t\t\t\t#\r\n\t\t\t\t\t# BASIC: No special height fog effect. Fog is calculated based on camera distance \r\n\t\t\t\t\t# IGNORE_HEIGHT: Ignore height completely. Fog is calculated based on horizontal distance \r\n\t\t\t\t\t# ADDITION: heightFog + farFog \r\n\t\t\t\t\t# MAX: max(heightFog, farFog) \r\n\t\t\t\t\t# MULTIPLY: heightFog * farFog \r\n\t\t\t\t\t# INVERSE_MULTIPLY: 1 - (1-heightFog) * (1-farFog) \r\n\t\t\t\t\t# LIMITED_ADDITION: farFog + max(farFog, heightFog) \r\n\t\t\t\t\t# MULTIPLY_ADDITION: farFog + farFog * heightFog \r\n\t\t\t\t\t# INVERSE_MULTIPLY_ADDITION: farFog + 1 - (1-heightFog) * (1-farFog) \r\n\t\t\t\t\t# AVERAGE: farFog*0.5 + heightFog*0.5 \r\n\t\t\t\t\t#\r\n\t\t\t\t\t# Note that for 'BASIC' mode and 'IGNORE_HEIGHT' mode, fog settings for height fog has no effect.\r\n\t\t\t\t\t#\r\n\t\t\t\t\theightFogMixMode = \"BASIC\"\r\n\t\t\t\t\t# Where should the height fog be located? \r\n\t\t\t\t\t#\r\n\t\t\t\t\t# ABOVE_CAMERA: Height fog starts from camera to the sky \r\n\t\t\t\t\t# BELOW_CAMERA: Height fog starts from camera to the void \r\n\t\t\t\t\t# ABOVE_AND_BELOW_CAMERA: Height fog starts from camera to both the sky and the void \r\n\t\t\t\t\t# ABOVE_SET_HEIGHT: Height fog starts from a set height to the sky \r\n\t\t\t\t\t# BELOW_SET_HEIGHT: Height fog starts from a set height to the void \r\n\t\t\t\t\t# ABOVE_AND_BELOW_SET_HEIGHT: Height fog starts from a set height to both the sky and the void \r\n\t\t\t\t\t#\r\n\t\t\t\t\t#\r\n\t\t\t\t\theightFogMode = \"ABOVE_AND_BELOW_CAMERA\"\r\n\t\t\t\t\t# If the height fog is calculated around a set height, what is that height position? \r\n\t\t\t\t\t#\r\n\t\t\t\t\t#\r\n\t\t\t\t\theightFogHeight = 70.0\r\n\t\t\t\t\t# How far the start of height fog should offset? \r\n\t\t\t\t\t#\r\n\t\t\t\t\t#   '0.0': Fog start with no offset.\r\n\t\t\t\t\t#   '1.0': Fog start with offset of the entire world's height. (Include depth)\r\n\t\t\t\t\t#\r\n\t\t\t\t\theightFogStart = 0.0\r\n\t\t\t\t\t# How far the end of height fog should offset? \r\n\t\t\t\t\t#\r\n\t\t\t\t\t#   '0.0': Fog end with no offset.\r\n\t\t\t\t\t#   '1.0': Fog end with offset of the entire world's height. (Include depth)\r\n\t\t\t\t\t#\r\n\t\t\t\t\theightFogEnd = 1.0\r\n\t\t\t\t\t# What is the minimum fog thickness? \r\n\t\t\t\t\t#\r\n\t\t\t\t\t#   '0.0': No fog at all.\r\n\t\t\t\t\t#   '1.0': Fully fog color.\r\n\t\t\t\t\t#\r\n\t\t\t\t\theightFogMin = 0.0\r\n\t\t\t\t\t# What is the maximum fog thickness? \r\n\t\t\t\t\t#\r\n\t\t\t\t\t#   '0.0': No fog at all.\r\n\t\t\t\t\t#   '1.0': Fully fog color.\r\n\t\t\t\t\t#\r\n\t\t\t\t\theightFogMax = 1.0\r\n\t\t\t\t\t# How the fog thickness should be calculated from height? \r\n\t\t\t\t\t#\r\n\t\t\t\t\t# LINEAR: Linear based on height (will ignore 'density')\r\n\t\t\t\t\t# EXPONENTIAL: 1/(e^(height*density)) \r\n\t\t\t\t\t# EXPONENTIAL_SQUARED: 1/(e^((height*density)^2) \r\n\t\t\t\t\t#\r\n\t\t\t\t\theightFogType = \"EXPONENTIAL_SQUARED\"\r\n\t\t\t\t\t# What is the fog density? \r\n\t\t\t\t\t#\r\n\t\t\t\t\theightFogDensity = 2.5\r\n\r\n\t\t[client.graphics.advancedGraphics]\r\n\t\t\t# If false fake chunks behind the player's camera \r\n\t\t\t# aren't drawn, increasing GPU performance. \r\n\t\t\t#\r\n\t\t\t# If true all LODs are drawn, even those behind \r\n\t\t\t# the player's camera, decreasing GPU performance. \r\n\t\t\t#\r\n\t\t\t# Disable this if you see LODs disappearing at the corners of your vision. \r\n\t\t\t#\r\n\t\t\tdisableDirectionalCulling = false\r\n\t\t\t# How often should LODs be drawn on top of regular chunks? \r\n\t\t\t# HALF and ALWAYS will prevent holes in the world, \r\n\t\t\t# but may look odd for transparent blocks or in caves. \r\n\t\t\t#\r\n\t\t\t# NEVER: \r\n\t\t\t#     LODs won't render on top of vanilla chunks. Use Overdraw offset to change the border offset. \r\n\t\t\t# DYNAMIC: \r\n\t\t\t#     LODs will render on top of distant vanilla chunks to hide delayed loading. \r\n\t\t\t#     Will dynamically decide the border offset based on vanilla render distance. \r\n\t\t\t# ALWAYS: \r\n\t\t\t#     LODs will render on all vanilla chunks preventing all holes in the world. \r\n\t\t\t#\r\n\t\t\t# This setting shouldn't affect performance. \r\n\t\t\t#\r\n\t\t\tvanillaOverdraw = \"DYNAMIC\"\r\n\t\t\t# If on Vanilla Overdraw mode of NEVER, how much should should the border be offset? \r\n\t\t\t#\r\n\t\t\t#  '1': The start of lods will be shifted inwards by 1 chunk, causing 1 chunk of overdraw. \r\n\t\t\t# '-1': The start fo lods will be shifted outwards by 1 chunk, causing 1 chunk of gap. \r\n\t\t\t#\r\n\t\t\t# This setting can be used to deal with gaps due to our vanilla rendered chunk \r\n\t\t\t#   detection not being perfect. \r\n\t\t\t#\r\n\t\t\toverdrawOffset = 0\r\n\t\t\t# Will prevent some overdraw issues, but may cause nearby fake chunks to render incorrectly \r\n\t\t\t# especially when in/near an ocean. \r\n\t\t\t#\r\n\t\t\t# This setting shouldn't affect performance. \r\n\t\t\t#\r\n\t\t\tuseExtendedNearClipPlane = true\r\n\t\t\t# How bright fake chunk colors are. \r\n\t\t\t#\r\n\t\t\t# 0 = black \r\n\t\t\t# 1 = normal \r\n\t\t\t# 2 = near white \r\n\t\t\t#\r\n\t\t\tbrightnessMultiplier = 1.0\r\n\t\t\t# How saturated fake chunk colors are. \r\n\t\t\t#\r\n\t\t\t# 0 = black and white \r\n\t\t\t# 1 = normal \r\n\t\t\t# 2 = very saturated \r\n\t\t\t#\r\n\t\t\tsaturationMultiplier = 1.0\r\n\t\t\t# If enabled caves will be culled \r\n\t\t\t#\r\n\t\t\t# NOTE: This feature is under development and \r\n\t\t\t#  it is VERY experimental! Please don't report \r\n\t\t\t# any issues related to this feature. \r\n\t\t\t#\r\n\t\t\t# Additional Info: Currently this cull all faces \r\n\t\t\t#  with skylight value of 0 in dimensions that \r\n\t\t\t#  does not have a ceiling. \r\n\t\t\t#\r\n\t\t\tenableCaveCulling = true\r\n\t\t\t# At what Y value should cave culling start? \r\n\t\t\t#\r\n\t\t\tcaveCullingHeight = 40\r\n\t\t\t# This is the earth size ratio when applying the curvature shader effect. \r\n\t\t\t#\r\n\t\t\t# NOTE: This feature is just for fun and is VERY experimental! \r\n\t\t\t#Please don't report any issues related to this feature. \r\n\t\t\t#\r\n\t\t\t# 0 = flat/disabled \r\n\t\t\t# 1 = 1 to 1 (6,371,000 blocks) \r\n\t\t\t# 100 = 1 to 100 (63,710 blocks) \r\n\t\t\t# 10000 = 1 to 10000 (637.1 blocks) \r\n\t\t\t#\r\n\t\t\t# NOTE: Due to current limitations, the min value is 50 \r\n\t\t\t# and the max value is 5000. Any values outside this range \r\n\t\t\t# will be set to 0(disabled).\r\n\t\t\tearthCurveRatio = 0\r\n\r\n\t[client.worldGenerator]\r\n\t\t# Whether to enable Distant chunks generator? \r\n\t\t#\r\n\t\t# Turning this on allows Distant Horizons to make lods for chunks \r\n\t\t# that are outside of vanilla view distance. \r\n\t\t#\r\n\t\t# Note that in server, distant generation is always off. \r\n\t\t#\r\n\t\tenableDistantGeneration = true\r\n\t\tdistanceGenerationMode = \"FEATURES\"\r\n\t\t# How should block and sky lights be processed for distant generation? \r\n\t\t#\r\n\t\t# Note that this include already existing chunks since vanilla \r\n\t\t# does not store sky light values to save file. \r\n\t\t#\r\n\t\t# FAST: Use height map to fake the light values. \r\n\t\t# FANCY: Use actaul light engines to generate proper values. \r\n\t\t#\r\n\t\t# This will effect generation speed, but not the rendering performance.\r\n\t\tlightGenerationMode = \"FANCY\"\r\n\t\t# In what priority should fake chunks be generated outside the vanilla render distance? \r\n\t\t#\r\n\t\t# FAR_FIRST \r\n\t\t# Fake chunks are generated from lowest to highest detail \r\n\t\t# with a priority for far away regions. \r\n\t\t# This fills in the world fastest, but you will have large low detail \r\n\t\t# blocks for a while while the generation happens. \r\n\t\t#\r\n\t\t# NEAR_FIRST \r\n\t\t# Fake chunks are generated around the player \r\n\t\t# in a spiral, similar to vanilla minecraft. \r\n\t\t# Best used when on a server since we can't generate \r\n\t\t# fake chunks. \r\n\t\t#\r\n\t\t# BALANCED \r\n\t\t# A mix between NEAR_FIRSTandFAR_FIRST. \r\n\t\t# First prioritise completing nearby highest detail chunks, \r\n\t\t# then focus on filling in the low detail areas away from the player. \r\n\t\t#\r\n\t\t# AUTO \r\n\t\t# Uses BALANCED when on a single player world \r\n\t\t# and NEAR_FIRST when connected to a server. \r\n\t\t#\r\n\t\t# This shouldn't affect performance.\r\n\t\tgenerationPriority = \"NEAR_FIRST\"\r\n\t\t# When generating fake chunks, what blocks should be ignored? \r\n\t\t# Ignored blocks don't affect the height of the fake chunk, but might affect the color. \r\n\t\t# So using BOTH will prevent snow covered blocks from appearing one block too tall, \r\n\t\t# but will still show the snow's color.\r\n\t\t#\r\n\t\t# NONE: Use all blocks when generating fake chunks \r\n\t\t# NON_FULL: Only use full blocks when generating fake chunks (ignores slabs, lanterns, torches, tall grass, etc.) \r\n\t\t# NO_COLLISION: Only use solid blocks when generating fake chunks (ignores tall grass, torches, etc.) \r\n\t\t# BOTH: Only use full solid blocks when generating fake chunks \r\n\t\t#\r\n\t\t# This wont't affect performance.\r\n\t\tblocksToAvoid = \"BOTH\"\r\n\r\n\t[client.multiplayer]\r\n\t\t# What multiplayer save folders should be named. \r\n\t\t#\r\n\t\t# AUTO: NAME_IP for LAN connections, NAME_IP_PORT for all others. \r\n\t\t# NAME_ONLY: Example: \"Minecraft Server\" \r\n\t\t# NAME_IP: Example: \"Minecraft Server IP 192.168.1.40\" \r\n\t\t# NAME_IP_PORT: Example: \"Minecraft Server IP 192.168.1.40:25565\" \r\n\t\t#\r\n\t\t#\r\n\t\tserverFolderNameMode = \"AUTO\"\r\n\t\t# When matching worlds of the same dimension type the \r\n\t\t# tested chunks must be at least this percent the same \r\n\t\t# in order to be considered the same world. \r\n\t\t#\r\n\t\t# Note: If you use portals to enter a dimension at two \r\n\t\t# different locations this system may think it is two different worlds. \r\n\t\t#\r\n\t\t# 1.0 (100%) the chunks must be identical. \r\n\t\t# 0.5 (50%)  the chunks must be half the same. \r\n\t\t# 0.0 (0%)   disables multi-dimension support, \r\n\t\t#            only one world will be used per dimension. \r\n\t\t#\r\n\t\t#\r\n\t\tmultiDimensionRequiredSimilarity = 0.0\r\n\r\n\t[client.advanced]\r\n\t\t# Due to some demand for playing without vanilla terrains, \r\n\t\t# we decided to add this mode for fun. \r\n\t\t#\r\n\t\t# NOTE: Do not report any issues when this mode is on! \r\n\t\t#   Again, this setting is only for fun, and mod \r\n\t\t#   compatibility is not guaranteed. \r\n\t\t#\r\n\t\t#\r\n\t\tlodOnlyMode = false\r\n\r\n\t\t[client.advanced.threading]\r\n\t\t\t# How many threads should be used when generating fake \r\n\t\t\t# chunks outside the normal render distance? \r\n\t\t\t#\r\n\t\t\t# If it's less than 1, it will be treated as a percentage \r\n\t\t\t# of time single thread can run before going to idle. \r\n\t\t\t#\r\n\t\t\t# If you experience stuttering when generating distant LODs, \r\n\t\t\t# decrease  this number. If you want to increase LOD \r\n\t\t\t# generation speed, increase this number. \r\n\t\t\t#\r\n\t\t\t# This and the number of buffer builder threads are independent, \r\n\t\t\t# so if they add up to more threads than your CPU has cores, \r\n\t\t\t# that shouldn't cause an issue. \r\n\t\t\t#\r\n\t\t\tnumberOfWorldGenerationThreads = 4.0\r\n\t\t\t# How many threads are used when building vertex buffers? \r\n\t\t\t# (The things sent to your GPU to draw the fake chunks). \r\n\t\t\t#\r\n\t\t\t# If you experience high CPU usage when NOT generating distant \r\n\t\t\t# fake chunks, lower this number. A higher number will make fake\r\n\t\t\t# fake chunks' transition faster when moving around the world. \r\n\t\t\t#\r\n\t\t\t# This and the number of world generator threads are independent, \r\n\t\t\t# so if they add up to more threads than your CPU has cores, \r\n\t\t\t# that shouldn't cause an issue. \r\n\t\t\t#\r\n\t\t\t# The maximum value is the number of logical processors on your CPU. \r\n\t\t\t#\r\n\t\t\tnumberOfBufferBuilderThreads = 2\r\n\r\n\t\t[client.advanced.debugging]\r\n\t\t\t# What renderer is active? \r\n\t\t\t#\r\n\t\t\t# DEFAULT: Default lod renderer \r\n\t\t\t# DEBUG: Debug testing renderer \r\n\t\t\t# DISABLED: Disable rendering \r\n\t\t\t#\r\n\t\t\trendererType = \"DEFAULT\"\r\n\t\t\t# Should specialized colors/rendering modes be used? \r\n\t\t\t#\r\n\t\t\t# OFF: Fake chunks will be drawn with their normal colors. \r\n\t\t\t# SHOW_WIREFRAME: Fake chunks will be drawn as wireframes. \r\n\t\t\t# SHOW_DETAIL: Fake chunks color will be based on their detail level. \r\n\t\t\t# SHOW_DETAIL_WIREFRAME: Fake chunks color will be based on their detail level, drawn as a wireframe. \r\n\t\t\t# SHOW_GENMODE: Fake chunks color will be based on their distant generation mode. \r\n\t\t\t# SHOW_GENMODE_WIREFRAME: Fake chunks color will be based on their distant generation mode, drawn as a wireframe. \r\n\t\t\t# SHOW_OVERLAPPING_QUADS: Fake chunks will be drawn with total white, but overlapping quads will be drawn with red. \r\n\t\t\t# SHOW_OVERLAPPING_QUADS_WIREFRAME: Fake chunks will be drawn with total white, \r\n\t\t\t# but overlapping quads will be drawn with red, drawn as a wireframe. \r\n\t\t\t#\r\n\t\t\tdebugMode = \"OFF\"\r\n\t\t\t# If true the F8 key can be used to cycle through the different debug modes. \r\n\t\t\t# and the F6 key can be used to enable and disable LOD rendering.\r\n\t\t\tenableDebugKeybindings = false\r\n\r\n\t\t\t[client.advanced.debugging.debugSwitch]\r\n\t\t\t\t# If enabled, the mod will log information about the world generation process. \r\n\t\t\t\t# This can be useful for debugging. \r\n\t\t\t\t#\r\n\t\t\t\tlogWorldGenEvent = \"LOG_WARNING_TO_CHAT_AND_INFO_TO_FILE\"\r\n\t\t\t\t# If enabled, the mod will log performance about the world generation process. \r\n\t\t\t\t# This can be useful for debugging. \r\n\t\t\t\t#\r\n\t\t\t\tlogWorldGenPerformance = \"LOG_WARNING_TO_CHAT_AND_FILE\"\r\n\t\t\t\t# If enabled, the mod will log information about the world generation process. \r\n\t\t\t\t# This can be useful for debugging. \r\n\t\t\t\t#\r\n\t\t\t\tlogWorldGenLoadEvent = \"LOG_WARNING_TO_CHAT_AND_FILE\"\r\n\t\t\t\t# If enabled, the mod will log information about the LOD generation process. \r\n\t\t\t\t# This can be useful for debugging. \r\n\t\t\t\t#\r\n\t\t\t\tlogLodBuilderEvent = \"LOG_WARNING_TO_CHAT_AND_INFO_TO_FILE\"\r\n\t\t\t\t# If enabled, the mod will log information about the renderer buffer process. \r\n\t\t\t\t# This can be useful for debugging. \r\n\t\t\t\t#\r\n\t\t\t\tlogRendererBufferEvent = \"LOG_WARNING_TO_CHAT_AND_INFO_TO_FILE\"\r\n\t\t\t\t# If enabled, the mod will log information about the renderer OpenGL process. \r\n\t\t\t\t# This can be useful for debugging. \r\n\t\t\t\t#\r\n\t\t\t\tlogRendererGLEvent = \"LOG_WARNING_TO_CHAT_AND_INFO_TO_FILE\"\r\n\t\t\t\t# If enabled, the mod will log information about file read/write operations. \r\n\t\t\t\t# This can be useful for debugging. \r\n\t\t\t\t#\r\n\t\t\t\tlogFileReadWriteEvent = \"LOG_WARNING_TO_CHAT_AND_INFO_TO_FILE\"\r\n\t\t\t\t# If enabled, the mod will log information about file sub-dimension operations. \r\n\t\t\t\t# This can be useful for debugging. \r\n\t\t\t\t#\r\n\t\t\t\tlogFileSubDimEvent = \"LOG_WARNING_TO_CHAT_AND_INFO_TO_FILE\"\r\n\t\t\t\t# If enabled, the mod will log information about network operations. \r\n\t\t\t\t# This can be useful for debugging. \r\n\t\t\t\t#\r\n\t\t\t\tlogNetworkEvent = \"LOG_WARNING_TO_CHAT_AND_INFO_TO_FILE\"\r\n\r\n\t\t[client.advanced.buffers]\r\n\t\t\t# What method should be used to upload geometry to the GPU? \r\n\t\t\t#\r\n\t\t\t# AUTO: Picks the best option based on the GPU you have. \r\n\t\t\t# BUFFER_STORAGE: Default for NVIDIA if OpenGL 4.5 is supported. \r\n\t\t\t#                 Fast rendering, no stuttering. \r\n\t\t\t# SUB_DATA: Backup option for NVIDIA. \r\n\t\t\t#           Fast rendering but may stutter when uploading. \r\n\t\t\t# BUFFER_MAPPING: Slow rendering but won't stutter when uploading. Possibly the best option for integrated GPUs. \r\n\t\t\t#                Default option for AMD/Intel. \r\n\t\t\t#                May end up storing buffers in System memory. \r\n\t\t\t#                Fast rendering if in GPU memory, slow if in system memory, \r\n\t\t\t#                but won't stutter when uploading.  \r\n\t\t\t# DATA: Fast rendering but will stutter when uploading. \r\n\t\t\t#       Backup option for AMD/Intel. \r\n\t\t\t#       Fast rendering but may stutter when uploading. \r\n\t\t\t#\r\n\t\t\t# If you don't see any difference when changing these settings, or the world looks corrupted: \r\n\t\t\t# Restart the game to clear the old buffers. \r\n\t\t\t#\r\n\t\t\tgpuUploadMethod = \"AUTO\"\r\n\t\t\t# How long should a buffer wait per Megabyte of data uploaded?\r\n\t\t\t# Helpful resource for frame times: https://fpstoms.com \r\n\t\t\t#\r\n\t\t\t# Longer times may reduce stuttering but will make fake chunks \r\n\t\t\t# transition and load slower. Change this to [0] for no timeout.\r\n\t\t\t#\r\n\t\t\t# NOTE:\r\n\t\t\t# Before changing this config, try changing \"GPU Upload methods\"\r\n\t\t\t#  and determined the best method for your hardware first. \r\n\t\t\t#\r\n\t\t\tgpuUploadPerMegabyteInMilliseconds = 0\r\n\t\t\t# How frequently should vertex buffers (geometry) be rebuilt and sent to the GPU? \r\n\t\t\t# Higher settings may cause stuttering, but will prevent holes in the world \r\n\t\t\t#\r\n\t\t\trebuildTimes = \"NORMAL\"\r\n\r\n");
            }

            // Eliminar el archivo modpack.zip
            string fileToDelete = Path.Combine(selectedPath, "modpack.zip");
            File.Delete(fileToDelete);

            // Eliminar el archivo librerias.zip
            string fileToDelete2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "librerias.zip");
            File.Delete(fileToDelete2);

            ////////////////////////////////////////
            //Crear perfil en launcher_profiles.json
            ////////////////////////////////////////

            // Determinar la memoria RAM total del sistema
            double totalMemoryGB = GetTotalMemoryInGB();

            // Determinar los argumentos de Java en función de la RAM total
            string javaArgs = DetermineJavaArgs(totalMemoryGB);

            // Continuar con la creación o modificación del perfil
            string perfilesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "launcher_profiles.json");

            // Leer el contenido existente del archivo JSON
            var json = File.ReadAllText(perfilesPath);
            var jsonObj = JObject.Parse(json);

            // Crear el nuevo objeto JSON para "InfernoLand"
            var newProfile = new JObject
            {
                ["created"] = "2024-03-08T20:18:06.251Z",
                ["gameDir"] = @"C:\Users\krist\AppData\Roaming\.minecraft\_infernoland",
                ["icon"] = "Redstone_Block",
                ["javaArgs"] = javaArgs,
                ["lastUsed"] = "2024-03-08T20:31:10.859Z",
                ["lastVersionId"] = "InfernoLand",
                ["name"] = "InfernoLand",
                ["type"] = "custom"
            };

            // Los perfiles están en una propiedad "profiles" en el JSON
            if (jsonObj["profiles"] is JObject profiles) // Usar 'is' para intentar el cast y verificar por null
            {
                profiles["InfernoLand"] = newProfile; // Esto agregará el perfil "InfernoLand" justo debajo del último perfil existente
            }
            else
            {
                // Si "profiles" no es un JObject o es null se inicializa 'profiles' y se agrega a 'jsonObj'
                profiles = new JObject();
                profiles["InfernoLand"] = newProfile;
                jsonObj["profiles"] = profiles; // Asegúrate de que 'jsonObj' contenga una propiedad "profiles"
            }

            // Escribir el JSON modificado de vuelta en el archivo
            File.WriteAllText(perfilesPath, jsonObj.ToString(Formatting.Indented));

            static double GetTotalMemoryInGB()
            {
                double totalMemory = 0;
                var query = new ObjectQuery("SELECT * FROM Win32_ComputerSystem");
                using (var searcher = new ManagementObjectSearcher(query))
                {
                    foreach (ManagementObject mo in searcher.Get())
                    {
                        totalMemory = Convert.ToDouble(mo["TotalPhysicalMemory"]);
                    }
                }
                return totalMemory / (1024 * 1024 * 1024); // Convertir de bytes a GB
            }

            static string DetermineJavaArgs(double totalMemoryGB)
            {
                int javaMemory = totalMemoryGB switch
                {
                    <= 8 => 4,      // Si la RAM es 8GB o menos, asigna 4
                    <= 12 => 6,     // Si la RAM es más de 8GB y hasta 12GB, asigna 6
                    < 32 => 10,     // Si la RAM es más de 12GB y menos de 32GB, asigna 10
                    _ => 12         // Para 32GB o más, asigna 12
                };

                return $"-Xmx{javaMemory}G -XX:+UnlockExperimentalVMOptions -XX:+UseG1GC -XX:G1NewSizePercent=20 -XX:G1ReservePercent=20 -XX:MaxGCPauseMillis=50 -XX:G1HeapRegionSize=32M";
            }


            //////////////////////////////////
            // Reiniciar la barra de progreso
            /////////////////////////////////
            InitializeProgressBar();

            Cursor = Cursors.Default;
            button2.Text = "¡Listo! Modpack instalado correctamente!";
            button1.Text = "Finalizado";
            MessageBox.Show("La instalación del modpack oficial de InfernoLand ha finalizado. Ahora ejecute su Launcher y seleccione el perfil InfernoLand", "Instalación Completada", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Remover los controles TextBox y enlace del formulario
            Controls.Remove(logTextBox);
            Controls.Remove(linkMoreDetails);


            // Abrir form Ajustes y pasar los valores actuales
            Ajustes form2 = new Ajustes(selectedPath, ajustesCheckBoxValue, ajustesTrackBar1Value, ajustesTrackBar2Value);
            form2.ShowDialog();
        }


        private void SaveProgramVersion()
        {
            try
            {
                // Guardar versión del programa
                string programFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "version.txt");
                File.WriteAllText(programFilePath, program_version);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("No se puede guardar la versión actual del programa.", "Error de acceso", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveModpackVersion()
        {
            try
            {
                // Guardar versión del modpack
                string installedFilePath = Path.Combine(selectedPath, "modpack_version.txt");
                File.WriteAllText(installedFilePath, modpack_version);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("No se puede guardar la versión actual del programa.", "Error de acceso", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_MouseEnter(object sender, EventArgs e)
        {
            button2.BackgroundImage = Properties.Resources.widgetss1;
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            button2.BackgroundImage = Properties.Resources.widgets1;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Selecciona una instancia para el modpack";
            dialog.RootFolder = Environment.SpecialFolder.ApplicationData;
            dialog.ShowNewFolderButton = false;

            // Establecer la ubicación inicial del explorador de archivos
            dialog.SelectedPath = selectedPath; // Reemplaza "selectedPath" con la ubicación deseada

            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                selectedPath = dialog.SelectedPath;
                button2.Text = selectedPath;
            }
        }

        private void pictureBox5_Click(object? sender, EventArgs e)
        {
            selectedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "_infernoland");
            button2.Text = selectedPath;
            SaveCustomPath();
        }
        private void PictureBox5_MouseEnter(object? sender, EventArgs e)
        {
            pictureBox5.Image = Properties.Resources.restart_b;
        }
        private void PictureBox5_MouseLeave(object? sender, EventArgs e)
        {
            pictureBox5.Image = Properties.Resources.restart;
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            string installedFilePath = Path.Combine(selectedPath, "installed.txt");

            if (File.Exists(installedFilePath) && File.ReadAllText(installedFilePath).Trim() == "OK")
            {
                // Abrir form Ajustes y pasar los valores actuales
                Ajustes form2 = new Ajustes(selectedPath, ajustesCheckBoxValue, ajustesTrackBar1Value, ajustesTrackBar2Value);
                form2.ShowDialog();
            }
            else
            {
                MessageBox.Show("El modpack no está instalado correctamente. Por favor, realice la instalación antes de acceder a los ajustes.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pictureBox6_MouseEnter(object sender, EventArgs e)
        {
            // Cambiar el tamaño de la imagen al entrar el cursor
            pictureBox6.Size = new Size(pictureBox6.Width + 10, pictureBox6.Height + 10);
        }

        private void pictureBox6_MouseLeave(object sender, EventArgs e)
        {
            // Restaurar el tamaño original de la imagen al salir el cursor
            pictureBox6.Size = new Size(pictureBox6.Width - 10, pictureBox6.Height - 10);
        }


        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Esto eliminará el modpack incluido todas sus configuraciones. ¿Seguro que quiere continuar?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                Cursor = Cursors.WaitCursor;
                button2.Text = "Desinstalando Modpack...";
                // Borrar carpeta del modpack de la ruta elegida por el usuario
                if (Directory.Exists(selectedPath))
                {
                    // Elimina la carpeta y todos sus contenidos
                    Directory.Delete(selectedPath, true);
                }

                // Leer el contenido existente del archivo JSON
                string perfilesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "launcher_profiles.json");
                var json = File.ReadAllText(perfilesPath);
                var jsonObj = JObject.Parse(json);

                // Los perfiles están en una propiedad "profiles" en el JSON
                if (jsonObj["profiles"] is JObject profiles)
                {
                    // Verificar si el perfil "InfernoLand" existe antes de intentar eliminarlo
                    if (profiles.ContainsKey("InfernoLand"))
                    {
                        // Eliminar el perfil "InfernoLand"
                        profiles.Remove("InfernoLand");

                        // Escribir el JSON modificado de vuelta en el archivo
                        File.WriteAllText(perfilesPath, jsonObj.ToString(Formatting.Indented));
                    }
                }

                // Construir la ruta hacia la carpeta "InfernoLand" dentro de "versions"
                string infernoLandPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "versions", "InfernoLand");

                // Verificar si la carpeta existe antes de intentar eliminarla
                if (Directory.Exists(infernoLandPath))
                {
                    // Eliminar la carpeta "InfernoLand" y todo su contenido
                    Directory.Delete(infernoLandPath, true);
                }

                button2.Text = "¡Modpack Desinstalado correctamente!";
                button3.Text = "DESINSTALADO";
                MessageBox.Show("Desinstalación completada con éxito", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Cursor = Cursors.Default;
            }
            else if (result == DialogResult.No)
            {
                // El usuario hizo clic en Rechazar, se cancela el proceso
                MessageBox.Show("Proceso cancelado satisfactoriamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }



        private void button3_MouseEnter(object? sender, EventArgs e)
        {
            button3.BackgroundImage = Properties.Resources.half_widgetss4;
        }
        private void button3_MouseLeave(object? sender, EventArgs e)
        {
            button3.BackgroundImage = Properties.Resources.half_widgets4;
        }

    }
}