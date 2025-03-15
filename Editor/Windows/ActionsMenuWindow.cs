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
        public event Action? OnSceneLoaded;

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
            Debug.LogInformation($"Starting {Project.Name}.");

            Thread thread = new Thread(() =>
            {
                var process = ProcessExtensions.GetProcess($"dotnet run --project {Project.Path}/{Project.Name}.csproj", true);
                process.ErrorDataReceived += (sender, e) => Debug.LogInformation(e.Data);
                process.OutputDataReceived += (sender, e) => Debug.LogInformation(e.Data);
                process.Exited += (sender, e) => Debug.LogInformation("Process exited.");

                process.Start();

                // Read and display output
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                if (!string.IsNullOrWhiteSpace(output))
                    Debug.LogInformation(output);
                
                if (!string.IsNullOrWhiteSpace(error))
                    Debug.LogInformation(error);
            });

            thread.Start();
        }

        private static void SaveScene(Scene scene)
        {
            Console.WriteLine("Saving..");

            if (!scene.HasUnsavedChanges && !scene.HasSaveFile())
            {
                Console.WriteLine("No changes to save.");
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
