/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Mkamo.Common.Text.Internal;

namespace Mkamo.Common.Text {
    public static class TextUtil {
        public static ITextBuffer CreateTextBuffer() {
            return new TextBuffer(null);
        }

        public static ITextBuffer CreateTextBuffer(string text) {
            return new TextBuffer(text);
        }

        public static ITextBufferReferer CreateTextBufferReferer() {
            return CreateTextBufferReferer(CreateTextBuffer());
        }

        public static ITextBufferReferer CreateTextBufferReferer(string text) {
            return CreateTextBufferReferer(CreateTextBuffer(text));
        }

        public static ITextBufferReferer CreateTextBufferReferer(ITextBuffer target) {
            return new TextBufferReferer(target as TextBuffer);
        }


    }
}
