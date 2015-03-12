#region Using Directives

using System;

#endregion Using Directives


namespace ScintillaNet
{
	/// <summary>
	/// Specifies the line layout caching strategy used by a <see cref="Scintilla"/> control.
	/// </summary>
	public enum LayoutCacheMode : uint
	{
		/// <summary>
		/// No line layout data is cached.
		/// </summary>
		None = Constants.SC_CACHE_NONE,

		/// <summary>
		/// Line layout data of the current caret line is cached.
		/// </summary>
		Caret = Constants.SC_CACHE_CARET,

		/// <summary>
		/// Line layout data for all visible lines and the current caret line are cached.
		/// </summary>
		Page = Constants.SC_CACHE_PAGE,

		/// <summary>
		/// Line layout data for the entire document is cached.
		/// </summary>
		Document = Constants.SC_CACHE_DOCUMENT
	}
}
