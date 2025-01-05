using Core.Entities;
using OpenTK.Windowing.GraphicsLibraryFramework;

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

    public void HandleKeyboard(KeyboardState input, float deltaTime)
    {
        if (input.IsKeyDown(Keys.W))
        {
            _camera.Position += _camera.Front * MovementSpeed * deltaTime; // Forward
        }
        
        if (input.IsKeyDown(Keys.S))
        {
            _camera.Position -= _camera.Front * MovementSpeed * deltaTime; // Backwards
        }
        
        if (input.IsKeyDown(Keys.A))
        {
            _camera.Position -= _camera.Right * MovementSpeed * deltaTime; // Left
        }
        
        if (input.IsKeyDown(Keys.D))
        {
            _camera.Position += _camera.Right * MovementSpeed * deltaTime; // Right
        }
        
        if (input.IsKeyDown(Keys.Space))
        {
            _camera.Position += _camera.Up * MovementSpeed * deltaTime; // Up
        }
        
        if (input.IsKeyDown(Keys.LeftShift))
        {
            _camera.Position -= _camera.Up * MovementSpeed * deltaTime; // Down
        }
    }

    public void HandleMouse(MouseState mouse)
    {
    }

    public static bool IsInputNumber(Keys key, out int number)
    {
        number = -1;
        if (key is >= Keys.D0 and <= Keys.D9)
        {
            number = key - Keys.D0;
            return true;
        }

        return false;
    }
}
