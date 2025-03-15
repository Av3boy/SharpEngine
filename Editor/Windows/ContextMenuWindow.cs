using ImGuiNET;
using Launcher.UI;
using SharpEngine.Core.Primitives;
using SharpEngine.Core.Scenes;
using System.Numerics;

namespace SharpEngine.Editor.Windows
{
    /// <summary>
    ///     Represents the context menu window.
    /// </summary>
    public class ContextMenuWindow : ImGuiWindowBase
    {
        private readonly Scene _scene;
        private bool _showContextMenu;
        private bool _updateContextMenuLocation;

        /// <inheritdoc />
        public override string Name => "Context Menu";

        /// <inheritdoc />
        public override ImGuiWindowFlags ImGuiWindowFlags => ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove;

        /// <summary>
        ///     Initializes a new context of the .
        /// </summary>
        /// <param name="scene">Represents the current scene where the context menu will be displayed.</param>
        /// <param name="_">Discarded parameter. Required for getting the windows.</param>
        public ContextMenuWindow(Scene scene, Project _)
        {
            _scene = scene;
        }

        /// <summary>
        ///     Shows the context menu.
        /// </summary>
        public void ShowContextMenu()
        {
            _updateContextMenuLocation = true;
            _showContextMenu = true;
        }

        /// <inheritdoc />
        public override void Render()
        {
            if (!_showContextMenu)
                return;
            
            if (_updateContextMenuLocation)
            {
                ImGui.SetNextWindowPos(ImGui.GetMousePos());
                _updateContextMenuLocation = false;
            }

            ImGui.Begin("Context Menu", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove);

            if (ImGui.Button("Create cube"))
            {
                var cube = PrimitiveFactory.Create(PrimitiveType.Cube, new Vector3(0, 0, 0));
                cube.Name = "Cube" + _scene.Root.Children.Count;

                _scene.ActiveElement = cube;
                _scene.Root.AddChild(cube);
            }

            ImGui.End();
        }
    }
}
