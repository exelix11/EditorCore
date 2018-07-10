using System;
using Syroot.Maths;

namespace Syroot.NintenTools.MarioKart8
{
    /// <summary>
    /// Represents a collection of mathematical functions.
    /// </summary>
    internal static class Maths
    {
        // ---- METHODS (INTERNAL) -------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the next power of 2 which results in a value bigger than or equal to <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to which the next power of 2 will be determined.</param>
        /// <returns>The next power of resulting in a value bigger than or equal to the given value.</returns>
        internal static int GetNext2Exponent(float value)
        {
            return (int)Math.Ceiling(Math.Log(value, 2));
        }

        /// <summary>
        /// Returns a value indicating whether the given <paramref name="triangle"/> overlaps a cube positioned at the
        /// <paramref name="cubeCenter"/> expanding with <paramref name="cubeHalfSize"/>.
        /// </summary>
        /// <param name="triangle">The <see cref="Triangle"/> to check for overlaps.</param>
        /// <param name="cubeCenter">The positional <see cref="Vector3F "/> at which the cube originates.</param>
        /// <param name="cubeHalfSize">The half length of one edge of the cube.</param>
        /// <returns><c>true</c> when the triangle intersects with the cube, otherwise <c>false</c>.</returns>
        internal static bool TriangleCubeOverlap(Triangle triangle, Vector3F cubeCenter, float cubeHalfSize)
        {
            Vector3F v0 = triangle.Vertices[0] - cubeCenter;
            Vector3F v1 = triangle.Vertices[1] - cubeCenter;
            Vector3F v2 = triangle.Vertices[2] - cubeCenter;

            if (Math.Min(Math.Min(v0.X, v1.X), v2.X) > cubeHalfSize || Math.Max(Math.Max(v0.X, v1.X), v2.X) < -cubeHalfSize) return false;
            if (Math.Min(Math.Min(v0.Y, v1.Y), v2.Y) > cubeHalfSize || Math.Max(Math.Max(v0.Y, v1.Y), v2.Y) < -cubeHalfSize) return false;
            if (Math.Min(Math.Min(v0.Z, v1.Z), v2.Z) > cubeHalfSize || Math.Max(Math.Max(v0.Z, v1.Z), v2.Z) < -cubeHalfSize) return false;

            Vector3F normal = triangle.Normal;
            float dot = normal.Dot(v0);
            float r = cubeHalfSize * (Math.Abs(normal.X) + Math.Abs(normal.Y) + Math.Abs(normal.Z));
            if (dot > r || dot < -r) return false;

            Vector3F e = v1 - v0;
            if (AxisTest(e.Z, -e.Y, v0.Y, v0.Z, v2.Y, v2.Z, cubeHalfSize)) return false;
            if (AxisTest(-e.Z, e.X, v0.X, v0.Z, v2.X, v2.Z, cubeHalfSize)) return false;
            if (AxisTest(e.Y, -e.X, v1.X, v1.Y, v2.X, v2.Y, cubeHalfSize)) return false;

            e = v2 - v1;
            if (AxisTest(e.Z, -e.Y, v0.Y, v0.Z, v2.Y, v2.Z, cubeHalfSize)) return false;
            if (AxisTest(-e.Z, e.X, v0.X, v0.Z, v2.X, v2.Z, cubeHalfSize)) return false;
            if (AxisTest(e.Y, -e.X, v0.X, v0.Y, v1.X, v1.Y, cubeHalfSize)) return false;

            e = v0 - v2;
            if (AxisTest(e.Z, -e.Y, v0.Y, v0.Z, v1.Y, v1.Z, cubeHalfSize)) return false;
            if (AxisTest(-e.Z, e.X, v0.X, v0.Z, v1.X, v1.Z, cubeHalfSize)) return false;
            if (AxisTest(e.Y, -e.X, v1.X, v1.Y, v2.X, v2.Y, cubeHalfSize)) return false;

            return true;
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------
        
        private static bool AxisTest(float a1, float a2, float b1, float b2, float c1, float c2, float cubeHalfSize)
        {
            float p = a1 * b1 + a2 * b2;
            float q = a1 * c1 + a2 * c2;
            float r = cubeHalfSize * (Math.Abs(a1) + Math.Abs(a2));
            return Math.Min(p, q) > r || Math.Max(p, q) < -r;
        }
    }
	
}
