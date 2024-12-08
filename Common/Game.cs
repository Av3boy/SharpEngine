using Core;
using Minecraft.Block;

using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Minecraft
{
    public class Game : IGame
    {
        public Camera Camera { get; set; }
        public DirectionalLight DirectionalLight { get; set; }
        public PointLight[] PointLights { get; set; }
        public SpotLight SpotLight { get; set; }

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

            PointLights =
            [
                new()
                {
                    Position = new Vector3(0.7f, 0.2f, 2.0f),
                    Ambient = new Vector3(0.05f, 0.05f, 0.05f),
                    Diffuse = new Vector3(0.8f, 0.8f, 0.8f),
                    Specular = new Vector3(1.0f, 1.0f, 1.0f),
                    Constant = 1.0f,
                    Linear = 0.09f,
                    Quadratic = 0.032f
                },
                new()
                {
                    Position = new Vector3(2.3f, -3.3f, -4.0f),
                    Ambient = new Vector3(0.05f, 0.05f, 0.05f),
                    Diffuse = new Vector3(0.8f, 0.8f, 0.8f),
                    Specular = new Vector3(1.0f, 1.0f, 1.0f),
                    Constant = 1.0f,
                    Linear = 0.09f,
                    Quadratic = 0.032f
                },
                new()
                {
                    Position = new Vector3(-4.0f, 2.0f, -12.0f),
                    Ambient = new Vector3(0.05f, 0.05f, 0.05f),
                    Diffuse = new Vector3(0.8f, 0.8f, 0.8f),
                    Specular = new Vector3(1.0f, 1.0f, 1.0f),
                    Constant = 1.0f,
                    Linear = 0.09f,
                    Quadratic = 0.032f
                },
                new()
                {
                    Position = new Vector3(0.0f, 0.0f, -3.0f),
                    Ambient = new Vector3(0.05f, 0.05f, 0.05f),
                    Diffuse = new Vector3(0.8f, 0.8f, 0.8f),
                    Specular = new Vector3(1.0f, 1.0f, 1.0f),
                    Constant = 1.0f,
                    Linear = 0.09f,
                    Quadratic = 0.032f
                }
            ];

            SpotLight = new()
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
                    var dirt = new Dirt(new Vector3(x, 0, z), $"Dirt ({x}{z})");
                    _scene.AddNode(dirt);
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

        public void HandleMouseDown(MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Right)
            {
                PlaceBlock();
            }

            if (e.Button == MouseButton.Left)
            {
                DestroryBlock();
            }
        }

        private void DestroryBlock()
        {
            if (!IsBlockInView(out GameObject intersectingObject, out Vector3 _))
                return;

            _scene.RemoveNode(intersectingObject);
        }

        private void PlaceBlock()
        {
            if (!IsBlockInView(out GameObject intersectingObject, out Vector3 hitPosition))
                return;

            var newBlockPosition = GetNewBlockPosition(hitPosition, intersectingObject);

            if (newBlockPosition == Camera.Position || newBlockPosition == hitPosition)
                return;

            var newBlock = BlockFactory.CreateBlock(BlockType.Dirt, newBlockPosition, $"Dirt ({_scene.Root.Children.Count})"); // TODO:
            _scene.AddNode(newBlock);

            Console.WriteLine($"New block created: {newBlock.Position}, block in view location: {intersectingObject.Position}");

        }

        private Vector3 GetNewBlockPosition(Vector3 hitPosition, GameObject intersectingObject)
        {
            Vector3 normal = GetFaceNormal(hitPosition, intersectingObject);
            return intersectingObject.Position + normal * intersectingObject.Scale;
        }

        private Vector3 GetFaceNormal(Vector3 point, GameObject obj)
        {
            Vector3 min = obj.Position - (obj.Scale / 2);
            Vector3 max = obj.Position + (obj.Scale / 2);

            var distances = new Dictionary<Vector3, float>
            {
                { -Vector3.UnitX, Math.Abs(point.X - min.X) },
                { Vector3.UnitX, Math.Abs(point.X - max.X) },
                { -Vector3.UnitY, Math.Abs(point.Y - min.Y) },
                { Vector3.UnitY, Math.Abs(point.Y - max.Y) },
                { -Vector3.UnitZ, Math.Abs(point.Z - min.Z) },
                { Vector3.UnitZ, Math.Abs(point.Z - max.Z) }
            };

            return distances.OrderBy(d => d.Value).First().Key;
        }

        public bool IsBlockInView(out GameObject intersectingObject, out Vector3 hitPosition)
        {
            Vector3 rayOrigin = Camera.Position;
            Vector3 rayDirection = Camera.Front;

            const float maxDistance = 100.0f; // Maximum distance to check for intersections
            const float stepSize = 0.1f; // Step size for ray marching

            for (float t = 0; t < maxDistance; t += stepSize)
            {
                Vector3 currentPosition = rayOrigin + (t * rayDirection);

                intersectingObject = _scene.Blocks.FirstOrDefault(obj => IsPointInsideObject(currentPosition, obj));
                if (intersectingObject != null)
                {
                    hitPosition = currentPosition;
                    return true;
                }
            }

            intersectingObject = null;
            hitPosition = Vector3.Zero;
            return false;
        }

        private static bool IsPointInsideObject(Vector3 point, GameObject obj)
        {
            Vector3 min = obj.Position - (obj.Scale / 2);
            Vector3 max = obj.Position + (obj.Scale / 2);

            return point.X >= min.X && point.X <= max.X &&
                   point.Y >= min.Y && point.Y <= max.Y &&
                   point.Z >= min.Z && point.Z <= max.Z;
        }
    }
}
