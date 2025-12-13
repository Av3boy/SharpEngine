using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using SharpEngine.Shared.Attributes;

namespace SharpEngine.Shared.Dto;

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
    [DisplayName("Last Modified")]
    [Filter(false)]
    public DateTime LastModified { get; set; } = DateTime.Now;

    /// <summary>
    ///     Loads the given project file.
    /// </summary>
    /// <param name="projectFile">The file containing the project to load.</param>
    /// <returns>The loaded project. If unable to load, <see langword="null" />.</returns>
    public static Project? LoadProject(string projectFile)
    {
        var json = File.ReadAllText(projectFile);
        var project = JsonSerializer.Deserialize<Project>(json);

        if (project is not null)
            project.LastModified = File.GetLastWriteTime(projectFile);

        return project;
    }
}
