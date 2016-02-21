/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;

namespace Mkamo.Common.Csv
{
	/// <summary>
	/// Specifies the action to take when a parsing error has occured.
	/// </summary>
	public enum ParseErrorAction
	{
		/// <summary>
		/// Raises the <see cref="M:CsvReader.ParseError"/> event.
		/// </summary>
		RaiseEvent = 0,

		/// <summary>
		/// Tries to advance to next line.
		/// </summary>
		AdvanceToNextLine = 1,

		/// <summary>
		/// Throws an exception.
		/// </summary>
		ThrowException = 2,
	}
}
