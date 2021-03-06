﻿namespace MandelZoom
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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var settingsForm = new SettingsForm();
            long ownerHandle;
            if (owner != null && long.TryParse(owner, out ownerHandle))
            {
                NativeMethods.SetFormOwner(settingsForm, (IntPtr)ownerHandle);
            }
            settingsForm.ShowDialog();
            settingsForm.Dispose();
        }

        private static void RunPreview(string parent)
        {
            long parentHandle;
            if (!long.TryParse(parent, out parentHandle)) return;
            var previewForm = new ScreenSaverForm();
            if (!NativeMethods.TrySetFormChildWindowStyle(previewForm)) return;
            if (!NativeMethods.TrySetFormBoundsToMatchParent(previewForm, (IntPtr)parentHandle)) return;
            NativeMethods.USER32.SetParent(previewForm.Handle, (IntPtr)parentHandle);
            Application.Run(previewForm);
        }

        private static void RunScreenSaver()
        {
            var random = new Random();
            var viewports = Settings.Default.SpanScreens
                ? new List<Rectangle> { Screen.AllScreens.Select(s => s.Bounds).Aggregate(Rectangle.Union) }
                : Screen.AllScreens.Select(s => s.Bounds);

            foreach (var view in viewports)
            {
                var screenSaverForm = new ScreenSaverForm
                {
                    Bounds = view,
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