namespace Mkamo.Memopad.Internal.Controls {
    partial class FolderSettingsDetailPage {
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
            this._memoRootTextBox = new System.Windows.Forms.TextBox();
            this._memoRootButton = new System.Windows.Forms.Button();
            this._memoRootLabel = new System.Windows.Forms.Label();
            this._backupRootLabel = new System.Windows.Forms.Label();
            this._backupRootButton = new System.Windows.Forms.Button();
            this._backupRootTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // _memoRootTextBox
            // 
            this._memoRootTextBox.Location = new System.Drawing.Point(24, 35);
            this._memoRootTextBox.Name = "_memoRootTextBox";
            this._memoRootTextBox.Size = new System.Drawing.Size(214, 19);
            this._memoRootTextBox.TabIndex = 0;
            // 
            // _memoRootButton
            // 
            this._memoRootButton.BackColor = System.Drawing.SystemColors.Control;
            this._memoRootButton.Location = new System.Drawing.Point(237, 33);
            this._memoRootButton.Name = "_memoRootButton";
            this._memoRootButton.Size = new System.Drawing.Size(52, 23);
            this._memoRootButton.TabIndex = 1;
            this._memoRootButton.Text = "参照...";
            this._memoRootButton.UseVisualStyleBackColor = false;
            this._memoRootButton.Click += new System.EventHandler(this._memoRootButton_Click);
            // 
            // _memoRootLabel
            // 
            this._memoRootLabel.AutoSize = true;
            this._memoRootLabel.Location = new System.Drawing.Point(12, 12);
            this._memoRootLabel.Name = "_memoRootLabel";
            this._memoRootLabel.Size = new System.Drawing.Size(109, 12);
            this._memoRootLabel.TabIndex = 2;
            this._memoRootLabel.Text = "ノート格納フォルダ(&M):";
            // 
            // _backupRootLabel
            // 
            this._backupRootLabel.AutoSize = true;
            this._backupRootLabel.Location = new System.Drawing.Point(12, 69);
            this._backupRootLabel.Name = "_backupRootLabel";
            this._backupRootLabel.Size = new System.Drawing.Size(132, 12);
            this._backupRootLabel.TabIndex = 5;
            this._backupRootLabel.Text = "自動バックアップフォルダ(&B):";
            // 
            // _backupRootButton
            // 
            this._backupRootButton.BackColor = System.Drawing.SystemColors.Control;
            this._backupRootButton.Location = new System.Drawing.Point(237, 90);
            this._backupRootButton.Name = "_backupRootButton";
            this._backupRootButton.Size = new System.Drawing.Size(52, 23);
            this._backupRootButton.TabIndex = 4;
            this._backupRootButton.Text = "参照...";
            this._backupRootButton.UseVisualStyleBackColor = false;
            this._backupRootButton.Click += new System.EventHandler(this._backupRootButton_Click);
            // 
            // _backupRootTextBox
            // 
            this._backupRootTextBox.Location = new System.Drawing.Point(24, 92);
            this._backupRootTextBox.Name = "_backupRootTextBox";
            this._backupRootTextBox.Size = new System.Drawing.Size(214, 19);
            this._backupRootTextBox.TabIndex = 3;
            // 
            // FolderSettingsDetailPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this._backupRootLabel);
            this.Controls.Add(this._backupRootButton);
            this.Controls.Add(this._backupRootTextBox);
            this.Controls.Add(this._memoRootLabel);
            this.Controls.Add(this._memoRootButton);
            this.Controls.Add(this._memoRootTextBox);
            this.Name = "FolderSettingsDetailPage";
            this.Size = new System.Drawing.Size(332, 269);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _memoRootTextBox;
        private System.Windows.Forms.Button _memoRootButton;
        private System.Windows.Forms.Label _memoRootLabel;
        private System.Windows.Forms.Label _backupRootLabel;
        private System.Windows.Forms.Button _backupRootButton;
        private System.Windows.Forms.TextBox _backupRootTextBox;
    }
}
