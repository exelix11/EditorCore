using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using Syroot.BinaryData;
using System.Numerics;
using Syroot.NintenTools.MarioKart8.IO;
using EditorCore.Common;
using LibEveryFileExplorer._3D;
using System.Windows.Media.Media3D;
using ExtensionMethods;

namespace MarioKart.MK7
{
	public class KCL
	{
		KCLModel.KCLModelHeader GlobalHeader;
		List<KCLModel> Models = new List<KCLModel>();

		internal static readonly Vector3D MinOffset = new Vector3D(50, 80, 50);
		internal static readonly Vector3D MaxOffset = new Vector3D(50, 50, 50);

		public KCL() { }

		public KCL(byte[] Data, ByteOrder bo = ByteOrder.LittleEndian)
		{
			BinaryDataReader er = new BinaryDataReader(new MemoryStream(Data));
			er.ByteOrder = ByteOrder.BigEndian;
			if (er.ReadUInt32() != 0x02020000) throw new Exception("Wrong KCL Header");
			er.ByteOrder = bo;

			/*uint OctreeOffset = */er.ReadUInt32();
			uint ModelListOff = er.ReadUInt32();
			uint ModelCount = er.ReadUInt32();

			for (int ModelIndex = 0; ModelIndex < ModelCount; ModelIndex++)
			{
				KCLModel mod = new KCLModel();

				er.BaseStream.Position = ModelListOff + ModelIndex * 4;
				uint CurModelOffset = er.ReadUInt32();
				er.BaseStream.Position = CurModelOffset;

				mod.Header = new KCLModel.KCLModelHeader(er);
				er.BaseStream.Position = mod.Header.VerticesOffset + CurModelOffset;
				uint nr = (mod.Header.NormalsOffset - mod.Header.VerticesOffset) / 0xC;
				mod.Vertices = new Vector3D[nr];
				for (int i = 0; i < nr; i++) mod.Vertices[i] = er.ReadVector3D();

				er.BaseStream.Position = mod.Header.NormalsOffset + CurModelOffset;
				nr = (mod.Header.PlanesOffset - mod.Header.NormalsOffset) / 0xC;
				mod.Normals = new Vector3D[nr];
				for (int i = 0; i < nr; i++) mod.Normals[i] = er.ReadVector3D();

				er.BaseStream.Position = mod.Header.PlanesOffset + CurModelOffset;
				nr = (mod.Header.OctreeOffset - mod.Header.PlanesOffset) / 0x14;
				mod.Planes = new KCLModel.KCLPlane[nr];
				for (int i = 0; i < nr; i++) mod.Planes[i] = new KCLModel.KCLPlane(er);

				er.BaseStream.Position = mod.Header.OctreeOffset + CurModelOffset;
				int nodes = (int)(
					((~mod.Header.XMask >> (int)mod.Header.CoordShift) + 1) *
					((~mod.Header.YMask >> (int)mod.Header.CoordShift) + 1) *
					((~mod.Header.ZMask >> (int)mod.Header.CoordShift) + 1));
				mod.Octree = new KCLOctree(er, nodes);

				Models.Add(mod);
			}
		}

		public byte[] Write(ByteOrder byteOrder)
		{
			if (Models.Count != 8)
				throw new Exception("The root octree is not complete");

			var size = GlobalHeader.OctreeMax - GlobalHeader.OctreeOrigin;
			int worldLengthExp = KCLOctree.next_exponent(Math.Min(Math.Min(size.X, size.Y), size.Z));
			var exponents = new Vector3D(
				KCLOctree.next_exponent(size.X),
				KCLOctree.next_exponent(size.Y),
				KCLOctree.next_exponent(size.Z));
			var CoordinateShift = new Vector3D(
				(float)KCLOctree.next_exponent(size.X),	//worldLengthExp,
				(float)KCLOctree.next_exponent(size.Y),	//exponents.X - worldLengthExp,
				(float)KCLOctree.next_exponent(size.Z));//exponents.X - worldLengthExp + exponents.Y - worldLengthExp);

			using (MemoryStream m = new MemoryStream())
			{
				BinaryDataWriter er = new BinaryDataWriter(m);
				//Write KCL Header
				er.ByteOrder = ByteOrder.BigEndian; //The signature is always big endian
				er.Write(0x02020000);
				er.ByteOrder = byteOrder;
				er.Write((UInt32)0x38);
				er.Write((UInt32)0x58);
				er.Write((UInt32)Models.Count);
				er.Write(GlobalHeader.OctreeOrigin);
				er.Write(GlobalHeader.OctreeMax);
				er.Write((UInt32)CoordinateShift.X);
				er.Write((UInt32)CoordinateShift.Y);
				er.Write((UInt32)CoordinateShift.Z);
				er.Write((UInt32)GlobalHeader.Unknown1);
				List<KCLModel> WriteModels = new List<KCLModel>();
				uint modelCount = 0;
				for (int i = 0; i < 8; i++)
				{
					if (Models[i] != null)
					{
						er.Write((UInt32)(0x80000000 | modelCount));
						modelCount++;
						WriteModels.Add(Models[i]);
					}
					else
						er.Write((UInt32)(0xC0000000));
				}
				if (modelCount == 0)
					throw new Exception("No models in the global octree");

				uint ModelListOff = (uint)er.BaseStream.Position;
				for (int i = 0; i < modelCount; i++) //Update offsets later
					er.Write((UInt32)0);

				for (int i = 0; i < modelCount; i++)
				{
					er.Align(4);
					uint pos = (uint)er.BaseStream.Position;
					er.BaseStream.Position = ModelListOff + i * 4;
					er.Write((UInt32)pos);
					er.BaseStream.Position = pos;
					WriteModel(er, WriteModels[i]);
				}

				return m.ToArray();
			}
		}

		static void WriteModel(BinaryDataWriter er, KCLModel mod)
		{
			long HeaderPos = er.BaseStream.Position;
			mod.Header.Write(er);
			long curpos = er.BaseStream.Position;
			//Write vertices array position
			er.BaseStream.Position = HeaderPos;
			er.Write((uint)(curpos - HeaderPos));
			er.BaseStream.Position = curpos;
			foreach (Vector3D v in mod.Vertices) er.Write(v);
			er.Align(4);
			curpos = er.BaseStream.Position;
			//Write normal array position
			er.BaseStream.Position = HeaderPos + 4;
			er.Write((uint)(curpos - HeaderPos));
			er.BaseStream.Position = curpos;
			foreach (Vector3D v in mod.Normals) er.Write(v);
			er.Align(4);
			curpos = er.BaseStream.Position;
			//Write Triangles offset
			er.BaseStream.Position = HeaderPos + 8;
			er.Write((uint)(curpos - HeaderPos));
			er.BaseStream.Position = curpos;

			foreach (KCLModel.KCLPlane p in mod.Planes) p.Write(er);
			curpos = er.BaseStream.Position;
			//Write Spatial index offset
			er.BaseStream.Position = HeaderPos + 12;
			er.Write((uint)(curpos - HeaderPos));
			er.BaseStream.Position = curpos;
			mod.Octree.Write(er);
		}

		public static Vector3D NormalAvg(OBJ.Face face) => (face.VA.normal + face.VB.normal + face.VC.normal)/3;
		
		public static KCL FromOBJ(OBJ o)
		{
			KCL res = new KCL();
			res.GlobalHeader = new KCLModel.KCLModelHeader();

			Vector3D min = new Vector3D(float.MaxValue, float.MaxValue, float.MaxValue);
			Vector3D max = new Vector3D(float.MinValue, float.MinValue, float.MinValue);
			
			List<Triangle> Triangles = new List<Triangle>();
			foreach (var v in o.Faces)
			{
				Triangle t = new Triangle(v.VA.pos, v.VB.pos, v.VC.pos);

				#region FindMaxMin
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
				#endregion
				
				Triangles.Add(t);
			}

			max += KCL.MaxOffset;
			min -= KCL.MinOffset;
			res.GlobalHeader.OctreeOrigin = min;
			res.GlobalHeader.OctreeMax = max;
			var size = max - min;
			res.GlobalHeader.CoordShift = (uint)KCLOctree.next_exponent(size.X);
			res.GlobalHeader.YShift = (uint)KCLOctree.next_exponent(size.Y);
			res.GlobalHeader.ZShift = (uint)KCLOctree.next_exponent(size.Z);

			size /= 2;
			uint baseTriCount = 0;
			for (int i = 0; i < 2; i++)
				for (int j = 0; j < 2; j++)
					for (int k = 0; k < 2; k++)
					{
						var origin = min + new Vector3D(size.X * k, size.Y * j, size.Z * i);
						int index = k + j * 2 + i * 4;
						var mod = KCLModel.FromTriangles(Triangles, baseTriCount, origin, size);
						res.Models.Add(mod);
						if (mod != null) baseTriCount += (uint)mod.Planes.Length; 
					}

			res.GlobalHeader.Unknown1 = baseTriCount;
			//resMod.Vertices = Vertex.ToArray();
			//resMod.Normals = Normals.ToArray();
			//resMod.Planes = planes.ToArray();
			//resMod.Header = new KCLModel.KCLModelHeader();
			//resMod.Octree = KCLOctree.FromTriangles(Triangles.ToArray(), resMod.Header, 128, 50);			

			return res;
		}

		private static int ContainsVector3D(Vector3D a, List<Vector3D> b)
		{
			for (int i = 0; i < b.Count; i++)
			{
				if (b[i].X == a.X && b[i].Y == a.Y && b[i].Z == a.Z)
				{
					return i;
				}
			}
			return -1;
		}

		public OBJ ToOBJ()
		{
			var res = new OBJ();
			foreach (var model in Models) res.Merge(model.ToOBJ());
			return res;
		}

		public class KCLModel
		{
			public KCLModelHeader Header;
			public class KCLModelHeader : KCLHeader
			{
				public KCLModelHeader()
				{
					Unknown1 = 40f;
					Unknown2 = 0f;
				}

				public KCLModelHeader(BinaryDataReader er)
				{
					VerticesOffset = er.ReadUInt32();
					NormalsOffset = er.ReadUInt32();
					PlanesOffset = er.ReadUInt32();
					OctreeOffset = er.ReadUInt32();
					Unknown1 = er.ReadSingle();
					OctreeOrigin = er.ReadVector3D();
					XMask = er.ReadUInt32();
					YMask = er.ReadUInt32();
					ZMask = er.ReadUInt32();
					CoordShift = er.ReadUInt32();
					YShift = er.ReadUInt32();
					ZShift = er.ReadUInt32();
					Unknown2 = er.ReadSingle();
				}
				public void Write(BinaryDataWriter er)
				{
					er.Write(VerticesOffset);
					er.Write(NormalsOffset);
					er.Write((uint)(PlanesOffset));
					er.Write(OctreeOffset);
					er.Write(Unknown1);
					er.Write(OctreeOrigin);
					er.Write(XMask);
					er.Write(YMask);
					er.Write(ZMask);
					er.Write(CoordShift);
					er.Write(YShift);
					er.Write(ZShift);
					er.Write(Unknown2);
				}
			}

			public Vector3D[] Vertices;
			public Vector3D[] Normals;

			public KCLPlane[] Planes;
			public class KCLPlane
			{
				public KCLPlane() { }
				public KCLPlane(BinaryDataReader er)
				{
					Length = er.ReadSingle();
					VertexIndex = er.ReadUInt16();
					NormalIndex = er.ReadUInt16();
					NormalAIndex = er.ReadUInt16();
					NormalBIndex = er.ReadUInt16();
					NormalCIndex = er.ReadUInt16();
					CollisionType = er.ReadUInt16();
					TriangleIndex = er.ReadUInt32(); //Global plane index
				}
				public void Write(BinaryDataWriter er)
				{
					er.Write(Length);
					er.Write(VertexIndex);
					er.Write(NormalIndex);
					er.Write(NormalAIndex);
					er.Write(NormalBIndex);
					er.Write(NormalCIndex);
					er.Write(CollisionType);
					er.Write((UInt32)TriangleIndex);
				}
				public Single Length;
				public UInt16 VertexIndex;
				public UInt16 NormalIndex;
				public UInt16 NormalAIndex;
				public UInt16 NormalBIndex;
				public UInt16 NormalCIndex;
				public UInt16 CollisionType;
				public UInt32 TriangleIndex;
			}

			public KCLOctree Octree;

			Vector3D NormalAvg(KCLPlane Plane) => (Normals[Plane.NormalAIndex] + Normals[Plane.NormalBIndex] + Normals[Plane.NormalCIndex]) / 3;

			public Triangle GetTriangle(KCLPlane Plane)
			{
				Vector3D A = Vertices[Plane.VertexIndex];
				Vector3D CrossA = Normals[Plane.NormalAIndex].Cross(Normals[Plane.NormalIndex]);
				Vector3D CrossB = Normals[Plane.NormalBIndex].Cross(Normals[Plane.NormalIndex]);
				Vector3D B = A + CrossB * (Plane.Length / CrossB.Dot(Normals[Plane.NormalCIndex]));
				Vector3D C = A + CrossA * (Plane.Length / CrossA.Dot(Normals[Plane.NormalCIndex]));
				return new Triangle(A, B, C);
			}

			public OBJ ToOBJ()
			{
				OBJ o = new OBJ();
				foreach (var vv in Planes)
				{
					Triangle t = GetTriangle(vv);
					var mat = o.GetOrAddMaterial("COL_" + vv.CollisionType.ToString("X"));
					var col = KCLColors.GetMaterialColor(vv.CollisionType);
					mat.Colors = new OBJ.Vertex(new Vector3D(), new Vector3D(col.R / 255f, col.G / 255f, col.B / 255f));
					o.Faces.Add(new OBJ.Face()
					{
						VA = new OBJ.Vertex(t.PointA, t.Normal),
						VB = new OBJ.Vertex(t.PointB, t.Normal),
						VC = new OBJ.Vertex(t.PointC, t.Normal),
						Mat = mat.Name
					});
				}
				return o;
			}

			internal static KCLModel FromTriangles(List<Triangle> triangles, uint baseTriCount, Vector3D origin, Vector3D halfSize)
			{
				List<Triangle> modelTri = new List<Triangle>();
				List<Vector3D> Vertices = new List<Vector3D>();
				List<Vector3D> Normals = new List<Vector3D>();
				List<KCLPlane> Planes = new List<KCLPlane>();

				var center = origin + halfSize;
				for (int i = 0; i < triangles.Count; i++)
				{
					if (!KCLExt.KCL.TriangleBoxIntersect.IntersectsBox(triangles[i], center, halfSize)) continue;
					modelTri.Add(triangles[i]);

					KCLModel.KCLPlane p = new KCLModel.KCLPlane();
					p.CollisionType = 0;// Mapping[v.Material];
					Vector3D a = (triangles[i].PointC - triangles[i].PointA).Cross(triangles[i].Normal);
					a.Normalize();
					a = -a;
					Vector3D b = (triangles[i].PointB - triangles[i].PointA).Cross(triangles[i].Normal);
					b.Normalize();
					Vector3D c = (triangles[i].PointC - triangles[i].PointB).Cross(triangles[i].Normal);
					c.Normalize();
					p.Length = (triangles[i].PointC - triangles[i].PointA).Dot(c);
					int q = ContainsVector3D(triangles[i].PointA, Vertices);
					if (q == -1) { p.VertexIndex = (ushort)Vertices.Count; Vertices.Add(triangles[i].PointA); }
					else p.VertexIndex = (ushort)q;
					q = ContainsVector3D(triangles[i].Normal, Normals);
					if (q == -1) { p.NormalIndex = (ushort)Normals.Count; Normals.Add(triangles[i].Normal); }
					else p.NormalIndex = (ushort)q;
					q = ContainsVector3D(a, Normals);
					if (q == -1) { p.NormalAIndex = (ushort)Normals.Count; Normals.Add(a); }
					else p.NormalAIndex = (ushort)q;
					q = ContainsVector3D(b, Normals);
					if (q == -1) { p.NormalBIndex = (ushort)Normals.Count; Normals.Add(b); }
					else p.NormalBIndex = (ushort)q;
					q = ContainsVector3D(c, Normals);
					if (q == -1) { p.NormalCIndex = (ushort)Normals.Count; Normals.Add(c); }
					else p.NormalCIndex = (ushort)q;

					p.TriangleIndex = baseTriCount + (uint)modelTri.Count -1;
					Planes.Add(p);
				}
				if (modelTri.Count == 0) return null;
				if (modelTri.Count > 65535) throw new Exception("Too many triangles");

				KCLModel resMod = new KCLModel();
				resMod.Vertices = Vertices.ToArray();
				resMod.Normals = Normals.ToArray();
				if (Planes.Count != modelTri.Count)
					throw new Exception();
				resMod.Planes = Planes.ToArray();
				resMod.Header = new KCLModelHeader();
				resMod.Octree = KCLOctree.FromTriangles(modelTri.ToArray(), resMod.Header);

				resMod.Header.Unknown1 = 40f; //odyssey values
				resMod.Header.Unknown2 = 0;
				return resMod;
			}
		}
			
    }
}
