/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Figure.Core;
using Mkamo.Common.Structure;
using Mkamo.Common.Visitor;
using Mkamo.Figure.Figures;
using Mkamo.Common.Event;
using Mkamo.Editor.Internal.Core;
using Mkamo.Common.Collection;
using System.Drawing;
using Mkamo.Common.Externalize;
using Mkamo.Common.Command;
using System.Collections.ObjectModel;
using Mkamo.Common.Core;
using Mkamo.Common.Diagnostics;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel;
using Mkamo.Common.Forms.Drawing;

namespace Mkamo.Editor.Internal.Editors {

    /// <summary>
    /// Editor実装．
    /// 
    /// 
    /// ==Editorの追加・削除とModelの追加・削除について==
    /// 
    /// * EditorはIEditor#Childrenでの追加・削除
    ///   Modelには反映しない
    /// * 
    /// </summary>
    [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = false)]
    internal partial class Editor: IEditor, IStructured<Editor> {
        // ========================================
        // static field
        // ========================================
        private static Editor[] EmptyEditors = new Editor[0];

        // ========================================
        // field
        // ========================================
        private readonly StructuredSupport<Editor> _structure;
        private readonly VisitableSupport<IEditor> _visitable;
        private readonly FigureStructureMaintainer _figureMaintainer;

        private readonly List<IRole> _roles;
        private readonly Dictionary<string, IFigure> _requestIdToFeedback;

        private readonly List<IEditorHandle> _editorHandles;
        private readonly InsertionOrderedDictionary<IAuxiliaryHandle, HandleStickyKind> _auxHandleToStickyKind;

        private IFocus _focus;

        private IController _controller;
        private IFigure _figure;
        private object _model;
        private INotifyPropertyChanged _notifier;

        private bool _isEnabled;

        private bool _canSelect;
        private bool _isFocused;
        private bool _canFocus;

        private bool _isMouseEntered;

        private IDictionary<string, object> _transientData; /// lazy

        //private string _transientId;

        // ========================================
        // constructor
        // ========================================
        public Editor() {
            /// Editorの親子構造の準備
            _structure = new StructuredSupport<Editor>(this);
            _structure.DetailedPropertyChanging += (sender, e) => {
                if (e.PropertyName == ICompositeProperty.Parent) {
                    OnParentChanging(e);
                } else if (e.PropertyName == ICompositeProperty.Children) {
                    OnChildrenChanging(e);
                }
            };
            _structure.DetailedPropertyChanged += (sender, e) => {
                if (e.PropertyName == ICompositeProperty.Parent) {
                    OnParentChanged(e);
                } else if (e.PropertyName == ICompositeProperty.Children) {
                    OnChildrenChanged(e);
                }
            };

            _visitable = new VisitableSupport<IEditor>(
                this,
                editor => {
                    if (_structure.HasChildren) {
                        return editor.Children;
                    } else {
                        return EmptyEditors;
                    }
                }
            );


            _figureMaintainer = new FigureStructureMaintainer(this);

            _roles = new List<IRole>();
            _requestIdToFeedback = new Dictionary<string, IFigure>();

            _editorHandles = new List<IEditorHandle>();
            _auxHandleToStickyKind = new InsertionOrderedDictionary<IAuxiliaryHandle, HandleStickyKind>();

            _isEnabled = false;

            _canSelect = true;
            _isFocused = false;
            _canFocus = true;

            _isMouseEntered = false;

            //_transientId = Guid.NewGuid().ToString();
        }

        // ========================================
        // event
        // ========================================
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;
        public event EventHandler<EventArgs> FocusChanged;
        
        // ========================================
        // property
        // ========================================
        // === IStructured ==========
        public Editor Parent {
            get { return _structure.Parent; }
            set { _structure.Parent = value; }
        }

        public Collection<Editor> Children {
            get { return _structure.Children; }
        }

        // === IEditor ==========
        // --- structure ---
        IEditor IEditor.Parent {
            get { return _structure.Parent; }
        }

        IEnumerable<IEditor> IEditor.Children {
            get {
                if (_structure.HasChildren) {
                    foreach (var child in _structure.Children) {
                        yield return child;
                    }
                }
            }
        }

        // --- core ---
        public IController Controller {
            get { return _Controller; }
        }

        public IFigure Figure {
            get { return _Figure; }
            set { _Figure = value; }
        }

        public object Model {
            get { return _model; }
            set {
                Contract.Requires(value != null);

                if (value == _model) {
                    return;
                }

                if (_model != null) {
                    if (_notifier != null) {
                        _notifier.PropertyChanged -= HandleModelPropertyChanged;
                    }
                }
                _model = value;
                if (_model != null) {
                    _notifier = _model as INotifyPropertyChanged;
                    if (_notifier != null && IsEnabled) {
                        _notifier.PropertyChanged += HandleModelPropertyChanged;
                    }
                }

                Refresh(new RefreshContext(EditorRefreshKind.ModelSet));
            }
        }

        // --- controller and figure type ---
        public bool IsContainer {
            get { return _controller is IContainerController; }
        }

        public bool IsConnectable {
            get { return _controller is IConnectableController && _figure is IConnectable; }
        }

        public bool IsConnection {
            get { return _controller is IConnectionController && _figure is IConnection; }
        }


        // --- role ---
        public IList<IRole> Roles {
            get { return _roles; }
        }

        // --- structure ---
        public virtual IRootEditor Root {
            get { return Parent == null? null: Parent.Root; }
        }

        public bool HasRoot {
            get { return Root != null; }
        }

        public virtual bool IsRoot {
            get { return false; }
        }

        public bool HasNextSibling {
            get { return _structure.HasNextSibling; }
        }

        public bool HasPreviousSibling {
            get { return _structure.HasPreviousSibling; }
        }

        public IEditor NextSibling {
            get { return _structure.NextSibling; }
        }

        public IEditor PreviousSibling {
            get { return _structure.PreviousSibling; }
        }

        // --- decoration ---
        public IList<IEditorHandle> EditorHandles {
            get { return _editorHandles; }
        }

        public IList<IAuxiliaryHandle> AuxiliaryHandles {
            get { return _auxHandleToStickyKind.InsertionOrderedKeys; }
        }

        public IFocus Focus {
            get { return _focus; }
        }


        // --- selection ---
        public bool IsSelected {
            get { return Site.SelectionManager.SelectedEditors.Contains(this); }
            set {
                if (value == IsSelected) {
                    return;
                }
                if (CanSelect) {
                    if (value) {
                        Site.SelectionManager.Select(this);
                    } else {
                        Site.SelectionManager.Deselect(this);
                    }
                    OnSelectionChanged(value);
                }
            }
        }

        public bool CanSelect {
            get { return _canSelect; }
            set {
                if (value == _canSelect) {
                    return;
                }
                if (!value && IsSelected) {
                    IsSelected = false;
                }
                _canSelect = value;
            }
        }

        // --- focus ---
        public bool IsFocused {
            get { return _isFocused; }
            set {
                if (value == _isFocused) {
                    return;
                }

                /// ここではFocus#Begin()/Commit()/Rollback()は呼び出さない
                /// フィールド値のセットとFocus#Figureのfocus管理のみ
                if (CanFocus) {
                    _isFocused = value;
                    if (value) {
                        Site.FocusManager.PerformFocus(this);
                    } else {
                        Site.FocusManager.ClearFocus();
                    }
                    OnFocusChanged();
                } else {
                    Site.FocusManager.ClearFocus();
                }
            }
        }

        public bool CanFocus {
            get { return _canFocus && _focus != null; }
            set {
                if (value == _canFocus) {
                    return;
                }
                if (!value && IsFocused) {
                    IsFocused = false;
                }
                _canFocus = value;
            }
        }


        // --- mouse enter ---
        public bool IsMouseEntered {
            get { return _isMouseEntered; }
        }

        // --- visible ---
        public bool IsEnabled {
            get { return _isEnabled; }
            set {
                if (value == _isEnabled) {
                    return;
                }
                if (value) {
                    Enable();
                } else {
                    Disable();
                }
            }
            //get { return _figure != null && _figure.IsVisible; }
            //set {
            //    if (_figure == null) {
            //        return;
            //    }
            //    if (value == _figure.IsVisible) {
            //        return;
            //    }
            //    if (value) {
            //        Enable();
            //    } else {
            //        Disable();
            //    }
            //}
        }
                

        // --- service ---
        public virtual IEditorSite Site {
            get { return (Root == null? null: Root.Site)?? EditorConsts.NullEditorSite; }
        }


        // --- misc ---
        public virtual IDictionary<string, object> TransientData {
            get {
                if (_transientData == null) {
                    _transientData = new Dictionary<string, object>();
                }
                return _transientData;
            }
        }

        /// <summary>
        /// アプリケーション起動中の間だけアプリケーション内のみで有効なIdを取得する．
        /// </summary>
        //public string TransientId {
        //    get { return _transientId; }
        //}

        // ------------------------------
        // internal
        // ------------------------------
        // --- core ---
        internal IController _Controller {
            get { return _controller; }
            set {
                Contract.Requires(value != null);

                if (value == _controller) {
                    return;
                }

                if (_controller != null) {
                    _controller.Uninstalled(this);
                }
                _controller = value;
                if (_controller != null) {
                    _controller.Installed(this);
                }
            }
        }

        internal IFigure _Figure {
            get { return _figure; }
            set {
                Contract.Requires(value != null, "value");

                if (value == _figure) {
                    return;
                }

                var parent = Parent as Editor;
                var oldIndex = -1;
                var oldFigure = _figure;
                if (oldFigure != null) {
                    if (!((value is INode && oldFigure is INode) || (value is IEdge && oldFigure is IEdge))) {
                        throw new InvalidOperationException("Unsupported figure replacement");
                    }

                    if (parent != null) {
                        oldIndex = parent._FigureMaintainer.ChildEditorFigures.IndexOf(oldFigure);
                        parent._FigureMaintainer.RemoveChildEditorFigure(oldFigure);
                    }

                    foreach (var handle in _editorHandles) {
                        handle.Uninstall(this);
                    }

                    oldFigure.MouseEnter -= HandleFigureMouseEnter;
                    oldFigure.MouseLeave -= HandleFigureMouseLeave;

                    oldFigure.BoundsChanged -= HandleFigureBoundsChanged;
                    if (oldFigure is IEdge) {
                        var edge = oldFigure as IEdge;
                        edge.EdgePointsChanged -= HandleFigureEdgePointsChanged;
                    }
                }

                _figure = value;
                _figure.IsVisible = false;

                if (_figure != null) {
                    if (parent != null && oldIndex > -1) {
                        parent._FigureMaintainer.InsertChildEditorFigure(_figure, oldIndex);
                    }

                    _figureMaintainer.EditorFigure = _figure;

                    foreach (var handle in _editorHandles) {
                        handle.Install(this);
                    }

                    _figure.MouseEnter += HandleFigureMouseEnter;
                    _figure.MouseLeave += HandleFigureMouseLeave;

                    _figure.BoundsChanged += HandleFigureBoundsChanged;
                    if (_figure is IEdge) {
                        var edge = _figure as IEdge;
                        edge.EdgePointsChanged += HandleFigureEdgePointsChanged;
                    }

                    if (oldFigure != null) {
                        if (_figure is INode && oldFigure is INode) {
                            _figure.Bounds = oldFigure.Bounds;

                        } else if (_figure is IEdge && oldFigure is IEdge) {
                            /// EdgePointsを合わせる
                            using (_figure.DirtManager.BeginDirty()) {
                                var edge = _figure as IEdge;
                                var oldEdge = oldFigure as IEdge;
                                edge.ClearBendPoints();
                                edge.First = oldEdge.First;
                                edge.Last = oldEdge.Last;
                                foreach (var pt in oldEdge.BendPoints) {
                                    edge.AddBendPoint(pt);
                                }
                            }
                        }
                    }
                }
            }
        }

        internal FigureStructureMaintainer _FigureMaintainer {
            get { return _figureMaintainer; }
        }

        // ========================================
        // method
        // ========================================
        // === IStructured ==========
        public void Accept(IVisitor<IEditor> visitor) {
            _visitable.Accept(visitor);
        }

        public void Accept(IVisitor<IEditor> visitor, NextVisitOrder order) {
            _visitable.Accept(visitor, order);
        }

        public void Accept(Predicate<IEditor> visitPred) {
            _visitable.Accept(visitPred);
        }

        public void Accept(
            Predicate<IEditor> visitPred,
            Action<IEditor> endVisitAction,
            NextVisitOrder order
        ) {
            _visitable.Accept(visitPred, endVisitAction, order);
        }

        // === IEditor ==========
        public void Enable() {
            if (!_isEnabled) {
                _isEnabled = true;
                if (_notifier != null) {
                    _notifier.PropertyChanged += HandleModelPropertyChanged;
                }
                if (Controller != null) {
                    Controller.Activate();
                }
                if (_figure != null) {
                    _figure.IsVisible = true;
                    if (Root != null) {
                        Site.UpdateHandleLayer();
                    }
                    RelocateAllHandles();
                    RelocateFocus();
                }

                if (_structure.HasChildren) {
                    foreach (var child in Children) {
                        child.Enable();
                    }
                }
            }
        }

        public void Disable() {
            Disable(false);
        }

        public void Disable(bool onlyController) {
            if (_isEnabled) {
                if (!onlyController && _figure != null) {
                    _figure.IsVisible = false;
                    if (Root != null) {
                        Site.UpdateHandleLayer();
                    }
                }
                if (Controller != null) {
                    Controller.Deactivate();
                }
                if (_notifier != null) {
                    _notifier.PropertyChanged -= HandleModelPropertyChanged;
                }
                _isEnabled = false;

                if (_structure.HasChildren) {
                    foreach (var child in Children) {
                        child.Disable(onlyController);
                    }
                }
            }
        }

        public IEnumerable<IEditor> GetChildrenByPosition() {
            if (!_structure.HasChildren) {
                return EmptyEditors;
            }

            var ret = _structure.Children.OrderBy(
                editor => editor.Figure.Bounds,
                new RectanglePositionalComparer()
            );
            return ret.ToArray();
        }

        // --- request ---
        public void InstallRole(IRole role) {
            if (_roles.Contains(role)) {
                return;
            }
            role.Installed(this);
            _roles.Add(role);
        }

        public bool CanUnderstand(IRequest request) {
            foreach (IRole role in _roles) {
                if (role.CanUnderstand(request)) {
                    return true;
                }
            }
            return false;
        }

        public bool CanUnderstandAll(IEnumerable<IRequest> requests) {
            foreach (var req in requests) {
                if (!CanUnderstand(req)) {
                    return false;
                }
            }
            return true;
        }

        public ICommand GetCommand(IRequest request) {
            foreach (var role in _roles) {
                if (role.CanUnderstand(request)) {
                    return role.CreateCommand(request);
                }
            }
            return null;
        }

        public ICommand GetChainedCommand(IEnumerable<IRequest> requests) {
            var ret = default(ICommand);

            foreach (var req in requests) {
                var cmd = GetCommand(req);
                if (cmd != null) {
                    ret = ret == null? cmd: ret.Chain(cmd);
                }
            }

            return ret;
        }

        public ICommand PerformRequest(IRequest request) {
            using (Figure.DirtManager.BeginDirty()) {
                var cmd = GetCommand(request);
                if (cmd != null) {
                    Site.CommandExecutor.Execute(cmd);
                }
                return cmd;
            }
        }

        public void ShowFeedback(IRequest request) {
            foreach (var role in _roles) {
                if (role.CanUnderstand(request)) {
                    var feedback = default(IFigure);
                    if (_requestIdToFeedback.ContainsKey(request.Id)) {
                        feedback = _requestIdToFeedback[request.Id];
                    } else {
                        feedback = request.CustomFeedback?? role.CreateFeedback(request);
                        if (feedback != null) {
                            _requestIdToFeedback.Add(request.Id, feedback);
                        }
                    }

                    if (feedback != null) {
                        using (Site.EditorCanvas.DirtManager.BeginDirty()) {
                            if (!Site.FeedbackLayer.Children.Contains(feedback)) {
                                Site.FeedbackLayer.Children.Add(feedback);
                            }
                            role.UpdateFeedback(request, feedback);
                        }
                    }
                    return;
                }
            }
        }

        public void HideFeedback(IRequest request) {
            HideFeedback(request, true);
        }

        public void HideFeedback(IRequest request, bool disposeFeedback) {
            if (!_requestIdToFeedback.ContainsKey(request.Id)) {
                return;
            }
            var feedback = _requestIdToFeedback[request.Id];
            foreach (var role in _roles) {
                if (role.CanUnderstand(request)) {
                    using (feedback.DirtManager.BeginDirty()) {
                        Site.FeedbackLayer.Children.Remove(feedback);
                        if (disposeFeedback) {
                            role.DisposeFeedback(request, feedback);
                            _requestIdToFeedback.Remove(request.Id);
                        }
                        return;
                    }
                }
            }
        }


        // --- decoration ---
        public void InstallEditorHandle(IEditorHandle handle) {
            if (handle == null || _editorHandles.Contains(handle)) {
                return;
            }
            handle.Install(this);
            _editorHandles.Add(handle);
        }

        public void InstallHandle(IAuxiliaryHandle handle) {
            InstallHandle(handle, HandleStickyKind.Selected);
        }

        public void InstallHandle(IAuxiliaryHandle handle, HandleStickyKind stickyKind) {
            Contract.Requires(
                handle != null && handle.Figure != null && !_auxHandleToStickyKind.ContainsKey(handle)
            );

            handle.Install(this);
            _auxHandleToStickyKind.Add(handle, stickyKind);

            handle.Figure.Accept(
                fig => {
                    fig.SetEditor(this);
                    return false;
                }
            );

            if (Root != null) {
                Site.UpdateHandleLayer();
            }
        }

        public HandleStickyKind GetStickyKind(IAuxiliaryHandle handle) {
            Contract.Requires(_auxHandleToStickyKind.ContainsKey(handle));
            return _auxHandleToStickyKind[handle];
        }

        public void InstallFocus(IFocus focus) {
            if (focus == null || focus.Figure == null || focus == _focus) {
                return;
            }
            focus.Install(this);
            _focus = focus;
            if (_focus.Figure != null) {
                _focus.Figure.BoundsChanged += HandleFocusFigureBoundsChanged;
            }
        }

        // --- find ---
        public IList<IEditor> GetChildEditorsFor(object model) {
            if (!_structure.HasChildren) {
                return EmptyEditors;
            }

            var ret = new List<IEditor>();
            foreach (var child in Children) {
                if (child.Model == model) {
                    ret.Add(child);
                }
            }
            return ret;
        }

        public virtual IEditor FindEditor(Predicate<IEditor> finder) {
            var found = default(IEditor);
            if (_structure.HasChildren) {
                for (int i = Children.Count - 1; i >= 0; --i) {
                    found = Children[i].FindEditor(finder);
                    if (found != null) {
                        return found;
                    }
                }
            }
            if (finder(this)) {
                return this;
            }
            return null;
        }

        public virtual IEditor FindEditor(Point pt, Predicate<IEditor> finder) {
            if (!Figure.ContainsPoint(pt)) {
                /// handleがptを含むか調べる
                var handleFound = false;
                foreach (var auxHandle in _auxHandleToStickyKind.Keys) {
                    var auxFig = auxHandle.Figure;
                    if (auxFig.IsVisible && auxFig.ContainsPoint(pt)) {
                        handleFound = true;
                        break;
                    }
                }
                if (!handleFound) {
                    return null;
                }
            }

            var found = default(IEditor);
            if (_structure.HasChildren) {
                for (int i = Children.Count - 1; i >= 0; --i) {
                    found = Children[i].FindEditor(pt, finder);
                    if (found != null) {
                        return found;
                    }
                }
            }

            if (finder(this)) {
                return this;
            }
            return null;
        }

        public virtual IEditor FindEnabledEditor(Point pt) {
            return FindEditor(pt, editor => editor.IsEnabled);
        }

        public virtual IEditor FindEditorFor(object model) {
            return FindEditor(editor => editor.Model == model);
        }
        
        public virtual IList<IEditor> FindEditors(Predicate<IEditor> finder) {
            List<IEditor> ret = new List<IEditor>();

            Accept(
                null,
                fig => {
                    if (finder(fig)) {
                        ret.Add(fig);
                    }
                },
                NextVisitOrder.NegativeOrder
            );

            return ret;
        }

        public IList<IEditor> FindEditors(Point pt, Predicate<IEditor> finder) {
            List<IEditor> ret = new List<IEditor>();

            Accept(
                editor => !editor.Figure.ContainsPoint(pt),
                editor => {
                    if (editor.Figure.ContainsPoint(pt) && finder(editor)) {
                        ret.Add(editor);
                    }
                },
                NextVisitOrder.NegativeOrder
            );

            return ret;
        }

        public virtual IList<IEditor> FindEditorsFor(object model) {
            return FindEditors(editor => editor.Model == model);
        }
        
        // --- refresh ---
        public void Refresh(RefreshContext context) {
            if (Figure == null || Controller == null) {
                return;
            }
            using (Figure.DirtManager.BeginDirty()) {
                var containerCtrl = Controller as IContainerController;
                if (containerCtrl != null && containerCtrl.SyncChildEditors) {
                    RebuildChildren();
                }

                RefreshFigure(context);
            }
        }

        public void RefreshFigure(RefreshContext context) {
            if (Figure == null || Controller == null) {
                return;
            }

            using (Figure.DirtManager.BeginDirty()) {
                Controller.RefreshEditor(context, Figure, Model);
                Figure.InvalidatePaint();
            }
        }

        public void RebuildChildren() {
            if (Figure == null || Controller == null) {
                return;
            }
            var container = Controller as IContainerController;
            if (container == null) {
                return;
            }

            /// Editorの子構造とModelの子構造を同期
            using (Figure.DirtManager.BeginDirty()) {
                /// ModelにはあるけどEditorにない子のEditorを作成
                foreach (var cChild in container.Children) {
                    var found = Children.Find(childEditor => childEditor.Model == cChild);
                    if (found == null) {
                        var childEditor = EditorFactory.CreateEditor(cChild, Site.ControllerFactory);
                        AddChildEditor(childEditor);
                        childEditor.Enable();
                    }
                }
    
                /// EditorにはあるけどModelにはない子のEditorを削除
                var removeds = new List<Editor>();
                if (_structure.HasChildren) {
                    foreach (var childEditor in Children) {
                        var found = container.Children.Find(child => childEditor.Model == child);
                        if (found == null) {
                            removeds.Add(childEditor);
                            childEditor.Disable();
                        }
                    }
                }
                removeds.ForEach(e => RemoveChildEditor(e));
            }
        }


        // ------------------------------
        // protected
        // ------------------------------
        // --- handle ---
        protected void RelocateAllHandles() {
            using (Figure.DirtManager.BeginDirty()) {
                foreach (var handle in _auxHandleToStickyKind.Keys) {
                    handle.Relocate(Figure);
                }
            }
        }

        // --- focus ---
        protected void RelocateFocus() {
            using (Figure.DirtManager.BeginDirty()) {
                if (_focus != null) {
                    _focus.Relocate(Figure);
                }
            }
        }

        // --- event ---
        protected virtual void OnParentChanging(DetailedPropertyChangingEventArgs e) {
        }

        protected virtual void OnParentChanged(DetailedPropertyChangedEventArgs e) {
        }

        protected virtual void OnChildrenChanging(DetailedPropertyChangingEventArgs e) {
            /// 子のIsSelectedをfalseにする
            using (Figure.DirtManager.BeginDirty()) {
                switch (e.Kind) {
                    case PropertyChangeKind.Remove: {
                        var oldChild = e.OldValue as Editor;
                        if (oldChild != null && oldChild.IsSelected) {
                            oldChild.IsSelected = false;
                        }
                        break;
                    }
                    case PropertyChangeKind.Clear: {
                        var oldChildren = e.OldValue as Editor[];
                        if (oldChildren != null) {
                            foreach (var oldChild in oldChildren) {
                                if (oldChild.IsSelected) {
                                    oldChild.IsSelected = false;
                                }
                            }
                        }
                        break;
                    }
                    case PropertyChangeKind.Set: {
                        var oldChild = e.OldValue as Editor;
                        if (oldChild != null && oldChild.IsSelected) {
                            oldChild.IsSelected = false;
                        }
                        break;
                    }
                    default: {
                        break;
                    }
                }
            }
        }

        protected virtual void OnChildrenChanged(DetailedPropertyChangedEventArgs e) {
            using (Figure.DirtManager.BeginDirty()) {
                /// Figureの構造をEditorの構造に同期
                switch (e.Kind) {
                    case PropertyChangeKind.Add: {
                        var newChild = e.NewValue as Editor;
                        if (newChild != null && e.IsIndexed) {
                            _figureMaintainer.InsertChildEditorFigure(newChild.Figure, e.Index);
                        }
                        break;
                    }
                    case PropertyChangeKind.Remove: {
                        var oldChild = e.OldValue as Editor;
                        if (oldChild != null) {
                            _figureMaintainer.RemoveChildEditorFigure(oldChild.Figure);
                        }
                        break;
                    }
                    case PropertyChangeKind.Clear: {
                        _figureMaintainer.ClearChildEditorFigure();
                        break;
                    }
                    case PropertyChangeKind.Set: {
                        var oldChild = e.OldValue as Editor;
                        var newChild = e.NewValue as Editor;
                        if (oldChild != null) {
                            _figureMaintainer.RemoveChildEditorFigure(oldChild.Figure);
                        }
                        if (newChild != null && e.IsIndexed) {
                            _figureMaintainer.InsertChildEditorFigure(newChild.Figure, e.Index);
                        }
                        break;
                    }
                    default: {
                        break;
                    }
                }

                if (Root != null) {
                    Site.UpdateHandleLayer();
                }
            }
        }

        protected virtual void OnSelectionChanged(bool isSelected) {
            var handler = SelectionChanged;
            if (handler != null) {
                handler(this, new SelectionChangedEventArgs(isSelected));
            }
        }

        protected void OnFocusChanged() {
            var handler = FocusChanged;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnFigureBoundsChanged(BoundsChangedEventArgs e) {
            using (Figure.DirtManager.BeginDirty()) {
                RelocateAllHandles();
                RelocateFocus();
            }
        }

        protected virtual void OnFigureEdgePointsChanged(DetailedPropertyChangedEventArgs e) {
            using (Figure.DirtManager.BeginDirty()) {
                RelocateAllHandles();
                RelocateFocus();
            }
        }

        // ------------------------------
        // internal
        // ------------------------------
        /// <summary>
        /// modelに対応するeditorを生成し，子に追加する．
        /// Controllerへの通知も行う．
        /// </summary>
        internal Editor AddChild(object childModel) {
            Contract.Requires(childModel != null);
            var containerCtrl = Controller as IContainerController;
            Contract.Ensures(containerCtrl != null);
            return InsertChild(childModel, containerCtrl.ChildCount);
        }

        /// <summary>
        /// childを子に追加する．
        /// Controllerへの通知は行わない．
        /// </summary>
        internal void AddChildEditor(Editor childEditor) {
            Contract.Requires(childEditor != null);

            InsertChildEditor(childEditor, Children.Count);
        }

        /// <summary>
        /// modelに対応するeditorを生成し，子に追加する．
        /// Controllerへの通知も行う．
        /// </summary>
        internal Editor InsertChild(object childModel, int index) {
            var containerCtrl = Controller as IContainerController;
            Contract.Requires(containerCtrl != null);
            //Require.Argument(containerCtrl.CanCreateChild(childModel), "childModel");

            var childEditor = EditorFactory.CreateEditor(childModel, Site.ControllerFactory);

            /// InsertChildEditorしてからcontainerCtrl.InsertChild()しないと
            /// PropertyChangedでcontrolからRefresh()が呼ばれて子editorが二重登録される．
            /// なので必ずInsertChildEditor()，containerCtrl.InsertChild()の順に呼ぶ．
            InsertChildEditor(childEditor, index);
            containerCtrl.InsertChild(childModel, index);

            return childEditor;
        }

        /// <summary>
        /// childを子に追加する．
        /// Controllerへの通知は行わない．
        /// </summary>
        internal void InsertChildEditor(Editor childEditor, int index) {
            Contract.Requires(childEditor != null);
            Children.Insert(index, childEditor);
            if (!_inLoading) {
                childEditor.Refresh(new RefreshContext(EditorRefreshKind.Inserted));
            }
            //childEditor.Controller.Activate();
        }

        /// <summary>
        /// modelに対応するeditorを子から削除する．
        /// Controllerへの通知も行う．
        /// </summary>
        internal bool RemoveChild(object childModel) {
            Contract.Requires(childModel != null);

            var containerCtrl = Controller as IContainerController;
            Contract.Requires(containerCtrl != null);

            var childEditors = GetChildEditorsFor(childModel);
            if (childEditors == null || childEditors.Count == 0) {
                return false;
            }

            foreach (var childEditor in childEditors) {
                if (RemoveChildEditor(childEditor as Editor)) {
                    containerCtrl.RemoveChild(childModel);
                }
            }
            return true;
        }

        /// <summary>
        /// childを子から削除する．
        /// Controllerへの通知は行わない．
        /// </summary>
        internal bool RemoveChildEditor(Editor childEditor) {
            if (childEditor == null || childEditor.Parent != this) {
                return false;
            }

            //childEditor.Controller.Deactivate();
            return Children.Remove(childEditor);
        }


        // ------------------------------
        // private
        // ------------------------------
        private void HandleFigureBoundsChanged(object sender, BoundsChangedEventArgs e) {
            OnFigureBoundsChanged(e);
        }

        private void HandleFigureEdgePointsChanged(object sender, DetailedPropertyChangedEventArgs e) {
            OnFigureEdgePointsChanged(e);
        }

        private void HandleFigureMouseEnter(object sender, EventArgs e) {
            _isMouseEntered = true;
            if (_auxHandleToStickyKind.Values.Any(kind => kind == HandleStickyKind.MouseEnter)) {
                Site.UpdateHandleLayer();
            }
        }

        private void HandleFigureMouseLeave(object sender, EventArgs e) {
            _isMouseEntered = false;
            if (_auxHandleToStickyKind.Values.Any(kind => kind == HandleStickyKind.MouseEnter)) {
                Site.UpdateHandleLayer();
            }
        }

        private void HandleFocusFigureBoundsChanged(object sender, BoundsChangedEventArgs e) {
            if (_isFocused) {
                RelocateAllHandles();
            }
        }

        private void HandleModelPropertyChanged(object sender, PropertyChangedEventArgs e) {
            //OnModelPropertyChanged(e);
            Refresh(new RefreshContext(EditorRefreshKind.ModelUpdated, e));
        }

    }
}
