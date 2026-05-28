using Silk.NET.Assimp;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;

namespace SharpEngine.Core.Components.Properties.Textures;

/// <summary>
///     Represents a texture program.
/// </summary>
public partial class Texture : IDisposable, IDeepCloneable<Texture>, IEquatable<Texture>
{
    /// <summary>The OpenGL handle for the texture.</summary>
    public readonly uint Handle;

    /// <summary>Gets the type of the texture.</summary>
    public readonly TextureType Type;

    /// <summary>Gets the path to the texture file.</summary>
    public readonly string Path;
    
    private readonly GL _gl;

    /// <summary>
    ///     Initializes a new instance of <see cref="Texture"/>.
    /// </summary>
    /// <param name="gl">The OpenGL context where this texture should be available.</param>
    /// <param name="path">The path to the texture file.</param>
    /// <param name="type">The type of the texture.</param>
    public Texture(GL gl, string path, TextureType type = TextureType.Diffuse)
    {
        _gl = gl;
        Handle = _gl.GenTexture();

        Path = path;
        Type = type;

        Initialize();
    }

    /// <inheritdoc />
    /// <remarks>Assumes that the OpenGL context is the same as the original texture.</remarks>
    public Texture DeepClone()
        => new(_gl, Path, Type);

    /// <inheritdoc />
    public bool Equals(Texture? other)
    {
        if (other is null) 
            return false;
        
        if (ReferenceEquals(this, other))
            return true;

        return Handle == other.Handle && 
               Type == other.Type && 
               Path == other.Path;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => Equals(obj as Texture);

    /// <inheritdoc />
    public override int GetHashCode()
        => HashCode.Combine(Handle, Type, Path);
}
