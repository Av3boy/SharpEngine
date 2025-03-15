using ImGuiNET;
using Launcher.UI;
using SharpEngine.Core.Scenes;
using Silk.NET.Windowing;

namespace SharpEngine.Editor.Windows
{
    /// <summary>
    ///     A base class for all ImGui windows within the editor.
    /// </summary>
    public abstract class ImGuiWindowBase
    {
        /// <summary>Gets or sets the currently active project.</summary>
        public Project Project { get; set; }

        /// <summary>Gets or sets the currently active scene.</summary>
        public Scene Scene { get; private set; }

        public ImGuiWindowBase()
        {
            Scene = new Scene();
            Project = new Project();
        }

        private readonly Dictionary<string, bool> _previousDockingStates = [];

        /// <summary>Gets the name of the window.</summary>
        public abstract string Name { get; }

        /// <inheritdoc />
        public virtual ImGuiWindowFlags ImGuiWindowFlags => ImGuiWindowFlags.None;

        /// <summary>
        ///     Sets the current scene.
        /// </summary>
        /// <param name="scene">The new scene.</param>
        public void SetScene(Scene scene) => Scene = scene;

        /// <summary>
        ///     Sets the current project.
        /// </summary>
        /// <param name="project">The new project.</param>
        public void SetProject(Project project) => Project = project;

        /// <summary>
        ///     Renders a new ImGui window to the main window.
        /// </summary>
        public void RenderWindow()
        {
            ImGui.Begin(Name, ImGuiWindowFlags);

            Render();

            // Check docking state
            bool isDocked = ImGui.IsWindowDocked();
            if (_previousDockingStates.TryGetValue(Name, out bool wasDocked))
            {
                if (wasDocked && !isDocked)
                {
                    // Window was undocked
                    OnWindowUndocked();
                }
            }

            // Update previous docking state
            _previousDockingStates[Name] = isDocked;

            ImGui.End();
        }

        /// <summary>
        ///     Executes when the ImGui window was docked to the main editor window.
        /// </summary>
        protected virtual void OnWindowUndocked()
        {
            Console.WriteLine($"Window {Name} was undocked.");
            CreateSilkWindow();
        }

        private void CreateSilkWindow()
        {
            // TODO: Use the Core window.
            var window = Window.Create(WindowOptions.Default with
            {
                Title = Name
            });

            window.Load += () =>
            {
                // TODO: Initialize the ImGui context like we do in the Core window.
            };

            window.Render += delta => Render();

            window.Run();
        }

        /// <summary>
        ///     Renders the ImGui window contents.
        /// </summary>
        public abstract void Render();
    }
}
