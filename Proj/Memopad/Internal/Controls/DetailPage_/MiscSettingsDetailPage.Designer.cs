namespace Mkamo.Memopad.Internal.Controls {
    partial class MiscSettingsDetailPage {
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
            this._startOnWindowsStartUpCheckBox = new System.Windows.Forms.CheckBox();
            this._emptyTrasBoxCheckBox = new System.Windows.Forms.CheckBox();
            this._showStartPageOnStartCheckBox = new System.Windows.Forms.CheckBox();
            this._minimizeToTaskTrayCheckBox = new System.Windows.Forms.CheckBox();
            this._minimizeOnStartUpCheckBox = new System.Windows.Forms.CheckBox();
            this._replaceMeiryoCheckBox = new System.Windows.Forms.CheckBox();
            this._checkLatestOnStartCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // _startOnWindowsStartUpCheckBox
            // 
            this._startOnWindowsStartUpCheckBox.AutoSize = true;
            this._startOnWindowsStartUpCheckBox.Location = new System.Drawing.Point(12, 144);
            this._startOnWindowsStartUpCheckBox.Name = "_startOnWindowsStartUpCheckBox";
            this._startOnWindowsStartUpCheckBox.Size = new System.Drawing.Size(237, 16);
            this._startOnWindowsStartUpCheckBox.TabIndex = 4;
            this._startOnWindowsStartUpCheckBox.Text = "Windows起動時にConfidanteを起動する(&W)";
            this._startOnWindowsStartUpCheckBox.UseVisualStyleBackColor = true;
            // 
            // _emptyTrasBoxCheckBox
            // 
            this._emptyTrasBoxCheckBox.AutoSize = true;
            this._emptyTrasBoxCheckBox.Location = new System.Drawing.Point(12, 12);
            this._emptyTrasBoxCheckBox.Name = "_emptyTrasBoxCheckBox";
            this._emptyTrasBoxCheckBox.Size = new System.Drawing.Size(165, 16);
            this._emptyTrasBoxCheckBox.TabIndex = 0;
            this._emptyTrasBoxCheckBox.Text = "終了時にごみ箱を空にする(&E)";
            this._emptyTrasBoxCheckBox.UseVisualStyleBackColor = true;
            // 
            // _showStartPageOnStartCheckBox
            // 
            this._showStartPageOnStartCheckBox.AutoSize = true;
            this._showStartPageOnStartCheckBox.Location = new System.Drawing.Point(12, 34);
            this._showStartPageOnStartCheckBox.Name = "_showStartPageOnStartCheckBox";
            this._showStartPageOnStartCheckBox.Size = new System.Drawing.Size(201, 16);
            this._showStartPageOnStartCheckBox.TabIndex = 1;
            this._showStartPageOnStartCheckBox.Text = "起動時にスタートページを表示する(&S)";
            this._showStartPageOnStartCheckBox.UseVisualStyleBackColor = true;
            // 
            // _minimizeToTaskTrayCheckBox
            // 
            this._minimizeToTaskTrayCheckBox.AutoSize = true;
            this._minimizeToTaskTrayCheckBox.Location = new System.Drawing.Point(12, 78);
            this._minimizeToTaskTrayCheckBox.Name = "_minimizeToTaskTrayCheckBox";
            this._minimizeToTaskTrayCheckBox.Size = new System.Drawing.Size(201, 16);
            this._minimizeToTaskTrayCheckBox.TabIndex = 2;
            this._minimizeToTaskTrayCheckBox.Text = "最小化時にタスクトレイに格納する(&M)";
            this._minimizeToTaskTrayCheckBox.UseVisualStyleBackColor = true;
            // 
            // _minimizeOnStartUpCheckBox
            // 
            this._minimizeOnStartUpCheckBox.AutoSize = true;
            this._minimizeOnStartUpCheckBox.Location = new System.Drawing.Point(30, 100);
            this._minimizeOnStartUpCheckBox.Name = "_minimizeOnStartUpCheckBox";
            this._minimizeOnStartUpCheckBox.Size = new System.Drawing.Size(187, 16);
            this._minimizeOnStartUpCheckBox.TabIndex = 3;
            this._minimizeOnStartUpCheckBox.Text = "起動時にタスクトレイに格納する(&T)";
            this._minimizeOnStartUpCheckBox.UseVisualStyleBackColor = true;
            // 
            // _replaceMeiryoCheckBox
            // 
            this._replaceMeiryoCheckBox.AutoSize = true;
            this._replaceMeiryoCheckBox.Location = new System.Drawing.Point(12, 122);
            this._replaceMeiryoCheckBox.Name = "_replaceMeiryoCheckBox";
            this._replaceMeiryoCheckBox.Size = new System.Drawing.Size(295, 16);
            this._replaceMeiryoCheckBox.TabIndex = 4;
            this._replaceMeiryoCheckBox.Text = "ウィンドウのフォントをメイリオからMeiryo UIに置き換える(&R)";
            this._replaceMeiryoCheckBox.UseVisualStyleBackColor = true;
            // 
            // _checkLatestOnStartCheckBox
            // 
            this._checkLatestOnStartCheckBox.AutoSize = true;
            this._checkLatestOnStartCheckBox.Location = new System.Drawing.Point(12, 56);
            this._checkLatestOnStartCheckBox.Name = "_checkLatestOnStartCheckBox";
            this._checkLatestOnStartCheckBox.Size = new System.Drawing.Size(178, 16);
            this._checkLatestOnStartCheckBox.TabIndex = 4;
            this._checkLatestOnStartCheckBox.Text = "起動時に最新版をチェックする(&L)";
            this._checkLatestOnStartCheckBox.UseVisualStyleBackColor = true;
            // 
            // MiscSettingsDetailPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this._minimizeOnStartUpCheckBox);
            this.Controls.Add(this._minimizeToTaskTrayCheckBox);
            this.Controls.Add(this._replaceMeiryoCheckBox);
            this.Controls.Add(this._checkLatestOnStartCheckBox);
            this.Controls.Add(this._startOnWindowsStartUpCheckBox);
            this.Controls.Add(this._showStartPageOnStartCheckBox);
            this.Controls.Add(this._emptyTrasBoxCheckBox);
            this.Name = "MiscSettingsDetailPage";
            this.Size = new System.Drawing.Size(334, 330);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox _startOnWindowsStartUpCheckBox;
        private System.Windows.Forms.CheckBox _emptyTrasBoxCheckBox;
        private System.Windows.Forms.CheckBox _showStartPageOnStartCheckBox;
        private System.Windows.Forms.CheckBox _minimizeToTaskTrayCheckBox;
        private System.Windows.Forms.CheckBox _minimizeOnStartUpCheckBox;
        private System.Windows.Forms.CheckBox _replaceMeiryoCheckBox;
        private System.Windows.Forms.CheckBox _checkLatestOnStartCheckBox;
    }
}
