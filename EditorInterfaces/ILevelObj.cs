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
		Dictionary<string, dynamic> Prop { get; set; }

		dynamic this[string name] { get; set; }
		Vector3D Pos { get; set; }
		Vector3D Rot { get; set; }
		Vector3D Scale { get; set; }
		string ID { get; set; }
		string ModelName { get; }
		string Name { get; set; }

		[TypeConverter(typeof(DictionaryConverter))]
		Dictionary<string, dynamic> Properties { get; set; }

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
