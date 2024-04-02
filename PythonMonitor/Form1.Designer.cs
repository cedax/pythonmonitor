namespace PythonMonitor
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.openFileDialogPython = new System.Windows.Forms.OpenFileDialog();
            this.buttonAddPython = new System.Windows.Forms.Button();
            this.pythonLocation = new System.Windows.Forms.Label();
            this.checkedListBoxPythonLocations = new System.Windows.Forms.CheckedListBox();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.buttonStopLoopPython = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // openFileDialogPython
            // 
            this.openFileDialogPython.FileName = "openFileDialogPython";
            // 
            // buttonAddPython
            // 
            this.buttonAddPython.Location = new System.Drawing.Point(12, 15);
            this.buttonAddPython.Name = "buttonAddPython";
            this.buttonAddPython.Size = new System.Drawing.Size(200, 39);
            this.buttonAddPython.TabIndex = 0;
            this.buttonAddPython.Text = "Agregar Python";
            this.buttonAddPython.UseVisualStyleBackColor = true;
            this.buttonAddPython.Click += new System.EventHandler(this.buttonAddPython_Click);
            // 
            // pythonLocation
            // 
            this.pythonLocation.AutoSize = true;
            this.pythonLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pythonLocation.Location = new System.Drawing.Point(194, 19);
            this.pythonLocation.Name = "pythonLocation";
            this.pythonLocation.Size = new System.Drawing.Size(0, 25);
            this.pythonLocation.TabIndex = 1;
            // 
            // checkedListBoxPythonLocations
            // 
            this.checkedListBoxPythonLocations.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkedListBoxPythonLocations.HorizontalScrollbar = true;
            this.checkedListBoxPythonLocations.Location = new System.Drawing.Point(12, 75);
            this.checkedListBoxPythonLocations.Name = "checkedListBoxPythonLocations";
            this.checkedListBoxPythonLocations.Size = new System.Drawing.Size(644, 310);
            this.checkedListBoxPythonLocations.TabIndex = 0;
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(235, 15);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(200, 39);
            this.buttonDelete.TabIndex = 2;
            this.buttonDelete.Text = "Eliminar Python";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // textBoxLog
            // 
            this.textBoxLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxLog.Location = new System.Drawing.Point(674, 13);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxLog.Size = new System.Drawing.Size(342, 372);
            this.textBoxLog.TabIndex = 4;
            // 
            // buttonStopLoopPython
            // 
            this.buttonStopLoopPython.Location = new System.Drawing.Point(456, 15);
            this.buttonStopLoopPython.Name = "buttonStopLoopPython";
            this.buttonStopLoopPython.Size = new System.Drawing.Size(200, 39);
            this.buttonStopLoopPython.TabIndex = 5;
            this.buttonStopLoopPython.Text = "Detener loop del Python";
            this.buttonStopLoopPython.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1028, 397);
            this.Controls.Add(this.buttonStopLoopPython);
            this.Controls.Add(this.textBoxLog);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.checkedListBoxPythonLocations);
            this.Controls.Add(this.pythonLocation);
            this.Controls.Add(this.buttonAddPython);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Python Monitor";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialogPython;
        private System.Windows.Forms.Button buttonAddPython;
        private System.Windows.Forms.Label pythonLocation;
        private System.Windows.Forms.CheckedListBox checkedListBoxPythonLocations;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.TextBox textBoxLog;
        private System.Windows.Forms.Button buttonStopLoopPython;
    }
}

