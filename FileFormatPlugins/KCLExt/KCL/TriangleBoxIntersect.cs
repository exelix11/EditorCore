using ExtensionMethods;
using LibEveryFileExplorer._3D;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace KCLExt.KCL
{
	//based on http://fileadmin.cs.lth.se/cs/Personal/Tomas_Akenine-Moller/code/tribox3.txt
	static class TriangleBoxIntersect
	{
		private const int X = 0;
		private const int Y = 1;
		private const int Z = 2;
		private static bool planeBoxOverlap(Vector3 normal, Vector3 vert, Vector3 maxbox)    
		{
			int q;
			Vector3 vmin = new Vector3(), vmax = new Vector3();
			float v;
			for (q = X; q <= Z; q++)
			{
				v = vert.Get(q);                    // -NJMP-
				if (normal.Get(q) > 0.0f)
				{
					vmin.Set(q, -maxbox.Get(q) - v);   // -NJMP-
					vmax.Set(q,  maxbox.Get(q) - v);    // -NJMP-
				}
				else
				{
					vmin.Set(q, maxbox.Get(q) - v);    // -NJMP-
					vmax.Set(q, -maxbox.Get(q) - v);   // -NJMP-
				}
			}
			if (normal.Dot(vmin) > 0.0f) return false; // -NJMP-
			if (normal.Dot(vmax) >= 0.0f) return true;    // -NJMP-
			return false;
		}

		public static bool triBoxOverlap(Vector3 boxcenter, Vector3 boxhalfsize, Triangle tri)
		{
			/*    use separating axis theorem to test overlap between triangle and box */
			/*    need to test for overlap in these directions: */
			/*    1) the {x,y,z}-directions (actually, since we use the AABB of the triangle */
			/*       we do not even need to test these) */
			/*    2) normal of the triangle */
			/*    3) crossproduct(edge from tri, {x,y,z}-directin) */
			/*       this gives 3x3=9 more tests */
			Vector3 v0, v1, v2;
			//   float axis[3];
			double min, max, p0, p1, p2, rad, fex, fey, fez;     // -NJMP- "d" local variable removed
			Vector3 normal, e0, e1, e2;

			bool AXISTEST_X01(double a, double b, double fa, double fb)
			{
				p0 = (a * v0.Y - b * v0.Z);
				p2 = (a * v2.Y - b * v2.Z);
				if (p0 < p2) { min = p0; max = p2; } else { min = p2; max = p0; }
				rad = (fa * boxhalfsize.Y + fb * boxhalfsize.Z);
				if (min > rad || max < -rad) return false;
				return true;
			}

			bool AXISTEST_Y02(double a, double b, double fa, double fb)
			{
				p0 = -a * v0.X + b * v0.Z;
				p2 = -a * v2.X + b * v2.Z;
				if (p0 < p2) { min = p0; max = p2; } else { min = p2; max = p0; }
				rad = fa * boxhalfsize.X + fb * boxhalfsize.Z;
				if (min > rad || max < -rad) return false;
				return true;
			}

			bool AXISTEST_Z12(double a, double b, double fa, double fb)
			{
				p1 = a * v1.X - b * v1.Y;
				p2 = a * v2.X - b * v2.Y;
				if (p2 < p1) { min = p2; max = p1; } else { min = p1; max = p2; }
				rad = fa * boxhalfsize.X + fb * boxhalfsize.Y;
				return min <= rad && max >= -rad;
			}

			bool AXISTEST_Z0(double a, double b, double fa, double fb)
			{
				p0 = a * v0.X - b * v0.Y;
				p1 = a * v1.X - b * v1.Y;
				if (p0 < p1) { min = p0; max = p1; } else { min = p1; max = p0; }
				rad = fa * boxhalfsize.X + fb * boxhalfsize.Y;
				if (min > rad || max < -rad) return false;
				return true;
			}

			bool AXISTEST_X2(double a, double b, double fa, double fb)
			{
				p0 = a * v0.Y - b * v0.Z;
				p1 = a * v1.Y - b * v1.Z;
				if (p0 < p1) { min = p0; max = p1; } else { min = p1; max = p0; }
				rad = fa * boxhalfsize.Y + fb * boxhalfsize.Z;
				if (min > rad || max < -rad) return false;
				return true;
			}

			bool AXISTEST_Y1(double a, double b, double fa, double fb)
			{
				p0 = -a * v0.X + b * v0.Z;
				p1 = -a * v1.X + b * v1.Z;
				if (p0 < p1) { min = p0; max = p1; } else { min = p1; max = p0; }
				rad = fa * boxhalfsize.X + fb * boxhalfsize.Z;
				if (min > rad || max < -rad) return false;
				return true;
			}

			/* This is the fastest branch on Sun */

			/* move everything so that the boxcenter is in (0,0,0) */
			v0 = tri.PointA - boxcenter;
			v1 = tri.PointB - boxcenter;
			v2 = tri.PointC - boxcenter;
			/* compute triangle edges */
			e0 = v1 - v0;
			e1 = v2 - v1;
			e2 = v0 - v2;
			/* Bullet 3:  */
			/*  test the 9 tests first (this was faster) */
			fex = Math.Abs((float)e0.X);
			fey = Math.Abs((float)e0.Y);
			fez = Math.Abs((float)e0.Z);

			if (!AXISTEST_X01((float)e0.Z, (float)e0.Y, fez, fey)) return false;
			if (!AXISTEST_Y02(e0.Z, e0.X, fez, fex)) return false;
			if (!AXISTEST_Z12(e0.Y, e0.X, fey, fex)) return false;

			fex = Math.Abs(e1.X);
			fey = Math.Abs(e1.Y);
			fez = Math.Abs(e1.Z);
			if (!AXISTEST_X01(e1.Z, e1.Y, fez, fey)) return false;
			if (!AXISTEST_Y02(e1.Z, e1.X, fez, fex)) return false;
			if (!AXISTEST_Z0(e1.Y, e1.X, fey, fex)) return false;

			fex = Math.Abs(e2.X);
			fey = Math.Abs(e2.Y);
			fez = Math.Abs(e2.Z);
			if (!AXISTEST_X2(e2.Z, e2.Y, fez, fey)) return false;
			if (!AXISTEST_Y1(e2.Z, e2.X, fez, fex)) return false;
			if (!AXISTEST_Z12(e2.Y, e2.X, fey, fex)) return false;

			/* Bullet 1: */
			/*  first test overlap in the {x,y,z}-directions */
			/*  find min, max of the triangle each direction, and test for overlap in */
			/*  that direction -- this is equivalent to testing a minimal AABB around */
			/*  the triangle against the AABB */
			/* test in X-direction */

			void FINDMINMAX(double x0, double x1, double x2, out double m_min,out double m_max)
			{
				m_min = m_max = x0;
				if (x1 < m_min) m_min = x1;
				if (x1 > m_max) m_max = x1;
				if (x2 < m_min) m_min = x2;
				if (x2 > m_max) m_max = x2;
			}

			FINDMINMAX(v0.X, v1.X, v2.X, out min, out max);
			if (min > boxhalfsize.X || max < -boxhalfsize.X) return false;

			/* test in Y-direction */

			FINDMINMAX(v0.Y, v1.Y, v2.Y, out min, out max);
			if (min > boxhalfsize.Y || max < -boxhalfsize.Y) return false;

			/* test in Z-direction */
			FINDMINMAX(v0.Z, v1.Z, v2.Z, out min, out max);
			if (min > boxhalfsize.Z || max < -boxhalfsize.Z) return false;

			/* Bulet 2: */
			/*  test if the box intersects the plane of the triangle */
			/*  compute plane equation of triangle: normal*x+d=0 */
			normal = e0.Cross(e1);
			// -NJMP- (line removed here)
			if (!planeBoxOverlap(normal, v0, boxhalfsize)) return false;    // -NJMP-
			return true;   /* box and triangle overlaps */
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
