using ObjLoader.Loaders.ObjLoader;
using SharpEngine.Core._Resources;
using SharpEngine.Core.Components.Properties.Textures;
using SharpEngine.Core.Extensions;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Shader = SharpEngine.Core.Shaders.Shader;
using Texture = SharpEngine.Core.Components.Properties.Textures.Texture;

namespace Tutorial
{
    class Program
    {
        #region fields
        private static IWindow window;
        private static GL Gl;
        private static IKeyboard? primaryKeyboard;

        private static Texture Texture;
        private static Shader Shader;
        private static List<Model_Old> Models = [];

        //Setup the camera's location, directions, and movement speed
        private static Vector3 CameraPosition = new Vector3(0.0f, 0.0f, 3.0f);
        private static Vector3 CameraFront = new Vector3(0.0f, 0.0f, -1.0f);
        private static Vector3 CameraUp = Vector3.UnitY;
        private static Vector3 CameraDirection = Vector3.Zero;
        private static float CameraYaw = -90f;
        private static float CameraPitch = 0f;
        private static float CameraZoom = 45f;

        //Used to track change in mouse movement to allow for moving of the Camera
        private static Vector2 LastMousePosition;
        #endregion

        private static void Main(string[] args)
        {
            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(800, 600);
            options.Title = "LearnOpenGL with Silk.NET";
            window = Window.Create(options);

            window.Load += OnLoad;
            window.Update += OnUpdate;
            window.Render += OnRender;
            window.FramebufferResize += OnFramebufferResize;
            window.Closing += OnClose;

            window.Run();

            window.Dispose();
        }

        private static void OnLoad()
        {
            IInputContext input = window.CreateInput();
            primaryKeyboard = input.Keyboards.Count > 0 ? input.Keyboards[0] : null;
            
            if (primaryKeyboard != null)
                primaryKeyboard.KeyDown += KeyDown;
            
            for (int i = 0; i < input.Mice.Count; i++)
            {
                input.Mice[i].Cursor.CursorMode = CursorMode.Raw;
                input.Mice[i].MouseMove += OnMouseMove;
                input.Mice[i].Scroll += OnMouseWheel;
            }

            Gl = GL.GetApi(window);

            Shader = new Shader(Gl, PathExtensions.GetAssemblyPath("shader2.vert"), Default.LightShader, "test").Initialize();
            Texture = new Texture(Gl, "silk.png");

            var model = ObjLoaderFactory.Load(Gl, "Untitled2.obj");

            Models.Add(model);
            Models.Add(model);
        }

        private static unsafe void OnUpdate(double deltaTime)
        {
            var moveSpeed = 2.5f * (float) deltaTime;

            if (primaryKeyboard.IsKeyPressed(Key.W))
            {
                //Move forwards
                CameraPosition += moveSpeed * CameraFront;
            }
            if (primaryKeyboard.IsKeyPressed(Key.S))
            {
                //Move backwards
                CameraPosition -= moveSpeed * CameraFront;
            }
            if (primaryKeyboard.IsKeyPressed(Key.A))
            {
                //Move left
                CameraPosition -= Vector3.Normalize(Vector3.Cross(CameraFront, CameraUp)) * moveSpeed;
            }
            if (primaryKeyboard.IsKeyPressed(Key.D))
            {
                //Move right
                CameraPosition += Vector3.Normalize(Vector3.Cross(CameraFront, CameraUp)) * moveSpeed;
            }
        }

        private static unsafe void OnRender(double deltaTime)
        {
            Gl.Enable(EnableCap.DepthTest);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Texture.Use();
            // Shader.Use();
            //Shader.SetInt("uTexture0", 0);

            //Use elapsed time to convert to radians to allow our cube to rotate over time
            var difference = (float) (window.Time * 100);

            var size = window.FramebufferSize;

            var modelMatrix = Matrix4x4.CreateRotationY(SharpEngine.Core.Math.DegreesToRadians(difference)) * 
                              Matrix4x4.CreateRotationX(SharpEngine.Core.Math.DegreesToRadians(difference));

            var view = Matrix4x4.CreateLookAt(CameraPosition, CameraPosition + CameraFront, CameraUp);
            //Note that the apsect ratio calculation must be performed as a float, otherwise integer division will be performed (truncating the result).
            var projection = Matrix4x4.CreatePerspectiveFieldOfView(SharpEngine.Core.Math.DegreesToRadians(CameraZoom), (float)size.X / size.Y, 0.1f, 100.0f);

            for (int i = 0; i < Models.Count; i++)
            {
                if (i >= 1)
                    modelMatrix = Matrix4x4.CreateTranslation(0, -30.0f, 0.0f) * modelMatrix;

                RenderModel(Models[i], modelMatrix, view, projection);
            }
        }

        private static unsafe void RenderModel(Model_Old model, Matrix4x4 modelMatrix, Matrix4x4 view, Matrix4x4 projection)
        {
            foreach (var mesh in model.Meshes)
            {
                mesh.Bind();

                Texture.Use();

                Shader.Use();
                Shader.SetInt("uTexture0", 0);
                Shader.SetMatrix4("uModel", modelMatrix, false);
                Shader.SetMatrix4("uView", view, false);
                Shader.SetMatrix4("uProjection", projection, false);

                Gl.DrawArrays(PrimitiveType.Triangles, 0, (uint)mesh.Vertices.Length);
            }
        }

        private static void OnFramebufferResize(Vector2D<int> newSize)
        {
            Gl.Viewport(newSize);
        }

        private static unsafe void OnMouseMove(IMouse mouse, Vector2 position)
        {
            var lookSensitivity = 0.1f;
            if (LastMousePosition == default)
            {
                LastMousePosition = position;
            }
            else
            {
                var xOffset = (position.X - LastMousePosition.X) * lookSensitivity;
                var yOffset = (position.Y - LastMousePosition.Y) * lookSensitivity;
                LastMousePosition = position;

                CameraYaw += xOffset;
                CameraPitch -= yOffset;

                //We don't want to be able to look behind us by going over our head or under our feet so make sure it stays within these bounds
                CameraPitch = Math.Clamp(CameraPitch, -89.0f, 89.0f);

                CameraDirection.X = MathF.Cos(SharpEngine.Core.Math.DegreesToRadians(CameraYaw)) * MathF.Cos(SharpEngine.Core.Math.DegreesToRadians(CameraPitch));
                CameraDirection.Y = MathF.Sin(SharpEngine.Core.Math.DegreesToRadians(CameraPitch));
                CameraDirection.Z = MathF.Sin(SharpEngine.Core.Math.DegreesToRadians(CameraYaw)) * MathF.Cos(SharpEngine.Core.Math.DegreesToRadians(CameraPitch));
                CameraFront = Vector3.Normalize(CameraDirection);
            }
        }

        private static unsafe void OnMouseWheel(IMouse mouse, ScrollWheel scrollWheel)
        {
            //We don't want to be able to zoom in too close or too far away so clamp to these values
            CameraZoom = Math.Clamp(CameraZoom - scrollWheel.Y, 1.0f, 45f);
        }

        private static void OnClose()
        {
            foreach (var model in Models)
                model.Dispose();

            Shader.Dispose();
            Texture.Dispose();
        }

        private static void KeyDown(IKeyboard keyboard, Key key, int arg3)
        {
            if (key == Key.Escape)
            {
                window.Close();
            }
        }
    }
}
