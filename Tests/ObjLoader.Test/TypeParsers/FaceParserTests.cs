using FluentAssertions;
using Xunit;

using SharpEngine.Core.Components.ObjLoader.DataStore;
using SharpEngine.Core.Components.Properties.Meshes.MeshData;
using SharpEngine.Core.ObjLoader.Loader.TypeParsers;

namespace SharpEngine.Core.ObjLoader.Tests.TypeParsers
{
    public class FaceParserTests
    {
        private readonly FaceGroupSpy _faceGroupSpy;
        private readonly FaceParser _faceParser;

        public FaceParserTests()
        {
            _faceGroupSpy = new FaceGroupSpy();
            _faceParser = new FaceParser(_faceGroupSpy);
        }

        [Fact]
        public void CanParse_returns_true_on_face_line()
        {
            const string groupKeyword = "f";

            bool canParse = _faceParser.CanParse(groupKeyword);
            canParse.Should().BeTrue();
        }

        [Fact]
        public void CanParse_returns_false_on_non_normal_line()
        {
            const string invalidKeyword = "vt";

            bool canParse = _faceParser.CanParse(invalidKeyword);
            canParse.Should().BeFalse();
        }

        [Fact]
        public void Parses_normal_line_correctly_1()
        {
            const string faceLine = "1 2 3";
            _faceParser.Parse(faceLine);

            var parsedFace = _faceGroupSpy.ParsedFace;

            parsedFace[0].VertexIndex.Should().Be(1);
            parsedFace[0].TextureIndex.Should().Be(0);
            parsedFace[0].NormalIndex.Should().Be(0);

            parsedFace[1].VertexIndex.Should().Be(2);
            parsedFace[1].TextureIndex.Should().Be(0);
            parsedFace[1].NormalIndex.Should().Be(0);

            parsedFace[2].VertexIndex.Should().Be(3);
            parsedFace[2].TextureIndex.Should().Be(0);
            parsedFace[2].NormalIndex.Should().Be(0);
        }

        [Fact]
        public void Parses_normal_line_correctly_2()
        {
            const string faceLine = "3/1 4/2 5/3";
            _faceParser.Parse(faceLine);

            var parsedFace = _faceGroupSpy.ParsedFace;

            parsedFace.Count.Should().Be(3);

            parsedFace[0].VertexIndex.Should().Be(3);
            parsedFace[0].TextureIndex.Should().Be(1);
            parsedFace[0].NormalIndex.Should().Be(0);

            parsedFace[1].VertexIndex.Should().Be(4);
            parsedFace[1].TextureIndex.Should().Be(2);
            parsedFace[1].NormalIndex.Should().Be(0);

            parsedFace[2].VertexIndex.Should().Be(5);
            parsedFace[2].TextureIndex.Should().Be(3);
            parsedFace[2].NormalIndex.Should().Be(0);
        }

        [Fact]
        public void Parses_normal_line_correctly_3()
        {
            const string faceLine = "6/4/1 3/5/3 7/6/5";
            _faceParser.Parse(faceLine);

            var parsedFace = _faceGroupSpy.ParsedFace;

            parsedFace.Count.Should().Be(3);

            parsedFace[0].VertexIndex.Should().Be(6);
            parsedFace[0].TextureIndex.Should().Be(4);
            parsedFace[0].NormalIndex.Should().Be(1);

            parsedFace[1].VertexIndex.Should().Be(3);
            parsedFace[1].TextureIndex.Should().Be(5);
            parsedFace[1].NormalIndex.Should().Be(3);

            parsedFace[2].VertexIndex.Should().Be(7);
            parsedFace[2].TextureIndex.Should().Be(6);
            parsedFace[2].NormalIndex.Should().Be(5);
        }

        [Fact]
        public void Parses_normal_line_correctly_4()
        {
            const string faceLine = "6//1 3//3 7//5";
            _faceParser.Parse(faceLine);

            var parsedFace = _faceGroupSpy.ParsedFace;

            parsedFace.Count.Should().Be(3);

            parsedFace[0].VertexIndex.Should().Be(6);
            parsedFace[0].TextureIndex.Should().Be(0);
            parsedFace[0].NormalIndex.Should().Be(1);

            parsedFace[1].VertexIndex.Should().Be(3);
            parsedFace[1].TextureIndex.Should().Be(0);
            parsedFace[1].NormalIndex.Should().Be(3);

            parsedFace[2].VertexIndex.Should().Be(7);
            parsedFace[2].TextureIndex.Should().Be(0);
            parsedFace[2].NormalIndex.Should().Be(5);
        }
    }

    public class FaceGroupSpy : IFaceGroup
    {
        public Face ParsedFace { get; private set; }

        public Face GetFace(int i)
        {
            throw new System.NotImplementedException();
        }

        public void AddFace(Face face)
        {
            ParsedFace = face;
        }
    }
}