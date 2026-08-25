using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEngine.Core.Entities.Interfaces;

public interface IClickable
{
    void OnClick(PointerClickEvent e);
}

public class PointerClickEvent
{
}

public sealed class ClickableComponent : IComponent
{
    public event Action<PointerClickEvent>? Clicked;

    internal void RaiseClick(PointerClickEvent e)
    {
        Clicked?.Invoke(e);
    }
}