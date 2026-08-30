using SharpEngine.Core.Components.Properties;
using SharpEngine.Core.Numerics;
using Silk.NET.OpenGL;

namespace SharpEngine.Core.Shaders;

/// <summary>
///     Represents a shader program in the engine.
/// </summary>
public partial class Shader
{
    // Uniform setters
    // Uniforms are variables that can be set by user code, instead of reading them from the VBO.
    // You use VBOs for vertex-related data, and uniforms for almost everything else.

    // Setting a uniform is almost always the exact same, so I'll explain it here once, instead of in every method:
    //     1. Bind the program you want to set the uniform on
    //     2. Get a handle to the location of the uniform with GL.GetUniformLocation.
    //     3. Use the appropriate GL.Uniform* function to set the uniform.

    /// <summary>
    ///    Set a uniform on this shader.
    /// </summary>
    /// <typeparam name="TUniformType"></typeparam>
    /// <param name="uniformName">The name of the uniform.</param>
    /// <param name="data">The data to set.</param>
    /// <param name="setter">The setter action to use.</param>
    /// <exception cref="ArgumentException">Thrown if the uniform with the specified name is not found in the shader or if the shader program handle is null.</exception>
    private void SetUniform<TUniformType>(string uniformName, TUniformType data, Action<int, TUniformType> setter)
    {
        SetUniformInternal(uniformName, out int location);
        setter(location, data);
    }

    private void SetUniformInternal(string uniformName, out int location)
    {
        ArgumentNullException.ThrowIfNull(uniformName, nameof(uniformName));
        ArgumentNullException.ThrowIfNull(ProgramHandle, nameof(ProgramHandle));

        if (!_uniformLocations.TryGetValue(uniformName, out location))
            throw new ArgumentException($"Uniform '{uniformName}' not found in shader '{Name}'.", nameof(uniformName));

        // TODO: #95 The GL.UseProgram should be not be called here. Rather the renderer should call it once before rendering.
        GL.UseProgram(ProgramHandle);
    }

    /// <summary>
    ///     Set a uniform int on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The data to set.</param>
    /// <exception cref="ArgumentException">Thrown if the uniform with the specified name is not found in the shader or if the shader program handle is null.</exception>
    public void SetInt(string name, int data)
        => SetUniform(name, data, GL.Uniform1);

    /// <summary>
    ///     Set a uniform TextureUnit on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="textureUnit">The texture unit to set.</param>
    /// <exception cref="ArgumentException">Thrown if the uniform with the specified name is not found in the shader or if the shader program handle is null.</exception>
    public void SetTextureUnit(string name, TextureUnitIndex textureUnit)
        => SetUniform(name, (int)textureUnit, GL.Uniform1);

    /// <summary>
    ///     Set a uniform float on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The data to set.</param>
    /// <exception cref="ArgumentException">Thrown if the uniform with the specified name is not found in the shader or if the shader program handle is null.</exception>
    public void SetFloat(string name, float data)
        => SetUniform(name, data, GL.Uniform1);

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
    public void SetMatrix4(string name, Matrix4x4 data, bool transpose = true)
    {
        SetUniformInternal(name, out int location);
        GL.UniformMatrix4(location, transpose, data.ToSpan());
    }

    /// <summary>
    ///     Set a uniform Vector2 on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The data to set.</param>
    /// <exception cref="ArgumentException">Thrown if the uniform with the specified name is not found in the shader or if the shader program handle is null.</exception>
    public void SetVector2(string name, Vector2 data)
        => SetUniform(name, data, (uniform, d) => GL.Uniform2(uniform, d.X, d.Y));

    /// <summary>
    ///     Set a uniform Vector3 on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The data to set.</param>
    /// <exception cref="ArgumentException">Thrown if the uniform with the specified name is not found in the shader or if the shader program handle is null.</exception>
    public void SetVector3(string name, Vector3 data)
        => SetUniform(name, data, (uniform, d) => GL.Uniform3(uniform, d.X, d.Y, d.Z));

    /// <summary>
    ///     Set a uniform Vector4 on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The data to set.</param>
    /// <exception cref="ArgumentException">Thrown if the uniform with the specified name is not found in the shader or if the shader program handle is null.</exception>
    public void SetVector4(string name, Vector4 data)
        => SetUniform(name, data, (uniform, d) => GL.Uniform4(uniform, d.X, d.Y, d.Z, d.W));
}
