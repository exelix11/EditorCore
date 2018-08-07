using LibEveryFileExplorer._3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace KCLExt.KCL
{
	//from https://gist.github.com/StagPoint/76ae48f5d7ca2f9820748d08e55c9806#file-triangleboxintersect-cs
	static class TriangleBoxIntersect
	{
		public static bool IntersectsBox(Triangle tri, Vector3D boxCenter, Vector3D boxExtents)
		{
			// Translate triangle as conceptually moving AABB to origin
			var v0 = (tri.PointA - boxCenter);
			var v1 = (tri.PointB - boxCenter);
			var v2 = (tri.PointC - boxCenter);

			// Compute edge vectors for triangle
			var f0 = (v1 - v0);
			var f1 = (v2 - v1);
			var f2 = (v0 - v2);

			#region Test axes a00..a22 (category 3)

			// Test axis a00
			var a00 = new Vector3D(0, -f0.Z, f0.Y);
			var p0 = Vector3D.DotProduct(v0, a00);
			var p1 = Vector3D.DotProduct(v1, a00);
			var p2 = Vector3D.DotProduct(v2, a00);
			var r = boxExtents.Y * Math.Abs(f0.Z) + boxExtents.Z * Math.Abs(f0.Y);
			if (Math.Max(-fmax(p0, p1, p2), fmin(p0, p1, p2)) > r)
			{
				return false;
			}

			// Test axis a01
			var a01 = new Vector3D(0, -f1.Z, f1.Y);
			p0 = Vector3D.DotProduct(v0, a01);
			p1 = Vector3D.DotProduct(v1, a01);
			p2 = Vector3D.DotProduct(v2, a01);
			r = boxExtents.Y * Math.Abs(f1.Z) + boxExtents.Z * Math.Abs(f1.Y);
			if (Math.Max(-fmax(p0, p1, p2), fmin(p0, p1, p2)) > r)
			{
				return false;
			}

			// Test axis a02
			var a02 = new Vector3D(0, -f2.Z, f2.Y);
			p0 = Vector3D.DotProduct(v0, a02);
			p1 = Vector3D.DotProduct(v1, a02);
			p2 = Vector3D.DotProduct(v2, a02);
			r = boxExtents.Y * Math.Abs(f2.Z) + boxExtents.Z * Math.Abs(f2.Y);
			if (Math.Max(-fmax(p0, p1, p2), fmin(p0, p1, p2)) > r)
			{
				return false;
			}

			// Test axis a10
			var a10 = new Vector3D(f0.Z, 0, -f0.X);
			p0 = Vector3D.DotProduct(v0, a10);
			p1 = Vector3D.DotProduct(v1, a10);
			p2 = Vector3D.DotProduct(v2, a10);
			r = boxExtents.X * Math.Abs(f0.Z) + boxExtents.Z * Math.Abs(f0.X);
			if (Math.Max(-fmax(p0, p1, p2), fmin(p0, p1, p2)) > r)
			{
				return false;
			}

			// Test axis a11
			var a11 = new Vector3D(f1.Z, 0, -f1.X);
			p0 = Vector3D.DotProduct(v0, a11);
			p1 = Vector3D.DotProduct(v1, a11);
			p2 = Vector3D.DotProduct(v2, a11);
			r = boxExtents.X * Math.Abs(f1.Z) + boxExtents.Z * Math.Abs(f1.X);
			if (Math.Max(-fmax(p0, p1, p2), fmin(p0, p1, p2)) > r)
			{
				return false;
			}

			// Test axis a12
			var a12 = new Vector3D(f2.Z, 0, -f2.X);
			p0 = Vector3D.DotProduct(v0, a12);
			p1 = Vector3D.DotProduct(v1, a12);
			p2 = Vector3D.DotProduct(v2, a12);
			r = boxExtents.X * Math.Abs(f2.Z) + boxExtents.Z * Math.Abs(f2.X);
			if (Math.Max(-fmax(p0, p1, p2), fmin(p0, p1, p2)) > r)
			{
				return false;
			}

			// Test axis a20
			var a20 = new Vector3D(-f0.Y, f0.X, 0);
			p0 = Vector3D.DotProduct(v0, a20);
			p1 = Vector3D.DotProduct(v1, a20);
			p2 = Vector3D.DotProduct(v2, a20);
			r = boxExtents.X * Math.Abs(f0.Y) + boxExtents.Y * Math.Abs(f0.X);
			if (Math.Max(-fmax(p0, p1, p2), fmin(p0, p1, p2)) > r)
			{
				return false;
			}

			// Test axis a21
			var a21 = new Vector3D(-f1.Y, f1.X, 0);
			p0 = Vector3D.DotProduct(v0, a21);
			p1 = Vector3D.DotProduct(v1, a21);
			p2 = Vector3D.DotProduct(v2, a21);
			r = boxExtents.X * Math.Abs(f1.Y) + boxExtents.Y * Math.Abs(f1.X);
			if (Math.Max(-fmax(p0, p1, p2), fmin(p0, p1, p2)) > r)
			{
				return false;
			}

			// Test axis a22
			var a22 = new Vector3D(-f2.Y, f2.X, 0);
			p0 = Vector3D.DotProduct(v0, a22);
			p1 = Vector3D.DotProduct(v1, a22);
			p2 = Vector3D.DotProduct(v2, a22);
			r = boxExtents.X * Math.Abs(f2.Y) + boxExtents.Y * Math.Abs(f2.X);
			if (Math.Max(-fmax(p0, p1, p2), fmin(p0, p1, p2)) > r)
			{
				return false;
			}

			#endregion

			#region Test the three axes corresponding to the face normals of AABB b (category 1)

			// Exit if...
			// ... [-extents.X, extents.X] and [min(v0.X,v1.X,v2.X), max(v0.X,v1.X,v2.X)] do not overlap
			if (fmax(v0.X, v1.X, v2.X) < -boxExtents.X || fmin(v0.X, v1.X, v2.X) > boxExtents.X)
			{
				return false;
			}

			// ... [-extents.Y, extents.Y] and [min(v0.Y,v1.Y,v2.Y), max(v0.Y,v1.Y,v2.Y)] do not overlap
			if (fmax(v0.Y, v1.Y, v2.Y) < -boxExtents.Y || fmin(v0.Y, v1.Y, v2.Y) > boxExtents.Y)
			{
				return false;
			}

			// ... [-extents.Z, extents.Z] and [min(v0.Z,v1.Z,v2.Z), max(v0.Z,v1.Z,v2.Z)] do not overlap
			if (fmax(v0.Z, v1.Z, v2.Z) < -boxExtents.Z || fmin(v0.Z, v1.Z, v2.Z) > boxExtents.Z)
			{
				return false;
			}

			#endregion

			#region Test separating axis corresponding to triangle face normal (category 2)

			var planeNormal = Vector3D.CrossProduct(f0, f1);
			var planeDistance = Vector3D.DotProduct(planeNormal, v0);

			// Compute the projection interval radius of b onto L(t) = b.c + t * p.n
			r = boxExtents.X * Math.Abs(planeNormal.X)
				+ boxExtents.Y * Math.Abs(planeNormal.Y)
				+ boxExtents.Z * Math.Abs(planeNormal.Z);

			// Intersection occurs when plane distance falls within [-r,+r] interval
			if (planeDistance > r)
			{
				return false;
			}

			#endregion

			return true;
		}

		private static float fmin(double a, double b, double c)
		{
			return (float)Math.Min(a, Math.Min(b, c));
		}

		private static float fmax(double a, double b, double c)
		{
			return (float)Math.Max(a, Math.Max(b, c));
		}
	}
}
