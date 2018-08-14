using EditorCore.Interfaces;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EditorCore.OtherForms
{
	public partial class EditorForm2 : Form, IEditorFormContext
	{
		Point lastMouseLoc;
		Matrix4 projectionMtx;
		int defaultShader;
		private Matrix4 v;
		private float rot = 0;
		private float lookup = (float)Math.PI;
		private float zoom = -20f;
		private int uTransformMtxLoc;

		private void InitializeGL()
		{
			projectionMtx = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI*0.2f, 1.5f, 0.1f, 1000f);

			
			#region shaders
			int default_frag = GL.CreateShader(ShaderType.FragmentShader);
			GL.ShaderSource(default_frag,
			  @"#version 330
				in vec4 fragPosition;
				void main(){
					gl_FragColor = vec4(min( max(0.0,fragPosition.x),1.0 ) , 0.0, 0.0, 1.0);
				}"
			);
			GL.CompileShader(default_frag);

			int default_vert = GL.CreateShader(ShaderType.VertexShader);
			GL.ShaderSource(default_vert,
			  @"#version 330
				uniform mat4 uTransformMtx;
				in vec4 vPosition;
				out vec4 fragPosition;
				void main(){
					gl_Position = vPosition*uTransformMtx;
					fragPosition = gl_Position;
				}"
			);
			GL.CompileShader(default_vert);

			defaultShader = GL.CreateProgram();
			GL.AttachShader(defaultShader, default_vert);
			GL.AttachShader(defaultShader, default_frag);
			GL.LinkProgram(defaultShader);
			GL.GetShaderInfoLog(default_frag);
			GL.GetShaderInfoLog(default_vert);
			#endregion

			#region uniforms
			uTransformMtxLoc = GL.GetUniformLocation(defaultShader, "uTransformMtx");
			#endregion
		}

		private void viewGL_Paint(object sender, PaintEventArgs e)
		{
			viewGL.MakeCurrent();
			
			GL.ClearColor(0.125f, 0.125f, 0.125f, 1.0f);

			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			v = Matrix4.CreateRotationX(0.2f * lookup) *
				Matrix4.CreateRotationY(0.5f * rot) *
				Matrix4.CreateTranslation(0, 0, zoom) *
				Matrix4.CreatePerspectiveFieldOfView(1.2f, viewGL.Width / (float)viewGL.Height, 0.01f, 1000f);
				//Matrix4.CreatePerspectiveOffCenter(Left, Right, Bottom, Top, 0.01f, 10000f);

			GL.UseProgram(defaultShader);

			GL.UniformMatrix4(0, false, ref v);


			GL.MatrixMode(MatrixMode.Modelview);
			foreach (ILevelObj obj in ObjectsListBox.Items)
			{
				float x = (float)obj.Pos.X * 0.01f;
				float y = (float)obj.Pos.Y * 0.01f;
				float z = (float)obj.Pos.Z * 0.01f;
				GL.Begin(BeginMode.Triangles);
				GL.Vertex3(-0.5f+x, -0.5f + y, 0f+z);
				GL.Vertex3(0.5f + x, -0.5f + y, 0f+z);
				GL.Vertex3(0f+x, 0.5f + y, 0f+z);
				GL.End();
			}

			viewGL.SwapBuffers();
		}

		private void viewGL_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			lastMouseLoc = e.Location;
		}

		private void viewGL_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				rot += (e.Location.X - lastMouseLoc.X)*0.1f;
				lookup += (e.Location.Y - lastMouseLoc.Y) * 0.1f;

				Refresh();
			}

			lastMouseLoc = e.Location;
		}

		private void viewGL_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{

		}
	}
}
