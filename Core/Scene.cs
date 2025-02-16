using Core.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core;

public class Scene
{
    /// <summary>
    ///     Gets or sets the root node of the scene.
    /// </summary>
    public SceneNode Root { get; set; } = new SceneNode("Root");

    /// <summary>
    ///     Gets the nodes in the scene.
    /// </summary>
    private List<SceneNode> Nodes { get; set; } = new();

    public List<UIElement> UIElements { get; private set; } = new();

    /// <summary>
    ///     Adds an empty node to the scene root.
    /// </summary>
    /// <param name="name">The name of the new empty node.</param>
    public void AddNode(string name)
    {
        var node = new SceneNode(name);
        Nodes.Add(node);
        Root.Children.Add(node);
    }

    /// <summary>
    ///    Removes a node from the scene root.
    /// </summary>
    /// <param name="node">The node to be removed.</param>
    public virtual void RemoveNode(SceneNode node)
    {
        Nodes.Remove(node);
    }

    /// <summary>
    ///     Gets a node by the given <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name of the node to be retrieved.</param>
    /// <returns>The found node; <see langword="null"/> if not found.</returns>
    public SceneNode? GetNode(string name)
    {
        static SceneNode? FindNode(SceneNode node, string name)
        {
            if (node.Name == name)
                return node;

            foreach (var child in node.Children)
            {
                var result = FindNode(child, name);
                if (result != null)
                    return result;
            }

            return null;
        }

        foreach (var node in Nodes)
        {
            var result = FindNode(node, name);
            if (result != null)
                return result;
        }
        return null;
    }

    public List<GameObject> GetAllGameObjects()
    {
        var gameObjects = new List<GameObject>();
        static void FindGameObjects(SceneNode node, List<GameObject> gameObjects)
        {
            if (node is GameObject gameObject)
                gameObjects.Add(gameObject);

            foreach (var child in node.Children)
                FindGameObjects(child, gameObjects);
        }

        foreach (var node in Root.Children)
            FindGameObjects(node, gameObjects);

        return gameObjects;
    }

    public void Iterate<TEntityType>(List<TEntityType> elements, Action<TEntityType> action) where TEntityType : SceneNode
    {
        foreach (var entity in elements)
        {
            action(entity);

            var children = entity.Children.OfType<TEntityType>().ToList();
            if (children.Count != 0)
                Iterate(children, action);
        }
    }

    public void Iterate(List<SceneNode> elements, Action<SceneNode> action)
        => Iterate<SceneNode>(elements, action);

    public async Task IterateAsync<TEntityType>(List<TEntityType> elements, Func<TEntityType, Task> action) where TEntityType : SceneNode
    {
        foreach (var entity in elements)
        {
            await action(entity);

            var children = entity.Children.OfType<TEntityType>().ToList();
            if (children.Count != 0)
                await IterateAsync(children, action);
        }
    }

    public Task IterateAsync(List<SceneNode> elements, Func<SceneNode, Task> action)
        => IterateAsync<SceneNode>(elements, action);
}

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
