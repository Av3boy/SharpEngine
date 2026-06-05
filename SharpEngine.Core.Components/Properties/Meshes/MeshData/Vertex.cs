// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Numerics;

namespace SharpEngine.Core.Components.Properties.Meshes.MeshData
{
    /// <summary>
    ///     Represents a vertex in a 3D mesh.
    /// </summary>
    public struct Vertex
    {
        /// <summary>
        ///     The position of the vertex in 3D space, represented as a <see cref="Vector3"/> containing the X, Y, and Z coordinates.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        ///     The normal vector at the vertex, which is used for lighting calculations to determine how light interacts with the surface of the mesh.
        /// </summary>
        public Vector3 Normal;

        /// <summary>
        ///     The tangent vector at the vertex, which is used in conjunction with the normal and bitangent vectors to create a tangent space for normal mapping and other advanced shading techniques.
        /// </summary>
        public Vector3 Tangent;

        /// <summary>
        ///     The texture coordinates (UV coordinates) for the vertex, represented as a <see cref="Vector2"/> containing the U and V coordinates that map the vertex to a specific point on a texture image.
        /// </summary>
        public Vector2 TexCoords;

        /// <summary>
        ///     The bitangent vector at the vertex, which is used in conjunction with the normal and tangent vectors to create a tangent space for normal mapping and other advanced shading techniques.
        /// </summary>
        public Vector3 Bitangent;

        // TODO: #65 Skeletal mesh

        /// <summary>
        ///     The maximum number of bone influences that can affect a single vertex, which is typically set to 4 in many 3D graphics applications to balance performance and visual quality when animating skeletal meshes.
        /// </summary>
        public const int MAX_BONE_INFLUENCE = 4;

        /// <summary>
        ///     The IDs of the bones that influence this vertex, where each ID corresponds to a specific bone in the skeleton of a skeletal mesh.
        ///     The length of this array should not exceed <see cref="MAX_BONE_INFLUENCE"/> to ensure efficient processing during animation.
        /// </summary>
        public int[] BoneIds;

        /// <summary>
        ///     The weights corresponding to each bone influence on this vertex, where each weight represents the degree of influence that a particular bone has on the vertex's position during skeletal animation.
        /// </summary>
        public float[] Weights;
    }
}
