using System.ComponentModel.DataAnnotations;
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
}
