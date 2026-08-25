using Microsoft.Extensions.Logging;
using SharpEngine.Core.Shaders;
using SharpEngine.Telemetry;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;

namespace SharpEngine.Core.Rendering;

/// <summary>
///     Binds C# properties and fields decorated with <see cref="ShaderParameterAttribute"/> to their
///     corresponding GLSL uniforms, validating coverage at bind time and dispatching values at render time.
/// </summary>
/// <remarks>
///     Call <see cref="Bind"/> once when initializing an object against a shader to validate the
///     attribute-to-uniform mapping. Then call <see cref="Apply"/> every frame (or whenever values change)
///     to upload the current property values to the shader without writing individual <c>Set*</c> calls.
///     Uniforms that have no matching attribute are reported at <c>Debug</c> level; attributes that
///     reference a non-existent uniform are reported at <c>Warning</c> level.
/// </remarks>
public class ShaderParameterBinder
{
    private readonly ILogger<ShaderParameterBinder> _logger;

    private readonly record struct MemberBinding(
        MemberInfo Member,
        ShaderParameterAttribute Attribute,
        ShaderParameterType ResolvedType);

    // Reflection metadata is expensive; cache per source type across all instances.
    private static readonly ConcurrentDictionary<Type, (MemberInfo Member, ShaderParameterAttribute Attribute)[]> _memberCache = new();

    private MemberBinding[] _bindings = [];

    /// <summary>
    ///     Initializes a new instance of <see cref="ShaderParameterBinder"/>.
    /// </summary>
    /// <param name="logger">Logger used to report validation results and dispatch warnings.</param>
    public ShaderParameterBinder(ILogger<ShaderParameterBinder>? logger = null)
    {
        _logger = logger ?? LoggingExtensions.CreateLogger<ShaderParameterBinder>();
    }

    /// <summary>
    ///     Scans <paramref name="source"/> for <see cref="ShaderParameterAttribute"/>-decorated members,
    ///     validates them against the shader's active uniforms, and caches the resolved bindings for
    ///     use by <see cref="Apply"/>.
    /// </summary>
    /// <param name="source">The object whose type is inspected for shader parameter attributes.</param>
    /// <param name="shader">The shader whose <see cref="IShader.UniformLocations"/> are validated against.</param>
    public void Bind(object source, IShader shader)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(shader);

        var type = source.GetType();
        var discovered = _memberCache.GetOrAdd(type, DiscoverMembers);

        var parameterNames = new HashSet<string>(discovered.Length, StringComparer.Ordinal);
        foreach (var (_, attr) in discovered)
            parameterNames.Add(attr.Name);

        // Uniforms with no matching [ShaderParameter] attribute — developer may be setting them manually.
        foreach (var uniformName in shader.UniformLocations.Keys)
        {
            if (!parameterNames.Contains(uniformName))
                _logger.LogDebug("Shader uniform '{Uniform}' has no [ShaderParameter] mapping on '{Type}'.", uniformName, type.Name);
        }

        var resolved = new List<MemberBinding>(discovered.Length);
        foreach (var (member, attr) in discovered)
        {
            if (!shader.UniformLocations.ContainsKey(attr.Name))
            {
                _logger.LogWarning(
                    "[ShaderParameter(\"{Name}\")] on '{Member}' of '{Type}' has no matching uniform in the shader.",
                    attr.Name, member.Name, type.Name);
                continue;
            }

            var resolvedType = attr.Type != ShaderParameterType.Unknown ? attr.Type : InferType(member);
            if (resolvedType == ShaderParameterType.Unknown)
            {
                _logger.LogWarning(
                    "Cannot determine ParameterType for [ShaderParameter(\"{Name}\")] on '{Member}'. Specify the type explicitly.",
                    attr.Name, member.Name);
                continue;
            }

            resolved.Add(new MemberBinding(member, attr, resolvedType));
        }

        _bindings = [.. resolved];
    }

    /// <summary>
    ///     Reads the current values of all bound members from <paramref name="source"/> and uploads
    ///     them to <paramref name="shader"/> using the appropriate <c>Set*</c> method.
    /// </summary>
    /// <param name="source">The object from which property/field values are read.</param>
    /// <param name="shader">The shader that receives the uniform values.</param>
    public void Apply(object source, IShader shader)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(shader);

        foreach (var binding in _bindings)
        {
            var value = GetMemberValue(binding.Member, source);
            if (value is null)
                continue;

            var uploaded = binding.ResolvedType switch
            {
                ShaderParameterType.Int when value is int i => shader.SetInt(binding.Attribute.Name, i),
                ShaderParameterType.Float when value is float f => shader.SetFloat(binding.Attribute.Name, f),
                ShaderParameterType.Vec2 when value is Vector2 v2 => shader.SetVector2(binding.Attribute.Name, v2),
                ShaderParameterType.Vec3 when value is Vector3 v3 => shader.SetVector3(binding.Attribute.Name, v3),
                ShaderParameterType.Vec4 when value is Vector4 v4 => shader.SetVector4(binding.Attribute.Name, v4),
                ShaderParameterType.Mat4 when value is Matrix4x4 m => shader.SetMatrix4(binding.Attribute.Name, m),
                ShaderParameterType.Texture when value is int slot => shader.SetInt(binding.Attribute.Name, slot),
                _ => LogTypeMismatch(binding, value)
            };

            _ = uploaded; // result intentionally unused; Set* implementations log their own failures
        }
    }

    private bool LogTypeMismatch(MemberBinding binding, object value)
    {
        _logger.LogWarning("Value type '{ValueType}' is incompatible with ParameterType '{ParamType}' for uniform '{Name}'.",
                           value.GetType().Name, binding.ResolvedType, binding.Attribute.Name);

        return false;
    }

    private static (MemberInfo Member, ShaderParameterAttribute Attribute)[] DiscoverMembers(Type type)
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        var results = new List<(MemberInfo, ShaderParameterAttribute)>();

        foreach (var prop in type.GetProperties(flags))
        {
            var attr = prop.GetCustomAttribute<ShaderParameterAttribute>();
            if (attr is not null)
                results.Add((prop, attr));
        }

        foreach (var field in type.GetFields(flags))
        {
            var attr = field.GetCustomAttribute<ShaderParameterAttribute>();
            if (attr is not null)
                results.Add((field, attr));
        }

        return [.. results];
    }

    private static object? GetMemberValue(MemberInfo member, object source)
        => member switch
        {
            PropertyInfo prop => prop.GetValue(source),
            FieldInfo field => field.GetValue(source),
            _ => null
        };

    private static ShaderParameterType InferType(MemberInfo member)
    {
        var memberType = member switch
        {
            PropertyInfo prop => prop.PropertyType,
            FieldInfo field => field.FieldType,
            _ => null
        };

        return memberType switch
        {
            not null when memberType == typeof(int) => ShaderParameterType.Int,
            not null when memberType == typeof(float) => ShaderParameterType.Float,
            not null when memberType == typeof(Vector2) => ShaderParameterType.Vec2,
            not null when memberType == typeof(Vector3) => ShaderParameterType.Vec3,
            not null when memberType == typeof(Vector4) => ShaderParameterType.Vec4,
            not null when memberType == typeof(Matrix4x4) => ShaderParameterType.Mat4,
            _ => ShaderParameterType.Unknown
        };
    }
}
