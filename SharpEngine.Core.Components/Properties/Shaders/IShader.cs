using SharpEngine.Core.Numerics;
using Silk.NET.OpenGL;

namespace SharpEngine.Core.Components.Properties.Shaders;

/// <summary>
///     Defines the contract for setting shader uniform parameters and querying their locations.
/// </summary>
public interface IShader
{
    /// <summary>Gets the uniform name-to-location map cached at shader initialization.</summary>
    IReadOnlyDictionary<string, int> UniformLocations { get; }

    /// <summary>
    ///     Set a uniform int on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The data to set.</param>
    /// <exception cref="ArgumentException">Thrown if the uniform with the specified name is not found in the shader or if the shader program handle is null.</exception>
    public void SetInt(string name, int data);

    /// <summary>
    ///     Set a uniform TextureUnit on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="textureUnit">The texture unit to set.</param>
    /// <exception cref="ArgumentException">Thrown if the uniform with the specified name is not found in the shader or if the shader program handle is null.</exception>
    public void SetTextureUnit(string name, TextureUnitIndex textureUnit);

    /// <summary>
    ///     Set a uniform float on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The data to set.</param>
    /// <exception cref="ArgumentException">Thrown if the uniform with the specified name is not found in the shader or if the shader program handle is null.</exception>
    public void SetFloat(string name, float data);

    /// <summary>
    ///     Set a uniform Matrix4 on this shader
    /// </summary>
    /// <remarks>
    ///   <para>
    ///   The matrix is transposed before being sent to the shader unless <paramref name="transpose"/> is set to <see langword="false"/>.
    ///   </para>
    /// </remarks>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The data to set.</param>
    /// <param name="transpose">Determines whether or not the matrix should be transposed. Defaults to <see langword="true"/>.</param>
    /// <exception cref="ArgumentException">Thrown if the uniform with the specified name is not found in the shader or if the shader program handle is null.</exception>
    public void SetMatrix4(string name, Matrix4x4 data, bool transpose = true);

    /// <summary>
    ///     Set a uniform Vector2 on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The data to set.</param>
    /// <exception cref="ArgumentException">Thrown if the uniform with the specified name is not found in the shader or if the shader program handle is null.</exception>
    public void SetVector2(string name, Vector2 data);

    /// <summary>
    ///     Set a uniform Vector3 on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The data to set.</param>
    /// <exception cref="ArgumentException">Thrown if the uniform with the specified name is not found in the shader or if the shader program handle is null.</exception>
    public void SetVector3(string name, Vector3 data);

    /// <summary>
    ///     Set a uniform Vector4 on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The data to set.</param>
    /// <exception cref="ArgumentException">Thrown if the uniform with the specified name is not found in the shader or if the shader program handle is null.</exception>
    public void SetVector4(string name, Vector4 data);
}
