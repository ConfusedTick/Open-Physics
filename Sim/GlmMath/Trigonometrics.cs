﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Sim.GlmMath
{
    public static partial class Trigonometrics
    {

        public static double drMultiplier = 0.017453292519943295d;

        public static double PIDivide = 180d / Math.PI;

        public static double DegToRad(double degrees)
        {
            return degrees * drMultiplier;
        }

        public static double RadToDeg(double radians)
        {
            return (double)(radians * 57.295779513082320876798154814105d);
        }

        public static double DCos(double angle)
        {
            return Math.Cos(angle * drMultiplier);
        }

        public static double DSin(double angle)
        {
            return Math.Sin(angle * drMultiplier);
        }

        public static double DTan(double angle)
        {
            return Math.Tan(angle * drMultiplier);
        }

        public static double Cos(double angle)
        {
            return Math.Cos(angle);
        }

        public static double Sin(double angle)
        {
            return Math.Sin(angle);
        }

        public static double Tan(double angle)
        {
            return Math.Tan(angle);
        }

        public static double Correct(int angle)
        {
            return RadToDeg(Math.Acos(DCos(angle)));
        }

        public static double DCosOfAngleBetween(int a, int b)
        {
            double s = DCos(a) + DCos(b);
            return Math.Abs(s);
        }

        public static double CosOfAngleBetween(double a, double b)
        {
            double s = Math.Cos(a) - Math.Cos(b);
            return Math.Abs(s);
        }

    }
}
