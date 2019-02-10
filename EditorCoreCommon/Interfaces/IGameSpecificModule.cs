using EditorCore.Drawing;
using EditorCoreCommon.Interfaces.GL;
using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace EditorCore.Interfaces
{
	public interface IEditorFormContext
	{
		ILevel LoadedLevel { get; }
		IObjList CurList { get; }
		ILevelObj SelectedObj { get; set; }
		ILevelObj[] SelectedObjs { get; }
		string GameFolder { get; set; }

		void RegisterClipBoardExt(ToolStripMenuItem item);
		void RegisterMenuStripExt(ToolStripMenuItem item);
		void RegisterMenuExtension(IMenuExtension ext);
		void LoadLevel(string path);
		void LoadLevel(ILevel lev);

		void SelectObject(IObjList List, ILevelObj obj);
		void EditPath(IPathObj path);
		void EditList(IObjList objlist);
		void EditList(IList<dynamic> objList);
		void AddToUndo(Action<dynamic> act, string desc, dynamic arg = null);
		void AddObj(ILevelObj o, IObjList list);
		void DeleteObj(ILevelObj o, IObjList list);

		void AddModel(ILevelObj obj, IObjList list);

		IEditorFormContext NewInstance(params string[] args);
	}

	public interface IGameModule
	{
		string ModuleName {get;}
		string ModelsFolder { get; }

		Tuple<Type, Type>[] GetClassConverters { get; }

		string[] ReservedPropNames { get; }
		string[] ModelFieldPropNames { get; }
		
		bool IsAddListSupported { get; }
		bool IsPropertyEditingSupported { get; }
		IEditorFormContext ViewForm { get; set; }

		string[] AutoHideList { get; }

		void InitModule(IEditorFormContext currentView);
		void FormLoaded(); //for startup checks
		void ParseArgs(string[] Args);
		RenderSceneBase GetRenderer();

		ILevel LoadLevel(string path = null); 
		ILevel NewLevel(string path = null);
		IObjList CreateObjList(string name, IList<dynamic> baseList);
		ILevelObj NewObject();

		void SaveLevel(ILevel level);
		void SaveLevelAs(ILevel level);

		bool OpenLevelFile(string name, Stream file); //if it fails the editor will try using OpenFileHandler
		string AddObjList(ILevel level);
		void EditChildrenNode(ILevelObj obj);

		Tuple<string, dynamic> GetNewProperty(dynamic target);
	}

	public interface IEditingOptionsModule
	{
		void InitOptionsMenu(ref ContextMenuStrip baseMenu);
		void OptionsMenuOpening(ILevelObj clickedObj);
	}

	public interface IActionButtonsModule
	{
		void InitActionButtons(ref ToolStrip baseButtonStrip);
	}
}
