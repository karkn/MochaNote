/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Forms.DetailSettings;
using Mkamo.Model.Uml;
using Mkamo.Common.Command;
using System.Reflection;

namespace Mkamo.Memopad.Internal.Controllers.UIProviders {
    internal class UmlOperationUIProvider: AbstractUmlFeatureUIProvider {
        // ========================================
        // field
        // ========================================
        private UmlOperation _operation;

        // ========================================
        // constructor
        // ========================================
        public UmlOperationUIProvider(UmlOperationController controller, UmlOperation property): base(controller) {
            _operation = property;
        }

        
        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public override void ConfigureDetailForm(DetailSettingsForm detailForm) {
            /// property detail page
            var propObj = new PropObj() {
                Name = _operation.Name,
                Stereotype = _operation.Stereotype,
                Type = _operation.TypeName,
                Parameters = _operation.Parameters,
                IsAbstract = _operation.IsAbstract,
                IsStatic = _operation.IsStatic,
                Visibility = _operation.Visibility,
            };

            Func<object, ICommand> updateCmdProvider = obj => {
                var old = new PropObj() {
                    Name = _operation.Name,
                    Stereotype = _operation.Stereotype,
                    Type = _operation.TypeName,
                    Parameters = _operation.Parameters,
                    IsAbstract = _operation.IsAbstract,
                    IsStatic = _operation.IsStatic,
                    Visibility = _operation.Visibility,
                };
                return new DelegatingCommand(
                    () => {
                        _operation.Name = propObj.Name;
                        _operation.Stereotype = propObj.Stereotype;
                        _operation.TypeName = propObj.Type;
                        _operation.Parameters = propObj.Parameters;
                        _operation.IsAbstract = propObj.IsAbstract;
                        _operation.IsStatic = propObj.IsStatic;
                        _operation.Visibility = propObj.Visibility;
                    },
                    () => {
                        _operation.Name = old.Name;
                        _operation.Stereotype = old.Stereotype;
                        _operation.TypeName = old.Type;
                        _operation.Parameters = old.Parameters;
                        _operation.IsAbstract = old.IsAbstract;
                        _operation.IsStatic = old.IsStatic;
                        _operation.Visibility = old.Visibility;
                    }
                );
            };
            var propPage = new PropertyDetailSettingsPage(propObj, updateCmdProvider);
            detailForm.RegisterPage("モデル", propPage);
        }

        [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = true)]
        private class PropObj {
            public string Name { get; set; }
            public string Stereotype { get; set; }
            public string Type { get; set; }
            public string Parameters { get; set; }
            public bool IsAbstract { get; set; }
            public bool IsStatic { get; set; }
            public UmlVisibilityKind Visibility { get; set; }
            
        }
    }
}
