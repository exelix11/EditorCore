using EditorCore.Interfaces;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp.Configs;
using Assimp;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace EditorCore.Drawing
{
	public class SimpleMeshForTesting : ObjMesh, ILevelObj
	{
		public object Clone()
		{
			throw new NotImplementedException();
		}
		public Vector3 Pos { get; set; } = new Vector3(0, 0, 0);
		public Vector3 Rot { get; set; } = new Vector3(0, 0, 0);
		public Vector3 Scale { get; set; } = new Vector3(1, 1, 1);
		public bool ReadOnly => false;

		public SimpleMeshForTesting(string mesh) : base(mesh) { }

		public string ID { get; set; }

		public string ModelName => throw new NotImplementedException();

		public string Name { get; set; }
		public int ID_int { get; set; }

		public Dictionary<string, dynamic> Prop { get; set; }
		public Transform transform { get; set; }

		public dynamic this[string name] { get => null; set { } }
	}

	public class ObjMesh : IRenderable
	{
		static AssimpContext importer = new AssimpContext();

		public virtual Vector3 ModelView_Pos { get; set; } = new Vector3();
		public virtual Vector3 ModelView_Rot { get; set; } = new Vector3();
		public virtual Vector3 ModelView_Scale { get; set; } = new Vector3(1,1,1);
		public virtual bool Selected { get; set; } = false;

		Scene m_model;
		public Vector3 mesh_bounding_min, mesh_bounding_max, mesh_center;
		Dictionary<string, int> tex_ids = new Dictionary<string, int>();
		string MeshBasePath = ""; //Used to find textures

		public ObjMesh(string filename)
		{
			MeshBasePath = new FileInfo(filename).Directory.FullName;
			m_model = importer.ImportFile(filename, PostProcessPreset.TargetRealTimeMaximumQuality);
			ComputeBoundingBox();
		}

		~ObjMesh()
		{
			foreach (var tex in tex_ids)
				GL.DeleteTexture(tex.Value);
			tex_ids.Clear();
		}

		private void ComputeBoundingBox()
		{
			mesh_bounding_min = new Vector3(1e10f, 1e10f, 1e10f);
			mesh_bounding_max = new Vector3(-1e10f, -1e10f, -1e10f);
			Matrix4 identity = Matrix4.Identity;

			ComputeBoundingBox(m_model.RootNode, ref mesh_bounding_min, ref mesh_bounding_max, ref identity);

			mesh_center.X = (mesh_bounding_min.X + mesh_bounding_max.X) / 2.0f;
			mesh_center.Y = (mesh_bounding_min.Y + mesh_bounding_max.Y) / 2.0f;
			mesh_center.Z = (mesh_bounding_min.Z + mesh_bounding_max.Z) / 2.0f;
		}

		private void ComputeBoundingBox(Node node, ref Vector3 min, ref Vector3 max, ref Matrix4 trafo)
		{
			Matrix4 prev = trafo;
			trafo = Matrix4.Mult(prev, FromMatrix(node.Transform));

			if (node.HasMeshes)
			{
				foreach (int index in node.MeshIndices)
				{
					Mesh mesh = m_model.Meshes[index];
					for (int i = 0; i < mesh.VertexCount; i++)
					{
						Vector3 tmp = FromVector(mesh.Vertices[i]);
						Vector3.TransformVector(ref tmp, ref trafo, out tmp);

						min.X = Math.Min(min.X, tmp.X);
						min.Y = Math.Min(min.Y, tmp.Y);
						min.Z = Math.Min(min.Z, tmp.Z);

						max.X = Math.Max(max.X, tmp.X);
						max.Y = Math.Max(max.Y, tmp.Y);
						max.Z = Math.Max(max.Z, tmp.Z);
					}
				}
			}

			for (int i = 0; i < node.ChildCount; i++)
			{
				ComputeBoundingBox(node.Children[i], ref min, ref max, ref trafo);
			}
			trafo = prev;
		}

		public virtual void Draw(GL_ControlModern control, Pass pass)
		{			
			Matrix4 mtx = Matrix4.CreateRotationX(ModelView_Rot.X);
			mtx *= Matrix4.CreateRotationY(ModelView_Rot.Y);
			mtx *= Matrix4.CreateRotationZ(ModelView_Rot.Z);
			mtx *= Matrix4.CreateTranslation(ModelView_Pos);
			mtx *= Matrix4.CreateScale(ModelView_Scale.X, ModelView_Scale.Y, ModelView_Scale.Z);
			control.UpdateModelMatrix(mtx);

			if (pass == Pass.OPAQUE)
				RecursiveRender(m_model, m_model.RootNode, null);
			else
				RecursiveRender(m_model, m_model.RootNode, control.nextPickingColor());
		}

		public virtual void Prepare(GL_ControlModern control)
		{

		}

		private void RecursiveRender(Scene scene, Node node, Color4? Picking)
		{
			//Matrix4 m = FromMatrix(node.Transform);
			//m.Transpose();
			//GL.PushMatrix();
			//GL.MultMatrix(ref m);

			if (node.HasMeshes)
			{
				foreach (int index in node.MeshIndices)
				{
					Mesh mesh = scene.Meshes[index];
					ApplyMaterial(scene.Materials[mesh.MaterialIndex], Picking);

					if (mesh.HasNormals)
					{
						GL.Enable(EnableCap.Lighting);
					}
					else
					{
						GL.Disable(EnableCap.Lighting);
					}

					bool hasColors = mesh.HasVertexColors(0);
					if (hasColors)
					{
						GL.Enable(EnableCap.ColorMaterial);
					}
					else
					{
						GL.Disable(EnableCap.ColorMaterial);
					}

					bool hasTexCoords = mesh.HasTextureCoords(0);

					foreach (Face face in mesh.Faces)
					{
						OpenTK.Graphics.OpenGL.PrimitiveType faceMode;
						switch (face.IndexCount)
						{
							case 1:
								faceMode = OpenTK.Graphics.OpenGL.PrimitiveType.Points;
								break;
							case 2:
								faceMode = OpenTK.Graphics.OpenGL.PrimitiveType.Lines;
								break;
							case 3:
								faceMode = OpenTK.Graphics.OpenGL.PrimitiveType.Triangles;
								break;
							default:
								faceMode = OpenTK.Graphics.OpenGL.PrimitiveType.Polygon;
								break;
						}

						GL.Begin(faceMode);
						for (int i = 0; i < face.IndexCount; i++)
						{
							int indice = face.Indices[i];
							if (hasColors)
							{
								Color4 vertColor = FromColor(mesh.VertexColorChannels[0][indice]);
							}
							if (mesh.HasNormals)
							{
								Vector3 normal = FromVector(mesh.Normals[indice]);
								GL.Normal3(normal);
							}
							if (hasTexCoords)
							{
								Vector3 uvw = FromVector(mesh.TextureCoordinateChannels[0][indice]);
								GL.TexCoord2(uvw.X, 1 - uvw.Y);
							}
							Vector3 pos = FromVector(mesh.Vertices[indice]);
							GL.Vertex3(pos);
						}
						GL.End();
					}
				}
			}

			for (int i = 0; i < node.ChildCount; i++)
			{
				RecursiveRender(m_model, node.Children[i], Picking);
			}
		}

		private void LoadTexture(string texName)
		{
			int m_texId = 0;
			if (!tex_ids.TryGetValue(texName, out m_texId))
			{
				string fileName = Path.Combine(MeshBasePath, texName);
				if (!File.Exists(fileName))
				{
					return;
				}
				Bitmap textureBitmap = new Bitmap(fileName);
				BitmapData TextureData =
						textureBitmap.LockBits(
						new System.Drawing.Rectangle(0, 0, textureBitmap.Width, textureBitmap.Height),
						System.Drawing.Imaging.ImageLockMode.ReadOnly,
						System.Drawing.Imaging.PixelFormat.Format24bppRgb
					);
				m_texId = GL.GenTexture();
				tex_ids.Add(texName, m_texId);
				GL.BindTexture(TextureTarget.Texture2D, m_texId);
				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, textureBitmap.Width, textureBitmap.Height, 0,
				OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, TextureData.Scan0);
				textureBitmap.UnlockBits(TextureData);
			}
			else GL.BindTexture(TextureTarget.Texture2D, m_texId);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
		}

		private void ApplyMaterial(Material mat, Color4? Picking)
		{
			if (Picking.HasValue)
			{
				//GL.BindTexture(TextureTarget.Texture2D, 0);
				GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, Picking.Value);
				GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, new Color4(0, 0, 0, 1.0f));
				GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, new Color4(0, 0, 0, 1.0f));
				GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, new Color4(0, 0, 0, 1.0f));
				GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, 0);
				return;
			}

			if (mat.GetMaterialTextureCount(TextureType.Diffuse) > 0)
			{
				TextureSlot tex;
				if (mat.GetMaterialTexture(TextureType.Diffuse, 0, out tex))
					LoadTexture(tex.FilePath);
			}

			Color4 color = new Color4(.8f, .8f, .8f, 1.0f);
			if (mat.HasColorDiffuse)
			{
				color = FromColor(mat.ColorDiffuse);
			}
			GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, color);

			color = new Color4(0, 0, 0, 1.0f);
			if (mat.HasColorSpecular)
			{
				color = FromColor(mat.ColorSpecular);
			}
			GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, color);

			color = new Color4(.2f, .2f, .2f, 1.0f);
			if (mat.HasColorAmbient)
			{
				color = FromColor(mat.ColorAmbient);
			}
			GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, color);

			color = new Color4(0, 0, 0, 1.0f);
			if (mat.HasColorEmissive)
			{
				color = FromColor(mat.ColorEmissive);
			}
			GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, color);

			float shininess = 1;
			float strength = 1;
			if (mat.HasShininess)
			{
				shininess = mat.Shininess;
			}
			if (mat.HasShininessStrength)
			{
				strength = mat.ShininessStrength;
			}

			GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, shininess * strength);
		}

		//Conversion from assimp types to openTK
		private static Matrix4 FromMatrix(Matrix4x4 mat)
		{
			Matrix4 m = new Matrix4();
			m.M11 = mat.A1;
			m.M12 = mat.A2;
			m.M13 = mat.A3;
			m.M14 = mat.A4;
			m.M21 = mat.B1;
			m.M22 = mat.B2;
			m.M23 = mat.B3;
			m.M24 = mat.B4;
			m.M31 = mat.C1;
			m.M32 = mat.C2;
			m.M33 = mat.C3;
			m.M34 = mat.C4;
			m.M41 = mat.D1;
			m.M42 = mat.D2;
			m.M43 = mat.D3;
			m.M44 = mat.D4;
			return m;
		}

		private static Vector3 FromVector(Vector3D vec)
		{
			Vector3 v;
			v.X = vec.X;
			v.Y = vec.Y;
			v.Z = vec.Z;
			return v;
		}

		private static Color4 FromColor(Color4D color)
		{
			Color4 c;
			c.R = color.R;
			c.G = color.G;
			c.B = color.B;
			c.A = color.A;
			return c;
		}
	}
}
