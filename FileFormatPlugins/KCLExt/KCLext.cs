using EditorCore.Interfaces;
using Smash_Forge;
using Syroot.NintenTools.MarioKart8.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KCLExt
{
	class KCLExt : ExtensionManifest
	{
		public string ModuleName => "KCL extension";

		public string Author => "";

		public string ThanksTo => "";

		public Version TargetVersion => throw new NotImplementedException();

		public bool HasGameModule => false;

		public IMenuExtension MenuExt { get; } = new menuExt();

		public IClipboardExtension ClipboardExt => null;

		public IGameModule GameModule => null;

		public IFileHander[] Handlers => null;

		public void CheckForUpdates() { }
	}

	class menuExt : IMenuExtension
	{
		public ToolStripMenuItem[] FileMenuExtensions => null;
		public ToolStripMenuItem[] ToolsMenuExtensions => toolsExt;
		public ToolStripMenuItem[] TitleBarExtensions => null;

		ToolStripMenuItem[] toolsExt = new ToolStripMenuItem[2];
		public menuExt()
		{
			toolsExt[0] = new ToolStripMenuItem("KCL to OBJ");
			toolsExt[0].Click += KCLToObj;
			toolsExt[1] = new ToolStripMenuItem("OBJ to KCL");
			toolsExt[1].Click += ObjToKCL;
		}

		private void ObjToKCL(object sender, EventArgs e)
		{
			OpenFileDialog opn = new OpenFileDialog();
			if (opn.ShowDialog() != DialogResult.OK) return;
			var mod = new Syroot.NintenTools.MarioKart8.Common.Custom.ObjModel(opn.FileName);
			var f = new Syroot.NintenTools.MarioKart8.Collisions.Custom.KclFile(mod);
			f.Save(opn.FileName + ".kcl");
		}

		private void KCLToObj(object sender, EventArgs e)
		{
			OpenFileDialog opn = new OpenFileDialog();
			if (opn.ShowDialog() != DialogResult.OK) return;
			var kcl = new KCL(File.ReadAllBytes(opn.FileName));
			using (System.IO.StreamWriter f = new System.IO.StreamWriter(opn.FileName + ".obj"))
			{
				int VertexOffest = 1;
				foreach (var mod in kcl.models)
				{
					var vert = mod.vertices;
					foreach (var v in vert)
					{
						f.WriteLine($"v {v.pos.X} {v.pos.Y} {v.pos.Z}"); //{v.col.X} {v.col.Y} {v.col.Z} for vertex colors
						f.WriteLine($"vn {v.nrm.X} {v.nrm.Y} {v.nrm.Z}");
					}
					var disp = mod.display;
					for (int i = 0; i < disp.Length;)
					{
						f.WriteLine(
							$"f {disp[i] + VertexOffest}//{disp[i++] + VertexOffest} " +
							  $"{disp[i] + VertexOffest}//{disp[i++] + VertexOffest} " +
							  $"{disp[i] + VertexOffest}//{disp[i++] + VertexOffest}");
					}
					VertexOffest += vert.Count;
				}
			}
		}
	}
}
