/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.StyledText.Core;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Common.Forms.KeyMap;

namespace Mkamo.Editor.Focuses {
    public static class StyledTextFocusKeyActions {
        [KeyAction("キャレットを左に移動")]
        public static void MoveBackwardChar(IFocus focus) {
            ((StyledTextFocus) focus).MoveBackwardChar();
        }

        [KeyAction("キャレットを右に移動")]
        public static void MoveForwardChar(IFocus focus) {
            ((StyledTextFocus) focus).MoveForwardChar();
        }

        [KeyAction("キャレットを上に移動")]
        public static void MovePreviousLine(IFocus focus) {
            ((StyledTextFocus) focus).MovePreviousLine();
        }

        [KeyAction("キャレットを下に移動")]
        public static void MoveNextLine(IFocus focus) {
            ((StyledTextFocus) focus).MoveNextLine();
        }

        [KeyAction("キャレットを1ページ上に移動")]
        public static void MovePreviousPage(IFocus focus) {
            ((StyledTextFocus) focus).MovePreviousPage();
        }

        [KeyAction("キャレットを1ページ下に移動")]
        public static void MoveNextPage(IFocus focus) {
            ((StyledTextFocus) focus).MoveNextPage();
        }

        [KeyAction("キャレットを行頭に移動")]
        public static void MoveBeginningOfLine(IFocus focus) {
            ((StyledTextFocus) focus).MoveBeginningOfLine();
        }
    
        [KeyAction("キャレットを行末に移動")]
        public static void MoveEndOfLine(IFocus focus) {
            ((StyledTextFocus) focus).MoveEndOfLine();
        }
    
        [KeyAction("キャレットを文頭に移動")]
        public static void MoveBeginningOfText(IFocus focus) {
            ((StyledTextFocus) focus).MoveBeginningOfText();
        }
    
        [KeyAction("キャレットを文末に移動")]
        public static void MoveEndOfText(IFocus focus) {
            ((StyledTextFocus) focus).MoveEndOfText();
        }
    
        [KeyAction("キャレットを一単語左に移動")]
        public static void MovePreviousWord(IFocus focus) {
            ((StyledTextFocus) focus).MovePreviousWord();
        }
    
        [KeyAction("キャレットを一単語右に移動")]
        public static void MoveNextWord(IFocus focus) {
            ((StyledTextFocus) focus).MoveNextWord();
        }
    
        [KeyAction("キャレットを一文字左に移動して範囲選択")]
        public static void SelectBackwardChar(IFocus focus) {
            ((StyledTextFocus) focus).SelectBackwardChar();
        }
    
        [KeyAction("キャレットを一文字右に移動して範囲選択")]
        public static void SelectForwardChar(IFocus focus) {
            ((StyledTextFocus) focus).SelectForwardChar();
        }
    
        [KeyAction("キャレットを一行上に移動して範囲選択")]
        public static void SelectPreviousLine(IFocus focus) {
            ((StyledTextFocus) focus).SelectPreviousLine();
        }
    
        [KeyAction("キャレットを一行下に移動して範囲選択")]
        public static void SelectNextLine(IFocus focus) {
            ((StyledTextFocus) focus).SelectNextLine();
        }
    
        [KeyAction("すべて選択")]
        public static void SelectAll(IFocus focus) {
            ((StyledTextFocus) focus).SelectAll();
        }
    
        [KeyAction("改段落")]
        public static void InsertBlockBreak(IFocus focus) {
            ((StyledTextFocus) focus).InsertBlockBreak();
        }
    
        [KeyAction("改行")]
        public static void InsertLineBreak(IFocus focus) {
            ((StyledTextFocus) focus).InsertLineBreak();
        }

        [KeyAction("キャレットの右に改行")]
        public static void OpenLineBreak(IFocus focus) {
            ((StyledTextFocus) focus).OpenLineBreak();
        }

        [KeyAction("確定")]
        public static void CommitAndSelect(IFocus focus) {
            ((StyledTextFocus) focus).CommitAndSelect();
        }
    
        [KeyAction("単語補完")]
        public static void InsertDynamicAbbrev(IFocus focus) {
            ((StyledTextFocus) focus).InsertDynamicAbbrev();
        }
    
        [KeyAction("貼り付け(改行文字で改段落)")]
        public static void PasteInlinesOrText(IFocus focus) {
            ((StyledTextFocus) focus).PasteInlinesOrText(false);
        }
    
        [KeyAction("貼り付け(改行文字で改行)")]
        public static void PasteInlinesOrTextInBlock(IFocus focus) {
            ((StyledTextFocus) focus).PasteInlinesOrText(true);
        }
    
        [KeyAction("切り取り")]
        public static void Cut(IFocus focus) {
            ((StyledTextFocus) focus).Cut();
        }
    
        [KeyAction("コピー")]
        public static void Copy(IFocus focus) {
            ((StyledTextFocus) focus).Copy();
        }
    
        [KeyAction("リージョンを切り取り")]
        public static void CutRegion(IFocus focus) {
            ((StyledTextFocus) focus).CutRegion();
        }
    
        [KeyAction("リージョンを選択してコピー")]
        public static void CopyRegion(IFocus focus) {
            ((StyledTextFocus) focus).CopyRegion();
        }
    
        [KeyAction("元に戻す")]
        public static void Undo(IFocus focus) {
            ((StyledTextFocus) focus).Undo();
        }
    
        [KeyAction("やり直し")]
        public static void Redo(IFocus focus) {
            ((StyledTextFocus) focus).Redo();
        }
    
        [KeyAction("削除")]
        public static void RemoveForward(IFocus focus) {
            ((StyledTextFocus) focus).RemoveForward();
        }
    
        [KeyAction("左を削除")]
        public static void RemoveBackward(IFocus focus) {
            ((StyledTextFocus) focus).RemoveBackward();
        }
    
        [KeyAction("箇条書きトグル")]
        public static void ToggleUnorderedList(IFocus focus) {
            ((StyledTextFocus) focus).ToggleUnorderedList();
        }
    
        [KeyAction("番号付き箇条書きトグル")]
        public static void ToggleOrderedList(IFocus focus) {
            ((StyledTextFocus) focus).ToggleOrderedList();
        }
    
        [KeyAction("チェックボックス状態変更")]
        public static void ChangeToNextListState(IFocus focus) {
            ((StyledTextFocus) focus).ChangeToNextListState();
        }
    
        [KeyAction("段落を見出し1に変更")]
        public static void SetParagraphKindToHeading1(IFocus focus) {
            ((StyledTextFocus) focus).SetParagraphKind(ParagraphKind.Heading1);
        }
    
        [KeyAction("段落を見出し2に変更")]
        public static void SetParagraphKindToHeading2(IFocus focus) {
            ((StyledTextFocus) focus).SetParagraphKind(ParagraphKind.Heading2);
        }
    
        [KeyAction("段落を見出し3に変更")]
        public static void SetParagraphKindToHeading3(IFocus focus) {
            ((StyledTextFocus) focus).SetParagraphKind(ParagraphKind.Heading3);
        }

        [KeyAction("段落を見出し4に変更")]
        public static void SetParagraphKindToHeading4(IFocus focus) {
            ((StyledTextFocus) focus).SetParagraphKind(ParagraphKind.Heading4);
        }
    
        [KeyAction("段落を見出し5に変更")]
        public static void SetParagraphKindToHeading5(IFocus focus) {
            ((StyledTextFocus) focus).SetParagraphKind(ParagraphKind.Heading5);
        }
    
        [KeyAction("段落を見出し6に変更")]
        public static void SetParagraphKindToHeading6(IFocus focus) {
            ((StyledTextFocus) focus).SetParagraphKind(ParagraphKind.Heading6);
        }
    
        [KeyAction("段落を標準に変更")]
        public static void SetParagraphKindToNormal(IFocus focus) {
            ((StyledTextFocus) focus).SetParagraphKind(ParagraphKind.Normal);
        }
    
        [KeyAction("次の入力または選択範囲を太字")]
        public static void ToggleFontBold(IFocus focus) {
            ((StyledTextFocus) focus).SetFont((FontDescription font) => FontDescription.GetBoldToggled(font));
        }
    
        [KeyAction("次の入力または選択範囲を斜体")]
        public static void ToggleFontItalic(IFocus focus) {
            ((StyledTextFocus) focus).SetFont((FontDescription font) => FontDescription.GetItalicToggled(font));
        }

        [KeyAction("次の入力または選択範囲を下線")]
        public static void ToggleFontUnderline(IFocus focus) {
            ((StyledTextFocus) focus).SetFont((FontDescription font) => FontDescription.GetUnderlineToggled(font));
        }
    
        [KeyAction("マークを設定")]
        public static void SetMark(IFocus focus) {
            ((StyledTextFocus) focus).SetMark();
        }
    
        [KeyAction("キャレットをマークに移動してマークを削除")]
        public static void PopMark(IFocus focus) {
            ((StyledTextFocus) focus).PopMark();
        }
    
        [KeyAction("キャレットをマークを交換")]
        public static void ExchangeCaretAndMark(IFocus focus) {
            ((StyledTextFocus) focus).ExchangeCaretAndMark();
        }
    
        [KeyAction("単語削除")]
        public static void KillWord(IFocus focus) {
            ((StyledTextFocus) focus).KillWord();
        }
    
        [KeyAction("インデント")]
        public static void Indent(IFocus focus) {
            ((StyledTextFocus) focus).Indent();
        }
    
        [KeyAction("アウトデント")]
        public static void Outdent(IFocus focus) {
            ((StyledTextFocus) focus).Outdent();
        }
    
        [KeyAction("最後の行を貼り付け")]
        public static void PasteLastLine(IFocus focus) {
            ((StyledTextFocus) focus).PasteLastLine();
        }
    
    }
}
