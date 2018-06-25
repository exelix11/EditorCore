using BfresLib;
using EditorCore;
using EditorCore.EditorFroms;
using EditorCore.Interfaces;
using EveryFileExplorer;
using SARCExt;
using Syroot.NintenTools.Byaml.Dynamic;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OdysseyExt
{
	public class OdysseyModule : IGameSpecificModule
	{
		public string ModuleName => "Odyssey level";

		public Tuple<Type, Type>[] GetClassConverters => new Tuple<Type, Type>[] {
			new Tuple<Type, Type>(typeof(LinksNode), typeof(LinksConveter))
		};

		public string[] ReservedPropNames => LevelObj.CantRemoveNames;
		public string[] ModelFieldPropNames => LevelObj.ModelFieldNames;

		public bool IsAddListSupported => true;
		public bool IsPropertyEditingSupported => true;
		public string LevelFormatFilter => "szs file | *.szs";
		public string[] AutoHideList => new string[] { "AreaList", "SkyList" };

		public EditorForm ViewForm { get; set; } = null;
		public string GameFolder => EditorForm.GameFolder;
		public bool GetModelFile(string ObjName, string path) => BfresConverter.Convert(BfresFromSzs(ObjName), path);

		public void InitModule(EditorForm currentView)
		{
			ViewForm = currentView;
		}

		public void ParseArgs(string[] Args)
		{
			foreach (string file in Args)
			{
				if (File.Exists(file))
				{
					if (file.EndsWith("byml") || file.EndsWith("byaml"))
					{
						ByamlViewer.OpenByml(file);
					}
					else if (file.EndsWith(".szs"))
					{
						ViewForm.LoadLevel(file);
						return;
					}
				}
			}
		}
#if DEBUG
		public ILevel LoadLevel(string file) => new Level(file, 0);
#else		
		public ILevel LoadLevel(string file) => new Level(file, -1);
#endif
		public ILevel NewLevel(string file) => new Level(true, file);
		public IObjList CreateObjList(string name, IList<dynamic> baseList) =>
			 new ObjList(name, baseList);

		public ILevelObj NewObject() => new LevelObj();

		public void OpenLevelFile(string name, Stream file)
		{
			var byml = ByamlFile.Load(file, false, Syroot.BinaryData.ByteOrder.LittleEndian);
			new ByamlViewer(byml, file).Show();
		}

		public string AddType(ILevel level)
		{
			var f = new EditorFroms.AddObjList();
			f.ShowDialog();
			if (f.Result == null || f.Result.Trim() == "") return null;
			level.objs.Add(f.Result, new ObjList(f.Result, null));
			return f.Result;
		}

		public void EditChildrenNode(ILevelObj obj)
		{
			if (obj[LevelObj.N_Links] != null)
			{
				var BakLinks = ((LinksNode)obj[LevelObj.N_Links]).Clone();

				ViewForm.AddToUndo((dynamic arg) =>
				{
					((ILevelObj)arg[0])[LevelObj.N_Links] = arg[1];
				},
					$"Edited links of {obj.ToString()}",
					new dynamic[] { obj, BakLinks });

				new EditorFroms.LinksEditor(obj[LevelObj.N_Links]).ShowDialog();
			}
		}

		string ModelsFolder => EditorForm.ModelsFolder;

		public void FormLoaded()
		{
			if (!Directory.Exists(ModelsFolder))
			{
				Directory.CreateDirectory(ModelsFolder);
				ZipArchive z = new ZipArchive(new MemoryStream(Properties.Resources.baseModels));
				z.ExtractToDirectory(ModelsFolder);
			}
			if (!Directory.Exists($"{ModelsFolder}/GameTextures"))
			{
				if (GameFolder == "" || !Directory.Exists(GameFolder))
					MessageBox.Show("The game path is not set or not valid, can't extract texture archives");
				else
				{
					MessageBox.Show($"The game texture archives will be extracted in {ModelsFolder}/GameTextures, this might take a while");
					LoadingForm.ShowLoading(ViewForm, "Extracting textures...\r\nThis might take a while");
					foreach (var a in Directory.GetFiles($"{GameFolder}ObjectData\\").Where(x => x.EndsWith("Texture.szs")))
					{
						BfresLib.BfresConverter.GetTextures(
							BfresFromSzs(Path.GetFileNameWithoutExtension(a)),
							ModelsFolder);
					}
					LoadingForm.EndLoading();
				}
			}
		}

		byte[] BfresFromSzs(string fileName)
		{
			if (File.Exists($"{GameFolder}ObjectData\\{fileName}.szs"))
			{
				var SzsFiles = new SARC().unpackRam(YAZ0.Decompress($"{GameFolder}ObjectData\\{fileName}.szs"));
				if (SzsFiles.ContainsKey(fileName + ".bfres"))
				{
					return SzsFiles[fileName + ".bfres"];
				}
			}
			return null;
		}
	}

	public class LinksConveter : System.ComponentModel.TypeConverter
	{
		public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, Type sourceType)
		{
			return false;
		}

		public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			return "<Links>";
		}
	}
}
