using EditorCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SARCExt
{
	class SarcExt : ExtensionManifest
	{
		public string ModuleName => "Sarc Extension";
		public string Author => "Exelix11";
		public string ThanksTo => "Gericom for Every File Explorer";

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
			ToolsMenuExtensions = new ToolStripMenuItem[]
			{
				new ToolStripMenuItem(){ Text = "Yaz0 compression"}
			};
			ToolsMenuExtensions[0].DropDownItems.Add(new ToolStripMenuItem() { Text = "Compress" });
			ToolsMenuExtensions[0].DropDownItems.Add(new ToolStripMenuItem() { Text = "Deompress" });
			ToolsMenuExtensions[0].DropDownItems[0].Click += Compress;
			ToolsMenuExtensions[0].DropDownItems[1].Click += Decompress;
		}

		public ToolStripMenuItem[] FileMenuExtensions { get; internal set; }
		public ToolStripMenuItem[] ToolsMenuExtensions { get; internal set; }
		public ToolStripMenuItem[] TitleBarExtensions { get; internal set; }

		void Compress(object sender, EventArgs e)
		{
			OpenFileDialog openFile = new OpenFileDialog();
			openFile.Filter = "every file | *.*";
			if (openFile.ShowDialog() != DialogResult.OK) return;
			System.IO.File.WriteAllBytes( openFile.FileName + ".yaz0",
				EveryFileExplorer.YAZ0.Compress(openFile.FileName));
			GC.Collect();
		}

		void Decompress(object sender, EventArgs e)
		{
			OpenFileDialog openFile = new OpenFileDialog();
			openFile.Filter = "every file | *.*";
			if (openFile.ShowDialog() != DialogResult.OK) return;
			System.IO.File.WriteAllBytes(openFile.FileName + ".bin",
				EveryFileExplorer.YAZ0.Decompress(openFile.FileName));
			GC.Collect();
		}

	}
}
