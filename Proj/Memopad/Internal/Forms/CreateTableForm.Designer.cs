namespace Mkamo.Memopad.Internal.Forms {
    partial class CreateTableForm {
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
            this._columnCountComboBox = new System.Windows.Forms.ComboBox();
            this._rowCountComboBox = new System.Windows.Forms.ComboBox();
            this._columnCountLabel = new System.Windows.Forms.Label();
            this._rowCountLabel = new System.Windows.Forms.Label();
            this._okButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _columnCountComboBox
            // 
            this._columnCountComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._columnCountComboBox.FormattingEnabled = true;
            this._columnCountComboBox.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10"});
            this._columnCountComboBox.Location = new System.Drawing.Point(65, 6);
            this._columnCountComboBox.Name = "_columnCountComboBox";
            this._columnCountComboBox.Size = new System.Drawing.Size(97, 20);
            this._columnCountComboBox.TabIndex = 1;
            // 
            // _rowCountComboBox
            // 
            this._rowCountComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._rowCountComboBox.FormattingEnabled = true;
            this._rowCountComboBox.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10"});
            this._rowCountComboBox.Location = new System.Drawing.Point(65, 32);
            this._rowCountComboBox.Name = "_rowCountComboBox";
            this._rowCountComboBox.Size = new System.Drawing.Size(97, 20);
            this._rowCountComboBox.TabIndex = 3;
            // 
            // _columnCountLabel
            // 
            this._columnCountLabel.AutoSize = true;
            this._columnCountLabel.Location = new System.Drawing.Point(12, 9);
            this._columnCountLabel.Name = "_columnCountLabel";
            this._columnCountLabel.Size = new System.Drawing.Size(47, 12);
            this._columnCountLabel.TabIndex = 0;
            this._columnCountLabel.Text = "列数(&C):";
            // 
            // _rowCountLabel
            // 
            this._rowCountLabel.AutoSize = true;
            this._rowCountLabel.Location = new System.Drawing.Point(12, 35);
            this._rowCountLabel.Name = "_rowCountLabel";
            this._rowCountLabel.Size = new System.Drawing.Size(47, 12);
            this._rowCountLabel.TabIndex = 2;
            this._rowCountLabel.Text = "行数(&R):";
            // 
            // _okButton
            // 
            this._okButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._okButton.Location = new System.Drawing.Point(44, 66);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(75, 23);
            this._okButton.TabIndex = 4;
            this._okButton.Text = "OK(&O)";
            this._okButton.UseVisualStyleBackColor = true;
            this._okButton.Click += new System.EventHandler(this._okButton_Click);
            // 
            // _cancelButton
            // 
            this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.Location = new System.Drawing.Point(125, 66);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(88, 23);
            this._cancelButton.TabIndex = 5;
            this._cancelButton.Text = "キャンセル(&C)";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // CreateTableForm
            // 
            this.AcceptButton = this._okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.ClientSize = new System.Drawing.Size(225, 98);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._okButton);
            this.Controls.Add(this._rowCountLabel);
            this.Controls.Add(this._columnCountLabel);
            this.Controls.Add(this._rowCountComboBox);
            this.Controls.Add(this._columnCountComboBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateTableForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "表の追加";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox _columnCountComboBox;
        private System.Windows.Forms.ComboBox _rowCountComboBox;
        private System.Windows.Forms.Label _columnCountLabel;
        private System.Windows.Forms.Label _rowCountLabel;
        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.Button _cancelButton;
    }
}
