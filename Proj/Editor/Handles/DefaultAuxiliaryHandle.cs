/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;

namespace Mkamo.Editor.Handles {
    /// <summary>
    /// 継承を使わずにユーザ定義の処理をイベントハンドラとして実装して利用することを
    /// 目的としたクラス．
    /// </summary>
    public class DefaultAuxiliaryHandle: AbstractAuxiliaryHandle {
        // ========================================
        // field
        // ========================================
        private IFigure _figure;
        private Action<IFigure, IFigure> _figureRelocator;

        // ========================================
        // constructor
        // ========================================
        /// <summary>
        /// figureはこのハンドラの表示を行うIFigure，
        /// figureRelocatorはEditor.FigureのBoundsが変更されたときに
        /// このハンドラのfigureの位置を調整するための関数を渡す．
        /// figureRelocatorの引数にはこのハンドラのfigureとEditorのfigureが渡される．
        /// </summary>
        public DefaultAuxiliaryHandle(IFigure figure, Action<IFigure, IFigure> figureRelocator) {
            _figure = figure;
            _figureRelocator = figureRelocator;
        }

        // ========================================
        // property
        // ========================================
        public override IFigure Figure {
            get { return _figure; }
        }

        // ========================================
        // method
        // ========================================
        public override void Relocate(IFigure hostFigure) {
            if (_figureRelocator != null) {
                _figureRelocator(_figure, hostFigure);
            }
        }
    }
}
