using SharpEngine.Core.Entities;
using SharpEngine.Core.Entities.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SharpEngine.Core.Scenes;

/// <summary>
///     Represents a scene in the game.
/// </summary>
public class Scene
{
    /// <summary>The file extension by which saved scenes are associated with.</summary>
    public const string SceneFileExtension = "sharpscene";

    private string? _fileFullPath;

    /// <summary>Gets or sets whether the scene has unsaved changes.</summary>
    public bool HasUnsavedChanges { get; set; }

    /// <summary>Gets or sets the name of the scene file.</summary>
    public string Name { get; private set; } = "New Scene";

    /// <summary>
    ///     Initializes a new instance of <see cref="Scene"/>.
    /// </summary>
    public Scene() { }

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
    public SceneNode Root { get; set; } = new SceneNode("Root");

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
        // TODO: Can we make this async?
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

    /// <summary>
    ///    Loads a scene from the given <paramref name="sceneFile"/>.
    /// </summary>
    /// <param name="sceneFile">The file containing the scene.</param>
    /// <returns>The scene from the given file. Loads an empty scene if unable to load the scene.</returns>
    public static Scene LoadScene(string sceneFile)
    {
        Console.WriteLine(sceneFile);

        var loadedScene = JsonSerializer.Deserialize<Scene>(sceneFile);

        if (loadedScene is not null)
        {
            loadedScene.SetFileFullPath(sceneFile);
            return loadedScene;
        }

        return new();
    }

    /// <summary>
    ///     Sets the full path and name of the scene file.
    /// </summary>
    /// <param name="sceneFile">The full path of the file to be set and processed for name extraction.</param>
    public void SetFileFullPath(string sceneFile)
    {
        _fileFullPath = sceneFile;
        Name = System.IO.Path.GetFileNameWithoutExtension(sceneFile);
    }

    /// <summary>
    ///     Determines whether the scene has been saved before and has a save file.
    /// </summary>
    /// <returns><see langword="true"/> if the scene has a save file; otherwise, <see langword="false"/>.</returns>
    public bool HasSaveFile()
        => _fileFullPath is not null;

    /// <summary>
    ///     Saves the scene.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing an asynchronous operation.</returns>
    public async Task SaveScene()
        => await SaveScene(_fileFullPath!);

    /// <summary>
    ///     Saves the scene to the given <paramref name="fileName"/>.
    /// </summary>
    /// <param name="fileName">The name of the file to be saved.</param>
    /// <returns>A <see cref="Task"/> representing an asynchronous operation.</returns>
    public async Task SaveScene(string fileName)
    {
        var json = JsonSerializer.Serialize(this);
        await System.IO.File.WriteAllTextAsync(fileName, json);
    }
}