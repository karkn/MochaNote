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
using Mkamo.Editor.Requests;
using System.Drawing;
using Mkamo.Editor.Commands;
using Mkamo.Common.Command;
using Mkamo.Common.DataType;
using Mkamo.Common.Core;

namespace Mkamo.Editor.Roles {
    //using FormatAndPicker = Tuple<string, EditorDataPicker>;

    public class CopyRole: AbstractRole {
        // ========================================
        // field
        // ========================================
        //private Lazy<List<FormatAndPicker>> _formatAndCopier;

        // ========================================
        // constructor
        // ========================================
        public CopyRole() {
            //_formatAndCopier = new Lazy<List<FormatAndPicker>>(() => new List<FormatAndPicker>());
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public override bool CanUnderstand(IRequest request) {
            return request.Id == RequestIds.Copy;
        }

        public override ICommand CreateCommand(IRequest request) {
            var req = request as CopyRequest;
            if (req == null) {
                return null;
            }

            //var aggregatorMan = _Host.Site.EditorDataAggregatorManager;
            //aggregatorMan.Formats
            // text，htmlのコピーは全Editorに対してやるが，
            // それ以外でtargetのEditorに登録されているformatのコピーはtargetに対してだけやる
            
            return new CopyCommand(req.TargetEditors);
        }

        public override IFigure CreateFeedback(IRequest request) {
            return null;
        }

        public override void UpdateFeedback(IRequest request, IFigure feedback) {
        }

        public override void DisposeFeedback(IRequest request, IFigure feedback) {
        }

        //public void RegisterPicker(string format, EditorDataPicker picker) {
        //    if (!_formatAndCopier.Value.Any(item => item.Item1 == format)) {
        //        _formatAndCopier.Value.Add(Tuple.Create(format, picker));
        //    }
        //}
    }
}
