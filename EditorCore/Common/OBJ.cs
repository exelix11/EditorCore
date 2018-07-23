using ExtensionMethods;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorCore.Common
{
	public class OBJ
	{
		public struct Vertex : IEquatable<Vertex>
		{
			public Vertex(float x, float y, float z, float nx , float ny, float nz , float? u = null, float? v = null, float? w = null)
			{
				X = x; Y = y; Z = z;
				NX = nx; NY = ny; NZ = nz;
				U = u;
				V = v;
				W = z;
			}

			public float X, Y, Z;
			public float NX, NY, NZ;
			public float? U, V, W; //Texture Coords

			public bool Equals(Vertex other)
			{
				return
					other.X == X && other.Y == Y && other.Z == Z &&
					other.NX == NX && other.NY == NY && other.NZ == NZ &&
					other.U == U && other.V == V && other.W == W;
			}

			public override bool Equals(object obj) => obj is Vertex ? Equals((Vertex)obj) : false;
			public static bool operator ==(Vertex c1, Vertex c2) => c1.Equals(c2);
			public static bool operator !=(Vertex c1, Vertex c2) => !c1.Equals(c2);
		}

		public struct Face : IEquatable<Face>
		{
			public Material Mat;

			public Vertex VA;
			public Vertex VB;
			public Vertex VC;

			public Vertex[] Vertices => new Vertex[3] {VA,VB,VC};

			public bool Equals(Face other) => VA == other.VA && VB == other.VB && VC == other.VB && Mat == other.Mat;
			public static bool operator ==(Face c1, Face c2) => c1.Equals(c2);
			public static bool operator !=(Face c1, Face c2) => !c1.Equals(c2);
		}

		public class Material : IEquatable<Material>
		{
			public string Name;
			public Vertex Colors; //XYZ are Ambient (KA) NXNYNZ are diffuse (KD) UVW are specular (KS)

			public string TextureMapName = null;
			public bool Equals(Material other)
			{
				return
					other.Name == Name &&
					other.TextureMapName == TextureMapName &&
					other.Colors == Colors;
			}
			public static bool operator ==(Material c1, Material c2) => c1.Equals(c2);
			public static bool operator !=(Material c1, Material c2) => !c1.Equals(c2);
		}

		public List<Face> Faces = new List<Face>();

		public WritableObj toWritableObj()
		{
			WritableObj res = new WritableObj();
			List<Material> Materials = new List<Material>();
			List<Vertex> Vertices = new List<Vertex>();
			List<WritableObj.WritableFace> WFaces = new List<WritableObj.WritableFace>();

			foreach (var f in Faces)
			{
				var face = new WritableObj.WritableFace();
				face.MaterialIndex = Materials.AddIfNotContins(f.Mat);
				face.FaceIndex = Vertices.Count + 1; //Objs are 1-indexed
				Vertices.Add(f.VA);
				Vertices.Add(f.VB);
				Vertices.Add(f.VC);
				WFaces.Add(face);
			}

			res.Vertices = Vertices;
			res.Materials = Materials;
			res.Faces = WFaces;

			return res;
		}
	}

	public class WritableObj
	{
		public struct WritableFace
		{
			public int FaceIndex;
			public int MaterialIndex;
		}

		public List<OBJ.Material> Materials;
		public List<OBJ.Vertex> Vertices;
		public List<WritableFace> Faces;

		public void WriteObj(string FileName)
		{
			using (StreamWriter f = new System.IO.StreamWriter(FileName))
				WriteObj(f, Path.GetFileName(FileName + ".mtl"));

			using (StreamWriter f = new System.IO.StreamWriter(FileName + ".mtl"))
				WriteMtl(f);
		}

		public void WriteObj(StreamWriter s, string mtlLibName)
		{
			if (mtlLibName != null)
				s.WriteLine($"mtllib {mtlLibName}");

			foreach (var v in Vertices)
			{
				s.WriteLine($"v {v.X} {v.Y} {v.Z}");
				s.WriteLine($"vn {v.NX} {v.NY} {v.NZ}");
				if (v.U != null)
					s.WriteLine($"vt {v.U} {v.V}{(v.W != null ? v.W.ToString() : "")}");
			}

			int curmat = -1;
			foreach (var f in Faces.OrderBy(x => x.MaterialIndex))
			{
				if (f.MaterialIndex != curmat)
				{
					curmat = f.MaterialIndex;
					s.WriteLine($"usemtl {Materials[curmat].Name}");
				}

				if (Vertices[f.FaceIndex].U != null)
					s.WriteLine(
						$"f {f.FaceIndex}//{f.FaceIndex}" +
						 $" {f.FaceIndex + 1}//{f.FaceIndex + 1}" +
						 $" {f.FaceIndex + 2}//{f.FaceIndex + 2}");
				else
					s.WriteLine(
						$"f {f.FaceIndex}/{f.FaceIndex}/{f.FaceIndex}" +
						 $" {f.FaceIndex + 1}/{f.FaceIndex + 1}/{f.FaceIndex + 1}" +
						 $" {f.FaceIndex + 2}/{f.FaceIndex + 2}/{f.FaceIndex + 2}");
			}
		}

		public void WriteMtl(StreamWriter s)
		{
			foreach (var m in Materials)
			{
				s.WriteLine($"newmtl {m.Name}");
				s.WriteLine($"Ka {m.Colors.X} {m.Colors.Y} {m.Colors.Z}");
				s.WriteLine($"Kd {m.Colors.NX} {m.Colors.NY} {m.Colors.NZ}");
				if (m.Colors.U != null) s.WriteLine($"Ks {m.Colors.U} {m.Colors.V} {m.Colors.W}");
				if (m.TextureMapName != null) s.WriteLine($"map_Kd {m.TextureMapName}");
			}
		}

		public OBJ ToObj()
		{
			throw new NotImplementedException();
		}
	}
}
