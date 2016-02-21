namespace Mkamo.Control.Configurator {
    partial class SolidBackgroundConfigurator {
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
            this._colorLabel = new System.Windows.Forms.Label();
            this._opacityLabel = new System.Windows.Forms.Label();
            this._colorPanel = new System.Windows.Forms.Panel();
            this._showColorDialogButton = new System.Windows.Forms.Button();
            this._opacityTrackBar = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize) (this._opacityTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // _colorLabel
            // 
            this._colorLabel.AutoSize = true;
            this._colorLabel.Location = new System.Drawing.Point(3, 8);
            this._colorLabel.Name = "_colorLabel";
            this._colorLabel.Size = new System.Drawing.Size(35, 12);
            this._colorLabel.TabIndex = 0;
            this._colorLabel.Text = "色(&C):";
            // 
            // _opacityLabel
            // 
            this._opacityLabel.AutoSize = true;
            this._opacityLabel.Location = new System.Drawing.Point(3, 39);
            this._opacityLabel.Name = "_opacityLabel";
            this._opacityLabel.Size = new System.Drawing.Size(58, 12);
            this._opacityLabel.TabIndex = 2;
            this._opacityLabel.Text = "透過率(&T):";
            // 
            // _colorPanel
            // 
            this._colorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._colorPanel.Location = new System.Drawing.Point(75, 4);
            this._colorPanel.Name = "_colorPanel";
            this._colorPanel.Size = new System.Drawing.Size(25, 20);
            this._colorPanel.TabIndex = 1;
            // 
            // _showColorDialogButton
            // 
            this._showColorDialogButton.BackColor = System.Drawing.SystemColors.Control;
            this._showColorDialogButton.Location = new System.Drawing.Point(106, 3);
            this._showColorDialogButton.Name = "_showColorDialogButton";
            this._showColorDialogButton.Size = new System.Drawing.Size(60, 23);
            this._showColorDialogButton.TabIndex = 1;
            this._showColorDialogButton.Text = "選択(&S)";
            this._showColorDialogButton.UseVisualStyleBackColor = false;
            this._showColorDialogButton.Click += new System.EventHandler(this._showColorDialogButton_Click);
            // 
            // _opacityTrackBar
            // 
            this._opacityTrackBar.Location = new System.Drawing.Point(76, 36);
            this._opacityTrackBar.Maximum = 100;
            this._opacityTrackBar.Name = "_opacityTrackBar";
            this._opacityTrackBar.Size = new System.Drawing.Size(104, 37);
            this._opacityTrackBar.TabIndex = 3;
            this._opacityTrackBar.TickFrequency = 10;
            this._opacityTrackBar.Value = 100;
            this._opacityTrackBar.ValueChanged += new System.EventHandler(this._opacityTrackBar_ValueChanged);
            // 
            // SolidBackgroundConfigurator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._opacityTrackBar);
            this.Controls.Add(this._showColorDialogButton);
            this.Controls.Add(this._colorPanel);
            this.Controls.Add(this._opacityLabel);
            this.Controls.Add(this._colorLabel);
            this.Name = "SolidBackgroundConfigurator";
            this.Size = new System.Drawing.Size(198, 73);
            ((System.ComponentModel.ISupportInitialize) (this._opacityTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _colorLabel;
        private System.Windows.Forms.Label _opacityLabel;
        private System.Windows.Forms.Panel _colorPanel;
        private System.Windows.Forms.Button _showColorDialogButton;
        private System.Windows.Forms.TrackBar _opacityTrackBar;
    }
}
