namespace Mkamo.Memopad.Internal.Forms {
    partial class RegisterLicenseForm {
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
            this._registerButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._licenseFileTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this._licenseFileButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _registerButton
            // 
            this._registerButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._registerButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._registerButton.Location = new System.Drawing.Point(295, 56);
            this._registerButton.Name = "_registerButton";
            this._registerButton.Size = new System.Drawing.Size(75, 23);
            this._registerButton.TabIndex = 3;
            this._registerButton.Text = "登録(&R)";
            this._registerButton.UseVisualStyleBackColor = true;
            // 
            // _cancelButton
            // 
            this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.Location = new System.Drawing.Point(376, 56);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(84, 23);
            this._cancelButton.TabIndex = 4;
            this._cancelButton.Text = "キャンセル(&C)";
            this._cancelButton.UseVisualStyleBackColor = true;
            // 
            // _licenseFileTextBox
            // 
            this._licenseFileTextBox.Location = new System.Drawing.Point(119, 12);
            this._licenseFileTextBox.Name = "_licenseFileTextBox";
            this._licenseFileTextBox.Size = new System.Drawing.Size(288, 19);
            this._licenseFileTextBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "ライセンスファイル(&F):";
            // 
            // _licenseFileButton
            // 
            this._licenseFileButton.Location = new System.Drawing.Point(413, 10);
            this._licenseFileButton.Name = "_licenseFileButton";
            this._licenseFileButton.Size = new System.Drawing.Size(47, 23);
            this._licenseFileButton.TabIndex = 2;
            this._licenseFileButton.Text = "参照...";
            this._licenseFileButton.UseVisualStyleBackColor = true;
            this._licenseFileButton.Click += new System.EventHandler(this._licenseFileButton_Click);
            // 
            // RegisterLicenseForm
            // 
            this.AcceptButton = this._registerButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.ClientSize = new System.Drawing.Size(472, 91);
            this.Controls.Add(this._licenseFileButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._licenseFileTextBox);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._registerButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RegisterLicenseForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ライセンスの登録";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _registerButton;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.TextBox _licenseFileTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button _licenseFileButton;
    }
}
