using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Core.Interfaces;

public interface IGame
{
    public ISettings CoreSettings { get; }

    public Camera Camera { get; set; }

    public void HandleMouseDown(MouseButtonEventArgs e);
    public void HandleMouse(MouseState mouse);
    public void HandleKeyboard(KeyboardState input, float deltaTime);
    public void Initialize();
    public void Update(FrameEventArgs args, KeyboardState keyboardState, MouseState mouseState);
}
