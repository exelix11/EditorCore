using EditorCore.Interfaces;
using Smash_Forge;
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

		ToolStripMenuItem[] toolsExt = new ToolStripMenuItem[1];
		public menuExt()
		{
			toolsExt[0] = new ToolStripMenuItem("KCL to OBJ");
			toolsExt[0].Click += KCLToObj;
		}

		private void KCLToObj(object sender, EventArgs e)
		{
			OpenFileDialog opn = new OpenFileDialog();
			if (opn.ShowDialog() != DialogResult.OK) return;
			var kcl = new KCL(File.ReadAllBytes(opn.FileName));
			using (System.IO.StreamWriter f = new System.IO.StreamWriter(opn.FileName + ".obj"))
			{
				int VertexOffest = 0;
				foreach (var mod in kcl.models)
				{
					var vert = mod.CreateDisplayVertices();
					foreach (var v in vert)
					{
						f.WriteLine($"v {v.pos.X} {v.pos.Y} {v.pos.Z} {v.col.X} {v.col.Y} {v.col.Z}");
						f.WriteLine($"vn {v.nrm.X} {v.nrm.Y} {v.nrm.Z}");
					}
					var disp = mod.display;
					for (int i = 0; i < mod.displayFaceSize;)
						{
						f.WriteLine(
							$"f {disp[i] + VertexOffest}//{disp[i++] + VertexOffest} " +
							  $"{disp[i] + VertexOffest}//{disp[i++] + VertexOffest} " +
							  $"{disp[i] + VertexOffest}//{disp[i++] + VertexOffest}");
					}
					VertexOffest += vert.Count;
					break; //export just the first mesh
				}
			}
		}
	}
}
