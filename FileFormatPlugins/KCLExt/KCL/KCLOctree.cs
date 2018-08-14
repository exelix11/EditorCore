using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LibEveryFileExplorer._3D;
using Syroot.BinaryData;
using System.Windows.Media.Media3D;
using ExtensionMethods;
using LibEveryFileExplorer.Math;

namespace MarioKart
{
	public class KCLOctree
	{
		public KCLOctree() { }
		public KCLOctree(BinaryDataReader er, int NrNodes)
		{
			long baseoffset = er.BaseStream.Position;
			RootNodes = new KCLOctreeNode[NrNodes];
			for (int i = 0; i < NrNodes; i++)
			{
				RootNodes[i] = new KCLOctreeNode(er, baseoffset);
			}
		}

		public void Write(BinaryDataWriter er)
		{
			long basepos = er.BaseStream.Position;
			Queue<uint> NodeBaseOffsets = new Queue<uint>();
			Queue<KCLOctreeNode> Nodes = new Queue<KCLOctreeNode>();
			foreach (var v in RootNodes)
			{
				NodeBaseOffsets.Enqueue(0);
				Nodes.Enqueue(v);
			}
			uint offs = (uint)(RootNodes.Length * 4);
			while (Nodes.Count > 0)
			{
				KCLOctreeNode n = Nodes.Dequeue();
				if (n.IsLeaf)
				{
					NodeBaseOffsets.Dequeue();
					er.Write((uint)0);
				}
				else
				{
					n.DataOffset = offs - NodeBaseOffsets.Dequeue();
					er.Write(n.DataOffset);
					foreach (var v in n.SubNodes)
					{
						NodeBaseOffsets.Enqueue(offs);
						Nodes.Enqueue(v);
					}
					offs += 8 * 4;
				}
			}
			foreach (var v in RootNodes)
			{
				NodeBaseOffsets.Enqueue(0);
				Nodes.Enqueue(v);
			}
			long leafstartpos = er.BaseStream.Position;
			uint relleafstartpos = offs;
			er.BaseStream.Position = basepos;
			offs = (uint)(RootNodes.Length * 4);
			while (Nodes.Count > 0)
			{
				KCLOctreeNode n = Nodes.Dequeue();
				if (n.IsLeaf)
				{
					er.Write((uint)(0x80000000 | (relleafstartpos - NodeBaseOffsets.Dequeue() - 2)));
					long curpos = er.BaseStream.Position;
					er.BaseStream.Position = leafstartpos;
					foreach (var v in n.Triangles)
					{
						er.Write((ushort)(v));
					}
					er.Write((ushort)0xFFFF);
					relleafstartpos += (uint)(n.Triangles.Length * 2) + 2;
					leafstartpos = er.BaseStream.Position;
					er.BaseStream.Position = curpos;
				}
				else
				{
					er.BaseStream.Position += 4;
					NodeBaseOffsets.Dequeue();
					foreach (var v in n.SubNodes)
					{
						NodeBaseOffsets.Enqueue(offs);
						Nodes.Enqueue(v);
					}
					offs += 8 * 4;
				}
			}
			er.Position = (int)er.BaseStream.Length;
		}

		static int GetNodeCount(KCLOctreeNode[] nodes)
		{
			int count = nodes.Length;
			foreach (KCLOctreeNode node in nodes)
			{
				if (!node.IsLeaf)
					count += GetNodeCount(node.SubNodes);
			}
			return count;
		}

		//public void Write(BinaryDataWriter er)
		//{
		//	long basepos = er.BaseStream.Position;
		//	int emptyListPos = GetNodeCount(RootNodes) * sizeof(uint);
		//	int triangleListPos = emptyListPos + sizeof(ushort);
		//	Queue<KCLOctreeNode[]> queuedNodes = new Queue<KCLOctreeNode[]>();
		//	queuedNodes.Enqueue(RootNodes);
		//	while (queuedNodes.Count > 0)
		//	{
		//		KCLOctreeNode[] nodes = queuedNodes.Dequeue();
		//		long offset = er.Position - basepos;
		//		foreach (KCLOctreeNode node in nodes)
		//		{
		//			uint key;
		//			if (node.IsLeaf)
		//			{
		//				// Node is a leaf and points to triangle index list.
		//				int listPos;
		//				if (node.Triangles.Length == 0)
		//				{
		//					listPos = emptyListPos;
		//				}
		//				else
		//				{
		//					listPos = triangleListPos;
		//					triangleListPos += (node.Triangles.Length + 1) * sizeof(ushort);
		//				}
		//				key = 0x80000000u | (uint)(listPos - offset - sizeof(ushort));
		//			}
		//			else
		//			{
		//				// Node is a branch and points to 8 children.
		//				key = (uint)(nodes.Length + queuedNodes.Count * 8) * sizeof(uint);
		//				queuedNodes.Enqueue(node.SubNodes);
		//			}
		//			er.Write(key);
		//		}
		//	}
		//	// Iterate through the nodes again and write their triangle lists now.
		//	er.Write((ushort)0xFFFF); // Terminator for all empty lists.
		//	queuedNodes.Enqueue(RootNodes);
		//	while (queuedNodes.Count > 0)
		//	{
		//		KCLOctreeNode[] nodes = queuedNodes.Dequeue();
		//		foreach (KCLOctreeNode node in nodes)
		//		{
		//			if (node.IsLeaf)
		//			{
		//				if (node.Triangles.Length > 0)
		//				{
		//					// Node is a leaf and points to triangle index list.
		//					er.Write(node.Triangles);
		//					er.Write((ushort)0xFFFF);
		//				}
		//			}
		//			else
		//			{
		//				// Node is a branch and points to 8 children.
		//				queuedNodes.Enqueue(node.SubNodes);
		//			}
		//		}
		//	}
		//}


		public KCLOctreeNode[] RootNodes;

		public class KCLOctreeNode
		{
			public KCLOctreeNode() { }
			public KCLOctreeNode(BinaryDataReader er, long BaseOffset)
			{
				DataOffset = er.ReadUInt32();
				IsLeaf = (DataOffset >> 31) == 1;
				DataOffset &= 0x7FFFFFFF;
				long curpos = er.BaseStream.Position;
				er.BaseStream.Position = BaseOffset + DataOffset;
				if (IsLeaf)
				{
					er.BaseStream.Position += 2;//Skip starting zero
					List<ushort> tris = new List<ushort>();
					while (true)
					{
						ushort v = er.ReadUInt16();
						if (v == 0xFFFF) break;
						tris.Add((ushort)(v));
					}
					Triangles = tris.ToArray();
				}
				else
				{
					SubNodes = new KCLOctreeNode[8];
					for (int i = 0; i < 8; i++)
					{
						SubNodes[i] = new KCLOctreeNode(er, BaseOffset + DataOffset);
					}
				}
				er.BaseStream.Position = curpos;
			}

			public UInt32 DataOffset;
			public Boolean IsLeaf;

			public KCLOctreeNode[] SubNodes;
			public ushort[] Triangles;

			public static KCLOctreeNode Generate(Dictionary<ushort, Triangle> Triangles, Vector3D Position, float BoxSize, int MaxTris, int MinSize)
			{
				KCLOctreeNode n = new KCLOctreeNode();
				Vector3D midpos = Position + new Vector3D(BoxSize / 2f, BoxSize / 2f, BoxSize / 2f);
				
				Dictionary<ushort, Triangle> t = new Dictionary<ushort, Triangle>();
				foreach (var v in Triangles)
				{
					if (tricube_overlap(v.Value, midpos, BoxSize)) t.Add(v.Key, v.Value);
				}
				if (BoxSize > MinSize && t.Count > MaxTris)
				{
					n.IsLeaf = false;
					float childsize = BoxSize / 2f;
					n.SubNodes = new KCLOctreeNode[8];
					int i = 0;
					for (int z = 0; z < 2; z++)
					{
						for (int y = 0; y < 2; y++)
						{
							for (int x = 0; x < 2; x++)
							{
								Vector3D pos = Position + childsize * new Vector3D(x, y, z);
								n.SubNodes[i] = KCLOctreeNode.Generate(t, pos, childsize, MaxTris, MinSize);
								i++;
							}
						}
					}
				}
				else
				{
					n.IsLeaf = true;
					n.Triangles = t.Keys.ToArray();
				}
				return n;
			}

			private static bool axis_test(double a1, double a2, double b1, double b2, double c1, double c2, double half)
			{
				float p = ((float)a1 * (float)b1 + (float)a2 * (float)b2);
				float q = ((float)a1 * (float)c1 + (float)a2 * (float)c2);
				float r = ((float)half * (Math.Abs((float)a1) + Math.Abs((float)a2)));
				return Math.Min(p, q) > r || Math.Max(p, q) < -r;
			}
			//Based on this algorithm: http://jgt.akpeters.com/papers/AkenineMoller01/tribox.html
			public static bool tricube_overlap(Triangle t, Vector3D Position, float BoxSize)
			{
				float half = BoxSize / 2f;
				//Position is the min pos, so add half the box size
				Position += new Vector3D(half, half, half);
				Vector3D v0 = t.PointA - Position;
				Vector3D v1 = t.PointB - Position;
				Vector3D v2 = t.PointC - Position;

				if (Math.Min(Math.Min(v0.X, v1.X), v2.X) > half || Math.Max(Math.Max(v0.X, v1.X), v2.X) < -half) return false;
				if (Math.Min(Math.Min(v0.Y, v1.Y), v2.Y) > half || Math.Max(Math.Max(v0.Y, v1.Y), v2.Y) < -half) return false;
				if (Math.Min(Math.Min(v0.Z, v1.Z), v2.Z) > half || Math.Max(Math.Max(v0.Z, v1.Z), v2.Z) < -half) return false;

				float d = t.Normal.Dot(v0);
				float r = (float)(half * (Math.Abs(t.Normal.X) + Math.Abs(t.Normal.Y) + Math.Abs(t.Normal.Z)));
				if (d > r || d < -r) return false;

				Vector3D e = v1 - v0;
				if (axis_test(e.Z, -e.Y, v0.Y, v0.Z, v2.Y, v2.Z, half)) return false;
				if (axis_test(-e.Z, e.X, v0.X, v0.Z, v2.X, v2.Z, half)) return false;
				if (axis_test(e.Y, -e.X, v1.X, v1.Y, v2.X, v2.Y, half)) return false;

				e = v2 - v1;
				if (axis_test(e.Z, -e.Y, v0.Y, v0.Z, v2.Y, v2.Z, half)) return false;
				if (axis_test(-e.Z, e.X, v0.X, v0.Z, v2.X, v2.Z, half)) return false;
				if (axis_test(e.Y, -e.X, v0.X, v0.Y, v1.X, v1.Y, half)) return false;

				e = v0 - v2;
				if (axis_test(e.Z, -e.Y, v0.Y, v0.Z, v1.Y, v1.Z, half)) return false;
				if (axis_test(-e.Z, e.X, v0.X, v0.Z, v1.X, v1.Z, half)) return false;
				if (axis_test(e.Y, -e.X, v1.X, v1.Y, v2.X, v2.Y, half)) return false;
				return true;
			}			
		}

		public static int next_exponent(double value) => value <= 1 ? 0 : (int)Math.Ceiling(Math.Log(value, 2));

		public static KCLOctree FromTriangles(Triangle[] Triangles, KCLHeader Header,  int MinCubeSize = 32, int MaxNrTris = 50,uint MaxRootSize = 2048, uint MinRootSize = 128)//35)
		{
			Vector3D min = new Vector3D(float.MaxValue, float.MaxValue, float.MaxValue);
			Vector3D max = new Vector3D(float.MinValue, float.MinValue, float.MinValue);
			Dictionary<ushort, Triangle> tt = new Dictionary<ushort, Triangle>();
			ushort index = 0;
			foreach (var t in Triangles)
			{
				if (t.PointA.X < min.X) min.X = t.PointA.X;
				if (t.PointA.Y < min.Y) min.Y = t.PointA.Y;
				if (t.PointA.Z < min.Z) min.Z = t.PointA.Z;
				if (t.PointA.X > max.X) max.X = t.PointA.X;
				if (t.PointA.Y > max.Y) max.Y = t.PointA.Y;
				if (t.PointA.Z > max.Z) max.Z = t.PointA.Z;

				if (t.PointB.X < min.X) min.X = t.PointB.X;
				if (t.PointB.Y < min.Y) min.Y = t.PointB.Y;
				if (t.PointB.Z < min.Z) min.Z = t.PointB.Z;
				if (t.PointB.X > max.X) max.X = t.PointB.X;
				if (t.PointB.Y > max.Y) max.Y = t.PointB.Y;
				if (t.PointB.Z > max.Z) max.Z = t.PointB.Z;

				if (t.PointC.X < min.X) min.X = t.PointC.X;
				if (t.PointC.Y < min.Y) min.Y = t.PointC.Y;
				if (t.PointC.Z < min.Z) min.Z = t.PointC.Z;
				if (t.PointC.X > max.X) max.X = t.PointC.X;
				if (t.PointC.Y > max.Y) max.Y = t.PointC.Y;
				if (t.PointC.Z > max.Z) max.Z = t.PointC.Z;
				tt.Add(index, t);
				index++;
			}
			//in real mkds, 25 is subtracted from the min pos
			min -= MarioKart.MK7.KCL.MinOffset;
			//TODO: after that, from some of the components (may be more than one) 30 is subtracted aswell => How do I know from which ones I have to do that?

			//Assume the same is done for max:
			max += MarioKart.MK7.KCL.MaxOffset;
			//TODO: +30
			Header.OctreeOrigin = min;
			Header.OctreeMax = max;
			Vector3D size = max - min;
			float mincomp = (float)Math.Min(Math.Min(size.X, size.Y), size.Z);
			int CoordShift = next_exponent(mincomp);

			//if (CoordShift > MathUtil.GetNearest2Power((float)MaxRootSize))
			//	CoordShift = MathUtil.GetNearest2Power((float)MaxRootSize);
			//else if (CoordShift < next_exponent(MinRootSize))
			//	CoordShift = next_exponent(MinRootSize);

			Header.CoordShift = (uint)CoordShift;
			int cubesize = 1 << CoordShift;
			int NrX = (1 << next_exponent(size.X)) / cubesize;
			int NrY = (1 << next_exponent(size.Y)) / cubesize;
			int NrZ = (1 << next_exponent(size.Z)) / cubesize;
			if (NrX <= 0) NrX = 1;
			if (NrY <= 0) NrY = 1;
			if (NrZ <= 0) NrZ = 1;
			Header.YShift = (uint)(next_exponent(size.X) - CoordShift);
			Header.ZShift = (uint)(next_exponent(size.X) - CoordShift + next_exponent(size.Y) - CoordShift);
			Header.XMask = 0xFFFFFFFF << next_exponent(size.X);
			Header.YMask = 0xFFFFFFFF << next_exponent(size.Y);
			Header.ZMask = 0xFFFFFFFF << next_exponent(size.Z);

			KCLOctree k = new KCLOctree();
			k.RootNodes = new KCLOctreeNode[NrX * NrY * NrZ];
			int i = 0;
			for (int z = 0; z < NrZ; z++)
			{
				for (int y = 0; y < NrY; y++)
				{
					for (int x = 0; x < NrX; x++)
					{
						Vector3D pos = min + ((float)cubesize) * new Vector3D(x, y, z);
						k.RootNodes[i] = KCLOctreeNode.Generate(tt, pos, cubesize, MaxNrTris, MinCubeSize);
						i++;
					}
				}
			}
			return k;
		}
	}
}