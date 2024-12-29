using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core;
public class UIElement : SceneNode
{
    public Vector3 Position { get; set; } = new(0, 0, 0);
    public Vector3 Scale { get; set; } = new(1, 1, 1);

    /// <summary>
    ///     Render the UI element.
    /// </summary>
    public void Render(Shader uiShader)
    {
        //element.Material.DiffuseMap.Use(TextureUnit.Texture0);

        var model = Matrix4.CreateTranslation(Position);
        model *= Matrix4.CreateScale(Scale);
        uiShader.SetMatrix4("model", model);

        GL.DrawArrays(PrimitiveType.Triangles, 0, 2);
    }
}
