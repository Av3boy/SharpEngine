using LearnOpenTK.Common;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Collections.Generic;

namespace LearnOpenTK;

public interface IGame
{
    public DirectionalLight DirectionalLight { get; }
    public PointLight[] PointLights { get; }
    public SpotLight SpotLight { get; }
    public List<GameObject> Cubes { get; set; }

    public Camera Camera { get; set; }

    void HandleMouseMovement(MouseState mouse, ref bool firstMove, ref Vector2 lastPos);
    void HandleMovement(KeyboardState input, float deltaTime);
}
