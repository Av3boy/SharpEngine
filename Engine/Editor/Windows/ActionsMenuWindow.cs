using ImGuiNET;
using SharpEngine.Core;
using SharpEngine.Core.Audio;
using SharpEngine.Core.Scenes;
using SharpEngine.Shared;
using SharpEngine.Shared.Extensions;
using System.Text;

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
            if (ImGui.Button("Save"))
                SaveScene(Scene);

            if (ImGui.Button("Load") && SelectFile(out string selectedFile))
                LoadScene(selectedFile);

            if (ImGui.Button("Play"))
                StartGame();

            const uint size = 256;
            var buffer = new byte[size];
            
            ImGui.InputText("Audio path", buffer, size);
            if (ImGui.Button("Test audio"))
                PlayTestAudio(Encoding.Default.GetString(buffer));
        }

        private void PlayTestAudio(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Debug.Log.Warning("Audio file '{FilePath}' does not exist.", filePath);
                return;
            }

            try
            {
                var audioPlayer = new WavPlayer();
                audioPlayer.AudioProperties.IsLooping = false;

                audioPlayer.Play(filePath);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex, "Error during audio playback: {Message}", ex.Message);
            }
        }

        private void StartGame()
        {
            if (Project is null)
            {
                // TODO: #78 This should never be possible. If the Project is null when it's loaded we should make sure it's initialized. 
                // Editor service probably needs to be made shared so that we can initialize the project / solution when the editor is opened.

                // TODO: #77 Save automatically
                Debug.Log.Information("The project was null");
                return;
            }

            Debug.Log.Information("Starting {ProjectName}.", Project.Name);
            ProcessExtensions.RunProcess($"dotnet run --project {Project.Path}/{Project.Name}.csproj", true, Debug.Log.Information);
        }

        private void SaveScene(Scene scene)
        {
            Debug.Log.Information("Saving..");

            if (!scene.HasUnsavedChanges && scene.HasSaveFile())
            {
                Debug.Log.Information("No changes to save.");
                return;
            }

            if (scene.HasSaveFile())
            {
                scene.SaveScene().Wait();
                OnSceneLoaded?.Invoke(scene);
                Debug.Log.Information("File saved successfully!");
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

                Debug.Log.Information("File saved successfully!");
            }
            else
                Debug.Log.Information("File save cancelled.");
        }

        private static bool SelectFile(out string selectedFile)
        {
            Debug.Log.Information("Selecting file..");

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
            Debug.Log.Information($"Loading scene from file: {sceneFile}.");

            var newScene = Scene.LoadScene(sceneFile);
            SetScene(newScene);

            Debug.Log.Information("Scene loaded successfully!");
        }
    }
}
