using Core.Interfaces;

namespace Core
{
    public class EditorWindow : Window
    {
        // TODO: Game and scene are redundant in editor windows.
        public EditorWindow(IGame game, Scene scene, Silk.NET.Windowing.WindowOptions options) : base(game, scene, options)
        {
        }
    }
}
