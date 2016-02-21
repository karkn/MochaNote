/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Mkamo.Common.Core;
using System.ComponentModel;
using Mkamo.Memopad.Core;

namespace Mkamo.Memopad.Internal.Core {
    internal class BackupExecutor: IDisposable {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // ========================================
        // field
        // ========================================
        private Timer _checkTimer = new Timer();
        private Timer _idleTimer = new Timer();

        private Lazy<BackgroundWorker> _worker;

        private string _dailyBackupPath;
        private string _weeklyBackupPath;
        private string _monthlyBackupPath;

        private bool _needDailyBackup = false;
        private bool _needWeeklyBackup = false;
        private bool _needMonthlyBackup = false;

        private bool _inDailyBackup = false;
        private bool _inWeeklyBackup = false;
        private bool _inMonthlyBackup = false;

        private bool _lockIdle = false;

        // ========================================
        // constructor
        // ========================================
        public BackupExecutor(
            string dailyBackupPath,
            string weeklyBackupPath,
            string monthlyBackupPath
        ) {
            _checkTimer.Interval = 600000; /// 10min * 60 * 1000
            //_checkTimer.Interval = 10000;
            _checkTimer.Tick += HandleCheckTimerTick;
            _checkTimer.Enabled = false;

            _idleTimer.Interval = 300000; /// 5min * 60 * 1000
            //_idleTimer.Interval = 10000;
            _idleTimer.Tick += HandleIdleTimerTick;
            _idleTimer.Enabled = false;

            _dailyBackupPath = dailyBackupPath;
            _weeklyBackupPath = weeklyBackupPath;
            _monthlyBackupPath = monthlyBackupPath;

            _worker = new Lazy<BackgroundWorker>(CreateWorker);
        }

        private BackgroundWorker CreateWorker() {
            var ret = new BackgroundWorker();
            ret.DoWork += WorkerDoWork;
            ret.RunWorkerCompleted += WorkerRunWorkerCompleted;
            return ret;
        }

        // ========================================
        // destructor
        // ========================================
        public virtual void Dispose() {
            _checkTimer.Stop();
            _idleTimer.Stop();
            
            _checkTimer.Dispose();
            _idleTimer.Dispose();

            if (_worker.IsValueCreated) {
                _worker.Value.Dispose();
            }
        }

        // ========================================
        // event
        // ========================================
        public event EventHandler DailyBackupStarted;
        public event EventHandler DailyBackupFinished;
        public event EventHandler WeeklyBackupStarted;
        public event EventHandler WeeklyBackupFinished;
        public event EventHandler MonthlyBackupStarted;
        public event EventHandler MonthlyBackupFinished;

        // ========================================
        // property
        // ========================================
        public bool LockIdle {
            get { return _lockIdle; }
            set { _lockIdle = value; }
        }


        public bool NeedDailyBackup {
            get { return _needDailyBackup; }
        }

        public bool NeedWeeklyBackup {
            get { return _needWeeklyBackup; }
        }

        public bool NeedMonthlyBackup {
            get { return _needMonthlyBackup; }
        }

        // ========================================
        // method
        // ========================================
        public void Start() {
            _checkTimer.Start();
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected void OnDailyBackupStarted() {
            var handler = DailyBackupStarted;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        protected void OnDailyBackupFinished() {
            var handler = DailyBackupFinished;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        protected void OnWeeklyBackupStarted() {
            var handler = WeeklyBackupStarted;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        protected void OnWeeklyBackupFinished() {
            var handler = WeeklyBackupFinished;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        protected void OnMonthlyBackupStarted() {
            var handler = MonthlyBackupStarted;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        protected void OnMonthlyBackupFinished() {
            var handler = MonthlyBackupFinished;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        // ------------------------------
        // private
        // ------------------------------
        private bool NeedBackup(string backupDirPath, Func<DateTime, bool> judge) {
            var backupDir = new DirectoryInfo(backupDirPath);
            if (!backupDir.Exists) {
                return true;
            }

            var notePath = Path.Combine(backupDirPath, "note.sdf");
            var exDataPath = Path.Combine(backupDirPath, "exdata.sdf");
            var noteFile = new FileInfo(notePath);
            var exDataFile = new FileInfo(exDataPath);

            if (!noteFile.Exists || !exDataFile.Exists) {
                return true;
            }

            var origNoteFile = new FileInfo(MemopadConsts.MemoSdfFilePath);
            var origExDataFile = new FileInfo(MemopadConsts.ExtendedDataSdfFilePath);
            if (
                noteFile.LastWriteTime.Date == origNoteFile.LastWriteTime.Date &&
                exDataFile.LastWriteTime.Date == origExDataFile.LastWriteTime.Date
            ) {
                /// バックアップ元とバックアップ先が同じ日付なら何もしない
                return false;
            }

            if (judge(noteFile.LastWriteTime.Date)) {
                return true;
            }

            if (judge(exDataFile.LastWriteTime.Date)) {
                return true;
            }

            return false;
        }

        private void WorkerDoWork(object sender, DoWorkEventArgs e) {
            Logger.Debug("Backup: do work");

            var sync = new MemoDataFolderSync(null);
            if (_needDailyBackup) {
                _inDailyBackup = true;
                _needDailyBackup = false;
                OnDailyBackupStarted();
                lock (MemopadConsts.DataLock) {
                    sync.SyncMemoDataFoldersTo(_dailyBackupPath);
                }
            }

            if (_needWeeklyBackup) {
                _inWeeklyBackup = true;
                _needWeeklyBackup = false;
                OnWeeklyBackupStarted();
                lock (MemopadConsts.DataLock) {
                    sync.SyncMemoDataFoldersTo(_weeklyBackupPath);
                }
            }

            if (_needMonthlyBackup) {
                _inMonthlyBackup = true;
                _needMonthlyBackup = false;
                OnMonthlyBackupStarted();
                lock (MemopadConsts.DataLock) {
                    sync.SyncMemoDataFoldersTo(_monthlyBackupPath);
                }
            }

        }

        private void WorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            Logger.Debug("Backup: complete");

            if (e.Error != null) {
                Logger.Warn("Backup failed.", e.Error);
            }

            if (_inDailyBackup) {
                OnDailyBackupFinished();
                _inDailyBackup = false;
            }
            if (_inWeeklyBackup) {
                OnWeeklyBackupFinished();
                _inWeeklyBackup = false;
            }
            if (_inMonthlyBackup) {
                OnMonthlyBackupFinished();
                _inMonthlyBackup = false;
            }

            _checkTimer.Start();
        }

        // --- handler ---
        private void HandleCheckTimerTick(object sender, EventArgs e) {
            Logger.Debug("Backup: check tick");

            /// バックアップが必要かどうかをチェック
            /// 存在チェックと書き込みタイムスタンプチェック
            var today = DateTime.Today;

            if (
                _needDailyBackup || _needWeeklyBackup || _needMonthlyBackup ||
                _inDailyBackup || _inWeeklyBackup || _inMonthlyBackup
            ) {
                /// どれかのバックアップが予約済みまたは実行中なら新たな予約のチェックはしない
                return;
            }

            _needDailyBackup = NeedBackup(
                _dailyBackupPath,
                date => {
                    var yesterday = today;
                    yesterday.AddDays(-1);
                    return date < yesterday;
                }
            );
            if (_needDailyBackup) {
                Application.Idle += HandleApplicationIdle;
                /// 他のバックアップは同時に予約しない
                return;
            }

            _needWeeklyBackup = NeedBackup(
                _weeklyBackupPath,
                date => {
                    var firstDayOfWeek1 = DateTimeUtil.GetFirstDayOfWeek(today, DayOfWeek.Sunday);
                    var firstDayOfWeek2 = DateTimeUtil.GetFirstDayOfWeek(date, DayOfWeek.Sunday);
                    return firstDayOfWeek1 != firstDayOfWeek2;
                }
            );
            if (_needWeeklyBackup) {
                Application.Idle += HandleApplicationIdle;
                /// 他のバックアップは同時に予約しない
                return;
            }

            _needMonthlyBackup = NeedBackup(
                _monthlyBackupPath,
                date => today.Year != date.Year || today.Month != date.Month
            );
            if (_needMonthlyBackup) {
                Application.Idle += HandleApplicationIdle;
            }
        }

        private void HandleIdleTimerTick(object sender, EventArgs e) {
            Logger.Debug("Backup: idle tick");

            Application.Idle -= HandleApplicationIdle;
            _idleTimer.Stop();

            /// バックアップ処理
            if (_needDailyBackup || _needWeeklyBackup || _needMonthlyBackup) {
                _worker.Value.RunWorkerAsync();
            }
        }

        private void HandleApplicationIdle(object sender, EventArgs e) {
            Logger.Debug("Backup: app idle");

            _checkTimer.Stop();
            if (!_lockIdle) {
                _idleTimer.Stop();
            }
            _idleTimer.Start();
        }

    }
}
