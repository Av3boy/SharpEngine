using SharpEngine.Core.Components.ObjLoader.DataStore;
using SharpEngine.Core.Components.Properties.Meshes.MeshData;
using SharpEngine.Core.ObjLoader.TypeParsers;
using SharpEngine.Shared.Extensions;

using System;

namespace SharpEngine.Core.ObjLoader.Loader.TypeParsers
{
    /// <summary>
    ///     Parses face definitions ("f") and populates the current group's faces.
    /// </summary>
    public class FaceParser : TypeParserBase, ITypeParser
    {
        private readonly IFaceGroup _faceGroup;

        /// <summary>
        ///     Initializes a new instance of the <see cref="FaceParser"/>.
        /// </summary>
        /// <param name="faceGroup"></param>
        public FaceParser(IFaceGroup faceGroup)
        {
            _faceGroup = faceGroup;
        }

        /// <inheritdoc />
        protected override string Keyword => "f";

        /// <inheritdoc />
        public override bool Parse(string line)
        {
            string[] vertices = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var face = new Face();

            foreach (string vertexString in vertices)
            {
                if (TryParseFaceVertex(vertexString, out FaceVertex faceVertex))
                {
                    face.AddVertex(faceVertex);
                }
            }

            _faceGroup.AddFace(face);
            return true;
        }

        private static bool TryParseFaceVertex(string vertexString, out FaceVertex faceVertex)
        {
            string[] fields = vertexString.Split('/', StringSplitOptions.None);

            int vertexIndex = fields[0].ParseInvariantInt();
            faceVertex = new FaceVertex(vertexIndex, 0, 0);

            if (fields.Length > 1)
            {
                int textureIndex = fields[1].Length == 0 ? 0 : fields[1].ParseInvariantInt();
                faceVertex.TextureIndex = textureIndex;
            }

            if (fields.Length > 2)
            {
                int normalIndex = fields.Length > 2 && fields[2].Length == 0 ? 0 : fields[2].ParseInvariantInt();
                faceVertex.NormalIndex = normalIndex;
            }

            return true;
        }
    }
}