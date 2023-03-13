using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Sim.Map;

namespace Sim.OpenGL.Elements
{
    public struct Quadrilateral
    {

        public List<float> Vertices;

        public Quadrilateral(Size size, OpenTK.Mathematics.Vector2 vector)
        {
            Vertices = new List<float>() { };
            //
            Vertices.Add(vector.X);
            Vertices.Add(vector.Y);
            Vertices.Add(0.0f);
            //
            Vertices.Add(vector.X + (float)size.Width);
            Vertices.Add(vector.Y);
            Vertices.Add(0.0f);
            //
            Vertices.Add(vector.X + (float)size.Width);
            Vertices.Add(vector.Y + (float)size.Height);
            Vertices.Add(0.0f);
            //
            Vertices.Add(vector.X);
            Vertices.Add(vector.Y + (float)size.Height);
            Vertices.Add(0.0f);
            //
        }
    }
}
