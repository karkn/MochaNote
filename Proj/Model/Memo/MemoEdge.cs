/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Association;
using Mkamo.Container.Core;
using Mkamo.Common.Externalize;
using Mkamo.Model.Core;
using System.Runtime.Serialization;

namespace Mkamo.Model.Memo {
    [Externalizable(Type = typeof(MemoEdge), FactoryMethodType = typeof(MemoFactory), FactoryMethod = "CreateEdge")]
    [DataContract]
    [Serializable]
    public class MemoEdge: MemoContent {
        // ========================================
        // field
        // ========================================
        [DataMember(Name = "Source")]
        private MemoNode _source;
        [DataMember(Name = "Target")]
        private MemoNode _target;

        [DataMember(Name = "CanConnectSource")]
        private bool _canConnectSource = true;
        [DataMember(Name = "CanConnectTarget")]
        private bool _canConnectTarget = true;

        [DataMember(Name = "Kind")]
        private MemoEdgeKind _kind = MemoEdgeKind.Normal;
        //private MemoEdgeDashStyle _dashStyle;

        [DataMember(Name = "StartCapKind")]
        private MemoEdgeCapKind _startCapKind = MemoEdgeCapKind.Normal;
        [DataMember(Name = "EndCapKind")]
        private MemoEdgeCapKind _endCapKind = MemoEdgeCapKind.Normal;

        // ========================================
        // constructor
        // ========================================
        protected internal MemoEdge() {
        }

        protected internal MemoEdge(MemoNode source, MemoNode target) {
            Source = source;
            Target = target;
        }

        // ========================================
        // property
        // ========================================
        [Persist, External]
        public virtual MemoNode Source {
            get { return _source; }
            set {
                var old = _source;
                var result = AssociationUtil.EnsureAssociation(
                    _source,
                    value,
                    node => _source = node,
                    node => {
                        if (!node.Outgoings.Contains(this)) {
                            node.Outgoings.Add(this);
                        }
                    },
                    node => node.Outgoings.Remove(this)
                );
                if (result != AssociationUtil.EnsureResult.None) {
                    OnPropertySet(this, "Source", old, value);
                }
            }
        }

        [Persist, External]
        public virtual MemoNode Target {
            get { return _target; }
            set {
                var old = _target;
                var result = AssociationUtil.EnsureAssociation(
                    _target,
                    value,
                    node => _target = node,
                    node => {
                        if (!node.Incomings.Contains(this)) {
                            node.Incomings.Add(this);
                        }
                    },
                    node => node.Incomings.Remove(this)
                );
                if (result != AssociationUtil.EnsureResult.None) {
                    OnPropertySet(this, "Target", old, value);
                }
            }
        }

        [Persist, External]
        public virtual MemoEdgeKind Kind {
            get { return _kind; }
            set {
                if (value == _kind) {
                    return;
                }
                var old = _kind;

                /// 初期にCap情報もEdgeKindに含ませていたための対応
                if (value == MemoEdgeKind.Arrow) {
                    _kind = MemoEdgeKind.Normal;
                    _endCapKind = MemoEdgeCapKind.Arrow;
                } else if (value == MemoEdgeKind.OrthogonalArrow) {
                    _kind = MemoEdgeKind.Orthogonal;
                    _endCapKind = MemoEdgeCapKind.Arrow;
                } else {
                    _kind = value;
                }

                OnPropertySet(this, "Kind", old, value);
            }
        }


        [Persist, External]
        public virtual MemoEdgeCapKind StartCapKind {
            get { return _startCapKind; }
            set {
                if (_startCapKind == value) {
                    return;
                }
                var old = _startCapKind;
                _startCapKind = value;
                OnPropertySet(this, "StartCapKind", old, value);
            }
        }

        [Persist, External]
        public virtual MemoEdgeCapKind EndCapKind {
            get { return _endCapKind; }
            set {
                if (_endCapKind == value) {
                    return;
                }
                var old = _endCapKind;
                _endCapKind = value;
                OnPropertySet(this, "EndCapKind", old, value);
            }
        }

        [Persist, External]
        public virtual bool CanConnectSource {
            get { return _canConnectSource; }
            set {
                if (_canConnectSource == value) {
                    return;
                }
                var old = _canConnectSource;
                _canConnectSource = value;
                OnPropertySet(this, "CanConnectSource", old, value);
            }
        }

        [Persist, External]
        public virtual bool CanConnectTarget {
            get { return _canConnectTarget; }
            set {
                if (_canConnectTarget == value) {
                    return;
                }
                var old = _canConnectTarget;
                _canConnectTarget = value;
                OnPropertySet(this, "CanConnectTarget", old, value);
            }
        }

        // ========================================
        // method
        // ========================================

    }
}
