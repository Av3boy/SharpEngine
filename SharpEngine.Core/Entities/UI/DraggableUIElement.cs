using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Silk.NET.OpenGL;

namespace SharpEngine.Core.Entities.UI;

public class DraggableUIElement : UIElement
{
    public DraggableUIElement(GL gl) : base(gl)
    {
    }

    public DraggableUIElement(GL gl, string name) : base(gl, name)
    {
    }
}
