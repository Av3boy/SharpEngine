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
        /// <returns><see langword="true"/> if the keyword can be parsed; otherwise, <see langword="false"/>.</returns>
        bool CanParse(string keyword);

        /// <summary>
        /// Parses a single input line and updates the object's state accordingly.
        /// </summary>
        /// <param name="line">The input line to parse.</param>
        /// <returns><see langword="true"/> if the line was successfully parsed; otherwise, <see langword="false"/>.</returns>
        bool Parse(string line);
    }
}