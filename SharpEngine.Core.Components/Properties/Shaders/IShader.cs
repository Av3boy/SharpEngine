using System.Collections.Generic;
using System.Numerics;

namespace SharpEngine.Core.Shaders;

/// <summary>
///     Defines the contract for setting shader uniform parameters and querying their locations.
/// </summary>
public interface IShader
{
    /// <summary>Gets the uniform name-to-location map cached at shader initialization.</summary>
    IReadOnlyDictionary<string, int> UniformLocations { get; }

    /// <summary>Sets a uniform <see langword="int"/> on this shader.</summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The value to set.</param>
    /// <returns><see langword="true"/> if the uniform was found and set; otherwise <see langword="false"/>.</returns>
    bool SetInt(string name, int data);

    /// <summary>Sets a uniform <see langword="float"/> on this shader.</summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The value to set.</param>
    /// <returns><see langword="true"/> if the uniform was found and set; otherwise <see langword="false"/>.</returns>
    bool SetFloat(string name, float data);

    /// <summary>Sets a uniform <see cref="Matrix4x4"/> on this shader.</summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The value to set.</param>
    /// <param name="transpose">Whether to transpose the matrix before uploading. Defaults to <see langword="true"/>.</param>
    /// <returns><see langword="true"/> if the uniform was found and set; otherwise <see langword="false"/>.</returns>
    bool SetMatrix4(string name, Matrix4x4 data, bool transpose = true);

    /// <summary>Sets a uniform <see cref="Vector2"/> on this shader.</summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The value to set.</param>
    /// <returns><see langword="true"/> if the uniform was found and set; otherwise <see langword="false"/>.</returns>
    bool SetVector2(string name, Vector2 data);

    /// <summary>Sets a uniform <see cref="Vector3"/> on this shader.</summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The value to set.</param>
    /// <returns><see langword="true"/> if the uniform was found and set; otherwise <see langword="false"/>.</returns>
    bool SetVector3(string name, Vector3 data);

    /// <summary>Sets a uniform <see cref="Vector4"/> on this shader.</summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The value to set.</param>
    /// <returns><see langword="true"/> if the uniform was found and set; otherwise <see langword="false"/>.</returns>
    bool SetVector4(string name, Vector4 data);
}
