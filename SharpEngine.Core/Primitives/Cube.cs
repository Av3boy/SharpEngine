using SharpEngine.Core.Entities.Properties.Meshes;
using SharpEngine.Core.Windowing;
using Tutorial;

namespace SharpEngine.Core.Primitives;

/// <summary>
///     Used to create a primitive cube object.
/// </summary>
public static class Cube
{
    static Cube()
    {
        if (_loaded)
            return;

        var mesh = new Mesh(Window.GL)
        {
            Vertices = [.. Vertices],
            Normals = [.. Normals],
            TextureCoordinates = [.. TextureCoordinates],
            Indices = [.. Indices],
            Textures = [] //[defaultTexture],
            //Materials = [MaterialService.Instance.LoadMaterial(Default.DebugMaterial)],
            //Materials = [new(defaultTexture)]
        };

        Mesh = MeshService.Instance.LoadMesh(nameof(Cube), mesh);
        Model = new(Window.GL, Mesh);

        _loaded = true;
    }

    private static bool _loaded;

    public static Model_Old Model;

    /// <summary>The cube mesh.</summary>
    public static readonly Mesh Mesh;

    public static float[] Vertices =
        [
            -0.5f, -0.5f, -0.5f,
             0.5f, -0.5f, -0.5f,
             0.5f,  0.5f, -0.5f,
             0.5f,  0.5f, -0.5f,
            -0.5f,  0.5f, -0.5f,
            -0.5f, -0.5f, -0.5f,

            -0.5f, -0.5f,  0.5f,
             0.5f, -0.5f,  0.5f,
             0.5f,  0.5f,  0.5f,
             0.5f,  0.5f,  0.5f,
            -0.5f,  0.5f,  0.5f,
            -0.5f, -0.5f,  0.5f,

            -0.5f,  0.5f,  0.5f,
            -0.5f,  0.5f, -0.5f,
            -0.5f, -0.5f, -0.5f,
            -0.5f, -0.5f, -0.5f,
            -0.5f, -0.5f,  0.5f,
            -0.5f,  0.5f,  0.5f,

             0.5f,  0.5f,  0.5f,
             0.5f,  0.5f, -0.5f,
             0.5f, -0.5f, -0.5f,
             0.5f, -0.5f, -0.5f,
             0.5f, -0.5f,  0.5f,
             0.5f,  0.5f,  0.5f,

            -0.5f, -0.5f, -0.5f,
             0.5f, -0.5f, -0.5f,
             0.5f, -0.5f,  0.5f,
             0.5f, -0.5f,  0.5f,
            -0.5f, -0.5f,  0.5f,
            -0.5f, -0.5f, -0.5f,

            -0.5f,  0.5f, -0.5f,
             0.5f,  0.5f, -0.5f,
             0.5f,  0.5f,  0.5f,
             0.5f,  0.5f,  0.5f,
            -0.5f,  0.5f,  0.5f,
            -0.5f,  0.5f, -0.5f,
        ];
    public static float[] Normals =
    [
          0.0f, 0.0f, -1.0f,
              0.0f, 0.0f, -1.0f,
              0.0f, 0.0f, -1.0f,
              0.0f, 0.0f, -1.0f,
              0.0f, 0.0f, -1.0f,
              0.0f, 0.0f, -1.0f,

              0.0f, 0.0f, 1.0f,
              0.0f, 0.0f, 1.0f,
              0.0f, 0.0f, 1.0f,
              0.0f, 0.0f, 1.0f,
              0.0f, 0.0f, 1.0f,
              0.0f, 0.0f, 1.0f,

             -1.0f, 0.0f, 0.0f,
             -1.0f, 0.0f, 0.0f,
             -1.0f, 0.0f, 0.0f,
             -1.0f, 0.0f, 0.0f,
             -1.0f, 0.0f, 0.0f,
             -1.0f, 0.0f, 0.0f,

              1.0f, 0.0f, 0.0f,
              1.0f, 0.0f, 0.0f,
              1.0f, 0.0f, 0.0f,
              1.0f, 0.0f, 0.0f,
              1.0f, 0.0f, 0.0f,
              1.0f, 0.0f, 0.0f,

              0.0f, -1.0f, 0.0f,
              0.0f, -1.0f, 0.0f,
              0.0f, -1.0f, 0.0f,
              0.0f, -1.0f, 0.0f,
              0.0f, -1.0f, 0.0f,
              0.0f, -1.0f, 0.0f,

              0.0f, 1.0f, 0.0f,
              0.0f, 1.0f, 0.0f,
              0.0f, 1.0f, 0.0f,
              0.0f, 1.0f, 0.0f,
              0.0f, 1.0f, 0.0f,
              0.0f, 1.0f, 0.0f,
        ];
    public static float[] TextureCoordinates =
        [
              0.0f, 0.0f,
              1.0f, 0.0f,
              1.0f, 1.0f,
              1.0f, 1.0f,
              0.0f, 1.0f,
              0.0f, 0.0f,

              0.0f, 0.0f,
              1.0f, 0.0f,
              1.0f, 1.0f,
              1.0f, 1.0f,
              0.0f, 1.0f,
              0.0f, 0.0f,

              1.0f, 0.0f,
              1.0f, 1.0f,
              0.0f, 1.0f,
              0.0f, 1.0f,
              0.0f, 0.0f,
              1.0f, 0.0f,

              1.0f, 0.0f,
              1.0f, 1.0f,
              0.0f, 1.0f,
              0.0f, 1.0f,
              0.0f, 0.0f,
              1.0f, 0.0f,

              0.0f, 1.0f,
              1.0f, 1.0f,
              1.0f, 0.0f,
              1.0f, 0.0f,
              0.0f, 0.0f,
              0.0f, 1.0f,

              0.0f, 1.0f,
              1.0f, 1.0f,
              1.0f, 0.0f,
              1.0f, 0.0f,
              0.0f, 0.0f,
              0.0f, 1.0f
        ];
    public static uint[] Indices =
        [
            // Front face
            0, 1, 2,
            2, 3, 0,

            // Back face
            4, 5, 6,
            6, 7, 4,

            // Left face
            4, 0, 3,
            3, 7, 4,

            // Right face
            1, 5, 6,
            6, 2, 1,

            // Top face
            3, 2, 6,
            6, 7, 3,

            // Bottom face
            4, 5, 1,
            1, 0, 4
        ];
}
