using SharpEngine.Core.Extensions;
using System.Reflection;
using Xunit;

namespace SharpEngine.Core.Tests.Extensions;

public class AssemblyExtensionsTests
{
    private class AssemblyStub : Assembly
    {
        public Version Version { get; }
        public AssemblyStub(Version version) 
        {
            Version = version;
        }

        public override AssemblyName GetName()
        {
            return new AssemblyName("TestAssembly")
            {
                Version = Version
            };
        }
    }

    [Fact]
    public void GetVersion_Returns_Correct_Version()
    {
        var assemblyVersion = new Version(1, 2, 3, 4);

        var assembly = new AssemblyStub(assemblyVersion);
        var version = assembly.GetVersion();
        
        Assert.NotNull(version);
        Assert.Equal(1, version.Major);
        Assert.Equal(2, version.Minor);
        Assert.Equal(3, version.Build);
        Assert.Equal(4, version.Revision);
    }
}
