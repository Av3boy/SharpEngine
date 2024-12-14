using Core;
using Core.Interfaces;
using Minecraft.Block;

using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

using System;
using System.Collections.Generic;

namespace Minecraft
{
    public class Game : IGame
    {
        // TODO: Add documentation to the code

        public Camera Camera { get; set; }
        public readonly Settings Settings;
        public ISettings CoreSettings => Settings;


        // TODO: Get rid of these light properties
        public DirectionalLight DirectionalLight { get; set; }
        public PointLight[] PointLights { get; set; }
        public SpotLight SpotLight { get; set; }

        private Scene _scene;
        private SceneNode _lightsNode;
        private SceneNode _blocksNode;

        private Input _input;

        private Inventory _inventory = new();
        private BlockType SelectedBlockType = BlockType.Dirt;

        public Game(Scene scene, Settings settings)
        {
            _scene = scene;
            Settings = settings;
        }

        public void Initialize()
        {
            _input = new Input(Camera);

            _lightsNode = _scene.Root.AddChild("lights");
            _blocksNode = _scene.Root.AddChild("blocks");

            InitializeWorld();
        }

        private void InitializeWorld()
        {
            InitializeLights();
            InitializeChunks();
        }

        private void InitializeLights()
        {
            _lightsNode.AddChild(new DirectionalLight());

            _lightsNode.AddChild(
                new PointLight(new Vector3(0.7f, 0.2f, 2.0f), 0),
                new PointLight(new Vector3(2.3f, -3.3f, -4.0f), 1),
                new PointLight(new Vector3(-4.0f, 2.0f, -12.0f), 2),
                new PointLight(new Vector3(0.0f, 0.0f, -3.0f), 3)
            );

            _lightsNode.AddChild(new SpotLight()
            {
                Ambient = new Vector3(0.0f, 0.0f, 0.0f),
                Diffuse = new Vector3(1.0f, 1.0f, 1.0f),
                Specular = new Vector3(1.0f, 1.0f, 1.0f),
            });

            // var _lightingShader = ShaderService.Instance.LoadShader("Shaders/shader.vert", "Shaders/lighting.frag", "lighting");
            // _lightingShader.SetInt("numDirLights", 1);
            // _lightingShader.SetInt("numPointLights", 4);
            // _lightingShader.SetInt("numSpotLights", 1);
        }

        private void InitializeChunks()
        {
            // TODO: Generate chunks when player moves

            // TODO: Generate chunks using 3d perlin noise

            const int chunkSize = 16;
            const int numChunks = 3;

            for (int i = 0; i < numChunks; i++)
            {
                var chunkPos = new Vector3(i * chunkSize, 0, 0);
                GenerateChunk(chunkSize, chunkPos);
            }
        }

        private void GenerateChunk(int chunkSize, Vector3 chunkPos)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    var blockPos = chunkPos + new Vector3(x, 0, z);

                    var dirt = new Dirt(blockPos, $"Dirt ({x}{z})");
                    _blocksNode.AddChild(dirt);
                    _scene.Blocks.Add(dirt);

                    for (int y = 1; y < chunkSize; y++)
                    {
                        blockPos.Y = -y;
                        var stone = new Stone(blockPos, $"Dirt ({x}{z}.{y})");
                        _blocksNode.AddChild(stone);
                        _scene.Blocks.Add(stone);
                    }
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

            if (input.IsKeyDown(Keys.D2))
            {
                SelectedBlockType = BlockType.Dirt;
            }

            if (input.IsKeyDown(Keys.F))
            {
                Settings.PrintFrameRate = !Settings.PrintFrameRate;
            }
        }

        public void HandleMouse(MouseState mouse)
            => _input.HandleMouse(mouse);

        public void HandleMouseDown(MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Right)
            {
                if (_inventory.Blocks.GetValueOrDefault(SelectedBlockType) > 0)
                {
                    PlaceBlock();
                    _inventory.Blocks[SelectedBlockType] -= 1;
                }
                else
                {
                    Console.WriteLine($"No more {SelectedBlockType}s.");
                }
            }

            if (e.Button == MouseButton.Left)
            {
                var destoryedBlockType = DestroyBlock();
                if (destoryedBlockType != BlockType.None)
                {
                    Console.WriteLine($"Block destroyed: {destoryedBlockType}.");
                    if (_inventory.Blocks.ContainsKey(destoryedBlockType))
                        _inventory.Blocks[destoryedBlockType] += 1;
                    else
                        _inventory.Blocks.Add(destoryedBlockType, 1);
                }
            }
        }

        private BlockType DestroyBlock()
        {
            if (!IsBlockInView(out GameObject intersectingObject, out Vector3 _))
                return BlockType.None;

            _blocksNode.RemoveChild(intersectingObject);
            _scene.Blocks.Remove((BlockBase)intersectingObject);

            return ((BlockBase)intersectingObject).BlockType;
        }

        private void PlaceBlock()
        {
            if (!IsBlockInView(out GameObject intersectingObject, out Vector3 hitPosition))
                return;

            var newBlockPosition = GetNewBlockPosition(hitPosition, intersectingObject);

            if (newBlockPosition == Camera.Position || newBlockPosition == hitPosition)
                return;

            var newBlock = BlockFactory.CreateBlock(SelectedBlockType, newBlockPosition, $"Dirt ({_blocksNode.Children.Count})");
            _blocksNode.AddChild(newBlock);
            _scene.Blocks.Add(newBlock);

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
