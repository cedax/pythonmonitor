using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Windows.Forms;

namespace PythonMonitor
{
    public partial class Form1 : Form
    {
        public bool firstRun = true;
        private const string settingsFile = "python_locations.json";
        private List<PythonFileInfo> pythonLocations;
        private NotifyIcon notifyIcon;

        public Form1()
        {
            InitializeComponent();
            pythonLocations = new List<PythonFileInfo>();

            checkedListBoxPythonLocations.ItemCheck += checkedListBoxPythonLocations_ItemCheck;

            // Inicializar el NotifyIcon
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = new Icon("Icons/icono.ico"); // Asigna un icono personalizado
            notifyIcon.Text = "Python Monitor";
            notifyIcon.Visible = true;

            // Manejar el evento de clic en el icono de la bandeja del sistema
            notifyIcon.MouseClick += NotifyIcon_MouseClick;

            this.FormBorderStyle = FormBorderStyle.FixedSingle; // Esto deshabilita la capacidad de redimensionamiento
        }

        private void buttonAddPython_Click(object sender, EventArgs e)
        {
            if (openFileDialogPython.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialogPython.FileName;
                if (IsPythonFile(filePath))
                {
                    if (!IsPathAlreadyAdded(filePath))
                    {
                        AddPythonLocation(filePath);
                    }
                    else
                    {
                        ShowErrorMessage("La ruta de archivo ya ha sido agregada.");
                    }
                }
                else
                {
                    ShowErrorMessage("Por favor, seleccione un archivo Python (.py).");
                }
            }
        }

        private bool IsPythonFile(string filePath)
        {
            return System.IO.Path.GetExtension(filePath).Equals(".py", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsPathAlreadyAdded(string filePath)
        {
            return pythonLocations.Any(info => info.Path.Equals(filePath, StringComparison.OrdinalIgnoreCase));
        }

        private void AddPythonLocation(string filePath)
        {
            pythonLocations.Add(new PythonFileInfo { Id = pythonLocations.Count + 1, Path = filePath, Checked = false });
            SavePythonLocations();
            RefreshCheckedListBox();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadPythonLocations();
            RefreshCheckedListBox();

            // Crear el ContextMenuStrip
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            contextMenuStrip.Items.Add("Mostrar", null, (sendera, a) => Show());
            contextMenuStrip.Items.Add("Cerrar programa", null, (sendera, a) => Application.Exit());

            // Asignar el ContextMenuStrip al NotifyIcon
            notifyIcon.ContextMenuStrip = contextMenuStrip;
            firstRun = false;
        }

        private void SavePythonLocations()
        {
            string json = JsonSerializer.Serialize(pythonLocations);
            File.WriteAllText(settingsFile, json);
        }

        private void LoadPythonLocations()
        {
            if (File.Exists(settingsFile))
            {
                string json = File.ReadAllText(settingsFile);
                pythonLocations = JsonSerializer.Deserialize<List<PythonFileInfo>>(json);
            }
        }

        private void RefreshCheckedListBox()
        {
            checkedListBoxPythonLocations.Items.Clear();
            foreach (var location in pythonLocations)
            {
                checkedListBoxPythonLocations.Items.Add(location.Path, location.Checked);
            }
        }

        private void ExecutePythonScript(string scriptPath, int index)
        {
            try
            {
                // Obtener el nombre del archivo .py sin la extensión
                string scriptName = Path.GetFileNameWithoutExtension(scriptPath);

                // Obtener la fecha y hora actual
                string dateTimeString = DateTime.Now.ToString("yyyyMMdd_HHmmss");

                // Crear el nombre completo del archivo de log usando el nombre del script y la fecha y hora
                string logFileName = Path.Combine(Path.GetDirectoryName(scriptPath), $"{scriptName}_{dateTimeString}_log.txt");

                Console.WriteLine(logFileName);

                // Obtener el ID del hilo actual
                int threadId = Thread.CurrentThread.ManagedThreadId;
                pythonLocations[index].LastPIDThread = threadId;

                // Verificar si el archivo de log no existe
                if (!File.Exists(logFileName))
                {
                    // Crear el archivo de log de forma sincrónica
                    using (File.Create(logFileName)) { }
                }

                // Configurar el proceso para ejecutar el archivo Python
                ProcessStartInfo startInfo = new ProcessStartInfo();
                
                startInfo.FileName = "cmd.exe"; // Nombre del ejecutable del comando cmd.exe
                startInfo.Arguments = $"/C python {scriptPath} > \"{logFileName}\" 2>&1"; // Ejecutar comando y redirigir la salida al archivo de log
                startInfo.UseShellExecute = false;

                // Ocultar la ventana del proceso secundario
                startInfo.CreateNoWindow = true;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                // Iniciar el proceso
                using (Process process = new Process())
                {
                    process.StartInfo = startInfo;
                    process.Start();

                    pythonLocations[index].LastLog = logFileName;
                    // Guardar los cambios en el archivo JSON
                    SavePythonLocations();

                    // Esperar a que el proceso termine
                    process.WaitForExit();

                    // Verificar si el proceso terminó con éxito
                    if (process.ExitCode != 0)
                    {
                        // El proceso terminó con un código de error
                        Console.WriteLine("El script Python ha terminado con un código de error: " + process.ExitCode);
                        // Aquí puedes agregar cualquier acción de limpieza necesaria o notificación de error

                        // ToDo: Enviar email para informar de error
                        // Obtener el timestamp actual en formato de cadena de texto
                        string lastEmailNotify = pythonLocations[index].LastEmailNotify;

                        if(lastEmailNotify == null)
                        {
                            // Obtener la fecha y hora actual
                            DateTime now = DateTime.Now;

                            // Restar 5 minutos a la fecha y hora actual
                            DateTime fiveMinutesAgo = now.AddMinutes(-59);

                            // Convertir la fecha y hora resultante a formato de cadena
                            lastEmailNotify = fiveMinutesAgo.ToString("yyyy-MM-dd HH:mm:ss");
                        }

                        // Obtener el timestamp actual
                        string currentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                        // Convertir lastEmailNotify a DateTime
                        DateTime lastEmailNotifyDateTime = DateTime.ParseExact(lastEmailNotify, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

                        // Convertir currentTimestamp a DateTime
                        DateTime currentDateTime = DateTime.ParseExact(currentTimestamp, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

                        // Calcular la diferencia entre los dos timestamps
                        TimeSpan difference = currentDateTime - lastEmailNotifyDateTime;

                        // Verificar si la diferencia es mayor a 5 minutos
                        if (difference.TotalMinutes > 5)
                        {
                            // Configurar las credenciales de Gmail
                            string fromEmail = "sedax.contact@gmail.com";
                            string password = "jqkoczsovdxvikqg"; // Asegúrate de usar una cuenta con autenticación de dos factores y una contraseña de aplicación para mayor seguridad
                            
                            
                            //USER_GMAIL = sedax.contact@gmail.com
                            //PASS_GMAIL = jqkoczsovdxvikqg


                            // Configurar el destinatario y el asunto del correo
                            string toEmail = "lopez17081@gmail.com";
                            string subject = "¡Hola desde C#!";
                            string body = "Este es un correo electrónico enviado desde una aplicación C#.";

                            // Configurar el cliente SMTP de Gmail
                            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
                            {
                                Port = 587,
                                Credentials = new NetworkCredential(fromEmail, password),
                                EnableSsl = true,
                            };

                            // Crear el correo electrónico
                            MailMessage mailMessage = new MailMessage(fromEmail, toEmail, subject, body);

                            try
                            {
                                // Enviar el correo electrónico
                                smtpClient.Send(mailMessage);
                                Console.WriteLine("Correo electrónico enviado exitosamente.");
                                lastEmailNotify = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                pythonLocations[index].LastEmailNotify = lastEmailNotify;
                                // Guardar los cambios en el archivo JSON
                                SavePythonLocations();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error al enviar el correo electrónico: {ex.Message}");
                            }
                        }

                        // Obtener el camino (path) del elemento chequeado
                        string filePath = pythonLocations[index].Path;

                        // Verificar si el archivo existe y es un archivo Python (.py)
                        if (File.Exists(filePath) && Path.GetExtension(filePath).Equals(".py", StringComparison.OrdinalIgnoreCase))
                        {
                            // Ejecutar el archivo Python en un hilo aparte
                            if (pythonLocations[index].Checked == true)
                            {
                                // ToDo: Agregar el ID de Thread al JSON para despues poder matar el hilo
                                // Esto se ejecuta cuando es la primer ejecucion, para los archivos que estan marcados como TRUE
                                Thread pythonThread = new Thread(() => ExecutePythonScript(filePath, index));
                                pythonThread.Start();
                            }
                        }
                        else
                        {
                            MessageBox.Show($"El archivo no existe o no es un archivo Python ({filePath}).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                    }
                    else
                    {
                        // El proceso terminó con éxito
                        Console.WriteLine("El script Python ha terminado correctamente.");
                        // Aquí puedes agregar cualquier acción adicional que necesites realizar después de que el script se ejecute correctamente
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que ocurra al ejecutar el script
                Console.WriteLine("Error al ejecutar el script Python: " + ex.Message);
                // Aquí puedes agregar cualquier acción de limpieza necesaria o notificación de error
            }
        }

        private void checkedListBoxPythonLocations_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Obtener el índice del elemento seleccionado
            int selectedIndex = e.Index;

            // Obtener la ruta del elemento seleccionado
            string selectedPath = pythonLocations[selectedIndex].Path;

            string actionPreset = "";

            if (e.NewValue == CheckState.Unchecked)
            {
                actionPreset = "desactivar";
            }else
            {
                actionPreset = "activar";
            }

            // Mostrar un cuadro de diálogo de confirmación
            if (firstRun)
            {
                // Obtener el camino (path) del elemento chequeado
                string filePath = pythonLocations[e.Index].Path;

                // Verificar si el archivo existe y es un archivo Python (.py)
                if (File.Exists(filePath) && Path.GetExtension(filePath).Equals(".py", StringComparison.OrdinalIgnoreCase))
                {
                    // Actualizar el estado de marcado en la lista pythonLocations
                    pythonLocations[e.Index].Checked = (e.NewValue == CheckState.Checked);

                    // Guardar los cambios en el archivo JSON
                    SavePythonLocations();

                    // Ejecutar el archivo Python en un hilo aparte
                    if (e.NewValue == CheckState.Checked)
                    {
                        // ToDo: Agregar el ID de Thread al JSON para despues poder matar el hilo
                        // Esto se ejecuta cuando es la primer ejecucion, para los archivos que estan marcados como TRUE
                        Thread pythonThread = new Thread(() => ExecutePythonScript(filePath, e.Index));
                        pythonThread.Start();
                    }
                }
                else
                {
                    MessageBox.Show($"El archivo no existe o no es un archivo Python ({filePath}).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Cancelar el cambio de estado del elemento chequeado
                    e.NewValue = CheckState.Unchecked;
                    pythonLocations[e.Index].Checked = false;
                    // Guardar los cambios en el archivo JSON
                    SavePythonLocations();
                }
            }
            else
            {
                // Crear y mostrar el formulario de confirmación personalizado
                using (ConfirmationForm confirmationForm = new ConfirmationForm(actionPreset))
                {
                    DialogResult resultTwo = confirmationForm.ShowDialog();

                    // Verificar la opción seleccionada por el usuario
                    if (resultTwo == DialogResult.Yes)
                    {
                        DialogResult result = MessageBox.Show($"¿Estás seguro de {actionPreset} el python seleccionado ({selectedPath})?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        // Verificar la respuesta del usuario
                        if (result == DialogResult.Yes)
                        {
                            // Obtener el camino (path) del elemento chequeado
                            string filePath = pythonLocations[e.Index].Path;

                            // Verificar si el archivo existe y es un archivo Python (.py)
                            if (File.Exists(filePath) && Path.GetExtension(filePath).Equals(".py", StringComparison.OrdinalIgnoreCase))
                            {
                                // Actualizar el estado de marcado en la lista pythonLocations
                                pythonLocations[e.Index].Checked = (e.NewValue == CheckState.Checked);

                                // Guardar los cambios en el archivo JSON
                                SavePythonLocations();

                                // Ejecutar el archivo Python en un hilo aparte
                                if (e.NewValue == CheckState.Checked)
                                {
                                    // ToDo: Agregar el ID de Thread al JSON para despues poder matar el hilo
                                    // Esto se ejecuta cuando es la primer ejecucion, para los archivos que estan marcados como TRUE
                                    Thread pythonThread = new Thread(() => ExecutePythonScript(filePath, e.Index));
                                    pythonThread.Start();
                                }
                            }
                            else
                            {
                                MessageBox.Show("El archivo no existe o no es un archivo Python (.py).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                // Cancelar el cambio de estado del elemento chequeado
                                e.NewValue = CheckState.Unchecked;
                                pythonLocations[e.Index].Checked = false;
                                // Guardar los cambios en el archivo JSON
                                SavePythonLocations();
                            }
                        }
                        else
                        {
                            e.NewValue = e.CurrentValue;
                        }
                    }
                    else if (resultTwo == DialogResult.No)
                    {
                        // Log de python
                        e.NewValue = e.CurrentValue;

                        try
                        {
                            string pathLog = pythonLocations[e.Index].LastLog;
                            
                            // Verificar si el archivo de log existe
                            if (File.Exists(pathLog))
                            {
                                // Leer el contenido del archivo de log
                                string originalLogPath = pythonLocations[e.Index].LastLog;
                                string tempLogPath = Path.GetTempFileName(); // Obtener una ruta temporal única

                                try
                                {
                                    // Copiar el archivo de log a la ruta temporal
                                    File.Copy(originalLogPath, tempLogPath, true);

                                    // Verificar si la copia temporal del archivo de log existe
                                    if (File.Exists(tempLogPath))
                                    {
                                        // Leer el contenido de la copia temporal del archivo de log
                                        string logContent = File.ReadAllText(tempLogPath);

                                        // Mostrar el contenido en el TextBoxLog
                                        textBoxLog.Text = logContent;
                                    }
                                    else
                                    {
                                        // La copia temporal del archivo de log no existe
                                        textBoxLog.Text = "El archivo de log no pudo ser copiado.";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    // Manejar cualquier excepción que pueda ocurrir al copiar o leer el archivo de log
                                    textBoxLog.Text = "Error al leer el archivo de log: " + ex.Message;
                                }
                                finally
                                {
                                    // Eliminar la copia temporal del archivo de log
                                    if (File.Exists(tempLogPath))
                                    {
                                        File.Delete(tempLogPath);
                                    }
                                }
                            }
                            else
                            {
                                // El archivo de log no existe
                                textBoxLog.Text = "El archivo de log no existe.";
                            }
                        }
                        catch (Exception ex)
                        {
                            // Manejar cualquier excepción que pueda ocurrir al abrir o leer el archivo de log
                            textBoxLog.Text = $"Error al abrir el archivo de log: {ex.Message}";
                        }

                    }
                    else
                    {
                        e.NewValue = e.CurrentValue;
                    }
                }
            }
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            // Verificar si hay algún elemento seleccionado para eliminar
            if (checkedListBoxPythonLocations.SelectedIndex != -1)
            {
                // Obtener el índice del elemento seleccionado
                int selectedIndex = checkedListBoxPythonLocations.SelectedIndex;

                // Verificar si el Python está desactivado
                if (!pythonLocations[selectedIndex].Checked)
                {
                    // Eliminar el elemento del CheckedListBox
                    checkedListBoxPythonLocations.Items.RemoveAt(selectedIndex);

                    // Eliminar el elemento de la lista pythonLocations
                    pythonLocations.RemoveAt(selectedIndex);

                    // Guardar los cambios en el archivo JSON
                    SavePythonLocations();
                }
                else
                {
                    ShowErrorMessage("El Python debe estar desactivado para poder eliminarlo.");
                }
            }
            else
            {
                ShowErrorMessage("Por favor, seleccione el Python a eliminar.");
            }
        }

        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            // Si el clic fue con el botón derecho, mostrar el menú contextual
            if (e.Button == MouseButtons.Right)
            {
                // Obtener la posición del ratón para mostrar el menú en esa posición
                Point mousePosition = Control.MousePosition;
                notifyIcon.ContextMenuStrip.Show(mousePosition);
            }
            // Si el clic fue con el botón izquierdo, restaurar el formulario
            else if (e.Button == MouseButtons.Left)
            {
                Show();
                WindowState = FormWindowState.Normal;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Prevenir el cierre y simplemente ocultar el formulario
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
                notifyIcon.ShowBalloonTip(1000, "Python Monitor", "La aplicación continua ejecutandose en segundo plano.", ToolTipIcon.Info);
            }
            else
            {
                // Asegúrate de que el icono en la bandeja del sistema se elimine cuando se cierre la aplicación
                notifyIcon.Dispose();
                base.OnFormClosing(e);
            }
        }
    }

    public class PythonFileInfo
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public bool Checked { get; set; }
        public string LastLog { get; set; }
        public int LastPIDThread { get; set; }
        public string LastEmailNotify { get; set; }
    }
}
