using Microsoft.Extensions.Logging;
using Silk.NET.OpenGL;
using System.Numerics;
using System.Text.RegularExpressions;

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

    private bool TrySetUniform<T>(string uniformName, T data, Action<int, T> setter)
    {
        if (!_uniformLocations.TryGetValue(uniformName, out int location))
        {
            _logger.LogInformation("Uniform '{UniformName}' not found in shader '{ShaderName}'.", uniformName, Name);
            return false;
        }

        // TODO: #95 The GL.UseProgram should be not be called here. Rather the renderer should call it once before rendering.
        GL.UseProgram(Handle);
        setter(location, data);
        
        return true;
    }

    /// <summary>
    ///     Set a uniform int on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The data to set.</param>
    public bool SetInt(string name, int data)
        => TrySetUniform(name, data, GL.Uniform1);

    /// <summary>
    ///     Set a uniform float on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The data to set.</param>
    public bool SetFloat(string name, float data)
        => TrySetUniform(name, data, GL.Uniform1);

    /// <summary>
    ///     Set a uniform Matrix4 on this shader
    /// </summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The data to set.</param>
    /// <param name="transpose">Determines whether or not the matrix should be transposed. Defaults to <see langword="true"/>.</param>
    /// <remarks>
    ///   <para>
    ///   The matrix is transposed before being sent to the shader unless <paramref name="transpose"/> is set to <see langword="false"/>.
    ///   </para>
    /// </remarks>
    public bool SetMatrix4(string name, Matrix4x4 data, bool transpose = true)
        => TrySetUniform(name, data, (uniform, d) => GL.UniformMatrix4(uniform, transpose, d.ToSpan()));

    /// <summary>
    ///     Set a uniform Vector2 on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The data to set.</param>
    public bool SetVector2(string name, Vector2 data)
        => TrySetUniform(name, data, GL.Uniform2);

    /// <summary>
    ///     Set a uniform Vector3 on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The data to set.</param>
    public bool SetVector3(string name, Vector3 data)
        => TrySetUniform(name, data, GL.Uniform3);

    /// <summary>
    ///     Set a uniform Vector3 on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The data to set.</param>
    public bool SetVector4(string name, Vector4 data)
        => TrySetUniform(name, data, GL.Uniform4);
}
