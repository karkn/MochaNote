namespace Mkamo.Memopad.Internal.Controls {
    partial class BasicSettingsDetailPage {
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
            this._memoDefaultFontNameLabel = new System.Windows.Forms.Label();
            this._memoDefaultFontSizeLabel = new System.Windows.Forms.Label();
            this._memoDefaultFontLabel = new System.Windows.Forms.Label();
            this._memoDefaultFontNameComboBox = new System.Windows.Forms.ComboBox();
            this._memoDefaultFontSizeComboBox = new System.Windows.Forms.ComboBox();
            this._keySchemeLabel = new System.Windows.Forms.Label();
            this._keySchemeComboBox = new System.Windows.Forms.ComboBox();
            this._themeLabel = new System.Windows.Forms.Label();
            this._themeComboBox = new System.Windows.Forms.ComboBox();
            this._memoTextFrameVisiblePolicylabel = new System.Windows.Forms.Label();
            this._memoTextFrameVisiblePolicyComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this._memoTextDefaultMaxWidthcomboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this._showLineBreakCheckBox = new System.Windows.Forms.CheckBox();
            this._showBlockBreakCheckBox = new System.Windows.Forms.CheckBox();
            this._useClearTypeCheckBox = new System.Windows.Forms.CheckBox();
            this._editorCanvasImeOnCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // _memoDefaultFontNameLabel
            // 
            this._memoDefaultFontNameLabel.AutoSize = true;
            this._memoDefaultFontNameLabel.Location = new System.Drawing.Point(24, 35);
            this._memoDefaultFontNameLabel.Name = "_memoDefaultFontNameLabel";
            this._memoDefaultFontNameLabel.Size = new System.Drawing.Size(67, 12);
            this._memoDefaultFontNameLabel.TabIndex = 1;
            this._memoDefaultFontNameLabel.Text = "フォント名(&F):";
            // 
            // _memoDefaultFontSizeLabel
            // 
            this._memoDefaultFontSizeLabel.AutoSize = true;
            this._memoDefaultFontSizeLabel.Location = new System.Drawing.Point(24, 58);
            this._memoDefaultFontSizeLabel.Name = "_memoDefaultFontSizeLabel";
            this._memoDefaultFontSizeLabel.Size = new System.Drawing.Size(84, 12);
            this._memoDefaultFontSizeLabel.TabIndex = 3;
            this._memoDefaultFontSizeLabel.Text = "フォントサイズ(&S):";
            // 
            // _memoDefaultFontLabel
            // 
            this._memoDefaultFontLabel.AutoSize = true;
            this._memoDefaultFontLabel.Location = new System.Drawing.Point(12, 12);
            this._memoDefaultFontLabel.Name = "_memoDefaultFontLabel";
            this._memoDefaultFontLabel.Size = new System.Drawing.Size(120, 12);
            this._memoDefaultFontLabel.TabIndex = 0;
            this._memoDefaultFontLabel.Text = "ノートのデフォルトフォント:";
            // 
            // _memoDefaultFontNameComboBox
            // 
            this._memoDefaultFontNameComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._memoDefaultFontNameComboBox.FormattingEnabled = true;
            this._memoDefaultFontNameComboBox.Location = new System.Drawing.Point(120, 32);
            this._memoDefaultFontNameComboBox.Name = "_memoDefaultFontNameComboBox";
            this._memoDefaultFontNameComboBox.Size = new System.Drawing.Size(134, 20);
            this._memoDefaultFontNameComboBox.TabIndex = 2;
            // 
            // _memoDefaultFontSizeComboBox
            // 
            this._memoDefaultFontSizeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._memoDefaultFontSizeComboBox.FormattingEnabled = true;
            this._memoDefaultFontSizeComboBox.Location = new System.Drawing.Point(120, 55);
            this._memoDefaultFontSizeComboBox.Name = "_memoDefaultFontSizeComboBox";
            this._memoDefaultFontSizeComboBox.Size = new System.Drawing.Size(69, 20);
            this._memoDefaultFontSizeComboBox.TabIndex = 4;
            // 
            // _keySchemeLabel
            // 
            this._keySchemeLabel.AutoSize = true;
            this._keySchemeLabel.Location = new System.Drawing.Point(12, 117);
            this._keySchemeLabel.Name = "_keySchemeLabel";
            this._keySchemeLabel.Size = new System.Drawing.Size(66, 12);
            this._keySchemeLabel.TabIndex = 6;
            this._keySchemeLabel.Text = "キー操作(&K):";
            // 
            // _keySchemeComboBox
            // 
            this._keySchemeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._keySchemeComboBox.FormattingEnabled = true;
            this._keySchemeComboBox.Items.AddRange(new object[] {
            "デフォルト",
            "Emacs"});
            this._keySchemeComboBox.Location = new System.Drawing.Point(120, 114);
            this._keySchemeComboBox.Name = "_keySchemeComboBox";
            this._keySchemeComboBox.Size = new System.Drawing.Size(134, 20);
            this._keySchemeComboBox.TabIndex = 7;
            // 
            // _themeLabel
            // 
            this._themeLabel.AutoSize = true;
            this._themeLabel.Location = new System.Drawing.Point(12, 152);
            this._themeLabel.Name = "_themeLabel";
            this._themeLabel.Size = new System.Drawing.Size(50, 12);
            this._themeLabel.TabIndex = 8;
            this._themeLabel.Text = "テーマ(&T):";
            // 
            // _themeComboBox
            // 
            this._themeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._themeComboBox.FormattingEnabled = true;
            this._themeComboBox.Items.AddRange(new object[] {
            "デフォルト",
            "青",
            "銀",
            "黒"});
            this._themeComboBox.Location = new System.Drawing.Point(120, 149);
            this._themeComboBox.Name = "_themeComboBox";
            this._themeComboBox.Size = new System.Drawing.Size(134, 20);
            this._themeComboBox.TabIndex = 9;
            // 
            // _memoTextFrameVisiblePolicylabel
            // 
            this._memoTextFrameVisiblePolicylabel.AutoSize = true;
            this._memoTextFrameVisiblePolicylabel.Location = new System.Drawing.Point(12, 187);
            this._memoTextFrameVisiblePolicylabel.Name = "_memoTextFrameVisiblePolicylabel";
            this._memoTextFrameVisiblePolicylabel.Size = new System.Drawing.Size(104, 12);
            this._memoTextFrameVisiblePolicylabel.TabIndex = 10;
            this._memoTextFrameVisiblePolicylabel.Text = "テキストの枠表示(&F):";
            // 
            // _memoTextFrameVisiblePolicyComboBox
            // 
            this._memoTextFrameVisiblePolicyComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._memoTextFrameVisiblePolicyComboBox.FormattingEnabled = true;
            this._memoTextFrameVisiblePolicyComboBox.Items.AddRange(new object[] {
            "枠内をマウスでポイントしたとき",
            "移動バー内をマウスでポイントしたとき",
            "選択したとき"});
            this._memoTextFrameVisiblePolicyComboBox.Location = new System.Drawing.Point(24, 210);
            this._memoTextFrameVisiblePolicyComboBox.Name = "_memoTextFrameVisiblePolicyComboBox";
            this._memoTextFrameVisiblePolicyComboBox.Size = new System.Drawing.Size(230, 20);
            this._memoTextFrameVisiblePolicyComboBox.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 245);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(159, 12);
            this.label1.TabIndex = 12;
            this.label1.Text = "テキストの自動折り返し位置(&W):";
            // 
            // _memoTextDefaultMaxWidthcomboBox
            // 
            this._memoTextDefaultMaxWidthcomboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._memoTextDefaultMaxWidthcomboBox.FormattingEnabled = true;
            this._memoTextDefaultMaxWidthcomboBox.Location = new System.Drawing.Point(120, 268);
            this._memoTextDefaultMaxWidthcomboBox.Name = "_memoTextDefaultMaxWidthcomboBox";
            this._memoTextDefaultMaxWidthcomboBox.Size = new System.Drawing.Size(134, 20);
            this._memoTextDefaultMaxWidthcomboBox.TabIndex = 13;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 303);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 14;
            this.label2.Text = "編集記号の表示:";
            // 
            // _showLineBreakCheckBox
            // 
            this._showLineBreakCheckBox.AutoSize = true;
            this._showLineBreakCheckBox.Location = new System.Drawing.Point(24, 326);
            this._showLineBreakCheckBox.Name = "_showLineBreakCheckBox";
            this._showLineBreakCheckBox.Size = new System.Drawing.Size(62, 16);
            this._showLineBreakCheckBox.TabIndex = 15;
            this._showLineBreakCheckBox.Text = "改行(&L)";
            this._showLineBreakCheckBox.UseVisualStyleBackColor = true;
            // 
            // _showBlockBreakCheckBox
            // 
            this._showBlockBreakCheckBox.AutoSize = true;
            this._showBlockBreakCheckBox.Location = new System.Drawing.Point(24, 348);
            this._showBlockBreakCheckBox.Name = "_showBlockBreakCheckBox";
            this._showBlockBreakCheckBox.Size = new System.Drawing.Size(76, 16);
            this._showBlockBreakCheckBox.TabIndex = 16;
            this._showBlockBreakCheckBox.Text = "改段落(&B)";
            this._showBlockBreakCheckBox.UseVisualStyleBackColor = true;
            // 
            // _useClearTypeCheckBox
            // 
            this._useClearTypeCheckBox.AutoSize = true;
            this._useClearTypeCheckBox.Location = new System.Drawing.Point(26, 81);
            this._useClearTypeCheckBox.Name = "_useClearTypeCheckBox";
            this._useClearTypeCheckBox.Size = new System.Drawing.Size(165, 16);
            this._useClearTypeCheckBox.TabIndex = 5;
            this._useClearTypeCheckBox.Text = "常にClearTypeを使用する(&C)";
            this._useClearTypeCheckBox.UseVisualStyleBackColor = true;
            // 
            // _editorCanvasImeOnCheckBox
            // 
            this._editorCanvasImeOnCheckBox.AutoSize = true;
            this._editorCanvasImeOnCheckBox.Location = new System.Drawing.Point(14, 383);
            this._editorCanvasImeOnCheckBox.Name = "_editorCanvasImeOnCheckBox";
            this._editorCanvasImeOnCheckBox.Size = new System.Drawing.Size(235, 16);
            this._editorCanvasImeOnCheckBox.TabIndex = 17;
            this._editorCanvasImeOnCheckBox.Text = "ノートエディタを開いたときにIMEをオンにする(&I)";
            this._editorCanvasImeOnCheckBox.UseVisualStyleBackColor = true;
            // 
            // BasicSettingsDetailPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoScrollMargin = new System.Drawing.Size(0, 20);
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this._useClearTypeCheckBox);
            this.Controls.Add(this._editorCanvasImeOnCheckBox);
            this.Controls.Add(this._showBlockBreakCheckBox);
            this.Controls.Add(this._showLineBreakCheckBox);
            this.Controls.Add(this._memoDefaultFontSizeComboBox);
            this.Controls.Add(this._memoTextDefaultMaxWidthcomboBox);
            this.Controls.Add(this._memoTextFrameVisiblePolicyComboBox);
            this.Controls.Add(this._themeComboBox);
            this.Controls.Add(this._keySchemeComboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._memoTextFrameVisiblePolicylabel);
            this.Controls.Add(this._themeLabel);
            this.Controls.Add(this._memoDefaultFontNameComboBox);
            this.Controls.Add(this._memoDefaultFontSizeLabel);
            this.Controls.Add(this._keySchemeLabel);
            this.Controls.Add(this._memoDefaultFontLabel);
            this.Controls.Add(this._memoDefaultFontNameLabel);
            this.Name = "BasicSettingsDetailPage";
            this.Size = new System.Drawing.Size(317, 436);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _memoDefaultFontNameLabel;
        private System.Windows.Forms.Label _memoDefaultFontSizeLabel;
        private System.Windows.Forms.Label _memoDefaultFontLabel;
        private System.Windows.Forms.ComboBox _memoDefaultFontNameComboBox;
        private System.Windows.Forms.ComboBox _memoDefaultFontSizeComboBox;
        private System.Windows.Forms.Label _keySchemeLabel;
        private System.Windows.Forms.ComboBox _keySchemeComboBox;
        private System.Windows.Forms.Label _themeLabel;
        private System.Windows.Forms.ComboBox _themeComboBox;
        private System.Windows.Forms.Label _memoTextFrameVisiblePolicylabel;
        private System.Windows.Forms.ComboBox _memoTextFrameVisiblePolicyComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox _memoTextDefaultMaxWidthcomboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox _showLineBreakCheckBox;
        private System.Windows.Forms.CheckBox _showBlockBreakCheckBox;
        private System.Windows.Forms.CheckBox _useClearTypeCheckBox;
        private System.Windows.Forms.CheckBox _editorCanvasImeOnCheckBox;
    }
}
