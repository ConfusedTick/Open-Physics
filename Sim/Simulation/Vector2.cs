using System;
using System.Collections.Generic;
using System.Text;
using Sim.GlmMath;

namespace Sim.Simulation
{
    public class Vector2
    {

        public double X;
        public double Y;
        public double Angle;
        public double Lenght;

        public Vector2(double x, double y, double angle, double lenght)
        {
            X = x;
            Y = y;
            Angle = angle;
            Lenght = lenght;
        }

        public void Negotiate()
        {
            Angle += 180d;
        }

        public static Vector2 operator +(Vector2 left, Vector2 right)
        {
            int betweenAngle = (int)Trigonometrics.Correct((int)(/** 180d - **/(double)(left.Angle + right.Angle)));
            double F = Math.Sqrt((left.Lenght * left.Lenght) + (right.Lenght * right.Lenght) - (2 * left.Lenght * right.Lenght * Trigonometrics.DCos(betweenAngle)));
            double A = left.Angle + (int)Trigonometrics.RadToDeg(Math.Atan((double)((double)(right.Lenght * (double)Trigonometrics.DSin(betweenAngle)) / (double)(left.Lenght + (double)(right.Lenght * (double)Trigonometrics.DCos(betweenAngle))))));
            return new Vector2(left.X, left.Y, A, F);
        }

        public static Vector2 operator -(Vector2 left, Vector2 right)
        {
            right.Negotiate();
            int betweenAngle = (int)Trigonometrics.Correct((int)(/** 180d - **/(double)(left.Angle + right.Angle)));
            double F = Math.Sqrt((left.Lenght * left.Lenght) + (right.Lenght * right.Lenght) - (2 * left.Lenght * right.Lenght * Trigonometrics.DCos(betweenAngle)));
            double A = left.Angle + (int)Trigonometrics.RadToDeg(Math.Atan((double)((double)(right.Lenght * (double)Trigonometrics.DSin(betweenAngle)) / (double)(left.Lenght + (double)(right.Lenght * (double)Trigonometrics.DCos(betweenAngle))))));
            right.Negotiate();
            return new Vector2(left.X, left.Y, A, F);
        }

    }
}
