using Core.Entities;
using Core.Enums;
using Core.Interfaces;
using SharpEngine.Core.Interfaces;
using SharpEngine.Core.Scenes;
using Silk.NET.Input;

namespace SharpEngine.Editor;

public class Editor : IGame
{
    public ISettings CoreSettings { get; } = new EditorSettings();

    public CameraView Camera { get; set; }

    public void HandleKeyboard(IKeyboard input, double deltaTime)
    {

    }
    public void HandleMouse(IMouse mouse)
    {

    }
    public void HandleMouseDown(IMouse mouse, MouseButton button)
    {

    }
    public void HandleMouseWheel(MouseWheelScrollDirection direction, ScrollWheel scrollWheel)
    {

    }
    public void Initialize()
    {

    }

    public void Update(double deltaTime, IInputContext input)
    {

    }

    public void SaveScene(Scene scene)
    {

    }

    public void LoadScene(string sceneFile)
    {
    }
}