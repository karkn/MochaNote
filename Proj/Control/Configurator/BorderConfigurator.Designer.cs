namespace Mkamo.Control.Configurator {
    partial class BorderConfigurator {
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
            this.panel1 = new System.Windows.Forms.Panel();
            this._borderRadioButton = new System.Windows.Forms.RadioButton();
            this._noBorderRadioButton = new System.Windows.Forms.RadioButton();
            this._lineConfigurator = new Mkamo.Control.Configurator.LineConfigurator();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this._borderRadioButton);
            this.panel1.Controls.Add(this._noBorderRadioButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(281, 52);
            this.panel1.TabIndex = 5;
            // 
            // _borderRadioButton
            // 
            this._borderRadioButton.AutoSize = true;
            this._borderRadioButton.Location = new System.Drawing.Point(7, 28);
            this._borderRadioButton.Name = "_borderRadioButton";
            this._borderRadioButton.Size = new System.Drawing.Size(53, 16);
            this._borderRadioButton.TabIndex = 4;
            this._borderRadioButton.TabStop = true;
            this._borderRadioButton.Text = "枠あり";
            this._borderRadioButton.UseVisualStyleBackColor = true;
            this._borderRadioButton.CheckedChanged += new System.EventHandler(this._borderRadioButton_CheckedChanged);
            // 
            // _noBorderRadioButton
            // 
            this._noBorderRadioButton.AutoSize = true;
            this._noBorderRadioButton.Location = new System.Drawing.Point(7, 6);
            this._noBorderRadioButton.Name = "_noBorderRadioButton";
            this._noBorderRadioButton.Size = new System.Drawing.Size(54, 16);
            this._noBorderRadioButton.TabIndex = 3;
            this._noBorderRadioButton.TabStop = true;
            this._noBorderRadioButton.Text = "枠なし";
            this._noBorderRadioButton.UseVisualStyleBackColor = true;
            this._noBorderRadioButton.CheckedChanged += new System.EventHandler(this._noBorderRadioButton_CheckedChanged);
            // 
            // _lineConfigurator
            // 
            this._lineConfigurator.BackColor = System.Drawing.Color.Transparent;
            this._lineConfigurator.Dock = System.Windows.Forms.DockStyle.Fill;
            this._lineConfigurator.IsModified = true;
            this._lineConfigurator.LineColor = System.Drawing.Color.Transparent;
            this._lineConfigurator.LineDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            this._lineConfigurator.LineWidth = 0;
            this._lineConfigurator.Location = new System.Drawing.Point(0, 52);
            this._lineConfigurator.Name = "_lineConfigurator";
            this._lineConfigurator.Size = new System.Drawing.Size(281, 238);
            this._lineConfigurator.TabIndex = 6;
            // 
            // BorderConfigurator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this._lineConfigurator);
            this.Controls.Add(this.panel1);
            this.Name = "BorderConfigurator";
            this.Size = new System.Drawing.Size(281, 290);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton _borderRadioButton;
        private System.Windows.Forms.RadioButton _noBorderRadioButton;
        private LineConfigurator _lineConfigurator;
    }
}
