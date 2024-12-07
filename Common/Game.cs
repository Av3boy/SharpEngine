using LearnOpenTK.Common;

using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

using System;
using System.Collections.Generic;
using System.Linq;

namespace LearnOpenTK
{
    public class Game : IGame
    {
        public Camera Camera { get; set; }
        public DirectionalLight DirectionalLight { get; set; }
        public PointLight[] PointLights { get; set; }
        public SpotLight SpotLight { get; set; }
        public List<GameObject> Cubes { get; set; } = new();

        public readonly Vector3[] PointLightPositions =
        {
            new Vector3(0.7f, 0.2f, 2.0f),
            new Vector3(2.3f, -3.3f, -4.0f),
            new Vector3(-4.0f, 2.0f, -12.0f),
            new Vector3(0.0f, 0.0f, -3.0f)
        };

        private Scene _scene;

        public Game(Scene scene)
        {
            _scene = scene;
            InitializeLights();
            InitializeCubes();
        }

        private void InitializeLights()
        {
            DirectionalLight = new DirectionalLight
            {
                Direction = new Vector3(-0.2f, -1.0f, -0.3f),
                Ambient = new Vector3(0.05f, 0.05f, 0.05f),
                Diffuse = new Vector3(0.4f, 0.4f, 0.4f),
                Specular = new Vector3(0.5f, 0.5f, 0.5f)
            };

            PointLights = new PointLight[PointLightPositions.Length];
            for (int i = 0; i < PointLightPositions.Length; i++)
            {
                PointLights[i] = new PointLight
                {
                    Position = PointLightPositions[i],
                    Ambient = new Vector3(0.05f, 0.05f, 0.05f),
                    Diffuse = new Vector3(0.8f, 0.8f, 0.8f),
                    Specular = new Vector3(1.0f, 1.0f, 1.0f),
                    Constant = 1.0f,
                    Linear = 0.09f,
                    Quadratic = 0.032f
                };
            }

            SpotLight = new SpotLight
            {
                Ambient = new Vector3(0.0f, 0.0f, 0.0f),
                Diffuse = new Vector3(1.0f, 1.0f, 1.0f),
                Specular = new Vector3(1.0f, 1.0f, 1.0f),
                Constant = 1.0f,
                Linear = 0.09f,
                Quadratic = 0.032f,
                CutOff = MathF.Cos(MathHelper.DegreesToRadians(12.5f)),
                OuterCutOff = MathF.Cos(MathHelper.DegreesToRadians(17.5f))
            };
        }

        private void InitializeCubes()
        {
            int chunkSize = 16;

            for (int x = 0; x < chunkSize; x++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    var cube = Primitives.Cube.Create(new Vector3(x, 0, z));

                    Cubes.Add(cube);
                    _scene.Nodes.Add(cube);
                }
            }
        }

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