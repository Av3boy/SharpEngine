using Core.Entities;
using Core.Interfaces;

namespace Core
{
    public class EditorWindow : Window
    {
        public EditorWindow(Scene scene, ISettings settings, View camera) : base(scene, settings, camera)
        {
        }
    }
}
