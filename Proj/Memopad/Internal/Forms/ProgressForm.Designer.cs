namespace Mkamo.Memopad.Internal.Forms {
    partial class ProgressForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this._messageLabel = new System.Windows.Forms.Label();
            this._recoverProgressBar = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // _messageLabel
            // 
            this._messageLabel.AutoSize = true;
            this._messageLabel.Location = new System.Drawing.Point(10, 38);
            this._messageLabel.Name = "_messageLabel";
            this._messageLabel.Size = new System.Drawing.Size(50, 12);
            this._messageLabel.TabIndex = 0;
            this._messageLabel.Text = "message";
            // 
            // _recoverProgressBar
            // 
            this._recoverProgressBar.Location = new System.Drawing.Point(12, 12);
            this._recoverProgressBar.Name = "_recoverProgressBar";
            this._recoverProgressBar.Size = new System.Drawing.Size(268, 23);
            this._recoverProgressBar.TabIndex = 1;
            // 
            // ProgressForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 100);
            this.ControlBox = false;
            this.Controls.Add(this._recoverProgressBar);
            this.Controls.Add(this._messageLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "復元";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _messageLabel;
        private System.Windows.Forms.ProgressBar _recoverProgressBar;
    }
}
