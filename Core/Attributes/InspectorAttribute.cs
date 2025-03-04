using System;
using System.Runtime.CompilerServices;

namespace SharpEngine.Core.Attributes
{
    public class InspectorAttribute : Attribute
    {
        public bool DisplayInInspector { get; set; }

        public string DisplayName { get; set; }

        public InspectorAttribute(bool displayInInspector = true, [CallerMemberName] string displayName = null)
        {
            DisplayInInspector = displayInInspector;
            DisplayName = displayName;
        }
    }
}