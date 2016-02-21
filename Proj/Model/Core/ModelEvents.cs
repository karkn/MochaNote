/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Model.Memo;
using Mkamo.Common.Event;

namespace Mkamo.Model.Core {
    public class MemoTagEventArgs: EventArgs {
        // ========================================
        // field
        // ========================================
        private MemoTag _tag;

        // ========================================
        // constructor
        // ========================================
        public MemoTagEventArgs(MemoTag tag) {
            _tag = tag;
        }

        // ========================================
        // property
        // ========================================
        public MemoTag Tag {
            get { return _tag; }
        }
    }

    public class MemoTagChangedEventArgs: MemoTagEventArgs {
        // ========================================
        // field
        // ========================================
        private DetailedPropertyChangedEventArgs _cause;

        // ========================================
        // constructor
        // ========================================
        public MemoTagChangedEventArgs(MemoTag tag, DetailedPropertyChangedEventArgs cause)
            : base(tag)
        {
            _cause = cause;
        }

        // ========================================
        // property
        // ========================================
        public DetailedPropertyChangedEventArgs Cause {
            get { return _cause; }
        }
    }

    public class MemoFolderEventArgs: EventArgs {
        // ========================================
        // field
        // ========================================
        private MemoFolder _folder;

        // ========================================
        // constructor
        // ========================================
        public MemoFolderEventArgs(MemoFolder folder) {
            _folder = folder;
        }

        // ========================================
        // property
        // ========================================
        public MemoFolder Folder {
            get { return _folder; }
        }
    }

    public class MemoFolderChangedEventArgs: MemoFolderEventArgs {
        // ========================================
        // field
        // ========================================
        private DetailedPropertyChangedEventArgs _cause;

        // ========================================
        // constructor
        // ========================================
        public MemoFolderChangedEventArgs(MemoFolder folder, DetailedPropertyChangedEventArgs cause)
            : base(folder)
        {
            _cause = cause;
        }

        // ========================================
        // property
        // ========================================
        public DetailedPropertyChangedEventArgs Cause {
            get { return _cause; }
        }
    }

    public class MemoSmartFolderEventArgs: EventArgs {
        // ========================================
        // field
        // ========================================
        private MemoSmartFolder _smartFolder;

        // ========================================
        // constructor
        // ========================================
        public MemoSmartFolderEventArgs(MemoSmartFolder smartFolder) {
            _smartFolder = smartFolder;
        }

        // ========================================
        // property
        // ========================================
        public MemoSmartFolder SmartFolder {
            get { return _smartFolder; }
        }
    }

    public class MemoSmartFolderChangedEventArgs: MemoSmartFolderEventArgs {
        // ========================================
        // field
        // ========================================
        private DetailedPropertyChangedEventArgs _cause;

        // ========================================
        // constructor
        // ========================================
        public MemoSmartFolderChangedEventArgs(MemoSmartFolder smartFolder, DetailedPropertyChangedEventArgs cause)
            : base(smartFolder)
        {
            _cause = cause;
        }

        // ========================================
        // property
        // ========================================
        public DetailedPropertyChangedEventArgs Cause {
            get { return _cause; }
        }
    }

    public class MemoSmartFilterEventArgs: EventArgs {
        // ========================================
        // field
        // ========================================
        private MemoSmartFilter _smartFilter;

        // ========================================
        // constructor
        // ========================================
        public MemoSmartFilterEventArgs(MemoSmartFilter smartFilter) {
            _smartFilter = smartFilter;
        }

        // ========================================
        // property
        // ========================================
        public MemoSmartFilter SmartFilter {
            get { return _smartFilter; }
        }
    }

}
