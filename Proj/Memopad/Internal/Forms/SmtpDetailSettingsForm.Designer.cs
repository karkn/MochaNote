namespace Mkamo.Memopad.Internal.Forms {
    partial class SmtpDetailSettingsForm {
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
            this.label1 = new System.Windows.Forms.Label();
            this._portTextBox = new System.Windows.Forms.TextBox();
            this._userNameLabel = new System.Windows.Forms.Label();
            this._userNameTextBox = new System.Windows.Forms.TextBox();
            this._enableAuthCheckBox = new System.Windows.Forms.CheckBox();
            this._passwordLabel = new System.Windows.Forms.Label();
            this._passwordTextBox = new System.Windows.Forms.TextBox();
            this._enableSslCheckBox = new System.Windows.Forms.CheckBox();
            this._okButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "ポート(&P):";
            // 
            // _portTextBox
            // 
            this._portTextBox.Location = new System.Drawing.Point(68, 12);
            this._portTextBox.Name = "_portTextBox";
            this._portTextBox.Size = new System.Drawing.Size(74, 19);
            this._portTextBox.TabIndex = 1;
            this._portTextBox.Validating += new System.ComponentModel.CancelEventHandler(this._portTextBox_Validating);
            // 
            // _userNameLabel
            // 
            this._userNameLabel.AutoSize = true;
            this._userNameLabel.Location = new System.Drawing.Point(32, 69);
            this._userNameLabel.Name = "_userNameLabel";
            this._userNameLabel.Size = new System.Drawing.Size(75, 12);
            this._userNameLabel.TabIndex = 3;
            this._userNameLabel.Text = "ユーザー名(&U):";
            // 
            // _userNameTextBox
            // 
            this._userNameTextBox.Location = new System.Drawing.Point(113, 66);
            this._userNameTextBox.Name = "_userNameTextBox";
            this._userNameTextBox.Size = new System.Drawing.Size(167, 19);
            this._userNameTextBox.TabIndex = 4;
            // 
            // _useAuthCheckBox
            // 
            this._enableAuthCheckBox.AutoSize = true;
            this._enableAuthCheckBox.Location = new System.Drawing.Point(14, 44);
            this._enableAuthCheckBox.Name = "_useAuthCheckBox";
            this._enableAuthCheckBox.Size = new System.Drawing.Size(146, 16);
            this._enableAuthCheckBox.TabIndex = 2;
            this._enableAuthCheckBox.Text = "SMTP認証を使用する(&A)";
            this._enableAuthCheckBox.UseVisualStyleBackColor = true;
            this._enableAuthCheckBox.CheckedChanged += new System.EventHandler(this._enableAuthCheckBox_CheckedChanged);
            // 
            // _passwordLabel
            // 
            this._passwordLabel.AutoSize = true;
            this._passwordLabel.Location = new System.Drawing.Point(32, 94);
            this._passwordLabel.Name = "_passwordLabel";
            this._passwordLabel.Size = new System.Drawing.Size(69, 12);
            this._passwordLabel.TabIndex = 5;
            this._passwordLabel.Text = "パスワード(&P):";
            // 
            // _passwordTextBox
            // 
            this._passwordTextBox.Location = new System.Drawing.Point(113, 91);
            this._passwordTextBox.Name = "_passwordTextBox";
            this._passwordTextBox.Size = new System.Drawing.Size(167, 19);
            this._passwordTextBox.TabIndex = 6;
            this._passwordTextBox.UseSystemPasswordChar = true;
            // 
            // _enableSslCheckBox
            // 
            this._enableSslCheckBox.AutoSize = true;
            this._enableSslCheckBox.Location = new System.Drawing.Point(14, 123);
            this._enableSslCheckBox.Name = "_enableSslCheckBox";
            this._enableSslCheckBox.Size = new System.Drawing.Size(111, 16);
            this._enableSslCheckBox.TabIndex = 7;
            this._enableSslCheckBox.Text = "SSLを使用する(&S)";
            this._enableSslCheckBox.UseVisualStyleBackColor = true;
            // 
            // _okButton
            // 
            this._okButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._okButton.Location = new System.Drawing.Point(108, 153);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(75, 23);
            this._okButton.TabIndex = 8;
            this._okButton.Text = "OK(&O)";
            this._okButton.UseVisualStyleBackColor = true;
            // 
            // _cancelButton
            // 
            this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.Location = new System.Drawing.Point(189, 153);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(91, 23);
            this._cancelButton.TabIndex = 9;
            this._cancelButton.Text = "キャンセル(&C)";
            this._cancelButton.UseVisualStyleBackColor = true;
            // 
            // SmtpDetailSettingsForm
            // 
            this.AcceptButton = this._okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.ClientSize = new System.Drawing.Size(292, 188);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._okButton);
            this.Controls.Add(this._enableSslCheckBox);
            this.Controls.Add(this._enableAuthCheckBox);
            this.Controls.Add(this._passwordTextBox);
            this.Controls.Add(this._userNameTextBox);
            this.Controls.Add(this._portTextBox);
            this.Controls.Add(this._passwordLabel);
            this.Controls.Add(this._userNameLabel);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SmtpDetailSettingsForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SMTP詳細設定";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label _userNameLabel;
        private System.Windows.Forms.Label _passwordLabel;
        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.Button _cancelButton;
        internal System.Windows.Forms.TextBox _portTextBox;
        internal System.Windows.Forms.TextBox _userNameTextBox;
        internal System.Windows.Forms.CheckBox _enableAuthCheckBox;
        internal System.Windows.Forms.TextBox _passwordTextBox;
        internal System.Windows.Forms.CheckBox _enableSslCheckBox;
    }
}
