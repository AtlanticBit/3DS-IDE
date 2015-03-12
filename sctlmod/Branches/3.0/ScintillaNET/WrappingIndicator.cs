#region Using Directives

using System;

#endregion Using Directives


namespace ScintillaNet
{
	/// <summary>
	/// Specifies the text wrapping indicators that can be applied to a <see cref="Scintilla"/> control.
	/// </summary>
	/// <dev>
	/// This enum is a combination of the SC_WRAPVISUALFLAG_* and SC_WRAPVISUALFLAGLOC_* contants.
	/// They are combined as (SC_WRAPVISUALFLAG | (SC_WRAPVISUALFLAGLOC << 16).
	/// </dev>
	public enum WrappingIndicator : uint
	{
		/// <summary>
		/// No wrapping indicators are displayed.
		/// </summary>
		None = (Constants.SC_WRAPVISUALFLAG_NONE | (Constants.SC_WRAPVISUALFLAGLOC_DEFAULT << 16)), // ;)

		/// <summary>
		/// Indicators are displayed at the end of a wrapped line of text adjacent to the border.
		/// </summary>
		EndBorder = (Constants.SC_WRAPVISUALFLAG_END | (Constants.SC_WRAPVISUALFLAGLOC_DEFAULT << 16)),

		/// <summary>
		/// Indicators are displayed at the end of a wrapped line of text adjacent to the text.
		/// </summary>
		EndText = (Constants.SC_WRAPVISUALFLAG_END | (Constants.SC_WRAPVISUALFLAGLOC_END_BY_TEXT << 16)),

		/// <summary>
		/// Indicators are displayed at the start of wrapped lines of text adjacent to the border.
		/// </summary>
		StartBorder = (Constants.SC_WRAPVISUALFLAG_START | (Constants.SC_WRAPVISUALFLAGLOC_DEFAULT << 16)),

		/// <summary>
		/// Indicators are displayed at the start of wrapped lines of text adjacent to the text.
		/// </summary>
		StartText = (Constants.SC_WRAPVISUALFLAG_START | (Constants.SC_WRAPVISUALFLAGLOC_START_BY_TEXT << 16))
	}
}
