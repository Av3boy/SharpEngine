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

    public virtual SceneNode AddChild(params SceneNode[] nodes)
    {
        foreach (var node in nodes)
            Children.Add(node);

        return this;
    }

    public void RemoveChild(SceneNode node)
    {
        Children.Remove(node);
    }
}
