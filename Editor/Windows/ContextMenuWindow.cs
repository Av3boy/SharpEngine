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
        private bool _showContextMenu;
        private bool _updateContextMenuLocation;

        /// <inheritdoc />
        public override string Name => "Context Menu";

        /// <inheritdoc />
        public override ImGuiWindowFlags ImGuiWindowFlags => ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove;

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

            if (Scene is null)
                throw new InvalidOperationException("Scene is not set.");

            if (_updateContextMenuLocation)
            {
                ImGui.SetNextWindowPos(ImGui.GetMousePos());
                _updateContextMenuLocation = false;
            }

            ImGui.Begin("Context Menu", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove);

            if (ImGui.Button("Create cube"))
            {
                var cube = PrimitiveFactory.Create(PrimitiveType.Cube, new Vector3(0, 0, 0));
                cube.Name = "Cube" + Scene.Root.Children.Count;

                Scene.ActiveElement = cube;
                Scene.Root.AddChild(cube);
            }

            ImGui.End();
        }
    }
}
