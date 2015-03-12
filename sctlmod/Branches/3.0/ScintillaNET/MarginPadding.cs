#region Using Directives

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Drawing;

#endregion Using Directives


namespace ScintillaNet
{
	/// <summary>
	/// Represents padding or margin information associated with a user interface (UI) element.
	/// </summary>
	[Serializable, StructLayout(LayoutKind.Sequential), TypeConverter(typeof(MarginPaddingConverter))]
	public struct MarginPadding
	{
		#region Fields

		/// <summary>
		/// Provides a <see cref="MarginPadding"/> object with no padding.
		/// </summary>
		public static MarginPadding Empty = new MarginPadding(0);

		private bool _both;
		private int _left;
		private int _right;

		#endregion Fields


		#region Methods

		/// <summary>
		/// Computes the sum of the two specified <see cref="MarginPadding"/> values.
		/// </summary>
		/// <param name="p1">A <see cref="MarginPadding"/>.</param>
		/// <param name="p2">A <see cref="MarginPadding"/>.</param>
		/// <returns>A <see cref="MarginPadding"/> that contains the sum of the two specified <see cref="MarginPadding"/> values.</returns>
		public static MarginPadding Add(MarginPadding p1, MarginPadding p2)
		{
			return (p1 + p2);
		}


		/// <summary>
		/// Overridden. Determines whether the value of the specified object is equivalent to the current <see cref="MarginPadding"/>.
		/// </summary>
		/// <param name="obj">The object to compare to the current <see cref="MarginPadding"/>.</param>
		/// <returns><c>true</c> if the <see cref="MarginPadding"/> objects are equivalent; otherwise, <c>false</c>.</returns>
		public override bool Equals(object obj)
		{
			if (obj is MarginPadding)
				return ((MarginPadding)obj) == this;

			return false;
		}


		/// <summary>
		/// Overridden. Generates a hash code for the current <see cref="MarginPadding"/>.
		/// </summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return Left ^ Right;
		}


		private void ResetBoth()
		{
			Both = 0;
		}


		private void ResetLeft()
		{
			Left = 0;
		}


		private void ResetRight()
		{
			Right = 0;
		}


		[Conditional("DEBUG")]
		private void SanityCheck()
		{
			if (_both)
			{
				Debug.Assert(ShouldSerializeBoth(), "_both is true, but ShouldSerializeBoth() is false.");
				Debug.Assert(Both == Left && Left == Right, "_both is true, but Both/Left/Right are inconsistent.");
			}
			else
			{
				Debug.Assert(Both == -1, "_both is false, but Both != -1.");
				Debug.Assert(!ShouldSerializeBoth(), "ShouldSerializeBoth() should not be true when _both flag is not set.");
			}
		}


		internal bool ShouldSerializeBoth()
		{
			return _both;
		}


		/// <summary>
		/// Subtracts one specified <see cref="MarginPadding"/> value from another.
		/// </summary>
		/// <param name="p1">A <see cref="MarginPadding"/>.</param>
		/// <param name="p2">A <see cref="MarginPadding"/>.</param>
		/// <returns>
		/// A <see cref="MarginPadding"/> that contains the result of the subtraction of one 
		/// specified <see cref="MarginPadding"/> value from another.
		/// </returns>
		public static MarginPadding Subtract(MarginPadding p1, MarginPadding p2)
		{
			return (p1 - p2);
		}


		/// <summary>
		/// Overridden. Returns a string that represents the current <see cref="MarginPadding"/>.
		/// </summary>
		/// <returns>A <see cref="String"/> that represents the current <see cref="MarginPadding"/>.</returns>
		public override string ToString()
		{
			return "{Left=" + Left.ToString(CultureInfo.CurrentCulture) + ",Right=" + Right.ToString(CultureInfo.CurrentCulture) + "}";
		}

		#endregion Methods


		#region Properties

		/// <summary>
		/// Gets or sets the padding value for both margins.
		/// </summary>
		/// <value>The padding, in pixels, for both margins if the same; otherwise, -1.</value>
		[RefreshProperties(RefreshProperties.All)]
		public int Both
		{
			get
			{
				return _both ? _left : -1;
			}
			set
			{
				if (!_both || _left != value)
				{
					_both = true;
					_left = _right = value;
				}
				SanityCheck();
			}
		}


		/// <summary>
		/// Gets the combined padding for the right and left margins.
		/// </summary>
		/// <value>
		/// Gets the sum, in pixels, of the <see cref="Left"/> and <see cref="Right"/> margin padding values.
		/// </value>
		[Browsable(false)]
		public int Horizontal
		{
			get
			{
				return (Left + Right);
			}
		}


		/// <summary>
		/// Gets or sets the padding value for the left margin.
		/// </summary>
		/// <value>The padding, in pixels, for the left margin.</value>
		[RefreshProperties(RefreshProperties.All)]
		public int Left
		{
			get
			{
				return _left;
			}
			set
			{
				if (_both || _left != value)
				{
					_both = false;
					_left = value;
				}
				SanityCheck();
			}
		}


		/// <summary>
		/// Gets or sets the padding value for the right margin.
		/// </summary>
		/// <value>The padding, in pixels, for the right margin.</value>
		[RefreshProperties(RefreshProperties.All)]
		public int Right
		{
			get
			{
				return _right;
			}
			set
			{
				if (_both || _right != value)
				{
					_both = false;
					_right = value;
				}
				SanityCheck();
			}
		}

		#endregion Properties


		#region Operators

		/// <summary>
		/// Performs vector addition on the two specified <see cref="MarginPadding"/> objects, resulting in a new <see cref="MarginPadding"/>.
		/// </summary>
		/// <param name="p1">The first <see cref="MarginPadding"/> to add.</param>
		/// <param name="p2">The second <see cref="MarginPadding"/> to add.</param>
		/// <returns>A new <see cref="MarginPadding"/> that results from adding <paramref name="p1"/> and <paramref name="p2"/>.</returns>
		public static MarginPadding operator +(MarginPadding p1, MarginPadding p2)
		{
			return new MarginPadding(p1.Left + p2.Left, p1.Right + p2.Right);
		}


		/// <summary>
		/// Tests whether two specified <see cref="MarginPadding"/> objects are equivalent.
		/// </summary>
		/// <param name="p1">A <see cref="MarginPadding"/> to test.</param>
		/// <param name="p2">A <see cref="MarginPadding"/> to test.</param>
		/// <returns><c>true</c> if the two <see cref="MarginPadding"/> objects are equal; otherwise, <c>false</c>.</returns>
		public static bool operator ==(MarginPadding p1, MarginPadding p2)
		{
			return (p1.Left == p2.Left && p1.Right == p2.Right);
		}


		/// <summary>
		/// Tests whether two specified <see cref="MarginPadding"/> objects are not equivalent.
		/// </summary>
		/// <param name="p1">A <see cref="MarginPadding"/> to test.</param>
		/// <param name="p2">A <see cref="MarginPadding"/> to test.</param>
		/// <returns><c>true</c> if the two <see cref="MarginPadding"/> objects are different; otherwise, <c>false</c>.</returns>
		public static bool operator !=(MarginPadding p1, MarginPadding p2)
		{
			return !(p1 == p2);
		}


		/// <summary>
		/// Performs vector subtraction on the two specified <see cref="MarginPadding"/> objects, resulting in a new <see cref="MarginPadding"/>.
		/// </summary>
		/// <param name="p1">The <see cref="MarginPadding"/> to subtract from (the minuend).</param>
		/// <param name="p2">The <see cref="MarginPadding"/> to subtract from (the subtrahend).</param>
		/// <returns>The <see cref="MarginPadding"/> result of subtracting p2 from p1</returns>
		public static MarginPadding operator -(MarginPadding p1, MarginPadding p2)
		{
			return new MarginPadding(p1.Left - p2.Left, p1.Right - p2.Right);
		}

		#endregion Operators


		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="MarginPadding"/> class using the supplied padding size for both margins.
		/// </summary>
		/// <param name="both">The number of pixels to be used for padding for both margins.</param>
		public MarginPadding(int both)
		{
			_both = true;
			_left = _right = both;
			SanityCheck();
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="MarginPadding"/> class using a separate padding size for each margin.
		/// </summary>
		/// <param name="left">The padding size, in pixels, for the left margin.</param>
		/// <param name="right">The padding size, in pixels, for the right margin.</param>
		public MarginPadding(int left, int right)
		{
			_left = left;
			_right = right;
			_both = (left == right);
			SanityCheck();
		}

		#endregion Constructors
	}
}
