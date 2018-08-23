using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using static EditorCore.PropertyGridTypes;

namespace EditorCore.Interfaces
{
	public struct Transform
	{
		public Vector3D Pos, Rot, Scale;
	}

	public interface ILevelObj : ICloneable
	{
		[Browsable(false)]
		bool ReadOnly { get; set; } //if this object is not actually part of the level, it can't be selected nor dragged

		[TypeConverter(typeof(DictionaryConverter))]
		Dictionary<string, dynamic> Prop { get; set; }

		dynamic this[string name] { get; set; }
		Vector3D Pos { get; set; }
		Vector3D Rot { get; set; }
		Vector3D Scale { get; set; }
		string ID { get; set; }
		string ModelName { get; }
		string Name { get; set; }
		
		[Browsable(false)]
		int ID_int { get; set; }
		[Browsable(false)]
		Vector3D ModelView_Pos { get; set; }
		[Browsable(false)]
		Vector3D ModelView_Rot { get; }
		[Browsable(false)]
		Vector3D ModelView_Scale { get; }

		[Browsable(false)]
		Transform transform { get; set; }
	}
}
