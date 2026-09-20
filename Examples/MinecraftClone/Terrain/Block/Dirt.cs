using SharpEngine.Core.Defaults;
using SharpEngine.Core.Numerics;
using SharpEngine.IO.Extensions;

namespace Minecraft.Terrain.Block;

/// <summary>
///    Represents a dirt block.
/// </summary>
internal class Dirt : BlockBase
{
    /// <summary>
    ///     Initializes a new <see cref="Dirt"/> block.
    /// </summary>
    /// <param name="position">The position where the block should be initialized.</param>
    /// <param name="name">The name of the block to initialize.</param>
    public Dirt(Silk.NET.OpenGL.GL gl, Vector3 position, string name) : base(gl, position, name, PathExtensions.GetAssemblyPath("Resources\\grass.jpg"), 
                                                                      PathExtensions.GetAssemblyPath("Resources\\container2_specular.png"),
                                                                      Defaults.VertexShader, 
                                                                      Defaults.FragmentShader) { }

    /// <inheritdoc />
    public override BlockId BlockId => BlockId.Dirt;
}
