namespace Mkamo.Editor.Internal.Controls {
    partial class Completion {
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
            this._completionListBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // _completionListBox
            // 
            this._completionListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._completionListBox.FormattingEnabled = true;
            this._completionListBox.ItemHeight = 12;
            this._completionListBox.Location = new System.Drawing.Point(1, 1);
            this._completionListBox.Name = "_completionListBox";
            this._completionListBox.Size = new System.Drawing.Size(360, 180);
            this._completionListBox.TabIndex = 0;
            // 
            // Completion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._completionListBox);
            this.Name = "Completion";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Size = new System.Drawing.Size(403, 284);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.ListBox _completionListBox;

    }
}
