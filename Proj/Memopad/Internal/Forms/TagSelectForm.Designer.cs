namespace Mkamo.Memopad.Internal.Forms {
    partial class TagSelectForm {
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
            this.components = new System.ComponentModel.Container();
            this._okButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._anyRadioButton = new System.Windows.Forms.RadioButton();
            this._allRadioButton = new System.Windows.Forms.RadioButton();
            this._tagTreePanel = new System.Windows.Forms.Panel();
            this._tagTree = new Mkamo.Memopad.Internal.Controls.TagTree();
            this._tagTreePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _okButton
            // 
            this._okButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._okButton.Location = new System.Drawing.Point(80, 420);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(75, 23);
            this._okButton.TabIndex = 3;
            this._okButton.Text = "OK(&O)";
            this._okButton.UseVisualStyleBackColor = true;
            this._okButton.Click += new System.EventHandler(this._okButton_Click);
            // 
            // _cancelButton
            // 
            this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.Location = new System.Drawing.Point(161, 420);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(94, 23);
            this._cancelButton.TabIndex = 4;
            this._cancelButton.Text = "キャンセル(&C)";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _anyRadioButton
            // 
            this._anyRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._anyRadioButton.AutoSize = true;
            this._anyRadioButton.Location = new System.Drawing.Point(12, 398);
            this._anyRadioButton.Name = "_anyRadioButton";
            this._anyRadioButton.Size = new System.Drawing.Size(111, 16);
            this._anyRadioButton.TabIndex = 1;
            this._anyRadioButton.TabStop = true;
            this._anyRadioButton.Text = "いずれかを含む(&N)";
            this._anyRadioButton.UseVisualStyleBackColor = true;
            // 
            // _allRadioButton
            // 
            this._allRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._allRadioButton.AutoSize = true;
            this._allRadioButton.Location = new System.Drawing.Point(129, 398);
            this._allRadioButton.Name = "_allRadioButton";
            this._allRadioButton.Size = new System.Drawing.Size(97, 16);
            this._allRadioButton.TabIndex = 2;
            this._allRadioButton.TabStop = true;
            this._allRadioButton.Text = "すべてを含む(&L)";
            this._allRadioButton.UseVisualStyleBackColor = true;
            // 
            // _tagTreePanel
            // 
            this._tagTreePanel.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._tagTreePanel.Controls.Add(this._tagTree);
            this._tagTreePanel.Location = new System.Drawing.Point(0, 0);
            this._tagTreePanel.Name = "_tagTreePanel";
            this._tagTreePanel.Padding = new System.Windows.Forms.Padding(1);
            this._tagTreePanel.Size = new System.Drawing.Size(267, 392);
            this._tagTreePanel.TabIndex = 5;
            // 
            // _tagTree
            // 
            this._tagTree.AllowDrop = true;
            this._tagTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tagTree.CheckBoxes = true;
            this._tagTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tagTree.ImageIndex = 0;
            this._tagTree.LabelEdit = true;
            this._tagTree.Location = new System.Drawing.Point(1, 1);
            this._tagTree.Name = "_tagTree";
            this._tagTree.SelectedImageIndex = 0;
            this._tagTree.Size = new System.Drawing.Size(265, 390);
            this._tagTree.Sorted = true;
            this._tagTree.TabIndex = 1;
            // 
            // TagSelectForm
            // 
            this.AcceptButton = this._okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.ClientSize = new System.Drawing.Size(267, 455);
            this.Controls.Add(this._tagTreePanel);
            this.Controls.Add(this._allRadioButton);
            this.Controls.Add(this._anyRadioButton);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._okButton);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TagSelectForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "タグの選択";
            this._tagTreePanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.RadioButton _anyRadioButton;
        private System.Windows.Forms.RadioButton _allRadioButton;
        private System.Windows.Forms.Panel _tagTreePanel;
        private Mkamo.Memopad.Internal.Controls.TagTree _tagTree;

    }
}
