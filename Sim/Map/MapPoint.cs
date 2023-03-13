using System;
using System.Collections.Generic;
using System.Text;

namespace Sim.Map
{
    public struct MapPoint
    {

        public readonly double X, Y;

        public readonly double[] AsArray;

        public MapPoint(double x, double y)
        {
            X = x;
            Y = y;
            AsArray = new double[] { x, y };
        }

        public double[] ToArray()
        {
            return AsArray;
        }

    }
}
