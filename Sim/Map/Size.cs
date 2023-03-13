using System;
using System.Collections.Generic;
using System.Text;

namespace Sim.Map
{
    public class Size : ICloneable
    {

        private readonly static Lazy<Size> DefaultSize = new Lazy<Size>(new Size(1d, 1d));

        public const double DefaultWidth = 1d;
        public const double DefaultMaxRaduis = .75d;

        public readonly double Width;
        public readonly double Height;

        public readonly double Area;

        public readonly double MaxRaduis;

        public Size(double width, double height)
        {
            Width = width;
            Height = height;
            Area = width * height;
            MaxRaduis = ((double)((double)width / 2) * (double)Math.Sqrt(2d));
            MaxRaduis += MaxRaduis * 0.01d;
        }

        public override string ToString()
        {
            return Width + ":" + Height;
        }

        public static Size GetDefaultSize()
        {
            return DefaultSize.Value;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
