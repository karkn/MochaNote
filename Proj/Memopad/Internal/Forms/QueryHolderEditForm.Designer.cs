namespace Mkamo.Memopad.Internal.Forms {
    partial class QueryHolderEditForm {
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
            this._flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._nameLabel = new System.Windows.Forms.Label();
            this._nameTextBox = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this._searchLabel = new System.Windows.Forms.Label();
            this._searchTextBox = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this._titleLabel = new System.Windows.Forms.Label();
            this._titleTextBox = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this._tagLabel = new System.Windows.Forms.Label();
            this._tagTextBox = new Mkamo.Memopad.Internal.Controls.TagSelectTextBox();
            this._importanceLabel = new System.Windows.Forms.Label();
            this._importanceTextBox = new Mkamo.Memopad.Internal.Controls.ImportanceSelectTextBox();
            this._markLabel = new System.Windows.Forms.Label();
            this._markTextBox = new Mkamo.Memopad.Internal.Controls.MarkSelectTextBox();
            this._timeSpanLabel = new System.Windows.Forms.Label();
            this._timeSpanTextBox = new Mkamo.Memopad.Internal.Controls.TimeSpanPickTextBox();
            this._okButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._flowLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _flowLayoutPanel
            // 
            this._flowLayoutPanel.BackColor = System.Drawing.SystemColors.Control;
            this._flowLayoutPanel.Controls.Add(this._nameLabel);
            this._flowLayoutPanel.Controls.Add(this._nameTextBox);
            this._flowLayoutPanel.Controls.Add(this._searchLabel);
            this._flowLayoutPanel.Controls.Add(this._searchTextBox);
            this._flowLayoutPanel.Controls.Add(this._titleLabel);
            this._flowLayoutPanel.Controls.Add(this._titleTextBox);
            this._flowLayoutPanel.Controls.Add(this._tagLabel);
            this._flowLayoutPanel.Controls.Add(this._tagTextBox);
            this._flowLayoutPanel.Controls.Add(this._importanceLabel);
            this._flowLayoutPanel.Controls.Add(this._importanceTextBox);
            this._flowLayoutPanel.Controls.Add(this._markLabel);
            this._flowLayoutPanel.Controls.Add(this._markTextBox);
            this._flowLayoutPanel.Controls.Add(this._timeSpanLabel);
            this._flowLayoutPanel.Controls.Add(this._timeSpanTextBox);
            this._flowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this._flowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this._flowLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this._flowLayoutPanel.Name = "_flowLayoutPanel";
            this._flowLayoutPanel.Padding = new System.Windows.Forms.Padding(5);
            this._flowLayoutPanel.Size = new System.Drawing.Size(309, 306);
            this._flowLayoutPanel.TabIndex = 0;
            // 
            // _nameLabel
            // 
            this._nameLabel.AutoSize = true;
            this._nameLabel.ForeColor = System.Drawing.SystemColors.WindowText;
            this._nameLabel.Location = new System.Drawing.Point(8, 5);
            this._nameLabel.Name = "_nameLabel";
            this._nameLabel.Size = new System.Drawing.Size(106, 12);
            this._nameLabel.TabIndex = 0;
            this._nameLabel.Text = "スマートフォルダ名(&N):";
            // 
            // _nameTextBox
            // 
            this._nameTextBox.Location = new System.Drawing.Point(8, 20);
            this._nameTextBox.Name = "_nameTextBox";
            this._nameTextBox.Size = new System.Drawing.Size(280, 23);
            this._nameTextBox.StateCommon.Border.Color1 = System.Drawing.Color.Gray;
            this._nameTextBox.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders) ((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom)
                        | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left)
                        | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this._nameTextBox.TabIndex = 1;
            // 
            // _searchLabel
            // 
            this._searchLabel.AutoSize = true;
            this._searchLabel.ForeColor = System.Drawing.SystemColors.WindowText;
            this._searchLabel.Location = new System.Drawing.Point(8, 46);
            this._searchLabel.Name = "_searchLabel";
            this._searchLabel.Size = new System.Drawing.Size(82, 12);
            this._searchLabel.TabIndex = 2;
            this._searchLabel.Text = "検索文字列(&S):";
            // 
            // _searchTextBox
            // 
            this._searchTextBox.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._searchTextBox.Location = new System.Drawing.Point(8, 61);
            this._searchTextBox.Name = "_searchTextBox";
            this._searchTextBox.Size = new System.Drawing.Size(280, 23);
            this._searchTextBox.StateCommon.Border.Color1 = System.Drawing.Color.Gray;
            this._searchTextBox.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders) ((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom)
                        | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left)
                        | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this._searchTextBox.TabIndex = 3;
            // 
            // _titleLabel
            // 
            this._titleLabel.AutoSize = true;
            this._titleLabel.ForeColor = System.Drawing.SystemColors.WindowText;
            this._titleLabel.Location = new System.Drawing.Point(8, 87);
            this._titleLabel.Name = "_titleLabel";
            this._titleLabel.Size = new System.Drawing.Size(57, 12);
            this._titleLabel.TabIndex = 4;
            this._titleLabel.Text = "タイトル(&T):";
            // 
            // _titleTextBox
            // 
            this._titleTextBox.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._titleTextBox.Location = new System.Drawing.Point(8, 102);
            this._titleTextBox.Name = "_titleTextBox";
            this._titleTextBox.Size = new System.Drawing.Size(280, 23);
            this._titleTextBox.StateCommon.Border.Color1 = System.Drawing.Color.Gray;
            this._titleTextBox.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders) ((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom)
                        | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left)
                        | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this._titleTextBox.TabIndex = 5;
            // 
            // _tagLabel
            // 
            this._tagLabel.AutoSize = true;
            this._tagLabel.ForeColor = System.Drawing.SystemColors.WindowText;
            this._tagLabel.Location = new System.Drawing.Point(8, 128);
            this._tagLabel.Name = "_tagLabel";
            this._tagLabel.Size = new System.Drawing.Size(39, 12);
            this._tagLabel.TabIndex = 6;
            this._tagLabel.Text = "タグ(&T):";
            // 
            // _tagTextBox
            // 
            this._tagTextBox.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._tagTextBox.CheckedTags = new Mkamo.Model.Memo.MemoTag[0];
            this._tagTextBox.IsAnyChecked = true;
            this._tagTextBox.IsUntaggedChecked = false;
            this._tagTextBox.Location = new System.Drawing.Point(8, 143);
            this._tagTextBox.Name = "_tagTextBox";
            this._tagTextBox.ReadOnly = true;
            this._tagTextBox.Size = new System.Drawing.Size(280, 24);
            this._tagTextBox.StateCommon.Border.Color1 = System.Drawing.Color.Gray;
            this._tagTextBox.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders) ((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom)
                        | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left)
                        | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this._tagTextBox.TabIndex = 7;
            this._tagTextBox.Text = "条件なし";
            // 
            // _importanceLabel
            // 
            this._importanceLabel.AutoSize = true;
            this._importanceLabel.ForeColor = System.Drawing.SystemColors.WindowText;
            this._importanceLabel.Location = new System.Drawing.Point(8, 170);
            this._importanceLabel.Name = "_importanceLabel";
            this._importanceLabel.Size = new System.Drawing.Size(54, 12);
            this._importanceLabel.TabIndex = 8;
            this._importanceLabel.Text = "重要度(&I):";
            // 
            // _importanceTextBox
            // 
            this._importanceTextBox.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._importanceTextBox.CheckedImportanceKinds = new Mkamo.Model.Memo.MemoImportanceKind[0];
            this._importanceTextBox.Location = new System.Drawing.Point(8, 185);
            this._importanceTextBox.Name = "_importanceTextBox";
            this._importanceTextBox.ReadOnly = true;
            this._importanceTextBox.Size = new System.Drawing.Size(280, 24);
            this._importanceTextBox.StateCommon.Border.Color1 = System.Drawing.Color.Gray;
            this._importanceTextBox.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders) ((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom)
                        | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left)
                        | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this._importanceTextBox.TabIndex = 9;
            this._importanceTextBox.Text = "条件なし";
            // 
            // _markLabel
            // 
            this._markLabel.AutoSize = true;
            this._markLabel.ForeColor = System.Drawing.SystemColors.WindowText;
            this._markLabel.Location = new System.Drawing.Point(8, 212);
            this._markLabel.Name = "_markLabel";
            this._markLabel.Size = new System.Drawing.Size(51, 12);
            this._markLabel.TabIndex = 10;
            this._markLabel.Text = "マーク(&M):";
            // 
            // _markTextBox
            // 
            this._markTextBox.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._markTextBox.CheckedMarkKinds = new Mkamo.Model.Memo.MemoMarkKind[0];
            this._markTextBox.IsAnyChecked = true;
            this._markTextBox.Location = new System.Drawing.Point(8, 227);
            this._markTextBox.Name = "_markTextBox";
            this._markTextBox.ReadOnly = true;
            this._markTextBox.Size = new System.Drawing.Size(280, 24);
            this._markTextBox.StateCommon.Border.Color1 = System.Drawing.Color.Gray;
            this._markTextBox.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders) ((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom)
                        | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left)
                        | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this._markTextBox.TabIndex = 11;
            this._markTextBox.Text = "条件なし";
            // 
            // _timeSpanLabel
            // 
            this._timeSpanLabel.AutoSize = true;
            this._timeSpanLabel.ForeColor = System.Drawing.SystemColors.WindowText;
            this._timeSpanLabel.Location = new System.Drawing.Point(8, 254);
            this._timeSpanLabel.Name = "_timeSpanLabel";
            this._timeSpanLabel.Size = new System.Drawing.Size(46, 12);
            this._timeSpanLabel.TabIndex = 12;
            this._timeSpanLabel.Text = "期間(&P):";
            // 
            // _timeSpanTextBox
            // 
            this._timeSpanTextBox.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._timeSpanTextBox.Location = new System.Drawing.Point(8, 269);
            this._timeSpanTextBox.Name = "_timeSpanTextBox";
            this._timeSpanTextBox.ReadOnly = true;
            this._timeSpanTextBox.Size = new System.Drawing.Size(280, 24);
            this._timeSpanTextBox.StateCommon.Border.Color1 = System.Drawing.Color.Gray;
            this._timeSpanTextBox.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders) ((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom)
                        | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left)
                        | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this._timeSpanTextBox.TabIndex = 13;
            this._timeSpanTextBox.Text = "条件なし";
            // 
            // _okButton
            // 
            this._okButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._okButton.Location = new System.Drawing.Point(133, 315);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(75, 23);
            this._okButton.TabIndex = 1;
            this._okButton.Text = "OK(&O)";
            this._okButton.UseVisualStyleBackColor = true;
            this._okButton.Click += new System.EventHandler(this._okButton_Click);
            // 
            // _cancelButton
            // 
            this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.Location = new System.Drawing.Point(214, 315);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(83, 23);
            this._cancelButton.TabIndex = 2;
            this._cancelButton.Text = "キャンセル(&C)";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // QueryHolderEditForm
            // 
            this.AcceptButton = this._okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.ClientSize = new System.Drawing.Size(309, 350);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._okButton);
            this.Controls.Add(this._flowLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "QueryHolderEditForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "スマートフォルダの設定";
            this._flowLayoutPanel.ResumeLayout(false);
            this._flowLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel _flowLayoutPanel;
        private System.Windows.Forms.Label _nameLabel;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox _nameTextBox;
        private System.Windows.Forms.Label _searchLabel;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox _searchTextBox;
        private System.Windows.Forms.Label _titleLabel;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox _titleTextBox;
        private System.Windows.Forms.Label _tagLabel;
        private Mkamo.Memopad.Internal.Controls.TagSelectTextBox _tagTextBox;
        private System.Windows.Forms.Label _markLabel;
        private Mkamo.Memopad.Internal.Controls.MarkSelectTextBox _markTextBox;
        private System.Windows.Forms.Label _timeSpanLabel;
        private Mkamo.Memopad.Internal.Controls.TimeSpanPickTextBox _timeSpanTextBox;
        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Label _importanceLabel;
        private Mkamo.Memopad.Internal.Controls.ImportanceSelectTextBox _importanceTextBox;
    }
}
