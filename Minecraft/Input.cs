using Core.Entities;
using Silk.NET.Input;

namespace Minecraft;

public class Input
{
    public float MouseSensitivity
    {
        get => _camera.Sensitivity;
        set => _camera.Sensitivity = value;
    }

    public float MovementSpeed { get; set; } = 1.5f;

    private readonly Camera _camera;

    public Input(Camera camera)
    {
        _camera = camera;
    }

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

    public void HandleMouse(IMouse mouse)
    {
    }

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
