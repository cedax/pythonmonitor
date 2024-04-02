using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PythonMonitor
{
    public partial class ConfirmationForm : Form
    {
        public ConfirmationForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle; // Esto deshabilita la capacidad de redimensionamiento
        }

        private void ConfirmationForm_Load(object sender, EventArgs e)
        {

        }

        private void controlPython_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
            Close();
        }

        private void pythonLog_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
            Close();
        }
    }
}
