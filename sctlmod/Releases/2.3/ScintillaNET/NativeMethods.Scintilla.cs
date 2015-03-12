#region Using Directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion Using Directives


namespace ScintillaNet
{
    partial class NativeMethods
    {
        #region Callbacks

        public delegate IntPtr Scintilla_DirectFunction(
            IntPtr sci,
            int iMessage,
            IntPtr wParam,
            IntPtr lParam);

        #endregion Callbacks
    }
}
