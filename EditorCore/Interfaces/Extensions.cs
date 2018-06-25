using System;
using System.Collections.Generic;
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

		IMenuExtension MenuExt { get; }
		IClipboardExtension ClipboardExt { get; }
		IGameSpecificModule GameModule { get; }
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
}
