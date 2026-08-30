using Microsoft.Extensions.Logging;
using SharpEngine.Core.Components.Properties;
using SharpEngine.Core.Entities.Views;
using SharpEngine.Core.Numerics;
using SharpEngine.Core.Windowing;
using SharpEngine.Telemetry;

using Silk.NET.Core;
using Silk.NET.OpenGL;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;

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
    public List<SceneNode> Children { get; set; } = new List<SceneNode>();

    /// <summary>
    ///     The parent node in the scene graph. Null for the root.
    /// </summary>
    public SceneNode? Parent { get; private set; }

    // TODO: This has to be redundant.
    // This was a part of a AI vibe code but wasn't able to figure out how to make things work without it.
    // Need to investigate if this is really needed or not.
    /// <summary>
    ///     The scene this node belongs to. May be null for detached nodes.
    /// </summary>
    public Scene? Scene { get; internal set; }

    private bool _initialized;

    private readonly ILogger<SceneNode> _logger;

    /// <summary>
    ///     Initializes a new empty <see cref="SceneNode"/>.
    /// </summary>
    protected SceneNode() : this("New Object") { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="SceneNode"/> with the specified <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name of the new empty node.</param>
    protected SceneNode(string name)
    {
        Name = name;
        _logger = LoggingExtensions.CreateLogger<SceneNode>();
    }

    /// <summary>
    ///     Adds an empty child node to this node by the given <paramref name="name"/>.
    /// </summary>
    /// <typeparam name="TTransform">The type of the transform. This is usually either a 2D (<see cref="Transform2D"/>) or 3D transform (<see cref="Transform"/>).</typeparam>
    /// <typeparam name="TVector">The type of the vector.</typeparam>
    /// <param name="name">The name of the new child node.</param>
    /// <returns>The current node where the child is added to.</returns>
    public virtual SceneNode AddChild<TTransform, TVector>(string name) where TTransform : ITransform<TVector>, new() where TVector : IVector, new()
    {
        var node = new EmptyNode<TTransform, TVector>(name);
        AddChildInternal(node);

        return this;
    }

    /// <inheritdoc cref="AddChild{TTransform, TVector}(string)" />
    /// <param name="node">The node to be added.</param>
    public virtual SceneNode AddChild(SceneNode node)
    {
        AddChildInternal(node);
        return this;
    }

    /// <inheritdoc cref="AddChild{TTransform, TVector}(string)" />
    /// <param name="nodes">The nodes to be added.</param>
    public virtual SceneNode AddChild(params SceneNode[] nodes)
    {
        foreach (var node in nodes)
        {
            AddChildInternal(node);
        }

        return this;
    }

    private SceneNode AddChildInternal(SceneNode node)
    {
        node.Parent = this;
        // propagate scene reference so node knows which scene it belongs to
        node.SetSceneRecursive(Scene);
        Children.Add(node);

        // Ensure lifecycle hook runs for newly added nodes
        try
        {
            node.OnCreated();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Message}", ex.Message);
        }

        // Notify the scene that structure changed
        Scene?.IncrementRevision();

        return node;
    }

    /// <summary>
    ///     Removes a child node from this node.
    /// </summary>
    /// <param name="node">The node to be removed.</param>
    public void RemoveChild(SceneNode node)
    {
        if (node is null || !Children.Remove(node))
            return;

        RemoveSubtree(node);
    }

    private static void RemoveSubtree(SceneNode node)
    {
        foreach (var child in node.Children.ToArray())
            RemoveSubtree(child);

        // Detach children
        foreach (var child in node.Children)
        {
            child.Parent = null;
            child.SetSceneRecursive(null);
        }

        node.Children.Clear();
        node.OnDeleted();
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

    /// <summary>
    ///     Called when the node is created and added to the scene.
    /// </summary>
    public virtual void OnCreated() { }

    /// <summary>
    ///    Called every frame to update the node's state.
    /// </summary>
    /// <param name="deltaTime"></param>
    public virtual void Update(float deltaTime) { }

    /// <summary>
    ///     Called when the node is deleted and removed from the scene.
    /// </summary>
    public virtual void OnDeleted() { }

    /// <summary>
    ///     Set the Scene reference for this node and all descendants.
    /// </summary>
    internal void SetSceneRecursive(Scene? scene)
    {
        Scene = scene;
        foreach (var child in Children)
            child.SetSceneRecursive(scene);
    }
}
