using SharpEngine.Core.Entities;
using SharpEngine.Core.Primitives;

using System.Numerics;

namespace Minecraft.Block;

/// <summary>
///     Represents a block in the game.
/// </summary>
public class BlockBase : GameObject
{
    /// <summary>
    ///     Gets the block type.
    /// </summary>
    public virtual BlockType BlockType { get; }

    /// <summary>Gets a value indicating whether the block is solid.</summary>
    public virtual bool IsSolid { get; }

    /// <summary>Gets or sets the size of the block in the inventory.</summary>
    public int SizeInInventory { get; set; } = 1;

    /// <summary>
    ///     The event executed when the player interacts with a block.
    /// </summary>
    public virtual void Interact() { }

    /// <summary>
    ///     Initializes a new instance of <see cref="BlockBase"/>.
    /// </summary>
    /// <param name="position">The position of the block in the game world.</param>
    /// <param name="name">The name of the block.</param>
    /// <param name="diffuseMapFile">The full path of the diffuse map texture.</param>
    /// <param name="specularMapFile">The full path of the specular map texture.</param>
    /// <param name="vertShaderFile">The full path of the vertex shader.</param>
    /// <param name="fragShaderFile">The full path of the fragment shader.</param>
    protected BlockBase(Vector3 position, string name, string diffuseMapFile, string specularMapFile, string vertShaderFile, string fragShaderFile)
    {
        Initialize(position, name, diffuseMapFile, specularMapFile, vertShaderFile, fragShaderFile);
    }

    private void Initialize(Vector3 position, string name, string diffuseMapFile, string specularMapFile, string vertShaderFile, string fragShaderFile)
    {
        var cube = PrimitiveFactory.Create(PrimitiveType.Cube, position, diffuseMapFile, specularMapFile, vertShaderFile, fragShaderFile);
        AssignProperties(cube, name);
    }

    private void AssignProperties(GameObject cube, string name)
    {
        Transform = new()
        {
            Position = cube.Transform.Position,
            Rotation = cube.Transform.Rotation,
            Scale = cube.Transform.Scale
        };

        Name = name;
        Material = cube.Material;
        Meshes = cube.Meshes;
    }
}
