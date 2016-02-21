/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Mkamo.Common.Forms.KeyMap {
    /// <summary>
    /// 「キー入力を消費する」とは，
    /// キーに対応するアクションが実行されたときに
    /// そのキー入力に対する処理を完了したとみなすか，
    /// そのまま続けてPressKeyイベントなどを起こすべきかを表す．
    /// </summary>
    public interface IKeyMap<TTarget> {
        // ========================================
        // method
        // ========================================
        // --- action ---
        /// <summary>
        /// keyに対してactionが登録されているかどうかを返す．
        /// </summary>
        bool IsDefined(Keys key);

        /// <summary>
        /// keyにactionを設定する．
        /// isConsumeKeyはactionが実行されるときにキー入力を消費すべきかどうか．
        /// </summary>
        void SetAction(Keys key, Action<TTarget> action, bool isConsumeKey);
        /// <summary>
        /// isConsumeKeyにtrueを渡すオーバーロード．
        /// </summary>
        void SetAction(Keys key, Action<TTarget> action);

        void UnsetAction(Keys key);

        /// <summary>
        /// keyに対して登録されたactionを実行してキー入力を消費すべきかどうかを返すFuncを返す．
        /// </summary>
        Func<TTarget, bool> GetAction(Keys key);

        // --- prefix ---
        bool IsPrefixDefined(Keys key);

        /// <summary>
        /// transitActionはPrefixedKeyMapへの移行時に呼ばれるAction，nullなら何もしない．
        /// returnActionはPrefixedKeyMapから復帰時に呼ばれるAction，nullなら何もしない．
        /// returnJudgeはPrefixedKeyMapから復帰するかを判断するFunc，nullなら何が押されても復帰．
        /// isConsumeKeyOnReturnはPrefixedKeyMapから復帰するときにキー入力を消費すべきかどうか．
        /// </summary>
        KeyMap<TTarget> SetPrefix(
            Keys key,
            Action<Keys, TTarget> transitAction,
            Action<Keys, TTarget> returnAction,
            Func<Keys, TTarget, bool> returnJudge,
            bool isConsumeKeyOnReturn
        );
        /// <summary>
        /// transitAction，returnAction，returnJudgeにnullを
        /// isConsumeKeyOnReturnにtrueを渡すオーバーロード．
        /// </summary>
        KeyMap<TTarget> SetPrefix(Keys key);

        void UnsetPrefix(Keys key);
        KeyMap<TTarget> GetPrefixKeyMap(Keys key);

        void Clear();

        /// <summary>
        /// 現在入力されたプレフィクスキーを無効にして
        /// 何もキーが入力されていない状態に戻す．
        /// </summary>
        void ClearPrefixInput();

    }
}
