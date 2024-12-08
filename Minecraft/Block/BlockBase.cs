using Core;
using Core.Primitives;

using OpenTK.Mathematics;

namespace Minecraft.Block;

public class BlockBase : GameObject
{
    public virtual string Name { get; set; }
    public virtual bool IsSolid { get; }
    public virtual void Interact() { }

    protected BlockBase(Vector3 position, string name, string diffuseMapFile, string specularMapFile, string vertShaderFile, string fragShaderFile)
    {
        Intialize(position, name, diffuseMapFile, specularMapFile, vertShaderFile, fragShaderFile);
    }

    private void Intialize(Vector3 position, string name, string diffuseMapFile, string specularMapFile, string vertShaderFile, string fragShaderFile)
    {
        var cube = Cube.Create(position, diffuseMapFile, specularMapFile, vertShaderFile, fragShaderFile);
        AssignProperties(cube, name);
    }

    private void AssignProperties(GameObject cube, string name)
    {
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
