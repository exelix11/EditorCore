using EditorCore.Interfaces;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using OpenTK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorCore.Drawing
{
	public abstract class RenderSceneBase : AbstractGlDrawable, IList<ILevelObj>
	{
		public delegate void ObjectMovedHandler(IReadOnlyList<ILevelObj> Selected);
		public delegate void ClickSelectionHandler(ILevelObj item, bool MultiSelect, ref bool Cancel);
		public event ClickSelectionHandler ClickSelection;
		public event ObjectMovedHandler ObjectMoved;

		protected List<ILevelObj> glDrawables = new List<ILevelObj>();
		protected List<ILevelObj> Selected = new List<ILevelObj>();

		public ILevelObj this[int index] { get => glDrawables[index]; set => glDrawables[index] = value; }
		public int Count => glDrawables.Count;
		public bool IsReadOnly => false;

		protected GL_ControlModern GlControl;

		public bool Contains(ILevelObj item) => glDrawables.Contains(item);
		public void CopyTo(ILevelObj[] array, int arrayIndex) => glDrawables.CopyTo(array, arrayIndex);
		public IEnumerator<ILevelObj> GetEnumerator() => glDrawables.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => glDrawables.GetEnumerator();
		public int IndexOf(ILevelObj item) => glDrawables.IndexOf(item);

		public virtual bool Remove(ILevelObj item)
		{
			RemoveFromSelection(item);
			return glDrawables.Remove(item);
		}

		public virtual void RemoveAt(int index)
		{
			RemoveObjIndexFromSelection(index);
			glDrawables.RemoveAt(index);
		}

		public virtual void Add(ILevelObj item)
		{
			item.Prepare(GlControl);
			glDrawables.Add(item);
		}

		public virtual void Clear()
		{
			glDrawables.Clear();
			Selected.Clear();
		}

		public virtual void Insert(int index, ILevelObj item)
		{
			item.Prepare(GlControl);
			glDrawables.Insert(index, item);
		}

		public override void Draw(GL_ControlModern control)
		{
			for (int i = 0; i < glDrawables.Count; i++)
				glDrawables[i].Draw(control);
		}

		public override void DrawPicking(GL_ControlModern control)
		{
			for (int i = 0; i < glDrawables.Count; i++)
				glDrawables[i].DrawPicking(control);
		}

		public override void Prepare(GL_ControlModern control)
		{
			GlControl = control;
			this.Add(new CubeMeshObjForTesting());
			this.Add(new CubeMeshObjForTesting() { ModelView_Pos = new Vector3(1, 1, 1), ModelView_Scale = new Vector3(1, 1, 1) });
		}

		public virtual void ClearSelection(bool NoEvent = false)
		{
			foreach (var o in Selected) o.Selected = false;
			Selected.Clear();
			if (!NoEvent)
			{
				bool Cancel = false;
				ClickSelection?.Invoke(null, true, ref Cancel);
			}
		}

		public virtual void ToggleSelected(int index)
		{
			var item = glDrawables[index];
			if (item.Selected)
				Selected.Remove(item);
			else
				Selected.Add(item);
			item.Selected = !item.Selected;
		}

		void RemoveObjIndexFromSelection(int index) => RemoveFromSelection(glDrawables[index]);
		public virtual void RemoveFromSelection(ILevelObj item)
		{
			if (!item.Selected) return;
			item.Selected = false;
			Selected.Remove(item);
		}

		void AddToSelection(int item) => AddToSelection(glDrawables[item]);
		public virtual void AddToSelection(ILevelObj item)
		{
			if (item.Selected) return;
			item.Selected = true;
			Selected.Add(item);
		}
		
		public virtual void SetSelected(List<ILevelObj> item)
		{
			ClearSelection(true);
			foreach (var i in item)
				AddToSelection(i);
		}

		protected void InvokeObjectMoved() => ObjectMoved?.Invoke(Selected);
		protected bool InbokeClickSelection(ILevelObj item, bool MultiSelect)
		{
			bool Cancel = false;
			ClickSelection?.Invoke(item, MultiSelect, ref Cancel);
			return Cancel;
		}

		public virtual void Focus() => GlControl.Focus();

		public virtual void LookAt(Vector3 position) => GlControl.CameraTarget = position;
		public virtual Vector3 GetPositionInView() => GlControl.CameraTarget;

		public override void Prepare(GL_ControlLegacy control)
		{
			throw new Exception("Legacy control unsupported");
		}

		public override void Draw(GL_ControlLegacy control)
		{
			throw new Exception("Legacy control unsupported");
		}
	}
}
