using SharpEngine.Core._Resources;
using SharpEngine.Core.Components.Properties;
using SharpEngine.Core.Entities.Properties.Meshes;
using SharpEngine.Core.Textures;
using SharpEngine.Core.Windowing;
using System.Collections.Generic;
using Tutorial;
using TextureType = Silk.NET.Assimp.TextureType;

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

        var defaultTexture = TextureService.Instance.LoadTexture(Default.DebugTexture);

        var mesh = new Mesh(Window.SharedGL)
        {
            Vertices = [.. Vertices],
            Normals = [.. Normals],
            TextureCoordinates = [.. TextureCoordinates],
            Indices = [.. Indices],
            Textures = [defaultTexture],
            // Materials = [MaterialService.Instance.LoadMaterial(Default.DebugMaterial)],
            Materials = [new(defaultTexture)]
        };

        Mesh = MeshService.Instance.LoadMesh(nameof(Cube), mesh);
        Model = new(Window.SharedGL, Mesh);

        _loaded = true;
    }

    private static bool _loaded;

    public static Model_Old Model;

    public static Model_Old CreateModel(string diffuseMapFile, string? specularMapFile = null)
    {
        var diffuseTexture = TextureService.Instance.LoadTexture(diffuseMapFile, TextureType.Diffuse);
        var specularTexture = string.IsNullOrWhiteSpace(specularMapFile)
            ? null
            : TextureService.Instance.LoadTexture(specularMapFile, TextureType.Specular);

        var material = new Material(diffuseTexture, specularTexture);
        var textures = new List<Components.Properties.Textures.Texture> { diffuseTexture };

        if (specularTexture is not null && specularTexture.Handle != diffuseTexture.Handle)
            textures.Add(specularTexture);

        var mesh = new Mesh(Window.SharedGL, BuildVertices(), [], textures)
        {
            Name = Mesh.Name,
            Materials = [material]
        };

        var model = new Model_Old(Window.SharedGL, string.Empty);
        model.Meshes.Add(mesh);

        return model;
    }

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

    private static float[] BuildVertices()
    {
        var vertices = new List<float>(Vertices.Length + Normals.Length + TextureCoordinates.Length);

        for (int i = 0; i < Vertices.Length / 3; i++)
        {
            var vertexIndex = i * 3;
            var texCoordIndex = i * 2;

            vertices.Add(Vertices[vertexIndex]);
            vertices.Add(Vertices[vertexIndex + 1]);
            vertices.Add(Vertices[vertexIndex + 2]);

            vertices.Add(Normals[vertexIndex]);
            vertices.Add(Normals[vertexIndex + 1]);
            vertices.Add(Normals[vertexIndex + 2]);

            vertices.Add(TextureCoordinates[texCoordIndex]);
            vertices.Add(TextureCoordinates[texCoordIndex + 1]);
        }

        return [.. vertices];
    }
}
