using SharpEngine.Core.Components.Properties;
using SharpEngine.Core.Textures;
using Shader = SharpEngine.Core.Shaders.Shader;

namespace SharpEngine.Core.Entities;

public static class MaterialExtensions
{
    public static Material Default(Shader shader)
    {
        var debugTexture = TextureService.Instance.LoadTexture(Defaults.Defaults.DebugTexture);
        var material = new Material("defaultMaterial", debugTexture) { Shader = shader };
        return material;
    }
}