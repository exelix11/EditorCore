using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EditorCore.Interfaces
{
	public interface ExtensionManifest
	{
		string ModuleName { get; }
		string Author { get; }
		string ExtraText { get; }

		bool HasGameModule { get; }
		IGameModule GetNewGameModule();

		//MenuExt and Handlers should be only instantiated once to save memory
		IMenuExtension MenuExt { get; }
		IFileHander[] Handlers { get; }

		void CheckForUpdates();
	}

	public interface IMenuExtension
	{
		ToolStripMenuItem[] FileMenuExtensions { get; }
		ToolStripMenuItem[] ToolsMenuExtensions { get; }
		ToolStripMenuItem[] TitleBarExtensions { get; }
	}

	//public interface IClipboardExtension
	//{
	//	ToolStripMenuItem[] CopyExtensions { get; }
	//	ToolStripMenuItem[] PasteExtensions { get; }
	//}

	public interface IFileHander
	{
		string HandlerName { get; }
		bool IsFormatSupported(string filename, Stream file);
		void OpenFile(string filename, Stream file);
	}

	public interface IEditableFileHandler : IFileHander
	{
		void OpenFileEdit(string filename, Stream file, Stream saveStream);
	}
}
