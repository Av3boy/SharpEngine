using Microsoft.Extensions.Logging;
using SharpEngine.Telemetry;
using System.IO;
using System.IO.Abstractions;

namespace SharpEngine.Core.ObjLoader.Loader.Loaders
{
    /// <summary>
    ///     Represents a base class for loading .obj and .mtl files, providing common line parsing functionality.
    /// </summary>
    public abstract class LoaderBase
    {
        private readonly IFileStreamFactory _fileStreamFactory;
        private readonly IFileManager _fileManager;

        private readonly ILogger<LoaderBase> _logger = LoggingExtensions.CreateLogger<LoaderBase>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="LoaderBase"/>.
        /// </summary>
        protected LoaderBase(IFileStreamFactory fileStreamFactory, IFileManager fileManager)
        {
            _fileStreamFactory = fileStreamFactory;
            _fileManager = fileManager;
        }

        /// <summary>
        ///     Parses the file at the specified path and processes each line via <see cref="ParseLine(string, string)"/>.
        /// </summary>
        /// <remarks>
        ///     I/O exceptions (for example FileNotFoundException, UnauthorizedAccessException, IOException) are propagated to the caller. 
        /// </remarks>
        /// <param name="path">The path of the file to open and parse.</param>
        /// <returns><see langword="true"/> if the file was parsed successfully; otherwise, <see langword="false"/>.</returns>
        public bool ParseFile(string path)
        {
            if (!_fileStreamFactory.FileSystem.File.Exists(path))
            {
                _logger.LogError("File not found: {Path}", path);
                return false;
            }

            using var fileStream = _fileStreamFactory.FileSystem.FileStream.New(path, FileMode.Open, FileAccess.Read);
            using var lineStreamReader = _fileManager.StreamReader(path);
            
            while (!lineStreamReader.EndOfStream)
            {
                var currentLine = lineStreamReader.ReadLine();

                if (!string.IsNullOrWhiteSpace(currentLine))
                {
                    if (!ParseLine(currentLine))
                    {
                        _logger.LogError("Failed to parse line: {Line}", currentLine);
                        // return false;
                    }
                }
            }

            return true;
        }

        private bool ParseLine(string currentLine)
        {
            if (currentLine[0] == '#')
                return true;

            var fields = currentLine.Trim().Split(null, 2);
            var keyword = fields[0].Trim();
            var data = fields[1].Trim();

            return ParseLine(keyword, data);
        }

        /// <summary>
        ///     Parses a single line consisting of a keyword and its associated data.
        /// </summary>
        /// <param name="keyword">Keyword that identifies the line type or command.</param>
        /// <param name="data">Data associated with the keyword; may be empty or contain parameters or raw text to interpret.</param>
        /// <returns><see langword="true"/> if the line was parsed successfully; otherwise, <see langword="false"/>.</returns>
        protected abstract bool ParseLine(string keyword, string data);
    }
}