namespace MandelZoom
{
    // This struct is used instead of the built-in single-precision RectangleF struct.
    internal struct RectangleD
    {
        internal double Left, Top, Width, Height;

        internal RectangleD(double left, double top, double width, double height)
        {
            this.Left = left;
            this.Top = top;
            this.Width = width;
            this.Height = height;
        }
    }
}