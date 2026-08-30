using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SharpEngine.Core.Components.Properties.Shaders;
using SharpEngine.Core.Numerics;
using SharpEngine.Core.Shaders;
using SharpEngine.Core.Shaders.Rendering;
using Xunit;

namespace SharpEngine.Core.Tests.Shaders;

public class ShaderParameterBinderTests
{
    // ---------------------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------------------

    private static ShaderParameterBinder CreateBinder()
        => new(NullLogger<ShaderParameterBinder>.Instance);

    private static Mock<IShader> CreateShaderMock(params string[] uniformNames)
    {
        var dict = new Dictionary<string, int>(uniformNames.Length);
        for (int i = 0; i < uniformNames.Length; i++)
            dict[uniformNames[i]] = i;

        var mock = new Mock<IShader>();
        mock.Setup(s => s.UniformLocations);
        mock.Setup(s => s.SetFloat(It.IsAny<string>(), It.IsAny<float>()));
        mock.Setup(s => s.SetInt(It.IsAny<string>(), It.IsAny<int>()));
        mock.Setup(s => s.SetVector2(It.IsAny<string>(), It.IsAny<Vector2>()));
        mock.Setup(s => s.SetVector3(It.IsAny<string>(), It.IsAny<Vector3>()));
        mock.Setup(s => s.SetVector4(It.IsAny<string>(), It.IsAny<Vector4>()));
        mock.Setup(s => s.SetMatrix4(It.IsAny<string>(), It.IsAny<Matrix4x4>(), It.IsAny<bool>()));
        return mock;
    }

    // ---------------------------------------------------------------------------
    // Test objects
    // ---------------------------------------------------------------------------

    private class FloatSource
    {
        [ShaderParameter("myFloat", ShaderParameterType.Float)]
        public float Value { get; set; } = 3.14f;
    }

    private class IntSource
    {
        [ShaderParameter("myInt", ShaderParameterType.Int)]
        public int Value { get; set; } = 7;
    }

    private class Vec2Source
    {
        [ShaderParameter("myVec2", ShaderParameterType.Vec2)]
        public Vector2 Value { get; set; } = new(1f, 2f);
    }

    private class Vec3Source
    {
        [ShaderParameter("myVec3", ShaderParameterType.Vec3)]
        public Vector3 Value { get; set; } = new(1f, 2f, 3f);
    }

    private class Vec4Source
    {
        [ShaderParameter("myVec4", ShaderParameterType.Vec4)]
        public Vector4 Value { get; set; } = new(1f, 2f, 3f, 4f);
    }

    private class Mat4Source
    {
        [ShaderParameter("myMat4", ShaderParameterType.Mat4)]
        public Matrix4x4 Value { get; set; } = Matrix4x4.Identity;
    }

    private class TextureSource
    {
        [ShaderParameter("tex", ShaderParameterType.Texture)]
        public int Slot { get; set; } = 0;
    }

    private class PrivateFieldSource
    {
        [ShaderParameter("fieldUniform", ShaderParameterType.Float)]
#pragma warning disable CS0414
        private float _field = 9.9f;
#pragma warning restore CS0414
    }

    private class InferredFloatSource
    {
        // No explicit ParameterType — should be inferred from C# float
        [ShaderParameter("inferredFloat")]
        public float Value { get; set; } = 1.5f;
    }

    private class InferredMat4Source
    {
        // No explicit ParameterType — should be inferred from Matrix4x4
        [ShaderParameter("inferredMat4")]
        public Matrix4x4 Value { get; set; } = Matrix4x4.Identity;
    }

    private class MismatchSource
    {
        // Attribute says Vec2 but C# type is float — Apply should not call any Set*
        [ShaderParameter("mismatch", ShaderParameterType.Vec2)]
        public float Value { get; set; } = 1f;
    }

    private class UnmappedAttributeSource
    {
        [ShaderParameter("doesNotExistInShader", ShaderParameterType.Float)]
        public float Value { get; set; } = 1f;
    }

    private class MultiPropertySource
    {
        [ShaderParameter("width", ShaderParameterType.Float)]
        public float Width { get; set; } = 100f;

        [ShaderParameter("height", ShaderParameterType.Float)]
        public float Height { get; set; } = 200f;
    }

    // ---------------------------------------------------------------------------
    // Bind – validation
    // ---------------------------------------------------------------------------

    [Fact]
    public void Bind_AttributeWithNoMatchingUniform_DoesNotAddBinding()
    {
        var binder = CreateBinder();
        var shader = CreateShaderMock("otherUniform"); // no "doesNotExistInShader"
        var source = new UnmappedAttributeSource();

        binder.Bind(source, shader.Object);
        binder.Apply(source, shader.Object);

        shader.Verify(s => s.SetFloat(It.IsAny<string>(), It.IsAny<float>()), Times.Never);
    }

    [Fact]
    public void Bind_NullSource_ThrowsArgumentNullException()
    {
        var binder = CreateBinder();
        var shader = CreateShaderMock();
        var act = () => binder.Bind(null!, shader.Object);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Bind_NullShader_ThrowsArgumentNullException()
    {
        var binder = CreateBinder();
        var act = () => binder.Bind(new FloatSource(), null!);
        act.Should().Throw<ArgumentNullException>();
    }

    // ---------------------------------------------------------------------------
    // Apply – correct dispatch per ParameterType
    // ---------------------------------------------------------------------------

    [Fact]
    public void Apply_Float_CallsSetFloat()
    {
        var binder = CreateBinder();
        var source = new FloatSource { Value = 3.14f };
        var shader = CreateShaderMock("myFloat");

        binder.Bind(source, shader.Object);
        binder.Apply(source, shader.Object);

        shader.Verify(s => s.SetFloat("myFloat", 3.14f), Times.Once);
    }

    [Fact]
    public void Apply_Int_CallsSetInt()
    {
        var binder = CreateBinder();
        var source = new IntSource { Value = 7 };
        var shader = CreateShaderMock("myInt");

        binder.Bind(source, shader.Object);
        binder.Apply(source, shader.Object);

        shader.Verify(s => s.SetInt("myInt", 7), Times.Once);
    }

    [Fact]
    public void Apply_Vec2_CallsSetVector2()
    {
        var binder = CreateBinder();
        var source = new Vec2Source { Value = new Vector2(1f, 2f) };
        var shader = CreateShaderMock("myVec2");

        binder.Bind(source, shader.Object);
        binder.Apply(source, shader.Object);

        shader.Verify(s => s.SetVector2("myVec2", new Vector2(1f, 2f)), Times.Once);
    }

    [Fact]
    public void Apply_Vec3_CallsSetVector3()
    {
        var binder = CreateBinder();
        var source = new Vec3Source { Value = new Vector3(1f, 2f, 3f) };
        var shader = CreateShaderMock("myVec3");

        binder.Bind(source, shader.Object);
        binder.Apply(source, shader.Object);

        shader.Verify(s => s.SetVector3("myVec3", new Vector3(1f, 2f, 3f)), Times.Once);
    }

    [Fact]
    public void Apply_Vec4_CallsSetVector4()
    {
        var binder = CreateBinder();
        var source = new Vec4Source { Value = new Vector4(1f, 2f, 3f, 4f) };
        var shader = CreateShaderMock("myVec4");

        binder.Bind(source, shader.Object);
        binder.Apply(source, shader.Object);

        shader.Verify(s => s.SetVector4("myVec4", new Vector4(1f, 2f, 3f, 4f)), Times.Once);
    }

    [Fact]
    public void Apply_Mat4_CallsSetMatrix4()
    {
        var binder = CreateBinder();
        var source = new Mat4Source { Value = Matrix4x4.Identity };
        var shader = CreateShaderMock("myMat4");

        binder.Bind(source, shader.Object);
        binder.Apply(source, shader.Object);

        shader.Verify(s => s.SetMatrix4("myMat4", Matrix4x4.Identity, true), Times.Once);
    }

    [Fact]
    public void Apply_Texture_CallsSetInt()
    {
        var binder = CreateBinder();
        var source = new TextureSource { Slot = 2 };
        var shader = CreateShaderMock("tex");

        binder.Bind(source, shader.Object);
        binder.Apply(source, shader.Object);

        shader.Verify(s => s.SetInt("tex", 2), Times.Once);
    }

    [Fact]
    public void Apply_PrivateField_IsDispatched()
    {
        var binder = CreateBinder();
        var source = new PrivateFieldSource();
        var shader = CreateShaderMock("fieldUniform");

        binder.Bind(source, shader.Object);
        binder.Apply(source, shader.Object);

        shader.Verify(s => s.SetFloat("fieldUniform", 9.9f), Times.Once);
    }

    [Fact]
    public void Apply_TypeMismatch_DoesNotCallAnySetMethod()
    {
        var binder = CreateBinder();
        var source = new MismatchSource { Value = 1f };
        var shader = CreateShaderMock("mismatch");

        binder.Bind(source, shader.Object);
        binder.Apply(source, shader.Object);

        shader.Verify(s => s.SetFloat(It.IsAny<string>(), It.IsAny<float>()), Times.Never);
        shader.Verify(s => s.SetVector2(It.IsAny<string>(), It.IsAny<Vector2>()), Times.Never);
    }

    // ---------------------------------------------------------------------------
    // Apply – type inference (ParameterType.Unknown)
    // ---------------------------------------------------------------------------

    [Fact]
    public void Apply_InferredFloat_CallsSetFloat()
    {
        var binder = CreateBinder();
        var source = new InferredFloatSource { Value = 1.5f };
        var shader = CreateShaderMock("inferredFloat");

        binder.Bind(source, shader.Object);
        binder.Apply(source, shader.Object);

        shader.Verify(s => s.SetFloat("inferredFloat", 1.5f), Times.Once);
    }

    [Fact]
    public void Apply_InferredMat4_CallsSetMatrix4()
    {
        var binder = CreateBinder();
        var source = new InferredMat4Source { Value = Matrix4x4.Identity };
        var shader = CreateShaderMock("inferredMat4");

        binder.Bind(source, shader.Object);
        binder.Apply(source, shader.Object);

        shader.Verify(s => s.SetMatrix4("inferredMat4", Matrix4x4.Identity, true), Times.Once);
    }

    // ---------------------------------------------------------------------------
    // Apply – multiple properties
    // ---------------------------------------------------------------------------

    [Fact]
    public void Apply_MultipleProperties_EachDispatchedSeparately()
    {
        var binder = CreateBinder();
        var source = new MultiPropertySource { Width = 100f, Height = 200f };
        var shader = CreateShaderMock("width", "height");

        binder.Bind(source, shader.Object);
        binder.Apply(source, shader.Object);

        shader.Verify(s => s.SetFloat("width", 100f), Times.Once);
        shader.Verify(s => s.SetFloat("height", 200f), Times.Once);
    }

    [Fact]
    public void Apply_BeforeBind_DoesNotCallAnySetMethod()
    {
        var binder = CreateBinder();
        var source = new FloatSource();
        var shader = CreateShaderMock("myFloat");

        // Apply without calling Bind first — _bindings is empty, nothing should be dispatched
        binder.Apply(source, shader.Object);

        shader.Verify(s => s.SetFloat(It.IsAny<string>(), It.IsAny<float>()), Times.Never);
    }

    [Fact]
    public void Apply_ReflectsCurrentPropertyValues()
    {
        var binder = CreateBinder();
        var source = new FloatSource { Value = 1f };
        var shader = CreateShaderMock("myFloat");

        binder.Bind(source, shader.Object);

        source.Value = 42f;
        binder.Apply(source, shader.Object);

        shader.Verify(s => s.SetFloat("myFloat", 42f), Times.Once);
        shader.Verify(s => s.SetFloat("myFloat", 1f), Times.Never);
    }
}
