using ExtensionMethods;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace EditorCore.Common
{
	public class OBJ
	{

		static Vector3D Vec(string x, string y, string z) => new Vector3D(float.Parse(x), float.Parse(y), float.Parse(z));
		
		public static OBJ Read(Stream mesh, Stream mtl)
		{
			var res = new OBJ();

			List<Vector3D> v = new List<Vector3D>();
			List<Vector3D> vn = new List<Vector3D>();
			List<Vector3D> vt = new List<Vector3D>();

			using (var f = new StreamReader(mesh, Encoding.UTF8))
			{
				string line;
				while ((line = f.ReadLine()) != null)
				{
					if (line.Length < 1 || line.StartsWith("#")) continue;
					string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					if (parts.Length < 1) continue;

					Material usingMaterial = null;
					switch (parts[0])
					{
						case "v":
							v.Add(Vec(parts[1], parts[2], parts[3]));
							break;
						case "vn":
							vn.Add(Vec(parts[1], parts[2], parts[3]));
							break;
						case "vt":
							if (parts.Length == 3)
								vt.Add(Vec(parts[1], parts[2], "0"));
							else
								vt.Add(Vec(parts[1], parts[2], parts[3]));
							break;
						case "f":
							{
								if (parts.Length < 4) continue;
								Face fa = new Face();
								if (usingMaterial is null)
									usingMaterial = res.GetOrAddMaterial("defMat");
								fa.Mat = usingMaterial.Name;
								Vertex[] vertices = new Vertex[3];
								for (int i = 0; i < parts.Length - 1; i++)
								{
									String[] Parts = parts[i + 1].Split('/');
									if (Parts.Length > 1 && Parts[1] != "") vertices[i] = new Vertex(v[int.Parse(Parts[0]) - 1], vn[int.Parse(Parts[2]) - 1], vt[int.Parse(Parts[1]) - 1]);
									else if (Parts.Length > 2 && Parts[2] != "") vertices[i] = new Vertex(v[int.Parse(Parts[0]) - 1], vn[int.Parse(Parts[2]) - 1]);
									else vertices[i] = new Vertex(v[int.Parse(Parts[0]) - 1]);
								}
								fa.Vertices = vertices;
								res.Faces.Add(fa);
								break;
							}
						case "usemtl":
							{
								if (parts.Length < 2) continue;
								usingMaterial = res.GetOrAddMaterial(parts[1]);
								break;
							}
					}

				}
			}


			if (mtl != null)
			{
				using (var f = new StreamReader(mtl, Encoding.UTF8))
				{
					string line;
					while ((line = f.ReadLine()) != null)
					{
						if (line.Length < 1 || line.StartsWith("#")) continue;
						string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
						if (parts.Length < 1) continue;

						Material usingMaterial = null;
						switch (parts[0])
						{
							case "newmtl":
								usingMaterial = res.GetOrAddMaterial(parts[1]);
								break;
							case "Ka":
								if (usingMaterial == null) continue;
								usingMaterial.Colors.pos = new Vector3D(float.Parse(parts[1]),float.Parse(parts[2]),float.Parse(parts[3]));
								break;
							case "Kd":
								if (usingMaterial == null) continue;
								usingMaterial.Colors.normal = new Vector3D(float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
								break;
							case "Ks":
								if (usingMaterial == null) continue;
								usingMaterial.Colors.tex = new Vector3D(float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
								break;
							case "map_Kd":
								if (usingMaterial == null) continue;
								usingMaterial.TextureMapName = parts[1];
								break;
						}
					}
				}
			}

			return res;
		}

		public struct Vertex :IEquatable<Vertex>
		{
			public Vertex(float x, float y, float z, float nx , float ny, float nz , float? u = null, float? v = null, float? w = null)
			{
				pos = new Vector3D(x, y, z);
				normal = new Vector3D(nx, ny, nz);
				if (u == null) tex = null;
				else tex = new Vector3D(u.Value, v.Value, w.Value);
			}

			public Vertex(Vector3D v, Vector3D vn = new Vector3D(), Vector3D? vt = null)
			{
				pos = v;
				normal = vn;
				tex = vt;
			}

			public Vector3D pos;
			public Vector3D normal;
			public Vector3D? tex;

			public bool Equals(Vertex other)
			{
				return
					other.pos == pos &&
					other.normal == normal &&
					other.tex == tex;
			}

			public override bool Equals(object obj) => obj is Vertex ? Equals((Vertex)obj) : false;
			public static bool operator ==(Vertex c1, Vertex c2) =>  c1.Equals(c2);
			public static bool operator !=(Vertex c1, Vertex c2) => !c1.Equals(c2);
			public override int GetHashCode() => base.GetHashCode(); //Probably not going to work, but silences warnings
		}

		public struct Face : IEquatable<Face>
		{
			public string Mat;

			public Vertex VA;
			public Vertex VB;
			public Vertex VC;

			public Vertex[] Vertices
			{
				get => new Vertex[3] { VA, VB, VC };
				internal set
				{
					VA = value[0];
					VB = value[1];
					VC = value[2];
				}
			}

			public bool Equals(Face other) => VA == other.VA && VB == other.VB && VC == other.VB;// && Mat == other.Mat;
			public override bool Equals(object other) => other is Face ? this.Equals((Face)other) : false;
			public static bool operator ==(Face c1, Face c2) => c1.Equals(c2);
			public static bool operator !=(Face c1, Face c2) => !c1.Equals(c2);
			public override int GetHashCode() => base.GetHashCode();
		}

		public class Material : IEquatable<Material>
		{
			public string Name;
			public Vertex Colors; //XYZ are Ambient (KA) NXNYNZ are diffuse (KD) UVW are specular (KS)

			public string TextureMapName;
			public bool Equals(Material other)
			{
				return
					other.Name == Name &&
					other.TextureMapName == TextureMapName &&
					other.Colors == Colors;
			}

			public override bool Equals(object other) => other is Material ? this.Equals((Material)other) : false;
			public static bool operator ==(Material c1, Material c2) => c2 is null ? false : c1.Equals(c2);
			public static bool operator !=(Material c1, Material c2) => !c1.Equals(c2);
			public override int GetHashCode() => base.GetHashCode();
		}

		public bool HasMaterial(string name) => Materials.Any(x => x.Name == name);
		public Material GetMaterial(string name) => Materials.Where(x => x.Name == name).FirstOrDefault();

		public Material GetOrAddMaterial(string name)
		{
			var mat = GetMaterial(name);
			if (mat is null)
			{
				mat = new Material() { Name = name };
				Materials.Add(mat);
			}
			return mat;
		}

		public int GetOrAddMaterialIndex(string name)
		{
			for (int i = 0; i < Materials.Count; i++)
				if (Materials[i].Name == name) return i;
			Materials.Add(new Material() { Name = name });
			return Materials.Count - 1 ;
		}

		public List<Material> Materials = new List<Material>();
		public List<Face> Faces = new List<Face>();

		public WritableObj toWritableObj()
		{
			WritableObj res = new WritableObj();
			List<Vertex> Vertices = new List<Vertex>();
			List<WritableObj.WritableFace> WFaces = new List<WritableObj.WritableFace>();

			foreach (var f in Faces)
			{
				var face = new WritableObj.WritableFace();
				face.MaterialIndex = GetOrAddMaterialIndex(f.Mat);
				face.FaceIndex = Vertices.Count + 1; //Objs are 1-indexed
				Vertices.Add(f.VA);
				Vertices.Add(f.VB);
				Vertices.Add(f.VC);
				WFaces.Add(face);
			}

			res.Vertices = Vertices;
			res.Materials = new List<Material>(Materials);
			res.Faces = WFaces;

			return res;
		}

		public void Merge(OBJ other)
		{
			foreach (var mat in other.Materials)
				if (!HasMaterial(mat.Name))
					Materials.Add(mat);
			//TODO: Proper obj merging/exporting;
			Faces.AddRange(other.Faces);
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
		
		public void WriteObj(string FileName, string mtlLibName)
		{
			using (StreamWriter f = new System.IO.StreamWriter(FileName))
				WriteObj(f, Path.GetFileName(FileName + ".mtl"));

			if (mtlLibName != null)
			{
				using (StreamWriter f = new System.IO.StreamWriter(mtlLibName))
					WriteMtl(f);
			}
		}

		public void WriteObj(StreamWriter s, string mtlLibName)
		{
			if (mtlLibName != null)
				s.WriteLine($"mtllib {mtlLibName}");

			foreach (var v in Vertices)
			{
				s.WriteLine($"v {(float)v.pos.X} {(float)v.pos.Y} {(float)v.pos.Z}");
				s.WriteLine($"vn {(float)v.normal.X} {(float)v.normal.Y} {(float)v.normal.Z}");
				if (v.tex != null)
					s.WriteLine($"vt {(float)v.tex.Value.X} {(float)v.tex.Value.Y}{(v.tex.Value.Z != 0 ? ((float)v.tex.Value.Z).ToString() : "")}");
			}

			int curmat = -1;
			foreach (var f in Faces.OrderBy(x => x.MaterialIndex))
			{
				if (f.MaterialIndex != curmat)
				{
					curmat = f.MaterialIndex;
					s.WriteLine($"usemtl {Materials[curmat].Name}");
				}

				if (Vertices[f.FaceIndex].tex == null)
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
				s.WriteLine($"Ka {m.Colors.pos.X} {m.Colors.pos.Y} {m.Colors.pos.Z}");
				s.WriteLine($"Kd {m.Colors.normal.X} {m.Colors.normal.Y} {m.Colors.normal.Z}");
				if (m.Colors.tex != null) s.WriteLine($"Ks {m.Colors.tex.Value.X} {m.Colors.tex.Value.Y} {m.Colors.tex.Value.Z}");
				if (m.TextureMapName != null) s.WriteLine($"map_Kd {m.TextureMapName}");
			}
		}

		public OBJ ToObj()
		{
			var res = new OBJ();
			foreach (var f in Faces)
			{
				res.Faces.Add(new OBJ.Face()
				{
					Mat = Materials[f.MaterialIndex].Name,
					VA = Vertices[f.FaceIndex - 1],
					VB = Vertices[f.FaceIndex],
					VC = Vertices[f.FaceIndex + 1],
				});
			}
			return res;
		}
	}
}
