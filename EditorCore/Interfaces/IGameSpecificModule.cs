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
		string ModelsFolder { get; }

		Tuple<Type, Type>[] GetClassConverters { get; }

		string[] ReservedPropNames { get; }
		string[] ModelFieldPropNames { get; }
		
		bool IsAddListSupported { get; }
		bool IsPropertyEditingSupported { get; }
		EditorForm ViewForm { get; set; }

		string[] AutoHideList { get; }

		void InitModule(EditorForm currentView);
		void FormLoaded(); //for startup checks
		void ParseArgs(string[] Args);

		ILevel LoadLevel(string path = null); 
		ILevel NewLevel(string path = null);
		IObjList CreateObjList(string name, IList<dynamic> baseList);
		ILevelObj NewObject();

		void SaveLevel(ILevel level);
		void SaveLevelAs(ILevel level);

		bool ConvertModelFile(string ObjName, string path);
		string GetPlaceholderModel(string ObjName, string ListName);

		bool OpenLevelFile(string name, Stream file); //if it fails the editor will try using OpenFileHandler
		string AddObjList(ILevel level);
		void EditChildrenNode(ILevelObj obj);
	}
}
