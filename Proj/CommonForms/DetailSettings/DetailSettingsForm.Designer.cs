namespace Mkamo.Common.Forms.DetailSettings {
    partial class DetailSettingsForm {
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
            this._cancelButton = new System.Windows.Forms.Button();
            this._pageSelectorListBox = new System.Windows.Forms.ListBox();
            this._okButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _cancelButton
            // 
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.Location = new System.Drawing.Point(312, 238);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(85, 23);
            this._cancelButton.TabIndex = 2;
            this._cancelButton.Text = "キャンセル(&C)";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _pageSelectorListBox
            // 
            this._pageSelectorListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._pageSelectorListBox.FormattingEnabled = true;
            this._pageSelectorListBox.IntegralHeight = false;
            this._pageSelectorListBox.ItemHeight = 12;
            this._pageSelectorListBox.Location = new System.Drawing.Point(12, 12);
            this._pageSelectorListBox.Name = "_pageSelectorListBox";
            this._pageSelectorListBox.Size = new System.Drawing.Size(80, 216);
            this._pageSelectorListBox.TabIndex = 0;
            // 
            // _okButton
            // 
            this._okButton.Location = new System.Drawing.Point(225, 238);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(75, 23);
            this._okButton.TabIndex = 1;
            this._okButton.Text = "OK(&O)";
            this._okButton.UseVisualStyleBackColor = true;
            this._okButton.Click += new System.EventHandler(this._okButton_Click);
            // 
            // DetailForm
            // 
            this.AcceptButton = this._okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.ClientSize = new System.Drawing.Size(409, 273);
            this.Controls.Add(this._okButton);
            this.Controls.Add(this._pageSelectorListBox);
            this.Controls.Add(this._cancelButton);
            this.DoubleBuffered = true;
            this.Name = "DetailForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "詳細設定";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.ListBox _pageSelectorListBox;
    }
}
