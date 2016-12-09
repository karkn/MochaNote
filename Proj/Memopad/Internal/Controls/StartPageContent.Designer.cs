namespace Mkamo.Memopad.Internal.Controls {
    partial class StartPageContent {
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
            CleanUp();
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナで生成されたコード

        /// <summary> 
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent() {
            this._showOnStartCheckBox = new System.Windows.Forms.CheckBox();
            this._titleLabel = new System.Windows.Forms.Label();
            this._recentLabel = new System.Windows.Forms.Label();
            this.kryptonBorderEdge1 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this._manageTagsLinkLabel = new ComponentFactory.Krypton.Toolkit.KryptonLinkLabel();
            this._createMemoLinkLabel = new ComponentFactory.Krypton.Toolkit.KryptonLinkLabel();
            this._recentlyClosedListBox = new ComponentFactory.Krypton.Toolkit.KryptonListBox();
            this.kryptonBorderEdge2 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.SuspendLayout();
            // 
            // _showOnStartCheckBox
            // 
            this._showOnStartCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._showOnStartCheckBox.AutoSize = true;
            this._showOnStartCheckBox.Location = new System.Drawing.Point(25, 341);
            this._showOnStartCheckBox.Name = "_showOnStartCheckBox";
            this._showOnStartCheckBox.Size = new System.Drawing.Size(186, 16);
            this._showOnStartCheckBox.TabIndex = 4;
            this._showOnStartCheckBox.Text = "起動時にスタートページを表示する";
            this._showOnStartCheckBox.UseVisualStyleBackColor = true;
            this._showOnStartCheckBox.CheckedChanged += new System.EventHandler(this._showOnStartCheckBox_CheckedChanged);
            // 
            // _titleLabel
            // 
            this._titleLabel.AutoSize = true;
            this._titleLabel.BackColor = System.Drawing.Color.Transparent;
            this._titleLabel.Font = new System.Drawing.Font("MS UI Gothic", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._titleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(92)))), ((int)(((byte)(104)))));
            this._titleLabel.Location = new System.Drawing.Point(19, 14);
            this._titleLabel.Name = "_titleLabel";
            this._titleLabel.Size = new System.Drawing.Size(137, 22);
            this._titleLabel.TabIndex = 13;
            this._titleLabel.Text = "スタートページ";
            // 
            // _recentLabel
            // 
            this._recentLabel.AutoSize = true;
            this._recentLabel.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this._recentLabel.ForeColor = System.Drawing.Color.Gray;
            this._recentLabel.Location = new System.Drawing.Point(20, 128);
            this._recentLabel.Name = "_recentLabel";
            this._recentLabel.Size = new System.Drawing.Size(114, 16);
            this._recentLabel.TabIndex = 11;
            this._recentLabel.Text = "最近使ったノート";
            // 
            // kryptonBorderEdge1
            // 
            this.kryptonBorderEdge1.Location = new System.Drawing.Point(133, 138);
            this.kryptonBorderEdge1.Name = "kryptonBorderEdge1";
            this.kryptonBorderEdge1.Size = new System.Drawing.Size(190, 2);
            this.kryptonBorderEdge1.StateCommon.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(242)))), ((int)(((byte)(254)))));
            this.kryptonBorderEdge1.StateCommon.Width = 2;
            this.kryptonBorderEdge1.Text = "kryptonBorderEdge1";
            // 
            // _manageTagsLinkLabel
            // 
            this._manageTagsLinkLabel.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.Custom1;
            this._manageTagsLinkLabel.LinkBehavior = ComponentFactory.Krypton.Toolkit.KryptonLinkBehavior.HoverUnderline;
            this._manageTagsLinkLabel.Location = new System.Drawing.Point(29, 87);
            this._manageTagsLinkLabel.Name = "_manageTagsLinkLabel";
            this._manageTagsLinkLabel.Size = new System.Drawing.Size(90, 20);
            this._manageTagsLinkLabel.StateCommon.DrawFocus = ComponentFactory.Krypton.Toolkit.InheritBool.False;
            this._manageTagsLinkLabel.StateCommon.ShortText.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(95)))), ((int)(((byte)(235)))));
            this._manageTagsLinkLabel.TabIndex = 1;
            this._manageTagsLinkLabel.Values.Image = global::Mkamo.Memopad.Properties.Resources.tag_label;
            this._manageTagsLinkLabel.Values.Text = "タグを管理...";
            this._manageTagsLinkLabel.LinkClicked += new System.EventHandler(this._manageTagsLinkLabel_LinkClicked);
            // 
            // _createMemoLinkLabel
            // 
            this._createMemoLinkLabel.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.Custom1;
            this._createMemoLinkLabel.LinkBehavior = ComponentFactory.Krypton.Toolkit.KryptonLinkBehavior.HoverUnderline;
            this._createMemoLinkLabel.Location = new System.Drawing.Point(29, 61);
            this._createMemoLinkLabel.Name = "_createMemoLinkLabel";
            this._createMemoLinkLabel.Size = new System.Drawing.Size(91, 20);
            this._createMemoLinkLabel.StateCommon.DrawFocus = ComponentFactory.Krypton.Toolkit.InheritBool.False;
            this._createMemoLinkLabel.StateCommon.ShortText.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(95)))), ((int)(((byte)(235)))));
            this._createMemoLinkLabel.TabIndex = 0;
            this._createMemoLinkLabel.Values.Image = global::Mkamo.Memopad.Properties.Resources.sticky_note;
            this._createMemoLinkLabel.Values.Text = "ノートを作成";
            this._createMemoLinkLabel.LinkClicked += new System.EventHandler(this._createMemoLinkLabel_LinkClicked);
            // 
            // _recentlyClosedListBox
            // 
            this._recentlyClosedListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this._recentlyClosedListBox.Location = new System.Drawing.Point(23, 147);
            this._recentlyClosedListBox.Name = "_recentlyClosedListBox";
            this._recentlyClosedListBox.Padding = new System.Windows.Forms.Padding(10, 1, 10, 1);
            this._recentlyClosedListBox.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this._recentlyClosedListBox.Size = new System.Drawing.Size(305, 187);
            this._recentlyClosedListBox.StateCommon.Border.DrawBorders = ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.None;
            this._recentlyClosedListBox.StateCommon.Item.Content.Padding = new System.Windows.Forms.Padding(-1, 2, -1, 2);
            this._recentlyClosedListBox.StateCommon.Item.Content.ShortText.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(95)))), ((int)(((byte)(235)))));
            this._recentlyClosedListBox.TabIndex = 3;
            // 
            // kryptonBorderEdge2
            // 
            this.kryptonBorderEdge2.Location = new System.Drawing.Point(23, 45);
            this.kryptonBorderEdge2.Name = "kryptonBorderEdge2";
            this.kryptonBorderEdge2.Size = new System.Drawing.Size(300, 2);
            this.kryptonBorderEdge2.StateCommon.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(242)))), ((int)(((byte)(254)))));
            this.kryptonBorderEdge2.StateCommon.Width = 2;
            this.kryptonBorderEdge2.Text = "kryptonBorderEdge1";
            // 
            // StartPageContent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this._showOnStartCheckBox);
            this.Controls.Add(this._titleLabel);
            this.Controls.Add(this._recentlyClosedListBox);
            this.Controls.Add(this._recentLabel);
            this.Controls.Add(this.kryptonBorderEdge2);
            this.Controls.Add(this.kryptonBorderEdge1);
            this.Controls.Add(this._manageTagsLinkLabel);
            this.Controls.Add(this._createMemoLinkLabel);
            this.Name = "StartPageContent";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Size = new System.Drawing.Size(344, 372);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox _showOnStartCheckBox;
        private System.Windows.Forms.Label _titleLabel;
        private System.Windows.Forms.Label _recentLabel;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge1;
        private ComponentFactory.Krypton.Toolkit.KryptonLinkLabel _manageTagsLinkLabel;
        private ComponentFactory.Krypton.Toolkit.KryptonLinkLabel _createMemoLinkLabel;
        private ComponentFactory.Krypton.Toolkit.KryptonListBox _recentlyClosedListBox;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge2;


    }
}
