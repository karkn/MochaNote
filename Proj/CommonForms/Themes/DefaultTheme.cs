/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Text;

namespace Mkamo.Common.Forms.Themes {
    public class DefaultTheme: ITheme {
        // ========================================
        // field
        // ========================================
        private Font _captionFont;
        private Font _descriptionFont;
        private Font _inputFont;
        private Font _menuFont;
        private Font _statusFont;

        private Color _darkBackColor;

        // ========================================
        // constructor
        // ========================================
        public DefaultTheme() {
            _captionFont = SystemFonts.MessageBoxFont;
            _descriptionFont = SystemFonts.SmallCaptionFont;
            //_inputFont = SystemFonts.DefaultFont;
            _inputFont = SystemFonts.MessageBoxFont;
            _menuFont = SystemFonts.MenuFont;
            _statusFont = SystemFonts.StatusFont;
        }

        public DefaultTheme(
            Font captionFont,
            Font descriptionFont,
            Font inputFont,
            Font menuFont,
            Font statusFont
        ) {
            _captionFont = captionFont;
            _descriptionFont = descriptionFont;
            _inputFont = inputFont;
            _menuFont = menuFont;
            _statusFont = statusFont;
        }

        // ========================================
        // property
        // ========================================
        // --- font ---
        public Font CaptionFont {
            get { return _captionFont; }
            set { _captionFont = value; }
        }

        public Font DescriptionFont {
            get { return _descriptionFont; }
            set { _descriptionFont = value; }
        }

        public Font InputFont {
            get { return _inputFont; }
            set { _inputFont = value; }
        }

        public Font MenuFont {
            get { return _menuFont; }
            set { _menuFont = value; }
        }

        public Font StatusFont {
            get { return _statusFont; }
            set { _statusFont = value; }
        }

        // --- color ---
        public Color DarkBackColor {
            get { return _darkBackColor; }
            set { _darkBackColor = value; }
        }

        // ========================================
        // method
        // ========================================
        public void ReplaceFont(string oldFontName, string newFontName) {
            using (var fcol = new InstalledFontCollection()) {
                var families = fcol.Families;
                if (!families.Any(family => string.Equals(family.Name, newFontName, StringComparison.OrdinalIgnoreCase))) {
                    return;
                }
            }

            if (string.Equals(_captionFont.Name, oldFontName, StringComparison.OrdinalIgnoreCase)) {
                _captionFont = new Font(newFontName, _captionFont.SizeInPoints);
            }
            if (string.Equals(_descriptionFont.Name, oldFontName, StringComparison.OrdinalIgnoreCase)) {
                _descriptionFont = new Font(newFontName, _descriptionFont.SizeInPoints);
            }
            if (string.Equals(_inputFont.Name, oldFontName, StringComparison.OrdinalIgnoreCase)) {
                _inputFont = new Font(newFontName, _inputFont.SizeInPoints);
            }
            if (string.Equals(_menuFont.Name, oldFontName, StringComparison.OrdinalIgnoreCase)) {
                _menuFont = new Font(newFontName, _menuFont.SizeInPoints);
            }
            if (string.Equals(_statusFont.Name, oldFontName, StringComparison.OrdinalIgnoreCase)) {
                _statusFont = new Font(newFontName, _statusFont.SizeInPoints);
            }
        }

    }
}
