/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mkamo.Common.Command;
using Mkamo.Common.Diagnostics;
using Mkamo.Common.Forms.Themes;

namespace Mkamo.Common.Forms.DetailSettings {
    [ToolboxItem(false)]
    public partial class PropertyDetailSettingsPage: UserControl, IDetailSettingsPage {
        // ========================================
        // field
        // ========================================
        private bool _isModified;
        private Func<object, ICommand> _updateCommandProvider;
        private object _targetObject;

        private ITheme _theme;

        // ========================================
        // constructor
        // ========================================
        public PropertyDetailSettingsPage(object targetObj, Func<object, ICommand> updateCommandProvider) {
            Contract.Requires(updateCommandProvider != null);

            _updateCommandProvider = updateCommandProvider;
            _isModified = false;

            InitializeComponent();

            _targetObject = targetObj;

            _propertyGrid.SelectedObject = _targetObject;
            _propertyGrid.ExpandAllGridItems();
            _propertyGrid.PropertyValueChanged += HandlePropertyGridValueChanged;
        }

        // ========================================
        // property
        // ========================================
        public Control PageControl {
            get { return this; }
        }

        public bool NeedBorder {
            get { return false; }
        }

        public bool IsModified {
            get { return _isModified; }
            set { _isModified = value; }
        }

        public PropertyGrid PropertyGrid {
            get { return _propertyGrid; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ITheme Theme {
            get { return _theme; }
            set {
                if (value == _theme) {
                    return;
                }
                _theme = value;

                var inputFont = value.InputFont;
                _propertyGrid.Font = inputFont;
            }
        }

        // ========================================
        // method
        // ========================================
        public ICommand GetUpdateCommand() {
            return _updateCommandProvider(_targetObject);
        }

        private void HandlePropertyGridValueChanged(object sender, EventArgs e) {
            _isModified = true;
        }

    }
}
