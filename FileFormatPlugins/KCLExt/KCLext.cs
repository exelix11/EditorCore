using EditorCore.Common;
using EditorCore.Interfaces;
using MarioKart.MK7;
using Syroot.NintenTools.MarioKart8.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KCLExt
{
	public class KCLExt : ExtensionManifest
	{
		public string ModuleName => "KCL extension";

		public string Author => "Exelix11";

		public string ExtraText => null;
		
		public bool HasGameModule => false;

		public IMenuExtension MenuExt { get; } = new menuExt();

		public IGameModule GetNewGameModule() => null;

		public IFileHander[] Handlers => null;

		public void CheckForUpdates() { }
	}

	class menuExt : IMenuExtension
	{
		public ToolStripMenuItem[] FileMenuExtensions => null;
		public ToolStripMenuItem[] ToolsMenuExtensions => toolsExt;
		public ToolStripMenuItem[] TitleBarExtensions => null;

		ToolStripMenuItem[] toolsExt = new ToolStripMenuItem[1];
		public menuExt()
		{
			toolsExt[0] = new ToolStripMenuItem("KCL to OBJ");
			toolsExt[0].Click += KCLToObj;
			//toolsExt[1] = new ToolStripMenuItem("OBJ to KCL");
			//toolsExt[1].Click += ObjToKCL;
		}

		private void ObjToKCL(object sender, EventArgs e)
		{
			OpenFileDialog opn = new OpenFileDialog();
			if (opn.ShowDialog() != DialogResult.OK) return;
			var mod = OBJ.Read(new MemoryStream(File.ReadAllBytes(opn.FileName)),null);
			if (mod.Faces.Count > 65535)
			{
				MessageBox.Show("this model has too many faces, only models with less than 65535 triangles can be converted");
				return;
			}
			var f = MarioKart.MK7.KCL.FromOBJ(mod);
			File.WriteAllBytes(opn.FileName + ".kcl", f.Write(Syroot.BinaryData.ByteOrder.LittleEndian));
		}

		private void KCLToObj(object sender, EventArgs e)
		{
			OpenFileDialog opn = new OpenFileDialog();
			if (opn.ShowDialog() != DialogResult.OK) return;
			var kcl = new MarioKart.MK7.KCL(File.ReadAllBytes(opn.FileName));
#if DEBUG
			using (StreamWriter f = new System.IO.StreamWriter(opn.FileName + ".obj"))
				kcl.ToOBJ().toWritableObj().WriteObj(f, null);
#else
			kcl.ToOBJ().toWritableObj().WriteObj(opn.FileName + ".obj");
#endif
		}

	}	
}
