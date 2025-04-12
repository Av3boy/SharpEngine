using Silk.NET.OpenGL;

namespace SharpEngine.Core.Shaders;

/// <summary>
///     Represents a shader program.
/// </summary>
public partial class Shader : IDisposable
{
    /// <summary>Gets the handle to the shader program.</summary>
    public uint Handle;

    /// <summary>Gets or sets the identifying name of the shader.</summary>
    public string Name { get; set; }
    public string VertPath { get; set; }
    public string FragPath { get; set; }

    private Dictionary<string, int> _uniformLocations = [];

    private readonly GL _gl;

    /// <summary>
    ///    Initializes a new instance of <see cref="Shader"/>.
    /// </summary>
    /// <remarks>
    ///     Shaders are written in GLSL, which is a language very similar to C in its semantics.
    ///     The GLSL source is compiled *at runtime*, so it can optimize itself for the graphics card it's currently being used on.
    /// </remarks>
    /// <param name="vertPath">The vertex shader full path.</param>
    /// <param name="fragPath">The fragment shader full path.</param>
    /// <param name="name">The identifier name of the shader.</param>
    public Shader(GL gl, string vertPath, string fragPath, string name)
    {
        Name = name;
        _gl = gl;

        // There are several different types of shaders, but the only two you need for basic rendering are the vertex and fragment shaders.
        // The vertex shader is responsible for moving around vertices, and uploading that data to the fragment shader.
        //   The vertex shader won't be too important here, but they'll be more important later.
        // The fragment shader is responsible for then converting the vertices to "fragments", which represent all the data OpenGL needs to draw a pixel.
        //   The fragment shader is what we'll be using the most here.

        if (!vertPath.EndsWith(".vert"))
            Console.WriteLine("Vertex shaders should have the file extension '.vert' for easier manageability.");

        if (!fragPath.EndsWith(".frag"))
            Console.WriteLine("Fragment shaders should have the file extension '.frag' for easier manageability.");

        VertPath = vertPath;
        FragPath = fragPath;
    }

    public void InitializeUniforms(Dictionary<string, int> uniforms)
        => _uniformLocations = uniforms;

    public Dictionary<string, int> GetUniformLocations() => _uniformLocations;

    public void Dispose()
    {
        _gl.DeleteProgram(Handle);
    }
}
