/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.Serialization;

namespace Mkamo.Common.Forms.Descriptions {

    /// <summary>
    /// Font情報を格納するクラス．Immutable．
    /// </summary>
    [Serializable]
    [DataContract]
    public sealed class FontDescription: ICloneable {
        // ========================================
        // static field
        // ========================================
        public static FontDescription GetBoldToggled(FontDescription prototype) {
            return new FontDescription(prototype, prototype.IsBold ? prototype.Style & ~FontStyle.Bold : prototype.Style | FontStyle.Bold);
        }

        public static FontDescription GetItalicToggled(FontDescription prototype) {
            return new FontDescription(prototype, prototype.IsItalic ? prototype.Style & ~FontStyle.Italic : prototype.Style | FontStyle.Italic);
        }

        public static FontDescription GetUnderlineToggled(FontDescription prototype) {
            return new FontDescription(prototype, prototype.IsUnderline ? prototype.Style & ~FontStyle.Underline : prototype.Style | FontStyle.Underline);
        }

        public static FontDescription GetStrikeoutToggled(FontDescription prototype) {
            return new FontDescription(prototype, prototype.IsStrikeout ? prototype.Style & ~FontStyle.Strikeout : prototype.Style | FontStyle.Strikeout);
        }

        // ========================================
        // field
        // ========================================
        [DataMember]
        private string _name;
        [DataMember]
        private float _size;

        [DataMember]
        private FontStyle _style;
        [DataMember]
        private GraphicsUnit? _unit;

        // ========================================
        // constructor
        // ========================================
        public FontDescription(string name, float size, FontStyle style, GraphicsUnit? unit) {
            _name = name;
            _size = size;

            _style = style;
            _unit = unit;
        }

        public FontDescription(string name, float size, FontStyle style)
            : this(name, size, style, null) {
        }

        public FontDescription(string name, float size) :
            this(name, size, FontStyle.Regular, null) {
        }

        public FontDescription(FontDescription prototype, string name):
            this(name, prototype.Size, prototype.Style, prototype.Unit) {
        }

        public FontDescription(FontDescription prototype, float size):
            this(prototype.Name, size, prototype.Style, prototype.Unit) {
        }

        public FontDescription(FontDescription prototype, FontStyle style):
            this(prototype.Name, prototype.Size, style, prototype.Unit) {
        }

        public FontDescription(Font prototype):
            this(prototype.Name, prototype.Size, prototype.Style, prototype.Unit) {
        }

        // ========================================
        // property
        // ========================================
        public string Name {
            get { return _name; }
        }

        public float Size {
            get { return _size; }
        }

        public FontStyle Style {
            get { return _style; }
        }

        public GraphicsUnit? Unit {
            get { return _unit; }
        }

        public bool IsRegular {
            get { return _style == FontStyle.Regular; }
        }

        public bool IsBold {
            get { return (_style & FontStyle.Bold) == FontStyle.Bold; }
        }

        public bool IsItalic {
            get { return (_style & FontStyle.Italic) == FontStyle.Italic; }
        }

        public bool IsUnderline {
            get { return (_style & FontStyle.Underline) == FontStyle.Underline; }
        }

        public bool IsStrikeout{
            get { return (_style & FontStyle.Strikeout) == FontStyle.Strikeout; }
        }

        // ========================================
        // method
        // ========================================
        // === object ==========
        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }
            return Equals(obj as FontDescription);
        }

        public override int GetHashCode() {
            var nameHash = _name == null? 0: _name.GetHashCode();
            var sizeHash = _size.GetHashCode();
            var styleHash = _style.GetHashCode();
            var unitHash = _unit.HasValue? _unit.Value.GetHashCode(): 0;
            return nameHash ^ sizeHash ^ styleHash ^ unitHash;
        }

        // === FontDescription ==========
        public bool Equals(FontDescription other) {
            if (other == null) {
                return false;
            }
            return (
                _name == other._name &&
                _size == other._size &&
                _style == other._style &&
                _unit == other._unit
            );
        }


        public object Clone() {
            return MemberwiseClone();
        }

        public Font CreateFont() {
            if (_unit.HasValue) {
                return new Font(_name, _size, _style, _unit.Value);
            } else {
                return new Font(_name, _size, _style);
            }
        }

    }
}
