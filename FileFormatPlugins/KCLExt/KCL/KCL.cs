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
		List<KCLModel> Models = new List<KCLModel>();

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
			if (Models.Count != 1) throw new Exception("Exporting KCL with more than one model is not supported");
			var mod = Models[0];

			using (MemoryStream m = new MemoryStream())
			{
				BinaryDataWriter er = new BinaryDataWriter(m);
				//Write KCL Header
				er.ByteOrder = ByteOrder.BigEndian; //The signature is always big endian
				er.Write(0x02020000);
				er.ByteOrder = byteOrder;
				er.Write((UInt32)0x38);
				er.Write((UInt32)0x58);
				er.Write((UInt32)1);
				er.Write(mod.Header.OctreeOrigin);
				er.Write(mod.Header.OctreeMax);
				er.Write((UInt32)mod.Header.n_x);
				er.Write((UInt32)mod.Header.n_y);
				er.Write((UInt32)mod.Header.n_z);
				er.Write((UInt32)8);
				for (int i = 0; i < 8; i++) er.Write((UInt32)0x80000000); //Fake global model octree, supports only one model
				er.Write((UInt32)0x5C);
				//Write actual model
				long HeaderPos = er.BaseStream.Position;
				mod.Header.Write(er);
				long curpos = er.BaseStream.Position;
				//Write vertices array position
				er.BaseStream.Position = HeaderPos;
				er.Write((uint)curpos - HeaderPos);
				er.BaseStream.Position = curpos;
				foreach (Vector3D v in mod.Vertices) er.Write(v);
				while ((er.BaseStream.Position % 4) != 0) er.Write((byte)0);
				curpos = er.BaseStream.Position;
				//Write normal array position
				er.BaseStream.Position = HeaderPos + 4;
				er.Write((uint)curpos - HeaderPos);
				er.BaseStream.Position = curpos;
				foreach (Vector3D v in mod.Normals) er.Write(v);
				while ((er.BaseStream.Position % 4) != 0) er.Write((byte)0);
				curpos = er.BaseStream.Position;
				//Write Triangles offset
				er.BaseStream.Position = HeaderPos + 8;
				er.Write((uint)(curpos - HeaderPos));
				er.BaseStream.Position = curpos;
				int planeIndex = 0;
				foreach (KCLModel.KCLPlane p in mod.Planes) p.Write(er, planeIndex++);
				curpos = er.BaseStream.Position;
				//Write Spatial index offset
				er.BaseStream.Position = HeaderPos + 12;
				er.Write((uint)curpos - HeaderPos);
				er.BaseStream.Position = curpos;
				mod.Octree.Write(er);
				er.BaseStream.Position = er.BaseStream.Length;
				er.Write((ushort)0xFFFF); //Terminate Octree

				return m.ToArray();
			}
		}
		
		public static KCL FromOBJ(OBJ o)
		{
			KCL res = new KCL();

			List<Vector3D> Vertex = new List<Vector3D>();
			List<Vector3D> Normals = new List<Vector3D>();
			List<KCLModel.KCLPlane> planes = new List<KCLModel.KCLPlane>();
			List<Triangle> Triangles = new List<Triangle>();
			foreach (var v in o.Faces)
			{
				Triangle t = new Triangle(v.VA.pos, v.VB.pos, v.VC.pos);
				Vector3D qq = (t.PointB - t.PointA).Cross(t.PointC - t.PointA);
				if ((qq.X * qq.X + qq.Y * qq.Y + qq.Z * qq.Z) < 0.01) continue;
				KCLModel.KCLPlane p = new KCLModel.KCLPlane();
				p.CollisionType = 0;// Mapping[v.Material];
				Vector3D a = (t.PointC - t.PointA).Cross(t.Normal);
				a.Normalize();
				a = -a;
				Vector3D b = (t.PointB - t.PointA).Cross(t.Normal);
				b.Normalize();
				Vector3D c = (t.PointC - t.PointB).Cross(t.Normal);
				c.Normalize();
				p.Length = (t.PointC - t.PointA).Dot(c);
				int q = ContainsVector3D(t.PointA, Vertex);
				if (q == -1) { p.VertexIndex = (ushort)Vertex.Count; Vertex.Add(t.PointA); }
				else p.VertexIndex = (ushort)q;
				q = ContainsVector3D(t.Normal, Normals);
				if (q == -1) { p.NormalIndex = (ushort)Normals.Count; Normals.Add(t.Normal); }
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
				planes.Add(p);
				Triangles.Add(t);
			}

			KCLModel resMod = new KCLModel();
			resMod.Vertices = Vertex.ToArray();
			resMod.Normals = Normals.ToArray();
			resMod.Planes = planes.ToArray();
			resMod.Header = new KCLModel.KCLModelHeader();
			resMod.Octree = KCLOctree.FromTriangles(Triangles.ToArray(), resMod.Header, 2048, 128, 128, 50);
			res.Models.Add(resMod);

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
				public KCLModelHeader() { }
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
					er.ReadUInt32(); //Global plane index
				}
				public void Write(BinaryDataWriter er, int index)
				{
					er.Write(Length);
					er.Write(VertexIndex);
					er.Write(NormalIndex);
					er.Write(NormalAIndex);
					er.Write(NormalBIndex);
					er.Write(NormalCIndex);
					er.Write(CollisionType);
					er.Write((UInt32)index);
				}
				public Single Length;
				public UInt16 VertexIndex;
				public UInt16 NormalIndex;
				public UInt16 NormalAIndex;
				public UInt16 NormalBIndex;
				public UInt16 NormalCIndex;
				public UInt16 CollisionType;
			}

			public KCLOctree Octree;

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
		}
			
    }
}
