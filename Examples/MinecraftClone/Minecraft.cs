using ImGuiNET;
using Microsoft.Extensions.Logging;
using Minecraft.Terrain.Block;
using Minecraft.Terrain.Layers;
using SharpEngine.Core;
using SharpEngine.Core.Entities;
using SharpEngine.Core.Entities.UI;
using SharpEngine.Core.Enums;
using SharpEngine.Core.Interfaces;
using SharpEngine.Core.Numerics;
using SharpEngine.Core.Scenes;
using SharpEngine.Core.Windowing;
using Silk.NET.Input;
using System;

namespace Minecraft;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Justification = Those values are set in the Initialize method.

/// <summary>
///     Contains the logic for the Minecraft game.
/// </summary>
public class Minecraft : Game
{
    private readonly Scene _scene;
    private readonly ILogger<Minecraft> _logger;

    private Input _input;
    private readonly Inventory _inventory;

    private UIElement _uiElem;

    private readonly Terrain.TerrainGenerator_New _terrain;
    private readonly SceneNode _blocksNode;

    /// <summary>
    ///     Gets the main window.
    /// </summary>
    public static Window Window
    {
        get => field ?? throw new InvalidOperationException("The game window has not been assigned yet.");
        set;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Minecraft"/>.
    /// </summary>
    public Minecraft(Scene scene, ISettings settings, ILogger<Minecraft> logger)
    {
        _scene = scene;
        CoreSettings = settings;
        _logger = logger;

        _inventory = new Inventory();

        _blocksNode = _scene.AddNode("Blocks");
        _terrain = new Terrain.TerrainGenerator_New(_scene, _blocksNode, 
        [
            new HeightMapPass(),
            new BaseTerrainPass(),
            new CavePass(),
            // new OrePass(),
            // new WaterPass(),
            // new SurfaceDecorationPass(),
            // new FoliagePass()
        ]);
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        try
        {
            base.Initialize();

            _input = new Input(Camera);
            _inventory.Initialize();

            _terrain.InitializeWorld();

            InitializeUI();
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "{Message}", ex.Message);
        }
    }

    private void InitializeUI()
    {
        // var gridLayout = new GridLayout<UIElement>();

        // TODO: #89 Fix UI renderer
        _uiElem = new UIElement(Window.GetGL(), "uiElement");
        _scene.UIElements.Add(_uiElem);

        var uiElem2 = new UIElement(Window.GetGL(), "uiElement");
        uiElem2.Transform.Scale = new Vector2(0.2f, 0.2f);
        uiElem2.Transform.Position = new Vector2(30, 0);

        // gridLayout.AddChild(_uiElem, uiElem2);
        _scene.UIElements.Add(_uiElem);
        _scene.UIElements.Add(uiElem2);

        // _scene.UIElements.Add(gridLayout);
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
        ImGui.SliderFloat("width", ref width, 0, Window.Width);

        ImGui.Text($"UI Element height: {_uiElem.Height}");
        ImGui.SliderFloat("height", ref height, 0, Window.Height);

        ImGui.End();

        _uiElem.Transform.Position = new SharpEngine.Core.Numerics.Vector2(x, y);
        _uiElem.Transform.Scale = new SharpEngine.Core.Numerics.Vector2(sx, sy);
        _uiElem.Transform.Rotation.Angle = rotation;

        _uiElem.Height = height;
        _uiElem.Width = width;
    }

    /// <inheritdoc />
    public override void Update(double deltaTime, IInputContext input) 
        => _input.HandleKeyboard(input.Keyboards[0], (float)deltaTime);

    // TODO: #21 Input system to let users change change key bindings?
    /// <inheritdoc />
    public override void HandleKeyboard(IKeyboard input, double deltaTime)
    {
        for (int i = 1; i <= 9; i++)
        {
            if (input.IsKeyPressed(Key.Number0 + i))
            {
                _inventory.SetSelectedSlot(i);
                _logger.LogInformation("Selected slot: {I} ({Type})", i, _inventory.SelectedSlot.Items.Type);
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
    public override void HandleMouseDown(IMouse mouse, MouseButton button) 
    {
        if (button == MouseButton.Right)
        {
            if (_inventory.SelectedSlot.Items.Type != BlockId.Air && _inventory.SelectedSlot.Items.Amount > 0)
            {
                PlaceBlock();
                _inventory.SelectedSlot.Items.Amount -= 1;

                if (_inventory.SelectedSlot.Items.Amount < 0)
                    _inventory.SelectedSlot.Items.Type = BlockId.Air;
            }
            else
            {
                _logger.LogInformation("No more {Type}s.", _inventory.SelectedSlot.Items.Type);
            }
        }

        if (button == MouseButton.Left)
        {
            var destroyedBlockId = DestroyBlock();
            if (destroyedBlockId != BlockId.Air)
            {
                // TODO: #86 The block should be added to the slot so that 0 is the last slot instead of 9.
                // TODO: #86 The first block destroyed doesn't seem to be added to the inventory.
                _logger.LogInformation("Block destroyed: {DestroyedBlockId}.", destroyedBlockId);
                _inventory.AddToolbarItem(destroyedBlockId);
            }
        }
    }

    private BlockId DestroyBlock()
    {
        if (!Camera.IsInView(_scene, out GameObject? intersectingObject, out _, allowedTypes: typeof(BlockBase)))
            return BlockId.Air;

        var block = (BlockBase)intersectingObject!;
        _blocksNode.RemoveChild(intersectingObject!);

        return block.BlockId;
    }

    private void PlaceBlock()
    {
        if (!Camera.IsInView(_scene, out GameObject? intersectingObject, out System.Numerics.Vector3 hitPosition, allowedTypes: typeof(BlockBase)))
            return;

        var newBlockPosition = (System.Numerics.Vector3)GetNewBlockPosition((Vector3)hitPosition, intersectingObject!);
        if (newBlockPosition == Camera.Position || newBlockPosition == hitPosition)
            return;

        var newBlock = BlockFactory.CreateBlock(_inventory.SelectedSlot.Items.Type, (Vector3)newBlockPosition, $"Dirt ({_blocksNode.Children.Count})");
        _blocksNode.AddChild(newBlock);

        _logger.LogInformation("New block created: {Pos}, block in view location: {IntersectingPos}", newBlock.Transform.Position, intersectingObject!.Transform.Position);
    }

    private static Vector3 GetNewBlockPosition(Vector3 hitPosition, GameObject intersectingObject)
    {
        Vector3 normal = (Vector3)Ray.GetClosestFaceNormal(hitPosition, intersectingObject);
        return intersectingObject.Transform.Position + (normal * intersectingObject.Transform.Scale);
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
        _logger.LogInformation("Selected slot: {Index}", slotIndex);

    }
}
