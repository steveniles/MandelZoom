namespace MandelZoom
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;

    internal static class NativeMethods
    {
        // WARNING: This class is designed for use on 64bit systems only!

        internal const int GWLP_HWNDPARENT = -8; // flag for setting a window's owner (not actually parent)
        internal const int GWL_STYLE = -16; // flag for setting a window's style
        internal const long WS_CHILD = 0x40000000; // flag for setting a window as a child
        internal const long WS_POPUP = 0x80000000; // flag for setting a window as a popup
        internal const uint SRCCOPY = 0xCC0020; // flag for copying a source rectangle directly to a destination rectangle

        // This method only exists on 64bit systems.
        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        internal static extern long GetWindowLongPtr(IntPtr handle, int flag);

        // This method only exists on 64bit systems.
        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        internal static extern long SetWindowLongPtr(IntPtr handle, int flag, long newValue);

        [DllImport("user32.dll", EntryPoint = "GetClientRect")]
        internal static extern bool GetClientRect(IntPtr handle, out Rectangle rectangle);

        [DllImport("user32.dll", EntryPoint = "SetParent")]
        internal static extern long SetParent(IntPtr childHandle, IntPtr newParentHandle);

        internal static class GDI32
        {
            [DllImport("gdi32.dll", EntryPoint = "BitBlt")]
            internal static extern bool BitBlt([In] IntPtr hdcDest, [In] int nXDest, [In]int nYDest, [In]int nWidth, [In] int nHeight, [In] IntPtr hdcSrc, [In] int nXSrc, [In]int nYSrc, [In] uint dwRop);

            [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC")]
            internal static extern IntPtr CreateCompatibleDC([In] IntPtr hdc);

            [DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
            internal static extern bool DeleteDC([In] IntPtr hdc);

            [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
            internal static extern bool DeleteObject([In] IntPtr hObject);

            [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
            internal static extern IntPtr SelectObject([In] IntPtr hdc, [In] IntPtr hgdiobj);
        }
    }
}