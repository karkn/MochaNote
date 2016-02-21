namespace Mkamo.Control.Configurator {
    partial class BackgroundConfigurator {
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
            this._gradientPaintRadioButton = new System.Windows.Forms.RadioButton();
            this._solidPaintRadioButton = new System.Windows.Forms.RadioButton();
            this._noPaintRadioButton = new System.Windows.Forms.RadioButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this._gradientBackgroundConfigurator = new Mkamo.Control.Configurator.GradientBackgroundConfigurator();
            this._solidBackgroundConfigurator = new Mkamo.Control.Configurator.SolidBackgroundConfigurator();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this._gradientPaintRadioButton);
            this.panel1.Controls.Add(this._solidPaintRadioButton);
            this.panel1.Controls.Add(this._noPaintRadioButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(215, 79);
            this.panel1.TabIndex = 4;
            // 
            // _gradientPaintRadioButton
            // 
            this._gradientPaintRadioButton.AutoSize = true;
            this._gradientPaintRadioButton.Location = new System.Drawing.Point(7, 50);
            this._gradientPaintRadioButton.Name = "_gradientPaintRadioButton";
            this._gradientPaintRadioButton.Size = new System.Drawing.Size(144, 16);
            this._gradientPaintRadioButton.TabIndex = 5;
            this._gradientPaintRadioButton.TabStop = true;
            this._gradientPaintRadioButton.Text = "グラデーションで塗りつぶす";
            this._gradientPaintRadioButton.UseVisualStyleBackColor = true;
            this._gradientPaintRadioButton.CheckedChanged += new System.EventHandler(this._gradientPaintRadioButton_CheckedChanged);
            // 
            // _solidPaintRadioButton
            // 
            this._solidPaintRadioButton.AutoSize = true;
            this._solidPaintRadioButton.Location = new System.Drawing.Point(7, 28);
            this._solidPaintRadioButton.Name = "_solidPaintRadioButton";
            this._solidPaintRadioButton.Size = new System.Drawing.Size(106, 16);
            this._solidPaintRadioButton.TabIndex = 4;
            this._solidPaintRadioButton.TabStop = true;
            this._solidPaintRadioButton.Text = "単色で塗りつぶす";
            this._solidPaintRadioButton.UseVisualStyleBackColor = true;
            this._solidPaintRadioButton.CheckedChanged += new System.EventHandler(this._solidPaintRadioButton_CheckedChanged);
            // 
            // _noPaintRadioButton
            // 
            this._noPaintRadioButton.AutoSize = true;
            this._noPaintRadioButton.Location = new System.Drawing.Point(7, 6);
            this._noPaintRadioButton.Name = "_noPaintRadioButton";
            this._noPaintRadioButton.Size = new System.Drawing.Size(90, 16);
            this._noPaintRadioButton.TabIndex = 3;
            this._noPaintRadioButton.TabStop = true;
            this._noPaintRadioButton.Text = "塗りつぶさない";
            this._noPaintRadioButton.UseVisualStyleBackColor = true;
            this._noPaintRadioButton.CheckedChanged += new System.EventHandler(this._noPaintRadioButton_CheckedChanged);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Controls.Add(this._gradientBackgroundConfigurator);
            this.panel2.Controls.Add(this._solidBackgroundConfigurator);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 79);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(215, 167);
            this.panel2.TabIndex = 5;
            // 
            // _gradientBackgroundConfigurator
            // 
            this._gradientBackgroundConfigurator.Angle = 0F;
            this._gradientBackgroundConfigurator.BackColor = System.Drawing.Color.Transparent;
            this._gradientBackgroundConfigurator.Dock = System.Windows.Forms.DockStyle.Fill;
            this._gradientBackgroundConfigurator.EndColor = System.Drawing.Color.Transparent;
            this._gradientBackgroundConfigurator.IsModified = false;
            this._gradientBackgroundConfigurator.Location = new System.Drawing.Point(0, 0);
            this._gradientBackgroundConfigurator.Name = "_gradientBackgroundConfigurator";
            this._gradientBackgroundConfigurator.Size = new System.Drawing.Size(215, 167);
            this._gradientBackgroundConfigurator.StartColor = System.Drawing.Color.Transparent;
            this._gradientBackgroundConfigurator.TabIndex = 5;
            // 
            // _solidBackgroundConfigurator
            // 
            this._solidBackgroundConfigurator.BackColor = System.Drawing.Color.Transparent;
            this._solidBackgroundConfigurator.Color = System.Drawing.Color.Transparent;
            this._solidBackgroundConfigurator.Dock = System.Windows.Forms.DockStyle.Fill;
            this._solidBackgroundConfigurator.IsModified = false;
            this._solidBackgroundConfigurator.Location = new System.Drawing.Point(0, 0);
            this._solidBackgroundConfigurator.Name = "_solidBackgroundConfigurator";
            this._solidBackgroundConfigurator.Opacity = 1F;
            this._solidBackgroundConfigurator.Size = new System.Drawing.Size(215, 167);
            this._solidBackgroundConfigurator.TabIndex = 4;
            // 
            // BackgroundConfigurator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "BackgroundConfigurator";
            this.Size = new System.Drawing.Size(215, 246);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton _gradientPaintRadioButton;
        private System.Windows.Forms.RadioButton _solidPaintRadioButton;
        private System.Windows.Forms.RadioButton _noPaintRadioButton;
        private System.Windows.Forms.Panel panel2;
        private SolidBackgroundConfigurator _solidBackgroundConfigurator;
        private GradientBackgroundConfigurator _gradientBackgroundConfigurator;

    }
}
