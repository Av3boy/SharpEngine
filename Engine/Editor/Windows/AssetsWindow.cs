using ImGuiNET;

namespace SharpEngine.Editor.Windows
{
    /// <summary>
    ///     Represents a window that displays the assets in the project.
    /// </summary>
    public class AssetsWindow : ImGuiWindowBase
    {
        /// <inheritdoc />
        public override string Name => "Assets";

        /// <inheritdoc />
        public override void Render()
        {
            if (!string.IsNullOrWhiteSpace(Project?.Path))
                RenderDirectory(Project.Path);
        }

        private static void RenderDirectory(string path)
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
                    // TODO: #76 Open the file in the IDE
                    // Process.Start(Settings.PreferredIDE.Name + Settings.PreferredIDE.CLIArguments + file);

                    // Scene.ActiveElement = file; // Set the file as the active element
                }
        }
    }
}
