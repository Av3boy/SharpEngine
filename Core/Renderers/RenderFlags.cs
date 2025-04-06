using System;

namespace SharpEngine.Core.Renderers;

/// <summary>
///    Contains the different render flags.
/// </summary>
[Flags]
public enum RenderFlags
{
    /// <summary>No renderer should be enabled.</summary>
    None = 0,

    /// <summary>All the vertices will for the mesh will be rendered.</summary>
    Renderer3D = 1,

    /// <summary>The front face of the mesh will be rendered.</summary>
    UIRenderer = 2,

    /// <summary>Text components will be rendered.</summary>
    Text = 3,

    /// <summary>Enables all renderers.</summary>
    All = 4,
}
