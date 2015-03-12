#region Using Directives

using System;

#endregion Using Directives


namespace ScintillaNet
{
	/// <summary>
	/// Represents a range of items identified by a starting and ending index.
	/// </summary>
	public struct Range
	{
		#region Fields

		/// <summary>
		/// Represents a <see cref="Range"/> that has <see cref="StartIndex"/> and
		/// <see cref="EndIndex"/> values set to zero.
		/// </summary>
		public static readonly Range Empty = new Range();

		private int _startIndex;
		private int _endIndex;

		#endregion Fields


		#region Methods

		/// <summary>
		/// Overridden. See <see cref="Object.Equals"/>.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (!(obj is Range))
				return false;

			Range r = (Range)obj;
			return r._startIndex == _startIndex && r._endIndex == _endIndex;
		}


		/// <summary>
		/// Overridden. See <see cref="Object.GetHashCode"/>.
		/// </summary>
		public override int GetHashCode()
		{
			return _startIndex ^ _endIndex;
		}

		#endregion Methods


		#region Properties

		/// <summary>
		/// Gets or sets the end index of this <see cref="Range"/>.
		/// </summary>
		/// <value>The end index of this <see cref="Range"/>.</value>
		public int EndIndex
		{
			get
			{
				return _endIndex;
			}
			set
			{
				_endIndex = value;
			}
		}


		/// <summary>
		/// Gets a value indicating whether this <see cref="Range"/> is empty.
		/// </summary>
		/// <value>
		/// <c>true</c> if the <see cref="StartIndex"/> and <see cref="EndIndex"/>
		/// are zero; otherwise, <c>false</c>.
		/// </value>
		public bool IsEmpty
		{
			get
			{
				return _startIndex == 0 && _endIndex == 0;
			}
		}


		/// <summary>
		/// Gets the total distance between <see cref="StartIndex"/> and <see cref="EndIndex"/>.
		/// </summary>
		/// <value>The total length of the range.</value>
		public int Length
		{
			get
			{
				return Math.Abs(_endIndex - _startIndex);
			}
		}


		/// <summary>
		/// Gets or sets the starting index of this <see cref="Range"/>.
		/// </summary>
		/// <value>The start index of this <see cref="Range"/>.</value>
		public int StartIndex
		{
			get
			{
				return _startIndex;
			}
			set
			{
				_startIndex = value;
			}
		}

		#endregion Properties


		#region Operators

		/// <summary>
		/// Compares two <see cref="Range"/> objects. The result specifies whether the values of the
		/// <see cref="StartIndex"/> and <see cref="EndIndex"/> properties of the two <see cref="Range"/>
		/// objects are unequal.
		/// </summary>
		/// <param name="left">A <see cref="Range"/> to compare.</param>
		/// <param name="right">A <see cref="Range"/> to compare.</param>
		/// <returns>
		/// <c>true</c> if the <see cref="StartIndex"/> and <see cref="EndIndex"/> values of
		/// <paramref name="left"/> and <paramref name="right"/> differ; otherwise, <c>false</c>.
		/// </returns>
		public static bool operator !=(Range left, Range right)
		{
			return !(left == right);
		}


		/// <summary>
		/// Compares two <see cref="Range"/> objects. The result specifies whether the values of the
		/// <see cref="StartIndex"/> and <see cref="EndIndex"/> properties of the two <see cref="Range"/>
		/// objects are equal.
		/// </summary>
		/// <param name="left">A <see cref="Range"/> to compare.</param>
		/// <param name="right">A <see cref="Range"/> to compare.</param>
		/// <returns><c>true</c> if the <see cref="StartIndex"/> and <see cref="EndIndex"/> values of
		/// <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, <c>false</c>.
		/// </returns>
		public static bool operator ==(Range left, Range right)
		{
			return left._startIndex == right._startIndex && left._endIndex == right._endIndex;
		}

		#endregion Operators


		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="Range"/> structure 
		/// with the specified start and end indexes.
		/// </summary>
		/// <param name="startIndex">The start index of the range.</param>
		/// <param name="endIndex">The end index of the range.</param>
		public Range(int startIndex, int endIndex)
		{
			_startIndex = startIndex;
			_endIndex = endIndex;
		}

		#endregion Constructors
	}
}
