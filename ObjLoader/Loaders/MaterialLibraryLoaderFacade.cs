namespace ObjLoader.Loader.Loaders
{
    public class MaterialLibraryLoaderFacade : IMaterialLibraryLoaderFacade
    {
        private readonly MaterialLibraryLoader _loader;

        public MaterialLibraryLoaderFacade(MaterialLibraryLoader loader)
        {
            _loader = loader;
        }

        public void Load(string materialFileName)
        {
            using var stream = _loader.Open(materialFileName);

            if (stream != null)
                _loader.Load(stream);
        }
    }
}