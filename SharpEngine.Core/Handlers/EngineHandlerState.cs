/// <summary>
///     Represents the lifecycle state of an engine handler.
/// </summary>
public enum EngineHandlerState
{
    /// <summary>
    ///     The engine handler has not been started yet.
    /// </summary>
    NotStarted,

    /// <summary>
    ///     The engine handler is currently running.
    /// </summary>
    Running,

    /// <summary>
    ///     The engine handler has been stopped gracefully.
    /// </summary>
    Stopped,

    /// <summary>
    ///     The engine handler has encountered an error and is in a faulted state.
    /// </summary>
    Faulted
}
