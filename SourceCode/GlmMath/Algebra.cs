using System;
using System.Collections.Generic;
using System.Text;

namespace Sim.GlmMath
{
    class Algebra
    {
        public static double Pow(double x, double y)
        {
            if (x >= 0) return Math.Pow(x, y);
            else return -Math.Pow(-x, y);
        }
    }
}
