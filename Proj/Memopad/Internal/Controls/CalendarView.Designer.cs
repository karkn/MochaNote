namespace Mkamo.Memopad.Internal.Controls {
    partial class CalendarView {
        /// <summary> 
        /// 必要なデザイナー変数です。
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

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent() {
            this._monthCalendar = new ComponentFactory.Krypton.Toolkit.KryptonMonthCalendar();
            this.SuspendLayout();
            // 
            // _monthCalendar
            // 
            this._monthCalendar.Location = new System.Drawing.Point(17, 3);
            this._monthCalendar.Name = "_monthCalendar";
            this._monthCalendar.Size = new System.Drawing.Size(230, 184);
            this._monthCalendar.TabIndex = 0;
            // 
            // CalendarView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._monthCalendar);
            this.Name = "CalendarView";
            this.Size = new System.Drawing.Size(269, 577);
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonMonthCalendar _monthCalendar;
    }
}
