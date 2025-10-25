using SharpEngine.Shared.Dto;

namespace Launcher.TestData;

/// <summary>
///     Contains test data for the launcher.
/// </summary>
public static class TestData
{
    /// <summary>Indicates whether test data should be loaded.</summary>
    public const bool LoadTestData = true;

    /// <summary>Indicates whether to simulate loading.</summary>
    public const bool SimulateLoading = false;

    /// <summary>A collection of test data projects.</summary>
    public static IEnumerable<Project> Projects = Enumerable.Range(0, 25).Select(i => new Project()
    {
        Name = $"Project {i}",
        Path = @"C:\test",
    });
}
