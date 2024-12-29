using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    /// <summary>
    ///     Represents a ray in 3D space.
    /// </summary>
    public class Ray
    {
        /// <summary>
        ///     Gets or sets the ray origin.
        /// </summary>
        public Vector3 Origin { get; set; }

        /// <summary>
        ///    Gets or sets the ray direction.
        /// </summary>
        public Vector3 Direction { get; set; }

        /// <summary>
        ///    Initializes a new instance of <see cref="Ray"/>.
        /// </summary>
        /// <param name="origin">The origin of the ray.</param>
        /// <param name="direction">The direction of the ray.</param>
        public Ray(Vector3 origin, Vector3 direction)
        {
            Origin = origin;
            Direction = direction;
        }

        /// <summary>
        ///   Checks if the ray intersects with any objects in the scene.
        /// </summary>
        /// <param name="scene">The scene to check for intersections.</param>
        /// <param name="intersectingObject">The intersecting object; <see langword="null"/> if the ray did not intersect an object.</param>
        /// <param name="hitPosition">The position where the ray hits the object.</param>
        /// <param name="allowedTypes">The game object types the ray is able to intersect.</param>
        /// <returns><see langword="true"/> if the ray intersects with an object; otherwise <see langword="false"/>.</returns>
        public bool IsGameObjectInView(Scene scene, out GameObject? intersectingObject, out Vector3 hitPosition, params Type[] allowedTypes)
        {
            const float maxDistance = 100.0f; // Maximum distance to check for intersections
            const float stepSize = 0.1f; // Step size for ray marching

            var sceneNodes = scene.GetAllGameObjects();
            
            if (allowedTypes.Any())
                sceneNodes = sceneNodes.Where(go => allowedTypes.Any(type => type.IsInstanceOfType(go))).ToList();

            for (float t = 0; t < maxDistance; t += stepSize)
            {
                Vector3 currentPosition = Origin + (t * Direction);

                intersectingObject = sceneNodes.FirstOrDefault(obj => IsPointInsideObject(currentPosition, obj));
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

        /// <summary>
        ///   Determines if a point is inside a given object.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <param name="obj">The object to check against.</param>
        /// <returns><see langword="true"/> if the point is inside the object, otherwise otherwise <see langword="false"/>.</returns>
        public static bool IsPointInsideObject(Vector3 point, GameObject obj)
        {
            Vector3 min = obj.Position - (obj.Scale / 2);
            Vector3 max = obj.Position + (obj.Scale / 2);

            return point.X >= min.X && point.X <= max.X &&
                   point.Y >= min.Y && point.Y <= max.Y &&
                   point.Z >= min.Z && point.Z <= max.Z;
        }

        /// <summary>
        ///   Gets the normal of the closest face of an object to a given point.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <param name="obj">The object to check against.</param>
        /// <returns>The normal of the closest face.</returns>
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
