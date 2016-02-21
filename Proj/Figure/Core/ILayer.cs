/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Figure.Core {
    /// <summary>
    /// レイヤ．
    /// 自身は描画せず子Figureを格納するコンテナとなる役割を持つ．
    /// Bounds変更時に子Figureは移動しない．
    /// </summary>
    public interface ILayer {
    }
}
