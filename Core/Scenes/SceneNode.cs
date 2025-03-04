﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpEngine.Core.Scenes;

/// <summary>
///     Represents a node in the scene.
/// </summary>
public class SceneNode
{
    /// <summary>
    ///     Gets or sets the name of the node.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     Gets or sets the children of the node.
    /// </summary>
    public List<SceneNode> Children { get; set; } = new();

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
    public SceneNode AddChild(string name)
    {
        var node = new SceneNode(name);
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
        Children.Remove(node);
    }

    /// <summary>
    ///     Renders the current object to the screen.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing an asynchronous operation.</returns>
    public virtual Task Render() => Task.CompletedTask;
}
