using ExtensionMethods;
using System;
using System.Collections;
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

		public Vector3D PointA { get; internal set; }
		public Vector3D PointB { get; internal set; }
		public Vector3D PointC { get; internal set; }

		public Vector3D Normal
		{
			get
			{
				return (PointB - PointA).Cross(PointC - PointA).GetNormalize();
			}
		}
	}
}
