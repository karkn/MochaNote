namespace Mkamo.Memopad.Internal.Controls {
    partial class ConfirmSettingsDetailPage {
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
            this._confirmMemoRemoveCheckBox = new System.Windows.Forms.CheckBox();
            this._confirmTagRemoveCheckBox = new System.Windows.Forms.CheckBox();
            this._confirmSmartFolderRemoveCheckBox = new System.Windows.Forms.CheckBox();
            this._confirmFolderRemoveCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // _confirmMemoRemoveCheckBox
            // 
            this._confirmMemoRemoveCheckBox.AutoSize = true;
            this._confirmMemoRemoveCheckBox.Location = new System.Drawing.Point(12, 12);
            this._confirmMemoRemoveCheckBox.Name = "_confirmMemoRemoveCheckBox";
            this._confirmMemoRemoveCheckBox.Size = new System.Drawing.Size(165, 16);
            this._confirmMemoRemoveCheckBox.TabIndex = 0;
            this._confirmMemoRemoveCheckBox.Text = "ノートの削除時に確認する(&M)";
            this._confirmMemoRemoveCheckBox.UseVisualStyleBackColor = true;
            // 
            // _confirmTagRemoveCheckBox
            // 
            this._confirmTagRemoveCheckBox.AutoSize = true;
            this._confirmTagRemoveCheckBox.Location = new System.Drawing.Point(12, 34);
            this._confirmTagRemoveCheckBox.Name = "_confirmTagRemoveCheckBox";
            this._confirmTagRemoveCheckBox.Size = new System.Drawing.Size(154, 16);
            this._confirmTagRemoveCheckBox.TabIndex = 1;
            this._confirmTagRemoveCheckBox.Text = "タグの削除時に確認する(&T)";
            this._confirmTagRemoveCheckBox.UseVisualStyleBackColor = true;
            // 
            // _confirmSmartFolderRemoveCheckBox
            // 
            this._confirmSmartFolderRemoveCheckBox.AutoSize = true;
            this._confirmSmartFolderRemoveCheckBox.Location = new System.Drawing.Point(12, 56);
            this._confirmSmartFolderRemoveCheckBox.Name = "_confirmSmartFolderRemoveCheckBox";
            this._confirmSmartFolderRemoveCheckBox.Size = new System.Drawing.Size(208, 16);
            this._confirmSmartFolderRemoveCheckBox.TabIndex = 1;
            this._confirmSmartFolderRemoveCheckBox.Text = "スマートフォルダの削除時に確認する(&S)";
            this._confirmSmartFolderRemoveCheckBox.UseVisualStyleBackColor = true;
            this._confirmSmartFolderRemoveCheckBox.Visible = false;
            // 
            // _confirmFolderRemoveCheckBox
            // 
            this._confirmFolderRemoveCheckBox.AutoSize = true;
            this._confirmFolderRemoveCheckBox.Location = new System.Drawing.Point(12, 78);
            this._confirmFolderRemoveCheckBox.Name = "_confirmFolderRemoveCheckBox";
            this._confirmFolderRemoveCheckBox.Size = new System.Drawing.Size(196, 16);
            this._confirmFolderRemoveCheckBox.TabIndex = 1;
            this._confirmFolderRemoveCheckBox.Text = "クリアファイルの削除時に確認する(&C)";
            this._confirmFolderRemoveCheckBox.UseVisualStyleBackColor = true;
            this._confirmFolderRemoveCheckBox.Visible = false;
            // 
            // ConfirmSettingsDetailPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this._confirmFolderRemoveCheckBox);
            this.Controls.Add(this._confirmSmartFolderRemoveCheckBox);
            this.Controls.Add(this._confirmTagRemoveCheckBox);
            this.Controls.Add(this._confirmMemoRemoveCheckBox);
            this.Name = "ConfirmSettingsDetailPage";
            this.Size = new System.Drawing.Size(279, 207);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox _confirmMemoRemoveCheckBox;
        private System.Windows.Forms.CheckBox _confirmTagRemoveCheckBox;
        private System.Windows.Forms.CheckBox _confirmSmartFolderRemoveCheckBox;
        private System.Windows.Forms.CheckBox _confirmFolderRemoveCheckBox;
    }
}
