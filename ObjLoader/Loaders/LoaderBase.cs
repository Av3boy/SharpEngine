using System.IO;

namespace SharpEngine.Core.ObjLoader.Loader.Loaders
{
    /// <summary>
    ///     Represents a base class for loading .obj and .mtl files, providing common line parsing functionality.
    /// </summary>
    public abstract class LoaderBase
    {
        /// <summary>
        ///     Parses the file at the specified path and processes each line via <see cref="ParseLine(string, string)"/>.
        /// </summary>
        /// <remarks>
        ///     I/O exceptions (for example FileNotFoundException, UnauthorizedAccessException, IOException) are propagated to the caller. 
        /// </remarks>
        /// <param name="path">The path of the file to open and parse.</param>
        public void ParseFile(string path)
        {
            using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            using var lineStreamReader = new StreamReader(fileStream);
            
            while (!lineStreamReader.EndOfStream)
            {
                var currentLine = lineStreamReader.ReadLine();

                if (!string.IsNullOrWhiteSpace(currentLine))
                    ParseLine(currentLine);
            }
        }

        private void ParseLine(string currentLine)
        {
            if (currentLine[0] == '#')
                return;

            var fields = currentLine.Trim().Split(null, 2);
            var keyword = fields[0].Trim();
            var data = fields[1].Trim();

            ParseLine(keyword, data);
        }

        /// <summary>
        ///     Parses a single line consisting of a keyword and its associated data.
        /// </summary>
        /// <param name="keyword">Keyword that identifies the line type or command.</param>
        /// <param name="data">Data associated with the keyword; may be empty or contain parameters or raw text to interpret.</param>
        protected abstract void ParseLine(string keyword, string data);
    }
}