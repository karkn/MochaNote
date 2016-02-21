/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Externalize;
using System.Drawing;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Common.Disposable;
using System.Drawing.Imaging;
using Mkamo.Figure.Core;
using System.IO;
using Mkamo.Common.Forms.Drawing;

namespace Mkamo.Figure.Figures {
    public class ImageFigure: AbstractNode {
        // ========================================
        // static field
        // ========================================
        protected static readonly string ImageResource = "ImageFigure.Image";
        protected static readonly string ImageMemResource = "ImageFigure.ImageMem";

        // ========================================
        // field
        // ========================================
        private IImageDescription _imageDesc;
        private float _opacity;

        private Size _imageSize;
        private bool _needRecreateImageOnBoundsChanged;
        private bool _needImageFitted;

        // ========================================
        // constructor
        // ========================================
        public ImageFigure() {
            _opacity = 1f;

            _needRecreateImageOnBoundsChanged = true;
            _needImageFitted = true;

            _ResourceCache.RegisterResourceCreator(
                ImageMemResource,
                () => {
                    return new MemoryStream();
                },
                ResourceDisposingPolicy.Explicit
            );
            _ResourceCache.RegisterResourceCreator(
                ImageResource,
                () => {
                    var origImage = _imageDesc.CreateImage();
                    _imageSize = origImage.Size;

                    if (origImage is Metafile) {
                        /// metafile
                        using (_ResourceCache.UseResource())
                        using (var bmp = new Bitmap(1, 1))
                        using (var bmpg = Graphics.FromImage(bmp)) {
                            var mem = _ResourceCache.GetResource(ImageMemResource) as MemoryStream;
                            var hdc = bmpg.GetHdc();
                            var retImage = new Metafile(mem, hdc);
                            bmpg.ReleaseHdc(hdc);
                            using (var g = Graphics.FromImage(retImage)) {
                                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                /// ImageAttributesを設定しても透明にならないのでそのまま描画
                                if (_needImageFitted) {
                                    g.DrawImage(origImage, Point.Empty);
                                } else {
                                    g.DrawImageUnscaled(origImage, Point.Empty);
                                }
                            }
                            origImage.Dispose();
                            return retImage;
                        }

                    } else {
                        /// bitmap
                        var size = _needImageFitted? Size: _imageSize;
                        var retImage = new Bitmap(size.Width, size.Height);
                        using (var g = Graphics.FromImage(retImage)) {
                            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            if (Math.Abs(_opacity - 1) <= float.Epsilon * _opacity) {
                                g.DrawImage(
                                    origImage,
                                    new Rectangle(0, 0, size.Width, size.Height),
                                    0,
                                    0,
                                    origImage.Width,
                                    origImage.Height,
                                    GraphicsUnit.Pixel
                                );
    
                            } else {
                                using (var imageAttrs = ImageUtil.GetImageAttributes(_opacity)) {
                                    g.DrawImage(
                                        origImage,
                                        new Rectangle(0, 0, size.Width, size.Height),
                                        0,
                                        0,
                                        origImage.Width,
                                        origImage.Height,
                                        GraphicsUnit.Pixel,
                                        imageAttrs
                                    );
                                }
                            }
                        }
                        //if (Image.IsAlphaPixelFormat(origImage.PixelFormat)) {
                        //    retImage.MakeTransparent();
                        //}
                        origImage.Dispose();
                        return retImage;
                    }
                },
                ResourceDisposingPolicy.Explicit
            );
        }

        // ========================================
        // property
        // ========================================
        public IImageDescription ImageDesc {
            get { return _imageDesc; }
            set {
                _imageDesc = value;
                _ResourceCache.DisposeResource(ImageResource);
                _ResourceCache.DisposeResource(ImageMemResource);
            }
        }

        public float Opacity {
            get { return _opacity; }
            set {
                _opacity = value > 1? 1: value;
                _ResourceCache.DisposeResource(ImageResource);
                _ResourceCache.DisposeResource(ImageMemResource);
            }
        }

        /// <summary>
        /// 画像本来のサイズ．
        /// </summary>
        public Size ImageSize {
            get { return _imageSize; }
        }

        /// <summary>
        /// サイズ変更時にImageのキャッシュを作り直すかどうか
        /// </summary>
        public bool NeedRecreateImageOnBoundsChanged {
            get { return _needRecreateImageOnBoundsChanged; }
            set {
                if (value == _needRecreateImageOnBoundsChanged) {
                    return;
                }
                _needRecreateImageOnBoundsChanged = value;
                if (value) {
                    _ResourceCache.DisposeResource(ImageResource);
                    _ResourceCache.DisposeResource(ImageMemResource);
                }
            }
        }

        /// <summary>
        /// ImageのサイズをFigureのサイズに拡大・縮小すべきかどうか
        /// </summary>
        public bool NeedImageFitted {
            get { return _needImageFitted; }
            set {
                if (value == _needImageFitted) {
                    return;
                }
                _needImageFitted = value;
                if (value) {
                    _ResourceCache.DisposeResource(ImageResource);
                    _ResourceCache.DisposeResource(ImageMemResource);
                }
            }
        }

        public override Rectangle Bounds {
            get { return base.Bounds; }
            set {
                var old = base.Bounds;

                base.Bounds = value;

                if (_needRecreateImageOnBoundsChanged && value.Size != old.Size) {
                    _ResourceCache.DisposeResource(ImageResource);
                    _ResourceCache.DisposeResource(ImageMemResource);
                }
            }
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected Image _ImageResource {
            get { return _ResourceCache.GetResource(ImageResource) as Image; }
        }

        // ========================================
        // method
        // ========================================
        // === IPersistable ==========
        public override void WriteExternal(IMemento memento, ExternalizeContext context) {
            base.WriteExternal(memento, context);

            memento.WriteSerializable("ImageDesc", _imageDesc == null? null: _imageDesc.Clone());

            memento.WriteSerializable("ImageSize", _imageSize);
            memento.WriteFloat("Opacity", _opacity);
            memento.WriteBool("NeedRecreateImageOnBoundsChanged", _needRecreateImageOnBoundsChanged);
            memento.WriteBool("NeedImageFitted", _needImageFitted);
        }

        public override void ReadExternal(IMemento memento, ExternalizeContext context) {
            base.ReadExternal(memento, context);

            _imageDesc = memento.ReadSerializable("ImageDesc") as IImageDescription;

            _imageSize = (Size) memento.ReadSerializable("ImageSize");
            _opacity = memento.ReadFloat("Opacity");
            _needRecreateImageOnBoundsChanged = memento.ReadBool("NeedRecreateImageOnBoundsChanged");
            _needImageFitted = memento.ReadBool("NeedImageFitted");
        }

        public override void MakeTransparent(float ratio) {
            base.MakeTransparent(ratio);
            Opacity = Opacity * ratio;
        }


        // ------------------------------
        // protected
        // ------------------------------
        // === AbstractFigure ==========
        protected override void PaintSelf(Graphics g) {
            using (_ResourceCache.UseResource()) {
                if (IsBackgroundEnabled) {
                    g.DrawImage(_ImageResource, Bounds);
                }
                if (IsForegroundEnabled && BorderWidth > 0) {
                    g.DrawRectangle(_PenResource, Left, Top, Width - 1, Height - 1);
                }
                PaintText(g);
                PaintSelection(g);
                PaintStyledText(g);
            }
        }

        protected override Size MeasureSelf(SizeConstraint constraint) {
            //if (Root == null || Root.Canvas == null || Root.Canvas.IsDisposed) {
            //    /// 測れないので現状維持
            //    return constraint.MeasureConstrainedSize(Size);
            //}

            if (!IsVisible) {
                /// 現状維持
                return constraint.MeasureConstrainedSize(Size);
            }

            using (_ResourceCache.UseResource()) {
                var image = _ImageResource;
                if (image != null) {
                    return constraint.MeasureConstrainedSize(_imageSize);
                }

                return base.MeasureSelf(constraint);
            }
        }

    }
}
