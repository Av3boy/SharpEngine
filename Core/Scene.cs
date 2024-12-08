using System.Collections.Generic;

namespace Core;

public class Scene
{
    public List<GameObject> Blocks { get; set; } = new();
    public SceneNode Root { get; set; } = new SceneNode { Name = "Root" };
    public List<SceneNode> Nodes { get; private set; } = new();

    public virtual void AddNode(SceneNode node)
    {
        Nodes.Add(node);
        if (node is GameObject gameObject)
        {
            Blocks.Add(gameObject);
        }
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
}
