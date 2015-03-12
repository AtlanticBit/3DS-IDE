#region Using Directives

using System;
using System.Runtime.InteropServices;

#endregion Using Directives


namespace ScintillaPad
{
	internal static class NativeMethods
	{
		#region Constants

		private const string
			DLL_NAME_USER32 = "user32.dll";

		public const int
			WM_KEYDOWN = 0x0100,
			WM_CHAR = 0x0102,
			WM_SYSCOMMAND = 0x0112;

		public const int
			SC_MINIMIZE = 0xF020,
			SC_MAXIMIZE = 0xF030;

		#endregion Constants


		#region Functions

		[DllImport(DLL_NAME_USER32, CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool PostMessage(
			IntPtr hWnd,
			int Msg,
			IntPtr wParam,
			IntPtr lParam);

		[DllImport(DLL_NAME_USER32, CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr SendMessage(
			IntPtr hWnd,
			int Msg,
			IntPtr wParam,
			IntPtr lParam);

		[DllImport(DLL_NAME_USER32, CharSet = CharSet.Auto)]
		public static extern short VkKeyScan(
			char ch);

		#endregion Functions
	}
}
