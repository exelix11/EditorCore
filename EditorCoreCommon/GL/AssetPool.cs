using Assimp;
using GL_EditorFramework.GL_Core;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EditorCoreCommon.Drawing.ImportedModel;

namespace EditorCoreCommon.Drawing
{
	public class ImportedModel
	{
		public class GLMesh
		{
			public Matrix4 Transform;
			public uint VAO, VBO, EBO, TEX;
			public uint VertexCount, FaceCount;
		}

		public GLMesh[] Meshes;
	}

	public class AssetPool
	{
		static AssimpContext importer = new AssimpContext();
		static Dictionary<GL_ControlModern, AssetPool> Pools = new Dictionary<GL_ControlModern, AssetPool>();

		static AssetPool GetForContext(GL_ControlModern control)
		{
			AssetPool p;
			if (Pools.TryGetValue(control, out p))
				return p;
			p = new AssetPool();
			Pools.Add(control, p);
			return p;
		}

		static void FreeContext(GL_ControlModern control)
		{
			AssetPool p;
			if (!Pools.TryGetValue(control, out p))
				return;
			Pools.Remove(control);
			p.FreeAssets();
		}

		public ShaderProgram DefaultShaderProgram;
		public ShaderProgram PickingShaderProgram;

		public AssetPool()
		{
			var defaultFrag = new FragmentShader(
						@"#version 330
						uniform sampler2D tex;
						in vec4 fragColor;
						in vec2 uv;
				
						void main(){
							gl_FragColor = fragColor*texture(tex, uv);
						}");
			var solidColorFrag = new FragmentShader(
				@"#version 330
						uniform vec4 color;
						void main(){
							gl_FragColor = color;
						}");
			var defaultVert = new VertexShader(
				@"#version 330
						layout(location = 0) in vec4 position;
						uniform vec4 color;
						uniform mat4 mtxMdl;
						uniform mat4 mtxCam;
						out vec4 fragColor;
						out vec3 fragPosition;
						out vec2 uv;

						vec2 map(vec2 value, vec2 min1, vec2 max1, vec2 min2, vec2 max2) {
							return min2 + (value - min1) * (max2 - min2) / (max1 - min1);
						}

						void main(){
							fragPosition = position.xyz;
							uv = map(fragPosition.xz,vec2(-1.0625,-1.0625),vec2(1.0625,1.0625), vec2(0.5,0.5), vec2(0.75,1.0));
							gl_Position = mtxCam*mtxMdl*position;
							fragColor = color;
						}");
			var solidColorVert = new VertexShader(
				@"#version 330
						layout(location = 0) in vec4 position;
						uniform mat4 mtxMdl;
						uniform mat4 mtxCam;
						void main(){
							gl_Position = mtxCam*mtxMdl*position;
						}");
			DefaultShaderProgram = new ShaderProgram(defaultFrag, defaultVert);
			PickingShaderProgram = new ShaderProgram(solidColorFrag, solidColorVert);
		}

		//Dictionary<string, ImportedModel> LoadedMeshes = new Dictionary<string, ImportedModel>();
		//ImportedModel LoadMesh(string filename)
		//{
		//	ImportedModel m;
		//	if (LoadedMeshes.TryGetValue(filename, out m))
		//		return m;

		//	var	m_model = importer.ImportFile(filename, PostProcessPreset.TargetRealTimeMaximumQuality);
		//	List<GLMesh> meshes = new List<GLMesh>();

		//	void RecursiveLoadMesh(Node node)
		//	{
		//		if (node.)
		//	}

		//	RecursiveLoadMesh(m_model.RootNode);
		//}

		void FreeAssets()
		{

		}


	}
}
