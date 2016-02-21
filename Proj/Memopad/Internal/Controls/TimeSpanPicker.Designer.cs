namespace Mkamo.Memopad.Internal.Controls {
    partial class TimeSpanPicker {
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
            this._targetComboBox = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this._dayRadioButton = new ComponentFactory.Krypton.Toolkit.KryptonRadioButton();
            this._weekRadioButton = new ComponentFactory.Krypton.Toolkit.KryptonRadioButton();
            this._monthRadioButton = new ComponentFactory.Krypton.Toolkit.KryptonRadioButton();
            this._dateRadioButton = new ComponentFactory.Krypton.Toolkit.KryptonRadioButton();
            this._dayComboBox = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this._weekComboBox = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this._monthComboBox = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this._fromDateTimePicker = new ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker();
            this._toDateTimePicker = new ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker();
            this._dayLabel = new System.Windows.Forms.Label();
            this._weekLabel = new System.Windows.Forms.Label();
            this._monthLabel = new System.Windows.Forms.Label();
            this._fromLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize) (this._targetComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this._dayComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this._weekComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this._monthComboBox)).BeginInit();
            this.SuspendLayout();
            // 
            // _targetComboBox
            // 
            this._targetComboBox.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._targetComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._targetComboBox.DropDownWidth = 145;
            this._targetComboBox.Items.AddRange(new object[] {
            "なし",
            "更新されたノート",
            "作成されたノート",
            "アクセスされたノート"});
            this._targetComboBox.Location = new System.Drawing.Point(6, 6);
            this._targetComboBox.Name = "_targetComboBox";
            this._targetComboBox.Size = new System.Drawing.Size(210, 22);
            this._targetComboBox.TabIndex = 1;
            this._targetComboBox.Text = "なし";
            // 
            // _dayRadioButton
            // 
            this._dayRadioButton.Location = new System.Drawing.Point(6, 32);
            this._dayRadioButton.Name = "_dayRadioButton";
            this._dayRadioButton.Size = new System.Drawing.Size(48, 20);
            this._dayRadioButton.TabIndex = 2;
            this._dayRadioButton.Values.Text = "過去";
            // 
            // _weekRadioButton
            // 
            this._weekRadioButton.Location = new System.Drawing.Point(6, 58);
            this._weekRadioButton.Name = "_weekRadioButton";
            this._weekRadioButton.Size = new System.Drawing.Size(48, 20);
            this._weekRadioButton.TabIndex = 3;
            this._weekRadioButton.Values.Text = "過去";
            // 
            // _monthRadioButton
            // 
            this._monthRadioButton.Location = new System.Drawing.Point(6, 84);
            this._monthRadioButton.Name = "_monthRadioButton";
            this._monthRadioButton.Size = new System.Drawing.Size(48, 20);
            this._monthRadioButton.TabIndex = 4;
            this._monthRadioButton.Values.Text = "過去";
            // 
            // _dateRadioButton
            // 
            this._dateRadioButton.Location = new System.Drawing.Point(6, 112);
            this._dateRadioButton.Name = "_dateRadioButton";
            this._dateRadioButton.Size = new System.Drawing.Size(73, 20);
            this._dateRadioButton.TabIndex = 5;
            this._dateRadioButton.Values.Text = "日付指定";
            // 
            // _dayComboBox
            // 
            this._dayComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._dayComboBox.DropDownWidth = 40;
            this._dayComboBox.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14"});
            this._dayComboBox.Location = new System.Drawing.Point(60, 32);
            this._dayComboBox.Name = "_dayComboBox";
            this._dayComboBox.Size = new System.Drawing.Size(46, 22);
            this._dayComboBox.TabIndex = 1;
            this._dayComboBox.Text = "1";
            // 
            // _weekComboBox
            // 
            this._weekComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._weekComboBox.DropDownWidth = 40;
            this._weekComboBox.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15"});
            this._weekComboBox.Location = new System.Drawing.Point(60, 58);
            this._weekComboBox.Name = "_weekComboBox";
            this._weekComboBox.Size = new System.Drawing.Size(46, 22);
            this._weekComboBox.TabIndex = 1;
            this._weekComboBox.Text = "1";
            // 
            // _monthComboBox
            // 
            this._monthComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._monthComboBox.DropDownWidth = 40;
            this._monthComboBox.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "24"});
            this._monthComboBox.Location = new System.Drawing.Point(60, 84);
            this._monthComboBox.Name = "_monthComboBox";
            this._monthComboBox.Size = new System.Drawing.Size(46, 22);
            this._monthComboBox.TabIndex = 1;
            this._monthComboBox.Text = "1";
            // 
            // _fromDateTimePicker
            // 
            this._fromDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this._fromDateTimePicker.Location = new System.Drawing.Point(60, 132);
            this._fromDateTimePicker.Name = "_fromDateTimePicker";
            this._fromDateTimePicker.Size = new System.Drawing.Size(91, 20);
            this._fromDateTimePicker.TabIndex = 6;
            // 
            // _toDateTimePicker
            // 
            this._toDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this._toDateTimePicker.Location = new System.Drawing.Point(60, 159);
            this._toDateTimePicker.Name = "_toDateTimePicker";
            this._toDateTimePicker.Size = new System.Drawing.Size(91, 20);
            this._toDateTimePicker.TabIndex = 6;
            // 
            // _dayLabel
            // 
            this._dayLabel.AutoSize = true;
            this._dayLabel.Location = new System.Drawing.Point(109, 37);
            this._dayLabel.Name = "_dayLabel";
            this._dayLabel.Size = new System.Drawing.Size(29, 12);
            this._dayLabel.TabIndex = 7;
            this._dayLabel.Text = "日間";
            this._dayLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _weekLabel
            // 
            this._weekLabel.AutoSize = true;
            this._weekLabel.Location = new System.Drawing.Point(109, 63);
            this._weekLabel.Name = "_weekLabel";
            this._weekLabel.Size = new System.Drawing.Size(29, 12);
            this._weekLabel.TabIndex = 7;
            this._weekLabel.Text = "週間";
            this._weekLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _monthLabel
            // 
            this._monthLabel.AutoSize = true;
            this._monthLabel.Location = new System.Drawing.Point(109, 89);
            this._monthLabel.Name = "_monthLabel";
            this._monthLabel.Size = new System.Drawing.Size(39, 12);
            this._monthLabel.TabIndex = 7;
            this._monthLabel.Text = "か月間";
            this._monthLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // _fromLabel
            // 
            this._fromLabel.AutoSize = true;
            this._fromLabel.Location = new System.Drawing.Point(40, 161);
            this._fromLabel.Name = "_fromLabel";
            this._fromLabel.Size = new System.Drawing.Size(17, 12);
            this._fromLabel.TabIndex = 7;
            this._fromLabel.Text = "～";
            // 
            // TimeSpanPicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._fromLabel);
            this.Controls.Add(this._monthLabel);
            this.Controls.Add(this._weekLabel);
            this.Controls.Add(this._dayLabel);
            this.Controls.Add(this._toDateTimePicker);
            this.Controls.Add(this._fromDateTimePicker);
            this.Controls.Add(this._dateRadioButton);
            this.Controls.Add(this._monthRadioButton);
            this.Controls.Add(this._weekRadioButton);
            this.Controls.Add(this._dayRadioButton);
            this.Controls.Add(this._monthComboBox);
            this.Controls.Add(this._weekComboBox);
            this.Controls.Add(this._dayComboBox);
            this.Controls.Add(this._targetComboBox);
            this.Name = "TimeSpanPicker";
            this.Size = new System.Drawing.Size(223, 198);
            ((System.ComponentModel.ISupportInitialize) (this._targetComboBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this._dayComboBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this._weekComboBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this._monthComboBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonComboBox _targetComboBox;
        private ComponentFactory.Krypton.Toolkit.KryptonRadioButton _dayRadioButton;
        private ComponentFactory.Krypton.Toolkit.KryptonRadioButton _weekRadioButton;
        private ComponentFactory.Krypton.Toolkit.KryptonRadioButton _monthRadioButton;
        private ComponentFactory.Krypton.Toolkit.KryptonRadioButton _dateRadioButton;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox _dayComboBox;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox _weekComboBox;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox _monthComboBox;
        private ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker _fromDateTimePicker;
        private ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker _toDateTimePicker;
        private System.Windows.Forms.Label _dayLabel;
        private System.Windows.Forms.Label _weekLabel;
        private System.Windows.Forms.Label _monthLabel;
        private System.Windows.Forms.Label _fromLabel;

    }
}
