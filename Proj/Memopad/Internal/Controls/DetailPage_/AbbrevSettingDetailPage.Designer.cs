namespace Mkamo.Memopad.Internal.Controls {
    partial class AbbrevSettingDetailPage {
        /// <summary> 
        /// 必要なデザイナー変数です。
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

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent() {
            this._wordsTextBox = new System.Windows.Forms.TextBox();
            this._messageLabel = new System.Windows.Forms.Label();
            this._wordsLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _wordsTextBox
            // 
            this._wordsTextBox.AcceptsReturn = true;
            this._wordsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this._wordsTextBox.Location = new System.Drawing.Point(14, 27);
            this._wordsTextBox.Multiline = true;
            this._wordsTextBox.Name = "_wordsTextBox";
            this._wordsTextBox.Size = new System.Drawing.Size(153, 237);
            this._wordsTextBox.TabIndex = 1;
            this._wordsTextBox.WordWrap = false;
            // 
            // _messageLabel
            // 
            this._messageLabel.AutoSize = true;
            this._messageLabel.Location = new System.Drawing.Point(173, 27);
            this._messageLabel.Name = "_messageLabel";
            this._messageLabel.Size = new System.Drawing.Size(50, 12);
            this._messageLabel.TabIndex = 2;
            this._messageLabel.Text = "message";
            // 
            // _wordsLabel
            // 
            this._wordsLabel.AutoSize = true;
            this._wordsLabel.Location = new System.Drawing.Point(12, 12);
            this._wordsLabel.Name = "_wordsLabel";
            this._wordsLabel.Size = new System.Drawing.Size(48, 12);
            this._wordsLabel.TabIndex = 0;
            this._wordsLabel.Text = "単語(&W):";
            // 
            // AbbrevSettingDetailPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this._wordsLabel);
            this.Controls.Add(this._messageLabel);
            this.Controls.Add(this._wordsTextBox);
            this.Name = "AbbrevSettingDetailPage";
            this.Size = new System.Drawing.Size(290, 276);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _wordsTextBox;
        private System.Windows.Forms.Label _messageLabel;
        private System.Windows.Forms.Label _wordsLabel;

    }
}
