namespace Mkamo.Memopad.Internal.Forms {
    partial class SendMailForm {
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
            this._fromLabel = new System.Windows.Forms.Label();
            this._toLabel = new System.Windows.Forms.Label();
            this._fromTextBox = new System.Windows.Forms.TextBox();
            this._subjectLabel = new System.Windows.Forms.Label();
            this._subjectTextBox = new System.Windows.Forms.TextBox();
            this._bodyLabel = new System.Windows.Forms.Label();
            this._bodyComboBox = new System.Windows.Forms.ComboBox();
            this._sendButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._smtpServerLabel = new System.Windows.Forms.Label();
            this._smtpServerTextBox = new System.Windows.Forms.TextBox();
            this._toTextBox = new System.Windows.Forms.TextBox();
            this._smtpDetailButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _fromLabel
            // 
            this._fromLabel.AutoSize = true;
            this._fromLabel.Location = new System.Drawing.Point(12, 70);
            this._fromLabel.Name = "_fromLabel";
            this._fromLabel.Size = new System.Drawing.Size(122, 12);
            this._fromLabel.TabIndex = 5;
            this._fromLabel.Text = "送信者メールアドレス(&F):";
            // 
            // _toLabel
            // 
            this._toLabel.AutoSize = true;
            this._toLabel.Location = new System.Drawing.Point(12, 44);
            this._toLabel.Name = "_toLabel";
            this._toLabel.Size = new System.Drawing.Size(122, 12);
            this._toLabel.TabIndex = 3;
            this._toLabel.Text = "送信先メールアドレス(&T):";
            // 
            // _fromTextBox
            // 
            this._fromTextBox.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._fromTextBox.Location = new System.Drawing.Point(140, 67);
            this._fromTextBox.Name = "_fromTextBox";
            this._fromTextBox.Size = new System.Drawing.Size(318, 19);
            this._fromTextBox.TabIndex = 6;
            // 
            // _subjectLabel
            // 
            this._subjectLabel.AutoSize = true;
            this._subjectLabel.Location = new System.Drawing.Point(12, 99);
            this._subjectLabel.Name = "_subjectLabel";
            this._subjectLabel.Size = new System.Drawing.Size(46, 12);
            this._subjectLabel.TabIndex = 7;
            this._subjectLabel.Text = "件名(&S):";
            // 
            // _subjectTextBox
            // 
            this._subjectTextBox.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._subjectTextBox.Location = new System.Drawing.Point(76, 96);
            this._subjectTextBox.Name = "_subjectTextBox";
            this._subjectTextBox.Size = new System.Drawing.Size(382, 19);
            this._subjectTextBox.TabIndex = 8;
            // 
            // _bodyLabel
            // 
            this._bodyLabel.AutoSize = true;
            this._bodyLabel.Location = new System.Drawing.Point(12, 124);
            this._bodyLabel.Name = "_bodyLabel";
            this._bodyLabel.Size = new System.Drawing.Size(47, 12);
            this._bodyLabel.TabIndex = 9;
            this._bodyLabel.Text = "本文(&B):";
            // 
            // _bodyComboBox
            // 
            this._bodyComboBox.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._bodyComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._bodyComboBox.Items.AddRange(new object[] {
            "テキストのみ",
            "画像のみ",
            "テキストと画像"});
            this._bodyComboBox.Location = new System.Drawing.Point(76, 121);
            this._bodyComboBox.Name = "_bodyComboBox";
            this._bodyComboBox.Size = new System.Drawing.Size(382, 20);
            this._bodyComboBox.TabIndex = 10;
            // 
            // _sendButton
            // 
            this._sendButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._sendButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._sendButton.Location = new System.Drawing.Point(290, 157);
            this._sendButton.Name = "_sendButton";
            this._sendButton.Size = new System.Drawing.Size(75, 23);
            this._sendButton.TabIndex = 11;
            this._sendButton.Text = "送る(&S)";
            this._sendButton.UseVisualStyleBackColor = true;
            // 
            // _cancelButton
            // 
            this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.Location = new System.Drawing.Point(371, 157);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(87, 23);
            this._cancelButton.TabIndex = 12;
            this._cancelButton.Text = "キャンセル(&C)";
            this._cancelButton.UseVisualStyleBackColor = true;
            // 
            // _smtpServerLabel
            // 
            this._smtpServerLabel.AutoSize = true;
            this._smtpServerLabel.Location = new System.Drawing.Point(12, 15);
            this._smtpServerLabel.Name = "_smtpServerLabel";
            this._smtpServerLabel.Size = new System.Drawing.Size(82, 12);
            this._smtpServerLabel.TabIndex = 0;
            this._smtpServerLabel.Text = "SMTPサーバ(&S):";
            // 
            // _smtpServerTextBox
            // 
            this._smtpServerTextBox.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._smtpServerTextBox.Location = new System.Drawing.Point(100, 12);
            this._smtpServerTextBox.Name = "_smtpServerTextBox";
            this._smtpServerTextBox.Size = new System.Drawing.Size(266, 19);
            this._smtpServerTextBox.TabIndex = 1;
            // 
            // _toTextBox
            // 
            this._toTextBox.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._toTextBox.Location = new System.Drawing.Point(140, 41);
            this._toTextBox.Name = "_toTextBox";
            this._toTextBox.Size = new System.Drawing.Size(318, 19);
            this._toTextBox.TabIndex = 4;
            // 
            // _smtpDetailButton
            // 
            this._smtpDetailButton.Location = new System.Drawing.Point(368, 10);
            this._smtpDetailButton.Name = "_smtpDetailButton";
            this._smtpDetailButton.Size = new System.Drawing.Size(90, 23);
            this._smtpDetailButton.TabIndex = 2;
            this._smtpDetailButton.Text = "詳細設定(&C)...";
            this._smtpDetailButton.UseVisualStyleBackColor = true;
            this._smtpDetailButton.Click += new System.EventHandler(this._smtpDetailButton_Click);
            // 
            // SendMailForm
            // 
            this.AcceptButton = this._sendButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.ClientSize = new System.Drawing.Size(470, 192);
            this.Controls.Add(this._smtpDetailButton);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._sendButton);
            this.Controls.Add(this._bodyComboBox);
            this.Controls.Add(this._subjectTextBox);
            this.Controls.Add(this._smtpServerTextBox);
            this.Controls.Add(this._toTextBox);
            this.Controls.Add(this._fromTextBox);
            this.Controls.Add(this._bodyLabel);
            this.Controls.Add(this._subjectLabel);
            this.Controls.Add(this._smtpServerLabel);
            this.Controls.Add(this._toLabel);
            this.Controls.Add(this._fromLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SendMailForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "メールの送信";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _fromLabel;
        private System.Windows.Forms.Label _toLabel;
        private System.Windows.Forms.TextBox _fromTextBox;
        private System.Windows.Forms.Label _subjectLabel;
        private System.Windows.Forms.TextBox _subjectTextBox;
        private System.Windows.Forms.Label _bodyLabel;
        private System.Windows.Forms.ComboBox _bodyComboBox;
        private System.Windows.Forms.Button _sendButton;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Label _smtpServerLabel;
        private System.Windows.Forms.TextBox _smtpServerTextBox;
        private System.Windows.Forms.TextBox _toTextBox;
        private System.Windows.Forms.Button _smtpDetailButton;
    }
}
