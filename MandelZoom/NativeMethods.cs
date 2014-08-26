namespace MandelZoom
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    internal static class NativeMethods
    {
        private const int GWLP_HWNDPARENT = -8; // flag for setting a window's owner (not actually parent)
        private const int GWL_STYLE = -16; // flag for setting a window's style
        private const long WS_CHILD = 0x40000000; // flag for setting a window as a child
        private const long WS_POPUP = 0x80000000; // flag for setting a window as a popup
        private const uint SRCCOPY = 0xCC0020; // flag for copying a source rectangle directly to a destination rectangle

        internal static void SetFormOwner(Form form, IntPtr ownerHandle)
        {
            USER32.SetWindowLongPtr(form.Handle, GWLP_HWNDPARENT, ownerHandle);
        }

        internal static bool TrySetFormChildWindowStyle(Form form)
        {
            long newStyle = (long)USER32.GetWindowLongPtr(form.Handle, GWL_STYLE);
            if (newStyle == 0) return false;
            newStyle = (newStyle | WS_CHILD) & ~WS_POPUP;
            return USER32.SetWindowLongPtr(form.Handle, GWL_STYLE, (IntPtr)newStyle) != IntPtr.Zero;
        }

        internal static bool TrySetFormBoundsToMatchParent(Form form, IntPtr parentHandle)
        {
            Rectangle parentBounds;
            if (!USER32.GetClientRect(parentHandle, out parentBounds)) return false;
            form.Bounds = parentBounds;
            return true;
        }

        internal static bool BitBlockTransfer(IntPtr targetDC, int targetX, int targetY, int width, int height, IntPtr sourceDC, int sourceX, int sourceY)
        {
            return GDI32.BitBlt(targetDC, targetX, targetY, width, height, sourceDC, sourceX, sourceY, SRCCOPY);
        }

        internal static class USER32
        {
            [DllImport("user32.dll", EntryPoint = "GetClientRect")]
            internal static extern bool GetClientRect([In] IntPtr hWnd, [Out] out Rectangle lpRect);

            [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr"), SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist")]
            internal static extern IntPtr GetWindowLongPtr([In] IntPtr hWnd, [In] int nIndex);

            [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr"), SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist")]
            internal static extern IntPtr SetWindowLongPtr([In] IntPtr hWnd, [In] int nIndex, [In] IntPtr dwNewLong);

            [DllImport("user32.dll", EntryPoint = "SetParent")]
            internal static extern IntPtr SetParent([In] IntPtr hWndChild, [In, Optional] IntPtr hWndNewParent);
        }

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