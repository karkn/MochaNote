/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Command;

namespace Mkamo.Editor.Core {
    /// <summary>
    /// Editorをまとめたもの．
    /// 複数Editorに対してのCommandの取得・実行を行う．
    /// </summary>
    public class EditorBundle {
        // ========================================
        // field
        // ========================================
        private List<IEditor> _editors;

        // ========================================
        // constructor
        // ========================================
        public EditorBundle(IEnumerable<IEditor> editors) {
            _editors = editors.ToList();
        }

        // ========================================
        // property
        // ========================================
        public IEnumerable<IEditor> Editors {
            get { return _editors; }
        }

        // ========================================
        // method
        // ========================================
        public bool CanUnderstandAll(IRequest request) {
            if (_editors.Count < 1) {
                return false;
            }

            for (int i = 0, len = _editors.Count; i < len; ++i) {
                var editor = _editors[i];
                if (!editor.CanUnderstand(request)) {
                    return false;
                }
            }
            return true;
        }

        public bool CanUnderstandAny(IRequest request) {
            if (_editors.Count < 1) {
                return false;
            }

            for (int i = 0, len = _editors.Count; i < len; ++i) {
                var editor = _editors[i];
                if (editor.CanUnderstand(request)) {
                    return true;
                }
            }
            return false;
        }

        public void ShowFeedback(IRequest request) {
            foreach (var editor in _editors) {
                editor.ShowFeedback(request);
            }
        }

        public void HideFeedback(IRequest request) {
            foreach (var editor in _editors) {
                editor.HideFeedback(request);
            }
        }

        public void HideFeedback(IRequest request, bool disposeFeedback) {
            foreach (var editor in _editors) {
                editor.HideFeedback(request, disposeFeedback);
            }
        }


        // --- group ---
        /// <summary>
        /// Editorsを対象としたグループ全体に対する一つのcommandを取得する．
        /// </summary>
        public ICommand GetGroupCommand(IRequest request, IEditor commandProvider) {
            if (!CanUnderstandAll(request)) {
                return null;
            }

            commandProvider = commandProvider?? _editors[0];

            return commandProvider.GetCommand(request);
        }

        public ICommand GetGroupCommand(IRequest request) {
            return GetGroupCommand(request, null);
        }

        public ICommand PerformGroupRequest(IRequest request, IEditor commandProvider, ICommandExecutor executor) {
            var cmd = GetGroupCommand(request, commandProvider);
            if (cmd == null) {
                return null;
            }

            commandProvider = commandProvider?? _editors[0];
            executor = executor?? commandProvider.Site.CommandExecutor;
            using (commandProvider.Site.DirtManager.BeginDirty()) {
                executor.Execute(cmd);
            }
            return cmd;
        }

        public ICommand PerformGroupRequest(IRequest request) {
            return PerformGroupRequest(request, null, null);
        }

        public ICommand PerformGroupRequest(IRequest request, IEditor commandProvider) {
            return PerformGroupRequest(request, commandProvider, null);
        }

        public ICommand PerformGroupRequest(IRequest request, ICommandExecutor executor) {
            return PerformGroupRequest(request, null, executor);
        }


        // --- composite ---
        /// <summary>
        /// CanUnderstand(request)なeditorを対象としたcommandをまとめたCompositeCommandを取得する．
        /// </summary>
        public ICommand GetCompositeCommand(IRequest request) {
            var ret = new CompositeCommand();
            foreach (var editor in _editors) {
                if (editor.CanUnderstand(request)) {
                    ret.Children.Add(editor.GetCommand(request));
                }
            }
            return ret;
        }

        /// <summary>
        /// CanUnderstand(request)なeditorを対象にrequestを実行させる。
        /// </summary>
        public ICommand PerformCompositeRequest(IRequest request, ICommandExecutor executor) {
            if (!_editors.Any()) {
                return null;
            }

            var cmd = GetCompositeCommand(request);
            if (cmd == null) {
                return null;
            }

            var editor = _editors[0];
            executor = executor?? _editors[0].Site.CommandExecutor;
            using (editor.Site.DirtManager.BeginDirty()) {
                executor.Execute(cmd);
            }
            return cmd;
        }

    }
}
