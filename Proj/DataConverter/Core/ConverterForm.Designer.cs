namespace Mkamo.DataConverter.Core {
    partial class ConvertForm {
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

        #region Windows フォーム デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent() {
            this._convertButton = new System.Windows.Forms.Button();
            this._progressBar = new System.Windows.Forms.ProgressBar();
            this._closeButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _convertButton
            // 
            this._convertButton.Location = new System.Drawing.Point(243, 41);
            this._convertButton.Name = "_convertButton";
            this._convertButton.Size = new System.Drawing.Size(75, 23);
            this._convertButton.TabIndex = 0;
            this._convertButton.Text = "変換";
            this._convertButton.UseVisualStyleBackColor = true;
            this._convertButton.Click += new System.EventHandler(this._convertButton_Click);
            // 
            // _progressBar
            // 
            this._progressBar.Location = new System.Drawing.Point(12, 12);
            this._progressBar.Name = "_progressBar";
            this._progressBar.Size = new System.Drawing.Size(387, 23);
            this._progressBar.TabIndex = 1;
            // 
            // _closeButton
            // 
            this._closeButton.Location = new System.Drawing.Point(324, 41);
            this._closeButton.Name = "_closeButton";
            this._closeButton.Size = new System.Drawing.Size(75, 23);
            this._closeButton.TabIndex = 2;
            this._closeButton.Text = "閉じる";
            this._closeButton.UseVisualStyleBackColor = true;
            this._closeButton.Click += new System.EventHandler(this._closeButton_Click);
            // 
            // ConvertForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(411, 74);
            this.ControlBox = false;
            this.Controls.Add(this._closeButton);
            this.Controls.Add(this._progressBar);
            this.Controls.Add(this._convertButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ConvertForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "V1データ変換";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _convertButton;
        private System.Windows.Forms.ProgressBar _progressBar;
        private System.Windows.Forms.Button _closeButton;
    }
}

