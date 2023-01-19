using System;
using System.Collections.Generic;
using System.Text;

namespace Sim.Utils
{
    sealed class Utils
    {
        public static bool IsAcceptable(double test) => double.IsNormal(test) || test == 0d;

        public static bool ToBool(string test) => test == "1";
        public static bool ToBool(int test) => test == 1;

        public static Random GetNewRandom() => new Random(Convert.ToInt32(DateTime.Now.ToString("fffff")));

        public static double Pow(double x, double y)
        {
            if (x >= 0) return Math.Pow(x, y);
            else return -1d * Math.Pow(-1d * x, y);
        }

        public static double GetCosineBySinus(double sinus)
        {
            return Math.Sqrt(1 - sinus * sinus);
        }

        public static double GetSinusByCosine(double cosine)
        {
            return Math.Sqrt(1 - cosine * cosine);
        }

    }
}
