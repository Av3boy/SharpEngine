using SharpEngine.Core.Components.Properties.Meshes.MeshData;
using SharpEngine.Core.Entities.Properties.Meshes;
using EngineTexture = SharpEngine.Core.Components.Properties.Textures.Texture;
using EngineTextureType = SharpEngine.Core.Components.Properties.Textures.TextureType;

using Silk.NET.OpenGL;

namespace SharpEngine.Core.Components.Properties.Meshes
{
    /// <summary>
    ///     Represents a 3D model, which may consist of multiple meshes, materials, and textures. 
    /// </summary>
    /// <remarks>
    ///     This class is responsible for processing raw mesh data into a format suitable for rendering, including expanding vertices based on face definitions and building materials and textures as needed. 
    /// </remarks>
    public class Model : IDisposable
    {
        private readonly GL _gl;

        /// <summary>Gets the file path to where the model is stored.</summary>
        public string Path { get; }

        /// <summary>Gets the list of meshes contained within the model.</summary>
        public List<Mesh> Meshes { get; set; } = [];

        public string Name { get; }

        /// <summary>
        ///     Initializes a new instance of the Model class with the specified OpenGL context and model file path.
        /// </summary>
        /// <param name="gl">The GL context used for rendering and resource management.</param>
        /// <param name="path">The file path of the model asset.</param>
        public Model(GL gl, string path) : this(gl, path, Enumerable.Empty<Mesh>())
        {
            _gl = gl;
            Path = path;
        }

        /// <summary>
        ///     Initializes a new instance of the Model class, stores the GL context and path, and processes the provided meshes.
        /// </summary>
        /// <remarks>
        ///     Input meshes are enumerated and processed immediately to populate the Model's Meshes collection.
        /// </remarks>
        /// <param name="gl">The OpenGL context used for creating and managing rendering resources.</param>
        /// <param name="path">The file path of the model resource.</param>
        /// <param name="meshes">A sequence of meshes to be processed and stored; each mesh is converted via ProcessMesh.</param>
        public Model(GL gl, string path, IEnumerable<Mesh> meshes)
        {
            _gl = gl;
            Path = path;

            if (meshes.Any())
                Meshes = meshes.Select(ProcessMesh).ToList();

            Name = meshes.FirstOrDefault()?.Name ?? System.IO.Path.GetFileNameWithoutExtension(path) ?? "UnnamedModel";
        }

        /// <inheritdoc />
        public void Dispose()
        {
            foreach (var mesh in Meshes)
                mesh.Dispose();

            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Processes the provided mesh in place, updating its vertex data, materials, textures, and GPU buffers so it is ready for rendering.
        /// </summary>
        /// <param name="mesh">The mesh to process. Must not be null.</param>
        /// <returns>
        ///     The same <paramref name="mesh"/> instance with updated <see cref="Mesh.Vertices"/>, <see cref="Mesh.Indices"/>,
        ///     <see cref="Mesh.Textures"/>, <see cref="Mesh.Materials"/>, and reinitialized GPU buffers.
        ///     When the mesh already carries GPU-ready vertex data (<see cref="Mesh.Vertices"/> is non-empty) its
        ///     materials and textures are refreshed in place; otherwise vertices are expanded from face groups and all
        ///     data properties are computed and assigned.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="mesh"/> is null.</exception>
        public Mesh ProcessMesh(Mesh mesh)
        {
            ArgumentNullException.ThrowIfNull(mesh);

            if (mesh.Vertices.Length > 0)
            {
                var existingMaterials = mesh.Materials.Select(CloneMaterial).ToList();
                var existingTextures = (existingMaterials.Count > 0 ? BuildTextures(existingMaterials) : CloneTextures(mesh.Textures)).ToList();
                mesh.Materials = existingMaterials;
                mesh.Textures = existingTextures;
                mesh.ReinitializeGpuBuffers();
                return mesh;
            }

            var vertices = ExpandVertices(mesh);
            var indices = Enumerable.Range(0, vertices.Count).Select(index => (uint)index).ToArray();
            var materials = BuildMaterials(mesh).ToList();
            var textures = BuildTextures(materials).ToList();

            mesh.Vertices = BuildVertices(vertices);
            mesh.Indices = indices;
            mesh.Textures = textures;
            mesh.Vertices2 = vertices;
            mesh.Materials = materials;
            mesh.ReinitializeGpuBuffers();
            return mesh;
        }

        private static List<Vertex> ExpandVertices(Mesh mesh)
        {
            var vertices = new List<Vertex>();

            foreach (var faceVertex in EnumerateFaceVertices(mesh.Groups))
                vertices.Add(CreateVertex(mesh, faceVertex));

            if (vertices.Count > 0)
                return vertices;

            return [.. mesh.Vertices2.Select(vertex =>
            {
                vertex.BoneIds ??= new int[Vertex.MAX_BONE_INFLUENCE];
                vertex.Weights ??= new float[Vertex.MAX_BONE_INFLUENCE];
                return vertex;
            })];
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
            => textureIndex > 0 && textureIndex <= mesh.TextureCoordinates2.Count ? 
                new System.Numerics.Vector2(mesh.TextureCoordinates2[textureIndex - 1].X, mesh.TextureCoordinates2[textureIndex - 1].Y) : 
                default;

        private IEnumerable<Material> BuildMaterials(Mesh mesh)
        {
            var materialDefinitions = mesh.Groups
                .Select(group => group.Material)
                .Where(material => material is not null)
                .DistinctBy(material => material!.Name)
                .ToList();

            if (materialDefinitions.Count == 0)
                materialDefinitions = [.. mesh.Materials
                    .Where(material => material is not null)
                    .DistinctBy(material => material!.Name)];

            foreach (var material in materialDefinitions)
            {
                var runtimeMaterial = CreateRuntimeMaterial(material!);
                if (runtimeMaterial is not null)
                    yield return runtimeMaterial;
            }
        }

        private Material? CreateRuntimeMaterial(Material definition)
        {
            if (string.IsNullOrWhiteSpace(definition.DiffuseMap?.Path))
                return null;

            var diffuseTexture = new EngineTexture(_gl, ResolveAssetPath(definition.DiffuseMap.Path), EngineTextureType.Diffuse);
            EngineTexture? specularTexture = null;

            if (!string.IsNullOrWhiteSpace(definition.SpecularMap?.Path))
                specularTexture = new EngineTexture(_gl, ResolveAssetPath(definition.SpecularMap.Path), EngineTextureType.Specular);

            return new Material(definition.Name, diffuseTexture, specularTexture)
            {
                Name = definition.Name,
                AmbientColor = definition.AmbientColor,
                DiffuseColor = definition.DiffuseColor,
                SpecularColor = definition.SpecularColor,
                SpecularCoefficient = definition.SpecularCoefficient,
                Transparency = definition.Transparency,
                IlluminationModel = definition.IlluminationModel,
                DiffuseMap = definition.DiffuseMap,
                SpecularMap = definition.SpecularMap,
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
            EngineTexture? diffuseTexture = null;
            if (material.DiffuseMap?.Texture is not null)
                diffuseTexture = new EngineTexture(_gl, material.DiffuseMap.Path, material.DiffuseMap.Texture.Type);

            EngineTexture? specularTexture = null;
            if (material.SpecularMap?.Texture is not null)
                specularTexture = new EngineTexture(_gl, material.SpecularMap.Path, material.SpecularMap.Texture.Type);

            return new Material(material.Name, diffuseTexture, specularTexture)
            {
                Name = material.Name,
                AmbientColor = material.AmbientColor,
                DiffuseColor = material.DiffuseColor,
                SpecularColor = material.SpecularColor,
                SpecularCoefficient = material.SpecularCoefficient,
                Transparency = material.Transparency,
                IlluminationModel = material.IlluminationModel,
                DiffuseMap = material.DiffuseMap,
                SpecularMap = material.SpecularMap,
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
                .SelectMany(GetMaterialEngineTextures)
                .Distinct();
        private static EngineTexture[] GetMaterialEngineTextures(Material material)
        {
            ArgumentNullException.ThrowIfNull(material);

            var materials = new List<EngineTexture>();
            if (material.SpecularMap?.Texture is not null)
                materials.Add(material.SpecularMap.Texture!);

            if (material.DiffuseMap?.Texture is not null)
                materials.Add(material.DiffuseMap.Texture!);

            return [.. materials];
        }

        private string ResolveAssetPath(string assetPath)
        {
            if (System.IO.Path.IsPathRooted(assetPath) || string.IsNullOrWhiteSpace(Path))
                return assetPath;

            var directory = System.IO.Path.GetDirectoryName(Path);

            return string.IsNullOrWhiteSpace(directory) ? 
                assetPath : System.IO.Path.Combine(directory, assetPath);
        }

        /// <summary>
        ///     Converts a collection of <see cref="Vertex"/> objects into an interleaved float array
        ///     (position XYZ, normal XYZ, texture-coordinate UV per vertex) suitable for uploading to a GPU vertex buffer.
        /// </summary>
        /// <param name="vertexCollection">The vertices to convert.</param>
        /// <returns>A flat <see cref="float"/> array with eight elements per vertex.</returns>
        internal static float[] BuildVertices(IEnumerable<Vertex> vertexCollection)
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

            return [.. vertices];
        }
    }
}
