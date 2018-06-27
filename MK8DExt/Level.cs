using EditorCore.Interfaces;
using Syroot.NintenTools.Byaml.Dynamic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MK8DExt
{
	class ObjList : List<ILevelObj>, IObjList
	{
		IList<dynamic> bymlNode;
		public ObjList(string _name, IList<dynamic> _bymlNode)
		{
			name = _name;
			if (_bymlNode == null)
			{
				bymlNode = new List<dynamic>();
				return;
			}
			bymlNode = _bymlNode;
			foreach (var o in bymlNode)
			{
				var obj = new LevelObj(o);
				int objID = obj.ID_int;
				if (Level._HighestID < objID) Level._HighestID = objID;
				this.Add(obj);
			}
		}

		public void ApplyToNode()
		{
			bymlNode.Clear();
			foreach (var o in this) bymlNode.Add(o.Prop);
		}

		public bool IsHidden { get; set; } = false;
		public string name { get; set; } = "";
	}

	class Level : ILevel
	{
		public Dictionary<string, byte[]> LevelFiles { get { return null; } set { return; } }
		public Dictionary<string, IObjList> objs { get; set; } = new Dictionary<string, IObjList>();
		public dynamic LoadedLevelData { get; set; }
		public string FilePath { get; set; }
		public int HighestID { get => _HighestID; set => _HighestID = value; }
		public static int _HighestID = 0;

		const string N_DEFLIST = "Default";
		const string N_OBJSNODE = "Obj";

		private IObjList defList = null;
		public IObjList FindListByObj(ILevelObj o) => defList;

		public Level(string path, int scenarioIndex = -1)
		{
			FilePath = path;
			LoadByml(ByamlFile.Load(path,true, Syroot.BinaryData.ByteOrder.LittleEndian));
		}

		void LoadByml(dynamic rootNode)
		{
			LoadedLevelData = rootNode;
			defList = new ObjList(N_DEFLIST, rootNode[N_OBJSNODE]);
			objs.Add(N_DEFLIST, defList);
		}

		public bool HasList(string name) => name == N_DEFLIST;

		public byte[] Save(string newPath = null)
		{
			if (newPath != null)
			{
				FilePath = newPath;
			}
			defList.ApplyToNode();
			MemoryStream mem = new MemoryStream();
			ByamlFile.Save(mem, LoadedLevelData, true, Syroot.BinaryData.ByteOrder.LittleEndian);
			return mem.ToArray();
		}
	}
}
