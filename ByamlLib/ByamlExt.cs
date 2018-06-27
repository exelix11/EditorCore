using EditorCore.Interfaces;
using System;
using System.Collections.Generic;
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
		public IGameModule GameModule => null;
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
			openFile.Filter = "byaml file |*.byml;*.byaml| every file | *.*";
			if (openFile.ShowDialog() != DialogResult.OK) return;
			ByamlViewer.OpenByml(openFile.FileName);
		}

		public ToolStripMenuItem[] FileMenuExtensions { get; internal set; }
		public ToolStripMenuItem[] ToolsMenuExtensions { get; internal set; }
		public ToolStripMenuItem[] TitleBarExtensions { get; internal set; }
	}
}
