using SharpEngine.Core.Windowing;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text.RegularExpressions;

namespace SharpEngine.Core.Shaders;

public static class ShaderExtensions
{
    public static Shader Initialize(this Shader shader)
    {
        // Load and compile shader
        if (!LoadShader(ShaderType.VertexShader, shader.VertPath, out uint vertexShader))
        {
            Console.WriteLine("Unable to load vertex shader.");
            return shader;
        }

        if (!LoadShader(ShaderType.FragmentShader, shader.FragPath, out uint fragmentShader))
        {
            Console.WriteLine("Unable to load fragment shader.");
            return shader;
        }

        // These two shaders must then be merged into a shader program, which can then be used by OpenGL.
        // To do this, create a program...

        shader.Handle = Window.GL.CreateProgram();

        // Attach both shaders...
        Window.GL.AttachShader(shader.Handle, vertexShader);
        Window.GL.AttachShader(shader.Handle, fragmentShader);

        // And then link them together.
        if (!LinkProgram(shader.Handle))
        {
            Console.WriteLine("Unable to link shader program.");
            return shader;
        }

        // When the shader program is linked, it no longer needs the individual shaders attached to it; the compiled code is copied into the shader program.
        // Detach them, and then delete them.
        Window.GL.DetachShader(shader.Handle, vertexShader);
        Window.GL.DetachShader(shader.Handle, fragmentShader);
        Window.GL.DeleteShader(fragmentShader);
        Window.GL.DeleteShader(vertexShader);

        // The shader is now ready to go, but first, we're going to cache all the shader uniform locations.
        // Querying this from the shader is very slow, so we do it once on initialization and reuse those values
        // later.
        SetUniformLocations(shader);

        return shader;
    }

    private static bool LoadShader(ShaderType shaderType, string shaderPath, out uint shader)
    {
        if (!File.Exists(shaderPath))
        {
            Console.WriteLine($"Shader file not found: {shaderPath}");

            shader = 0;
            return false;
        }

        string shaderSource = File.ReadAllText(shaderPath);
        shaderSource = ProcessIncludes(shaderSource, Path.GetDirectoryName(shaderPath)!);

        // GL.CreateShader will create an empty shader (obviously). The ShaderType enum denotes which type of shader will be created.
        shader = Window.GL.CreateShader(shaderType);
        Window.GL.ShaderSource(shader, shaderSource);

        if (!CompileShader(shader))
        {
            Console.WriteLine($"Unable to load {shaderType} shader from '{shaderPath}'.");
            return false;
        }

        return true;
    }

    public static Shader SetUniformLocations(this Shader shader)
    {
        // First, we have to get the number of active uniforms in the shader.
        Window.GL.GetProgram(shader.Handle, GLEnum.ActiveUniforms, out var numberOfUniforms);

        // Next, allocate the dictionary to hold the locations.
        Dictionary<string, int> uniformLocations = [];

        // Loop over all the uniforms,
        for (uint i = 0; i < numberOfUniforms; i++)
        {
            // get the name of this uniform,
            var key = Window.GL.GetActiveUniform(shader.Handle, i, out _, out _);

            // get the location,
            var location = Window.GL.GetUniformLocation(shader.Handle, key);

            // and then add it to the dictionary.
            uniformLocations.Add(key, location);
        }

        shader.InitializeUniforms(uniformLocations);
        return shader;
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

    private static bool CompileShader(uint shader)
    {
        // Try to compile the shader
        Window.GL.CompileShader(shader);

        // Check for compilation errors
        Window.GL.GetShader(shader, GLEnum.CompileStatus, out var statusCode);
        if (statusCode != (int)GLEnum.True)
        {
            // We can use `GL.GetShaderInfoLog(shader)` to get information about the error.
            var infoLog = Window.GL.GetShaderInfoLog(shader);
            Console.WriteLine($"Error occurred whilst compiling Shader({shader}).\n\n{infoLog}");

            return false;
        }

        return true;
    }

    private static bool LinkProgram(uint program)
    {
        Window.GL.LinkProgram(program);
        Window.GL.GetProgram(program, GLEnum.LinkStatus, out var statusCode);
        
        if (statusCode != (int)GLEnum.True)
        {
            string infoLog = Window.GL.GetProgramInfoLog(program);
            Console.WriteLine($"Error occurred whilst linking Program({program}): {infoLog}");

            return false;
        }

        return true;
    }

    /// <summary>
    ///     Enables the shader program.
    /// </summary>
    public static void Use(this Shader shader)
        => Window.GL.UseProgram(shader.Handle);

    /// <summary>
    ///     Checks if the shader attribute exists within the current shader.
    /// </summary>
    /// <param name="attribName">The name of the attribute that's being looked for.</param>
    /// <param name="location">Outputs the location of the attribute in the shader if found; otherwise -1.</param>
    /// <returns>If the attribute exists, <see langword="true"/>; otherwise, <see langword="false"/>. </returns>
    public static bool TryGetAttribLocation(this Shader shader, string attribName, out int location)
    {
        location = Window.GL.GetAttribLocation(shader.Handle, attribName);
        if (location == ShaderAttributes.AttributeLocationNotFound)
        {
            Console.WriteLine($"Attribute '{attribName}' not found in shader program.");
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

    private static bool TrySetUniform<T>(this Shader shader, string name, T data, Action<int, T> setter)
    {
        var uniforms = shader.GetUniformLocations();
        if (!uniforms.TryGetValue(name, out int uniform))
        {
            Console.WriteLine($"Uniform '{name}' not found in shader program.");
            return false;
        }

        setter(uniform, data);
        return true;
    }

    /// <summary>
    ///     Set a uniform int on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The data to set.</param>
    public static void SetInt(this Shader shader, string name, int data)
    {
        Window.GL.UseProgram(shader.Handle);
        shader.TrySetUniform(name, data, Window.GL.Uniform1);
    }

    /// <summary>
    ///     Set a uniform float on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The data to set.</param>
    public static void SetFloat(this Shader shader, string name, float data)
    {
        Window.GL.UseProgram(shader.Handle);
        shader.TrySetUniform(name, data, Window.GL.Uniform1);
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
    public static void SetMatrix4(this Shader shader, string name, Matrix4x4 data, bool transpose = true)
    {
        Window.GL.UseProgram(shader.Handle);
        shader.TrySetUniform(name, data, (uniform, d) => Window.GL.UniformMatrix4(uniform, transpose, d.ToSpan()));
    }

    /// <summary>
    ///     Set a uniform Vector2 on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The data to set.</param>
    public static void SetVector2(this Shader shader, string name, Vector2 data)
    {
        Window.GL.UseProgram(shader.Handle);
        shader.TrySetUniform(name, data, Window.GL.Uniform2);
    }

    /// <summary>
    ///     Set a uniform Vector3 on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The data to set.</param>
    public static void SetVector3(this Shader shader, string name, Vector3 data)
    {
        Window.GL.UseProgram(shader.Handle);
        shader.TrySetUniform(name, data, Window.GL.Uniform3);
    }

    /// <summary>
    ///     Set a uniform Vector3 on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The data to set.</param>
    public static void SetVector4(this Shader shader, string name, Vector4 data)
    {
        Window.GL.UseProgram(shader.Handle);
        shader.TrySetUniform(name, data, Window.GL.Uniform4);
    }
}
