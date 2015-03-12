#region Using Directives

using System;

#endregion Using Directives


namespace ScintillaNET
{
    [Flags]
    public enum WrapVisualLocation
    {
        Default = 0x0000,
        EndByText = 0x0001,
        StartByText = 0x0002,
    }
}
