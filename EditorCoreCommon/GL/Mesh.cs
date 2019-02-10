using EditorCore.Interfaces;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorCore.Drawing
{
	class ObjMesh : IRenderable
	{
		public virtual Vector3 ModelView_Pos { get; set; } = new Vector3();
		public virtual Vector3 ModelView_Rot { get; set; } = new Vector3();
		public virtual Vector3 ModelView_Scale { get; set; } = new Vector3(1,1,1);
		public virtual bool Selected { get; set; } = false;

		public virtual void Draw(GL_ControlModern control, Pass pass)
		{
			throw new NotImplementedException();
		}

		public virtual void Prepare(GL_ControlModern control)
		{
			throw new NotImplementedException();
		}
	}
}
