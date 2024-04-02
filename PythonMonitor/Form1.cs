using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
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
                //startInfo.CreateNoWindow = true;
                //startInfo.WindowStyle = ProcessWindowStyle.Hidden;

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
                using (ConfirmationForm confirmationForm = new ConfirmationForm())
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
                                string logContent = File.ReadAllText(pathLog);

                                // Mostrar el contenido en el TextBoxLog
                                textBoxLog.Text = logContent;
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

                // Eliminar el elemento del CheckedListBox
                checkedListBoxPythonLocations.Items.RemoveAt(selectedIndex);

                // Eliminar el elemento de la lista pythonLocations
                pythonLocations.RemoveAt(selectedIndex);

                // Guardar los cambios en el archivo JSON
                SavePythonLocations();
            }else
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
    }
}
