using System;
using System.Runtime.CompilerServices;

namespace SharpEngine.Core.Attributes
{
    /// <summary>
    ///    Represents an attribute that controls how properties should be displayed in the inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class InspectorAttribute : Attribute
    {
        /// <summary>Gets or sets whether the property should be shown in the inspector.</summary>
        public bool DisplayInInspector { get; set; }

        /// <summary>Gets or sets the name by which the object should be shown in the inspector.</summary>
        public string DisplayName { get; set; }


        /// <summary>
        ///     Initializes a new instance of the <see cref="InspectorAttribute"/>.
        /// </summary>
        /// <param name="displayInInspector">Determines whether the property should be shown in the inspector.</param>
        /// <param name="displayName">The name by which the object should be shown in the inspector.</param>
        public InspectorAttribute(bool displayInInspector = true, [CallerMemberName] string displayName = "")
        {
            DisplayInInspector = displayInInspector;
            DisplayName = displayName;
        }
    }
}
