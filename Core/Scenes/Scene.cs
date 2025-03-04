using Core.Entities;

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
    public List<UIElement> UIElements { get; private set; } = [];

    /// <summary>Gets or sets the active element in the scene.</summary>
    /// <remarks>Editor only.</remarks>
    public SceneNode ActiveElement { get; set; }

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
    ///     Gets all game objects in the scene.
    /// </summary>
    /// <returns>All the gameobjects in the current scene.</returns>
    public List<GameObject> GetAllGameObjects()
    {
        // TODO: Can we make this async?
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

    /// <summary>
    ///     Iterates over the given <paramref name="elements"/> and performs the given <paramref name="action"/> for each element.
    /// </summary>
    /// <param name="elements">The scene nodes to iterate over</param>
    /// <param name="action">The action to be executed for each element.</param>
    /// <returns>A <see cref="Task"/> representing an asynchronous operation.</returns>
    public async Task IterateAsync(List<SceneNode> elements, Func<SceneNode, Task> action)
        => await IterateAsync<SceneNode>(elements, action);

    /// <summary>
    ///    Loads a scene from the given <paramref name="sceneFile"/>.
    /// </summary>
    /// <param name="sceneFile">The file containing the scene.</param>
    /// <returns>The scene from the given file. Loads an empty scene if unable to load the scene.</returns>
    public static Scene LoadScene(string sceneFile)
    {
        var loadedScene = JsonSerializer.Deserialize<Scene>(sceneFile);

        if (loadedScene is not null)
        {
            loadedScene.SetFileFullPath(sceneFile);
            return loadedScene;
        }

        return new();
    }

    private void SetFileFullPath(string sceneFile)
    {
        _fileFullPath = sceneFile;
        Name = System.IO.Path.GetFileNameWithoutExtension(sceneFile);
    }

    public bool HasSaveFile()
        => _fileFullPath is not null;

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