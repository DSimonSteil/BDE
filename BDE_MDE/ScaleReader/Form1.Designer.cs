namespace ScaleReader
{
    partial class Form1
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
            this.tbx_result = new System.Windows.Forms.TextBox();
            this.tbx_feedback = new System.Windows.Forms.TextBox();
            this.tbx_comPort = new System.Windows.Forms.TextBox();
            this.btn_start = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tbx_result
            // 
            this.tbx_result.Location = new System.Drawing.Point(13, 44);
            this.tbx_result.Multiline = true;
            this.tbx_result.Name = "tbx_result";
            this.tbx_result.Size = new System.Drawing.Size(775, 394);
            this.tbx_result.TabIndex = 0;
            // 
            // tbx_feedback
            // 
            this.tbx_feedback.Location = new System.Drawing.Point(13, 461);
            this.tbx_feedback.Multiline = true;
            this.tbx_feedback.Name = "tbx_feedback";
            this.tbx_feedback.Size = new System.Drawing.Size(775, 50);
            this.tbx_feedback.TabIndex = 1;
            // 
            // tbx_comPort
            // 
            this.tbx_comPort.Location = new System.Drawing.Point(134, 12);
            this.tbx_comPort.Name = "tbx_comPort";
            this.tbx_comPort.Size = new System.Drawing.Size(100, 20);
            this.tbx_comPort.TabIndex = 2;
            // 
            // btn_start
            // 
            this.btn_start.Location = new System.Drawing.Point(328, 12);
            this.btn_start.Name = "btn_start";
            this.btn_start.Size = new System.Drawing.Size(75, 23);
            this.btn_start.TabIndex = 3;
            this.btn_start.Text = "Start";
            this.btn_start.UseVisualStyleBackColor = true;
            this.btn_start.Click += new System.EventHandler(this.btn_start_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(43, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "COM Port:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 523);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_start);
            this.Controls.Add(this.tbx_comPort);
            this.Controls.Add(this.tbx_feedback);
            this.Controls.Add(this.tbx_result);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbx_result;
        private System.Windows.Forms.TextBox tbx_feedback;
        private System.Windows.Forms.TextBox tbx_comPort;
        private System.Windows.Forms.Button btn_start;
        private System.Windows.Forms.Label label1;
    }
}

