#region Using Directives

using System;

#endregion Using Directives


namespace ScintillaNet
{
	/// <summary>
	/// Specifies the text wrapping modes that can be applied to a <see cref="Scintilla"/> control.
	/// </summary>
	public enum WrappingMode : uint
	{
		/// <summary>
		/// Text wrapping is disabled.
		/// </summary>
		None = Constants.SC_WRAP_NONE,

		/// <summary>
		/// Text wraps on word boundaries.
		/// </summary>
		Word = Constants.SC_WRAP_WORD,

		/// <summary>
		/// Text wraps between characters.
		/// </summary>
		Char = Constants.SC_WRAP_CHAR,
	}

}
