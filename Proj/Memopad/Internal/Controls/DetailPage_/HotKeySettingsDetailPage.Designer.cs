namespace Mkamo.Memopad.Internal.Controls {
    partial class HotKeySettingsDetailPage {
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
            this._createMemoLabel = new System.Windows.Forms.Label();
            this._createMemoTextBox = new Mkamo.Control.KeyBindTextBox.KeyBindTextBox();
            this._clipMemoTextBox = new Mkamo.Control.KeyBindTextBox.KeyBindTextBox();
            this._clipMemoLabel = new System.Windows.Forms.Label();
            this._captureScreenLabel = new System.Windows.Forms.Label();
            this._captureScreenTextBox = new Mkamo.Control.KeyBindTextBox.KeyBindTextBox();
            this._activateLabel = new System.Windows.Forms.Label();
            this._activateTextBox = new Mkamo.Control.KeyBindTextBox.KeyBindTextBox();
            this.SuspendLayout();
            // 
            // _createMemoLabel
            // 
            this._createMemoLabel.AutoSize = true;
            this._createMemoLabel.Location = new System.Drawing.Point(12, 64);
            this._createMemoLabel.Name = "_createMemoLabel";
            this._createMemoLabel.Size = new System.Drawing.Size(83, 12);
            this._createMemoLabel.TabIndex = 2;
            this._createMemoLabel.Text = "ノートを作成(&M):";
            // 
            // _createMemoTextBox
            // 
            this._createMemoTextBox.Location = new System.Drawing.Point(24, 83);
            this._createMemoTextBox.Name = "_createMemoTextBox";
            this._createMemoTextBox.Size = new System.Drawing.Size(149, 19);
            this._createMemoTextBox.TabIndex = 3;
            // 
            // _clipMemoTextBox
            // 
            this._clipMemoTextBox.Location = new System.Drawing.Point(24, 135);
            this._clipMemoTextBox.Name = "_clipMemoTextBox";
            this._clipMemoTextBox.Size = new System.Drawing.Size(149, 19);
            this._clipMemoTextBox.TabIndex = 1;
            // 
            // _clipMemoLabel
            // 
            this._clipMemoLabel.AutoSize = true;
            this._clipMemoLabel.Location = new System.Drawing.Point(12, 116);
            this._clipMemoLabel.Name = "_clipMemoLabel";
            this._clipMemoLabel.Size = new System.Drawing.Size(89, 12);
            this._clipMemoLabel.TabIndex = 0;
            this._clipMemoLabel.Text = "ノートにクリップ(&C):";
            // 
            // _captureScreenLabel
            // 
            this._captureScreenLabel.AutoSize = true;
            this._captureScreenLabel.Location = new System.Drawing.Point(12, 168);
            this._captureScreenLabel.Name = "_captureScreenLabel";
            this._captureScreenLabel.Size = new System.Drawing.Size(99, 12);
            this._captureScreenLabel.TabIndex = 2;
            this._captureScreenLabel.Text = "画面を取り込み(&D):";
            // 
            // _captureScreenTextBox
            // 
            this._captureScreenTextBox.Location = new System.Drawing.Point(24, 187);
            this._captureScreenTextBox.Name = "_captureScreenTextBox";
            this._captureScreenTextBox.Size = new System.Drawing.Size(149, 19);
            this._captureScreenTextBox.TabIndex = 3;
            // 
            // _activateLabel
            // 
            this._activateLabel.AutoSize = true;
            this._activateLabel.Location = new System.Drawing.Point(12, 12);
            this._activateLabel.Name = "_activateLabel";
            this._activateLabel.Size = new System.Drawing.Size(93, 12);
            this._activateLabel.TabIndex = 0;
            this._activateLabel.Text = "アクティブにする(&A):";
            // 
            // _activateTextBox
            // 
            this._activateTextBox.Location = new System.Drawing.Point(24, 31);
            this._activateTextBox.Name = "_activateTextBox";
            this._activateTextBox.Size = new System.Drawing.Size(149, 19);
            this._activateTextBox.TabIndex = 1;
            // 
            // HotKeySettingsDetailPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this._activateTextBox);
            this.Controls.Add(this._clipMemoTextBox);
            this.Controls.Add(this._activateLabel);
            this.Controls.Add(this._clipMemoLabel);
            this.Controls.Add(this._captureScreenTextBox);
            this.Controls.Add(this._createMemoTextBox);
            this.Controls.Add(this._captureScreenLabel);
            this.Controls.Add(this._createMemoLabel);
            this.Name = "HotKeySettingsDetailPage";
            this.Size = new System.Drawing.Size(292, 239);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _createMemoLabel;
        private Mkamo.Control.KeyBindTextBox.KeyBindTextBox _createMemoTextBox;
        private Mkamo.Control.KeyBindTextBox.KeyBindTextBox _clipMemoTextBox;
        private System.Windows.Forms.Label _clipMemoLabel;
        private System.Windows.Forms.Label _captureScreenLabel;
        private Mkamo.Control.KeyBindTextBox.KeyBindTextBox _captureScreenTextBox;
        private System.Windows.Forms.Label _activateLabel;
        private Mkamo.Control.KeyBindTextBox.KeyBindTextBox _activateTextBox;
    }
}
