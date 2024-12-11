using System.Collections.Generic;
using static System.Reflection.Metadata.BlobBuilder;

namespace Core;

public class Scene
{
    public List<GameObject> Blocks { get; set; } = new();
    public SceneNode Root { get; set; } = new SceneNode("Root");
    public List<SceneNode> Nodes { get; private set; } = new();

    public void AddNode(string name)
    {
        var node = new SceneNode(name);
        Nodes.Add(node);
        Root.Children.Add(node);
    }

    public virtual void RemoveNode(SceneNode node)
    {
        Nodes.Remove(node);
        if (node is GameObject gameObject)
        {
            Blocks.Remove(gameObject);
        }
    }
}

public class SceneNode
{
    public string Name { get; set; }
    public List<SceneNode> Children { get; set; } = new();

    public SceneNode() { }

    public SceneNode(string name)
    {
        Name = name;
    }

    public SceneNode AddChild(string name)
    {
        var node = new SceneNode(name);
        Children.Add(node);

        return node;
    }

    public void RemoveChild(SceneNode node)
    {
        Children.Remove(node);
    }

    public virtual void AddChild(SceneNode node)
    {
        Children.Add(node);
    }
}
