/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;

namespace Mkamo.Common.Csv
{
	[Flags]
	public enum ValueTrimmingOptions
	{
		None = 0,
		UnquotedOnly = 1,
		QuotedOnly = 2,
		All = UnquotedOnly | QuotedOnly
	}
}
