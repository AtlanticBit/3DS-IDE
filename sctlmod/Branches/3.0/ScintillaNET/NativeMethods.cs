#region Using Directives

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

#endregion Using Directives


namespace ScintillaNet
{
	// TODO Move to SafeNativeMethods and/or UnsafeNativeMethods classes?
	internal static class NativeMethods
	{
		#region Constants

		private const string
			DLL_NAME_GDI32 = "gdi32.dll",
			DLL_NAME_KERNEL32 = "kernel32.dll",
			DLL_NAME_SHELL32 = "shell32.dll",
			DLL_NAME_USER32 = "user32.dll";

		public const int
			WM_DESTROY = 0x0002,
			WM_GETTEXT = 0x000D,
			WM_GETTEXTLENGTH = 0x000E,
			WM_PAINT = 0x000F,
			WM_SETCURSOR = 0x0020,
			WM_NOTIFY = 0x004E,
			WM_NCPAINT = 0x0085,
			WM_HSCROLL = 0x0114,
			WM_VSCROLL = 0x0115,
			WM_DROPFILES = 0x0233,
			WM_USER = 0x0400,
			WM_REFLECT = WM_USER + 0x1C00;

		public const int
			WS_EX_CLIENTEDGE = 0x00000200;
		
		public const int
			WS_BORDER = 0x00800000;

		public const int
			ES_NUMBER = 0x2000;

		public const int
			GWL_STYLE = -16,
			GWL_EXSTYLE = -20;

		public const uint
			SWP_NOSIZE = 0x0001,
			SWP_NOMOVE = 0x0002,
			SWP_NOZORDER = 0x0004,
			SWP_FRAMECHANGED = 0x0020;

		public const int
			RGN_AND = 1;

		public const int
			LOGPIXELSY = 90;

		public const int
			FW_NORMAL = 400;

		public const int
			ERROR_MOD_NOT_FOUND = 126;

		public static readonly IntPtr
			HWND_MESSAGE = new IntPtr(-3);

		#endregion Constants


		#region Functions

		[DllImport(DLL_NAME_GDI32)]
		public static extern int CombineRgn(
			IntPtr hrgnDest,
			IntPtr hrgnSrc1,
			IntPtr hrgnSrc2,
			int fnCombineMode);

		[DllImport(DLL_NAME_GDI32)]
		public static extern IntPtr CreateRectRgnIndirect(
			[In] ref RECT lprect);

		[DllImport(DLL_NAME_GDI32)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeleteObject(
			IntPtr ho);

		[DllImport(DLL_NAME_SHELL32)]
		public static extern void DragAcceptFiles(
			IntPtr hwnd,
			[MarshalAs(UnmanagedType.Bool)] bool accept);

		[DllImport(DLL_NAME_SHELL32)]
		public static extern int DragFinish(
			IntPtr hDrop);

		[DllImport(DLL_NAME_SHELL32, CharSet = CharSet.Auto, BestFitMapping = false)]
		public static extern uint DragQueryFile(
			IntPtr hDrop,
			uint iFile,
			[Out] StringBuilder lpszFile,
			uint cch);

		[DllImport(DLL_NAME_USER32)]
		public static extern IntPtr GetDC(
			IntPtr hWnd);

		[DllImport(DLL_NAME_GDI32)]
		public static extern int GetDeviceCaps(
			IntPtr hdc,
			int index);

		[DllImport(DLL_NAME_GDI32, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetTextMetrics(
			IntPtr hdc,
			out TEXTMETRIC lptm);

		[DllImport(DLL_NAME_USER32)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetUpdateRect(
			IntPtr hWnd,
			out RECT lpRect,
			[MarshalAs(UnmanagedType.Bool)] bool bErase);

		[DllImport(DLL_NAME_USER32)]
		public static extern IntPtr GetWindowDC(
			IntPtr hWnd);

		public static IntPtr GetWindowLong(IntPtr hWnd, int nIndex)
		{
			// Call the appropriate function based on 32 or 64-bit support
			if (IntPtr.Size == 4)
				return GetWindowLongPtr32(hWnd, nIndex);

			return GetWindowLongPtr64(hWnd, nIndex);
		}

		[DllImport(DLL_NAME_USER32, EntryPoint = "GetWindowLong", CharSet = CharSet.Auto)]
		private static extern IntPtr GetWindowLongPtr32(
			IntPtr hWnd,
			int nIndex);

		[DllImport(DLL_NAME_USER32, EntryPoint = "GetWindowLongPtr", CharSet = CharSet.Auto)]
		private static extern IntPtr GetWindowLongPtr64(
			IntPtr hWnd,
			int nIndex);

		[DllImport(DLL_NAME_USER32)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetWindowRect(
			IntPtr hWnd,
			out RECT lpRect);

		[DllImport(DLL_NAME_KERNEL32, CharSet = CharSet.Auto, BestFitMapping = false, SetLastError = true)]
		public extern static IntPtr LoadLibrary(
			string lpLibFileName);

		[DllImport(DLL_NAME_KERNEL32)]
		public extern static int MulDiv(
			int nNumber,
			int nNumerator,
			int nDenominator);

		[DllImport(DLL_NAME_USER32)]
		public static extern int ReleaseDC(
			IntPtr hWnd,
			IntPtr hDC);


		[DllImport(DLL_NAME_GDI32)]
		public static extern IntPtr SelectObject(
			IntPtr hdc,
			IntPtr h);


		[DllImport(DLL_NAME_USER32)]
		public static extern IntPtr SetParent(
			IntPtr hWndChild,
			IntPtr hWndNewParent);

		public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
		{
			// Call the appropriate function based on 32 or 64-bit support
			if (IntPtr.Size == 4)
				return SetWindowLongPtr32(hWnd, nIndex, dwNewLong);

			return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
		}

		[DllImport(DLL_NAME_USER32, EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
		private static extern IntPtr SetWindowLongPtr32(
			IntPtr hWnd,
			int nIndex,
			IntPtr dwNewLong);

		[DllImport(DLL_NAME_USER32, EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto)]
		private static extern IntPtr SetWindowLongPtr64(
			IntPtr hWnd,
			int nIndex,
			IntPtr dwNewLong);

		[DllImport(DLL_NAME_USER32)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetWindowPos(
			IntPtr hWnd,
			IntPtr hWndInsertAfter,
			int X,
			int Y,
			int cx,
			int cy,
			uint uFlags);

		#endregion Functions


		#region Structures

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public class LOGFONT
		{
			public const int LF_FACESIZE = 32;

			public int lfHeight;
			public int lfWidth;
			public int lfEscapement;
			public int lfOrientation;
			public int lfWeight;
			public byte lfItalic;
			public byte lfUnderline;
			public byte lfStrikeOut;
			public byte lfCharSet;
			public byte lfOutPrecision;
			public byte lfClipPrecision;
			public byte lfQuality;
			public byte lfPitchAndFamily;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = LF_FACESIZE)]
			public string lfFaceName;
		}

		// This is identical to the Windows NMHDR structure
		[StructLayout(LayoutKind.Sequential)]
		public struct NotifyHeader
		{
			public IntPtr hwndFrom;
			public IntPtr idFrom;
			public uint code;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct SCNotification
		{
			public NotifyHeader nmhdr;
			public int position;
			public char ch;
			public int modifiers;
			public int modificationType;
			public IntPtr text;
			public int length;
			public int linesAdded;
			public int message;
			public IntPtr wParam;
			public IntPtr lParam;
			public int line;
			public int foldLevelNow;
			public int foldLevelPrev;
			public int margin;
			public int listType;
			public int x;
			public int y;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public struct TEXTMETRIC
		{
			public int tmHeight;
			public int tmAscent;
			public int tmDescent;
			public int tmInternalLeading;
			public int tmExternalLeading;
			public int tmAveCharWidth;
			public int tmMaxCharWidth;
			public int tmWeight;
			public int tmOverhang;
			public int tmDigitizedAspectX;
			public int tmDigitizedAspectY;
			public char tmFirstChar;
			public char tmLastChar;
			public char tmDefaultChar;
			public char tmBreakChar;
			public byte tmItalic;
			public byte tmUnderlined;
			public byte tmStruckOut;
			public byte tmPitchAndFamily;
			public byte tmCharSet;
		}

		#endregion Structures
	}
}
