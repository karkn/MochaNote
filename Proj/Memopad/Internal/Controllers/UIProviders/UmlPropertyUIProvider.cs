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
    internal class UmlPropertyUIProvider: AbstractUmlFeatureUIProvider {
        // ========================================
        // field
        // ========================================
        private UmlProperty _property;

        // ========================================
        // constructor
        // ========================================
        public UmlPropertyUIProvider(UmlPropertyController controller, UmlProperty property): base(controller) {
            _property = property;
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
                Name = _property.Name,
                Stereotype = _property.Stereotype,
                Type = _property.TypeName,
                IsStatic = _property.IsStatic,
                IsDerived = _property.IsDerived,
                IsReadOnly = _property.IsReadOnly,
                IsUnique = _property.IsUnique,
                IsOrdered = _property.IsOrdered,
                IsUpperUnlimited = _property.IsUpperUnlimited,
                Upper = _property.Upper,
                Lower = _property.Lower,
                Default = _property.Default,
                Visibility = _property.Visibility,
            };

            Func<object, ICommand> updateCmdProvider = obj => {
                var old = new PropObj() {
                    Name = _property.Name,
                    Stereotype = _property.Stereotype,
                    Type = _property.TypeName,
                    IsStatic = _property.IsStatic,
                    IsDerived = _property.IsDerived,
                    IsReadOnly = _property.IsReadOnly,
                    IsUnique = _property.IsUnique,
                    IsOrdered = _property.IsOrdered,
                    IsUpperUnlimited = _property.IsUpperUnlimited,
                    Upper = _property.Upper,
                    Lower = _property.Lower,
                    Default = _property.Default,
                    Visibility = _property.Visibility,
                };
                return new DelegatingCommand(
                    () => {
                        _property.Name = propObj.Name;
                        _property.Stereotype = propObj.Stereotype;
                        _property.TypeName = propObj.Type;
                        _property.IsStatic = propObj.IsStatic;
                        _property.IsDerived = propObj.IsDerived;
                        _property.IsReadOnly = propObj.IsReadOnly;
                        _property.IsUnique= propObj.IsUnique;
                        _property.IsOrdered = propObj.IsOrdered;
                        _property.IsUpperUnlimited = propObj.IsUpperUnlimited;
                        _property.Upper = propObj.Upper;
                        _property.Lower = propObj.Lower;
                        _property.Default = propObj.Default;
                        _property.Visibility = propObj.Visibility;
                    },
                    () => {
                        _property.Name = old.Name;
                        _property.Stereotype = old.Stereotype;
                        _property.TypeName = old.Type;
                        _property.IsStatic = old.IsStatic;
                        _property.IsDerived = old.IsDerived;
                        _property.IsReadOnly = old.IsReadOnly;
                        _property.IsUnique= old.IsUnique;
                        _property.IsOrdered = old.IsOrdered;
                        _property.IsUpperUnlimited = old.IsUpperUnlimited;
                        _property.Upper = old.Upper;
                        _property.Lower = old.Lower;
                        _property.Default = old.Default;
                        _property.Visibility = old.Visibility;
                    }
                );
            };
            var propPage = new PropertyDetailSettingsPage(propObj, updateCmdProvider);
            detailForm.RegisterPage("モデル", propPage);
        }

        // ========================================
        // class
        // ========================================
        [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = true)]
        private class PropObj {
            public string Name { get; set; }
            public string Stereotype { get; set; }
            public string Type { get; set; }
            public bool IsStatic { get; set; }
            public bool IsDerived { get; set; }
            public bool IsReadOnly { get; set; }
            public bool IsUnique { get; set; }
            public bool IsOrdered { get; set; }
            public bool IsUpperUnlimited { get; set; }
            public int Upper { get; set; }
            public int Lower { get; set; }
            public string Default { get; set; }
            public UmlVisibilityKind Visibility { get; set; }
            
        }
    }
}
