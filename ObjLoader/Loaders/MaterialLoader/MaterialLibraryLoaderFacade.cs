using System;
using System.IO;

namespace ObjLoader.Loaders.MaterialLoader
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
            if (!File.Exists(materialFileName))
            {
                Console.WriteLine($"Material file '{materialFileName}' doesn't exist.");
                return;
            }    

            using var stream = _loader.Open(materialFileName);

            if (stream != null)
                _loader.Load(stream);
        }
    }
}