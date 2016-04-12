namespace Mkamo.Memopad.Internal.Controls {
    partial class MemoQueryBuilderView {
        /// <summary> 
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナで生成されたコード

        /// <summary> 
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent() {
            this._tagLabel = new System.Windows.Forms.Label();
            this._titleLabel = new System.Windows.Forms.Label();
            this._flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._titleTextBox = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this._tagTextBox = new Mkamo.Memopad.Internal.Controls.TagSelectTextBox();
            this._importanceLabel = new System.Windows.Forms.Label();
            this._importanceTextBox = new Mkamo.Memopad.Internal.Controls.ImportanceSelectTextBox();
            this._markLabel = new System.Windows.Forms.Label();
            this._markTextBox = new Mkamo.Memopad.Internal.Controls.MarkSelectTextBox();
            this._timeSpanLabel = new System.Windows.Forms.Label();
            this._timeSpanTextBox = new Mkamo.Memopad.Internal.Controls.TimeSpanPickTextBox();
            this._saveAsSmartFolderButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this._saveAsSmartFilterButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this._returnButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this._flowLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _tagLabel
            // 
            this._tagLabel.AutoSize = true;
            this._tagLabel.ForeColor = System.Drawing.Color.Navy;
            this._tagLabel.Location = new System.Drawing.Point(8, 50);
            this._tagLabel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 0);
            this._tagLabel.Name = "_tagLabel";
            this._tagLabel.Size = new System.Drawing.Size(39, 12);
            this._tagLabel.TabIndex = 2;
            this._tagLabel.Text = "タグ(&T):";
            // 
            // _titleLabel
            // 
            this._titleLabel.AutoSize = true;
            this._titleLabel.ForeColor = System.Drawing.Color.Navy;
            this._titleLabel.Location = new System.Drawing.Point(8, 5);
            this._titleLabel.Name = "_titleLabel";
            this._titleLabel.Size = new System.Drawing.Size(58, 12);
            this._titleLabel.TabIndex = 0;
            this._titleLabel.Text = "タイトル(&N):";
            // 
            // _flowLayoutPanel
            // 
            this._flowLayoutPanel.AutoSize = true;
            this._flowLayoutPanel.BackColor = System.Drawing.Color.WhiteSmoke;
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
            this._flowLayoutPanel.Controls.Add(this._saveAsSmartFolderButton);
            this._flowLayoutPanel.Controls.Add(this._saveAsSmartFilterButton);
            this._flowLayoutPanel.Controls.Add(this._returnButton);
            this._flowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._flowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this._flowLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this._flowLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this._flowLayoutPanel.Name = "_flowLayoutPanel";
            this._flowLayoutPanel.Padding = new System.Windows.Forms.Padding(5);
            this._flowLayoutPanel.Size = new System.Drawing.Size(216, 337);
            this._flowLayoutPanel.TabIndex = 0;
            // 
            // _titleTextBox
            // 
            this._titleTextBox.Location = new System.Drawing.Point(8, 20);
            this._titleTextBox.Name = "_titleTextBox";
            this._titleTextBox.Size = new System.Drawing.Size(117, 23);
            this._titleTextBox.TabIndex = 1;
            this._titleTextBox.TextChanged += new System.EventHandler(this._titleTextBox_TextChanged);
            // 
            // _tagTextBox
            // 
            this._tagTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._tagTextBox.CheckedTags = new Mkamo.Model.Memo.MemoTag[0];
            this._tagTextBox.IsAnyChecked = true;
            this._tagTextBox.IsUntaggedChecked = false;
            this._tagTextBox.Location = new System.Drawing.Point(8, 65);
            this._tagTextBox.Name = "_tagTextBox";
            this._tagTextBox.ReadOnly = true;
            this._tagTextBox.Size = new System.Drawing.Size(141, 24);
            this._tagTextBox.StateCommon.Content.Color1 = System.Drawing.SystemColors.GrayText;
            this._tagTextBox.TabIndex = 3;
            this._tagTextBox.Text = "条件なし";
            // 
            // _importanceLabel
            // 
            this._importanceLabel.AutoSize = true;
            this._importanceLabel.ForeColor = System.Drawing.Color.Navy;
            this._importanceLabel.Location = new System.Drawing.Point(8, 96);
            this._importanceLabel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 0);
            this._importanceLabel.Name = "_importanceLabel";
            this._importanceLabel.Size = new System.Drawing.Size(54, 12);
            this._importanceLabel.TabIndex = 4;
            this._importanceLabel.Text = "重要度(&I):";
            // 
            // _importanceTextBox
            // 
            this._importanceTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._importanceTextBox.CheckedImportanceKinds = new Mkamo.Model.Memo.MemoImportanceKind[0];
            this._importanceTextBox.Location = new System.Drawing.Point(8, 111);
            this._importanceTextBox.Name = "_importanceTextBox";
            this._importanceTextBox.ReadOnly = true;
            this._importanceTextBox.Size = new System.Drawing.Size(141, 24);
            this._importanceTextBox.StateCommon.Content.Color1 = System.Drawing.SystemColors.GrayText;
            this._importanceTextBox.TabIndex = 5;
            this._importanceTextBox.Text = "条件なし";
            // 
            // _markLabel
            // 
            this._markLabel.AutoSize = true;
            this._markLabel.ForeColor = System.Drawing.Color.Navy;
            this._markLabel.Location = new System.Drawing.Point(8, 142);
            this._markLabel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 0);
            this._markLabel.Name = "_markLabel";
            this._markLabel.Size = new System.Drawing.Size(51, 12);
            this._markLabel.TabIndex = 6;
            this._markLabel.Text = "マーク(&M):";
            // 
            // _markTextBox
            // 
            this._markTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._markTextBox.CheckedMarkKinds = new Mkamo.Model.Memo.MemoMarkKind[0];
            this._markTextBox.IsAnyChecked = true;
            this._markTextBox.Location = new System.Drawing.Point(8, 157);
            this._markTextBox.Name = "_markTextBox";
            this._markTextBox.ReadOnly = true;
            this._markTextBox.Size = new System.Drawing.Size(141, 24);
            this._markTextBox.StateCommon.Content.Color1 = System.Drawing.SystemColors.GrayText;
            this._markTextBox.TabIndex = 7;
            this._markTextBox.Text = "条件なし";
            // 
            // _timeSpanLabel
            // 
            this._timeSpanLabel.AutoSize = true;
            this._timeSpanLabel.ForeColor = System.Drawing.Color.Navy;
            this._timeSpanLabel.Location = new System.Drawing.Point(8, 188);
            this._timeSpanLabel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 0);
            this._timeSpanLabel.Name = "_timeSpanLabel";
            this._timeSpanLabel.Size = new System.Drawing.Size(47, 12);
            this._timeSpanLabel.TabIndex = 8;
            this._timeSpanLabel.Text = "期間(&D):";
            // 
            // _timeSpanTextBox
            // 
            this._timeSpanTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._timeSpanTextBox.Location = new System.Drawing.Point(8, 203);
            this._timeSpanTextBox.Name = "_timeSpanTextBox";
            this._timeSpanTextBox.ReadOnly = true;
            this._timeSpanTextBox.Size = new System.Drawing.Size(141, 24);
            this._timeSpanTextBox.TabIndex = 9;
            this._timeSpanTextBox.Text = "条件なし";
            // 
            // _saveAsSmartFolderButton
            // 
            this._saveAsSmartFolderButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this._saveAsSmartFolderButton.Location = new System.Drawing.Point(8, 238);
            this._saveAsSmartFolderButton.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
            this._saveAsSmartFolderButton.Name = "_saveAsSmartFolderButton";
            this._saveAsSmartFolderButton.Size = new System.Drawing.Size(141, 22);
            this._saveAsSmartFolderButton.StateCommon.Content.Padding = new System.Windows.Forms.Padding(0);
            this._saveAsSmartFolderButton.TabIndex = 10;
            this._saveAsSmartFolderButton.Values.Text = "スマートフォルダに保存(&S)";
            this._saveAsSmartFolderButton.Visible = false;
            this._saveAsSmartFolderButton.Click += new System.EventHandler(this._saveAsSmartFolderButton_Click);
            // 
            // _saveAsSmartFilterButton
            // 
            this._saveAsSmartFilterButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this._saveAsSmartFilterButton.Location = new System.Drawing.Point(8, 271);
            this._saveAsSmartFilterButton.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
            this._saveAsSmartFilterButton.Name = "_saveAsSmartFilterButton";
            this._saveAsSmartFilterButton.Size = new System.Drawing.Size(141, 22);
            this._saveAsSmartFilterButton.StateCommon.Content.Padding = new System.Windows.Forms.Padding(0);
            this._saveAsSmartFilterButton.TabIndex = 11;
            this._saveAsSmartFilterButton.Values.Text = "スマートフィルタに保存(&F)";
            this._saveAsSmartFilterButton.Visible = false;
            this._saveAsSmartFilterButton.Click += new System.EventHandler(this._saveAsSmartFilterButton_Click);
            // 
            // _returnButton
            // 
            this._returnButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this._returnButton.Location = new System.Drawing.Point(97, 304);
            this._returnButton.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
            this._returnButton.Name = "_returnButton";
            this._returnButton.Size = new System.Drawing.Size(52, 22);
            this._returnButton.StateCommon.Content.Padding = new System.Windows.Forms.Padding(0);
            this._returnButton.TabIndex = 12;
            this._returnButton.Values.Text = "戻る(&R)";
            this._returnButton.Click += new System.EventHandler(this._returnButton_Click);
            // 
            // MemoQueryBuilderView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this._flowLayoutPanel);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "MemoQueryBuilderView";
            this.Size = new System.Drawing.Size(216, 337);
            this._flowLayoutPanel.ResumeLayout(false);
            this._flowLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _tagLabel;
        private System.Windows.Forms.Label _titleLabel;
        private System.Windows.Forms.FlowLayoutPanel _flowLayoutPanel;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox _titleTextBox;
        private System.Windows.Forms.Label _markLabel;
        private System.Windows.Forms.Label _timeSpanLabel;
        private ComponentFactory.Krypton.Toolkit.KryptonButton _saveAsSmartFolderButton;
        private TagSelectTextBox _tagTextBox;
        private MarkSelectTextBox _markTextBox;
        private TimeSpanPickTextBox _timeSpanTextBox;
        private ComponentFactory.Krypton.Toolkit.KryptonButton _saveAsSmartFilterButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton _returnButton;
        private System.Windows.Forms.Label _importanceLabel;
        private ImportanceSelectTextBox _importanceTextBox;

    }
}
