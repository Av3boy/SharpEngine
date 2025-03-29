using SharpEngine.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEngine.Core._Resources;
public struct Default
{
    public static string DebugTexture => PathExtensions.GetAssemblyPath("Textures\\DefaultTextures\\debug.JPG");
    public static string VertexShader => PathExtensions.GetAssemblyPath("Shaders\\shader.vert");
    public static string FragmentShader => PathExtensions.GetAssemblyPath("Shaders\\lighting.frag");
}
