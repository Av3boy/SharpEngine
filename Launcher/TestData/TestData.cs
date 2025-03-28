using SharpEngine.Shared.Dto;

namespace Launcher.TestData;
public static class TestData
{
    public const bool LoadTestData = true;

    public const bool SimulateLoading = false;

    public static IEnumerable<Project> Projects = Enumerable.Range(0, 25).Select(i =>
    {
        return new Project()
        {
            Name = $"Project {i}",
            Path = @"C:\test",
        };
    });
}
