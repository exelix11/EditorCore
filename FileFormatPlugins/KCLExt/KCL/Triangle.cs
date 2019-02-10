using ExtensionMethods;
using OpenTK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibEveryFileExplorer._3D
{
	public class Triangle
	{
		public Triangle(Vector3 A, Vector3 B, Vector3 C)
		{
			PointA = A;
			PointB = B;
			PointC = C;
		}

		public ushort Collision { get; set; }

		public Vector3 PointA { get; internal set; }
		public Vector3 PointB { get; internal set; }
		public Vector3 PointC { get; internal set; }

		public Vector3 Normal
		{
			get
			{
				return Vector3.Cross((PointB - PointA),(PointC - PointA)).GetNormalize();
			}
		}
	}
}
