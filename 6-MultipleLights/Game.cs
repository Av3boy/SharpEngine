using LearnOpenTK.Common;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Collections.Generic;

namespace LearnOpenTK
{
    public class Game
    {
        public Camera Camera;

        // TODO: Read only once, load into OpenGL buffer once.
        // If already loaded, add mesh indetifier to a dictionary. If dict contains mesh, skip it.
        public float[] Vertices => GetVertices();

        private float[] GetVertices()
        {
            var mesh = Cubes[0].Mesh; // Mesh is identical for all cubes
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

        public readonly GameObject[] Cubes =
        {
            Primitives.Cube.Create(new Vector3(0.0f, 0.0f, 0.0f)),
            Primitives.Cube.Create(new Vector3(2.0f, 5.0f, -15.0f)),
            Primitives.Cube.Create(new Vector3(-1.5f, -2.2f, -2.5f)),
            Primitives.Cube.Create(new Vector3(-3.8f, -2.0f, -12.3f)),
            Primitives.Cube.Create(new Vector3(2.4f, -0.4f, -3.5f)),
            Primitives.Cube.Create(new Vector3(-1.7f, 3.0f, -7.5f)),
            Primitives.Cube.Create(new Vector3(1.3f, -2.0f, -2.5f)),
            Primitives.Cube.Create(new Vector3(1.5f, 2.0f, -2.5f)),
            Primitives.Cube.Create(new Vector3(1.5f, 0.2f, -1.5f)),
            Primitives.Cube.Create(new Vector3(-1.3f, 1.0f, -1.5f))
        };

        public readonly Vector3[] PointLightPositions =
        {
            new Vector3(0.7f, 0.2f, 2.0f),
            new Vector3(2.3f, -3.3f, -4.0f),
            new Vector3(-4.0f, 2.0f, -12.0f),
            new Vector3(0.0f, 0.0f, -3.0f)
        };

        public void HandleMovement(KeyboardState input, float deltaTime)
        {
            const float cameraSpeed = 1.5f;

            if (input.IsKeyDown(Keys.W))
            {
                Camera.Position += Camera.Front * cameraSpeed * deltaTime; // Forward
            }
            if (input.IsKeyDown(Keys.S))
            {
                Camera.Position -= Camera.Front * cameraSpeed * deltaTime; // Backwards
            }
            if (input.IsKeyDown(Keys.A))
            {
                Camera.Position -= Camera.Right * cameraSpeed * deltaTime; // Left
            }
            if (input.IsKeyDown(Keys.D))
            {
                Camera.Position += Camera.Right * cameraSpeed * deltaTime; // Right
            }
            if (input.IsKeyDown(Keys.Space))
            {
                Camera.Position += Camera.Up * cameraSpeed * deltaTime; // Up
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                Camera.Position -= Camera.Up * cameraSpeed * deltaTime; // Down
            }
        }

        public void HandleMouseMovement(MouseState mouse, ref bool firstMove, ref Vector2 lastPos)
        {
            const float sensitivity = 0.2f;

            if (firstMove)
            {
                lastPos = new Vector2(mouse.X, mouse.Y);
                firstMove = false;
            }
            else
            {
                var deltaX = mouse.X - lastPos.X;
                var deltaY = mouse.Y - lastPos.Y;
                lastPos = new Vector2(mouse.X, mouse.Y);

                Camera.Yaw += deltaX * sensitivity;
                Camera.Pitch -= deltaY * sensitivity;
            }
        }
    }
}
