using EditorCoreCommon.Interfaces.GL;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using OpenTK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EditorCore.PropertyGridTypes;

namespace EditorCore.Interfaces
{
	public struct Transform
	{
		public Vector3 Pos, Rot, Scale;
	}

	public interface ILevelObj : ICloneable , IRenderable
	{
		[Browsable(false)]
		bool ReadOnly { get; } //if this object is not actually part of the level, it can't be selected nor dragged

		[TypeConverter(typeof(DictionaryConverter))]
		Dictionary<string, dynamic> Prop { get; set; }

		dynamic this[string name] { get; set; }
		Vector3 Pos { get; set; }
		Vector3 Rot { get; set; }
		Vector3 Scale { get; set; }
		string ID { get; set; }
		string ModelName { get; }
		string Name { get; set; }
		
		[Browsable(false)]
		int ID_int { get; set; }

		[Browsable(false)]
		Transform transform { get; set; }
	}

	public interface IRenderable
	{
		Vector3 ModelView_Pos { get; set; }
		Vector3 ModelView_Rot { get; set; }
		Vector3 ModelView_Scale { get; set; }

		//Opengl stuff
		bool Selected { get; set; }

		void Prepare(GL_ControlModern control);
		void Draw(GL_ControlModern control, Pass pass);
	}
}
