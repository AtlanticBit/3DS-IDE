#region Using Directives

using System;

#endregion Using Directives


namespace ScintillaNET
{
    /// <summary>
    ///     Controls how line wrapping occurs
    /// </summary>
    public enum WrapMode
    {
        /// <summary>
        ///     No Wrapping
        /// </summary>
        None = 0,

        /// <summary>
        ///     Wraps at the nearest word
        /// </summary>
        Word = 1,

        /// <summary>
        ///     Wraps at the last character
        /// </summary>
        Char = 2,
    }
}
