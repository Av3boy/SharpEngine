using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;

using System.Collections.Generic;

namespace Core
{
    // In this tutorial we focus on how to set up a scene with multiple lights, both of different types but also
    // with several point lights
    public class Window : GameWindow
    {
        private IGame _game;
        private Renderer _renderer;
        private Scene _scene;

        private bool _firstMove = true;
        private Vector2 _lastPos;

        // TODO: Read only once, load into OpenGL buffer once.
        // If already loaded, add mesh indetifier to a dictionary. If dict contains mesh, skip it.
        public float[] Vertices => GetVertices();

        public Window(IGame game, Scene scene, GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            _game = game;
            _scene = scene;
        }

        private float[] GetVertices()
        {
            var mesh = _game.Cubes[0].Mesh; // Mesh is identical for all cubes
            var vertices = new List<float>();

            for (int i = 0; i < mesh.Vertices.Length / 3; i++)
            {
                vertices.Add(mesh.Vertices[i * 3]);
                vertices.Add(mesh.Vertices[i * 3 + 1]);
                vertices.Add(mesh.Vertices[i * 3 + 2]);

                vertices.Add(mesh.Normals[i * 3]);
                vertices.Add(mesh.Normals[i * 3 + 1]);
                vertices.Add(mesh.Normals[i * 3 + 2]);

                vertices.Add(mesh.TextureCoordinates[i * 2]);
                vertices.Add(mesh.TextureCoordinates[i * 2 + 1]);
            }

            return vertices.ToArray();
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            _renderer = new Renderer(_game, _scene, Vertices);

            _renderer.Initialize();

            _game.Camera = new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);

            CursorState = CursorState.Grabbed;
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            _renderer.Render(_game.Camera);

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (!IsFocused)
            {
                return;
            }

            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            _game.HandleMovement(KeyboardState, (float)args.Time);
            _game.HandleMouseMovement(MouseState, ref _firstMove, ref _lastPos);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            _game.Camera.Fov -= e.OffsetY;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
            _game.Camera.AspectRatio = Size.X / (float)Size.Y;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            // base.OnMouseDown(e);
            _game.HandleMouseDown(e);
        }
    }
}
