using ImGuiNET;
using Silk.NET.Windowing;

namespace SharpEngine.Editor.Windows
{
    /// <summary>
    ///     A base class for all ImGui windows within the editor.
    /// </summary>
    public abstract class ImGuiWindowBase
    {
        private readonly Dictionary<string, bool> _previousDockingStates = [];

        /// <summary>Gets the name of the window.</summary>
        public abstract string Name { get; }

        public virtual ImGuiWindowFlags ImGuiWindowFlags => ImGuiWindowFlags.None;

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
