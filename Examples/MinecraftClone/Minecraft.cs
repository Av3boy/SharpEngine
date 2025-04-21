using ImGuiNET;
using Minecraft.Block;
using ObjLoader.Loaders.ObjLoader;
using SharpEngine.Core;
using SharpEngine.Core.Entities;
using SharpEngine.Core.Entities.Lights;
using SharpEngine.Core.Entities.Properties;
using SharpEngine.Core.Entities.Properties.Meshes;
using SharpEngine.Core.Entities.UI;
using SharpEngine.Core.Entities.UI.Layouts;
using SharpEngine.Core.Enums;
using SharpEngine.Core.Interfaces;
using SharpEngine.Core.Scenes;
using SharpEngine.Core.Windowing;
using SharpEngine.Shared;
using Silk.NET.Core.Native;
using Silk.NET.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Tutorial;

namespace Minecraft;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Justification = Those values are set in the Initialize method.

/// <summary>
///     Contains the logic for the Minecraft game.
/// </summary>
public class Minecraft : Game
{
    private readonly Scene _scene;

    private SceneNode _lightsNode;
    private SceneNode _blocksNode;

    private Input _input;
    private readonly Inventory _inventory;

    private UIElement _uiElem;

    /// <summary>
    ///     Gets the main window.
    /// </summary>
    public Window? Window { get; set; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Minecraft"/>.
    /// </summary>
    public Minecraft(Scene scene, ISettings settings)
    {
        _scene = scene;
        CoreSettings = settings;

        _inventory = new Inventory();

    }

    /// <inheritdoc />
    public override void Initialize()
    {
        try
        {
            base.Initialize();

            _input = new Input(Camera);
            _inventory.Initialize();

            _lightsNode = _scene.Root.AddChild("lights");
            _blocksNode = _scene.Root.AddChild("blocks");

            var gridLayout = new GridLayout<UIElement>();

            // TODO: #89 Fix UI renderer
            _uiElem = new UIElement("uiElement");
            _scene.UIElements.Add(_uiElem);

            //var uiElem2 = new UIElement("uiElement");
            //uiElem2.Transform.Scale = new SharpEngine.Core.Numerics.Vector2(0.2f, 0.2f);
            //uiElem2.Transform.Position = new Vector2(30, 0);

            // gridLayout.AddChild(_uiElem, uiElem2);
            // _scene.UIElements.Add(_uiElem);
            // _scene.UIElements.Add(uiElem2);

            //_scene.UIElements.Add(gridLayout);

            InitializeWorld();
        }
        catch (Exception ex)
        {
            Debug.Log.Information(ex.Message, "{Message}", ex.Message);
        }
    }

    /// <summary>
    ///     Handles rendering after the frame is drawn.
    /// </summary>
    /// <param name="frame">Information about the frame.</param>
    public void OnAfterRender(Frame frame)
    {
        var x = _uiElem.Transform.Position.X;
        var y = _uiElem.Transform.Position.Y;

        var sx = _uiElem.Transform.Scale.X;
        var sy = _uiElem.Transform.Scale.Y;

        var rotation = _uiElem.Transform.Rotation.Angle;

        var height = _uiElem.Height;
        var width = _uiElem.Width;

        ImGui.Begin("Debug");
        ImGui.Text($"FPS: {frame.FrameRate}");
        ImGui.Text($"Camera position: {Camera.Position}");
        ImGui.Text($"UI Element position: {_uiElem.Transform.Position}");

        ImGui.SliderFloat("X", ref x, -2000, 2000);
        ImGui.SliderFloat("Y", ref y, -2000, 2000);

        ImGui.Text($"UI Element scale: {_uiElem.Transform.Scale}");

        ImGui.SliderFloat("sX", ref sx, 0, 1);
        ImGui.SliderFloat("sY", ref sy, 0, 1);

        ImGui.Text($"UI Element scale: {_uiElem.Transform.Scale}");
        ImGui.SliderFloat("rotation", ref rotation, 0, 360);

        ImGui.Text($"UI Element width: {_uiElem.Width}");
        ImGui.SliderFloat("width", ref width, 0, 100);

        ImGui.Text($"UI Element height: {_uiElem.Height}");
        ImGui.SliderFloat("height", ref height, 0, 100);

        ImGui.End();

        _uiElem.Transform.Position = new SharpEngine.Core.Numerics.Vector2(x, y);
        _uiElem.Transform.Scale = new SharpEngine.Core.Numerics.Vector2(sx, sy);
        _uiElem.Transform.Rotation.Angle = rotation;

        _uiElem.Height = height;
        _uiElem.Width = width;
    }

    private void InitializeWorld()
    {
        InitializeLights();
        InitializeChunks();

        // TODO: #2 Does not work yet.
        // var torus = MeshService.Instance.LoadMesh("torus", @"C:\Users\antti\Documents\Untitled2.obj");

        var model = ObjLoaderFactory.Load(Window.GL, @"C:\Users\antti\Documents\Untitled2.obj");
        var go = new GameObject(model);
        var go2 = new GameObject(model)
        {
            Transform = new Transform(new SharpEngine.Core.Numerics.Vector3(0, 0, 30))
        };

        _scene.Root.AddChild(go);
        _scene.Root.AddChild(go2);
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
    }

    private void InitializeChunks()
    {
        // TODO: #88 Generate chunks when player moves

        // TODO: #87 Generate chunks using 3d Perlin noise

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

                for (int y = 1; y < chunkSize; y++)
                {
                    blockPos.Y = -y;
                    var stone = new Stone(blockPos, $"Dirt ({x}{z}.{y})");
                    _blocksNode.AddChild(stone);
                }
            }
        }
    }

    /// <inheritdoc />
    public override void Update(double deltaTime, IInputContext input)
    {
        //UpdateUI();
        _input.HandleKeyboard(input.Keyboards[0], (float)deltaTime);
    }

    private void UpdateUI()
        => _uiElem.Transform.Rotation.Angle += 0.01f;

    // TODO: #21 Input system to let users change change key bindings?
    /// <inheritdoc />
    public override void HandleKeyboard(IKeyboard input, double deltaTime)
    {
        for (int i = 0; i <= 9; i++)
        {
            if (input.IsKeyPressed(Key.Number0 + i))
            {
                _inventory.SetSelectedSlot(i);
                Debug.Log.Information($"Selected slot: {i} ({_inventory.SelectedSlot.Items.Type})");
            }
        }

        input.KeyUp += Input_KeyUp;
    }

    private void Input_KeyUp(IKeyboard arg1, Key arg2, int arg3)
    {
        if (arg2 == Key.F)
        {
            CoreSettings.PrintFrameRate = !CoreSettings.PrintFrameRate;
        }

        if (arg2 == Key.L)
        {
            CoreSettings.UseWireFrame = !CoreSettings.UseWireFrame;
        }
    }

    /// <inheritdoc />
    public override void HandleMouse(IMouse mouse) { }

    /// <inheritdoc />
    public override void HandleMouseDown(IMouse mouse, MouseButton button) 
    {
        if (button == MouseButton.Right)
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
                Debug.Log.Information($"No more {_inventory.SelectedSlot.Items.Type}s.");
            }
        }

        if (button == MouseButton.Left)
        {
            var destroyedBlockType = DestroyBlock();
            if (destroyedBlockType != BlockType.None)
            {
                // TODO: #86 The block should be added to the slot so that 0 is the last slot instead of 9.
                // TODO: #86 The first block destroyed doesn't seem to be added to the inventory.
                Debug.Log.Information($"Block destroyed: {destroyedBlockType}.");
                _inventory.AddToolbarItem(destroyedBlockType);
            }
        }
    }

    private BlockType DestroyBlock()
    {
        if (!Camera.IsInView(_scene, out GameObject? intersectingObject, out Vector3 _, allowedTypes: typeof(BlockBase)))
            return BlockType.None;

        var block = (BlockBase)intersectingObject!;
        _blocksNode.RemoveChild(intersectingObject!);

        return block.BlockType;
    }

    private void PlaceBlock()
    {
        if (!Camera.IsInView(_scene, out GameObject? intersectingObject, out Vector3 hitPosition, allowedTypes: typeof(BlockBase)))
            return;

        var newBlockPosition = GetNewBlockPosition(hitPosition, intersectingObject!);
        if (newBlockPosition == Camera.Position || newBlockPosition == hitPosition)
            return;

        var newBlock = BlockFactory.CreateBlock(_inventory.SelectedSlot.Items.Type, newBlockPosition, $"Dirt ({_blocksNode.Children.Count})");
        _blocksNode.AddChild(newBlock);

        Debug.Log.Information($"New block created: {newBlock.Transform.Position}, block in view location: {intersectingObject!.Transform.Position}");

    }

    private static Vector3 GetNewBlockPosition(Vector3 hitPosition, GameObject intersectingObject)
    {
        Vector3 normal = Ray.GetClosestFaceNormal(hitPosition, intersectingObject);
        return (System.Numerics.Vector3)intersectingObject.Transform.Position + (normal * (System.Numerics.Vector3)intersectingObject.Transform.Scale);
    }

    /// <inheritdoc />
    public override void HandleMouseWheel(MouseWheelScrollDirection direction, ScrollWheel scrollWheel)
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
        Debug.Log.Information($"Selected slot: {slotIndex}");

    }
}
