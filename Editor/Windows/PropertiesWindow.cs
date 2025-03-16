using ImGuiNET;
using SharpEngine.Core.Attributes;
using System.Numerics;
using System.Reflection;

namespace SharpEngine.Editor.Windows;

/// <summary>
///     Represents a window that displays the properties of the active element.
/// </summary>
public class PropertiesWindow : ImGuiWindowBase
{
    /// <inheritdoc />
    public override string Name => "Properties";

    /// <inheritdoc />
    public override void Render()
    {
        if (Scene.ActiveElement is not null)
            RenderActiveElementProperties(Scene.ActiveElement);
    }

    /// <summary>
    ///    Renders the properties of the active element.
    /// </summary>
    /// <param name="obj">The object whose properties should be rendered onto the window.</param>
    /// <param name="maxRecursionDepth">Marks how deep the inspector is allowed render properties.</param>
    /// <param name="currentDepth">Marks the starting level or the currently rendered level of properties.</param>
    public void RenderActiveElementProperties(object obj, int maxRecursionDepth = 5, int currentDepth = 0)
    {
        if (currentDepth == 0)
            ImGui.Begin(obj is null ? "Properties" : Scene.ActiveElement!.Name + " properties");

        if (obj is null)
        {
            ImGui.Text("No properties to display.");
            ImGui.End();
            return;
        }

        if (currentDepth > maxRecursionDepth)
            return;

        try
        {
            var properties = obj.GetType()
                                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                .OrderBy(p => p.PropertyType.IsClass && p.PropertyType != typeof(string));

            foreach (var property in properties)
            {
                var inspectorAttribute = property.GetCustomAttribute<InspectorAttribute>();
                if (inspectorAttribute != null && !inspectorAttribute.DisplayInInspector)
                {
                    continue;
                }

                var propertyName = inspectorAttribute?.DisplayName ?? property.Name;
                var propertyValue = property.GetValue(obj);

                object? newValue = propertyValue switch
                {
                    string stringValue => ImGui.InputText(propertyName, ref stringValue, 100) ? stringValue : propertyValue,
                    int intValue => ImGui.SliderInt(propertyName, ref intValue, 0, 100) ? intValue : propertyValue,
                    float floatValue => ImGui.SliderFloat(propertyName, ref floatValue, 0f, 100f) ? floatValue : propertyValue,
                    Vector3 vector3Value => ImGui.SliderFloat3(propertyName, ref vector3Value, 0f, 100f) ? vector3Value : propertyValue,
                    _ => propertyValue
                };

                if (newValue != propertyValue)
                    property.SetValue(obj, newValue);

                if (propertyValue is object && property.PropertyType.IsClass && property.PropertyType != typeof(string) &&
                    ImGui.CollapsingHeader(propertyName, ImGuiTreeNodeFlags.DefaultOpen))
                {
                    ImGui.Indent();
                    RenderActiveElementProperties(propertyValue, maxRecursionDepth, currentDepth + 1);
                    ImGui.Unindent();
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        if (currentDepth == 0)
            ImGui.End();
    }
}
