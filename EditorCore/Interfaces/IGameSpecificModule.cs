using ModelViewer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorCore.Interfaces
{
	public delegate void HandleHotKeyExHandler(System.Windows.Input.KeyEventArgs e);
	public interface IGameModule
	{
		string ModuleName {get;}

		Tuple<Type, Type>[] GetClassConverters { get; }

		string[] ReservedPropNames { get; }
		string[] ModelFieldPropNames { get; }
		
		bool IsAddListSupported { get; }
		bool IsPropertyEditingSupported { get; }
		string LevelFormatFilter { get; }
		EditorForm ViewForm { get; set; }

		string[] AutoHideList { get; }

		void InitModule(EditorForm currentView);
		void ParseArgs(string[] Args);
		ILevel LoadLevel(string file);
		ILevel NewLevel(string file);
		bool GetModelFile(string ObjName, string path);
		IObjList CreateObjList(string name, IList<dynamic> baseList);
		ILevelObj NewObject();
		void OpenLevelFile(string name, Stream file);
		string AddObjList(ILevel level);
		void EditChildrenNode(ILevelObj obj);
		void FormLoaded(); //for startup checks
	}
}
