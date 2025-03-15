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
        private Scene _scene;

        /// <inheritdoc />
        public override string Name => "Scene";

        /// <summary>
        ///     Initializes a new instance of <see cref="SceneWindow"/>.
        /// </summary>
        /// <param name="scene">The scene </param>
        public SceneWindow(Scene scene, Project project)
        {
            _scene = scene;
        }

        /// <summary>
        ///     Sets the scene to be rendered.
        /// </summary>
        /// <param name="scene">The new scene.</param>
        public void SetScene(Scene scene)
            => _scene = scene;

        /// <inheritdoc />
        public override void Render()
        {
            if (_scene.Root.Children.Count == 0)
                ImGui.Text("No objects in the scene.");
            else
                RenderSceneNode(_scene.Root);
        }

        private void RenderSceneNode(SceneNode node)
        {
            foreach (var child in node.Children.Where(child => ImGui.TreeNode(child.Name)))
            {
                _scene.ActiveElement = child;

                ImGui.Indent();
                RenderSceneNode(child);
                ImGui.Unindent();
                ImGui.TreePop();
            }
        }
    }
}
