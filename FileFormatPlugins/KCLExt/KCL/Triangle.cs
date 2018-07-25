using ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace LibEveryFileExplorer._3D
{
	public class Triangle
	{
		public Triangle(Vector3D A, Vector3D B, Vector3D C)
		{
			PointA = A;
			PointB = B;
			PointC = C;
		}

		public Vector3D PointA { get; set; }
		public Vector3D PointB { get; set; }
		public Vector3D PointC { get; set; }

		public Vector3D Normal
		{
			get
			{
				Vector3D a = (PointB - PointA).Cross(PointC - PointA);
				return a / (float)System.Math.Sqrt(a.X * a.X + a.Y * a.Y + a.Z * a.Z);
			}
		}
	}
}
