namespace Mkamo.Memopad.Internal.Forms {
    partial class CreateMemoForm {
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
            this._createButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._titleLabel = new System.Windows.Forms.Label();
            this._titleTextBox = new System.Windows.Forms.TextBox();
            this._originalMemoGroupBox = new System.Windows.Forms.GroupBox();
            this._replaceWithLinkRadioButton = new System.Windows.Forms.RadioButton();
            this._removeSelectionRadioButton = new System.Windows.Forms.RadioButton();
            this._doNothingRadioButton = new System.Windows.Forms.RadioButton();
            this._originalMemoGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // _createButton
            // 
            this._createButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._createButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._createButton.Location = new System.Drawing.Point(229, 132);
            this._createButton.Name = "_createButton";
            this._createButton.Size = new System.Drawing.Size(75, 23);
            this._createButton.TabIndex = 3;
            this._createButton.Text = "作成(&N)";
            this._createButton.UseVisualStyleBackColor = true;
            // 
            // _cancelButton
            // 
            this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.Location = new System.Drawing.Point(310, 132);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(86, 23);
            this._cancelButton.TabIndex = 4;
            this._cancelButton.Text = "キャンセル(&C)";
            this._cancelButton.UseVisualStyleBackColor = true;
            // 
            // _titleLabel
            // 
            this._titleLabel.AutoSize = true;
            this._titleLabel.Location = new System.Drawing.Point(12, 9);
            this._titleLabel.Name = "_titleLabel";
            this._titleLabel.Size = new System.Drawing.Size(57, 12);
            this._titleLabel.TabIndex = 0;
            this._titleLabel.Text = "タイトル(&T):";
            // 
            // _titleTextBox
            // 
            this._titleTextBox.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._titleTextBox.Location = new System.Drawing.Point(75, 6);
            this._titleTextBox.Name = "_titleTextBox";
            this._titleTextBox.Size = new System.Drawing.Size(321, 19);
            this._titleTextBox.TabIndex = 1;
            this._titleTextBox.TextChanged += new System.EventHandler(this._titleTextBox_TextChanged);
            // 
            // _originalMemoGroupBox
            // 
            this._originalMemoGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._originalMemoGroupBox.Controls.Add(this._replaceWithLinkRadioButton);
            this._originalMemoGroupBox.Controls.Add(this._removeSelectionRadioButton);
            this._originalMemoGroupBox.Controls.Add(this._doNothingRadioButton);
            this._originalMemoGroupBox.Location = new System.Drawing.Point(14, 31);
            this._originalMemoGroupBox.Name = "_originalMemoGroupBox";
            this._originalMemoGroupBox.Size = new System.Drawing.Size(382, 87);
            this._originalMemoGroupBox.TabIndex = 2;
            this._originalMemoGroupBox.TabStop = false;
            this._originalMemoGroupBox.Text = "元のノート(&O):";
            // 
            // _replaceWithLinkRadioButton
            // 
            this._replaceWithLinkRadioButton.AutoSize = true;
            this._replaceWithLinkRadioButton.Location = new System.Drawing.Point(6, 62);
            this._replaceWithLinkRadioButton.Name = "_replaceWithLinkRadioButton";
            this._replaceWithLinkRadioButton.Size = new System.Drawing.Size(232, 16);
            this._replaceWithLinkRadioButton.TabIndex = 2;
            this._replaceWithLinkRadioButton.TabStop = true;
            this._replaceWithLinkRadioButton.Text = "選択部分を新しいノートへのリンクに置き換える";
            this._replaceWithLinkRadioButton.UseVisualStyleBackColor = true;
            // 
            // _removeSelectionRadioButton
            // 
            this._removeSelectionRadioButton.AutoSize = true;
            this._removeSelectionRadioButton.Location = new System.Drawing.Point(6, 40);
            this._removeSelectionRadioButton.Name = "_removeSelectionRadioButton";
            this._removeSelectionRadioButton.Size = new System.Drawing.Size(123, 16);
            this._removeSelectionRadioButton.TabIndex = 1;
            this._removeSelectionRadioButton.TabStop = true;
            this._removeSelectionRadioButton.Text = "選択部分を削除する";
            this._removeSelectionRadioButton.UseVisualStyleBackColor = true;
            // 
            // _doNothingRadioButton
            // 
            this._doNothingRadioButton.AutoSize = true;
            this._doNothingRadioButton.Checked = true;
            this._doNothingRadioButton.Location = new System.Drawing.Point(6, 18);
            this._doNothingRadioButton.Name = "_doNothingRadioButton";
            this._doNothingRadioButton.Size = new System.Drawing.Size(73, 16);
            this._doNothingRadioButton.TabIndex = 0;
            this._doNothingRadioButton.TabStop = true;
            this._doNothingRadioButton.Text = "何もしない";
            this._doNothingRadioButton.UseVisualStyleBackColor = true;
            // 
            // CreateMemoForm
            // 
            this.AcceptButton = this._createButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.ClientSize = new System.Drawing.Size(408, 167);
            this.Controls.Add(this._originalMemoGroupBox);
            this.Controls.Add(this._titleTextBox);
            this.Controls.Add(this._titleLabel);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._createButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateMemoForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ノートの作成";
            this._originalMemoGroupBox.ResumeLayout(false);
            this._originalMemoGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _createButton;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Label _titleLabel;
        private System.Windows.Forms.TextBox _titleTextBox;
        private System.Windows.Forms.GroupBox _originalMemoGroupBox;
        private System.Windows.Forms.RadioButton _replaceWithLinkRadioButton;
        private System.Windows.Forms.RadioButton _removeSelectionRadioButton;
        private System.Windows.Forms.RadioButton _doNothingRadioButton;
    }
}
