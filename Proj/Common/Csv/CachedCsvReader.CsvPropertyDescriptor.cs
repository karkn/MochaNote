/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Mkamo.Common.Csv
{
	public partial class CachedCsvReader
		: CsvReader
	{
		/// <summary>
		/// Represents a CSV field property descriptor.
		/// </summary>
		private class CsvPropertyDescriptor
			: PropertyDescriptor
		{
			#region Fields

			/// <summary>
			/// Contains the field index.
			/// </summary>
			private int _index;

			#endregion

			#region Constructors

			/// <summary>
			/// Initializes a new instance of the CsvPropertyDescriptor class.
			/// </summary>
			/// <param name="fieldName">The field name.</param>
			/// <param name="index">The field index.</param>
			public CsvPropertyDescriptor(string fieldName, int index)
				: base(fieldName, null)
			{
				_index = index;
			}

			#endregion

			#region Properties

			/// <summary>
			/// Gets the field index.
			/// </summary>
			/// <value>The field index.</value>
			public int Index
			{
				get { return _index; }
			}

			#endregion

			#region Overrides

			public override bool CanResetValue(object component)
			{
				return false;
			}

			public override object GetValue(object component)
			{
				return ((string[]) component)[_index];
			}

			public override void ResetValue(object component)
			{
			}

			public override void SetValue(object component, object value)
			{
			}

			public override bool ShouldSerializeValue(object component)
			{
				return false;
			}

			public override Type ComponentType
			{
				get
				{
					return typeof(CachedCsvReader);
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override Type PropertyType
			{
				get
				{
					return typeof(string);
				}
			}

			#endregion
		}
	}
}
