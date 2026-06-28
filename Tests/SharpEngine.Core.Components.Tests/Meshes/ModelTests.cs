using FluentAssertions;
using SharpEngine.Core.Components.Properties.Meshes;
using SharpEngine.Core.Components.Properties.Meshes.MeshData;
using System.Numerics;
using Xunit;

namespace SharpEngine.Core.Components.Tests.Meshes;

/// <summary>
///     Unit tests for <see cref="Model"/> covering the pure, GL-independent methods.
/// </summary>
/// <remarks>
///     Tests that require a real OpenGL context (e.g., <see cref="Model.ProcessMesh"/>) need an
///     integration test environment with a live windowing / GL context and are therefore not
///     covered here.
/// </remarks>
public class ModelTests
{
    // -------------------------------------------------------------------------
    // BuildVertices
    // -------------------------------------------------------------------------

    [Fact]
    public void BuildVertices_EmptyCollection_ReturnsEmptyArray()
    {
        var result = Model.BuildVertices([]);

        result.Should().BeEmpty();
    }

    [Fact]
    public void BuildVertices_SingleVertex_ReturnsEightFloatsInCorrectOrder()
    {
        var vertex = new Vertex
        {
            Position = new Vector3(1f, 2f, 3f),
            Normal = new Vector3(0f, 1f, 0f),
            TexCoords = new Vector2(0.5f, 0.25f)
        };

        var result = Model.BuildVertices([vertex]);

        result.Should().HaveCount(8);
        result.Should().ContainInOrder(
            1f, 2f, 3f,     // Position
            0f, 1f, 0f,     // Normal
            0.5f, 0.25f);   // TexCoords
    }

    [Fact]
    public void BuildVertices_MultipleVertices_ProducesContiguousFloatLayout()
    {
        var v1 = new Vertex { Position = new Vector3(1f, 0f, 0f), Normal = new Vector3(0f, 0f, 1f), TexCoords = new Vector2(0f, 0f) };
        var v2 = new Vertex { Position = new Vector3(0f, 1f, 0f), Normal = new Vector3(0f, 0f, 1f), TexCoords = new Vector2(1f, 1f) };

        var result = Model.BuildVertices([v1, v2]);

        result.Should().HaveCount(16);

        // First vertex
        result[0].Should().Be(1f);
        result[1].Should().Be(0f);
        result[2].Should().Be(0f);
        result[3].Should().Be(0f);
        result[4].Should().Be(0f);
        result[5].Should().Be(1f);
        result[6].Should().Be(0f);
        result[7].Should().Be(0f);

        // Second vertex
        result[8].Should().Be(0f);
        result[9].Should().Be(1f);
        result[10].Should().Be(0f);
        result[11].Should().Be(0f);
        result[12].Should().Be(0f);
        result[13].Should().Be(1f);
        result[14].Should().Be(1f);
        result[15].Should().Be(1f);
    }

    [Fact]
    public void BuildVertices_VertexWithNegativeCoordinates_PreservesSignedValues()
    {
        var vertex = new Vertex
        {
            Position = new Vector3(-0.5f, -0.5f, -0.5f),
            Normal = new Vector3(-1f, 0f, 0f),
            TexCoords = new Vector2(0f, 1f)
        };

        var result = Model.BuildVertices([vertex]);

        result.Should().ContainInOrder(
            -0.5f, -0.5f, -0.5f,
            -1f, 0f, 0f,
            0f, 1f);
    }
}
