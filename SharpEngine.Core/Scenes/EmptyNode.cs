using SharpEngine.Core.Components.Properties;
using SharpEngine.Core.Numerics;

namespace SharpEngine.Core.Scenes;

/// <summary>
///     Represents an empty node in a scene.
/// </summary>
/// <typeparam name="TTransform">Specifies the type used for transformations.</typeparam>
/// <typeparam name="TVector">Defines vector used by the transform.</typeparam>
public class EmptyNode<TTransform, TVector> : SceneNode where TTransform : ITransform<TVector>, new() where TVector : IVector, new()
{
    /// <summary>
    ///     Initializes a new instance of <see cref="EmptyNode{TTransform, TVector}"/>.
    /// </summary>
    /// <param name="name">The name of the node in the scene hierarchy.</param>
    public EmptyNode(string name) : base(name) { }

    /// <summary>
    ///     Gets or sets the transform of the node.
    /// </summary>
    public virtual TTransform Transform { get; set; } = new();
}