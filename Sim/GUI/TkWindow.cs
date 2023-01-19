using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Sim.OpenGL.Elements;
using Sim.Map;
using Sim;

namespace Sim
{
    public class TkWindow : GameWindow
    {

        public const string Name = "Open Physics - 1P";

        public int VertexBufferObject;

        public TkWindow(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { Size = (width, height), Title = title })
        {
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            KeyboardState input = KeyboardState;
            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            //Code goes here.

            Context.SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
        }

        protected override void OnLoad()
        {
            Quadrilateral quad = new Quadrilateral(new Size(10, 10), new OpenTK.Mathematics.Vector2(10f, 11f));
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, quad.Vertices.Count * sizeof(float), quad.Vertices.ToArray(), BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(VertexBufferObject);
        }

        public static TkWindow Create()
        {
            int width = 300;
            int height = 300;
            using TkWindow newWindow = new TkWindow(width, height, Name);
            newWindow.Run();
            return newWindow;
        }



    }
}
