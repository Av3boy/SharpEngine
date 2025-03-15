using ImGuiNET;
using Launcher.UI;
using SharpEngine.Core.Scenes;

namespace SharpEngine.Editor.Windows
{
    /// <summary>
    ///     Represents the scene window that renders the current scene as a tree view.
    /// </summary>
    public class SceneWindow : ImGuiWindowBase
    {
        /// <inheritdoc />
        public override string Name => "Scene";

        /// <inheritdoc />
        public override void Render()
        {
            if (Scene is null || Scene.Root.Children.Count == 0)
                ImGui.Text("No objects in the scene.");
            else
                RenderSceneNode(Scene.Root);
        }

        private void RenderSceneNode(SceneNode node)
        {
            foreach (var child in node.Children.Where(child => ImGui.TreeNode(child.Name)))
            {
                Scene!.ActiveElement = child;

                ImGui.Indent();
                RenderSceneNode(child);
                ImGui.Unindent();
                ImGui.TreePop();
            }
        }
    }
}
