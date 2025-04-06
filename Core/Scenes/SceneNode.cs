using SharpEngine.Core.Entities.Properties;
using SharpEngine.Core.Entities.Views;
using SharpEngine.Core.Numerics;
using SharpEngine.Core.Windowing;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpEngine.Core.Scenes;

/// <summary>
///     Represents a empty node in a scene.
/// </summary>
/// <typeparam name="TTransform">Specifies the type used for transformations.</typeparam>
/// <typeparam name="TVector">Defines vector used by the transform.</typeparam>
public class EmptyNode<TTransform, TVector> : SceneNode where TTransform : ITransform<TVector>, new() where TVector : IVector, new()
{
    /// <summary>
    ///     Initializes a new instance of <see cref="EmptyNode{TTransform, TVector}"/>.
    /// </summary>
    /// <param name="name"></param>
    public EmptyNode(string name) : base(name) { }

    /// <summary>
    ///     Gets or sets the transform of the node.
    /// </summary>
    public virtual TTransform Transform { get; set; } = new();
}

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

    /// <summary>
    ///     Initializes a new empty <see cref="SceneNode"/>.
    /// </summary>
    public SceneNode() { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="SceneNode"/> with the specified <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name of the new empty node.</param>
    public SceneNode(string name)
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
        var node = new EmptyNode<TTransform, TVector>(name) as SceneNode;
        Children.Add(node!);

        return node!;
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
        Children.Remove(node);
    }

    /// <summary>
    ///     Renders the current object to the screen.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing an asynchronous operation.</returns>
    public virtual Task Render(CameraView camera, Window window) => Task.CompletedTask;
}
