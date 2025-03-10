﻿using System;
using System.IO;
using System.Collections.Generic;

using System.Text.RegularExpressions;
using System.Numerics;

using Silk.NET.OpenGL;

namespace SharpEngine.Core.Shaders;

/// <summary>
///     Represents a shader program.
/// </summary>
public class Shader
{
    /// <summary>Gets the handle to the shader program.</summary>
    public readonly uint Handle;

    /// <summary>Gets or sets the identifying name of the shader.</summary>
    public string Name { get; set; }

    private Dictionary<string, int> _uniformLocations = [];

    /// <summary>
    ///    Initializes a new instance of <see cref="Shader"/>.
    /// </summary>
    /// <remarks>
    ///     Shaders are written in GLSL, which is a language very similar to C in its semantics.
    ///     The GLSL source is compiled *at runtime*, so it can optimize itself for the graphics card it's currently being used on.
    /// </remarks>
    /// <param name="vertPath">The vertex shader full path.</param>
    /// <param name="fragPath">The fragment shader full path.</param>
    /// <param name="name">The identfier name of the shader.</param>
    public Shader(string vertPath, string fragPath, string name)
    {
        Name = name;

        // There are several different types of shaders, but the only two you need for basic rendering are the vertex and fragment shaders.
        // The vertex shader is responsible for moving around vertices, and uploading that data to the fragment shader.
        //   The vertex shader won't be too important here, but they'll be more important later.
        // The fragment shader is responsible for then converting the vertices to "fragments", which represent all the data OpenGL needs to draw a pixel.
        //   The fragment shader is what we'll be using the most here.

        // Load and compile shader
        if (!LoadShader(ShaderType.VertexShader, vertPath, out uint vertexShader))
            return;

        if (!LoadShader(ShaderType.FragmentShader, fragPath, out uint fragmentShader))
            return;

        // These two shaders must then be merged into a shader program, which can then be used by OpenGL.
        // To do this, create a program...

        Handle = Window.GL.CreateProgram();

        // Attach both shaders...
        Window.GL.AttachShader(Handle, vertexShader);
        Window.GL.AttachShader(Handle, fragmentShader);

        // And then link them together.
        LinkProgram(Handle);

        // When the shader program is linked, it no longer needs the individual shaders attached to it; the compiled code is copied into the shader program.
        // Detach them, and then delete them.
        Window.GL.DetachShader(Handle, vertexShader);
        Window.GL.DetachShader(Handle, fragmentShader);
        Window.GL.DeleteShader(fragmentShader);
        Window.GL.DeleteShader(vertexShader);

        // The shader is now ready to go, but first, we're going to cache all the shader uniform locations.
        // Querying this from the shader is very slow, so we do it once on initialization and reuse those values
        // later.

        GetUniformLocations();
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
        CompileShader(shader);

        return true;
    }

    private void GetUniformLocations()
    {
        // First, we have to get the number of active uniforms in the shader.
        Window.GL.GetProgram(Handle, GLEnum.ActiveUniforms, out var numberOfUniforms);

        // Next, allocate the dictionary to hold the locations.
        _uniformLocations = new Dictionary<string, int>();

        // Loop over all the uniforms,
        for (uint i = 0; i < numberOfUniforms; i++)
        {
            // get the name of this uniform,
            var key = Window.GL.GetActiveUniform(Handle, i, out _, out _);

            // get the location,
            var location = Window.GL.GetUniformLocation(Handle, key);

            // and then add it to the dictionary.
            _uniformLocations.Add(key, location);
        }
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

    private static void CompileShader(uint shader)
    {
        // Try to compile the shader
        Window.GL.CompileShader(shader);

        // Check for compilation errors
        Window.GL.GetShader(shader, GLEnum.CompileStatus, out var code);
        if (code != (int)GLEnum.True)
        {
            // We can use `GL.GetShaderInfoLog(shader)` to get information about the error.
            var infoLog = Window.GL.GetShaderInfoLog(shader);
            throw new InvalidOperationException($"Error occurred whilst compiling Shader({shader}).\n\n{infoLog}");
        }
    }

    private static void LinkProgram(uint program)
    {
        Window.GL.LinkProgram(program);

        Window.GL.GetProgram(program, GLEnum.LinkStatus, out var code);
        if (code != (int)GLEnum.True)
        {
            string infoLog = Window.GL.GetProgramInfoLog(program);
            throw new InvalidOperationException($"Error occurred whilst linking Program({program}): {infoLog}");
        }
    }

    /// <summary>
    ///     Enables the shader program.
    /// </summary>
    public void Use()
        => Window.GL.UseProgram(Handle);

    /// <summary>
    ///     Checks if the shader attribute exists within the current shader.
    /// </summary>
    /// <param name="attribName">The name of the attribute that's being looked for.</param>
    /// <returns>If the attribute exists, the location of the attribute in the shader; otherwise -1.</returns>
    public int GetAttribLocation(string attribName)
    {
        int location = Window.GL.GetAttribLocation(Handle, attribName);
        if (location == -1)
            Console.WriteLine($"Attribute '{attribName}' not found in shader program.");

        return location;
    }

    // Uniform setters
    // Uniforms are variables that can be set by user code, instead of reading them from the VBO.
    // You use VBOs for vertex-related data, and uniforms for almost everything else.

    // Setting a uniform is almost always the exact same, so I'll explain it here once, instead of in every method:
    //     1. Bind the program you want to set the uniform on
    //     2. Get a handle to the location of the uniform with GL.GetUniformLocation.
    //     3. Use the appropriate GL.Uniform* function to set the uniform.

    /// <summary>
    ///     Set a uniform int on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The data to set.</param>
    public void SetInt(string name, int data)
    {
        Window.GL.UseProgram(Handle);
        Window.GL.Uniform1(_uniformLocations[name], data);
    }

    /// <summary>
    ///     Set a uniform float on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The data to set.</param>
    public void SetFloat(string name, float data)
    {
        Window.GL.UseProgram(Handle);
        Window.GL.Uniform1(_uniformLocations[name], data);
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
    public void SetMatrix4(string name, Matrix4x4 data, bool transpose = true)
    {
        Window.GL.UseProgram(Handle);
        Window.GL.UniformMatrix4(_uniformLocations[name], transpose, data.ToSpan());
    }

    /// <summary>
    ///     Set a uniform Vector3 on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The data to set.</param>
    public void SetVector3(string name, Vector3 data)
    {
        Window.GL.UseProgram(Handle);
        Window.GL.Uniform3(_uniformLocations[name], data);
    }
}
