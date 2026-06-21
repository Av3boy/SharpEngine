using Microsoft.Extensions.Logging;
using SharpEngine.Telemetry;
using Silk.NET.OpenGL;
using System.Text.RegularExpressions;

namespace SharpEngine.Core.Shaders;

public abstract class ShaderProgram : IDisposable
{
    protected readonly ILogger<ShaderProgram> _logger;

    protected readonly GL GL;

    protected Dictionary<string, int> _uniformLocations = [];
    private bool disposedValue;

    /// <summary>Gets the handle to the shader program.</summary>
    public uint Handle { get; protected set; }

    /// <summary>Gets or sets the vertex array object.</summary>
    public uint Vao { get; set; }

    /// <summary>
    ///    Sets the attributes for the shader.
    /// </summary>
    /// <returns>
    ///     <see langword="true" /> if the attributes were set successfully; otherwise <see langword="false" />.
    /// </returns>
    public virtual bool SetAttributes(GL gl) => true;

    protected ShaderProgram(GL gl)
    {
        GL = gl;
        _logger = LoggingExtensions.CreateLogger<ShaderProgram>();
    }

    /// <summary>
    ///     Initializes and compiles vertex and fragment shaders into a shader program.
    /// </summary>
    protected void Initialize(string vertPath, string fragPath)
    {
        // Load and compile shader
        if (!LoadShader(ShaderType.VertexShader, vertPath, out uint vertexShader))
        {
            _logger.LogError("Unable to load vertex shader.");
            return;
        }

        if (!LoadShader(ShaderType.FragmentShader, fragPath, out uint fragmentShader))
        {
            _logger.LogError("Unable to load fragment shader.");
            return;
        }
        // These two shaders must then be merged into a shader program, which can then be used by OpenGL.
        // To do this, create a program...

        Handle = GL.CreateProgram();

        // Attach both shaders...
        GL.AttachShader(Handle, vertexShader);
        GL.AttachShader(Handle, fragmentShader);

        // And then link them together.
        bool shaderLinked = LinkProgram(Handle);

        // When the shader program is linked, it no longer needs the individual shaders attached to it; the compiled code is copied into the shader program.
        // Detach them, and then delete them.
        GL.DetachShader(Handle, vertexShader);
        GL.DetachShader(Handle, fragmentShader);
        GL.DeleteShader(fragmentShader);
        GL.DeleteShader(vertexShader);

        if (!shaderLinked)
        {
            _logger.LogInformation("Unable to link shader program.");
            return;
        }

        // The shader is now ready to go, but first, we're going to cache all the shader uniform locations.
        // Querying this from the shader is very slow, so we do it once on initialization and reuse those values
        // later.
        SetUniformLocations(Handle);

        Vao = GL.GenVertexArray();
        GL.BindVertexArray(Vao);

        SetAttributes(GL);

        return;
    }

    private bool LoadShader(ShaderType shaderType, string shaderPath, out uint shaderProgram)
    {
        if (!File.Exists(shaderPath))
        {
            _logger.LogInformation("Shader file not found: {Path}", shaderPath);

            shaderProgram = 0;
            return false;
        }

        string shaderSource = File.ReadAllText(shaderPath);
        shaderSource = ProcessIncludes(shaderSource, Path.GetDirectoryName(shaderPath)!);

        // GL.CreateShader will create an empty shader (obviously). The ShaderType enum denotes which type of shader will be created.
        shaderProgram = GL.CreateShader(shaderType);
        GL.ShaderSource(shaderProgram, shaderSource);

        if (!CompileShader(shaderProgram))
        {
            _logger.LogInformation("Unable to load {Type} shader from '{Path}'.", shaderType, shaderPath);
            return false;
        }

        return true;
    }

    private void SetUniformLocations(uint shaderProgramHandle)
    {
        // First, we have to get the number of active uniforms in the shader.
        GL.GetProgram(shaderProgramHandle, GLEnum.ActiveUniforms, out var numberOfUniforms);

        Dictionary<string, int> uniformLocations = [];

        // Loop over all the uniforms,
        for (uint i = 0; i < numberOfUniforms; i++)
        {
            // get the name of this uniform,
            var key = GL.GetActiveUniform(shaderProgramHandle, i, out _, out _);

            // get the location,
            var location = GL.GetUniformLocation(shaderProgramHandle, key);

            // and then add it to the dictionary.
            uniformLocations.Add(key, location);
        }

        _uniformLocations = uniformLocations;
    }

    private static string ProcessIncludes(string shaderCode, string directory)
    {
        const string includePattern = @"#include\s+""(.+?)""";
        return Regex.Replace(shaderCode, includePattern, match =>
        {
            string includePath = Path.Combine(directory, match.Groups[1].Value);
            string includeCode = File.ReadAllText(includePath);
            return ProcessIncludes(includeCode, Path.GetDirectoryName(includePath)!);
        }, RegexOptions.NonBacktracking);
    }

    private bool CompileShader(uint shaderProgram)
    {
        // Try to compile the shader
        GL.CompileShader(shaderProgram);

        // Check for compilation errors
        GL.GetShader(shaderProgram, GLEnum.CompileStatus, out var statusCode);
        if (statusCode != (int)GLEnum.True)
        {
            // We can use `GL.GetShaderInfoLog(shader)` to get information about the error.
            var infoLog = GL.GetShaderInfoLog(shaderProgram);
            _logger.LogError("Error occurred whilst compiling Shader({Shader}).\n\n{Log}", shaderProgram, infoLog);

            return false;
        }

        return true;
    }

    private bool LinkProgram(uint program)
    {
        GL.LinkProgram(program);
        GL.GetProgram(program, GLEnum.LinkStatus, out var statusCode);

        if (statusCode != (int)GLEnum.True)
        {
            string infoLog = GL.GetProgramInfoLog(program);
            _logger.LogError("Error occurred whilst linking Program({Program}): {Info}", program, infoLog);

            return false;
        }

        return true;
    }

    /// <summary>
    ///     Enables the shader program.
    /// </summary>
    public void Use()
        => GL.UseProgram(Handle);

    /// <summary>
    ///     Checks if the shader attribute exists within the current shader.
    /// </summary>
    /// <param name="attribName">The name of the attribute that's being looked for.</param>
    /// <param name="location">Outputs the location of the attribute in the shader if found; otherwise -1.</param>
    /// <returns>If the attribute exists, <see langword="true"/>; otherwise, <see langword="false"/>. </returns>
    public bool TryGetAttribLocation(string attribName, out int location)
    {
        location = GL.GetAttribLocation(Handle, attribName);
        if (location == ShaderAttributes.AttributeLocationNotFound)
        {
            _logger.LogWarning("Attribute '{Attribute}' not found in shader program.", attribName);
            return false;
        }

        return true;
    }

    /// <inheritdoc />
    protected virtual void Dispose(bool disposing)
    {
        if (disposedValue)
            return;

        if (disposing)
        {
            // TODO: Collect handles in a separate container with proper access to the shared GL context.
            //_gl.DeleteProgram(Handle);
            Handle = 0;
            _uniformLocations.Clear();
        }

        disposedValue = true;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
