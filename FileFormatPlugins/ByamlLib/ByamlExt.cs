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
		public string ExtraText => "Thanks to Syroot for his useful libs";
		
		public IMenuExtension MenuExt => new MenuExt();
		
		public bool HasGameModule => false;
		public IGameModule GetNewGameModule() => null;
		public IFileHander[] Handlers { get; } = new IFileHander[] { new BymlFileHandler(), new XmlFileHandler() };

		public void CheckForUpdates()
		{
			return;
		}
	}

	class XmlFileHandler : IFileHander
	{
		public string HandlerName => "BymlFileHandler";

		public bool IsFormatSupported(string filename, Stream file)
		{
			if (filename.EndsWith(".xml"))
			{
				StreamReader t = new StreamReader(file, Encoding.GetEncoding(932));
				string s = t.ReadLine();
				if (s != "<?xml version=\"1.0\" encoding=\"shift_jis\"?>") return false;
				s = t.ReadLine();
				if (s != "<Root>") return false;
				return true;
			}
			return false;
		}

		public void OpenFile(string filename, Stream file)
		{
			StreamReader t = new StreamReader(file, Encoding.GetEncoding(932));
			ByamlViewer.OpenByml(Byaml.XmlConverter.ToByml(t.ReadToEnd()), filename);
		}
	}

	class BymlFileHandler : IEditableFileHandler
	{
		public string HandlerName => "BymlFileHandler";

		public bool IsFormatSupported(string filename, Stream file)
		{
			byte[] header = new byte[2] { (byte)file.ReadByte(), (byte)file.ReadByte() };
			return (header[0] == 0x42 && header[1] == 0x59) || (header[1] == 0x42 && header[0] == 0x59);
		}

		public void OpenFile(string filename, Stream file) =>
			ByamlViewer.OpenByml(file,filename);
		
		public void OpenFileEdit(string filename, Stream file, Stream saveStream)=>
			ByamlViewer.OpenByml(file, filename, saveStream, true);
		
	}

	class MenuExt : IMenuExtension
	{
		public MenuExt()
		{
			ToolsMenuExtensions = new ToolStripMenuItem[]
			{
				new ToolStripMenuItem(){ Text = "Byaml tools"}
			};
			var editor = ToolsMenuExtensions[0].DropDownItems.Add("Edit byaml");
			editor.Click += BymlEditor;
			var xmltool = ToolsMenuExtensions[0].DropDownItems.Add("Import xml");
			xmltool.Click += XmlImport;
		}

		void XmlImport(object sender, EventArgs e)
		{
			OpenFileDialog openFile = new OpenFileDialog();
			openFile.Filter = "xml file |*.xml| every file | *.*";
			if (openFile.ShowDialog() != DialogResult.OK) return;
			StreamReader t = new StreamReader(new FileStream(openFile.FileName, FileMode.Open), UnicodeEncoding.Unicode);
			ByamlViewer.OpenByml(Byaml.XmlConverter.ToByml(t.ReadToEnd()), Path.GetFileName(openFile.FileName));
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
