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
		string ThanksTo { get; }
		Version TargetVersion { get; }

		bool HasGameModule { get; } 
		IMenuExtension MenuExt { get; }
		IGameModule GetNewGameModule();
		IFileHander[] Handlers { get; }

		void CheckForUpdates();
	}

	public interface IMenuExtension
	{
		ToolStripMenuItem[] FileMenuExtensions { get; }
		ToolStripMenuItem[] ToolsMenuExtensions { get; }
		ToolStripMenuItem[] TitleBarExtensions { get; }
	}

	public interface IClipboardExtension
	{
		ToolStripMenuItem[] CopyExtensions { get; }
		ToolStripMenuItem[] PasteExtensions { get; }
	}

	public interface IFileHander
	{
		string HandlerName { get; }
		bool IsFormatSupported(string filename, Stream file);
		void OpenFile(string filename, Stream file);
	}
}
