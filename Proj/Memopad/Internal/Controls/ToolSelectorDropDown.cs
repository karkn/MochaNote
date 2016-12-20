/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Control.SelectorDropDown;
using Mkamo.Editor.Tools;
using Mkamo.Memopad.Properties;
using Mkamo.Editor.Core;
using Mkamo.Model.Memo;
using Mkamo.Figure.Figures;
using System.Drawing;
using Mkamo.Model.Core;
using Mkamo.Figure.Figures.EdgeDecorations;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Memopad.Internal.Controllers;
using Mkamo.Model.Uml;
using System.Drawing.Drawing2D;
using Mkamo.Figure.Core;
using Mkamo.Figure.Routers;
using Mkamo.Common.DataType;
using Mkamo.Memopad.Internal.Figures;
using Mkamo.Memopad.Internal.Forms;
using Mkamo.Common.Forms.Input;

namespace Mkamo.Memopad.Internal.Controls {
    using TextAndCreateNodeTools = Tuple<string, CreateNodeTool>;
    using TextAndImageAndAddEdgeTools = Tuple<string, Image, AddEdgeTool>;
    using Mkamo.Memopad.Internal.Tools;

    internal class ToolSelectorDropDown: SelectorDropDown, IToolRegistry {

        // ========================================
        // field
        // ========================================
        private MemopadFormBase _form;

        private ITool _defaultTool;
        private Dictionary<string, TextAndCreateNodeTools> _createNodeReg;
        private Dictionary<string, TextAndImageAndAddEdgeTools> _addEdgeReg;

        private bool _isPrepared;

        // ========================================
        // constructor
        // ========================================
        public ToolSelectorDropDown(MemopadFormBase form) {
            _form = form;
            _createNodeReg = new Dictionary<string, TextAndCreateNodeTools>();
            _addEdgeReg = new Dictionary<string, TextAndImageAndAddEdgeTools>();
            _isPrepared = false;
        }

        // ========================================
        // property
        // ========================================
        public IEnumerable<string> CreateNodeToolIds {
            get { return _createNodeReg.Keys; }
        }

        public IEnumerable<string> AddEdgeToolIds {
            get { return _addEdgeReg.Keys; }
        }

        // ========================================
        // method
        // ========================================
        public void Prepare() {
            if (_isPrepared) {
                return;
            }

            SuspendLayout();

            var operation = new SelectorCategory("操作");
            {
                var tool = new SelectTool();
                _defaultTool = tool;
                operation.AddLabel(
                    Resources.cursor,
                    "選択",
                    () => {
                        _form.EditorCanvas.Tool = tool;
                        Hide();
                        _form.Activate();
                        _form.EditorCanvas.Select();
                    }
                );

                AddCategory(operation);
            }

            var basic = new SelectorCategory("基本");
            AddCreateShapeTool(MemopadToolIds.CreateRect, "四角", Resources.rect, basic, MemoShapeKind.Rect, new Rect());
            AddCreateShapeTool("round_rect", "角丸四角", Resources.round_rect, basic, MemoShapeKind.RoundRect, new RoundedRect());
            AddCreateShapeTool("triangle", "三角", Resources.triangle, basic, MemoShapeKind.Triangle, new Triangle());
            AddCreateShapeTool("ellipse", "楕円", Resources.ellipse, basic, MemoShapeKind.Ellipse, new Ellipse());
            AddCreateShapeTool("diamond", "ひし形", Resources.diamond, basic, MemoShapeKind.Diamond, new Diamond());
            AddCreateShapeTool("parallelogram", "平行四辺形", Resources.parallelogram, basic, MemoShapeKind.Parallelogram, new Parallelogram());
            AddCreateShapeTool("cylinder", "円柱", Resources.cylinder, basic, MemoShapeKind.Cylinder, new Cylinder());
            AddCreateShapeTool("paper", "メモ", Resources.paper, basic, MemoShapeKind.Paper, new Paper());
            AddCategory(basic);

            // --- コメント ---
            var comment = new SelectorCategory("コメント");
            {
                var feedback = new LineEdge() {
                    LineColor = Color.DimGray,
                    LineWidth = 1,
                    Router = new CommentRouter(),
                    ConnectionMethod = ConnectionMethodKind.Comment,
                    LineDashStyle = System.Drawing.Drawing2D.DashStyle.Dash,
                };
                var tool = new CreateCommentEdgeTool(
                    new DelegatingModelFactory<MemoAnchorReference>(
                        () => {
                            var ret = MemoFactory.CreateAnchorReference();
                            return ret;
                        }
                    ),
                    feedback,
                    new MemoAnchorReferenceController()
                );
                AddCreateCommentEdgeTool("コメント線", Resources.comment_line, comment, tool);
            }
            AddCategory(comment);

            // --- 線 ---
            var line = new SelectorCategory("線 (接続あり)");
            {
                var feedback = new LineEdge() {
                    LineColor = Color.DimGray,
                    LineWidth = 1,
                    Router = new CentralRouter(),
                    ConnectionMethod = ConnectionMethodKind.Center,
                };
                var tool = new CreateEdgeTool(
                    new DelegatingModelFactory<MemoEdge>(
                        () => {
                            var ret = MemoFactory.CreateEdge();
                            ret.Kind = MemoEdgeKind.Central;
                            return ret;
                        }
                    ),
                    feedback,
                    null,
                    null,
                    null
                );
                AddCreateEdgeTool("直線 (経路 自由，接続位置 中心からの交点)", Resources.line, line, tool);
            }
            {
                var feedback = new LineEdge() {
                    LineColor = Color.DimGray,
                    LineWidth = 1,
                    Router = new CentralRouter(),
                    ConnectionMethod = ConnectionMethodKind.Center,
                    TargetDecoration = new ArrowEdgeDecoration() {
                        Foreground = Color.DimGray,
                    },
                };
                var tool = new CreateEdgeTool(
                    new DelegatingModelFactory<MemoEdge>(
                        () => {
                            var ret = MemoFactory.CreateEdge();
                            ret.Kind = MemoEdgeKind.Central;
                            ret.EndCapKind = MemoEdgeCapKind.Arrow;
                            return ret;
                        }
                    ),
                    feedback,
                    new MemoEdgeController()
                );
                AddCreateEdgeTool("矢印付き直線 (経路 自由，接続位置 中心からの交点)", Resources.arrow_line, line, tool);
            }
            {
                var feedback = new LineEdge() {
                    LineColor = Color.DimGray,
                    LineWidth = 1,
                    Router = new OrthogonalMidpointRouter(),
                    ConnectionMethod = ConnectionMethodKind.SideMidpointOfNearest,
                };
                var tool = new CreateEdgeTool(
                    new DelegatingModelFactory<MemoEdge>(
                        () => {
                            var ret = MemoFactory.CreateEdge();
                            ret.Kind = MemoEdgeKind.OrthogonalMidpoint;
                            return ret;
                        }
                    ),
                    feedback,
                    new MemoEdgeController()
                );
                AddCreateEdgeTool("直線 (経路 直角，接続位置 四辺の中点)", Resources.orthogonal_line, line, tool);
            }
            {
                var feedback = new LineEdge() {
                    LineColor = Color.DimGray,
                    LineWidth = 1,
                    TargetDecoration = new ArrowEdgeDecoration() {
                        Foreground = Color.DimGray,
                    },
                    Router = new OrthogonalMidpointRouter(),
                    ConnectionMethod = ConnectionMethodKind.SideMidpointOfNearest,
                };
                var tool = new CreateEdgeTool(
                    new DelegatingModelFactory<MemoEdge>(
                        () => {
                            var ret = MemoFactory.CreateEdge();
                            ret.Kind = MemoEdgeKind.OrthogonalMidpoint;
                            ret.EndCapKind = MemoEdgeCapKind.Arrow;
                            return ret;
                        }
                    ),
                    feedback,
                    new MemoEdgeController()
                );
                AddCreateEdgeTool("矢印付き直線 (経路 直角，接続位置 四辺の中点)", Resources.orthogonal_arrow_line, line, tool);
            }

            {
                var feedback = new LineEdge() {
                    LineColor = Color.DimGray,
                    LineWidth = 1,
                    LineDashStyle = DashStyle.Dash,
                    Router = new CentralRouter(),
                    ConnectionMethod = ConnectionMethodKind.Center,
                };
                var tool = new CreateEdgeTool(
                    new DelegatingModelFactory<MemoEdge>(
                        () => {
                            var ret = MemoFactory.CreateEdge();
                            ret.Kind = MemoEdgeKind.Central;
                            return ret;
                        }
                    ),
                    feedback,
                    new MemoEdgeController(),
                    (edge) => {
                        edge.LineDashStyle = DashStyle.Dash;
                    }
                );
                AddCreateEdgeTool("点線 (経路 自由，接続位置 中心からの交点)", Resources.dashed_line, line, tool);
            }
            {
                var feedback = new LineEdge() {
                    LineColor = Color.DimGray,
                    LineWidth = 1,
                    LineDashStyle = DashStyle.Dash,
                    Router = new CentralRouter(),
                    ConnectionMethod = ConnectionMethodKind.Center,
                    TargetDecoration = new ArrowEdgeDecoration() {
                        Foreground = Color.DimGray,
                    },
                };
                var tool = new CreateEdgeTool(
                    new DelegatingModelFactory<MemoEdge>(
                        () => {
                            var ret = MemoFactory.CreateEdge();
                            ret.Kind = MemoEdgeKind.Central;
                            ret.EndCapKind = MemoEdgeCapKind.Arrow;
                            return ret;
                        }
                    ),
                    feedback,
                    new MemoEdgeController(),
                    (edge) => {
                        edge.LineDashStyle = DashStyle.Dash;
                    }
                );
                AddCreateEdgeTool("矢印付き点線 (経路 自由，接続位置 中心からの交点)", Resources.dashed_arrow, line, tool);
            }
            {
                var feedback = new LineEdge() {
                    LineColor = Color.DimGray,
                    LineWidth = 1,
                    LineDashStyle = DashStyle.Dash,
                    Router = new OrthogonalMidpointRouter(),
                    ConnectionMethod = ConnectionMethodKind.SideMidpointOfNearest,
                };
                var tool = new CreateEdgeTool(
                    new DelegatingModelFactory<MemoEdge>(
                        () => {
                            var ret = MemoFactory.CreateEdge();
                            ret.Kind = MemoEdgeKind.OrthogonalMidpoint;
                            return ret;
                        }
                    ),
                    feedback,
                    new MemoEdgeController(),
                    (edge) => {
                        edge.LineDashStyle = DashStyle.Dash;
                    }
                );
                AddCreateEdgeTool("点線 (経路 直角，接続位置 四辺の中点)", Resources.orthogonal_dashed_line, line, tool);
            }
            {
                var feedback = new LineEdge() {
                    LineColor = Color.DimGray,
                    LineWidth = 1,
                    LineDashStyle = DashStyle.Dash,
                    TargetDecoration = new ArrowEdgeDecoration() {
                        Foreground = Color.DimGray,
                    },
                    Router = new OrthogonalMidpointRouter(),
                    ConnectionMethod = ConnectionMethodKind.SideMidpointOfNearest,
                };
                var tool = new CreateEdgeTool(
                    new DelegatingModelFactory<MemoEdge>(
                        () => {
                            var ret = MemoFactory.CreateEdge();
                            ret.Kind = MemoEdgeKind.OrthogonalMidpoint;
                            ret.EndCapKind = MemoEdgeCapKind.Arrow;
                            return ret;
                        }
                    ),
                    feedback,
                    new MemoEdgeController(),
                    (edge) => {
                        edge.LineDashStyle = DashStyle.Dash;
                    }
                );
                AddCreateEdgeTool("矢印付き点線 (経路 直角，接続位置 四辺の中点)", Resources.orthogonal_dashed_arrow, line, tool);
            }
            //{
            //    var feedback = new LineEdge() {
            //        LineColor = Color.DimGray,
            //        LineWidth = 1,
            //    };
            //    var tool = new CreateEdgeTool(
            //        new DelegatingModelFactory<MemoEdge>(
            //            () => {
            //                var ret = MemoFactory.CreateEdge();
            //                ret.Kind = MemoEdgeKind.Normal;
            //                return ret;
            //            }
            //        ),
            //        feedback,
            //        new MemoEdgeController()
            //    );
            //    AddCreateEdgeTool("直線 (フリー)", Resources.line, line, tool);
            //}
            //{
            //    var feedback = new LineEdge() {
            //        LineColor = Color.DimGray,
            //        LineWidth = 1,
            //        TargetDecoration = new ArrowEdgeDecoration() {
            //            Foreground = Color.DimGray,
            //        },
            //    };
            //    var tool = new CreateEdgeTool(
            //        new DelegatingModelFactory<MemoEdge>(
            //            () => {
            //                var ret = MemoFactory.CreateEdge();
            //                ret.EndCapKind = MemoEdgeCapKind.Arrow;
            //                return ret;
            //            }
            //        ),
            //        feedback,
            //        new MemoEdgeController()
            //    );
            //    AddCreateEdgeTool("矢印付直線 (フリー)", Resources.arrow_line, line, tool);
            //}
            AddCategory(line);

            // --- 非接続線 ---
            var lineNoConnection = new SelectorCategory("線 (接続なし)");
            {
                var feedback = new LineEdge() {
                    LineColor = Color.DimGray,
                    LineWidth = 1,
                    Router = new CentralRouter(),
                    ConnectionMethod = ConnectionMethodKind.Center,
                };
                var tool = new CreateEdgeTool(
                    new DelegatingModelFactory<MemoEdge>(
                        () => {
                            var ret = MemoFactory.CreateEdge();
                            ret.Kind = MemoEdgeKind.Central;
                            ret.CanConnectSource = false;
                            ret.CanConnectTarget = false;
                            return ret;
                        }
                    ),
                    feedback,
                    obj => false,
                    obj => false,
                    null
                );
                AddCreateEdgeTool("直線 (経路 自由)", Resources.line, lineNoConnection, tool);
            }
            {
                var feedback = new LineEdge() {
                    LineColor = Color.DimGray,
                    LineWidth = 1,
                    Router = new CentralRouter(),
                    ConnectionMethod = ConnectionMethodKind.Center,
                    TargetDecoration = new ArrowEdgeDecoration() {
                        Foreground = Color.DimGray,
                    },
                };
                var tool = new CreateEdgeTool(
                    new DelegatingModelFactory<MemoEdge>(
                        () => {
                            var ret = MemoFactory.CreateEdge();
                            ret.Kind = MemoEdgeKind.Central;
                            ret.EndCapKind = MemoEdgeCapKind.Arrow;
                            ret.CanConnectSource = false;
                            ret.CanConnectTarget = false;
                            return ret;
                        }
                    ),
                    feedback,
                    obj => false,
                    obj => false,
                    null
                );
                AddCreateEdgeTool("矢印付き直線 (経路 自由)", Resources.arrow_line, lineNoConnection, tool);
            }

            {
                var feedback = new LineEdge() {
                    LineColor = Color.DimGray,
                    LineWidth = 1,
                    Router = new OrthogonalMidpointRouter(),
                    ConnectionMethod = ConnectionMethodKind.SideMidpointOfNearest,
                };
                var tool = new CreateEdgeTool(
                    new DelegatingModelFactory<MemoEdge>(
                        () => {
                            var ret = MemoFactory.CreateEdge();
                            ret.Kind = MemoEdgeKind.OrthogonalMidpoint;
                            ret.CanConnectSource = false;
                            ret.CanConnectTarget = false;
                            return ret;
                        }
                    ),
                    feedback,
                    obj => false,
                    obj => false,
                    null
                );
                AddCreateEdgeTool("直線 (経路 直角)", Resources.orthogonal_line, lineNoConnection, tool);
            }
            {
                var feedback = new LineEdge() {
                    LineColor = Color.DimGray,
                    LineWidth = 1,
                    TargetDecoration = new ArrowEdgeDecoration() {
                        Foreground = Color.DimGray,
                    },
                    Router = new OrthogonalMidpointRouter(),
                    ConnectionMethod = ConnectionMethodKind.SideMidpointOfNearest,
                };
                var tool = new CreateEdgeTool(
                    new DelegatingModelFactory<MemoEdge>(
                        () => {
                            var ret = MemoFactory.CreateEdge();
                            ret.Kind = MemoEdgeKind.OrthogonalMidpoint;
                            ret.EndCapKind = MemoEdgeCapKind.Arrow;
                            ret.CanConnectSource = false;
                            ret.CanConnectTarget = false;
                            return ret;
                        }
                    ),
                    feedback,
                    obj => false,
                    obj => false,
                    null
                );
                AddCreateEdgeTool("矢印付き直線 (経路 直角)", Resources.orthogonal_arrow_line, lineNoConnection, tool);
            }

            {
                var feedback = new LineEdge() {
                    LineColor = Color.DimGray,
                    LineWidth = 1,
                    LineDashStyle = DashStyle.Dash,
                    Router = new CentralRouter(),
                    ConnectionMethod = ConnectionMethodKind.Center,
                };
                var tool = new CreateEdgeTool(
                    new DelegatingModelFactory<MemoEdge>(
                        () => {
                            var ret = MemoFactory.CreateEdge();
                            ret.Kind = MemoEdgeKind.Central;
                            ret.CanConnectSource = false;
                            ret.CanConnectTarget = false;
                            return ret;
                        }
                    ),
                    feedback,
                    obj => false,
                    obj => false,
                    (edge) => {
                        edge.LineDashStyle = DashStyle.Dash;
                    }
                );
                AddCreateEdgeTool("点線 (経路 自由)", Resources.dashed_line, lineNoConnection, tool);
            }
            {
                var feedback = new LineEdge() {
                    LineColor = Color.DimGray,
                    LineWidth = 1,
                    LineDashStyle = DashStyle.Dash,
                    Router = new CentralRouter(),
                    ConnectionMethod = ConnectionMethodKind.Center,
                    TargetDecoration = new ArrowEdgeDecoration() {
                        Foreground = Color.DimGray,
                    },
                };
                var tool = new CreateEdgeTool(
                    new DelegatingModelFactory<MemoEdge>(
                        () => {
                            var ret = MemoFactory.CreateEdge();
                            ret.Kind = MemoEdgeKind.Central;
                            ret.EndCapKind = MemoEdgeCapKind.Arrow;
                            ret.CanConnectSource = false;
                            ret.CanConnectTarget = false;
                            return ret;
                        }
                    ),
                    feedback,
                    obj => false,
                    obj => false,
                    (edge) => {
                        edge.LineDashStyle = DashStyle.Dash;
                    }
                );
                AddCreateEdgeTool("矢印付き点線 (経路 自由)", Resources.dashed_arrow, lineNoConnection, tool);
            }
            {
                var feedback = new LineEdge() {
                    LineColor = Color.DimGray,
                    LineWidth = 1,
                    LineDashStyle = DashStyle.Dash,
                    Router = new OrthogonalMidpointRouter(),
                    ConnectionMethod = ConnectionMethodKind.SideMidpointOfNearest,
                };
                var tool = new CreateEdgeTool(
                    new DelegatingModelFactory<MemoEdge>(
                        () => {
                            var ret = MemoFactory.CreateEdge();
                            ret.Kind = MemoEdgeKind.OrthogonalMidpoint;
                            ret.CanConnectSource = false;
                            ret.CanConnectTarget = false;
                            return ret;
                        }
                    ),
                    feedback,
                    obj => false,
                    obj => false,
                    (edge) => {
                        edge.LineDashStyle = DashStyle.Dash;
                    }
                );
                AddCreateEdgeTool("点線 (経路 直角)", Resources.orthogonal_dashed_line, lineNoConnection, tool);
            }
            {
                var feedback = new LineEdge() {
                    LineColor = Color.DimGray,
                    LineWidth = 1,
                    LineDashStyle = DashStyle.Dash,
                    TargetDecoration = new ArrowEdgeDecoration() {
                        Foreground = Color.DimGray,
                    },
                    Router = new OrthogonalMidpointRouter(),
                    ConnectionMethod = ConnectionMethodKind.SideMidpointOfNearest,
                };
                var tool = new CreateEdgeTool(
                    new DelegatingModelFactory<MemoEdge>(
                        () => {
                            var ret = MemoFactory.CreateEdge();
                            ret.Kind = MemoEdgeKind.OrthogonalMidpoint;
                            ret.EndCapKind = MemoEdgeCapKind.Arrow;
                            ret.CanConnectSource = false;
                            ret.CanConnectTarget = false;
                            return ret;
                        }
                    ),
                    feedback,
                    obj => false,
                    obj => false,
                    (edge) => {
                        edge.LineDashStyle = DashStyle.Dash;
                    }
                );
                AddCreateEdgeTool("矢印付き点線 (経路 直角)", Resources.orthogonal_dashed_arrow, lineNoConnection, tool);
            }
            AddCategory(lineNoConnection);

            // --- メモ ---
            var memo = new SelectorCategory("メモ");
            {
                var feedback = new Rect() {
                    Foreground = Color.Red,
                    IsBackgroundEnabled = false,
                    BorderWidth = 2,
                };
                var tool = new CreateNodeTool(
                    new DelegatingModelFactory<MemoShape>(
                        () => {
                            var ret = MemoFactory.CreateShape();
                            ret.Kind = MemoShapeKind.Rect;
                            return ret;
                        }
                    ),
                    feedback,
                    (node) => {
                        node.Foreground = Color.Red;
                        node.IsBackgroundEnabled = false;
                        node.BorderWidth = 2;
                    }
                );
                AddCreateNodeTool("rect_box", "矩形囲み", Resources.rect_box, memo, tool);
            }
            {
                var feedback = new RoundedRect() {
                    Foreground = Color.Red,
                    IsBackgroundEnabled = false,
                    BorderWidth = 2,
                };
                var tool = new CreateNodeTool(
                    new DelegatingModelFactory<MemoShape>(
                        () => {
                            var ret = MemoFactory.CreateShape();
                            ret.Kind = MemoShapeKind.RoundRect;
                            return ret;
                        }
                    ),
                    feedback,
                    (node) => {
                        node.Foreground = Color.Red;
                        node.IsBackgroundEnabled = false;
                        node.BorderWidth = 2;
                    }
                );
                AddCreateNodeTool("round_rect_box", "角丸囲み", Resources.round_rect_box, memo, tool);
            }
            AddCategory(memo);

            // --- 矢印 ---
            var arrow = new SelectorCategory("矢印");
            AddCreateShapeTool(
                "arrow_fig_left",
                "左矢印", Resources.arrow_fig_l, arrow,
                MemoShapeKind.LeftArrow, new ArrowFigure(Directions.Left)
            );
            AddCreateShapeTool(
                "arrow_fig_right",
                "右矢印", Resources.arrow_fig_r, arrow,
                MemoShapeKind.RightArrow, new ArrowFigure(Directions.Right)
            );
            AddCreateShapeTool(
                "arrow_fig_up",
                "上矢印", Resources.arrow_fig_u, arrow,
                MemoShapeKind.UpArrow, new ArrowFigure(Directions.Up)
            );
            AddCreateShapeTool(
                "arrow_fig_down",
                "下矢印", Resources.arrow_fig_d, arrow,
                MemoShapeKind.DownArrow, new ArrowFigure(Directions.Down)
            );
            AddCreateShapeTool(
                "arrow_fig_leftright",
                "左右矢印", Resources.arrow_fig_lr, arrow,
                MemoShapeKind.LeftRightArrow, new TwoHeadArrowFigure(false)
            );
            AddCreateShapeTool(
                "arrow_fig_updown",
                "上下矢印", Resources.arrow_fig_ud, arrow,
                MemoShapeKind.UpDownArrow, new TwoHeadArrowFigure(true)
            );
            AddCreateShapeTool(
                "pentagon",
                "ホームベース", Resources.pentagon, arrow,
                MemoShapeKind.Pentagon, new Pentagon()
            );
            AddCreateShapeTool(
                "chevron",
                "山形", Resources.chevron, arrow,
                MemoShapeKind.Chevron, new Chevron()
            );
            AddCategory(arrow);

            // --- 数式 ---
            var math = new SelectorCategory("数式");
            AddCreateShapeTool(
                "equal_fig",
                "等号", Resources.equal_fig, math,
                MemoShapeKind.Equal, new EqualFigure(false)
            );
            AddCreateShapeTool(
                "not_equal_fig",
                "不等号", Resources.not_equal_fig, math,
                MemoShapeKind.NotEqual, new EqualFigure(true)
            );
            AddCreateShapeTool(
                "plus_fig",
                "加算", Resources.plus_figure, math,
                MemoShapeKind.Plus, new PlusFigure()
            );
            AddCreateShapeTool(
                "minus_fig",
                "減算", Resources.minus_figure, math,
                MemoShapeKind.Minus, new MinusFigure()
            );
            AddCreateShapeTool(
                "times_fig",
                "乗算", Resources.times_figure, math,
                MemoShapeKind.Times, new TimesFigure()
            );
            AddCreateShapeTool(
                "devide_fig",
                "除算", Resources.devide_figure, math,
                MemoShapeKind.Devide, new DevideFigure()
            );
            AddCategory(math);

            // --- クラス図 ---
            //if (string.Equals(MemopadApplication.Instance.UserInfo.Email, "mkamo@mkamo.org", StringComparison.Ordinal)) {
#if false
            var classdia = new SelectorCategory("クラス図");
            {
                var feedback = new UmlClassFigure() {
                    Foreground = SystemColors.ButtonShadow,
                    Background = new SolidBrushDescription(Color.FromArgb(16, SystemColors.ButtonFace)),
                };
                var tool = new CreateNodeTool(
                    new DelegatingModelFactory<UmlClass>(
                        () => {
                            var ret = UmlFactory.CreateClass();
                            ret.Name = "Class";
                            return ret;
                        }
                    ),
                    feedback
                );
                tool.DefaultNodeSize = new Size(80, 48);
                tool.FocusOnCreated = true;
                AddCreateNodeTool("uml_class", "クラス", Resources.clazz, classdia, tool);
            }

            {
                var feedback = new UmlInterfaceFigure() {
                    Foreground = SystemColors.ButtonShadow,
                    Background = new SolidBrushDescription(Color.FromArgb(16, SystemColors.ButtonFace)),
                };
                var tool = new CreateNodeTool(
                    new DelegatingModelFactory<UmlInterface>(
                        () => {
                            var ret = UmlFactory.CreateInterface();
                            ret.Name = "Interface";
                            return ret;
                        }
                    ),
                    feedback
                );
                tool.DefaultNodeSize = new Size(80, 48);
                tool.FocusOnCreated = true;
                AddCreateNodeTool("uml_interface", "インタフェース", Resources.ifc, classdia, tool, true);
            }

            {
                var feedback = new LineEdge() {
                    LineColor = Color.DimGray,
                    LineWidth = 1,
                    Router = new CentralRouter(),
                    ConnectionMethod = ConnectionMethodKind.Center,
              };
                var tool = new CreateEdgeTool(
                    new DelegatingModelFactory<UmlAssociation>(() => UmlFactory.CreateAssociation()),
                    feedback,
                    new UmlAssociationController(),
                    (edge) => {
                        edge.Router = new CentralRouter();
                        edge.ConnectionMethod = ConnectionMethodKind.Center;
                    }
                );
                AddCreateEdgeTool("関連 (中心線)", Resources.line, classdia, tool);
            }
            {
                var feedback = new LineEdge() {
                    LineColor = Color.DimGray,
                    LineWidth = 1,
                    Router = new CentralRouter(),
                    ConnectionMethod = ConnectionMethodKind.Center,
                };
                var tool = new CreateEdgeTool(
                    new DelegatingModelFactory<UmlAssociation>(
                        () => {
                            var ret = UmlFactory.CreateAssociation();
                            ret.IsSourceNavigable = false;
                            return ret;
                        }
                    ),
                    feedback,
                    new UmlAssociationController(),
                    (edge) => {
                        edge.Router = new CentralRouter();
                        edge.ConnectionMethod = ConnectionMethodKind.Center;
                    }
                );
                AddCreateEdgeTool("関連 (中心線，矢印)", Resources.arrow_line, classdia, tool);
            }
            {
                var feedback = new LineEdge() {
                    LineColor = Color.DimGray,
                    LineWidth = 1,
                    Router = new CentralRouter(),
                    ConnectionMethod = ConnectionMethodKind.Center,
                    SourceDecoration = new DiamondEdgeDecoration() {
                        Foreground = Color.DimGray,
                        Background = Color.White,
                    },
                };
                var tool = new CreateEdgeTool(
                    new DelegatingModelFactory<UmlAssociation>(
                        () => {
                            var ret = UmlFactory.CreateAssociation();
                            ret.SourceMemberEnd.Aggregation = UmlAggregationKind.Shared;
                            return ret;
                        }
                    ),
                    feedback,
                    new UmlAssociationController(),
                    (edge) => {
                        edge.Router = new CentralRouter();
                        edge.ConnectionMethod = ConnectionMethodKind.Center;
                    }
                );
                AddCreateEdgeTool("集約 (中心線)", Resources.aggregation, classdia, tool);
            }
            {
                var feedback = new LineEdge() {
                    LineColor = Color.DimGray,
                    LineWidth = 1,
                    Router = new CentralRouter(),
                    ConnectionMethod = ConnectionMethodKind.Center,
                    SourceDecoration = new DiamondEdgeDecoration() {
                        Foreground = Color.DimGray,
                        Background = Color.DimGray,
                    },
                };
                var tool = new CreateEdgeTool(
                    new DelegatingModelFactory<UmlAssociation>(
                        () => {
                            var ret = UmlFactory.CreateAssociation();
                            ret.SourceMemberEnd.Aggregation = UmlAggregationKind.Composite;
                            return ret;
                        }
                    ),
                    feedback,
                    new UmlAssociationController(),
                    (edge) => {
                        edge.Router = new CentralRouter();
                        edge.ConnectionMethod = ConnectionMethodKind.Center;
                    }
                );
                AddCreateEdgeTool("コンポジション (中心線)", Resources.composition, classdia, tool);
            }
            {
                var feedback = new LineEdge() {
                    LineColor = Color.DimGray,
                    LineWidth = 1,
                    Router = new CentralRouter(),
                    ConnectionMethod = ConnectionMethodKind.Center,
                    TargetDecoration = new TriangleEdgeDecoration() {
                        Foreground = Color.DimGray,
                        Background = Color.White,
                    },
                };
                var tool = new CreateEdgeTool(
                    new DelegatingModelFactory<UmlGeneralization>(() => UmlFactory.CreateGeneralization()),
                    feedback,
                    new UmlGeneralizationController(),
                    (edge) => {
                        edge.Router = new CentralRouter();
                        edge.ConnectionMethod = ConnectionMethodKind.Center;
                    }
                );
                AddCreateEdgeTool("汎化 (中心線)", Resources.generalization, classdia, tool);
            }
            {
                var feedback = new LineEdge() {
                    LineColor = Color.DimGray,
                    LineWidth = 1,
                    LineDashStyle = DashStyle.Dash,
                    Router = new CentralRouter(),
                    ConnectionMethod = ConnectionMethodKind.Center,
                    TargetDecoration = new TriangleEdgeDecoration() {
                        Foreground = Color.DimGray,
                        Background = Color.White,
                    },
                };
                var tool = new CreateEdgeTool(
                    new DelegatingModelFactory<UmlInterfaceRealization>(
                        () => UmlFactory.CreateInterfaceRealization()
                    ),
                    feedback,
                    new UmlInterfaceRealizationController(),
                    (edge) => {
                        edge.Router = new CentralRouter();
                        edge.ConnectionMethod = ConnectionMethodKind.Center;
                    }
                );
                AddCreateEdgeTool("実現 (中心線)", Resources.realization, classdia, tool);
            }
            {
                var feedback = new LineEdge() {
                    LineColor = Color.DimGray,
                    LineWidth = 1,
                    LineDashStyle = DashStyle.Dash,
                    Router = new CentralRouter(),
                    ConnectionMethod = ConnectionMethodKind.Center,
                    TargetDecoration = new ArrowEdgeDecoration() {
                        Foreground = Color.DimGray,
                    },
                };
                var tool = new CreateEdgeTool(
                    new DelegatingModelFactory<UmlDependency>(() => UmlFactory.CreateDependency()),
                    feedback,
                    new UmlDependencyController(),
                    (edge) => {
                        edge.Router = new CentralRouter();
                        edge.ConnectionMethod = ConnectionMethodKind.Center;
                    }
                );
                AddCreateEdgeTool("依存 (中心線)", Resources.dashed_arrow, classdia, tool, true);
            }

            
            {
                var feedback = new LineEdge() {
                    LineColor = Color.DimGray,
                    LineWidth = 1,
                    Router = new OrthogonalRouter(),
                    ConnectionMethod = ConnectionMethodKind.Nearest,
                };
                var tool = new CreateEdgeTool(
                    new DelegatingModelFactory<UmlAssociation>(() => UmlFactory.CreateAssociation()),
                    feedback,
                    new UmlAssociationController(),
                    (edge) => {
                        edge.Router = new OrthogonalRouter();
                        edge.ConnectionMethod = ConnectionMethodKind.Nearest;
                    }
                );
                AddCreateEdgeTool("関連 (カギ線)", Resources.orthogonal_line, classdia, tool);
            }
            {
                var feedback = new LineEdge() {
                    LineColor = Color.DimGray,
                    LineWidth = 1,
                    Router = new OrthogonalRouter(),
                    ConnectionMethod = ConnectionMethodKind.Nearest,
                };
                var tool = new CreateEdgeTool(
                    new DelegatingModelFactory<UmlAssociation>(
                        () => {
                            var ret = UmlFactory.CreateAssociation();
                            ret.IsSourceNavigable = false;
                            return ret;
                        }
                    ),
                    feedback,
                    new UmlAssociationController(),
                    (edge) => {
                        edge.Router = new OrthogonalRouter();
                        edge.ConnectionMethod = ConnectionMethodKind.Nearest;
                    }
                );
                AddCreateEdgeTool("関連 (矢印，カギ線)", Resources.orthogonal_arrow_line, classdia, tool);
            }
            {
                var feedback = new LineEdge() {
                    LineColor = Color.DimGray,
                    LineWidth = 1,
                    SourceDecoration = new DiamondEdgeDecoration() {
                        Foreground = Color.DimGray,
                        Background = Color.White,
                    },
                    Router = new OrthogonalRouter(),
                    ConnectionMethod = ConnectionMethodKind.Nearest,
                };
                var tool = new CreateEdgeTool(
                    new DelegatingModelFactory<UmlAssociation>(
                        () => {
                            var ret = UmlFactory.CreateAssociation();
                            ret.SourceMemberEnd.Aggregation = UmlAggregationKind.Shared;
                            return ret;
                        }
                    ),
                    feedback,
                    new UmlAssociationController(),
                    (edge) => {
                        edge.Router = new OrthogonalRouter();
                        edge.ConnectionMethod = ConnectionMethodKind.Nearest;
                    }
                );
                AddCreateEdgeTool("集約 (カギ線)", Resources.orthogonal_aggregation, classdia, tool);
            }
            {
                var feedback = new LineEdge() {
                    LineColor = Color.DimGray,
                    LineWidth = 1,
                    SourceDecoration = new DiamondEdgeDecoration() {
                        Foreground = Color.DimGray,
                        Background = Color.DimGray,
                    },
                    Router = new OrthogonalRouter(),
                    ConnectionMethod = ConnectionMethodKind.Nearest,
                };
                var tool = new CreateEdgeTool(
                    new DelegatingModelFactory<UmlAssociation>(
                        () => {
                            var ret = UmlFactory.CreateAssociation();
                            ret.SourceMemberEnd.Aggregation = UmlAggregationKind.Composite;
                            return ret;
                        }
                    ),
                    feedback,
                    new UmlAssociationController(),
                    (edge) => {
                        edge.Router = new OrthogonalRouter();
                        edge.ConnectionMethod = ConnectionMethodKind.Nearest;
                    }
                );
                AddCreateEdgeTool("コンポジション (カギ線)", Resources.orthogonal_composition, classdia, tool);
            }
            {
                var feedback = new LineEdge() {
                    LineColor = Color.DimGray,
                    LineWidth = 1,
                    TargetDecoration = new TriangleEdgeDecoration() {
                        Foreground = Color.DimGray,
                        Background = Color.White,
                    },
                    Router = new OrthogonalRouter(),
                    ConnectionMethod = ConnectionMethodKind.Nearest,
                };
                var tool = new CreateEdgeTool(
                    new DelegatingModelFactory<UmlGeneralization>(() => UmlFactory.CreateGeneralization()),
                    feedback,
                    new UmlGeneralizationController(),
                    (edge) => {
                        edge.Router = new OrthogonalRouter();
                        edge.ConnectionMethod = ConnectionMethodKind.Nearest;
                    }
                );
                AddCreateEdgeTool("汎化 (カギ線)", Resources.orthogonal_generalization, classdia, tool);
            }
            {
                var feedback = new LineEdge() {
                    LineColor = Color.DimGray,
                    LineWidth = 1,
                    LineDashStyle = DashStyle.Dash,
                    TargetDecoration = new TriangleEdgeDecoration() {
                        Foreground = Color.DimGray,
                        Background = Color.White,
                    },
                    Router = new OrthogonalRouter(),
                    ConnectionMethod = ConnectionMethodKind.Nearest,
                };
                var tool = new CreateEdgeTool(
                    new DelegatingModelFactory<UmlInterfaceRealization>(
                        () => UmlFactory.CreateInterfaceRealization()
                    ),
                    feedback,
                    new UmlInterfaceRealizationController(),
                    (edge) => {
                        edge.Router = new OrthogonalRouter();
                        edge.ConnectionMethod = ConnectionMethodKind.Nearest;
                    }
                );
                AddCreateEdgeTool("実現 (カギ線)", Resources.orthogonal_realization, classdia, tool);
            }
            {
                var feedback = new LineEdge() {
                    LineColor = Color.DimGray,
                    LineWidth = 1,
                    LineDashStyle = DashStyle.Dash,
                    TargetDecoration = new ArrowEdgeDecoration() {
                        Foreground = Color.DimGray,
                    },
                    Router = new OrthogonalRouter(),
                    ConnectionMethod = ConnectionMethodKind.Nearest,
                };
                var tool = new CreateEdgeTool(
                    new DelegatingModelFactory<UmlDependency>(() => UmlFactory.CreateDependency()),
                    feedback,
                    new UmlDependencyController(),
                    (edge) => {
                        edge.Router = new OrthogonalRouter();
                        edge.ConnectionMethod = ConnectionMethodKind.Nearest;
                    }
                );
                AddCreateEdgeTool("依存 (カギ線)", Resources.orthogonal_dependency, classdia, tool);
            }
            
            AddCategory(classdia);
//}
#endif

            PrepareAddEdgeTools();
            
            ResumeLayout();

            _isPrepared = true;
        }

        /// <summary>
        /// ミニツールバーのツールの準備をする。
        /// </summary>
        private void PrepareAddEdgeTools() {
            {
                var feedback = new LineEdge() {
                    LineColor = Color.DimGray,
                    LineWidth = 1,
                    Router = new CentralRouter(),
                    ConnectionMethod = ConnectionMethodKind.Center,
                };
                var tool = new AddEdgeTool(
                    new DelegatingModelFactory<MemoEdge>(
                        () => {
                            var ret = MemoFactory.CreateEdge();
                            ret.Kind = MemoEdgeKind.Central;
                            return ret;
                        }
                    ),
                    feedback,
                    new MemoEdgeController()
                );
                AddAddEdgeTool(MemopadToolIds.AddLineCentral, "直線 (経路 自由，接続位置 中心からの交点)", Resources.line, tool);
            }
            {
                var feedback = new LineEdge() {
                    LineColor = Color.DimGray,
                    LineWidth = 1,
                    Router = new CentralRouter(),
                    ConnectionMethod = ConnectionMethodKind.Center,
                    TargetDecoration = new ArrowEdgeDecoration() {
                        Foreground = Color.DimGray,
                    },
                };
                var tool = new AddEdgeTool(
                    new DelegatingModelFactory<MemoEdge>(
                        () => {
                            var ret = MemoFactory.CreateEdge();
                            ret.Kind = MemoEdgeKind.Central;
                            ret.EndCapKind = MemoEdgeCapKind.Arrow;
                            return ret;
                        }
                    ),
                    feedback,
                    new MemoEdgeController()
                );
                AddAddEdgeTool(MemopadToolIds.AddLineArrowCentral, "矢印付き直線 (経路 自由，接続位置 中心からの交点)", Resources.arrow_line, tool);
            }

        
            {
                var feedback = new LineEdge() {
                    LineColor = Color.DimGray,
                    LineWidth = 1,
                    LineDashStyle = DashStyle.Dash,
                    Router = new CentralRouter(),
                    ConnectionMethod = ConnectionMethodKind.Center,
                };
                var tool = new AddEdgeTool(
                    new DelegatingModelFactory<MemoEdge>(
                        () => {
                            var ret = MemoFactory.CreateEdge();
                            ret.Kind = MemoEdgeKind.Central;
                            return ret;
                        }
                    ),
                    feedback,
                    new MemoEdgeController(),
                    (edge) => {
                        edge.LineDashStyle = DashStyle.Dash;
                    }
                );
                AddAddEdgeTool(MemopadToolIds.AddLineDashCentral, "点線 (経路 自由，接続位置 中心からの交点)", Resources.dashed_line, tool);
            }
            {
                var feedback = new LineEdge() {
                    LineColor = Color.DimGray,
                    LineWidth = 1,
                    LineDashStyle = DashStyle.Dash,
                    Router = new CentralRouter(),
                    ConnectionMethod = ConnectionMethodKind.Center,
                    TargetDecoration = new ArrowEdgeDecoration() {
                        Foreground = Color.DimGray,
                    },
                };
                var tool = new AddEdgeTool(
                    new DelegatingModelFactory<MemoEdge>(
                        () => {
                            var ret = MemoFactory.CreateEdge();
                            ret.Kind = MemoEdgeKind.Central;
                            ret.EndCapKind = MemoEdgeCapKind.Arrow;
                            return ret;
                        }
                    ),
                    feedback,
                    new MemoEdgeController(),
                    (edge) => {
                        edge.LineDashStyle = DashStyle.Dash;
                    }
                );
                AddAddEdgeTool(MemopadToolIds.AddLineDashArrowCentral, "矢印付き点線 (経路 自由，接続位置 中心からの交点)", Resources.dashed_arrow, tool);
            }

            AddOrth(
                MemopadToolIds.AddLineOrthogonalFreeDirection,
                "直線 (経路 直角，接続位置 四辺の中点，方向自由)",
                Resources.orthogonal_line,
                DashStyle.Solid,
                ConnectionMethodKind.SideMidpointOfOpposite,
                false
            );
            AddOrth(
                MemopadToolIds.AddLineOrthogonalUpDirection,
                "直線 (経路 直角，接続位置 四辺の中点，方向上)",
                Resources.orthogonal_line,
                DashStyle.Solid,
                ConnectionMethodKind.UpperSideMidpoint,
                false
            );
            AddOrth(
                MemopadToolIds.AddLineOrthogonalDownDirection,
                "直線 (経路 直角，接続位置 四辺の中点，方向下)",
                Resources.orthogonal_line,
                DashStyle.Solid,
                ConnectionMethodKind.LowerSideMidpoint,
                false
            );
            AddOrth(
                MemopadToolIds.AddLineOrthogonalLeftDirection,
                "直線 (経路 直角，接続位置 四辺の中点，方向左)",
                Resources.orthogonal_line,
                DashStyle.Solid,
                ConnectionMethodKind.LeftSideMidpoint,
                false
            );
            AddOrth(
                MemopadToolIds.AddLineOrthogonalRightDirection,
                "直線 (経路 直角，接続位置 四辺の中点，方向右)",
                Resources.orthogonal_line,
                DashStyle.Solid,
                ConnectionMethodKind.RightSideMidpoint,
                false
            );

            AddOrth(
                MemopadToolIds.AddLineDashOrthogonalFreeDirection,
                "点線 (経路 直角，接続位置 四辺の中点，方向自由)",
                Resources.orthogonal_dashed_line,
                DashStyle.Dash,
                ConnectionMethodKind.SideMidpointOfOpposite,
                false
            );
            AddOrth(
                MemopadToolIds.AddLineDashOrthogonalUpDirection,
                "点線 (経路 直角，接続位置 四辺の中点，方向上)",
                Resources.orthogonal_dashed_line,
                DashStyle.Dash,
                ConnectionMethodKind.UpperSideMidpoint,
                false
            );
            AddOrth(
                MemopadToolIds.AddLineDashOrthogonalDownDirection,
                "点線 (経路 直角，接続位置 四辺の中点，方向下)",
                Resources.orthogonal_dashed_line,
                DashStyle.Dash,
                ConnectionMethodKind.LowerSideMidpoint,
                false
            );
            AddOrth(
                MemopadToolIds.AddLineDashOrthogonalLeftDirection,
                "点線 (経路 直角，接続位置 四辺の中点，方向左)",
                Resources.orthogonal_dashed_line,
                DashStyle.Dash,
                ConnectionMethodKind.LeftSideMidpoint,
                false
            );
            AddOrth(
                MemopadToolIds.AddLineDashOrthogonalRightDirection,
                "点線 (経路 直角，接続位置 四辺の中点，方向右)",
                Resources.orthogonal_dashed_line,
                DashStyle.Dash,
                ConnectionMethodKind.RightSideMidpoint,
                false
            );

        
            /// arrow
            AddOrth(
                MemopadToolIds.AddLineArrowOrthogonalFreeDirection,
                "直線 (経路 直角，接続位置 四辺の中点，方向自由)",
                Resources.orthogonal_arrow_line,
                DashStyle.Solid,
                ConnectionMethodKind.SideMidpointOfOpposite,
                true
            );
            AddOrth(
                MemopadToolIds.AddLineArrowOrthogonalUpDirection,
                "直線 (経路 直角，接続位置 四辺の中点，方向上)",
                Resources.orthogonal_arrow_line,
                DashStyle.Solid,
                ConnectionMethodKind.UpperSideMidpoint,
                true
            );
            AddOrth(
                MemopadToolIds.AddLineArrowOrthogonalDownDirection,
                "直線 (経路 直角，接続位置 四辺の中点，方向下)",
                Resources.orthogonal_arrow_line,
                DashStyle.Solid,
                ConnectionMethodKind.LowerSideMidpoint,
                true
            );
            AddOrth(
                MemopadToolIds.AddLineArrowOrthogonalLeftDirection,
                "直線 (経路 直角，接続位置 四辺の中点，方向左)",
                Resources.orthogonal_arrow_line,
                DashStyle.Solid,
                ConnectionMethodKind.LeftSideMidpoint,
                true
            );
            AddOrth(
                MemopadToolIds.AddLineArrowOrthogonalRightDirection,
                "直線 (経路 直角，接続位置 四辺の中点，方向右)",
                Resources.orthogonal_arrow_line,
                DashStyle.Solid,
                ConnectionMethodKind.RightSideMidpoint,
                true
            );

            AddOrth(
                MemopadToolIds.AddLineDashArrowOrthogonalFreeDirection,
                "点線 (経路 直角，接続位置 四辺の中点，方向自由)",
                Resources.orthogonal_dashed_arrow,
                DashStyle.Dash,
                ConnectionMethodKind.SideMidpointOfOpposite,
                true
            );
            AddOrth(
                MemopadToolIds.AddLineDashArrowOrthogonalUpDirection,
                "点線 (経路 直角，接続位置 四辺の中点，方向上)",
                Resources.orthogonal_dashed_arrow,
                DashStyle.Dash,
                ConnectionMethodKind.UpperSideMidpoint,
                true
            );
            AddOrth(
                MemopadToolIds.AddLineDashArrowOrthogonalDownDirection,
                "点線 (経路 直角，接続位置 四辺の中点，方向下)",
                Resources.orthogonal_dashed_arrow,
                DashStyle.Dash,
                ConnectionMethodKind.LowerSideMidpoint,
                true
            );
            AddOrth(
                MemopadToolIds.AddLineDashArrowOrthogonalLeftDirection,
                "点線 (経路 直角，接続位置 四辺の中点，方向左)",
                Resources.orthogonal_dashed_arrow,
                DashStyle.Dash,
                ConnectionMethodKind.LeftSideMidpoint,
                true
            );
            AddOrth(
                MemopadToolIds.AddLineDashArrowOrthogonalRightDirection,
                "点線 (経路 直角，接続位置 四辺の中点，方向右)",
                Resources.orthogonal_dashed_arrow,
                DashStyle.Dash,
                ConnectionMethodKind.RightSideMidpoint,
                true
            );
        }

        private void AddOrth(string id, string text, Image img, DashStyle dash, ConnectionMethodKind conn, bool arrow) {
            var feedback = new LineEdge() {
                LineColor = Color.DimGray,
                LineWidth = 1,
                LineDashStyle = dash,
                Router = new OrthogonalMidpointRouter(),
                ConnectionMethod = conn,
            };
            if (arrow) {
                feedback.TargetDecoration = new ArrowEdgeDecoration() {
                    Foreground = Color.DimGray,
                };
            }

            var tool = new AddEdgeTool(
                    new DelegatingModelFactory<MemoEdge>(
                        () => {
                            var ret = MemoFactory.CreateEdge();
                            ret.Kind = MemoEdgeKind.OrthogonalMidpoint;
                            if (arrow) {
                                ret.EndCapKind = MemoEdgeCapKind.Arrow;
                            }
                            return ret;
                        }
                    ),
                    feedback,
                    new MemoEdgeController(),
                    (edge) => {
                        edge.LineDashStyle = dash;
                    }
                );
            AddAddEdgeTool(id, text, img, tool);
        }


        public string GetCreateNodeToolText(string id) {
            if (_createNodeReg.ContainsKey(id)) {
                return _createNodeReg[id].Item1;
            }
            return _createNodeReg[MemopadToolIds.CreateRect].Item1;
        }

        public CreateNodeTool GetCreateNodeTool(string id) {
            if (_createNodeReg.ContainsKey(id)) {
                return _createNodeReg[id].Item2;
            }
            return _createNodeReg[MemopadToolIds.CreateRect].Item2;
        }

        public string GetAddEdgeToolText(string id) {
            if (_addEdgeReg.ContainsKey(id)) {
                return _addEdgeReg[id].Item1;
            }
            return _addEdgeReg[MemopadToolIds.AddLineCentral].Item1;
        }

        public Image GetAddEdgeToolImage(string id) {
            if (_addEdgeReg.ContainsKey(id)) {
                return _addEdgeReg[id].Item2;
            }
            return _addEdgeReg[MemopadToolIds.AddLineCentral].Item2;
        }

        public AddEdgeTool GetAddEdgeTool(string id) {
            if (_addEdgeReg.ContainsKey(id)) {
                return _addEdgeReg[id].Item3;
            }
            return _addEdgeReg[MemopadToolIds.AddLineCentral].Item3;
        }


        // ------------------------------
        // private
        // ------------------------------
        private void AddCreateNodeTool(string id, string text, Image image, SelectorCategory cate, CreateNodeTool tool) {
            AddCreateNodeTool(id, text, image, cate, tool, false);
        }

        private void AddCreateNodeTool(string id, string text, Image image, SelectorCategory cate, CreateNodeTool tool, bool flowBreak) {
            _createNodeReg.Add(id, Tuple.Create(text, tool));
            tool.DefaultTool = _defaultTool;
            AddTool(text, image, cate, tool, flowBreak);
        }


        private void AddCreateShapeTool(
            string id, string text, Image image, SelectorCategory cate, MemoShapeKind shapeKind, INode feedback
        ) {
            feedback.MinSize = new Size(16, 16);
            feedback.Foreground = SystemColors.ButtonShadow;
            feedback.Background = new SolidBrushDescription(Color.FromArgb(16, SystemColors.ButtonFace));
            var tool = new CreateNodeTool(
                new DelegatingModelFactory<MemoShape>(
                    () => {
                        var ret = MemoFactory.CreateShape();
                        ret.Kind = shapeKind;
                        ret.StyledText.Font = MemopadApplication.Instance.Settings.GetDefaultMemoContentFont();
                        return ret;
                    }
                    ),
                feedback
                );
            AddCreateNodeTool(id, text, image, cate, tool);
        }

        private void AddCreateEdgeTool(string text, Image image, SelectorCategory cate, CreateEdgeTool tool) {
            AddCreateEdgeTool(text, image, cate, tool, false);
        }

        private void AddCreateEdgeTool(string text, Image image, SelectorCategory cate, CreateEdgeTool tool, bool flowBreak) {
            tool.DefaultTool = _defaultTool;
            AddTool(text, image, cate, tool, flowBreak);
        }

        private void AddCreateCommentEdgeTool(string text, Image image, SelectorCategory cate, CreateCommentEdgeTool tool) {
            tool.DefaultTool = _defaultTool;
            AddTool(text, image, cate, tool, false);
        }

        private void AddAddEdgeTool(string id, string text, Image image, AddEdgeTool tool) {
            tool.DefaultTool = _defaultTool;
            _addEdgeReg.Add(id, Tuple.Create(text, image, tool));
            ///AddTool(text, image, cate, tool);
        }

        private void AddTool(string text, Image image, SelectorCategory cate, ITool tool, bool flowBreak) {
            cate.AddLabel(
                image,
                text,
                () => {
                    if (_form.EditorCanvas != null) {
                        _form.EditorCanvas.Tool = tool;
                        Hide();
                        _form.Activate();
                        _form.EditorCanvas.Select();
                    }
                },
                flowBreak
            );
        }
    }
}
