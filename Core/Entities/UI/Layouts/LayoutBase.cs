using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEngine.Core.Entities.UI.Layouts;

public abstract class LayoutBase<T>
{
    public List<T> Items;

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
