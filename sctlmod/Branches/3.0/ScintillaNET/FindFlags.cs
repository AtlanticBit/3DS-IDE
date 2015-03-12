#region Using Directives

using System;

#endregion Using Directives


namespace ScintillaNet
{
	/// <summary>
	/// Specifies the interpretation of the search pattern when finding text in a <see cref="Scintilla" /> control.
	/// This enumeration has a <see cref="FlagsAttribute" /> attribute that allows a bitwise combination of its member values.
	/// </summary>
	[Flags]
	public enum FindFlags
	{
		/// <summary>
		/// Locate all instances of the search text.
		/// </summary>
		None = 0,

		/// <summary>
		/// Locate only instances of the search text that have the exact casing.
		/// </summary>
		MatchCase = (int)Constants.SCFIND_MATCHCASE,

		/// <summary>
		/// Locate only instances of the search text that are whole words.
		/// </summary>
		WholeWord = (int)Constants.SCFIND_WHOLEWORD,

		/// <summary>
		/// Locate only instances of the search text that start at the beginning of words.
		/// </summary>
		WordStart = (int)Constants.SCFIND_WORDSTART
	}
}
