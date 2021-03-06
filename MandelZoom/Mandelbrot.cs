﻿namespace MandelZoom
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Threading.Tasks;

    internal static class Mandelbrot
    {
        private static readonly Random Random = new Random();

        #region Render Functions

        internal static Bitmap RenderFractal(Size imageSize, RectangleD complexArea, int maximumIterations, Func<int, double, Color> colorFunction)
        {
            var fractalBitmap = new Bitmap(imageSize.Width, imageSize.Height, PixelFormat.Format24bppRgb);
            // Use LockBits and pointers for faster image processing. GetPixel and SetPixel are slow.
            BitmapData fractalBitmapData = fractalBitmap.LockBits(new Rectangle(0, 0, imageSize.Width, imageSize.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            // Process each row of the image in parallel.
            Parallel.For(0, imageSize.Height, yPixelCoord =>
            {
                for (long xPixelCoord = 0; xPixelCoord < imageSize.Width; xPixelCoord++)
                {
                    unsafe
                    {
                        // 24bit bitmaps store color bytes in BGR order, not RGB.
                        byte* pointerBGR = (byte*)((long)fractalBitmapData.Scan0 + (yPixelCoord * fractalBitmapData.Stride) + (xPixelCoord * 3));
                        // Convert pixel coords to complex plane coords, using the center of the pixel, not the upper left corner.
                        double x = (complexArea.Width * ((xPixelCoord + 0.5D) / imageSize.Width)) + complexArea.Left;
                        double y = (complexArea.Height * ((yPixelCoord + 0.5D) / imageSize.Height)) + complexArea.Top;
                        // Color pixel based on number of iterations and escape magnitude
                        double finalMagnitude;
                        Color color = colorFunction(CountIterations(x, y, maximumIterations, out finalMagnitude), finalMagnitude);
                        *pointerBGR = color.B;
                        *++pointerBGR = color.G;
                        *++pointerBGR = color.R;
                    }
                }
            });
            fractalBitmap.UnlockBits(fractalBitmapData);
            return fractalBitmap;
        }

        internal static Bitmap RenderInitialFractal(Size imageSize, RectangleD complexArea, int maximumIterations, Func<int, double, Color> colorFunction)
        {
            var fractalBitmap = new Bitmap(imageSize.Width, imageSize.Height, PixelFormat.Format24bppRgb);
            // Use LockBits and pointers for faster image processing. GetPixel and SetPixel are slow.
            BitmapData fractalBitmapData = fractalBitmap.LockBits(new Rectangle(Point.Empty, imageSize), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            // Process each row of the image in parallel, using multiple CPU cores for speed.
            Parallel.For(0, imageSize.Height / 2, yPixelCoord =>
            {
                for (long xPixelCoord = 0; xPixelCoord < imageSize.Width; xPixelCoord++)
                {
                    unsafe
                    {
                        // 24bit bitmaps store color bytes in BGR order, not RGB.
                        byte* pointerBGR = (byte*)((long)fractalBitmapData.Scan0 + (yPixelCoord * fractalBitmapData.Stride) + (xPixelCoord * 3));
                        // For initial fractal image, the bottom half is a mirror of top half.
                        byte* mirrorBGR = (byte*)((long)fractalBitmapData.Scan0 + (((imageSize.Height - 1) - yPixelCoord) * fractalBitmapData.Stride) + (xPixelCoord * 3));
                        // Convert pixel coords to complex plane coords, using the center of the pixel, not the upper left corner.
                        double x = (complexArea.Width * ((xPixelCoord + 0.5D) / imageSize.Width)) + complexArea.Left;
                        double y = (complexArea.Height * ((yPixelCoord + 0.5D) / imageSize.Height)) + complexArea.Top;
                        // Color pixel based on number of iterations and escape magnitude
                        // Save time by only calculating iterations for points not in one of the large bulbs, as many pixels in the initial fractal are.
                        double finalMagnitude;
                        var color = (IsInCardioid(x, y) || IsInPeriod2Bulb(x, y)) ? Color.Black : colorFunction(CountIterations(x, y, maximumIterations, out finalMagnitude), finalMagnitude);
                        *mirrorBGR = *pointerBGR = color.B;
                        *++mirrorBGR = *++pointerBGR = color.G;
                        *++mirrorBGR = *++pointerBGR = color.R;
                    }
                }
            });
            fractalBitmap.UnlockBits(fractalBitmapData);
            return fractalBitmap;
        }

        #endregion Render Functions

        #region Area Functions

        internal static RectangleD GetInitialArea(Size screenSize)
        {
            // Adjust viewing area to contain the whole Mandelbrot set while matching the screen's aspect ratio.
            var rectangle = new RectangleD(-2, -1, 3, 2);
            double scalar = (screenSize.Width / 3D) / (screenSize.Height / 2D);
            if (scalar > 1)
            {
                rectangle.Width *= scalar;
                rectangle.Left = -0.5 - (rectangle.Width / 2D);
            }
            else
            {
                rectangle.Height /= scalar;
                rectangle.Top = 0 - (rectangle.Height / 2D);
            }
            return rectangle;
        }

        internal static RectangleD GetNextZoomArea(Size screenSize, Point zoomLocation, RectangleD currentArea)
        {
            // Convert pixel coords of next zoom area into complex coords.
            double newLeft = (((double)zoomLocation.X / screenSize.Width) * currentArea.Width) + currentArea.Left;
            double newTop = (((double)zoomLocation.Y / screenSize.Height) * currentArea.Height) + currentArea.Top;
            return new RectangleD(newLeft, newTop, currentArea.Width / 10D, currentArea.Height / 10D);
        }

        internal static Rectangle GetNextZoomRectangle(Bitmap currentBitmap)
        {
            // Figure out where to zoom in next, in terms of pixel coords.
            var zoomRectangle = new Rectangle(Point.Empty, new Size(currentBitmap.Width / 10, currentBitmap.Height / 10));
            var searchArea = new Size(currentBitmap.Width - zoomRectangle.Width, currentBitmap.Height - zoomRectangle.Height);
            // If can't find viable area to zoom in on after many attempts, don't zoom in, zoom back out to starting fractal.
            for (int attempt = 0; attempt < 1000; attempt++)
            {
                zoomRectangle.Location = new Point(Random.Next(searchArea.Width), Random.Next(searchArea.Height));
                if (IsViableRegion(currentBitmap, zoomRectangle)) return zoomRectangle;
            }
            return Rectangle.Empty;
        }

        private static bool IsViableRegion(Bitmap testBitmap, Rectangle testRectangle)
        {
            // Check if test region of bitmap is an interesting area to zoom in on, i.e. is on the edge of the Mandelbrot set.
            int setMembers = 0;
            BitmapData testBitmapData = testBitmap.LockBits(testRectangle, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            // Count how many pixels are black.
            for (int y = 0; y < testBitmapData.Height; y++)
            {
                for (int x = 0; x < testBitmapData.Width; x++)
                {
                    unsafe
                    {
                        byte* pointerBGR = (byte*)((long)testBitmapData.Scan0 + (y * testBitmapData.Stride) + (x * 3));
                        if (*pointerBGR == 0 && *++pointerBGR == 0 && *++pointerBGR == 0) setMembers++;
                    }
                }
            }
            testBitmap.UnlockBits(testBitmapData);
            // Is the region 60%-80% colored and 20%-40% set members?
            double setMemberPercent = (double)setMembers / (testBitmapData.Width * testBitmapData.Height);
            return setMemberPercent <= 0.4 && setMemberPercent >= 0.2;
        }

        #endregion Area Functions

        #region Color Functions

        internal static Color NeonGlowColorFunction(int iterations, double magnitude)
        {
            if (iterations < 0) return Color.Black;
            int value = iterations % 512;
            if (value > 255) value = 511 - value;
            return Color.FromArgb(0, value, 255 - value);
        }

        internal static Color PsychedelicColorFunction(int iterations, double magnitude)
        {
            if (iterations < 0) return Color.Black;
            int red = (magnitude >= 16) ? 1 : 255 - (int)(((magnitude - 4) / 12) * 254);
            int green = ((iterations * 4) % 255) + 1;
            int blue = 255 - (((iterations * 4) + 127) % 255);
            return Color.FromArgb(red, green, blue);
        }

        internal static Color SilverFrostColorFunction(int iterations, double magnitude)
        {
            if (iterations < 0) return Color.Black;
            int value = iterations % 255;
            return Color.FromArgb(value + 1, value + 1, value + 1);
        }

        internal static Color SolarFlareColorFunction(int iterations, double magnitude)
        {
            if (iterations < 0) return Color.Black;
            int value = (iterations + 128) % 256;
            if (value > 127) value = 255 - value;
            return Color.FromArgb(255, value + 128, 0);
        }

        #endregion Color Functions

        #region Mandelbrot Functions

        private static int CountIterations(double x, double y, int maximumIterations, out double finalMagnitude)
        {
            double currentX = x;
            double currentY = y;
            double previousX = 0;
            double previousY = 0;
            // Iterate twice for every loop to cut expensive if statement usage in half, and also save use of a temp variable.
            for (int counter = 0; counter < maximumIterations; counter += 2)
            {
                double currentXSquared = currentX * currentX;
                double currentYSquared = currentY * currentY;
                if (currentXSquared + currentYSquared >= 16)
                {
                    if ((previousX * previousX) + (previousY * previousY) >= 16)
                    {
                        finalMagnitude = Math.Sqrt((previousX * previousX) + (previousY * previousY));
                        return counter - 1;
                    }
                    finalMagnitude = Math.Sqrt(currentXSquared + currentYSquared);
                    return counter;
                }
                previousX = (currentXSquared - currentYSquared) + x;
                previousY = ((currentX + currentX) * currentY) + y;
                currentX = (previousX * previousX) - (previousY * previousY) + x;
                currentY = ((previousX + previousX) * previousY) + y;
            }
            // Return -1 for points considered inside the set.
            finalMagnitude = 0;
            return -1;
        }

        private static bool IsInCardioid(double x, double y)
        {
            double q = ((x - 0.25) * (x - 0.25)) + (y * y);
            return q * (q + (x - 0.25)) < 0.25 * y * y;
        }

        private static bool IsInPeriod2Bulb(double x, double y)
        {
            return ((x + 1) * (x + 1)) + (y * y) < 0.0625;
        }

        #endregion Mandelbrot Functions
    }
}