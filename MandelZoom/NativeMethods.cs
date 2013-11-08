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
        internal const long WS_CHILD = 0x40000000L; // flag for setting a window as a child
        internal const long WS_POPUP = 0x80000000L; // flag for setting a window as a popup
        internal const uint SRCCOPY = 0x00CC0020U; // flag for copying a source rectangle directly to a destination rectangle

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

        [DllImport("gdi32.dll", EntryPoint = "BitBlt")]
        internal static extern bool BitBlt(IntPtr targetDC, int targetX, int targetY, int width, int height, IntPtr sourceDC, int sourceX, int sourceY, uint rasterOpCode);

        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC")]
        internal static extern IntPtr CreateCompatibleDC(IntPtr DCHandle);

        [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
        internal static extern IntPtr SelectObject(IntPtr DCHandle, IntPtr objectHandle);

        [DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
        internal static extern IntPtr DeleteDC(IntPtr DCHandle);

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        internal static extern IntPtr DeleteObject(IntPtr objectHandle);
    }
}