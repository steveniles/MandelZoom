namespace MandelZoom
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Threading;
    using System.Windows.Forms;
    using Properties;

    internal sealed partial class ScreenSaverForm : Form
    {
        private const int BaseMaximumIterations = 150;
        private const int WaitMilliseconds = 15000;
        private static readonly Random Random = new Random();

        private readonly Stopwatch stopwatch = new Stopwatch();

        private Func<int, double, Color> colorFunction;
        private RectangleD initialFractalArea;
        private Bitmap initialFractalBitmap;
        private BackgroundWorker looper;

        #region Init

        internal ScreenSaverForm()
        {
            this.InitializeComponent();
        }

        private void ScreenSaverForm_Load(object sender, EventArgs e)
        {
            this.AssignColorScheme();
            this.initialFractalArea = Mandelbrot.GetInitialArea(this.Size);
            this.initialFractalBitmap = Mandelbrot.RenderInitialFractal(this.Size, this.initialFractalArea, BaseMaximumIterations, this.colorFunction);
        }

        private void ScreenSaverForm_Shown(object sender, EventArgs e)
        {
            Graphics formGraphics = this.CreateGraphics();
            formGraphics.DrawImageUnscaled(this.initialFractalBitmap, 0, 0);
            this.stopwatch.Restart();
            this.looper = new BackgroundWorker();
            this.looper.DoWork += delegate
            {
                while (true)
                {
                    RectangleD fractalArea = this.initialFractalArea;
                    var currentBitmap = this.initialFractalBitmap.Clone() as Bitmap;
                    for (int zoom = 1; zoom < 10; zoom++)
                    {
                        Rectangle zoomRectangle = Mandelbrot.GetNextZoomRectangle(currentBitmap);
                        // If couldn't find suitable area to zoom in, zoom out to starting fractal and start over.
                        if (zoomRectangle == Rectangle.Empty) break;
                        fractalArea = Mandelbrot.GetNextZoomArea(this.Size, zoomRectangle.Location, fractalArea);
                        Bitmap nextBitmap = Mandelbrot.RenderFractal(this.Size, fractalArea, BaseMaximumIterations * (zoom + 1), this.colorFunction);
                        IntPtr[] nativeZoomInBitmapHandles = CreateNativeZoomInBitmaps(currentBitmap, nextBitmap, zoomRectangle.Location);
                        // Wait a bit between zooms, even if all computations are complete.
                        this.stopwatch.Stop();
                        if (this.stopwatch.ElapsedMilliseconds < WaitMilliseconds) Thread.Sleep(WaitMilliseconds - (int)this.stopwatch.ElapsedMilliseconds);
                        AnimateBlink(formGraphics, currentBitmap, zoomRectangle);
                        AnimateZoomIn(formGraphics, nativeZoomInBitmapHandles, this.Size);
                        this.stopwatch.Restart();
                        // Clean up animation bitmaps after use.
                        foreach (IntPtr nativeZoomInBitmapHandle in nativeZoomInBitmapHandles)
                        {
                            NativeMethods.GDI32.DeleteObject(nativeZoomInBitmapHandle);
                        }
                        currentBitmap.Dispose();
                        currentBitmap = nextBitmap;
                    }
                    // After 10 zoom-ins, zoom out to starting fractal and start over.
                    double percentX = ((fractalArea.Left + (fractalArea.Width / 2D)) - this.initialFractalArea.Left) / this.initialFractalArea.Width;
                    double percentY = ((fractalArea.Top + (fractalArea.Height / 2D)) - this.initialFractalArea.Top) / this.initialFractalArea.Height;
                    var zoomPoint = new Point((int)(this.Width * percentX), (int)(this.Height * percentY));
                    IntPtr[] nativeZoomOutBitmapHandles = CreateNativeZoomOutBitmaps(currentBitmap, this.initialFractalBitmap, zoomPoint);
                    currentBitmap.Dispose();
                    // Wait a bit between zooms, even if all computations are complete.
                    this.stopwatch.Stop();
                    if (this.stopwatch.ElapsedMilliseconds < WaitMilliseconds) Thread.Sleep(WaitMilliseconds - (int)this.stopwatch.ElapsedMilliseconds);
                    AnimateZoomOut(formGraphics, nativeZoomOutBitmapHandles, this.Size);
                    this.stopwatch.Restart();
                    // Clean up animation bitmaps after use.
                    foreach (IntPtr nativeZoomOutBitmapHandle in nativeZoomOutBitmapHandles)
                    {
                        NativeMethods.GDI32.DeleteObject(nativeZoomOutBitmapHandle);
                    }
                }
            };
            this.looper.RunWorkerAsync();
        }

        #endregion Init

        #region Animation Methods

        private static void AnimateBlink(Graphics formGraphics, Bitmap displayBitmap, Rectangle zoomRectangle)
        {
            Rectangle sectionRectangle = zoomRectangle;
            sectionRectangle.Width++;
            sectionRectangle.Height++;
            Bitmap section = displayBitmap.Clone(sectionRectangle, PixelFormat.Format24bppRgb);
            for (int flashCounter = 0; flashCounter < 10; flashCounter++)
            {
                formGraphics.DrawRectangle(Pens.White, zoomRectangle);
                Thread.Sleep(100);
                formGraphics.DrawImageUnscaled(section, zoomRectangle);
                Thread.Sleep(100);
            }
        }

        private static void AnimateZoomIn(Graphics formGraphics, IntPtr[] nativeBitmapHandles, Size size)
        {
            // Create device contexts.
            IntPtr targetDC = formGraphics.GetHdc();
            IntPtr sourceDC = NativeMethods.GDI32.CreateCompatibleDC(targetDC);

            // Capture sourceDC's original empty HBitmap and select first frame for drawing.
            IntPtr emptyNativeBitmapHandle = NativeMethods.GDI32.SelectObject(sourceDC, nativeBitmapHandles[0]);

            // Draw animation frames to form.
            for (int c = 1; c < 90; c++)
            {
                NativeMethods.GDI32.BitBlt(targetDC, 0, 0, size.Width, size.Height, sourceDC, 0, 0, NativeMethods.SRCCOPY);
                NativeMethods.GDI32.SelectObject(sourceDC, nativeBitmapHandles[c]);
                Thread.Sleep(5);
            }

            // Draw final frame.
            NativeMethods.GDI32.BitBlt(targetDC, 0, 0, size.Width, size.Height, sourceDC, 0, 0, NativeMethods.SRCCOPY);

            // Clean up.
            NativeMethods.GDI32.SelectObject(sourceDC, emptyNativeBitmapHandle);
            NativeMethods.GDI32.DeleteDC(sourceDC);
            formGraphics.ReleaseHdc(targetDC);
        }

        private static void AnimateZoomOut(Graphics formGraphics, IntPtr[] nativeBitmapHandles, Size size)
        {
            // Create device contexts.
            IntPtr targetDC = formGraphics.GetHdc();
            IntPtr sourceDC = NativeMethods.GDI32.CreateCompatibleDC(targetDC);

            // Capture sourceDC's original empty HBitmap and select first frame for drawing.
            IntPtr emptyNativeBitmapHandle = NativeMethods.GDI32.SelectObject(sourceDC, nativeBitmapHandles[99]);

            // Draw animation frames to form (index's in reverse order because they were drawn in reverse order in CreateNativeZoomOutBitmaps()).
            for (int c = 99; c >= 0; c--)
            {
                NativeMethods.GDI32.BitBlt(targetDC, 0, 0, size.Width, size.Height, sourceDC, 0, 0, 0x00CC0020);
                NativeMethods.GDI32.SelectObject(sourceDC, nativeBitmapHandles[c]);
                Thread.Sleep(5);
            }

            // Draw final frame.
            NativeMethods.GDI32.BitBlt(targetDC, 0, 0, size.Width, size.Height, sourceDC, 0, 0, 0x00CC0020);

            // Clean up.
            NativeMethods.GDI32.SelectObject(sourceDC, emptyNativeBitmapHandle);
            NativeMethods.GDI32.DeleteDC(sourceDC);
            formGraphics.ReleaseHdc(targetDC);
        }

        #endregion Animation Methods

        #region Bitmap Functions

        private static IntPtr[] CreateNativeZoomInBitmaps(Bitmap currentBitmap, Bitmap nextBitmap, Point zoomPoint)
        {
            // Create a full size bitmap for every frame of the zoom-in animation.
            var nativeBitmapHandles = new IntPtr[90];
            var animationFramesBitmap = currentBitmap.Clone() as Bitmap;
            var animationFramesGraphics = Graphics.FromImage(animationFramesBitmap);
            for (int c = 0; c < 89; c++)
            {
                double sizeScalar = (c + 11D) / 100D;
                double locationScalar = (89D - c) / 90D;
                var size = new Size((int)(nextBitmap.Width * sizeScalar), (int)(nextBitmap.Height * sizeScalar));
                var location = new Point((int)(zoomPoint.X * locationScalar), (int)(zoomPoint.Y * locationScalar));
                var rectangle = new Rectangle(location, size);
                animationFramesGraphics.DrawImage(nextBitmap, rectangle);
                animationFramesGraphics.DrawRectangle(Pens.White, rectangle);
                nativeBitmapHandles[c] = animationFramesBitmap.GetHbitmap();
            }
            nativeBitmapHandles[89] = nextBitmap.GetHbitmap();
            animationFramesGraphics.Dispose();
            animationFramesBitmap.Dispose();
            return nativeBitmapHandles;
        }

        private static IntPtr[] CreateNativeZoomOutBitmaps(Bitmap currentBitmap, Bitmap initialBitmap, Point zoomPoint)
        {
            // Create a full size bitmap for every frame of the zoom-out animation.
            var nativeBitmapHandles = new IntPtr[100];
            var animationFramesBitmap = initialBitmap.Clone() as Bitmap;
            var animationFramesGraphics = Graphics.FromImage(animationFramesBitmap);
            nativeBitmapHandles[0] = initialBitmap.GetHbitmap();
            // Draw them in reverse order, so the same image can be used for each (earlier frames have larger overlays that can completely draw over smaller, later frames).
            for (int c = 1; c < 100; c++)
            {
                double sizeScalar = c / 99D;
                double locationScalar = (100D - c) / 100D;
                var size = new Size((int)(initialBitmap.Width * sizeScalar), (int)(initialBitmap.Height * sizeScalar));
                var location = new Point((int)(zoomPoint.X * locationScalar), (int)(zoomPoint.Y * locationScalar));
                var rectangle = new Rectangle(location, size);
                animationFramesGraphics.DrawImage(currentBitmap, rectangle);
                animationFramesGraphics.DrawRectangle(Pens.White, rectangle);
                nativeBitmapHandles[c] = animationFramesBitmap.GetHbitmap();
            }
            animationFramesGraphics.Dispose();
            animationFramesBitmap.Dispose();
            return nativeBitmapHandles;
        }

        #endregion Bitmap Functions

        private void AssignColorScheme()
        {
            switch (Settings.Default.RandomColorScheme ? Random.Next(4) : Settings.Default.ColorScheme)
            {
                default:
                    this.colorFunction = Mandelbrot.SilverFrostColorFunction;
                    break;

                case 1:
                    this.colorFunction = Mandelbrot.NeonGlowColorFunction;
                    break;

                case 2:
                    this.colorFunction = Mandelbrot.SolarFlareColorFunction;
                    break;

                case 3:
                    this.colorFunction = Mandelbrot.PsychedelicColorFunction;
                    break;
            }
        }
    }
}