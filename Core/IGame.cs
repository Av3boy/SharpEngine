using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

using System.Collections.Generic;

namespace Core;

public interface IGame
{
    public DirectionalLight DirectionalLight { get; }
    public PointLight[] PointLights { get; }
    public SpotLight SpotLight { get; }

    public Camera Camera { get; set; }

    public void HandleMouseDown(MouseButtonEventArgs e);
    public void HandleMouse(MouseState mouse);
    public void HandleKeyboard(KeyboardState input, float deltaTime);
    public void Initialize();
    public void Update(FrameEventArgs args, KeyboardState keyboardState, MouseState mouseState);
}
