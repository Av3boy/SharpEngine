using Silk.NET.Maths;
using System;
using System.Collections.Generic;

namespace SharpEngine.Core.Entities;

/// <summary>
///     Represents a cubic Bezier curve.
/// </summary>
public class BezierCurve
{
    private Vector3D<float> P0, P1, P2, P3;

    /// <summary>Gets the start point of the curve.</summary>
    public Vector3D<float> Start => P0;

    /// <summary>Gets the end point of the curve.</summary>

    public Vector3D<float> End => P3;

    /// <summary>
    ///     Initializes a new instance of <see cref="BezierCurve" />.
    /// </summary>
    /// <param name="initializationPosition">The point where the curve should be created.</param>
    public BezierCurve(Vector3D<float> initializationPosition)
    {
        const float defaultWidth = 1;

        P0 = initializationPosition;
        P1 = new Vector3D<float>(initializationPosition.X + defaultWidth, initializationPosition.Y + defaultWidth, initializationPosition.Z);
        P2 = new Vector3D<float>(initializationPosition.X + defaultWidth, initializationPosition.Y - defaultWidth, initializationPosition.Z);
        P3 = new Vector3D<float>(initializationPosition.X + defaultWidth, initializationPosition.Y, initializationPosition.Z);
    }

    /// <summary>
    ///     Gets the points of the curve.
    /// </summary>
    /// <param name="resolution">The distance between each point, defines the roughness of the curve.</param>
    /// <returns>The points for the curve with the given <paramref name="resolution"/>.</returns>
    public List<Vector3D<float>> GetPoints(uint resolution)
    {
        var points = new List<Vector3D<float>>();
        for (var i = 0; i <= resolution; i++)
        {
            var t = i / (float)resolution;
            points.Add(GetPointAt(t));
        }

        return points;
    }

    private Vector3D<float> GetPointAt(float t)
    {
        var x = GetCoordinateAt(t, P0.X, P1.X, P2.X, P3.X);
        var y = GetCoordinateAt(t, P0.Y, P1.Y, P2.Y, P3.Y);
        var z = GetCoordinateAt(t, P0.Z, P1.Z, P2.Z, P3.Z);
        return new(x, y, z);
    }

    private static float GetCoordinateAt(float t, float p0, float p1, float p2, float p3)
    {
        float u = 1 - t;
        return MathF.Pow(u, 3) * p0 + 3 * MathF.Pow(u, 2) * t * p1 + 3 * u * MathF.Pow(t, 2) * p2 + MathF.Pow(t, 3) * p3;
    }
}
