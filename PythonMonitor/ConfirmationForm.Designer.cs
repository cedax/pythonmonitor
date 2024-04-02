namespace PythonMonitor
{
    partial class ConfirmationForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfirmationForm));
            this.controlPython = new System.Windows.Forms.Button();
            this.pythonLog = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // controlPython
            // 
            this.controlPython.Location = new System.Drawing.Point(12, 12);
            this.controlPython.Name = "controlPython";
            this.controlPython.Size = new System.Drawing.Size(200, 39);
            this.controlPython.TabIndex = 1;
            this.controlPython.Text = "Control";
            this.controlPython.UseVisualStyleBackColor = true;
            this.controlPython.Click += new System.EventHandler(this.controlPython_Click);
            // 
            // pythonLog
            // 
            this.pythonLog.Location = new System.Drawing.Point(232, 12);
            this.pythonLog.Name = "pythonLog";
            this.pythonLog.Size = new System.Drawing.Size(200, 39);
            this.pythonLog.TabIndex = 2;
            this.pythonLog.Text = "Log";
            this.pythonLog.UseVisualStyleBackColor = true;
            this.pythonLog.Click += new System.EventHandler(this.pythonLog_Click);
            // 
            // ConfirmationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(456, 71);
            this.ControlBox = false;
            this.Controls.Add(this.pythonLog);
            this.Controls.Add(this.controlPython);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfirmationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Control de Python";
            this.Load += new System.EventHandler(this.ConfirmationForm_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button controlPython;
        private System.Windows.Forms.Button pythonLog;
    }
}