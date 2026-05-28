using FluentAssertions;
using NUnit.Framework;

using SharpEngine.Core.Components.ObjLoader.DataStore;
using SharpEngine.Core.Components.Properties.Meshes.MeshData;
using SharpEngine.Core.ObjLoader.Loader.TypeParsers;

namespace SharpEngine.Core.ObjLoader.Tests.TypeParsers
{
    [TestFixture]
    public class TextureParserTests
    {
        private readonly TextureDataStoreMock _textureDataStoreMock;
        private readonly TextureParser _textureParser;

        public TextureParserTests()
        {
            _textureDataStoreMock = new TextureDataStoreMock();

            _textureParser = new TextureParser(_textureDataStoreMock);
        }

        [Test]
        public void CanParse_returns_true_on_normal_line()
        {
            const string textureKeyword = "vt";

            bool canParse = _textureParser.CanParse(textureKeyword);
            canParse.Should().BeTrue();
        }

        [Test]
        public void CanParse_returns_false_on_non_normal_line()
        {
            const string invalidKeyword = "vn";

            bool canParse = _textureParser.CanParse(invalidKeyword);
            canParse.Should().BeFalse();
        }

        [Test]
        public void Parses_normal_line_correctly()
        {
            const string textureLine = "0.500 -1.352";
            _textureParser.Parse(textureLine);

            var parsedNormal = _textureDataStoreMock.ParsedTexture;
            parsedNormal.X.Should().BeApproximately(0.5f, 0.000001f);
            parsedNormal.Y.Should().BeApproximately(-1.352f, 0.000001f);
        }

        class TextureDataStoreMock : ITextureDataStore
        {
            public TextureCoordinate ParsedTexture { get; private set; }

            public TextureCoordinate GetTexture(int i)
            {
                throw new System.NotImplementedException();
            }

            public void AddTexture(TextureCoordinate texture)
            {
                ParsedTexture = texture;
            }
        }
    }
}