using Core;
using Core.Interfaces;
using Minecraft.Block;

using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

using System;

namespace Minecraft
{
    public class Game : IGame
    {
        // TODO: Add documentation to the code

        public Camera Camera { get; set; }
        public readonly Settings Settings;
        public ISettings CoreSettings => Settings;

        private Scene _scene;
        private SceneNode _lightsNode;
        private SceneNode _blocksNode;

        private Input _input;

        private Inventory _inventory;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Game"/>.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="settings"></param>
        public Game(Scene scene, Settings settings)
        {
            _scene = scene;
            Settings = settings;
        }

        /// <inheritdoc />
        public void Initialize()
        {
            _input = new Input(Camera);
            _inventory = new Inventory();
            _inventory.Initialize();

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

        /// <inheritdoc />
        public void Update(FrameEventArgs args, KeyboardState keyboardState, MouseState mouseState)
        {

        }

        /// <inheritdoc />
        public void HandleKeyboard(KeyboardState input, float deltaTime)
        {
            _input.HandleKeyboard(input, deltaTime);

            for (int i = 0; i <= 9; i++)
            {
                if (input.IsKeyDown(Keys.D1 + i))
                {
                    _inventory.SetSelectedSlot(i);
                }
            }

            if (input.IsKeyDown(Keys.F))
            {
                Settings.PrintFrameRate = !Settings.PrintFrameRate;
            }
        }

        /// <inheritdoc />
        public void HandleMouse(MouseState mouse)
            => _input.HandleMouse(mouse);

        /// <inheritdoc />
        public void HandleMouseDown(MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Right)
            {
                if (_inventory.SelectedSlot.Items.Type != BlockType.None && _inventory.SelectedSlot.Items.Amount > 0)
                {
                    PlaceBlock();
                    _inventory.SelectedSlot.Items.Amount -= 1;

                    if (_inventory.SelectedSlot.Items.Amount < 0)
                        _inventory.SelectedSlot.Items.Type = BlockType.None;
                }
                else
                {
                    Console.WriteLine($"No more {_inventory.SelectedSlot.Items.Type}s.");
                }
            }

            if (e.Button == MouseButton.Left)
            {
                var destoryedBlockType = DestroyBlock();
                if (destoryedBlockType != BlockType.None)
                {
                    Console.WriteLine($"Block destroyed: {destoryedBlockType}.");
                    _inventory.AddToolbarItem(destoryedBlockType);
                }
            }
        }

        private BlockType DestroyBlock()
        {
            if (!IsBlockInView(out GameObject intersectingObject, out Vector3 _))
                return BlockType.None;

            var block = (BlockBase)intersectingObject;

            _blocksNode.RemoveChild(intersectingObject);
            _scene.Blocks.Remove(block);

            return block.BlockType;
        }

        private void PlaceBlock()
        {
            if (!IsBlockInView(out GameObject intersectingObject, out Vector3 hitPosition))
                return;

            var newBlockPosition = GetNewBlockPosition(hitPosition, intersectingObject);

            if (newBlockPosition == Camera.Position || newBlockPosition == hitPosition)
                return;

            var newBlock = BlockFactory.CreateBlock(_inventory.SelectedSlot.Items.Type, newBlockPosition, $"Dirt ({_blocksNode.Children.Count})");
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

        /// <inheritdoc />
        public void HandleMouseWheel(MouseWheelScrollDirection direction, MouseWheelEventArgs e)
        {
            int slotIndex = _inventory.SelectedSlotIndex;

            if (direction == MouseWheelScrollDirection.Up)
                slotIndex++;
            else if (direction == MouseWheelScrollDirection.Down)
                slotIndex--;

            if (slotIndex < 0)
                slotIndex = 9;
            else if (slotIndex > 9)
                slotIndex = 0;

            _inventory.SetSelectedSlot(slotIndex);
            Console.WriteLine($"Selected slot: {slotIndex}");

        }
    }
}
