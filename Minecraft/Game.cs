using Core;
using Minecraft.Block;

using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

using System;
using System.Collections.Generic;
using System.Drawing;
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
        private Input _input;

        private BlockType SelectedBlockType = BlockType.Dirt;

        public Game(Scene scene)
        {
            _scene = scene;
        }

        public void Initialize()
        {
            _input = new Input(Camera);

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

        public void Update(FrameEventArgs args, KeyboardState keyboardState, MouseState mouseState)
        {

        }

        public void HandleKeyboard(KeyboardState input, float deltaTime)
        {
            _input.HandleKeyboard(input, deltaTime);

            if (input.IsKeyDown(Keys.D1))
            {
                SelectedBlockType = BlockType.Stone;
            }
        }

        public void HandleMouse(MouseState mouse)
            => _input.HandleMouse(mouse);

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

            var newBlock = BlockFactory.CreateBlock(SelectedBlockType, newBlockPosition, $"Dirt ({_scene.Root.Children.Count})");
            _scene.AddNode(newBlock);

            Console.WriteLine($"New block created: {newBlock.Position}, block in view location: {intersectingObject.Position}");

        }

        private static Vector3 GetNewBlockPosition(Vector3 hitPosition, GameObject intersectingObject)
        {
            Vector3 normal = Ray.GetClosestFaceNormal(hitPosition, intersectingObject);
            return intersectingObject.Position + (normal * intersectingObject.Scale);
        }

        public bool IsBlockInView(out GameObject intersectingObject, out Vector3 hitPosition)
        {
            Ray ray = new Ray(Camera.Position, Camera.Front);
            return ray.IsBlockInView(_scene, out intersectingObject, out hitPosition);
        }
    }
}