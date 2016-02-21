/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Memopad.Core;
using System.Windows.Forms;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Model.Memo;
using Mkamo.Memopad.Internal.Forms;

namespace Mkamo.Memopad.Internal.Utils {
    internal static class MessageUtil {
        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public static bool ConfirmEmptyTrashBox() {
            var result = MessageBox.Show(
                "ごみ箱内のノートを削除します。よろしいですか。",
                "ごみ箱内のノートの削除の確認",
                MessageBoxButtons.YesNo
            );
            return result != DialogResult.No;
        }

        public static bool ConfirmMemoRemoval(IEnumerable<MemoInfo> infos) {
            if (infos == null || !infos.Any()) {
                return false;
            }

            var facade = MemopadApplication.Instance;
            var settings = facade.Settings;
            if (!settings.ConfirmMemoRemoval) {
                return true;
            }

            var msg = MessageUtil.GetMemoRemovalMessage(infos);
            var result = MessageBox.Show(msg, "ノートの削除の確認", MessageBoxButtons.YesNo);
            return result != DialogResult.No;
        }

        public static bool ConfirmMemoRemovalCompletely(IEnumerable<MemoInfo> infos) {
            if (infos == null || !infos.Any()) {
                return false;
            }

            var msg = MessageUtil.GetMemoRemovalMessage(infos);
            var result = MessageBox.Show(msg, "ノートの削除の確認", MessageBoxButtons.YesNo);
            return result != DialogResult.No;
        }

        public static bool ConfirmTagRemoval(MemoTag tag) {
            if (tag == null) {
                return false;
            }

            var facade = MemopadApplication.Instance;
            var settings = facade.Settings;
            if (!settings.ConfirmTagRemoval) {
                return true;
            }

            var msg = "\"" + tag.Name + "\"を削除してよろしいですか?";
            var result = MessageBox.Show(msg, "タグの削除の確認", MessageBoxButtons.YesNo);
            return result != DialogResult.No;
        }

        public static bool ConfirmFolderRemoval(MemoFolder folder) {
            if (folder == null) {
                return false;
            }

            var facade = MemopadApplication.Instance;
            var settings = facade.Settings;
            if (!settings.ConfirmFolderRemoval) {
                return true;
            }

            var msg = "\"" + folder.Name + "\"を削除してよろしいですか?";
            var result = MessageBox.Show(msg, "クリアファイルの削除の確認", MessageBoxButtons.YesNo);
            return result != DialogResult.No;
        }

        public static bool ConfirmSmartFolderRemoval(MemoSmartFolder smartFolder) {
            if (smartFolder == null) {
                return false;
            }

            var facade = MemopadApplication.Instance;
            var settings = facade.Settings;
            if (!settings.ConfirmSmartFolderRemoval) {
                return true;
            }

            var msg = "\"" + smartFolder.Name + "\"を削除してよろしいですか?";
            var result = MessageBox.Show(msg, "スマートフォルダの削除の確認", MessageBoxButtons.YesNo);
            return result != DialogResult.No;
        }

        public static void IntroducePremiumLicense() {
            using (var dialog = new EncourageUpgradeLicenseForm()) {
                var app = MemopadApplication.Instance;

                dialog.Message =
                    "Confidanteのご利用ありがとうございます。" + Environment.NewLine +
                    "プレミアムライセンスにアップグレードするとさらに便利な機能が使用できます。";
                dialog.Font = app.Theme.CaptionFont;

                dialog.ShowDialog(app.MainForm);
            }
        }

        public static void IntroducePremiumLicenseExtension() {
            using (var dialog = new EncourageUpgradeLicenseForm()) {
                var app = MemopadApplication.Instance;

                dialog.Message =
                    "プレミアムライセンスのバージョンアップ保証期間が終了しました。" + Environment.NewLine +
                    "次回のバージョンアップからはプレミアムライセンスの機能が使用できなくなります。";
                dialog.Font = app.Theme.CaptionFont;
                dialog.StartPosition = FormStartPosition.CenterScreen;

                dialog.ShowDialog(app.MainForm);
            }
        }

        public static void AlertMaxMemoCreationCount() {
            using (var dialog = new EncourageUpgradeLicenseForm()) {
                var app = MemopadApplication.Instance;

                dialog.Message =
                    "作成できるノートは一か月につき" + MemopadConsts.UnlicensedMaxMemoCreationCount + "個までです。" + Environment.NewLine +
                    "プレミアムライセンスにアップグレードすると無制限にノートを作成できます。";
                dialog.Font = app.Theme.CaptionFont;

                dialog.ShowDialog(app.MainForm);
            }
        }

        public static void AlertMaxTagCreationCount() {
            using (var dialog = new EncourageUpgradeLicenseForm()) {
                var app = MemopadApplication.Instance;

                dialog.Message =
                    "作成できるタグは" + MemopadConsts.UnlicensedMaxTagCreationCount + "個までです。" + Environment.NewLine +
                    "プレミアムライセンスにアップグレードすると無制限にタグを作成できます。";
                dialog.Font = app.Theme.CaptionFont;

                dialog.ShowDialog(app.MainForm);
            }
        }

        public static void AlertMaxMemoLoadCount() {
            using (var dialog = new EncourageUpgradeLicenseForm()) {
                var app = MemopadApplication.Instance;

                dialog.Message =
                    "閲覧できるノートは一か月につき" + MemopadConsts.UnlicensedMaxMemoLoadCount + "個までです。" + Environment.NewLine +
                    "プレミアムライセンスにアップグレードすると無制限にノートを閲覧できます。";
                dialog.Font = app.Theme.CaptionFont;

                dialog.ShowDialog(app.MainForm);
            }
        }

        // ------------------------------
        // private
        // ------------------------------
        private static string GetMemoRemovalMessage(IEnumerable<MemoInfo> infos) {
            var ret = string.Empty;

            var count = infos.Count();
            if (count == 0) {
                ret = "";
            } else if (count == 1) {
                ret = "\"" + infos.First().Title + "\"を削除してよろしいですか?";
            } else {
                ret = "選択された" + count + "個のノートを削除してよろしいですか?";
            }

            return ret;
        }

    }
}
