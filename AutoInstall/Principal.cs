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
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Windows.Forms.Design.AxImporter;

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
        private string program_pastebinUrl = "https://gist.github.com/Kristiansito/9c40f03bcbcce524e2fac36503524e9b/raw"; // URL de pastebin del programa
        private string programUpdateUrl = ""; // URL de descarga de actualización
        private string newProgramVersion = "";

        //Modpack version
        private string modpack_version = ""; // versión actual del modpack
        private string modpack_pastebinUrl = "https://gist.github.com/Kristiansito/5d113c31c97f6a090c9c8f9f97ade2cf/raw"; // URL de pastebin del modpack
        private string modpackUpdateUrl = ""; // URL de descarga de actualización
        private string newModpackVersion = "";

        //Forge libraries + AncientKraft version
        private string libraries_Url = "";

        //Distant Horizons (Optional)
        private string distantHorizons_url = "";

        private string selectedPath = ""; // Variable para almacenar la ruta seleccionada por el usuario
        private System.Windows.Forms.Timer animationTimer;
        private int animationIndex;
        private readonly string[] animationSequence = { "Descargando", "Descargando.", "Descargando..", "Descargando..." };


        public Principal()
        {
            InitializeComponent();
            CheckMinecraftIsInstalled();
            LoadCustomPath();
            // Obtener la versión actual del título de la ventana principal
            program_version = ObtenerVersion(this.Text);
            SaveProgramVersion();
            CheckModpackIsInstalled();
            LoadModpackVersion();
            CheckForProgramUpdates();

            InitializeProgressBar();
            animationTimer = new System.Windows.Forms.Timer();

            FixDistant();


            // Crear el objeto ToolTip
            System.Windows.Forms.ToolTip toolTip = new System.Windows.Forms.ToolTip();
            toolTip.AutoPopDelay = 5000; // Tiempo en milisegundos que muestra el ToolTip
            toolTip.InitialDelay = 250; // Retardo inicial antes de mostrar el ToolTip
            toolTip.ReshowDelay = 100; // Retardo antes de mostrar nuevamente el ToolTip si el cursor se mueve sobre el control

            // Asociar el ToolTip al control PictureBox5
            toolTip.SetToolTip(pictureBox5, "Restablecer ruta...");


            // Asociar el ToolTip al control PictureBox6
            toolTip.SetToolTip(pictureBox6, "Ajustes...");

            LoadUrlsFromGist().ConfigureAwait(false);
        }

        private async Task LoadUrlsFromGist()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // URL of the Gist containing the URLs
                    string gistUrl = "https://gist.github.com/Kristiansito/2a21453761d7e2940de7883645fd15ef/raw";

                    // Download the gist content as a plain text
                    string gistContent = await client.GetStringAsync(gistUrl);

                    // Split the content by new lines
                    var urls = gistContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                    // Ensure there are enough lines to get the URLs
                    if (urls.Length >= 2)
                    {
                        libraries_Url = urls[0];
                        distantHorizons_url = urls[1];
                    }
                    else
                    {
                        // Handle the case where not enough URLs are provided
                        libraries_Url = "default_libraries_url";
                        distantHorizons_url = "default_distantHorizons_url";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading URLs from Gist:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        // Función para detectar si Minecraft está instalado
        private string CheckMinecraftIsInstalled()
        {
            // Obtiene la ruta de la carpeta .minecraft
            string minecraftPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft");

            // Comprueba si la carpeta .minecraft existe
            if (!Directory.Exists(minecraftPath))
            {
                // Muestra un MessageBox de error al usuario si la carpeta no existe
                MessageBox.Show(
                    "No se ha detectado la carpeta .minecraft. Por favor, instale Minecraft o inícielo al menos una vez.",
                    "Error: Minecraft No Encontrado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                // Cierra la aplicación después de que el usuario haga clic en "OK"
                Application.Exit();

                // Retorna una cadena vacía, aunque el proceso se cerrará antes de usarla
                return string.Empty;
            }

            // Si la carpeta existe, retorna la ruta
            return minecraftPath;
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

        private void CheckModpackIsInstalled()
        {
            // Especifica la ruta del archivo instalado.txt
            string installedFilePath = Path.Combine(selectedPath, "installed.txt");

            // Comprueba si el archivo existe
            if (File.Exists(installedFilePath))
            {
                // Lee el contenido del archivo
                string content = File.ReadAllText(installedFilePath);

                // Comprueba si el contenido es "OK"
                if (content.Trim() == "OK")
                {
                    // Cambia el texto del botón
                    button1.Text = "REINSTALAR";
                }
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

        private void FixDistant()
        {
            // Nombre del directorio a buscar
            string oldDirectoryName = "play%2Eluminakraft%2Ecom25983";
            string oldDirectoryPath = Path.Combine(selectedPath, oldDirectoryName);

            // Nombre del nuevo directorio
            string newDirectoryName = "Distant_Horizons_server_data";
            string newDirectoryPath = Path.Combine(selectedPath, newDirectoryName);

            // Verifica si el directorio viejo existe
            if (Directory.Exists(oldDirectoryPath))
            {
                // Si el directorio nuevo ya existe, eliminarlo antes de mover
                if (Directory.Exists(newDirectoryPath))
                {
                    Directory.Delete(newDirectoryPath, true); // Elimina el directorio y su contenido
                    Console.WriteLine($"Directorio eliminado: {newDirectoryPath}");
                }

                // Crea el directorio nuevo
                Directory.CreateDirectory(newDirectoryPath);
                Console.WriteLine($"Directorio creado: {newDirectoryPath}");

                // Mueve el directorio viejo al nuevo directorio
                string newOldDirectoryPath = Path.Combine(newDirectoryPath, oldDirectoryName);
                Directory.Move(oldDirectoryPath, newOldDirectoryPath);
                Console.WriteLine($"Directorio movido a: {newOldDirectoryPath}");
            }
            else
            {
                Console.WriteLine($"El directorio viejo no existe: {oldDirectoryPath}");
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
                    string newProgramFileName = $"Asistente de AncientKraft v{newProgramVersion}.exe";

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
            string url = "https://discord.gg/UJZRrcUFMj";
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
                selectedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "_AncientKraft");
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

            // Iniciar animación Descargando...
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
            string[] foldersToClean = { "mods", "config/fancymenu", };

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
            logTextBox.BringToFront(); // Asegurar que el logTextBox esté en la capa superior

            // Crear Label para mostrar la velocidad de descarga
            Label speedLabel = new Label();
            speedLabel.AutoSize = true;
            speedLabel.ForeColor = Color.White; // Color del texto
            speedLabel.BackColor = Color.Transparent; // Sin fondo
            Controls.Add(speedLabel);

            // Crear Label para mostrar el tiempo restante
            Label timeLabel = new Label();
            timeLabel.AutoSize = true;
            timeLabel.ForeColor = Color.White; // Color del texto
            timeLabel.BackColor = Color.Transparent; // Sin fondo
            Controls.Add(timeLabel);

            // Crear carpeta _AncientKraft en caso de que la ruta sea por defecto
            if (selectedPath == Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "_AncientKraft"))
            {
                // Verificar si la carpeta _AncientKraft ya existe
                if (!Directory.Exists(selectedPath))
                {
                    // Si no existe, crear la carpeta
                    Directory.CreateDirectory(selectedPath);
                }
            }

            // Leer la decisión del usuario desde el archivo distant.txt
            bool downloadDistantHorizons = false;
            string distantFilePath = Path.Combine(selectedPath, "distant.txt");

            if (File.Exists(distantFilePath))
            {
                string userDecision = File.ReadAllText(distantFilePath).Trim();
                // Si distant.txt contiene "true", NO queremos descargar
                downloadDistantHorizons = !userDecision.Equals("true", StringComparison.OrdinalIgnoreCase);
            }

            // Si el archivo no existe o contiene "false", mostrar el MessageBox
            if (!File.Exists(distantFilePath) || downloadDistantHorizons)
            {
                DialogResult result = MessageBox.Show(
                    "¿Desea descargar Distant Horizons?\n\n" +
                    "Esta opción mejorará su experiencia permitiéndole ver más lejos en el juego.\n" +
                    "Tenga en cuenta que descargará el mapa completo, lo que puede requerir más de 50 GB y podría tardar un tiempo considerable.\n\n" +
                    "Nota: Esta descarga no es recomendable para PCs con bajos recursos.",
                    "Descargar Distant Horizons (recomendado)",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                downloadDistantHorizons = result == DialogResult.Yes;
            }

            DateTime startTime; // Declarar startTime aquí
            DateTime lastUpdateTime = DateTime.Now; // Tiempo de la última actualización

            button2.Text = "Descargando librerías...";

            // Descargar archivo zip librerías de Dropbox
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
                    startTime = DateTime.Now; // Tiempo de inicio

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

                            // Actualizar cada segundo
                            if (DateTime.Now - lastUpdateTime > TimeSpan.FromSeconds(2))
                            {
                                TimeSpan elapsedTime = DateTime.Now - startTime;
                                double bytesPerSecond = downloadedBytes / elapsedTime.TotalSeconds;
                                double bytesPerSecondInMB = bytesPerSecond / (1024 * 1024); // Convertir a MB/s
                                double estimatedTimeRemaining = (totalBytes - downloadedBytes) / bytesPerSecond;

                                // Actualizar el Label con el tiempo restante
                                timeLabel.Text = $"Tiempo restante: {TimeSpan.FromSeconds(estimatedTimeRemaining):hh\\:mm\\:ss}";

                                // Actualizar el Label con la velocidad
                                speedLabel.Text = $"{bytesPerSecondInMB:F2} MB/s";

                                // Actualizar la ubicación de los Labels
                                timeLabel.Location = new Point(progressBarUI.Left - timeLabel.Width - 10, progressBarUI.Top + progressBarUI.Height / 2 - timeLabel.Height / 2 - speedLabel.Height - 5);
                                speedLabel.Location = new Point(progressBarUI.Left - speedLabel.Width - 10, progressBarUI.Top + progressBarUI.Height / 2 - speedLabel.Height / 2);

                                lastUpdateTime = DateTime.Now; // Actualizar el tiempo de la última actualización
                            }

                            // Registrar el progreso en el TextBox (si está visible)
                            if (logTextBox.Visible)
                            {
                                string message = $"Descargando: {downloadedBytes} / {totalBytes} bytes ({speedLabel.Text}, {timeLabel.Text})";
                                Invoke((MethodInvoker)(() =>
                                {
                                    logTextBox.AppendText(message + Environment.NewLine);
                                    logTextBox.ScrollToCaret();
                                }));
                            }
                        }
                    }
                }
            }

            button2.Text = "Descargando modpack (puede tardar un rato)...";

            // Descargar archivo zip modpack de Dropbox
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
                    startTime = DateTime.Now; // Tiempo de inicio
                    lastUpdateTime = DateTime.Now; // Tiempo de la última actualización

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

                            // Actualizar cada segundo
                            if (DateTime.Now - lastUpdateTime > TimeSpan.FromSeconds(2))
                            {
                                TimeSpan elapsedTime = DateTime.Now - startTime;
                                double bytesPerSecond = downloadedBytes / elapsedTime.TotalSeconds;
                                double bytesPerSecondInMB = bytesPerSecond / (1024 * 1024); // Convertir a MB/s
                                double estimatedTimeRemaining = (totalBytes - downloadedBytes) / bytesPerSecond;

                                // Actualizar el Label con el tiempo restante
                                timeLabel.Text = $"Tiempo restante: {TimeSpan.FromSeconds(estimatedTimeRemaining):hh\\:mm\\:ss}";

                                // Actualizar el Label con la velocidad
                                speedLabel.Text = $"{bytesPerSecondInMB:F2} MB/s";

                                // Actualizar la ubicación de los Labels
                                timeLabel.Location = new Point(progressBarUI.Left - timeLabel.Width - 10, progressBarUI.Top + progressBarUI.Height / 2 - timeLabel.Height / 2 - speedLabel.Height - 5);
                                speedLabel.Location = new Point(progressBarUI.Left - speedLabel.Width - 10, progressBarUI.Top + progressBarUI.Height / 2 - speedLabel.Height / 2);

                                lastUpdateTime = DateTime.Now; // Actualizar el tiempo de la última actualización
                            }

                            // Registrar el progreso en el TextBox (si está visible)
                            if (logTextBox.Visible)
                            {
                                string message = $"Descargando: {downloadedBytes} / {totalBytes} bytes ({speedLabel.Text}, {timeLabel.Text})";
                                Invoke((MethodInvoker)(() =>
                                {
                                    logTextBox.AppendText(message + Environment.NewLine);
                                    logTextBox.ScrollToCaret();
                                }));
                            }
                        }
                    }
                }
            }

            // Desactivar la animación de "Descargando..."
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

            // Ruta de los directorios
            string configuredDefaultsDir = Path.Combine(selectedPath, "configureddefaults");
            string configDir = Path.Combine(selectedPath, "config");

            // Llamar a la función para copiar archivos
            CopyFilesIfNotExists(configuredDefaultsDir, configDir);



            // Eliminar el archivo modpack.zip
            string fileToDelete = Path.Combine(selectedPath, "modpack.zip");
            File.Delete(fileToDelete);

            // Eliminar el archivo librerias.zip
            string fileToDelete2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "librerias.zip");
            File.Delete(fileToDelete2);

            // Eliminar el archivo Distant_Horizons_server_data.zip
            string fileToDelete3 = Path.Combine(selectedPath, "Distant_Horizons_server_data.zip");

            // Verificar si el archivo existe antes de intentar eliminarlo
            if (File.Exists(fileToDelete3))
            {
                try
                {
                    File.Delete(fileToDelete3);
                }
                catch (IOException)
                {
                }
            }
            else
            {
            }

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

            // Crear el nuevo objeto JSON para "AncientKraft"
            var newProfile = new JObject
            {
                ["created"] = "2024-03-08T20:18:06.251Z",
                ["gameDir"] = @$"{selectedPath}",
                ["icon"] = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAABIElEQVRYR+2XsRHDIAxF7XlcuvBO9hqsYHZy4dIreI3kThfdgU4fhF2IIukCBL3/kQQZB+fP6Bx/eALwqUA37dm0+BfYDYACb9tWNGDfd543iTMtSpV7AGTKp2kqOnBdF81bnbA44AagJluMkRTO85w5cZ4nfV/XFTmkii054AZAgY/jUBXyIHIAzS/LolaH5kAfAHzWIQQiv+87c4TneRCdPa9L5jPR0IFuAGrK2IGaU48dcAPglpt0NBLb2geUfWw50A0A6v3yaGRVcG7U7gZYBeyAG4Bs6BIIOaAollvZcsATQMam1ozOGF19qO6LdoDN+gCQZYnuglrdP3bAE4Ch+YHCWSwfLHLc8txr+mPiDoAS/tW4yaZXESo//gN8Abal0iEPdkE5AAAAAElFTkSuQmCC",
                ["javaArgs"] = javaArgs,
                ["lastUsed"] = "2024-03-08T20:31:10.859Z",
                ["lastVersionId"] = "1.20.1-forge-47.3.0",
                ["name"] = "AncientKraft",
                ["type"] = "custom"
            };

            // Los perfiles están en una propiedad "profiles" en el JSON
            if (jsonObj["profiles"] is JObject profiles) // Usar 'is' para intentar el cast y verificar por null
            {
                profiles["AncientKraft"] = newProfile; // Esto agregará el perfil "AncientKraft" justo debajo del último perfil existente
            }
            else
            {
                // Si "profiles" no es un JObject o es null se inicializa 'profiles' y se agrega a 'jsonObj'
                profiles = new JObject();
                profiles["AncientKraft"] = newProfile;
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
                    < 23 => 10,     // Si la RAM es más de 12GB y menos de 24GB, asigna 10
                    _ => 12         // Para 23GB o más, asigna 12
                };

                return $"-Xmx{javaMemory}G -XX:+UnlockExperimentalVMOptions -XX:+UseG1GC -XX:G1NewSizePercent=20 -XX:G1ReservePercent=20 -XX:MaxGCPauseMillis=50 -XX:G1HeapRegionSize=32M";
            }

            ////////////////////////////////////////
            //FORZAR TEXTUREPACKS
            ////////////////////////////////////////

            // Ruta del archivo options.txt a actualizar
            string optionsFilePath = Path.Combine(selectedPath, "options.txt");

            // Ruta del archivo options.txt con las configuraciones
            string defaultOptionsFilePath = Path.Combine(selectedPath, "config", "defaultoptions", "options.txt");

            try
            {
                // Verifica si el archivo options.txt existe en selectedPath
                if (File.Exists(optionsFilePath))
                {
                    // Verifica si el archivo options.txt con configuraciones existe
                    if (File.Exists(defaultOptionsFilePath))
                    {
                        string[] defaultOptionsLines = File.ReadAllLines(defaultOptionsFilePath);
                        string[] optionsLines = File.ReadAllLines(optionsFilePath);

                        // Usa expresiones regulares para encontrar las líneas relevantes
                        string? resourcePacksLine = null;
                        string? incompatibleResourcePacksLine = null;

                        foreach (string line in defaultOptionsLines)
                        {
                            if (line.Trim().StartsWith("resourcePacks:"))
                            {
                                resourcePacksLine = line.Trim();
                            }
                            else if (line.Trim().StartsWith("incompatibleResourcePacks:"))
                            {
                                incompatibleResourcePacksLine = line.Trim();
                            }
                        }

                        using (StreamWriter writer = new StreamWriter(optionsFilePath, false))
                        {
                            foreach (string line in optionsLines)
                            {
                                if (line.Trim().StartsWith("resourcePacks:"))
                                {
                                    writer.WriteLine(resourcePacksLine ?? "resourcePacks:[]");
                                }
                                else if (line.Trim().StartsWith("incompatibleResourcePacks:"))
                                {
                                    writer.WriteLine(incompatibleResourcePacksLine ?? "incompatibleResourcePacks:[]");
                                }
                                else
                                {
                                    writer.WriteLine(line);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Opcional: manejar excepciones de manera silenciosa o loguearlas en un archivo de registro si es necesario
            }



            if (!downloadDistantHorizons)
            {
                button2.Text = "Desactivando Distant Horizons...";

                string modsDir = Path.Combine(selectedPath, "mods");
                DirectoryInfo modsDirectory = new DirectoryInfo(modsDir);

                foreach (FileInfo file in modsDirectory.GetFiles("DistantHorizons*.jar"))
                {
                    string newFileName = file.FullName + ".disabled";
                    File.Move(file.FullName, newFileName);
                    if (logTextBox.Visible)
                    {
                        string message = $"Renombrado: {file.Name} a {Path.GetFileName(newFileName)}";
                        Invoke((MethodInvoker)(() =>
                        {
                            logTextBox.AppendText(message + Environment.NewLine);
                            logTextBox.ScrollToCaret();
                        }));
                    }
                }
            }

            if (downloadDistantHorizons)
            {
                // Ruta de la carpeta de descargas del usuario
                string downloadsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                string distantFileToCheck = Path.Combine(downloadsFolder, "Distant_Horizons_server_data.zip");

                if (File.Exists(distantFileToCheck))
                {
                    // El archivo ya está presente en la carpeta de descargas, proceder con la extracción
                    button2.Text = "Extrayendo Distant Horizons...";
                    // Asegurarse de que el cambio de texto se aplique antes de continuar
                    await Task.Delay(500); // Esperar medio segundo para asegurar la actualización

                    try
                    {
                        using (ZipFile zip = ZipFile.Read(distantFileToCheck))
                        {
                            zip.ExtractProgress += (sender, e) =>
                            {
                                if (e.EventType == ZipProgressEventType.Extracting_AfterExtractEntry)
                                {
                                    string message = $"Extrayendo Distant Horizons: {e.CurrentEntry.FileName}";
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

                                    if (logTextBox.Visible)
                                    {
                                        string message = $"Extrayendo Distant Horizons: {e.BytesTransferred} / {e.TotalBytesToTransfer} bytes";
                                        Invoke((MethodInvoker)(() =>
                                        {
                                            logTextBox.AppendText(message + Environment.NewLine);
                                            logTextBox.ScrollToCaret();
                                        }));
                                    }
                                }
                            };
                            zip.ExtractAll(selectedPath, ExtractExistingFileAction.OverwriteSilently);
                            FixDistant();
                        }

                        // Eliminar el archivo ZIP después de la extracción
                        File.Delete(distantFileToCheck);

                        // Una vez completada la extracción, marcar distant.txt como "true"
                        File.WriteAllText(distantFilePath, "true");
                        MessageBox.Show("Distant Horizons se ha descargado e instalado correctamente.", "Instalación Completa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error durante la extracción: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // El archivo no está presente, abrir la página de descarga
                    button2.Text = "Abriendo la página de descarga...";
                    // Asegurarse de que el cambio de texto se aplique antes de continuar
                    await Task.Delay(500); // Esperar medio segundo para asegurar la actualización

                    Process.Start(new ProcessStartInfo
                    {
                        FileName = distantHorizons_url,
                        UseShellExecute = true
                    });

                    // Mostrar un MessageBox para esperar la confirmación del usuario
                    DialogResult result = MessageBox.Show(
                        "Se ha abierto la página de descarga de Distant Horizons. Descargue el archivo ZIP y cuando termine pulse OK para seleccionarlo.",
                        "Esperando Confirmación",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Information
                    );

                    if (result == DialogResult.OK)
                    {
                        button2.Text = "Esperando la selección del archivo ZIP...";
                        // Asegurarse de que el cambio de texto se aplique antes de continuar
                        await Task.Delay(500); // Esperar medio segundo para asegurar la actualización

                        // Abrir el cuadro de diálogo para seleccionar el archivo ZIP descargado
                        using (OpenFileDialog openFileDialog = new OpenFileDialog())
                        {
                            openFileDialog.Filter = "Archivos ZIP (*.zip)|*.zip";
                            openFileDialog.Title = "Selecciona el archivo ZIP descargado";

                            if (openFileDialog.ShowDialog() == DialogResult.OK)
                            {
                                string zipFilePath = openFileDialog.FileName;

                                button2.Text = "Extrayendo Distant Horizons...";
                                // Asegurarse de que el cambio de texto se aplique antes de continuar
                                await Task.Delay(500); // Esperar medio segundo para asegurar la actualización

                                try
                                {
                                    using (ZipFile zip = ZipFile.Read(zipFilePath))
                                    {
                                        zip.ExtractProgress += (sender, e) =>
                                        {
                                            if (e.EventType == ZipProgressEventType.Extracting_AfterExtractEntry)
                                            {
                                                string message = $"Extrayendo Distant Horizons: {e.CurrentEntry.FileName}";
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

                                                if (logTextBox.Visible)
                                                {
                                                    string message = $"Extrayendo Distant Horizons: {e.BytesTransferred} / {e.TotalBytesToTransfer} bytes";
                                                    Invoke((MethodInvoker)(() =>
                                                    {
                                                        logTextBox.AppendText(message + Environment.NewLine);
                                                        logTextBox.ScrollToCaret();
                                                    }));
                                                }
                                            }
                                        };
                                        zip.ExtractAll(selectedPath, ExtractExistingFileAction.OverwriteSilently);
                                        FixDistant();
                                    }

                                    // Eliminar el archivo ZIP después de la extracción
                                    File.Delete(zipFilePath);

                                    // Una vez completada la extracción, marcar distant.txt como "true"
                                    File.WriteAllText(distantFilePath, "true");
                                    MessageBox.Show("Distant Horizons se ha descargado e instalado correctamente.", "Instalación Completa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show($"Error durante la extracción: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show("No se seleccionó ningún archivo. La instalación de Distant Horizons se ha cancelado.", "Cancelado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("La instalación de Distant Horizons se ha cancelado.", "Cancelado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }


            //////////////////////////////////
            // Reiniciar la barra de progreso
            /////////////////////////////////
            InitializeProgressBar();

            Cursor = Cursors.Default;
            button2.Text = "¡Listo! Modpack instalado correctamente!";
            button1.Text = "Finalizado";
            MessageBox.Show("La instalación del modpack oficial de AncientKraft ha finalizado. Ahora ejecute su Launcher y seleccione el perfil AncientKraft", "Instalación Completada", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Remover los controles TextBox y enlace del formulario
            Controls.Remove(logTextBox);


            // Abrir form Ajustes y pasar los valores actuales
            Ajustes form2 = new Ajustes(selectedPath, ajustesCheckBoxValue, ajustesTrackBar1Value, ajustesTrackBar2Value);
            form2.ShowDialog();
        }

        private void CopyFilesIfNotExists(string sourceDir, string destDir)
        {
            // Verificar que el directorio fuente exista
            if (!Directory.Exists(sourceDir))
            {
                //El directorio de origen no existe
                return;
            }

            // Crear el directorio de destino si no existe
            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            // Obtener los archivos del directorio fuente
            string[] files = Directory.GetFiles(sourceDir);

            foreach (string file in files)
            {
                // Obtener el nombre del archivo sin la ruta
                string fileName = Path.GetFileName(file);
                // Ruta completa del archivo en el directorio destino
                string destFile = Path.Combine(destDir, fileName);

                // Verificar si el archivo ya existe en el directorio destino
                if (!File.Exists(destFile))
                {
                    // Copiar el archivo si no existe
                    File.Copy(file, destFile);
                }
                else
                {
                    //Archivo ya existe, no copiado
                }
            }

            // Obtener los subdirectorios del directorio fuente
            string[] dirs = Directory.GetDirectories(sourceDir);

            foreach (string dir in dirs)
            {
                // Obtener el nombre del subdirectorio sin la ruta
                string dirName = Path.GetFileName(dir);
                // Ruta completa del subdirectorio en el directorio destino
                string destSubDir = Path.Combine(destDir, dirName);

                // Copiar recursivamente los archivos y subdirectorios
                CopyFilesIfNotExists(dir, destSubDir);
            }
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
            selectedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "_AncientKraft");
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

                // La variable para almacenar el nombre del perfil a eliminar
                string perfilNombre = "AncientKraft";
                bool perfilEliminado = false;

                // Verificar si el JSON tiene la propiedad "profiles"
                if (jsonObj["profiles"] is JObject profiles)
                {
                    // Buscar el perfil con el nombre específico en "profiles"
                    foreach (var kvp in profiles)
                    {
                        var perfil = kvp.Value as JObject;
                        if (perfil != null && perfil["name"]?.ToString() == perfilNombre)
                        {
                            // Eliminar el perfil encontrado
                            profiles.Remove(kvp.Key);
                            perfilEliminado = true;
                            break;
                        }
                    }
                }

                // Si el perfil fue eliminado, escribir el JSON modificado de vuelta en el archivo
                if (perfilEliminado)
                {
                    File.WriteAllText(perfilesPath, jsonObj.ToString(Formatting.Indented));
                }



                // Construir la ruta hacia la carpeta "AncientKraft" dentro de "versions"
                string AncientKraftPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "versions", "AncientKraft");

                // Verificar si la carpeta existe antes de intentar eliminarla
                if (Directory.Exists(AncientKraftPath))
                {
                    // Eliminar la carpeta "AncientKraft" y todo su contenido
                    Directory.Delete(AncientKraftPath, true);
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