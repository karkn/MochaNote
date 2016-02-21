/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Mkamo.Common.Forms.Drawing {
    public class FreehandDrawer {
        // ========================================
        // field
        // ========================================
        private bool _inWriting;

        //private int _targetRange;
        //private int _angleRange;

        private List<PointF> _points;
        //private bool _newBlock;

        private PointF _sourceLocation;
        //private PointF _targetLocation;
        //private PointF _originLocation;
        //private PointF _destLocation;

        private Color _color;
        private int _width;

        // ========================================
        // constructor
        // ========================================
        public FreehandDrawer() {
            //_targetRange = 10;
            //_angleRange = 20;
            //_targetRange = 2;
            //_angleRange = 4;

            _points = new List<PointF>();
            //_newBlock = true;

            _color = Color.Black;
            _width = 2;
        }

        // ========================================
        // property
        // ========================================
        public IEnumerable<PointF> Points {
            get { return _points; }
        }

        public Color Color {
            get { return _color; }
            set { _color = value; }
        }

        public int Width {
            get { return _width; }
            set { _width = value; }
        }

        // ========================================
        // method
        // ========================================
        public void Start(PointF pt) {
            _inWriting = true;
            _points.Clear();
            //_sourceLocation = pt;
            _points.Add(pt);
            //SetOrigin(pt);
        }

        public void Write(Graphics g, PointF pt) {
            if (!_inWriting) {
                return;
            }

            /// draw hand line
            if (g != null) {
                using (var pen = new Pen(_color, _width)) {
                    g.DrawLine(Pens.Gray, _sourceLocation, pt);
                }
            }


            _sourceLocation = pt;

            _points.Add(pt);

            //if (IsRangeCircle(pt)) {
            //    return;
            //}

            //if (_newBlock) {
            //    SetTarget(pt);
            //    _newBlock = false;
            //    return;
            //}

            //if (IsRangeBox(pt)) {
            //    SetDestination(pt);
            //    AddPoint();
            //    _newBlock = true;
            //    SetOrigin(pt);
            //}
        }

        public void Finish(Graphics g, PointF pt) {
            //SetDestination(pt);
            _points.Add(pt);
            //_newBlock = true;

            if (g != null) {
                g.DrawCurve(Pens.Red, _points.ToArray(), 0.3f);
            }

            _inWriting = false;
        }

        // ------------------------------
        // private
        // ------------------------------
        //private void SetOrigin(PointF pt) {
        //    _originLocation = pt;
        //}

        //private void SetDestination(PointF pt) {
        //    _destLocation = pt;
        //}

        //private void SetTarget(PointF pt) {
        //    _targetLocation = pt;
        //}

        //private bool IsRangeCircle(PointF pt) {
        //    var dir = new PointF(pt.X - _originLocation.X, pt.Y - _originLocation.Y);
        //    var distance = Math.Sqrt(dir.X * dir.X + dir.Y * dir.Y);
        //    return !(distance > _targetRange);
        //}

        //private bool IsRangeBox(PointF pt) {
        //    var x1 = _originLocation.X;
        //    var y1 = _originLocation.Y;
        //    var x2 = _targetLocation.X;
        //    var y2 = _targetLocation.Y;

        //    var a = y1 - y2;
        //    var b = x2 - x1;
        //    var c = x1 * y2 - x2 * y1;
        //    var distance =
        //        (Math.Abs(a * pt.X + b * pt.Y + c) /
        //            Math.Sqrt(a * a + b * b));
        //    return !(distance > _angleRange);
        //}

        //private void AddPoint() {
        //    var x1 = _originLocation.X;
        //    var y1 = _originLocation.Y;
        //    var x2 = _targetLocation.X;
        //    var y2 = _targetLocation.Y;
        //    var x3 = _destLocation.X;
        //    var y3 = _destLocation.Y;

        //    var a = ((x2 - x1) * (x3 - x1) + (y2 - y1) * (y3 - y1)) / ((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        //    var x = a * (x2 - x1) + x1;
        //    var y = a * (y2 - y1) + y1;
        //    _points.Add(new PointF(x, y));
        //}
    }
}
