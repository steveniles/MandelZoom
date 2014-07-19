namespace MandelZoom
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
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
                var previewForm = new ScreenSaverForm()
                {
                    FormBorderStyle = FormBorderStyle.None,
                    BackColor = Color.Black,
                    ControlBox = false,
                    MaximizeBox = false,
                    MinimizeBox = false,
                    ShowIcon = false,
                    ShowInTaskbar = false,
                    SizeGripStyle = SizeGripStyle.Hide
                };
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
            var screenSaverForms = new List<ScreenSaverForm>();
            if (Settings.Default.SpanScreens)
            {
                // One form that covers all monitors.
                screenSaverForms.Add(new ScreenSaverForm() { FormBorderStyle = FormBorderStyle.None, Bounds = Screen.PrimaryScreen.Bounds });

                foreach (var screen in Screen.AllScreens)
                {
                    screenSaverForms[0].Bounds = Rectangle.Union(screenSaverForms[0].Bounds, screen.Bounds);
                }
            }
            else
            {
                foreach (var screen in Screen.AllScreens)
                {
                    // One form per monitor.
                    screenSaverForms.Add(new ScreenSaverForm() { FormBorderStyle = FormBorderStyle.None, Bounds = screen.Bounds });
                }
            }
            var random = new Random();
            foreach (ScreenSaverForm screenSaverForm in screenSaverForms)
            {
                screenSaverForm.StartPosition = FormStartPosition.Manual;
                screenSaverForm.BackColor = Color.Black;
                screenSaverForm.ControlBox = false;
                screenSaverForm.MaximizeBox = false;
                screenSaverForm.MinimizeBox = false;
                screenSaverForm.ShowIcon = false;
                screenSaverForm.ShowInTaskbar = false;
                screenSaverForm.SizeGripStyle = SizeGripStyle.Hide;
                screenSaverForm.TopMost = true;
                screenSaverForm.Opacity = Settings.Default.RandomOpacity ? (random.NextDouble() * 0.75D) + 0.25D : Settings.Default.OpacityPercent / 100D;
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