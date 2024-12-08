using Core;
using Core.Primitives;

using OpenTK.Mathematics;
using System;

namespace Minecraft.Block;

internal class BlockBase : GameObject
{
    public virtual string Name { get; set; }
    public virtual bool IsSolid { get; }
    public virtual void Interact() { }

    protected BlockBase(Vector3 position, string name)
    {
        var cube = Cube.Create(position);
        Position = cube.Position;
        Name = name;
        DiffuseMap = cube.DiffuseMap;
        Mesh = cube.Mesh;
        Quaternion = cube.Quaternion;
        Scale = cube.Scale;
        Shader = cube.Shader;
        SpecularMap = cube.SpecularMap;
    }
}
