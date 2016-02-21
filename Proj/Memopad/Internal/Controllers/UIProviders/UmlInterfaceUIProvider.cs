/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Command;
using Mkamo.Common.Forms.DetailSettings;
using Mkamo.Editor.Controllers;
using System.Windows.Forms;
using Mkamo.Editor.Core;
using Mkamo.Model.Uml;
using Mkamo.Model.Core;
using System.Drawing;
using Mkamo.Editor.Forms;
using System.Reflection;

namespace Mkamo.Memopad.Internal.Controllers.UIProviders {
    internal class UmlInterfaceUIProvider : AbstractUmlClassifierUIProvider {

        public UmlInterfaceUIProvider(UmlInterfaceController owner): base(owner, owner.Model) {
        }

        public override void ConfigureDetailForm(DetailSettingsForm detailForm) {
            /// property detail page
            var propObj = new PropObj() {
                Name = _Classifier.Name,
                Stereotype = _Classifier.Stereotype,
                Visibility = _Classifier.Visibility,
            };

            Func<object, ICommand> updateCmdProvider = obj => {
                var old = new PropObj() {
                    Name = _Classifier.Name,
                    Stereotype = _Classifier.Stereotype,
                    Visibility = _Classifier.Visibility,
                };
                return new DelegatingCommand(
                    () => {
                        _Classifier.Name = propObj.Name;
                        _Classifier.Stereotype = propObj.Stereotype;
                        _Classifier.Visibility = propObj.Visibility;
                    },
                    () => {
                        _Classifier.Name = old.Name;
                        _Classifier.Stereotype = old.Stereotype;
                        _Classifier.Visibility = old.Visibility;
                    }
                );
            };
            var propPage = new PropertyDetailSettingsPage(propObj, updateCmdProvider);
            detailForm.RegisterPage("モデル", propPage);

            base.ConfigureDetailForm(detailForm);
        }

        [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = true)]
        private class PropObj {
            public string Name { get; set; }
            public string Stereotype { get; set; }
            public UmlVisibilityKind Visibility { get; set; }
        }
    }

}
