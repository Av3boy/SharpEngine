using SharpEngine.Core.Entities.Properties;
using SharpEngine.Core.Numerics;
using SharpEngine.Core.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEngine.Core.Entities.UI.Layouts;

public abstract class LayoutBase<TItem> : SceneNode where TItem : SceneNode, new()
{
    public List<TItem> Items { get; set; } = [];

    public override LayoutBase<TItem> AddChild(params SceneNode[] nodes)
    {
        foreach (var node in nodes)
            AddItem((TItem)node);

        return this;
    }

    public virtual void AddItem(TItem item)
    {
        Items.Add(item);
    }

    public virtual bool RemoveItem(TItem item)
    {
        return Items.Remove(item);
    }

    public abstract TItem[][] GetValues();
}
