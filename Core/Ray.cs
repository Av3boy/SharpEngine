using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class Ray
    {
        public Vector3 Origin { get; set; }
        public Vector3 Direction { get; set; }

        public Ray(Vector3 origin, Vector3 direction)
        {
            Origin = origin;
            Direction = direction;
        }

        public bool IsBlockInView(Scene scene, out GameObject intersectingObject, out Vector3 hitPosition)
        {
            const float maxDistance = 100.0f; // Maximum distance to check for intersections
            const float stepSize = 0.1f; // Step size for ray marching

            for (float t = 0; t < maxDistance; t += stepSize)
            {
                Vector3 currentPosition = Origin + (t * Direction);

                intersectingObject = scene.Blocks.FirstOrDefault(obj => IsPointInsideObject(currentPosition, obj));
                if (intersectingObject != null)
                {
                    hitPosition = currentPosition;
                    return true;
                }
            }

            intersectingObject = null;
            hitPosition = Vector3.Zero;
            return false;
        }

        public static bool IsPointInsideObject(Vector3 point, GameObject obj)
        {
            Vector3 min = obj.Position - (obj.Scale / 2);
            Vector3 max = obj.Position + (obj.Scale / 2);

            return point.X >= min.X && point.X <= max.X &&
                   point.Y >= min.Y && point.Y <= max.Y &&
                   point.Z >= min.Z && point.Z <= max.Z;
        }

        public static Vector3 GetClosestFaceNormal(Vector3 point, GameObject obj)
        {
            Vector3 min = obj.Position - (obj.Scale / 2);
            Vector3 max = obj.Position + (obj.Scale / 2);

            var distances = new Dictionary<Vector3, float>
            {
                { -Vector3.UnitX, Math.Abs(point.X - min.X) },
                { Vector3.UnitX, Math.Abs(point.X - max.X) },
                { -Vector3.UnitY, Math.Abs(point.Y - min.Y) },
                { Vector3.UnitY, Math.Abs(point.Y - max.Y) },
                { -Vector3.UnitZ, Math.Abs(point.Z - min.Z) },
                { Vector3.UnitZ, Math.Abs(point.Z - max.Z) }
            };

            return distances.OrderBy(d => d.Value).First().Key;
        }
    }
}
