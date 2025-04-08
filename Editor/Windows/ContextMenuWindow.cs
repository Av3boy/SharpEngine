using ImGuiNET;
using SharpEngine.Core.Entities;
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
        private GameObject? IntersectedObject;

        /// <inheritdoc />
        public override string Name => "Context Menu";

        /// <inheritdoc />
        public override ImGuiWindowFlags ImGuiWindowFlags => ImGuiWindowFlags.NoTitleBar | 
                                                             ImGuiWindowFlags.NoResize | 
                                                             ImGuiWindowFlags.NoMove | 
                                                             ImGuiWindowFlags.AlwaysAutoResize | 
                                                             ImGuiWindowFlags.NoDocking;

        /// <summary>
        ///     Shows the context menu.
        /// </summary>
        /// <param name="intersectedObject">The object the cursor right-clicked on.</param>
        public void ShowContextMenu(GameObject? intersectedObject = null)
        {
            IntersectedObject = intersectedObject;
            _updateContextMenuLocation = true;
            _showContextMenu = true;
        }

        /// <inheritdoc />
        public override void PreRender()
        {
            if (_updateContextMenuLocation)
            {
                ImGui.SetNextWindowPos(ImGui.GetMousePos());
                _updateContextMenuLocation = false;
            }
        }

        /// <inheritdoc />
        public override void Render()
        {
            if (!_showContextMenu)
            {
                IntersectedObject = null;
                return;
            }

            if (Scene is not null)
                RenderSceneActions();

            if (IntersectedObject is not null)
                RenderIntersectedObjectActions();
        }

        private void RenderIntersectedObjectActions()
        {
            if (ImGui.Button("Delete"))
            {
                Scene.Root.RemoveChild(IntersectedObject!);
                IntersectedObject = null;
            }
        }

        private void RenderSceneActions()
        {
            var values = Enum.GetValues<PrimitiveType>();
            foreach (var value in values)
            {
                if (ImGui.Button($"Create {value}"))
                {
                    var cube = PrimitiveFactory.Create(value, new Vector3(0, 0, 0));
                    cube.Name = value.ToString() + Scene.Root.Children.Count;

                    Scene.ActiveElement = cube;
                    Scene.Root.AddChild(cube);
                }
            }

            if (ImGui.Button("Empty"))
            {
                var element = Scene.ActiveElement ?? Scene.Root;
                element.AddChild(SceneNode.Empty);
            }
        }
    }
}
