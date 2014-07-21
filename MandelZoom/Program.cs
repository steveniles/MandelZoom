namespace MandelZoom
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using Properties;

    internal static class Program
    {
        private static Point mousePosition;

        [STAThread]
        private static void Main(string[] args)
        {
            if (args.Length == 0) RunConfig();
            else if (args.Length == 1 && args[0].Length > 3 && args[0].Substring(0, 3).Equals("/c:", StringComparison.OrdinalIgnoreCase)) RunConfig(args[0].Substring(3));
            else if (args.Length == 2 && args[0].Equals("/p", StringComparison.OrdinalIgnoreCase)) RunPreview(args[1]);
            else if (args.Length == 1 && args[0].Equals("/s", StringComparison.OrdinalIgnoreCase)) RunScreenSaver();
        }

        private static void RunConfig(string owner = null)
        {
            long ownerHandle;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var settingsForm = new SettingsForm();
            // Try to set the control panel window as the owner of this settings window.
            if (owner != null && long.TryParse(owner, out ownerHandle))
            {
                NativeMethods.USER32.SetWindowLongPtr(settingsForm.Handle, NativeMethods.GWLP_HWNDPARENT, ownerHandle);
            }
            settingsForm.ShowDialog();
            settingsForm.Dispose();
        }

        private static void RunPreview(string parent)
        {
            long parentHandle;
            Rectangle parentBounds;
            if (long.TryParse(parent, out parentHandle) && NativeMethods.USER32.GetClientRect((IntPtr)parentHandle, out parentBounds))
            {
                var previewForm = new ScreenSaverForm();
                // Get current window style, add child flag, remove popup flag, and set as new window style.
                long newStyle = NativeMethods.USER32.GetWindowLongPtr(previewForm.Handle, NativeMethods.GWL_STYLE);
                if (newStyle == 0) return;
                newStyle = (newStyle | NativeMethods.WS_CHILD) & ~NativeMethods.WS_POPUP;
                if (NativeMethods.USER32.SetWindowLongPtr(previewForm.Handle, NativeMethods.GWL_STYLE, newStyle) == 0) return;
                previewForm.Bounds = parentBounds;
                NativeMethods.USER32.SetParent(previewForm.Handle, (IntPtr)parentHandle);
                Application.Run(previewForm);
            }
        }

        private static void RunScreenSaver()
        {
            IEnumerable<Rectangle> screenAreas;
            if (Settings.Default.SpanScreens)
            {
                screenAreas = new List<Rectangle>(1) { Screen.AllScreens.Select(s => s.Bounds).Aggregate((a, b) => Rectangle.Union(a, b)) };
            }
            else
            {
                screenAreas = Screen.AllScreens.Select(s => s.Bounds);
            }

            var random = new Random();
            foreach (var area in screenAreas)
            {
                var screenSaverForm = new ScreenSaverForm()
                {
                    Bounds = area,
                    StartPosition = FormStartPosition.Manual,
                    TopMost = true,
                    Opacity = Settings.Default.RandomOpacity ? (random.NextDouble() * 0.75D) + 0.25D : Settings.Default.OpacityPercent / 100D
                };
                screenSaverForm.KeyDown += (sender, e) => ShutDownScreenSaver();
                screenSaverForm.MouseDown += (sender, e) => ShutDownScreenSaver();
                screenSaverForm.MouseMove += (sender, e) =>
                {
                    // Don't end the screen saver if the mouse only moved a tiny bit
                    if (mousePosition != Point.Empty && (e.X - mousePosition.X > 5 || e.X - mousePosition.X < -5 || e.Y - mousePosition.Y > 5 || e.Y - mousePosition.Y < -5)) ShutDownScreenSaver(); else mousePosition = e.Location;
                };
                screenSaverForm.Show();
            }
            Cursor.Hide();
            Application.Run();
        }

        private static void ShutDownScreenSaver()
        {
            foreach (ScreenSaverForm screenSaverForm in Application.OpenForms) screenSaverForm.Hide();
            Cursor.Show();
            Application.Exit();
        }
    }
}