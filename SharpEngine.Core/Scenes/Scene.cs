using Microsoft.Extensions.Logging;
using SharpEngine.Core.Entities.Properties;
using SharpEngine.Telemetry;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharpEngine.IO;

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
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="Scene"/>.
    /// </summary>
    /// <param name="sceneFile"></param>
    public Scene(string sceneFile)
    {
        SetFileFullPath(sceneFile);
    }

    /// <summary>
    ///     Gets or sets the root node of the scene.
    /// </summary>
    public SceneNode Root { get; set; } = new EmptyNode<Transform, SharpEngine.Core.Numerics.Vector3>("Root");

    /// <summary>Gets or sets the nodes in the scene.</summary>
    private List<SceneNode> Nodes { get; set; } = [];

    /// <summary>Gets or sets the UI elements in the scene.</summary>
    public List<SceneNode> UIElements { get; private set; } = [];

    /// <summary>Gets or sets the active element in the scene.</summary>
    /// <remarks>Editor only.</remarks>
    public SceneNode? ActiveElement { get; set; }

    /// <summary>
    ///     Adds an empty node to the scene root.
    /// </summary>
    /// <param name="name">The name of the new empty node.</param>
    public SceneNode AddNode(string name)
    {
        var node = new EmptyNode<Transform, SharpEngine.Core.Numerics.Vector3>(name);
        Nodes.Add(node);
        Root.Children.Add(node);

        return node;
    }

    /// <summary>
    ///    Removes a node from the scene root.
    /// </summary>
    /// <param name="node">The node to be removed.</param>
    public virtual void RemoveNode(SceneNode node)
        => Nodes.Remove(node);

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

    /// <summary>
    ///     Gets all the objects in the scene of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the objects to be retrieved.</typeparam>
    /// <returns>All the game objects in the current scene.</returns>
    public List<T> GetObjectsOfType<T>(SceneNode? root = null)
    {
        // TODO: #94 Can we make this async?
        var result = new List<T>();
        static void FindsObjects(SceneNode node, List<T> result)
        {
            if (node is T obj)
                result.Add(obj);

            foreach (var child in node.Children)
                FindsObjects(child, result);
        }

        foreach (var node in root?.Children ?? Root.Children)
            FindsObjects(node, result);

        return result;
    }

    /// <summary>
    ///     Iterates over the given <paramref name="elements"/> and performs the given <paramref name="action"/> for each element.
    /// </summary>
    /// <typeparam name="TEntityType">The type of the element.</typeparam>
    /// <param name="elements">The scene nodes to iterate over</param>
    /// <param name="action">The action to be executed for each element.</param>
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

    /// <summary>
    ///     Iterates over the given <paramref name="elements"/> and performs the given <paramref name="action"/> for each element.
    /// </summary>
    /// <typeparam name="TEntityType">The type of the element.</typeparam>
    /// <param name="elements">The scene nodes to iterate over</param>
    /// <param name="action">The action to be executed for each element.</param>
    /// <returns>A <see cref="Task"/> representing an asynchronous operation.</returns>
    public IEnumerable<Task> IterateAsync<TEntityType>(IEnumerable<SceneNode> elements, Func<SceneNode, Task> action)
    {
        var tasks = new List<Task>();

        foreach (var entity in elements)
        {
            tasks.Add(action(entity));
            tasks.AddRange(IterateAsync(entity.Children, action));
        }

        return tasks;
    }

    /// <summary>
    ///     Iterates over the given <paramref name="elements"/> and performs the given <paramref name="action"/> for each element.
    /// </summary>
    /// <param name="elements">The scene nodes to iterate over</param>
    /// <param name="action">The action to be executed for each element.</param>
    /// <returns>A <see cref="Task"/> representing an asynchronous operation.</returns>
    public IEnumerable<Task> IterateAsync(List<SceneNode> elements, Func<SceneNode, Task> action)
        => IterateAsync<SceneNode>(elements, action);

    }