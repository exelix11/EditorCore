using EditorCore.Interfaces;
using GL_EditorFramework;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using OpenGl_EditorFramework;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;

namespace EditorCore.Drawing
{
	public class CubeMeshObjForTesting : CubeMesh, ILevelObj
	{
		public object Clone()
		{
			throw new NotImplementedException();
		}
		public Vector3 Pos { get; set; } = new Vector3(0, 0, 0);
		public Vector3 Rot { get; set; } = new Vector3(0, 0, 0);
		public Vector3 Scale { get; set; } = new Vector3(1, 1, 1);

		public string ID { get; set; }

		public string ModelName => throw new NotImplementedException();

		public string Name { get; set; }
		public int ID_int { get; set; }

		public Dictionary<string, dynamic> Prop { get; set; }
		public Transform transform { get; set; }

		public dynamic this[string name] { get => null; set { } }
	}

	public class CubeMesh : IRenderable
	{
		protected readonly Vector4 hoverColor = new Vector4(1, 1, 0.925f, 1);
		protected readonly Vector4 selectColor = new Vector4(1, 1, 0.675f, 1);

		protected static int blockVao;
		protected static ShaderProgram defaultShaderProgram;
		protected static ShaderProgram solidColorShaderProgram;
		protected static int linesVao;

		private static float[][] points = new float[][]
		{
			new float[]{-1,-1, 1},
			new float[]{ 1,-1, 1},
			new float[]{-1, 1, 1},
			new float[]{ 1, 1, 1},
			new float[]{-1,-1,-1},
			new float[]{ 1,-1,-1},
			new float[]{-1, 1,-1},
			new float[]{ 1, 1,-1}
		};

		public Vector4 CubeColor = new Vector4(0, 0.25f, 1, 1);
		
		public bool Selected { get; set; } = false;

		public bool ReadOnly => false;

		public virtual Vector3 ModelView_Pos { get; set; } = new Vector3(0, 0, 0);
		public virtual Vector3 ModelView_Rot { get;set; } = new Vector3(0, 0, 0);
		public virtual Vector3 ModelView_Scale { get;set; } = new Vector3(1,1,1);
		
		public void Draw(GL_ControlModern control, Pass pass)
		{			
			control.CurrentShader = solidColorShaderProgram;
			Matrix4 mtx = Matrix4.CreateRotationX(ModelView_Rot.X);
			mtx *= Matrix4.CreateRotationY(ModelView_Rot.Y);
			mtx *= Matrix4.CreateRotationZ(ModelView_Rot.Z);
			mtx *= Matrix4.CreateTranslation(ModelView_Pos);
			mtx *= Matrix4.CreateScale(ModelView_Scale.X, ModelView_Scale.Y, ModelView_Scale.Z);
			control.UpdateModelMatrix(mtx);

			if (pass == Pass.OPAQUE)
			{
				#region outlines
				GL.LineWidth(2.0f);

				GL.Uniform4(solidColorShaderProgram["color"],Selected ? selectColor: CubeColor);
				GL.BindVertexArray(linesVao);
				GL.DrawArrays(PrimitiveType.Lines, 0, 24);
				#endregion
				control.CurrentShader = defaultShaderProgram;

				GL.Uniform1(defaultShaderProgram["tex"], Framework.TextureSheet - 1);

				GL.Uniform4(defaultShaderProgram["color"], CubeColor);
			}
			else
				GL.Uniform4(solidColorShaderProgram["color"], control.nextPickingColor());

			GL.BindVertexArray(blockVao);
			GL.DrawArrays(PrimitiveType.Quads, 0, 24);
		}

		static List<GL_ControlModern> InitialitedControls = new List<GL_ControlModern>();
		public void Prepare(GL_ControlModern control)
		{
			if (!InitialitedControls.Contains(control))
			{
				InitialitedControls.Add(control);
				var defaultFrag = new FragmentShader(
						@"#version 330
						uniform sampler2D tex;
						in vec4 fragColor;
						in vec3 fragPosition;
						in vec2 uv;
				
						void main(){
							gl_FragColor = fragColor*((fragPosition.y+2)/3)*texture(tex, uv);
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
				defaultShaderProgram = new ShaderProgram(defaultFrag, defaultVert);
				solidColorShaderProgram = new ShaderProgram(solidColorFrag, solidColorVert);


				int buffer;

				#region block
				GL.BindVertexArray(blockVao = GL.GenVertexArray());

				GL.BindBuffer(BufferTarget.ArrayBuffer, buffer = GL.GenBuffer());
				List<float> list = new List<float>();

				Renderers.face(ref points, ref list, 0, 1, 2, 3);
				Renderers.faceInv(ref points, ref list, 4, 5, 6, 7);
				Renderers.faceInv(ref points, ref list, 0, 1, 4, 5);
				Renderers.face(ref points, ref list, 2, 3, 6, 7);
				Renderers.face(ref points, ref list, 0, 2, 4, 6);
				Renderers.faceInv(ref points, ref list, 1, 3, 5, 7);

				float[] data = list.ToArray();
				GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * data.Length, data, BufferUsageHint.StaticDraw);

				GL.EnableVertexAttribArray(0);
				GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 3, 0);
				#endregion

				#region lines
				GL.BindVertexArray(linesVao = GL.GenVertexArray());

				GL.BindBuffer(BufferTarget.ArrayBuffer, buffer = GL.GenBuffer());
				list = new List<float>();

				Renderers.lineFace(ref points, ref list, 0, 1, 2, 3);
				Renderers.lineFace(ref points, ref list, 4, 5, 6, 7);
				Renderers.line(ref points, ref list, 0, 4);
				Renderers.line(ref points, ref list, 1, 5);
				Renderers.line(ref points, ref list, 2, 6);
				Renderers.line(ref points,ref list, 3, 7);

				data = list.ToArray();
				GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * data.Length, data, BufferUsageHint.StaticDraw);

				GL.EnableVertexAttribArray(0);
				GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 3, 0);
				#endregion

			}
		}
	}
}
