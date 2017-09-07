namespace Plasan2Kitaron
{
    partial class bigMessageBox
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
            this.btnSerial = new System.Windows.Forms.Button();
            this.btnDigum = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnSerial
            // 
            this.btnSerial.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnSerial.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSerial.Font = new System.Drawing.Font("Microsoft Sans Serif", 19.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.btnSerial.Location = new System.Drawing.Point(68, 70);
            this.btnSerial.Name = "btnSerial";
            this.btnSerial.Size = new System.Drawing.Size(549, 202);
            this.btnSerial.TabIndex = 0;
            this.btnSerial.Text = "סדרתי";
            this.btnSerial.UseVisualStyleBackColor = false;
            // 
            // btnDigum
            // 
            this.btnDigum.BackColor = System.Drawing.Color.Coral;
            this.btnDigum.DialogResult = System.Windows.Forms.DialogResult.Retry;
            this.btnDigum.Font = new System.Drawing.Font("Microsoft Sans Serif", 19.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.btnDigum.Location = new System.Drawing.Point(68, 309);
            this.btnDigum.Name = "btnDigum";
            this.btnDigum.Size = new System.Drawing.Size(549, 202);
            this.btnDigum.TabIndex = 1;
            this.btnDigum.Text = "דיגום";
            this.btnDigum.UseVisualStyleBackColor = false;
            // 
            // bigMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(682, 597);
            this.Controls.Add(this.btnDigum);
            this.Controls.Add(this.btnSerial);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "bigMessageBox";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "בחירת סוג הקובץ";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSerial;
        private System.Windows.Forms.Button btnDigum;
    }
}