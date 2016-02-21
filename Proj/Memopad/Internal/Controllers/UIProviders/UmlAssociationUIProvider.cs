/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Requests;
using Mkamo.Common.Command;
using Mkamo.Editor.Core;
using System.Windows.Forms;
using Mkamo.Editor.Controllers;
using System.Diagnostics;
using System.IO;
using Mkamo.Common.Forms.DetailSettings;
using Mkamo.Memopad.Internal.Controls;
using Mkamo.Editor.Forms;
using Mkamo.Model.Uml;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace Mkamo.Memopad.Internal.Controllers.UIProviders {
    internal class UmlAssociationUIProvider: AbstractUIProvider {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // ========================================
        // field
        // ========================================
        private UmlAssociationController _owner;

        // ========================================
        // constructor
        // ========================================
        public UmlAssociationUIProvider(UmlAssociationController owner): base(true) {
            _owner = owner;
        }


        // ========================================
        // method
        // ========================================
        public override ContextMenuStrip GetContextMenu(MouseEventArgs e) {
            return _ContextMenu;
        }
    
        public override void ConfigureDetailForm(DetailSettingsForm detailForm) {
            var fig = _owner.Figure;

            var assoc = _owner.Model;
            var source = assoc.SourceMemberEnd;
            var target = assoc.TargetMemberEnd;

            /// property detail page
            var propObj = new PropObj() {
                IsSourceNavigable = assoc.IsSourceNavigable,
                IsTargetNavigable = assoc.IsTargetNavigable,
                SourceMemberEnd = new MemberEnd() {
                    Name = source.Name,
                    Stereotype = source.Stereotype,
                    Aggregation = source.Aggregation,
                    IsUpperUnlimited = source.IsUpperUnlimited,
                    Upper = source.Upper,
                    Lower = source.Lower,
                    Visibility = source.Visibility,
                },
                TargetMemberEnd = new MemberEnd() {
                    Name = target.Name,
                    Stereotype = target.Stereotype,
                    Aggregation = target.Aggregation,
                    IsUpperUnlimited = target.IsUpperUnlimited,
                    Upper = target.Upper,
                    Lower = target.Lower,
                    Visibility = target.Visibility,
                },
            };

            Func<object, ICommand> updateCmdProvider = obj => {
                var old = new PropObj() {
                    IsSourceNavigable = assoc.IsSourceNavigable,
                    IsTargetNavigable = assoc.IsTargetNavigable,
                    SourceMemberEnd = new MemberEnd() {
                        Name = source.Name,
                        Stereotype = source.Stereotype,
                        Aggregation = source.Aggregation,
                        IsUpperUnlimited = source.IsUpperUnlimited,
                        Upper = source.Upper,
                        Lower = source.Lower,
                        Visibility = source.Visibility,
                    },
                    TargetMemberEnd = new MemberEnd() {
                        Name = target.Name,
                        Stereotype = target.Stereotype,
                        Aggregation = target.Aggregation,
                        IsUpperUnlimited = target.IsUpperUnlimited,
                        Upper = target.Upper,
                        Lower = target.Lower,
                        Visibility = target.Visibility,
                    },
                };
                return new DelegatingCommand(
                    () => {
                        assoc.IsSourceNavigable = propObj.IsSourceNavigable;
                        assoc.IsTargetNavigable = propObj.IsTargetNavigable;

                        source.Name = propObj.SourceMemberEnd.Name;
                        source.Stereotype = propObj.SourceMemberEnd.Stereotype;
                        source.Aggregation = propObj.SourceMemberEnd.Aggregation;
                        source.IsUpperUnlimited = propObj.SourceMemberEnd.IsUpperUnlimited;
                        source.Upper = propObj.SourceMemberEnd.Upper;
                        source.Lower = propObj.SourceMemberEnd.Lower;
                        source.Visibility = propObj.SourceMemberEnd.Visibility;
                             
                        target.Name = propObj.TargetMemberEnd.Name;
                        target.Stereotype = propObj.TargetMemberEnd.Stereotype;
                        target.Aggregation = propObj.TargetMemberEnd.Aggregation;
                        target.IsUpperUnlimited = propObj.TargetMemberEnd.IsUpperUnlimited;
                        target.Upper = propObj.TargetMemberEnd.Upper;
                        target.Lower = propObj.TargetMemberEnd.Lower;
                        target.Visibility = propObj.TargetMemberEnd.Visibility;
                    },
                    () => {
                        assoc.IsSourceNavigable = old.IsSourceNavigable;
                        assoc.IsTargetNavigable = old.IsTargetNavigable;

                        source.Name = old.SourceMemberEnd.Name;
                        source.Stereotype = old.SourceMemberEnd.Stereotype;
                        source.Aggregation = old.SourceMemberEnd.Aggregation;
                        source.IsUpperUnlimited = old.SourceMemberEnd.IsUpperUnlimited;
                        source.Upper = old.SourceMemberEnd.Upper;
                        source.Lower = old.SourceMemberEnd.Lower;
                        source.Visibility = old.SourceMemberEnd.Visibility;
                             
                        target.Name = old.TargetMemberEnd.Name;
                        target.Stereotype = old.TargetMemberEnd.Stereotype;
                        target.Aggregation = old.TargetMemberEnd.Aggregation;
                        target.IsUpperUnlimited = old.TargetMemberEnd.IsUpperUnlimited;
                        target.Upper = old.TargetMemberEnd.Upper;
                        target.Lower = old.TargetMemberEnd.Lower;
                        target.Visibility = old.TargetMemberEnd.Visibility;
                    }
                );
            };
            var propPage = new PropertyDetailSettingsPage(propObj, updateCmdProvider);
            detailForm.RegisterPage("モデル", propPage);

            /// line detail page
            var borderPage = new EdgeLineDetailPage(new[] { _owner.Host });
            borderPage.LineColor = fig.LineColor;
            borderPage.LineWidth = fig.LineWidth;
            borderPage.LineDashStyle = fig.LineDashStyle;
            borderPage.IsModified = false;
            detailForm.RegisterPage("線", borderPage);

        }

        // ========================================
        // class
        // ========================================
        [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = true)]
        private class PropObj {
            public bool IsSourceNavigable  { get; set; }
            public bool IsTargetNavigable  { get; set; }

            [ReadOnly(true)]
            public MemberEnd SourceMemberEnd { get; set; }
            [ReadOnly(true)]
            public MemberEnd TargetMemberEnd { get; set; }
        }

        [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = true)]
        [TypeConverter(typeof(MemberEndTypeConverter))]
        private class MemberEnd {
            public string Name { get; set; }
            public string Stereotype { get; set; }
            public UmlAggregationKind Aggregation { get; set; }
            //public bool IsDerived { get; set; }
            //public bool IsUnique { get; set; }
            //public bool IsOrdered { get; set; }
            public bool IsUpperUnlimited { get; set; }
            public int Upper { get; set; }
            public int Lower { get; set; }
            public UmlVisibilityKind Visibility { get; set; }
        }

        private class MemberEndTypeConverter: ExpandableObjectConverter {
            public override object ConvertTo(
                ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType
            ) {
                if (destinationType == typeof(string)) {
                    return "";
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }
}
