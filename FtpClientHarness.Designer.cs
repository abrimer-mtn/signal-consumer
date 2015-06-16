namespace FtpClient
{
    partial class FtpClientHarness
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
            this.btnGetFiles = new System.Windows.Forms.Button();
            this.btnCheckFiles = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnGetFiles
            // 
            this.btnGetFiles.Location = new System.Drawing.Point(13, 13);
            this.btnGetFiles.Name = "btnGetFiles";
            this.btnGetFiles.Size = new System.Drawing.Size(75, 23);
            this.btnGetFiles.TabIndex = 0;
            this.btnGetFiles.Text = "GetFiles";
            this.btnGetFiles.UseVisualStyleBackColor = true;
            this.btnGetFiles.Click += new System.EventHandler(this.btnGetFiles_Click);
            // 
            // btnCheckFiles
            // 
            this.btnCheckFiles.Location = new System.Drawing.Point(95, 13);
            this.btnCheckFiles.Name = "btnCheckFiles";
            this.btnCheckFiles.Size = new System.Drawing.Size(75, 23);
            this.btnCheckFiles.TabIndex = 1;
            this.btnCheckFiles.Text = "Check Files";
            this.btnCheckFiles.UseVisualStyleBackColor = true;
            this.btnCheckFiles.Click += new System.EventHandler(this.btnCheckFiles_Click);
            // 
            // FtpClientHarness
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.btnCheckFiles);
            this.Controls.Add(this.btnGetFiles);
            this.Name = "FtpClientHarness";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnGetFiles;
        private System.Windows.Forms.Button btnCheckFiles;
    }
}

