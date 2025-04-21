using SharpEngine.Shared;
using Silk.NET.OpenGL;
using System.Numerics;
using System.Text.RegularExpressions;

namespace SharpEngine.Core.Shaders;

public partial class Shader
{
    public Shader Initialize()
    {
        // Load and compile shader
        if (!LoadShader(ShaderType.VertexShader, VertPath, out uint vertexShader))
        {
            Debug.Log.Error("Unable to load vertex shader.");
            return this;
        }

        if (!LoadShader(ShaderType.FragmentShader, FragPath, out uint fragmentShader))
        {
            Debug.Log.Error("Unable to load fragment shader.");
            return this;
        }

        // These two shaders must then be merged into a shader program, which can then be used by OpenGL.
        // To do this, create a program...

        Handle = _gl.CreateProgram();

        // Attach both shaders...
        _gl.AttachShader(Handle, vertexShader);
        _gl.AttachShader(Handle, fragmentShader);

        // And then link them together.
        bool shaderLinked = LinkProgram(Handle);

        // When the shader program is linked, it no longer needs the individual shaders attached to it; the compiled code is copied into the shader program.
        // Detach them, and then delete them.
        _gl.DetachShader(Handle, vertexShader);
        _gl.DetachShader(Handle, fragmentShader);
        _gl.DeleteShader(fragmentShader);
        _gl.DeleteShader(vertexShader);

        if (!shaderLinked)
        {
            Debug.Log.Information("Unable to link shader program.");
            return this;
        }

        // The shader is now ready to go, but first, we're going to cache all the shader uniform locations.
        // Querying this from the shader is very slow, so we do it once on initialization and reuse those values
        // later.
        SetUniformLocations();

        return this;
    }

    private bool LoadShader(ShaderType shaderType, string shaderPath, out uint shader)
    {
        if (!File.Exists(shaderPath))
        {
            Debug.Log.Information($"Shader file not found: {shaderPath}");

            shader = 0;
            return false;
        }

        string shaderSource = File.ReadAllText(shaderPath);
        shaderSource = ProcessIncludes(shaderSource, Path.GetDirectoryName(shaderPath)!);

        // GL.CreateShader will create an empty shader (obviously). The ShaderType enum denotes which type of shader will be created.
        shader = _gl.CreateShader(shaderType);
        _gl.ShaderSource(shader, shaderSource);

        if (!CompileShader(shader))
        {
            Debug.Log.Information($"Unable to load {shaderType} shader from '{shaderPath}'.");
            return false;
        }

        return true;
    }

    public Shader SetUniformLocations()
    {
        // First, we have to get the number of active uniforms in the shader.
        _gl.GetProgram(Handle, GLEnum.ActiveUniforms, out var numberOfUniforms);

        Dictionary<string, int> uniformLocations = [];

        // Loop over all the uniforms,
        for (uint i = 0; i < numberOfUniforms; i++)
        {
            // get the name of this uniform,
            var key = _gl.GetActiveUniform(Handle, i, out _, out _);

            // get the location,
            var location = _gl.GetUniformLocation(Handle, key);

            // and then add it to the dictionary.
            uniformLocations.Add(key, location);
        }

        _uniformLocations = uniformLocations;
        return this;
    }

    private static string ProcessIncludes(string shaderCode, string directory)
    {
        string includePattern = @"#include\s+""(.+?)""";
        return Regex.Replace(shaderCode, includePattern, match =>
        {
            string includePath = Path.Combine(directory, match.Groups[1].Value);
            string includeCode = File.ReadAllText(includePath);
            return ProcessIncludes(includeCode, Path.GetDirectoryName(includePath)!);
        });
    }

    private bool CompileShader(uint shader)
    {
        // Try to compile the shader
        _gl.CompileShader(shader);

        // Check for compilation errors
        _gl.GetShader(shader, GLEnum.CompileStatus, out var statusCode);
        if (statusCode != (int)GLEnum.True)
        {
            // We can use `GL.GetShaderInfoLog(shader)` to get information about the error.
            var infoLog = _gl.GetShaderInfoLog(shader);
            Debug.Log.Information($"Error occurred whilst compiling Shader({shader}).\n\n{infoLog}");

            return false;
        }

        return true;
    }

    private bool LinkProgram(uint program)
    {
        _gl.LinkProgram(program);
        _gl.GetProgram(program, GLEnum.LinkStatus, out var statusCode);
        
        if (statusCode != (int)GLEnum.True)
        {
            string infoLog = _gl.GetProgramInfoLog(program);
            Debug.Log.Information($"Error occurred whilst linking Program({program}): {infoLog}");

            return false;
        }

        return true;
    }

    /// <summary>
    ///     Enables the shader program.
    /// </summary>
    public void Use()
        => _gl.UseProgram(Handle);

    /// <summary>
    ///     Checks if the shader attribute exists within the current shader.
    /// </summary>
    /// <param name="attribName">The name of the attribute that's being looked for.</param>
    /// <param name="location">Outputs the location of the attribute in the shader if found; otherwise -1.</param>
    /// <returns>If the attribute exists, <see langword="true"/>; otherwise, <see langword="false"/>. </returns>
    public bool TryGetAttribLocation(string attribName, out int location)
    {
        location = _gl.GetAttribLocation(Handle, attribName);
        if (location == ShaderAttributes.AttributeLocationNotFound)
        {
            Debug.Log.Information($"Attribute '{attribName}' not found in shader program.");
            return false;
        }

        return true;
    }

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
            Debug.Log.Information("Uniform '{UniformName}' not found in shader '{ShaderName}'.", uniformName, Name);
            return false;
        }

        // TODO: #95 The _gl.UseProgram should be not be called here. Rather the renderer should call it once before rendering.
        _gl.UseProgram(Handle);
        setter(location, data);
        
        return true;
    }

    /// <summary>
    ///     Set a uniform int on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The data to set.</param>
    public void SetInt(string name, int data)
    {
        TrySetUniform(name, data, _gl.Uniform1);
    }

    /// <summary>
    ///     Set a uniform float on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The data to set.</param>
    public void SetFloat(string name, float data)
    {
        TrySetUniform(name, data, _gl.Uniform1);
    }

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
    public unsafe void SetMatrix4(string name, Matrix4x4 data, bool transpose = true)
    {
        // TrySetUniform(name, data, (uniform, d) => _gl.UniformMatrix4(uniform, transpose, d.ToSpan()));
        TrySetUniform(name, data, (uniform, d) => _gl.UniformMatrix4(uniform, 1, transpose, (float*) &d));
    }

    /// <summary>
    ///     Set a uniform Vector2 on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The data to set.</param>
    public void SetVector2(string name, Vector2 data)
    {
        TrySetUniform(name, data, _gl.Uniform2);
    }

    /// <summary>
    ///     Set a uniform Vector3 on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The data to set.</param>
    public void SetVector3(string name, Vector3 data)
    {
        TrySetUniform(name, data, _gl.Uniform3);
    }

    /// <summary>
    ///     Set a uniform Vector3 on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The data to set.</param>
    public void SetVector4(string name, Vector4 data)
    {
        TrySetUniform(name, data, _gl.Uniform4);
    }
}
