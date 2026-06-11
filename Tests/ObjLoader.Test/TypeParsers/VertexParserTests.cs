using FluentAssertions;
using Xunit;

using SharpEngine.Core.Components.ObjLoader.DataStore;
using SharpEngine.Core.Components.Properties.Meshes.MeshData;
using SharpEngine.Core.ObjLoader.Loader.TypeParsers;

namespace SharpEngine.Core.ObjLoader.Tests.TypeParsers
{
    public class VertexParserTests
    {
        private readonly VertexDataStoreMock _vertexDataStoreMock;
        private readonly VertexParser _vertexParser;

        public VertexParserTests()
        {
            _vertexDataStoreMock = new VertexDataStoreMock();
            _vertexParser = new VertexParser(_vertexDataStoreMock);
        }

        [Fact]
        public void CanParse_returns_false_on_non_vertex_line()
        {
            const string invalidVertex = "vt";

            bool canParse = _vertexParser.CanParse(invalidVertex);
            canParse.Should().BeFalse();
        }

        [Fact]
        public void CanParse_returns_true_on_vertex_line()
        {
            const string vertexLine = "v";

            bool canParse = _vertexParser.CanParse(vertexLine);
            canParse.Should().BeTrue();
        }

        [Fact]
        public void Parses_vertex_line_correctly()
        {
            const string vertexLine = "0.123 0.234 0.345";
            _vertexParser.Parse(vertexLine);

            var parsedNormal = _vertexDataStoreMock.ParsedVertex.Position;
            parsedNormal.X.Should().BeApproximately(0.123f, 0.000001f);
            parsedNormal.Y.Should().BeApproximately(0.234f, 0.000001f);
            parsedNormal.Z.Should().BeApproximately(0.345f, 0.000001f);
        }

        class VertexDataStoreMock : IVertexDataStore
        {
            public Vertex ParsedVertex { get; set; }

            public Vertex GetVertex(int i)
            {
                throw new System.NotImplementedException();
            }

            public void AddVertex(Vertex vertex)
            {
                ParsedVertex = vertex;
            }
        }
    }
}