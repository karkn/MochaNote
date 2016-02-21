namespace Mkamo.Memopad.Internal.Controls {
    partial class BackgroundSettingsDetailPage {
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
            this._enableBackgroundImageCheckBox = new System.Windows.Forms.CheckBox();
            this._imageFileTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this._opacityTrackBar = new System.Windows.Forms.TrackBar();
            this._scaleTrackBar = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this._imageFileButton = new System.Windows.Forms.Button();
            this._opacityValueLabel = new System.Windows.Forms.Label();
            this._scaleValueLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize) (this._opacityTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this._scaleTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // _enableBackgroundImageCheckBox
            // 
            this._enableBackgroundImageCheckBox.AutoSize = true;
            this._enableBackgroundImageCheckBox.Location = new System.Drawing.Point(12, 12);
            this._enableBackgroundImageCheckBox.Name = "_enableBackgroundImageCheckBox";
            this._enableBackgroundImageCheckBox.Size = new System.Drawing.Size(210, 16);
            this._enableBackgroundImageCheckBox.TabIndex = 1;
            this._enableBackgroundImageCheckBox.Text = "ノートエディタの背景に画像を表示する(&B)";
            this._enableBackgroundImageCheckBox.UseVisualStyleBackColor = true;
            // 
            // _imageFileTextBox
            // 
            this._imageFileTextBox.Location = new System.Drawing.Point(102, 41);
            this._imageFileTextBox.Name = "_imageFileTextBox";
            this._imageFileTextBox.Size = new System.Drawing.Size(157, 19);
            this._imageFileTextBox.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "画像ファイル(&F):";
            // 
            // _opacityTrackBar
            // 
            this._opacityTrackBar.LargeChange = 10;
            this._opacityTrackBar.Location = new System.Drawing.Point(94, 82);
            this._opacityTrackBar.Maximum = 100;
            this._opacityTrackBar.Name = "_opacityTrackBar";
            this._opacityTrackBar.Size = new System.Drawing.Size(165, 37);
            this._opacityTrackBar.TabIndex = 6;
            this._opacityTrackBar.TickFrequency = 10;
            // 
            // _scaleTrackBar
            // 
            this._scaleTrackBar.LargeChange = 10;
            this._scaleTrackBar.Location = new System.Drawing.Point(94, 125);
            this._scaleTrackBar.Maximum = 200;
            this._scaleTrackBar.Name = "_scaleTrackBar";
            this._scaleTrackBar.Size = new System.Drawing.Size(165, 37);
            this._scaleTrackBar.TabIndex = 9;
            this._scaleTrackBar.TickFrequency = 10;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 86);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "不透過率(&O):";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 128);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "拡大率(&S):";
            // 
            // _imageFileButton
            // 
            this._imageFileButton.BackColor = System.Drawing.SystemColors.Control;
            this._imageFileButton.Location = new System.Drawing.Point(259, 39);
            this._imageFileButton.Name = "_imageFileButton";
            this._imageFileButton.Size = new System.Drawing.Size(51, 23);
            this._imageFileButton.TabIndex = 4;
            this._imageFileButton.Text = "参照...";
            this._imageFileButton.UseVisualStyleBackColor = false;
            this._imageFileButton.Click += new System.EventHandler(this._imageFileButton_Click);
            // 
            // _opacityValueLabel
            // 
            this._opacityValueLabel.AutoSize = true;
            this._opacityValueLabel.Location = new System.Drawing.Point(257, 86);
            this._opacityValueLabel.Name = "_opacityValueLabel";
            this._opacityValueLabel.Size = new System.Drawing.Size(17, 12);
            this._opacityValueLabel.TabIndex = 7;
            this._opacityValueLabel.Text = "0%";
            // 
            // _scaleValueLabel
            // 
            this._scaleValueLabel.AutoSize = true;
            this._scaleValueLabel.Location = new System.Drawing.Point(257, 128);
            this._scaleValueLabel.Name = "_scaleValueLabel";
            this._scaleValueLabel.Size = new System.Drawing.Size(17, 12);
            this._scaleValueLabel.TabIndex = 0;
            this._scaleValueLabel.Text = "0%";
            // 
            // BackgroundSettingsDetailPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this._imageFileButton);
            this.Controls.Add(this._scaleTrackBar);
            this.Controls.Add(this._opacityTrackBar);
            this.Controls.Add(this.label3);
            this.Controls.Add(this._scaleValueLabel);
            this.Controls.Add(this._opacityValueLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._imageFileTextBox);
            this.Controls.Add(this._enableBackgroundImageCheckBox);
            this.Name = "BackgroundSettingsDetailPage";
            this.Size = new System.Drawing.Size(347, 330);
            ((System.ComponentModel.ISupportInitialize) (this._opacityTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this._scaleTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox _enableBackgroundImageCheckBox;
        private System.Windows.Forms.TextBox _imageFileTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar _opacityTrackBar;
        private System.Windows.Forms.TrackBar _scaleTrackBar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button _imageFileButton;
        private System.Windows.Forms.Label _opacityValueLabel;
        private System.Windows.Forms.Label _scaleValueLabel;
    }
}
