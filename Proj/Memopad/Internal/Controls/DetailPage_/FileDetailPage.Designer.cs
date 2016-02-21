namespace Mkamo.Memopad.Internal.Controls {
    partial class FileDetailPage {
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
            this._titleLabel = new System.Windows.Forms.Label();
            this._pathLabel = new System.Windows.Forms.Label();
            this._titleTextBox = new System.Windows.Forms.TextBox();
            this._pathTextBox = new System.Windows.Forms.TextBox();
            this._pathButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _titleLabel
            // 
            this._titleLabel.AutoSize = true;
            this._titleLabel.Location = new System.Drawing.Point(6, 14);
            this._titleLabel.Name = "_titleLabel";
            this._titleLabel.Size = new System.Drawing.Size(57, 12);
            this._titleLabel.TabIndex = 0;
            this._titleLabel.Text = "タイトル(&T):";
            // 
            // _pathLabel
            // 
            this._pathLabel.AutoSize = true;
            this._pathLabel.Location = new System.Drawing.Point(6, 39);
            this._pathLabel.Name = "_pathLabel";
            this._pathLabel.Size = new System.Drawing.Size(41, 12);
            this._pathLabel.TabIndex = 1;
            this._pathLabel.Text = "パス(&P):";
            // 
            // _titleTextBox
            // 
            this._titleTextBox.Location = new System.Drawing.Point(73, 11);
            this._titleTextBox.Name = "_titleTextBox";
            this._titleTextBox.Size = new System.Drawing.Size(222, 19);
            this._titleTextBox.TabIndex = 2;
            // 
            // _pathTextBox
            // 
            this._pathTextBox.Location = new System.Drawing.Point(73, 36);
            this._pathTextBox.Name = "_pathTextBox";
            this._pathTextBox.Size = new System.Drawing.Size(170, 19);
            this._pathTextBox.TabIndex = 2;
            // 
            // _pathButton
            // 
            this._pathButton.BackColor = System.Drawing.SystemColors.Control;
            this._pathButton.Location = new System.Drawing.Point(243, 34);
            this._pathButton.Name = "_pathButton";
            this._pathButton.Size = new System.Drawing.Size(52, 23);
            this._pathButton.TabIndex = 3;
            this._pathButton.Text = "参照(&R)";
            this._pathButton.UseVisualStyleBackColor = false;
            this._pathButton.Click += new System.EventHandler(this._pathButton_Click);
            // 
            // FileDetailPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._pathButton);
            this.Controls.Add(this._pathTextBox);
            this.Controls.Add(this._titleTextBox);
            this.Controls.Add(this._pathLabel);
            this.Controls.Add(this._titleLabel);
            this.Name = "FileDetailPage";
            this.Size = new System.Drawing.Size(324, 272);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _titleLabel;
        private System.Windows.Forms.Label _pathLabel;
        private System.Windows.Forms.TextBox _titleTextBox;
        private System.Windows.Forms.TextBox _pathTextBox;
        private System.Windows.Forms.Button _pathButton;
    }
}
