namespace CCEncrypting
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
            this.mtbEncrypted = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnDecrypt = new System.Windows.Forms.Button();
            this.btnEncrypt = new System.Windows.Forms.Button();
            this.mtbDecrypted = new System.Windows.Forms.TextBox();
            this.mtbLog = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbDecrypted = new System.Windows.Forms.TextBox();
            this.tbEncrypted = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mtbEncrypted
            // 
            this.mtbEncrypted.Location = new System.Drawing.Point(12, 51);
            this.mtbEncrypted.MaxLength = 1327670;
            this.mtbEncrypted.Multiline = true;
            this.mtbEncrypted.Name = "mtbEncrypted";
            this.mtbEncrypted.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.mtbEncrypted.Size = new System.Drawing.Size(409, 418);
            this.mtbEncrypted.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Encrypted";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(473, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Decrypted";
            // 
            // btnDecrypt
            // 
            this.btnDecrypt.Location = new System.Drawing.Point(427, 25);
            this.btnDecrypt.Name = "btnDecrypt";
            this.btnDecrypt.Size = new System.Drawing.Size(43, 223);
            this.btnDecrypt.TabIndex = 4;
            this.btnDecrypt.Text = ">>";
            this.btnDecrypt.UseVisualStyleBackColor = true;
            this.btnDecrypt.Click += new System.EventHandler(this.btnDecrypt_Click);
            // 
            // btnEncrypt
            // 
            this.btnEncrypt.Location = new System.Drawing.Point(427, 255);
            this.btnEncrypt.Name = "btnEncrypt";
            this.btnEncrypt.Size = new System.Drawing.Size(43, 214);
            this.btnEncrypt.TabIndex = 5;
            this.btnEncrypt.Text = "<<";
            this.btnEncrypt.UseVisualStyleBackColor = true;
            this.btnEncrypt.Click += new System.EventHandler(this.btnEncrypt_Click);
            // 
            // mtbDecrypted
            // 
            this.mtbDecrypted.Location = new System.Drawing.Point(476, 51);
            this.mtbDecrypted.MaxLength = 1320000;
            this.mtbDecrypted.Multiline = true;
            this.mtbDecrypted.Name = "mtbDecrypted";
            this.mtbDecrypted.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.mtbDecrypted.Size = new System.Drawing.Size(409, 418);
            this.mtbDecrypted.TabIndex = 7;
            // 
            // mtbLog
            // 
            this.mtbLog.Location = new System.Drawing.Point(12, 498);
            this.mtbLog.Multiline = true;
            this.mtbLog.Name = "mtbLog";
            this.mtbLog.ReadOnly = true;
            this.mtbLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.mtbLog.Size = new System.Drawing.Size(873, 134);
            this.mtbLog.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 482);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Error Log";
            // 
            // tbDecrypted
            // 
            this.tbDecrypted.Location = new System.Drawing.Point(476, 25);
            this.tbDecrypted.Name = "tbDecrypted";
            this.tbDecrypted.Size = new System.Drawing.Size(409, 20);
            this.tbDecrypted.TabIndex = 6;
            // 
            // tbEncrypted
            // 
            this.tbEncrypted.Location = new System.Drawing.Point(12, 25);
            this.tbEncrypted.Name = "tbEncrypted";
            this.tbEncrypted.Size = new System.Drawing.Size(409, 20);
            this.tbEncrypted.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(757, 639);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(128, 32);
            this.button1.TabIndex = 10;
            this.button1.Text = "Exit";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnClearLog
            // 
            this.btnClearLog.Location = new System.Drawing.Point(12, 639);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(135, 31);
            this.btnClearLog.TabIndex = 11;
            this.btnClearLog.Text = "Clear Log";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(898, 673);
            this.Controls.Add(this.btnClearLog);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.mtbLog);
            this.Controls.Add(this.mtbDecrypted);
            this.Controls.Add(this.tbDecrypted);
            this.Controls.Add(this.btnEncrypt);
            this.Controls.Add(this.btnDecrypt);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mtbEncrypted);
            this.Controls.Add(this.tbEncrypted);
            this.Name = "Form1";
            this.Text = "Encryption Tool";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox mtbEncrypted;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnDecrypt;
        private System.Windows.Forms.Button btnEncrypt;
        private System.Windows.Forms.TextBox mtbDecrypted;
        private System.Windows.Forms.TextBox mtbLog;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbDecrypted;
        private System.Windows.Forms.TextBox tbEncrypted;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnClearLog;
    }
}

