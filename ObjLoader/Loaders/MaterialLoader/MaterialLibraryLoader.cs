using SharpEngine.Core.Components.ObjLoader.DataStore;
using SharpEngine.Core.Components.Properties;
using SharpEngine.Core.ObjLoader.Loader.Loaders;
using SharpEngine.Shared.Extensions;

using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Numerics;

namespace SharpEngine.Core.ObjLoader.Loaders.MaterialLoader
{
    // https://paulbourke.net/dataformats/mtl/

    /// <summary>
    ///     Handles loading of material libraries (.mtl files) referenced by .obj files.
    /// </summary>
    /// <remarks>
    ///     Parses material properties and texture maps, and stores them in the provided <see cref="DataStore"/>. 
    ///     Also handles opening material library files based on the path of the .obj file.
    /// </remarks>
    public class MaterialLibraryLoader : LoaderBase
    {
        private readonly IMaterialDataStore _dataStore;
        private readonly Dictionary<string, Action<string>> _parseActionDictionary = [];
        private readonly List<string> _unrecognizedLines = [];

        private Material _currentMaterial = new(string.Empty);

        // TODO: #2 See Model class. This support both materials and textures

        /// <summary>
        ///     Initializes a new instance of <see cref="MaterialLibraryLoader"/> and registers parse actions for .mtl directives.
        /// </summary>
        /// <remarks>
        ///     Registers parse actions for common .mtl directives (for example: newmtl, Ka, Kd, Ks,
        ///     Ns, d, Tr, illum, map_*, bump, disp, decal) to populate Material instances.
        /// </remarks>
        /// <param name="dataStore">DataStore used to register and store parsed materials and texture references.</param>
        /// <param name="fileStreamFactory">Represents a factory for creating file streams.</param>
        /// <param name="fileManager">Represents a manager for handling file operations.</param>
        public MaterialLibraryLoader(IMaterialDataStore dataStore, IFileStreamFactory fileStreamFactory, IFileManager fileManager) : base(fileStreamFactory, fileManager)
        {         
            _dataStore = dataStore;

            AddParseAction("newmtl", PushMaterial);
            AddParseAction("Ka", d => _currentMaterial.AmbientColor = ParseVec3(d));
            AddParseAction("Kd", d => _currentMaterial.DiffuseColor = ParseVec3(d));
            AddParseAction("Ks", d => _currentMaterial.SpecularColor = ParseVec3(d));
            AddParseAction("Ns", d => _currentMaterial.SpecularCoefficient = d.ParseInvariantFloat());

            AddParseAction("d", d => _currentMaterial.Transparency = d.ParseInvariantFloat());
            AddParseAction("Tr", d => _currentMaterial.Transparency = d.ParseInvariantFloat());

            AddParseAction("illum", i => _currentMaterial.IlluminationModel = i.ParseInvariantInt());

            AddParseAction("map_Ka", m => _currentMaterial.AmbientTextureMap = new TextureDto(m));
            AddParseAction("map_Kd", m => _currentMaterial.DiffuseMap = new TextureDto(m));

            AddParseAction("map_Ks", m => _currentMaterial.SpecularMap = new TextureDto(m));
            AddParseAction("map_Ns", m => _currentMaterial.SpecularHighlightTextureMap = new TextureDto(m));

            AddParseAction("map_d", m => _currentMaterial.AlphaTextureMap = new TextureDto(m));

            AddParseAction("map_bump", m => _currentMaterial.BumpMap = new TextureDto(m));
            AddParseAction("bump", m => _currentMaterial.BumpMap = new TextureDto(m));

            AddParseAction("disp", m => _currentMaterial.DisplacementMap = new TextureDto(m));

            AddParseAction("decal", m => _currentMaterial.StencilDecalMap = new TextureDto(m));
        }

        private void AddParseAction(string key, Action<string> action) 
            => _parseActionDictionary.Add(key.ToLowerInvariant(), action);

        /// <inheritdoc />
        protected override void ParseLine(string keyword, string data)
        {
            var parseAction = GetKeywordAction(keyword, out bool found);

            if (!found)
            {
                _unrecognizedLines.Add(keyword + " " + data);
                return;
            }

            parseAction!(data);
        }

        private Action<string>? GetKeywordAction(string keyword, out bool found)
        {
            found = _parseActionDictionary.TryGetValue(keyword.ToLowerInvariant(), out var action);
            return action;
        }

        private void PushMaterial(string materialName)
        {
            _currentMaterial = new Material(materialName);
            _dataStore.AddMaterial(_currentMaterial);
        }

        private static Vector3 ParseVec3(string data)
        {
            string[] parts = data.Split(' ');

            float x = parts[0].ParseInvariantFloat();
            float y = parts[1].ParseInvariantFloat();
            float z = parts[2].ParseInvariantFloat();

            return new Vector3(x, y, z);
        }
    }
}