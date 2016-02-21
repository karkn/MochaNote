/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Mkamo.Common.Forms.Descriptions {
    public static class SystemFontDescriptions {
        // ========================================
        // static field
        // ========================================
        private static FontDescription _captionFont;
        private static FontDescription _defaultFont;
        private static FontDescription _dialogFont;
        private static FontDescription _iconTitleFont;
        private static FontDescription _menuFont;
        private static FontDescription _messageBoxFont;
        private static FontDescription _smallCaptionFont;
        private static FontDescription _statusFont;

        // ========================================
        // static property
        // ========================================
        public static FontDescription SystemCaptionFont {
            get {
                if (_captionFont == null) {
                    _captionFont = new FontDescription(SystemFonts.CaptionFont);
                }
                return _captionFont;
            }
        }

        public static FontDescription DefaultFont {
            get {
                if (_defaultFont == null) {
                    _defaultFont = new FontDescription(SystemFonts.DefaultFont);
                }
                return _defaultFont;
            }
        }

        public static FontDescription DialogFont {
            get {
                if (_dialogFont == null) {
                    _dialogFont = new FontDescription(SystemFonts.DialogFont);
                }
                return _dialogFont;
            }
        }

        public static FontDescription IconTitleFont {
            get {
                if (_iconTitleFont == null) {
                    _iconTitleFont = new FontDescription(SystemFonts.IconTitleFont);
                }
                return _iconTitleFont;
            }
        }

        public static FontDescription MenuFont {
            get {
                if (_menuFont == null) {
                    _menuFont = new FontDescription(SystemFonts.MenuFont);
                }
                return _menuFont;
            }
        }

        public static FontDescription MessageBoxFont {
            get {
                if (_messageBoxFont == null) {
                    _messageBoxFont = new FontDescription(SystemFonts.MessageBoxFont);
                }
                return _messageBoxFont;
            }
        }

        public static FontDescription SmallCaptionFont {
            get {
                if (_smallCaptionFont == null) {
                    _smallCaptionFont = new FontDescription(SystemFonts.SmallCaptionFont);
                }
                return _smallCaptionFont;
            }
        }

        public static FontDescription StatusFont {
            get {
                if (_statusFont == null) {
                    _statusFont = new FontDescription(SystemFonts.StatusFont);
                }
                return _statusFont;
            }
        }

    }
}
