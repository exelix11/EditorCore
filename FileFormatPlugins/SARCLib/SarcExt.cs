using EditorCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EveryFileExplorer;
using System.IO;

namespace SARCExt
{
	class SarcExt : ExtensionManifest
	{
		public string ModuleName => "Sarc Extension";
		public string Author => "Exelix11";
		public string ThanksTo => "Gericom for Every File Explorer";

		public Version TargetVersion => new Version(1, 0, 0, 0);
		
		public IMenuExtension MenuExt { get; } = new MenuExt();

		public IClipboardExtension ClipboardExt => null;

		public bool HasGameModule => false;
		public IGameModule GameModule => null;

		public IFileHander[] Handlers { get; } = new IFileHander[] { };

		public void CheckForUpdates()
		{
			return;
		}
	}

	class SzsHandler : IFileHander
	{
		public string HandlerName => "SZS file handler";

		public bool IsFormatSupported(string filename, Stream file)
		{
			if (filename.EndsWith(".szs") || filename.EndsWith(".sarc"))
			{
				byte[] header = new byte[4];
				file.Read(header, 0, 4);
				return (header[0] == 'Y' && header[1] == 'a' && header[1] == 'z' && header[1] == '0');
			}
			return false;
		}

		public void OpenFile(string filename, Stream file)
		{
			byte[] data;
			if (file is MemoryStream)
				data = ((MemoryStream)file).ToArray();
			else
			{
				return; //unsupported stream
			}
			new SarcEditor(SARC.UnpackRam(YAZ0.Decompress(data))).Show();
		}
	}

	class MenuExt : IMenuExtension
	{
		public MenuExt()
		{
			ToolsMenuExtensions = new ToolStripMenuItem[]
			{
				new ToolStripMenuItem(){ Text = "Yaz0 compression"},
				new ToolStripMenuItem(){ Text = "Szs editor"}
			};
			ToolsMenuExtensions[0].DropDownItems.Add(new ToolStripMenuItem() { Text = "Compress" });
			ToolsMenuExtensions[0].DropDownItems.Add(new ToolStripMenuItem() { Text = "Deompress" });
			ToolsMenuExtensions[0].DropDownItems[0].Click += Compress;
			ToolsMenuExtensions[0].DropDownItems[1].Click += Decompress;
			ToolsMenuExtensions[1].Click += SarcEditor;
		}

		public ToolStripMenuItem[] FileMenuExtensions { get; internal set; }
		public ToolStripMenuItem[] ToolsMenuExtensions { get; internal set; }
		public ToolStripMenuItem[] TitleBarExtensions { get; internal set; }

		void SarcEditor(object sender, EventArgs e)
		{
			OpenFileDialog opn = new OpenFileDialog() { Filter = "szs file|*.szs|every file|*.*" };
			if (opn.ShowDialog() != DialogResult.OK) return;
			new SarcEditor(SARC.UnpackRam(YAZ0.Decompress(opn.FileName))).Show();
		}

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
