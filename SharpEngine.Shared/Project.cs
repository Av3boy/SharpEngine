using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Launcher.UI;

/// <summary>
///     Represents a SharpEngine project.
/// </summary>
public class Project
{
    /// <summary>
    ///     An identifier for the current project within the launcher UI.
    /// </summary>
    public readonly Guid Id = Guid.NewGuid();

    /// <summary>
    ///     Gets or sets the name of the project.
    /// </summary>
    [Required]
    public string? Name { get; set; }

    /// <summary>
    ///     Gets or sets the path to the project file.
    /// </summary>
    [Required]
    public string? Path { get; set; }

    /// <summary>
    ///     Gets or sets when the project was last modified.
    /// </summary>
    [JsonIgnore]
    public DateTime LastModified { get; set; } = DateTime.Now;

    /// <summary>
    ///     Loads the given project file.
    /// </summary>
    /// <param name="projectFile">The file containing the project to load.</param>
    /// <returns>The loaded project. If unable to load, <see langword="null" />.</returns>
    public static Project? LoadProject(string projectFile)
    {
        var json = File.ReadAllText(projectFile);
        return JsonSerializer.Deserialize<Project>(json);
    }
}
