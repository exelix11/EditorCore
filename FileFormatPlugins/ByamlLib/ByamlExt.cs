using EditorCore.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ByamlExt
{
	class ByamlExt : ExtensionManifest
	{
		public string ModuleName => "ByamlExt";
		public string Author => "Exelix11";
		public string ThanksTo => "Syroot";

		public Version TargetVersion => new Version(1, 0, 0, 0);

		MenuExt _menuExt = new MenuExt();
		public IMenuExtension MenuExt => _menuExt;

		public IClipboardExtension ClipboardExt => null;

		public bool HasGameModule => false;
		public IGameModule GetNewGameModule() => null;
		public IFileHander[] Handlers { get; } = new IFileHander[] { new BymlFileHandler() };

		public void CheckForUpdates()
		{
			return;
		}
	}

	class BymlFileHandler : IFileHander
	{
		public string HandlerName => "BymlFileHandler";

		public bool IsFormatSupported(string filename, Stream file)
		{
			if (filename.EndsWith(".byml") || filename.EndsWith(".byaml"))
			{
				byte[] header = new byte[2] { (byte)file.ReadByte(), (byte)file.ReadByte() };
				return (header[0] == 0x42 && header[1] == 0x59) || (header[1] == 0x42 && header[0] == 0x59);
			}
			return false;
		}

		public void OpenFile(string filename, Stream file)
		{
			ByamlViewer.OpenByml(file,filename);
		}
	}

	class MenuExt : IMenuExtension
	{
		public MenuExt()
		{
			ToolsMenuExtensions = new ToolStripMenuItem[]
			{
				new ToolStripMenuItem(){ Text = "Byaml editor"}
			};
			ToolsMenuExtensions[0].Click += BymlEditor;
		}

		void BymlEditor(object sender, EventArgs e)
		{
			OpenFileDialog openFile = new OpenFileDialog();
			openFile.Filter = "byaml file |*.byml;*.byaml| every file | *.*";
			if (openFile.ShowDialog() != DialogResult.OK) return;
			ByamlViewer.OpenByml(openFile.FileName);
		}

		public ToolStripMenuItem[] FileMenuExtensions { get; internal set; }
		public ToolStripMenuItem[] ToolsMenuExtensions { get; internal set; }
		public ToolStripMenuItem[] TitleBarExtensions { get; internal set; }
	}	
}
