using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorCore.Interfaces
{
	public interface IObjList : IList<ILevelObj>
	{
		bool IsHidden { get; set; }
		string name { get; set; }

		void ApplyToNode();
	}

	public interface ILevel
	{
		Dictionary<string, byte[]> LevelFiles { get; set; }
		Dictionary<string, IObjList> objs { get; set; }
		dynamic LoadedLevelData { get; set; }
		string FilePath { get; set; }
		int HighestID { get; set; }

		byte[] Save(string newPath = null);

		bool HasList(string name);
		IObjList FindListByObj(ILevelObj o);
	}

	public struct SearchResult
	{
		public ILevelObj obj;
		public int Index;
		public string ListName;
	}
}
