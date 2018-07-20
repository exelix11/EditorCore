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

            PublicFunctions.WriteObj(kcl,opn.FileName);
		}

        
    }

    public static class PublicFunctions
    {
        public static void WriteObj(KCL kcl, string fileName, List<Color> typeColors = null)
        {
            StreamWriter f = new System.IO.StreamWriter(fileName + ".obj");
            StreamWriter fmat = new System.IO.StreamWriter(fileName + ".mtl");
            f.WriteLine($"mtllib {fileName.Split('\\').Last()}.mtl");
            int index = 0;

            List<int> addedMaterials = new List<int>();

            foreach (var mod in kcl.models)
            {
                f.WriteLine("o model" + index);

                Dictionary<int, List<KCL.KCLModel.Face>> faces = new Dictionary<int, List<KCL.KCLModel.Face>>();

                var vert = mod.vertices;
                foreach (var v in vert)
                {
                    f.WriteLine($"v {v.pos.X} {v.pos.Y} {v.pos.Z}"); //{v.col.X} {v.col.Y} {v.col.Z} for vertex colors
                    f.WriteLine($"vn {v.nrm.X} {v.nrm.Y} {v.nrm.Z}");
                }

                foreach (var face in mod.Faces)
                {
                    if (!faces.ContainsKey(face.MaterialFlag))
                        faces[face.MaterialFlag] = new List<KCL.KCLModel.Face>();
                    faces[face.MaterialFlag].Add(face);
                }

                int typeIndex = 0;
                foreach (int type in faces.Keys)
                {
                    if (!addedMaterials.Contains(type))
                    {
                        fmat.WriteLine($"newmtl collision{typeIndex}");
                        fmat.WriteLine("Ns 100");
                        fmat.WriteLine("Ka 0.000000 0.000000 0.000000");
                        fmat.WriteLine((typeColors != null) ? $"Kd {typeColors[type].R / 255.0} {typeColors[type].G / 255.0} {typeColors[type].B / 255.0}" : "Kd 1.000000 1.000000 1.000000");
                        fmat.WriteLine("Ks 0.000000 0.000000 0.000000");
                        fmat.WriteLine("Ke 0.000000 0.000000 0.000000");
                        fmat.WriteLine("Ni 1.000000");
                        fmat.WriteLine("d 1.000000");
                        fmat.WriteLine("illum 2");
                        addedMaterials.Add(type);
                    }

                    f.WriteLine("usemtl collision" + typeIndex);
                    foreach (KCL.KCLModel.Face face in faces[type])
                    {
                        f.WriteLine(
                        $"f {vert.IndexOf(face.vtx) + 1} " +
                          $"{vert.IndexOf(face.vtx2) + 1} " +
                          $"{vert.IndexOf(face.vtx3) + 1}");
                    }
                    typeIndex++;
                }

                index++;
            }

            f.Close();
            fmat.Close();
        }
    }
}
