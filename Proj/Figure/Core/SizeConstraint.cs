/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Mkamo.Figure.Core {
    /// <summary>
    /// サイズの制約を表す．
    /// </summary>
    public struct SizeConstraint {
        // ========================================
        // static field
        // ========================================
        public static readonly SizeConstraint Empty = new SizeConstraint(null, null);

        // ========================================
        // field
        // ========================================
        private int? _maxWidth;
        private int? _maxHeight;

        // ========================================
        // constructor
        // ========================================
        public SizeConstraint(int? maxWidth, int? maxHeight) {
            _maxWidth = maxWidth;
            _maxHeight = maxHeight;
        }

        public SizeConstraint(Size maxSize) {
            _maxWidth = maxSize.Width;
            _maxHeight = maxSize.Height;
        }

        // ========================================
        // property
        // ========================================
        /// <summary>
        /// 制約サイズ幅．
        /// </summary>
        public int? MaxWidth {
            get { return _maxWidth; }
        }

        /// <summary>
        /// 制約サイズ高さ．
        /// </summary>
        public int? MaxHeight {
            get { return _maxHeight; }
        }

        // ========================================
        // method
        // ========================================
        /// <summary>
        /// widthが制約サイズを超えているかどうか．
        /// </summary>
        public bool IsWidthOver(int width) {
            return MaxWidth.HasValue && MaxWidth.Value < width;
        }

        /// <summary>
        /// heightが制約サイズを超えているかどうか．
        /// </summary>
        public bool IsHeightOver(int height) {
            return MaxHeight.HasValue && MaxHeight.Value < height;
        }

        /// <summary>
        /// sizeを制約サイズ内に収めたサイズを返す．
        /// </summary>
        public Size MeasureConstrainedSize(Size size) {
            return new Size(
                IsWidthOver(size.Width)? MaxWidth.Value: size.Width,
                IsHeightOver(size.Height)? MaxHeight.Value: size.Height
            );
        }

        /// <summary>
        /// 制約サイズをdelta分増加させる．
        /// </summary>
        public SizeConstraint Inflate(Size delta) {
            int? newWidth = (MaxWidth.HasValue? MaxWidth.Value + delta.Width: (int?) null);
            int? newHeight = (MaxHeight.HasValue? MaxHeight.Value + delta.Height: (int?) null);
            return new SizeConstraint(newWidth, newHeight);
        }

        /// <summary>
        /// 制約サイズをdelta分減少させる．
        /// </summary>
        public SizeConstraint Deflate(Size delta) {
            int? newWidth = (MaxWidth.HasValue? MaxWidth.Value - delta.Width: (int?) null);
            int? newHeight = (MaxHeight.HasValue? MaxHeight.Value - delta.Height: (int?) null);
            return new SizeConstraint(newWidth, newHeight);
        }
    }
}
