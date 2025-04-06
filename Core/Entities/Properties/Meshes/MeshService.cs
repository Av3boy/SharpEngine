using SharpEngine.Core.Windowing;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SharpEngine.Core.Entities.Properties.Meshes;

/// <summary>
///     Represents a service that loads meshes into the GPU.
/// </summary>
public class MeshService
{
    /// <summary>A global instance of the service.</summary>
    public static readonly MeshService Instance = new();

    private readonly Dictionary<string, Mesh> Meshes = [];

    private MeshService() { }

    /// <summary>
    ///     Loads a mesh into the mesh cache.
    /// </summary>
    /// <param name="identifier">Used to cache the loaded mesh.</param>
    /// <param name="mesh">The mesh that should be stored in to the GPU buffer.</param>
    /// <returns>The loaded mesh.</returns>
    public Mesh LoadMesh(string identifier, Mesh mesh)
    {
        if (Meshes.TryGetValue(identifier, out var cachedMesh))
            return cachedMesh;

        var meshData = mesh.GetVertices();

        var vertexBufferObject = Window.GL.GenBuffer();
        Window.GL.BindBuffer(GLEnum.ArrayBuffer, vertexBufferObject);
        Window.GL.BufferData<float>(GLEnum.ArrayBuffer, (uint)meshData.Length * sizeof(float), meshData, GLEnum.StaticDraw);

        Meshes.Add(identifier, mesh);
        return mesh;
    }

    const string FbxExtension = ".fbx";
    const string ObjExtension = ".obj";

    /// <summary>
    ///     Loads a mesh based on the file extension of the provided path.
    /// </summary>
    /// <param name="identifier">Used to specify the unique identifier for the mesh being loaded.</param>
    /// <param name="meshFilePath">Specifies the file path of the mesh to be loaded, which determines the loading method based on its extension.</param>
    /// <returns>A collection of <see cref="Mesh"/> objects loaded from the specified file.</returns>
    /// <exception cref="NotSupportedException">Thrown when the file extension of the provided path is not recognized as a supported mesh format.</exception>
    public IEnumerable<Mesh> LoadMesh(string identifier, string meshFilePath)
        => Path.GetExtension(meshFilePath) switch
        {
            FbxExtension => LoadFbx(identifier, meshFilePath),
            ObjExtension => LoadObj(identifier, meshFilePath),
            _ => throw new NotSupportedException($"The file extension {Path.GetExtension(meshFilePath)} is not a supported mesh file.")
        };

    // TODO: #2 Load obj mesh from file
    private IEnumerable<Mesh> LoadObj(string identifier, string meshFilePath)
    {
        var vertices = new List<float>();
        var normals = new List<float>();
        var textureCoordinates = new List<float>();
        var indices = new List<uint>();

        Mesh LoadMeshIntoBuffer()
        {
            var mesh = LoadMesh(identifier, new Mesh
            {
                Vertices = [.. vertices],
                Normals = [.. normals],
                TextureCoordinates = [.. textureCoordinates],
                Indices = [.. indices]
            });

            vertices.Clear();
            normals.Clear();
            textureCoordinates.Clear();
            indices.Clear();

            return mesh;
        }

        static IEnumerable<float> GetData(string[] parts) => parts.Skip(1).Select(float.Parse);

        var contents = File.ReadAllLines(meshFilePath);
        foreach (var line in contents)
        {
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) continue;

            switch (parts[0])
            {
                case ObjMeshData.VERTEX:
                    vertices.AddRange(GetData(parts));
                    break;
                case ObjMeshData.VERTEX_NORMAL:
                    normals.AddRange(GetData(parts));
                    break;
                case ObjMeshData.VERTEX_TEXTURE:
                    textureCoordinates.AddRange(GetData(parts));
                    break;
                case ObjMeshData.FACE: // OBJ faces are defined as a list of vertex indices
                    foreach (var part in parts.Skip(1))
                    {
                        var indicesParts = part.Split('/');
                        indices.Add(uint.Parse(indicesParts[0]) - 1); // OBJ indices are 1-based
                    }
                    break;
                case ObjMeshData.OBJECT:
                    if (vertices.Count > 0)
                        yield return LoadMeshIntoBuffer();
                    break;
            }
        }

        if (vertices.Count > 0)
            yield return LoadMeshIntoBuffer();
    }

    // TODO: #3 Load fbx mesh from file
    private IEnumerable<Mesh> LoadFbx(string identifier, string meshFilePath)
    {
        throw new NotImplementedException();
    }

}