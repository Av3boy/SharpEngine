using ImGuiNET;
using Launcher.UI;
using SharpEngine.Core;
using SharpEngine.Core.Scenes;

namespace SharpEngine.Editor.Windows
{
    /// <summary>
    ///     Represents a window that displays the assets in the project.
    /// </summary>
    public class AssetsWindow : ImGuiWindowBase
    {
        private Project _project;

        /// <summary>
        ///     Initializes a new instance <see cref="AssetsWindow" />.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="project"></param>
        public AssetsWindow(Scene scene, Project project)
        {
            _project = project;
        }

        /// <inheritdoc />
        public override string Name => "Assets";

        /// <inheritdoc />
        public override void Render()
        {
            RenderDirectory(_project.Path);
        }

        private void RenderDirectory(string path)
        {
            string[] directories = Directory.GetDirectories(path);
            string[] files = Directory.GetFiles(path);

            foreach (var directory in directories)
                if (ImGui.TreeNode(Path.GetFileName(directory)))
                {
                    RenderDirectory(directory);
                    ImGui.TreePop();
                }

            foreach (var file in files)
                if (ImGui.Selectable(Path.GetFileName(file)))
                {
                    // Scene.ActiveElement = file; // Set the file as the active element
                }
        }
    }
}
