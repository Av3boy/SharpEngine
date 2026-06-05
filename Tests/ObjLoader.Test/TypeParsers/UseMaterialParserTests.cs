using FluentAssertions;
using NUnit.Framework;
using SharpEngine.Core.ObjLoader.Loader.TypeParsers;

namespace SharpEngine.Core.ObjLoader.Tests.TypeParsers
{
    [TestFixture]
    public class UseMaterialParserTests
    {
        private readonly ElementGroupSpy _elementGroupSpy;
        private readonly UseMaterialParser _parser;

        public UseMaterialParserTests()
        {
            _elementGroupSpy = new ElementGroupSpy();
            _parser = new UseMaterialParser(_elementGroupSpy);
        }

        [Test]
        public void CanParse_returns_true_on_usemtl_line()
        {
            const string groupKeyword = "usemtl";

            bool canParse = _parser.CanParse(groupKeyword);
            canParse.Should().BeTrue();
        }

        [Test]
        public void CanParse_returns_false_on_non_usemtl_line()
        {
            const string invalidKeyword = "vt";

            bool canParse = _parser.CanParse(invalidKeyword);
            canParse.Should().BeFalse();
        }

        [Test]
        public void Parses_usemtl_line_correctly_1()
        {
            const string useMtlLine = "materialName";
            _parser.Parse(useMtlLine);

            _elementGroupSpy.MaterialName.Should().BeEquivalentTo("materialName");
        }

        private class ElementGroupSpy : IMaterialDataStore
        {
            public string MaterialName { get; private set; }

            public void SetMaterial(string materialName)
            {
                MaterialName = materialName;
            }
        }
    }
}