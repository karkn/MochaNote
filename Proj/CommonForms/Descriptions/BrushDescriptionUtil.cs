/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Common.Forms.Descriptions {
    public static class BrushDescriptionUtil {
        // ========================================
        // static method
        // ========================================
        public static IBrushDescription CreateFrom(IBrushDescription prototype, float opacity) {
            if (prototype == null) {
                return null;
            }
            switch (prototype.Kind) {
                case BrushKind.Solid: {
                    var solid = prototype as SolidBrushDescription;
                    return new SolidBrushDescription(solid.Color, opacity);
                }
                case BrushKind.Gradient: {
                    var gradient = prototype as GradientBrushDescription;
                    if (gradient.ColorBlend != null) {
                        return new GradientBrushDescription(
                            gradient.ColorBlend,
                            gradient.Angle,
                            opacity
                        );
                    } else {
                        return new GradientBrushDescription(
                            gradient.Color1,
                            gradient.Color2,
                            gradient.Blend,
                            gradient.Angle,
                            opacity
                        );
                    }
                }
            }
            throw new ArgumentException("prototype is unknown kind of IBrushDescriptor");
        }
    }
}
