using EditorCore.Common;
using EditorCore.Interfaces;
using Smash_Forge;
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

#if DEBUG
			using (StreamWriter f = new System.IO.StreamWriter(opn.FileName + ".obj"))
				kcl.ToObj().toWritableObj().WriteObj(f, null);
#else
			kcl.ToObj().toWritableObj().WriteObj(opn.FileName + ".obj");
#endif
		}
        
    }

	public static class Exten
	{
		public static OBJ ToObj(this KCL kcl)
		{
			var res = new OBJ();
			Dictionary<int, OBJ.Material> materials = new Dictionary<int, OBJ.Material>();

			OBJ.Material GetOrAddMaterial(int materialFlag, KCL.KCLModel.Face f)
			{
				if (materials.ContainsKey(materialFlag))
					return materials[materialFlag];
				else
				{
					var mat = new OBJ.Material()
					{
						Name = $"MatID_{f.MaterialFlag}",
						Colors = new OBJ.Vertex(0, 0, 0, f.vtx.col.X, f.vtx.col.Y, f.vtx.col.Z)
					};
					materials.Add(materialFlag, mat);
					return mat;
				}
			}

			foreach (var m in kcl.models)
				foreach (var f in m.Faces)
				{
					res.Faces.Add(new OBJ.Face()
					{
						VA = new OBJ.Vertex(f.vtx.pos.X, f.vtx.pos.Y, f.vtx.pos.Z, f.vtx.nrm.X, f.vtx.nrm.Y, f.vtx.nrm.Z),
						VB = new OBJ.Vertex(f.vtx2.pos.X, f.vtx2.pos.Y, f.vtx2.pos.Z, f.vtx2.nrm.X, f.vtx2.nrm.Y, f.vtx2.nrm.Z),
						VC = new OBJ.Vertex(f.vtx3.pos.X, f.vtx3.pos.Y, f.vtx3.pos.Z, f.vtx3.nrm.X, f.vtx3.nrm.Y, f.vtx3.nrm.Z),
						Mat = GetOrAddMaterial(f.MaterialFlag,f)
					});
				}

			return res;
		}
	}
}
