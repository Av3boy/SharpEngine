using FluentAssertions;
using Xunit;

using SharpEngine.Core.Components.ObjLoader.DataStore;
using SharpEngine.Core.ObjLoader.Loader.TypeParsers;

namespace SharpEngine.Core.ObjLoader.Tests.TypeParsers
{
    public class GroupParserTests
    {
        private readonly GroupDataStoreMock _groupDataStoreMock;
        private readonly GroupParser _groupParser;

        public GroupParserTests()
        {
            _groupDataStoreMock = new GroupDataStoreMock();
            _groupParser = new GroupParser(_groupDataStoreMock);
        }

        [Fact]
        public void CanParse_returns_true_on_normal_line()
        {
            const string groupKeyword = "g";

            bool canParse = _groupParser.CanParse(groupKeyword);
            canParse.Should().BeTrue();
        }

        [Fact]
        public void CanParse_returns_false_on_non_normal_line()
        {
            const string invalidKeyword = "vt";

            bool canParse = _groupParser.CanParse(invalidKeyword);
            canParse.Should().BeFalse();
        }

        [Fact]
        public void Parses_normal_line_correctly()
        {
            const string normalLine = "test group";
            _groupParser.Parse(normalLine);

            var parsedGroupName = _groupDataStoreMock.ParsedGroupName;
            parsedGroupName.Should().Be("test group");
        }
    }

    class GroupDataStoreMock : IGroupDataStore
    {
        public string ParsedGroupName { get; set; }
        
        public void PushGroup(string groupName)
        {
            ParsedGroupName = groupName;
        }
    }
}