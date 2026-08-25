using SharpEngine.Core.Entities.Properties;
using SharpEngine.Core.Entities.Views;
using SharpEngine.Core.Numerics;
using SharpEngine.Core.Windowing;

using System.Collections.Generic;
using System.Threading.Tasks;
using Silk.NET.OpenGL;

namespace SharpEngine.Core.Scenes;

/// <summary>
///     Represents a node in the scene.
/// </summary>
public abstract class SceneNode
{
    /// <summary>Gets a new empty node.</summary>
    public static SceneNode Empty => new EmptyNode<Transform, Vector3>("Empty Node");

    /// <summary>
    ///     Gets or sets the name of the node.
    /// </summary>
    public string Name { get; set; } = "New Object";

    /// <summary>
    ///     Gets or sets the children of the node.
    /// </summary>
    public List<SceneNode> Children { get; set; } = [];

    private bool _initialized;

    /// <summary>
    ///     Initializes a new empty <see cref="SceneNode"/>.
    /// </summary>
    protected SceneNode() { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="SceneNode"/> with the specified <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name of the new empty node.</param>
    protected SceneNode(string name)
    {
        Name = name;
    }

    /// <summary>
    ///     Adds an empty child node to this node by the given <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name of the empty node to be added.</param>
    /// <returns>The created node.</returns>
    public virtual SceneNode AddChild(string name)
        => AddChild<Transform, Vector3>(name);

    /// <summary>
    ///     Adds an empty child node to this node by the given <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name of the empty node to be added.</param>
    /// <returns>The created node.</returns>
    public virtual SceneNode AddChild<TTransform, TVector>(string name) where TTransform : ITransform<TVector>, new() where TVector : IVector, new()
    {
        var node = new EmptyNode<TTransform, TVector>(name);
        Children.Add(node);

        return node;
    }

    /// <summary>
    ///     Adds a child node to this node.
    /// </summary>
    /// <param name="nodes">The nodes to be added.</param>
    /// <returns>The current node.</returns>
    public virtual SceneNode AddChild(params SceneNode[] nodes)
    {
        foreach (var node in nodes)
            Children.Add(node);

        return this;
    }

    /// <summary>
    ///     Removes a child node from this node.
    /// </summary>
    /// <param name="node">The node to be removed.</param>
    public void RemoveChild(SceneNode node)
    {
        // TODO: All children should be removed recursively, and all resources should be disposed of properly.

        Children.Remove(node);
    }

    /// <summary>
    ///     Renders the current object to the screen.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing an asynchronous operation.</returns>
    public virtual Task Render(CameraView camera, Window window) 
        => Task.CompletedTask;

    /// <summary>
    ///     Initializes the component when a scene is loaded.
    /// </summary>
    /// <remarks>
    ///     NOTE: This function does not load child nodes. <br />
    ///     It is the responsibility of the scene to call <see cref="OnInitialized"/> on all child nodes.
    /// </remarks>
    public virtual void OnInitialized(GL gl)
    {
        if (_initialized)
            return;

        _initialized = true;
    }

    public virtual void OnCreated() { }
    public virtual void Update(float deltaTime) { }
    public virtual void OnDeleted() { }
}
