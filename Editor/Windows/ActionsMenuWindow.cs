using ImGuiNET;
using Launcher.UI;
using SharpEngine.Core;
using SharpEngine.Core.Scenes;
using SharpEngine.Shared;

namespace SharpEngine.Editor.Windows
{
    /// <summary>
    ///     Represents the actions menu window.
    /// </summary>
    public class ActionsMenuWindow : ImGuiWindowBase
    {
        /// <inheritdoc />
        public override string Name => "Actions";

        /// <summary>
        ///     An event executed when a new scene is loaded.
        /// </summary>
        public event Action<Scene>? OnSceneLoaded;

        /// <inheritdoc />
        public override void Render()
        {
            ImGui.Text("some text");

            if (ImGui.Button("Save"))
                SaveScene(Scene);

            if (ImGui.Button("Load") && SelectFile(out string selectedFile))
                LoadScene(selectedFile);

            if (ImGui.Button("Play"))
                StartGame();
        }

        private void StartGame()
        {
            if (Project is null)
            {
                // TODO: This should never be possible. If the Project is null when it's loaded we should make sure it's initialized. 
                // Editor service probably needs to be made shared so that we can initialize the project / solution when the editor is opened.

                // TODO: Do we want to save here automatically or prompt the user to save?
                Debug.LogInformation("The project was null");
                return;
            }

            Debug.LogInformation($"Starting {Project.Name}.");
            ProcessExtensions.RunProcess($"dotnet run --project {Project.Path}/{Project.Name}.csproj", true, msg => Debug.LogInformation(msg));
        }

        private void SaveScene(Scene scene)
        {
            Console.WriteLine("Saving..");

            if (!scene.HasUnsavedChanges && scene.HasSaveFile())
            {
                Console.WriteLine("No changes to save.");
                return;
            }

            if (scene.HasSaveFile())
            {
                scene.SaveScene().Wait();
                OnSceneLoaded?.Invoke(scene);
                Console.WriteLine("File saved successfully!");
                return;
            }

            using var dialog = new SaveFileDialog()
            {
                Filter = $"Scene files (*.{Scene.SceneFileExtension})|*.{Scene.SceneFileExtension}",
                CheckWriteAccess = true,
                DefaultExt = Scene.SceneFileExtension,
                FileName = $"scene.{Scene.SceneFileExtension}",
                Title = "Save scene",
            };

            var dialogResult = dialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                scene.SaveScene(dialog.FileName).Wait();
                Scene.SetFileFullPath(dialog.FileName);

                Console.WriteLine("File saved successfully!");
            }
            else
                Console.WriteLine("File save cancelled.");
        }

        private static bool SelectFile(out string selectedFile)
        {
            Console.WriteLine("Selecting file..");

            using var dialog = new OpenFileDialog()
            {
                Title = "Load scene",
                Filter = $"Scene files (*.{Scene.SceneFileExtension})|*.{Scene.SceneFileExtension}",
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                selectedFile = dialog.FileName;
                return true;
            }
            else
            {
                selectedFile = string.Empty;
                return false;
            }
        }

        private void LoadScene(string sceneFile)
        {
            Console.WriteLine($"Loading scene from file: {sceneFile}.");

            var newScene = Scene.LoadScene(sceneFile);
            SetScene(newScene);

            Console.WriteLine("Scene loaded successfully!");
        }
    }
}
