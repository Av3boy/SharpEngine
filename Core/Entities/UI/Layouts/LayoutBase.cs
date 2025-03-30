using SharpEngine.Core.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEngine.Core.Entities.UI.Layouts;

public abstract class LayoutBase<T> : SceneNode where T : SceneNode
{
    public List<T> Items { get; set; } = [];

    public override SceneNode AddChild(params SceneNode[] nodes)
    {
        base.AddChild(nodes);

        foreach (var node in nodes)
            AddItem((T)node);

        return this;
    }

    public virtual void AddItem(T item)
    {
        Items.Add(item);
    }

    public virtual bool RemoveItem(T item)
    {
        return Items.Remove(item);
    }

    public abstract T[][] GetValues();
}
