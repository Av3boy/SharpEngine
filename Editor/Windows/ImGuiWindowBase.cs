using ImGuiNET;
using Launcher.UI;

using SharpEngine.Core.Entities.Views.Settings;
using SharpEngine.Core.Scenes;

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

        /// <summary>Gets the name of the window.</summary>
        public abstract string Name { get; }

        /// <inheritdoc />
        public virtual ImGuiWindowFlags ImGuiWindowFlags => ImGuiWindowFlags.None;

        private readonly Dictionary<string, bool> _previousDockingStates = [];
        private bool _isVisible = true;

        /// <summary>
        ///     Initializes a new instance of <see cref="ImGuiWindowBase"/>.
        /// </summary>
        public ImGuiWindowBase()
        {
            Scene = new Scene();
            Project = new Project();
        }

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

        public void Open() => _isVisible = true;

        /// <summary>
        ///     Executes operations required before rendering the ImGui window.
        /// </summary>
        public virtual void PreRender() { }

        /// <summary>
        ///     Renders a new ImGui window to the main window.
        /// </summary>
        public void RenderWindow()
        {
            if (!_isVisible)
                return;

            PreRender();

            ImGui.Begin(Name, ref _isVisible, ImGuiWindowFlags);

            Render();

            // Check docking state
            bool isDocked = ImGui.IsWindowDocked();
            if (_previousDockingStates.TryGetValue(Name, out bool wasDocked) && wasDocked && !isDocked)
                OnWindowUndocked();

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

            // TODO: This method of doing this does not work (at least in .Net 8).
            // See issue #44 for more information.
            //CreateSilkWindow();
        }

        private void CreateSilkWindow()
        {
            try
            {
                var thread = new Thread(() =>
                {

                    var window = new Core.Window(new(), new DefaultViewSettings());

                    window.Closing += () => _previousDockingStates[Name] = false;
                    window.OnAfterRender += deltaTime =>
                    {
                        ImGui.Begin(Name, ImGuiWindowFlags);
                        Render();
                        ImGui.End();
                    };

                    window.Run();
                });

                thread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create window: {ex.Message}");
            }
        }

        /// <summary>
        ///     Renders the ImGui window contents.
        /// </summary>
        public abstract void Render();
    }
}
