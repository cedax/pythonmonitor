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
            this.labelMessage = new System.Windows.Forms.Label();
            this.controlPython = new System.Windows.Forms.Button();
            this.pythonLog = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelMessage
            // 
            this.labelMessage.AutoSize = true;
            this.labelMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.labelMessage.Location = new System.Drawing.Point(26, 64);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(175, 20);
            this.labelMessage.TabIndex = 0;
            this.labelMessage.Text = "Iniciar/Detener Python";
            // 
            // controlPython
            // 
            this.controlPython.Location = new System.Drawing.Point(12, 12);
            this.controlPython.Name = "controlPython";
            this.controlPython.Size = new System.Drawing.Size(200, 39);
            this.controlPython.TabIndex = 1;
            this.controlPython.Text = "Control Python";
            this.controlPython.UseVisualStyleBackColor = true;
            this.controlPython.Click += new System.EventHandler(this.controlPython_Click);
            // 
            // pythonLog
            // 
            this.pythonLog.Location = new System.Drawing.Point(232, 12);
            this.pythonLog.Name = "pythonLog";
            this.pythonLog.Size = new System.Drawing.Size(200, 39);
            this.pythonLog.TabIndex = 2;
            this.pythonLog.Text = "Log Python";
            this.pythonLog.UseVisualStyleBackColor = true;
            this.pythonLog.Click += new System.EventHandler(this.pythonLog_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label1.Location = new System.Drawing.Point(256, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(145, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Ver log del Python";
            // 
            // ConfirmationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(456, 103);
            this.ControlBox = false;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pythonLog);
            this.Controls.Add(this.controlPython);
            this.Controls.Add(this.labelMessage);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfirmationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ConfirmationForm";
            this.Load += new System.EventHandler(this.ConfirmationForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.Button controlPython;
        private System.Windows.Forms.Button pythonLog;
        private System.Windows.Forms.Label label1;
    }
}