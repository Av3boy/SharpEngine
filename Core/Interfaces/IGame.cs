using Core.Entities;
using Core.Enums;
using Silk.NET.Input;

namespace Core.Interfaces;

/// <summary>
///     Contains definitions the Game class must implement.
///     To consider: move into a abstract class so that the game doesn't necessarily have to implement all methods.
/// </summary>
public interface IGame
{
    /// <summary>
    ///     Gets or sets the core settings of the game.
    /// </summary>
    public ISettings CoreSettings { get; }

    /// <summary>
    ///     Gets or sets the camera of the game.
    /// </summary>
    public Camera Camera { get; set; }

    /// <summary>
    ///     Executed when a mouse button is pressed.
    /// </summary>
    public void HandleMouseDown(IMouse mouse, Silk.NET.Input.MouseButton button);

    /// <summary>
    ///     A method executed when the mouse state is changed
    /// </summary>
    /// <param name="mouse">The state of the mouse on the current frame.</param>
    public void HandleMouse(IMouse mouse);

    /// <summary>
    ///     Gets or sets the event executed when the mouse wheel is scrolled.
    /// </summary>
    /// <param name="direction">The direction the wheel was scrolled.</param>
    /// <param name="scrollWheel">Contains information about the scroll wheel changes during the current frame.</param>
    public void HandleMouseWheel(MouseWheelScrollDirection direction, ScrollWheel scrollWheel);

    /// <summary>
    ///     Executed when a key is pressed.
    /// </summary>
    /// <param name="input">The state of the keyboard on the current frame.</param>
    public void HandleKeyboard(IKeyboard input);

    /// <summary>
    ///     Executed when the game is first loaded.
    /// </summary>
    public void Initialize();

    /// <summary>
    ///    Executed each frame.
    /// </summary>
    public void Update(double deltaTime, IInputContext input);
}
