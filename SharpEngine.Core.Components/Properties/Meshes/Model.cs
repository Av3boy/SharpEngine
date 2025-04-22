using SharpEngine.Core.Components.Properties.Meshes.MeshData;
using Silk.NET.Assimp;
using Silk.NET.OpenGL;
using System.Numerics;
using System.Linq;
using Mesh = SharpEngine.Core.Entities.Properties.Meshes.Mesh;
using Texture = SharpEngine.Core.Components.Properties.Textures.Texture;

namespace Tutorial
{
    public class Model : IDisposable
    {
        public readonly string Path;
        public List<Mesh> Meshes { get; set; } = new List<Mesh>();

        private readonly GL _gl;

        public Model(GL gl, string path)
        {
            _gl = gl;
            Path = path;
        }

        public Model(GL gl, string path, List<Mesh> meshes)
        {
            _gl = gl;
            Path = path;
            Meshes = meshes;

            foreach (var mesh in Meshes)
                ProcessMesh(mesh);
        }

        public void Dispose()
        {
            foreach (var mesh in Meshes)
            {
                mesh.Dispose();
            }

            GC.SuppressFinalize(this);
        }

        public Mesh ProcessMesh(Mesh mesh)
        {
            // data to fill
            for (int i = 0; i < mesh.Vertices2.Count; i++)
            {
                var vertex = mesh.Vertices2[i];
                vertex.BoneIds = new int[Vertex.MAX_BONE_INFLUENCE];
                vertex.Weights = new float[Vertex.MAX_BONE_INFLUENCE];
            }

            List<uint> indices = new List<uint>();
            foreach (var group in mesh.Groups)
            {
                foreach (var face in group.Faces)
                {
                    indices.AddRange(face._vertices.Select(v => (uint)v.VertexIndex));
                }
            }

            List<Texture> textures = new List<Texture>();
            foreach (var material in mesh.Materials)
            {
                if (material.DiffuseMap != null)
                    textures.Add(material.DiffuseMap);

                if (material.UseSpecularMap && material.SpecularMap != null)
                    textures.Add(material.SpecularMap);
            }

            // return a mesh object updated with the extracted mesh data
            mesh.Vertices = BuildVertices(mesh.Vertices2);
            mesh.Indices = BuildIndices(indices);
            mesh.Textures = textures;
            
            return mesh;
        }

        private float[] BuildVertices(List<Vertex> vertexCollection)
        {
            var vertices = new List<float>();

            foreach (var vertex in vertexCollection)
            {
                vertices.Add(vertex.Position.X);
                vertices.Add(vertex.Position.Y);
                vertices.Add(vertex.Position.Z);

                // vertices.Add(vertex.Normal.X);
                // vertices.Add(vertex.Normal.Y);

                vertices.Add(vertex.TexCoords.X);
                vertices.Add(vertex.TexCoords.Y);
            }

            return vertices.ToArray();
        }

        private uint[] BuildIndices(List<uint> indices)
            => indices.ToArray();
    }
}
