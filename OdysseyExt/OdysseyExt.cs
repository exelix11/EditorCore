using EditorCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OdysseyExt
{
	class OdysseyExt : ExtensionManifest
	{
		public string ModuleName => "OdysseyEditor";
		public string Author => "Exelix11";
		public string ThanksTo => "KillzXGaming for the C# BFRES loader\r\ngdkchan for Bn" +
								  "Txx\r\nGericom for Every File Explorer\r\nSyroot for his useful libs\r\nEveryone from " +
								  "masterf0x/RedCarpet";

		public Version TargetVersion => new Version(1, 0, 0, 0);

		MenuExt _menuExt = new MenuExt();
		public IMenuExtension MenuExt => _menuExt;

		public IClipboardExtension ClipboardExt => null;

		OdysseyModule _module = new OdysseyModule();
		public IGameSpecificModule GameModule => _module;
	}

	class MenuExt : IMenuExtension
	{
		public MenuExt()
		{
			FileMenuExtensions = new ToolStripMenuItem[]
			{
				new ToolStripMenuItem(){ Text = "Byaml editor"}
			};
			FileMenuExtensions[0].Click += BymlEditor;
		}

		void BymlEditor(object sender, EventArgs e)
		{
			OpenFileDialog openFile = new OpenFileDialog();
			openFile.Filter = "byaml file | *.byml *.byaml | every file | *.*";
			if (openFile.ShowDialog() != DialogResult.OK) return;
			EditorCore.ByamlViewer.OpenByml(openFile.FileName);
		}

		public ToolStripMenuItem[] FileMenuExtensions { get; internal set; }
		public ToolStripMenuItem[] ToolsMenuExtensions { get; internal set; }
		public ToolStripMenuItem[] TitleBarExtensions { get; internal set; }
	}
}
