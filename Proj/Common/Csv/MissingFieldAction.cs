/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;

namespace Mkamo.Common.Csv
{
	/// <summary>
	/// Specifies the action to take when a field is missing.
	/// </summary>
	public enum MissingFieldAction
	{
		/// <summary>
		/// Treat as a parsing error.
		/// </summary>
		ParseError = 0,

		/// <summary>
		/// Replaces by an empty value.
		/// </summary>
		ReplaceByEmpty = 1,

		/// <summary>
		/// Replaces by a null value (<see langword="null"/>).
		/// </summary>
		ReplaceByNull = 2,
	}
}
