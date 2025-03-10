using System.ComponentModel.DataAnnotations;

namespace Launcher.Attributes
{
    /// <summary>
    ///     Represents a validation attribute for file paths.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FileAttribute : ValidationAttribute
    {
        /// <inheritdoc />
        public override bool IsValid(object? value)
        {
            if (value is string path)
                return System.IO.File.Exists(path);

            return false;
        }
    }
}
