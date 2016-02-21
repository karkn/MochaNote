namespace Mkamo.Control.Configurator {
    partial class GradientBackgroundConfigurator {
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
            this._directionLabel = new System.Windows.Forms.Label();
            this._directionComboBox = new System.Windows.Forms.ComboBox();
            this._endColorLabel = new System.Windows.Forms.Label();
            this._showEndColorDialogButton = new System.Windows.Forms.Button();
            this._endColorPanel = new System.Windows.Forms.Panel();
            this._startColorLabel = new System.Windows.Forms.Label();
            this._startColorPanel = new System.Windows.Forms.Panel();
            this._showStartColorDialogButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _directionLabel
            // 
            this._directionLabel.AutoSize = true;
            this._directionLabel.Location = new System.Drawing.Point(4, 6);
            this._directionLabel.Name = "_directionLabel";
            this._directionLabel.Size = new System.Drawing.Size(47, 12);
            this._directionLabel.TabIndex = 0;
            this._directionLabel.Text = "方向(&D):";
            // 
            // _directionComboBox
            // 
            this._directionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._directionComboBox.FormattingEnabled = true;
            this._directionComboBox.Items.AddRange(new object[] {
            "上",
            "下",
            "左",
            "右",
            "左上",
            "右上",
            "左下",
            "右下"});
            this._directionComboBox.Location = new System.Drawing.Point(73, 3);
            this._directionComboBox.Name = "_directionComboBox";
            this._directionComboBox.Size = new System.Drawing.Size(91, 20);
            this._directionComboBox.TabIndex = 1;
            this._directionComboBox.SelectedIndexChanged += new System.EventHandler(this._directionComboBox_SelectedIndexChanged);
            // 
            // _endColorLabel
            // 
            this._endColorLabel.AutoSize = true;
            this._endColorLabel.Location = new System.Drawing.Point(4, 69);
            this._endColorLabel.Name = "_endColorLabel";
            this._endColorLabel.Size = new System.Drawing.Size(58, 12);
            this._endColorLabel.TabIndex = 0;
            this._endColorLabel.Text = "終了色(&E):";
            // 
            // _showEndColorDialogButton
            // 
            this._showEndColorDialogButton.BackColor = System.Drawing.SystemColors.Control;
            this._showEndColorDialogButton.Location = new System.Drawing.Point(104, 64);
            this._showEndColorDialogButton.Name = "_showEndColorDialogButton";
            this._showEndColorDialogButton.Size = new System.Drawing.Size(60, 23);
            this._showEndColorDialogButton.TabIndex = 5;
            this._showEndColorDialogButton.Text = "選択(&F)";
            this._showEndColorDialogButton.UseVisualStyleBackColor = false;
            this._showEndColorDialogButton.Click += new System.EventHandler(this._showEndColorDialogButton_Click);
            // 
            // _endColorPanel
            // 
            this._endColorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._endColorPanel.Location = new System.Drawing.Point(73, 65);
            this._endColorPanel.Name = "_endColorPanel";
            this._endColorPanel.Size = new System.Drawing.Size(25, 20);
            this._endColorPanel.TabIndex = 4;
            // 
            // _startColorLabel
            // 
            this._startColorLabel.AutoSize = true;
            this._startColorLabel.Location = new System.Drawing.Point(4, 37);
            this._startColorLabel.Name = "_startColorLabel";
            this._startColorLabel.Size = new System.Drawing.Size(58, 12);
            this._startColorLabel.TabIndex = 0;
            this._startColorLabel.Text = "開始色(&S):";
            // 
            // _startColorPanel
            // 
            this._startColorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._startColorPanel.Location = new System.Drawing.Point(73, 33);
            this._startColorPanel.Name = "_startColorPanel";
            this._startColorPanel.Size = new System.Drawing.Size(25, 20);
            this._startColorPanel.TabIndex = 4;
            // 
            // _showStartColorDialogButton
            // 
            this._showStartColorDialogButton.BackColor = System.Drawing.SystemColors.Control;
            this._showStartColorDialogButton.Location = new System.Drawing.Point(104, 32);
            this._showStartColorDialogButton.Name = "_showStartColorDialogButton";
            this._showStartColorDialogButton.Size = new System.Drawing.Size(60, 23);
            this._showStartColorDialogButton.TabIndex = 5;
            this._showStartColorDialogButton.Text = "選択(&T)";
            this._showStartColorDialogButton.UseVisualStyleBackColor = false;
            this._showStartColorDialogButton.Click += new System.EventHandler(this._showStartColorDialogButton_Click);
            // 
            // GradientBackgroundConfigurator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._showStartColorDialogButton);
            this.Controls.Add(this._showEndColorDialogButton);
            this.Controls.Add(this._startColorPanel);
            this.Controls.Add(this._endColorPanel);
            this.Controls.Add(this._directionComboBox);
            this.Controls.Add(this._startColorLabel);
            this.Controls.Add(this._endColorLabel);
            this.Controls.Add(this._directionLabel);
            this.Name = "GradientBackgroundConfigurator";
            this.Size = new System.Drawing.Size(189, 108);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _directionLabel;
        private System.Windows.Forms.ComboBox _directionComboBox;
        private System.Windows.Forms.Label _endColorLabel;
        private System.Windows.Forms.Button _showEndColorDialogButton;
        private System.Windows.Forms.Panel _endColorPanel;
        private System.Windows.Forms.Label _startColorLabel;
        private System.Windows.Forms.Panel _startColorPanel;
        private System.Windows.Forms.Button _showStartColorDialogButton;
    }
}
