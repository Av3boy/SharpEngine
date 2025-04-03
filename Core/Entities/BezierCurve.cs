using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SharpEngine.Core.Entities;
public class BezierCurve
{
    private Vector3D<float> P0, P1, P2, P3;
    public Vector3D<float> Start => P0;
    public Vector3D<float> End => P3;

    public List<Vector3D<float>> Points { get; set; }

    public BezierCurve(Vector3D<float> initializationPosition)
    {
        const float defaultWidth = 1;

        P0 = initializationPosition;
        P1 = new Vector3D<float>(initializationPosition.X + defaultWidth, initializationPosition.Y + defaultWidth, initializationPosition.Z);
        P2 = new Vector3D<float>(initializationPosition.X + defaultWidth, initializationPosition.Y - defaultWidth, initializationPosition.Z);
        P3 = new Vector3D<float>(initializationPosition.X + defaultWidth, initializationPosition.Y, initializationPosition.Z);

        Points = GetPoints(20);
    }

    public List<Vector3D<float>> GetPoints(int resolution)
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
