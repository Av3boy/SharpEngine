using SharpEngine.Core.Components.Obsolete.ObjLoader.DataStore;
using SharpEngine.Core.Components.Properties;

namespace ObjLoader.Loader.Data.Elements
{
    public class Group : IFaceGroup
    {
        private readonly List<Face> _faces = [];

        public Group(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
        public Material Material { get; set; }

        public IList<Face> Faces => _faces;

        public void AddFace(Face face) => _faces.Add(face);
    }
}