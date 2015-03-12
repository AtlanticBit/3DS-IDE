#region Using Directives

using System;

#endregion Using Directives


namespace ScintillaNET
{
    /// <summary>
    ///     How wrap visual indicators are displayed
    /// </summary>
    [Flags]
    public enum WrapVisualFlag
    {
        /// <summary>
        ///     No wrap indicators are displayed
        /// </summary>
        None = 0x0000,

        /// <summary>
        ///     Wrap indicators are displayed at the _end of the line
        /// </summary>
        End = 0x0001,

        /// <summary>
        ///     Wrap indicators are displayed at the _start of the line
        /// </summary>
        Start = 0x0002,
    }
}
