using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Windows.Media.Media3D;

namespace MarioKart
{
	public abstract class KCLHeader
	{
		public UInt32 VerticesOffset;
		public UInt32 NormalsOffset;
		public UInt32 PlanesOffset;//-0x10
		public UInt32 OctreeOffset;
		public Single Unknown1;
		public Vector3D OctreeOrigin;
		public Vector3D OctreeMax;
		public float n_x;
		public float n_y;
		public float n_z;
		public UInt32 XMask;
		public UInt32 YMask;
		public UInt32 ZMask;
		public UInt32 CoordShift;
		public UInt32 YShift;
		public UInt32 ZShift;
		public Single Unknown2;
	}
}
