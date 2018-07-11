using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Syroot.Maths;

namespace Syroot.NintenTools.MarioKart8.Common.Custom
{
	/// <summary>
	/// Represents a 3D model stored in the Wavefront OBJ format.
	/// </summary>
	public class ObjModel : ILoadableFile
	{
		// ---- CONSTANTS ----------------------------------------------------------------------------------------------

		private static readonly char[] _argSeparators = new char[] { ' ' };
		private static readonly char[] _vertexSeparators = new char[] { '/' };

		// ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance of the <see cref="ObjModel"/> class.
		/// </summary>
		public ObjModel()
		{
			Positions = new List<Vector3F>();
			TexCoords = new List<Vector2F>();
			Normals = new List<Vector3F>();
			Faces = new List<ObjFace>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ObjModel"/> class from the given stream.
		/// </summary>
		/// <param name="stream">The stream from which the instance will be loaded.</param>
		public ObjModel(Stream stream)
		{
			Load(stream);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ObjModel"/> class from the file with the given name.
		/// </summary>
		/// <param name="fileName">The name of the file from which the instance will be loaded.</param>
		public ObjModel(string fileName)
		{
			Load(fileName);
		}

		// ---- PROPERTIES ---------------------------------------------------------------------------------------------

		/// <summary>
		/// Gets or sets the list of positions of the model's vertices.
		/// </summary>
		public List<Vector3F> Positions { get; set; }

		/// <summary>
		/// Gets or sets the list of texture coordinates of the model's vertices.
		/// </summary>
		public List<Vector2F> TexCoords { get; set; }

		/// <summary>
		/// Gets or sets the list of normals of the model's vertices.
		/// </summary>
		public List<Vector3F> Normals { get; set; }

		/// <summary>
		/// Gets or sets the list of faces of the model.
		/// </summary>
		public List<ObjFace> Faces { get; set; }

		// ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

		/// <summary>
		/// Loads the data from the given <paramref name="stream"/>.
		/// </summary>
		/// <param name="stream">The <see cref="Stream"/> to load the data from.</param>
		/// <param name="leaveOpen"><c>true</c> to leave <paramref name="stream"/> open after loading the instance.
		/// </param>
		public void Load(Stream stream, bool leaveOpen = false)
		{
			using (StreamReader reader = new StreamReader(stream, Encoding.UTF8, true, 81920, leaveOpen))
			{
				Positions = new List<Vector3F>();
				TexCoords = new List<Vector2F>();
				Normals = new List<Vector3F>();
				Faces = new List<ObjFace>();

				while (!reader.EndOfStream)
				{
					string line = reader.ReadLine();

					// Ignore empty lines and comments.
					if (String.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;

					string[] args = line.Split(_argSeparators, StringSplitOptions.RemoveEmptyEntries);
					switch (args[0])
					{
						case "v":
							Positions.Add(new Vector3F(Single.Parse(args[1]), Single.Parse(args[2]),
								Single.Parse(args[3])));
							continue;
						case "vt":
							TexCoords.Add(new Vector2F(Single.Parse(args[1]), Single.Parse(args[2])));
							continue;
						case "vn":
							Normals.Add(new Vector3F(Single.Parse(args[1]), Single.Parse(args[2]),
								Single.Parse(args[3])));
							continue;
						case "f":
							// Only support triangles for now.
							ObjFace face = new ObjFace() { Vertices = new ObjVertex[3] };
							for (int i = 0; i < face.Vertices.Length; i++)
							{
								string[] vertexArgs = args[i + 1].Split(_vertexSeparators, StringSplitOptions.None);

								face.Vertices[i].PositionIndex = Int32.Parse(vertexArgs[0]) - 1;
								if (vertexArgs.Length > 1 && vertexArgs[1] != String.Empty)
								{
									face.Vertices[i].TexCoordIndex = Int32.Parse(vertexArgs[1]) - 1;
								}
								if (vertexArgs.Length > 2)
								{
									face.Vertices[i].NormalIndex = Int32.Parse(vertexArgs[2]) - 1;
								}
							}
							Faces.Add(face);
							continue;
					}
				}
			}
		}

		/// <summary>
		/// Loads the data from the given file.
		/// </summary>
		/// <param name="fileName">The name of the file to load the data from.</param>
		public void Load(string fileName)
		{
			using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				Load(stream);
			}
		}
	}

	/// <summary>
	/// Represents a triangle in an <see cref="ObjModel"/>.
	/// </summary>
	public struct ObjFace
	{
		/// <summary>
		/// The three <see cref="ObjVertex"/> vertices which define this triangle.
		/// </summary>
		public ObjVertex[] Vertices;
	}

	/// <summary>
	/// Represents the indices required to define a vertex of an <see cref="ObjModel"/>.
	/// </summary>
	public struct ObjVertex
	{
		// ---- FIELDS -------------------------------------------------------------------------------------------------

		/// <summary>
		/// The index of the spatial position in the positions array of the owning <see cref="ObjModel"/>.
		/// </summary>
		public int PositionIndex;

		/// <summary>
		/// The index of the texture coordinates in the texture coordinate array of the owning <see cref="ObjModel"/>.
		/// </summary>
		public int TexCoordIndex;

		/// <summary>
		/// The index of the normal in the normal array of the owning <see cref="ObjModel"/>.
		/// </summary>
		public int NormalIndex;
	}
}