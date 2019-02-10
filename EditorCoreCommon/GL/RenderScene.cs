using EditorCore.Interfaces;
using EditorCoreCommon.Interfaces.GL;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using OpenTK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EditorCore.Drawing
{
	public class RenderScene : RenderSceneBase
	{
		//public delegate void RightClickHandler(MouseEventArgs e, I3DControl control);
		//public event RightClickHandler RightClick;

		public override void Clear()
		{
			IsDragging = false;
			DoSelectObj = false;
			base.Clear();
		}

		Vector3[] DragBasePos = null;
		bool IsDragging = false;
		public override uint MouseDown(MouseEventArgs e, I3DControl control)
		{
			if (IsDragging)
				return 0;
			if (e.Button == MouseButtons.Left && KDown(OpenTK.Input.Key.LControl))
			{
				if (Selected.Count == 0)
				{
					DoSelectObj = true;
					return REPICK | /*FORCE_REPICK |*/ NO_CAMERA_ACTION;
				}
				else
				{
					DragBasePos = new Vector3[Selected.Count];
					for (int i = 0; i < Selected.Count; i++)
						DragBasePos[i] = Selected[i].ModelView_Pos;
					IsDragging = true;
				}
			}
			return 0;
		}

		public override uint MouseUp(MouseEventArgs e, I3DControl control)
		{
			if (IsDragging && e.Button == MouseButtons.Left)
				StopDragging();
			return 0;
		}

		public override uint MouseMove(MouseEventArgs e, Point lastMousePos, I3DControl control)
		{			
			if (IsDragging)
			{
				if (e.Button != MouseButtons.Left)
					StopDragging();

				Vector3 Translate = new Vector3();

				//code from Whitehole

				float deltaX = e.X - control.DragStartPos.X;
				float deltaY = e.Y - control.DragStartPos.Y;

				float Depth = control.PickingDepth;
				if (Depth >= control.ZFar)
					return REPICK | NO_CAMERA_ACTION;

				deltaX *= Depth * control.FactorX;
				deltaY *= Depth * control.FactorY;
				Console.WriteLine(Depth);
				
				Translate += Vector3.UnitX * deltaX * (float)Math.Cos(control.CamRotX);
				Translate -= Vector3.UnitX * deltaY * (float)Math.Sin(control.CamRotX) * (float)Math.Sin(control.CamRotY);
				Translate -= Vector3.UnitY * deltaY * (float)Math.Cos(control.CamRotY);
				Translate += Vector3.UnitZ * deltaX * (float)Math.Sin(control.CamRotX);
				Translate += Vector3.UnitZ * deltaY * (float)Math.Cos(control.CamRotX) * (float)Math.Sin(control.CamRotY);

				for (int i = 0; i < Selected.Count; i++)
					Selected[i].ModelView_Pos = DragBasePos[i] + Translate;

				return REDRAW | NO_CAMERA_ACTION;
			}
			return 0;
		}

		bool DoSelectObj = false;
		public override uint MouseClick(MouseEventArgs e, I3DControl control)
		{
			if (e.Button == MouseButtons.Left)
			{
				DoSelectObj = true;
				return REPICK;//| FORCE_REPICK;
			}
			return 0;
		}

		static bool KDown(OpenTK.Input.Key k) => OpenTK.Input.Keyboard.GetState().IsKeyDown(k);
		
		//Called only on repick because MouseMove returns 0
		public override uint MouseEnter(int index, I3DControl control)
		{
			if (!DoSelectObj || IsDragging) return 0;
			DoSelectObj = false;
			bool multiSelect = KDown(OpenTK.Input.Key.ShiftLeft);
			if (Selected.Count != 0 && !multiSelect)
				ClearSelection(true);
			if (index >= 0)
			{
				if (!InbokeClickSelection(glDrawables[index], multiSelect))
					return 0;
				ToggleSelected(index);
			}
			else ClearSelection();
			return REDRAW | NO_CAMERA_ACTION;
		}

		void StopDragging()
		{
			IsDragging = false;
			InvokeObjectMoved();
		}
	}
}
