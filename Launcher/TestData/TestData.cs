using Launcher.UI;

namespace Launcher.TestData;
public static class TestData
{
    public const bool LoadTestData = true;

    public const bool SimulateLoading = false;

    public static IEnumerable<Project> Projects = Enumerable.Range(0, 3).Select(i =>
    {
        return new Project()
        {
            Name = $"Project {i}",
        };
    });
}
