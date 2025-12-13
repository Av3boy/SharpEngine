using System;
using System.Runtime.CompilerServices;

namespace SharpEngine.Core.Attributes;

/// <summary>Handles the property appearance in the inspector.</summary>
[AttributeUsage(AttributeTargets.Property)]
public class InspectorAttribute : Attribute
{
    /// <summary>Gets or sets whether the property should be shown in the editor inspector.</summary>
    public bool DisplayInInspector { get; set; }

    /// <summary>Gets or sets name to use instead of the property's name.</summary>
    public string? DisplayName { get; set; }

    /// <summary>
    ///     Initializes a new instance of <see cref="InspectorAttribute"/>.
    /// </summary>
    /// <param name="displayInInspector">Determines whether the property should be shown in the editor inspector.</param>
    /// <param name="displayName">A name to use instead of the property's name.</param>
    public InspectorAttribute(bool displayInInspector = true, [CallerMemberName] string? displayName = null)
    {
        DisplayInInspector = displayInInspector;
        DisplayName = displayName;
    }
}