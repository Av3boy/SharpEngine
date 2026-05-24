namespace SharpEngine.Core.ObjLoader.TypeParsers
{
    /// <summary>
    ///     Defines a parser for handling different types of OBJ file lines.
    /// </summary>
    public interface ITypeParser
    {
        /// <summary>
        ///     Determines whether the specified keyword can be parsed.
        /// </summary>
        /// <param name="keyword">The keyword to check.</param>
        /// <returns>True if the keyword can be parsed; otherwise, false.</returns>
        bool CanParse(string keyword);

        /// <summary>
        /// Parses a single input line and updates the object's state accordingly.
        /// </summary>
        /// <param name="line">The input line to parse.</param>
        void Parse(string line);
    }
}