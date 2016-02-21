/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;

namespace Mkamo.Common.Forms.Drawing {
    public static class ImageUtil {
        public static ImageAttributes GetImageAttributes(float opacity) {
            var ret = new ImageAttributes();
            float[][] matrixItems ={
                new float[] {1, 0, 0, 0, 0},
                new float[] {0, 1, 0, 0, 0},
                new float[] {0, 0, 1, 0, 0},
                new float[] {0, 0, 0, opacity, 0},
                new float[] {0, 0, 0, 0, 1}
            };
            var colorMatrix = new ColorMatrix(matrixItems);
            ret.SetColorMatrix(colorMatrix);
            return ret;
        }

        public static ImageCodecInfo GetCodec(ImageFormat format) {
            var codecs = ImageCodecInfo.GetImageDecoders();
            foreach (var codec in codecs) {
                if (codec.FormatID == format.Guid) {
                    return codec;
                }
            }
            return null;
        }

        public static ImageCodecInfo GetCodec(Image image) {
            var codecs = ImageCodecInfo.GetImageDecoders();
            foreach (var codec in codecs) {
                if (codec.FormatID == image.RawFormat.Guid) {
                    return codec;
                }
            }
            return null;
        }

        public static EncoderParameters CreateEncoderParameters(long quality) {
            var encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = new EncoderParameter(
                System.Drawing.Imaging.Encoder.Quality,
                quality
            );
            return encoderParams;
        }
    }
}
