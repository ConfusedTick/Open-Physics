using System;
using System.Collections.Generic;
using System.Text;

namespace Sim.Map
{
    public readonly struct Size : ICloneable
    {

        public static Size DefaultSize { get { return new Size(0.9d, 0.9d); } }

        public readonly double Width;
        public readonly double Height;

        public readonly double Area;

        public Size(double width, double height)
        {
            Width = width;
            Height = height;
            Area = width * height;
        }

        public override string ToString()
        {
            return Width + ":" + Height;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
