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
    public void HandleMouseMovement(MouseState mouse, ref bool firstMove, ref Vector2 lastPos);
    public void HandleMovement(KeyboardState input, float deltaTime);
}
