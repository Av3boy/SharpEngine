using SharpEngine.Core.Entities.Views;
using Silk.NET.Input;

namespace Minecraft;

/// <summary>
///     Handles input for the game.
/// </summary>
public class Input
{
    /// <summary>Gets or sets the mouse sensitivity of the camera.</summary>
    public float MouseSensitivity
    {
        get => _camera.Sensitivity;
        set => _camera.Sensitivity = value;
    }

    /// <summary>Gets or sets the movement speed of the camera.</summary>
    public float MovementSpeed { get; set; } = 1.5f;

    private readonly CameraView _camera;

    /// <summary>
    ///     Initializes a new instance of <see cref="Input"/>.
    /// </summary>
    /// <param name="camera">The camera controlled by the input.</param>
    public Input(CameraView camera)
    {
        _camera = camera;
    }

    /// <summary>
    ///     Handles the keyboard input.
    /// </summary>
    /// <param name="input">The button pressed.</param>
    /// <param name="deltaTime">Time since the last frame.</param>
    public void HandleKeyboard(IKeyboard input, float deltaTime)
    {
        if (input.IsKeyPressed(Key.W))
        {
            _camera.Position += _camera.Front * MovementSpeed * deltaTime; // Forward
        }
        
        if (input.IsKeyPressed(Key.S))
        {
            _camera.Position -= _camera.Front * MovementSpeed * deltaTime; // Backwards
        }
        
        if (input.IsKeyPressed(Key.A))
        {
            _camera.Position -= _camera.Right * MovementSpeed * deltaTime; // Left
        }
        
        if (input.IsKeyPressed(Key.D))
        {
            _camera.Position += _camera.Right * MovementSpeed * deltaTime; // Right
        }
        
        if (input.IsKeyPressed(Key.Space))
        {
            _camera.Position += _camera.Up * MovementSpeed * deltaTime; // Up
        }
        
        if (input.IsKeyPressed(Key.ShiftLeft))
        {
            _camera.Position -= _camera.Up * MovementSpeed * deltaTime; // Down
        }
    }

    /// <summary>
    ///     Determines whether a number key was pressed.
    /// </summary>
    /// <param name="key">The key pressed.</param>
    /// <param name="number">Output the number pressed. -1 if the key was not a number.</param>
    /// <returns><see langword="true"/> if the key was a number key; otherwise, <see langword="false"/>.</returns>
    public static bool IsInputNumber(Key key, out int number)
    {
        number = -1;
        if (key is >= Key.Number0 and <= Key.Number9)
        {
            number = key - Key.Number0;
            return true;
        }

        return false;
    }
}
