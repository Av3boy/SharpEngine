using ObjLoader.Loader.Data.Elements;
using SharpEngine.Core.Components.Properties;
using SharpEngine.Core.Components.Properties.Meshes.MeshData;
using SharpEngine.Core.Entities.Properties.Meshes;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EngineTexture = SharpEngine.Core.Components.Properties.Textures.Texture;
using EngineTextureType = SharpEngine.Core.Components.Properties.Textures.TextureType;

namespace Tutorial
{
    public class Model : IDisposable
    {
        private readonly GL _gl;

        public string Path { get; }

        public List<Mesh> Meshes { get; set; } = [];

        public Model(GL gl, string path)
        {
            _gl = gl;
            Path = path;
        }

        public Model(GL gl, string path, IEnumerable<Mesh> meshes)
        {
            _gl = gl;
            Path = path;
            Meshes = meshes.Select(ProcessMesh).ToList();
        }

        public void Dispose()
        {
            foreach (var mesh in Meshes)
                mesh.Dispose();

            GC.SuppressFinalize(this);
        }

        public Mesh ProcessMesh(Mesh mesh)
        {
            ArgumentNullException.ThrowIfNull(mesh);

            if (mesh.Vertices.Length > 0)
                return CloneProcessedMesh(mesh);

            var vertices = ExpandVertices(mesh);
            var indices = Enumerable.Range(0, vertices.Count).Select(index => (uint)index).ToArray();
            var materials = BuildMaterials(mesh).ToList();
            var textures = BuildTextures(materials).ToList();

            return new Mesh(_gl, BuildVertices(vertices), indices, textures)
            {
                Name = mesh.Name,
                Vertices2 = vertices,
                Normals2 = mesh.Normals2,
                TextureCoordinates2 = mesh.TextureCoordinates2,
                Groups = mesh.Groups,
                Materials = materials,
            };
        }

        private Mesh CloneProcessedMesh(Mesh template)
        {
            var materials = template.Materials.Select(CloneMaterial).ToList();
            var textures = materials.Count > 0 ? BuildTextures(materials).ToList() : CloneTextures(template.Textures).ToList();

            return new Mesh(_gl, template.Vertices, template.Indices, textures)
            {
                Name = template.Name,
                Vertices2 = template.Vertices2,
                Normals2 = template.Normals2,
                TextureCoordinates2 = template.TextureCoordinates2,
                Groups = template.Groups,
                Materials = materials,
            };
        }

        private static List<Vertex> ExpandVertices(Mesh mesh)
        {
            var vertices = new List<Vertex>();

            foreach (var faceVertex in EnumerateFaceVertices(mesh.Groups))
                vertices.Add(CreateVertex(mesh, faceVertex));

            if (vertices.Count > 0)
                return vertices;

            return mesh.Vertices2.Select(vertex =>
            {
                vertex.BoneIds ??= new int[Vertex.MAX_BONE_INFLUENCE];
                vertex.Weights ??= new float[Vertex.MAX_BONE_INFLUENCE];
                return vertex;
            }).ToList();
        }

        private static IEnumerable<FaceVertex> EnumerateFaceVertices(IEnumerable<Group> groups)
        {
            foreach (var group in groups)
                foreach (var face in group.Faces)
                    for (int i = 0; i < face.Count; i++)
                        yield return face[i];
        }

        private static Vertex CreateVertex(Mesh mesh, FaceVertex faceVertex)
            => new()
            {
                BoneIds = new int[Vertex.MAX_BONE_INFLUENCE],
                Weights = new float[Vertex.MAX_BONE_INFLUENCE],
                Position = GetVertexPosition(mesh, faceVertex.VertexIndex),
                Normal = GetNormal(mesh, faceVertex.NormalIndex),
                TexCoords = GetTextureCoordinates(mesh, faceVertex.TextureIndex),
            };

        private static System.Numerics.Vector3 GetVertexPosition(Mesh mesh, int vertexIndex)
            => vertexIndex > 0 && vertexIndex <= mesh.Vertices2.Count
                ? mesh.Vertices2[vertexIndex - 1].Position
                : default;

        private static System.Numerics.Vector3 GetNormal(Mesh mesh, int normalIndex)
            => normalIndex > 0 && normalIndex <= mesh.Normals2.Count
                ? new System.Numerics.Vector3(
                    mesh.Normals2[normalIndex - 1].X,
                    mesh.Normals2[normalIndex - 1].Y,
                    mesh.Normals2[normalIndex - 1].Z)
                : default;

        private static System.Numerics.Vector2 GetTextureCoordinates(Mesh mesh, int textureIndex)
            => textureIndex > 0 && textureIndex <= mesh.TextureCoordinates2.Count
                ? new System.Numerics.Vector2(
                    mesh.TextureCoordinates2[textureIndex - 1].X,
                    mesh.TextureCoordinates2[textureIndex - 1].Y)
                : default;

        private IEnumerable<Material> BuildMaterials(Mesh mesh)
        {
            var materialDefinitions = mesh.Groups
                .Select(group => group.Material)
                .Where(material => material is not null)
                .DistinctBy(material => material.Name)
                .ToList();

            if (materialDefinitions.Count == 0)
                materialDefinitions = mesh.Materials
                    .Where(material => material is not null)
                    .DistinctBy(material => material.Name)
                    .ToList();

            foreach (var material in materialDefinitions)
            {
                var runtimeMaterial = CreateRuntimeMaterial(material);
                if (runtimeMaterial is not null)
                    yield return runtimeMaterial;
            }
        }

        private Material? CreateRuntimeMaterial(Material definition)
        {
            if (string.IsNullOrWhiteSpace(definition.DiffuseTextureMap))
                return null;

            var diffuseTexture = new EngineTexture(_gl, ResolveAssetPath(definition.DiffuseTextureMap), EngineTextureType.Diffuse);
            EngineTexture? specularTexture = null;

            if (!string.IsNullOrWhiteSpace(definition.SpecularTextureMap))
                specularTexture = new EngineTexture(_gl, ResolveAssetPath(definition.SpecularTextureMap), EngineTextureType.Specular);

            return new Material(diffuseTexture, specularTexture)
            {
                Name = definition.Name,
                AmbientColor = definition.AmbientColor,
                DiffuseColor = definition.DiffuseColor,
                SpecularColor = definition.SpecularColor,
                SpecularCoefficient = definition.SpecularCoefficient,
                Transparency = definition.Transparency,
                IlluminationModel = definition.IlluminationModel,
                DiffuseTextureMap = definition.DiffuseTextureMap,
                SpecularTextureMap = definition.SpecularTextureMap,
                AmbientTextureMap = definition.AmbientTextureMap,
                SpecularHighlightTextureMap = definition.SpecularHighlightTextureMap,
                AlphaTextureMap = definition.AlphaTextureMap,
                BumpMap = definition.BumpMap,
                DisplacementMap = definition.DisplacementMap,
                StencilDecalMap = definition.StencilDecalMap,
            };
        }

        private IEnumerable<EngineTexture> CloneTextures(IReadOnlyList<EngineTexture> textures)
            => textures?.Select(texture => new EngineTexture(_gl, texture.Path, texture.Type)) ?? [];

        private Material CloneMaterial(Material material)
        {
            var diffuseTexture = new EngineTexture(_gl, material.DiffuseMap.Path, material.DiffuseMap.Type);
            EngineTexture? specularTexture = null;

            if (material.UseSpecularMap)
                specularTexture = new EngineTexture(_gl, material.SpecularMap.Path, material.SpecularMap.Type);

            return new Material(diffuseTexture, specularTexture)
            {
                Name = material.Name,
                AmbientColor = material.AmbientColor,
                DiffuseColor = material.DiffuseColor,
                SpecularColor = material.SpecularColor,
                SpecularCoefficient = material.SpecularCoefficient,
                Transparency = material.Transparency,
                IlluminationModel = material.IlluminationModel,
                DiffuseTextureMap = material.DiffuseTextureMap,
                SpecularTextureMap = material.SpecularTextureMap,
                AmbientTextureMap = material.AmbientTextureMap,
                SpecularHighlightTextureMap = material.SpecularHighlightTextureMap,
                AlphaTextureMap = material.AlphaTextureMap,
                BumpMap = material.BumpMap,
                DisplacementMap = material.DisplacementMap,
                StencilDecalMap = material.StencilDecalMap,
                Specular = material.Specular,
                Shininess = material.Shininess,
            };
        }

        private static IEnumerable<EngineTexture> BuildTextures(IEnumerable<Material> materials)
            => materials
                .SelectMany(material => material.UseSpecularMap
                    ? new[] { material.DiffuseMap, material.SpecularMap }
                    : new[] { material.DiffuseMap })
                .Distinct();

        private string ResolveAssetPath(string assetPath)
        {
            if (System.IO.Path.IsPathRooted(assetPath) || string.IsNullOrWhiteSpace(Path))
                return assetPath;

            var directory = System.IO.Path.GetDirectoryName(Path);
            return string.IsNullOrWhiteSpace(directory)
                ? assetPath
                : System.IO.Path.Combine(directory, assetPath);
        }

        private static float[] BuildVertices(IEnumerable<Vertex> vertexCollection)
        {
            var vertices = new List<float>();

            foreach (var vertex in vertexCollection)
            {
                vertices.Add(vertex.Position.X);
                vertices.Add(vertex.Position.Y);
                vertices.Add(vertex.Position.Z);

                vertices.Add(vertex.Normal.X);
                vertices.Add(vertex.Normal.Y);
                vertices.Add(vertex.Normal.Z);

                vertices.Add(vertex.TexCoords.X);
                vertices.Add(vertex.TexCoords.Y);
            }

            return vertices.ToArray();
        }
    }
}
