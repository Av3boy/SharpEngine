using Microsoft.Extensions.Logging;

using SharpEngine.Core.Entities.Properties;
using SharpEngine.Telemetry;
using SharpEngine.IO;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpEngine.Core.Scenes;

/// <summary>
///     Represents a scene in the game.
/// </summary>
public class Scene : SaveableFile<Scene>
{
    private static readonly ILogger<Scene> Logger = LoggingExtensions.CreateLogger<Scene>();

    /// <summary>The file extension by which saved scenes are associated with.</summary>
    public const string SceneFileExtension = "sharpscene";

    /// <summary>
    ///     Initializes a new instance of <see cref="Scene"/>.
    /// </summary>
    public Scene()
    {
        Name = "New Scene";
        // ensure root and its current subtree know which scene they belong to
        Root.SetSceneRecursive(this);
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="Scene"/>.
    /// </summary>
    /// <param name="sceneFile"></param>
    public Scene(string sceneFile)
    {
        SetFileFullPath(sceneFile);
        Root.SetSceneRecursive(this);
    }

    /// <summary>
    ///     Gets or sets the root node of the scene.
    /// </summary>
    public SceneNode Root { get; set; } = new EmptyNode<Transform, SharpEngine.Core.Numerics.Vector3>("Root");

    /// <summary>Gets or sets the nodes in the scene.</summary>
    private List<SceneNode> Nodes { get; set; } = new List<SceneNode>();

    /// <summary>Gets or sets the UI elements in the scene.</summary>
    public List<SceneNode> UIElements { get; set; } = new List<SceneNode>();

    /// <summary>Gets or sets the active element in the scene.</summary>
    /// <remarks>Editor only.</remarks>
    public SceneNode? ActiveElement { get; set; }

    /// <summary>
    ///     Revision counter incremented when the scene structure changes.
    /// </summary>
    public int Revision { get; private set; }

    /// <summary>
    ///     Event fired when scene structure changes (nodes added/removed).
    /// </summary>
    public event Action? SceneChanged;

    internal void IncrementRevision()
    {
        Revision++;
        SceneChanged?.Invoke();
    }
}