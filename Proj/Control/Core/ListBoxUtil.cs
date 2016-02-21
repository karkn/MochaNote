/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Mkamo.Control.Core {
    public static class ListBoxUtil {
        public static void EnsureVisible(this ListBox listBox, int index) {
            if (listBox == null) {
                return;
            }

            var visibleCount = listBox.Height / listBox.ItemHeight;
            var top = listBox.TopIndex;
            if (index < top) {
                listBox.TopIndex = index;
            } else if (index >= top && index < top + visibleCount) {
                // do nothing
            } else {
                listBox.TopIndex = index - visibleCount + 1;
            }
        }

        public static void SelectPreviousItem(this ListBox listBox) {
            if (listBox.SelectedIndex == -1) {
                if (listBox.Items.Count > 0) {
                    EnsureVisible(listBox, 0);
                    listBox.SelectedIndex = 0;
                }
            } else {
                var index = listBox.SelectedIndices[0];
                if (index > 0) {
                    EnsureVisible(listBox, index - 1);
                    listBox.ClearSelected();
                    listBox.SelectedIndex = index - 1;
                }
            }
        }

        public static void SelectNextItem(this ListBox listBox) {
            if (listBox.SelectedIndex == -1) {
                if (listBox.Items.Count > 0) {
                    EnsureVisible(listBox, 0);
                    listBox.SelectedIndex = 0;
                    
                }
            } else {
                var index = listBox.SelectedIndices[listBox.SelectedIndices.Count - 1];
                if (index < listBox.Items.Count - 1) {
                    EnsureVisible(listBox, index + 1);
                    listBox.ClearSelected();
                    listBox.SelectedIndex = index + 1;
                }
            }
        }

    }
}
