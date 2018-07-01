using ModelViewer;
using EditorCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System.IO.Compression;
using EditorCore.EditorFroms;
using System.Diagnostics;
using EditorCore.Interfaces;
using ExtensionMethods;

namespace EditorCore
{
	public partial class EditorForm : Form
	{
		IGameModule GameModule = null;
		ExtensionManifest[] LoadedModules = null;

		public bool IsAddListSupported
		{
			set => Btn_addType.Enabled = value;
		}
		public bool IsPropertyEditingSupported
		{
			set
			{
				button4.Enabled = value;
				button5.Enabled = value;
			}
		}
		
        public string GameFolder = "";
        string ModelsFolder = null;

        public RendererControl render  = new RendererControl();

        public ILevel LoadedLevel { get; set; }
        public CustomStack<IObjList> ListEditingStack { get; set; } = new CustomStack<IObjList>();

        public ClipBoardItem StoredValue = null;
        public CustomStack<UndoAction> UndoList = new CustomStack<UndoAction>();

        public string CurListName
        {
            get {
                if (EditingList) return RendererControl.C0ListName;
                return comboBox1.Text;
            }
            set {
                if (EditingList) return;
				comboBox1.Text = value;
            }
        }

		public IObjList CurList
        {
            get
            {
                if (EditingList) return ListEditingStack.Peek(); ;
                return LoadedLevel.objs[CurListName];
            }
        }

		public int SelectionCount
        {
            get { return ObjectsListBox.SelectedItems.Count; }
        }

		ILevelObj SelectedObj
		{
			get
			{
				return SelectionCount == 0 ? null : CurList[ObjectsListBox.SelectedIndex];
			}
			set
			{
				ObjectsListBox.ClearSelected();
				if (value == null)
					return;
				if (!CurList.Contains(value))
				{
					if (EditingList) return; //if edit links edit only current list
					IObjList list = LoadedLevel.FindListByObj(value);
					if (list != null)
						CurListName = list.name;
				}
				ObjectsListBox.SelectedIndex = CurList.IndexOf(value);
			}
		}

		public ILevelObj[] SelectedObjs
        {
            get
            {
                if (SelectionCount > 0)
                {
                    var res = new ILevelObj[ObjectsListBox.SelectedIndices.Count];
                    for (int i = 0; i < res.Length; i++) res[i] = CurList[ObjectsListBox.SelectedIndices[i]];
                    return res;
                }
                else return new ILevelObj[0];
            }
        }

        bool EditingList { get { return ListEditingStack.Count != 0; } }

		public EditorForm(string[] args, ExtensionManifest[] Modules)
		{
			InitializeComponent();
#if DEBUG
			splitContainer2.Enabled = Debugger.IsAttached;
#else
			splitContainer2.Enabled = false;
#endif
			KeyPreview = true;
			RenderingCanvas.Child = render;
			render.MouseMove += render_MouseMove;
			render.MouseLeftButtonDown += render_MouseLeftButtonDown;
			render.MouseLeftButtonUp += render_MouseLeftButtonUp;
			render.KeyDown += render_KeyDown;
			render.KeyUp += render_KeyUP;
			render.CameraInertiaFactor = Properties.Settings.Default.CameraInertia;
			render.ShowFps = Properties.Settings.Default.ShowFps;
			render.ShowTriangleCount = Properties.Settings.Default.ShowTriCount;
			render.ShowDebugInfo = Properties.Settings.Default.ShowDbgInfo;
			render.CamMode = Properties.Settings.Default.CameraMode == 0 ? HelixToolkit.Wpf.CameraMode.Inspect : HelixToolkit.Wpf.CameraMode.WalkAround;
			render.ZoomSensitivity = Properties.Settings.Default.ZoomSen;
			render.RotationSensitivity = Properties.Settings.Default.RotSen;

#if DEBUG
			if (Debugger.IsAttached) this.Text += " - Debugger.IsAttached";
			else this.Text += " - DEBUG BUILD";
#endif
			List<ExtensionManifest> ExtensionsWithGameModules = new List<ExtensionManifest>();
			LoadedModules = Modules;
			foreach (var module in Modules)
			{
				if (module.HasGameModule) ExtensionsWithGameModules.Add(module);
				if (module.ClipboardExt != null)
				{
					if (module.ClipboardExt.PasteExtensions != null) RegisterMenuExt(ClipBoardMenu, module.ClipboardExt.PasteExtensions);
					if (module.ClipboardExt.CopyExtensions != null) RegisterMenuExtIndex(ClipBoardMenu, module.ClipboardExt.PasteExtensions);
				}
				if (module.MenuExt != null)
				{
					if (module.MenuExt.FileMenuExtensions != null)
						RegisterMenuExtIndex(FileMenu, module.MenuExt.FileMenuExtensions, FileMenu.DropDownItems.Count - 2); //last items are separator and settings
					if (module.MenuExt.ToolsMenuExtensions != null)
						RegisterMenuExtIndex(ToolsMenu, module.MenuExt.ToolsMenuExtensions);
					if (module.MenuExt.TitleBarExtensions != null)
						RegisterMenuExtIndex(menuStrip1, module.MenuExt.TitleBarExtensions);
				}
			}

			if (ExtensionsWithGameModules.Count == 0) return;
			else if (ExtensionsWithGameModules.Count == 1) GameModule = ExtensionsWithGameModules[0].GameModule;
			else
			{
				var dlg = new OtherForms.GameModuleSelect(ExtensionsWithGameModules);
				dlg.ShowDialog();
				if (dlg.result == null) return;
				GameModule = dlg.result.GameModule;
			}
			if (GameModule == null) return;

			GameModule.InitModule(this);
			ModelsFolder = GameModule.ModelsFolder;
			Properties.Settings.Default.Add<string>($"{GameModule.ModuleName}_GamePath","");
			GameFolder = (string)Properties.Settings.Default[$"{GameModule.ModuleName}_GamePath"];

			IsAddListSupported = GameModule.IsAddListSupported;
			IsPropertyEditingSupported = GameModule.IsPropertyEditingSupported;
			PropertyGridTypes.CustomClassConverter.Clear();

			if (GameModule.GetClassConverters != null)
			foreach (var t in GameModule.GetClassConverters)
				PropertyGridTypes.CustomClassConverter.Add(t.Item1, t.Item2);

			FileOpenArgs = args;

		}

		void RegisterMenuExt(ToolStripMenuItem target, ToolStripMenuItem[] list) => target.DropDownItems.AddRange(list);
		void RegisterMenuExt(ToolStrip target, ToolStripMenuItem[] list) => target.Items.AddRange(list);
		void RegisterMenuExtIndex(ToolStripMenuItem target, ToolStripMenuItem[] list, int index = 0)
		{
			foreach (var i in list)
				target.DropDownItems.Insert(index++, i);
		}
		void RegisterMenuExtIndex(ToolStrip target, ToolStripMenuItem[] list, int index = 0)
		{
			foreach (var i in list)
				target.Items.Insert(index++, i);
		}

		string[] FileOpenArgs = null;
        private void Form1_Load(object sender, EventArgs e)
		{
			if (GameModule == null)
			{
				MessageBox.Show("No GameModule found in loaded extensions !");
				this.Close();
				return;
			}

			GamePathAndModelCheck();
            if (Properties.Settings.Default.CheckUpdates)
                UpdateCheck.CheckForUpdatesAsync();

			if (FileOpenArgs != null)
				GameModule.ParseArgs(FileOpenArgs);
			//openToolStripMenuItem_Click(null, null);		
			this.Text += $" - {GameModule.ModuleName} module";
		}

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = GameModule.LevelFormatFilter;
            if (openFile.ShowDialog() != DialogResult.OK) return;
            LoadLevel(openFile.FileName);
        }

        void UnloadLevel()
        {
            List<Form> ToClose = new List<Form>();
            foreach (Form frm in Application.OpenForms)
            {
                if (frm != this)
                    ToClose.Add(frm);
            }
            for (int i = 0; i < ToClose.Count; i++)
                ToClose[i].Close();
            ToClose = null;

            saveAsToolStripMenuItem.Enabled = false;
            saveToolStripMenuItem.Enabled = false;

            splitContainer2.Enabled = false;
            FindMenu.Visible = false;

            HideGroup_CB.CheckedChanged -= HideGroup_CB_CheckedChanged;
            LevelFilesMenu.DropDownItems.Clear();
            render.UnloadLevel();
            LoadedLevel = null;

            ListEditingStack.Clear();
            //SelectionIndex = new Stack<int>();
            //InitialAllInfosSection = -1;

            //AllInfos = new Dictionary<string, AllInfoSection>();
            //AllRailInfos = new List<Rail>();
            //higestID = new Dictionary<string, int>();
            UndoList = new CustomStack<UndoAction>();
            comboBox1.Items.Clear();
            ObjectsListBox.Items.Clear();
            propertyGrid1.SelectedObject = null;

            if (SkipModels == null)
            {
                if (File.Exists($"{ModelsFolder}/SkipModels.txt"))
                    SkipModels = new List<string>(File.ReadAllLines($"{ModelsFolder}/SkipModels.txt"));
                else
                    SkipModels = new List<string>();
            }
            else File.WriteAllLines($"{ModelsFolder}/SkipModels.txt", SkipModels.ToArray());
        }

        public void LoadLevel(string path)
        {
            UnloadLevel();

			LoadedLevel = GameModule.LoadLevel(path);

			if (LoadedLevel.LevelFiles != null)
			{
				//Populate szs file list
				int index = 0;
				List<ToolStripMenuItem> Files = new List<ToolStripMenuItem>();
				foreach (var f in LoadedLevel.LevelFiles)
				{
					ToolStripMenuItem btn = new ToolStripMenuItem();
					btn.Name = "LoadFile" + index.ToString();
					btn.Text = f.Key;
					btn.Click += OpenSzsFile_click;
					Files.Add(btn);
					index++;
				}
				LevelFilesMenu.DropDownItems.AddRange(Files.ToArray());
			}
            //LoadedLevel.OpenBymlViewer();
            //Load models
            LoadingForm.ShowLoading(this, "Loading models...\r\nOpening a Level for the first time will take longer");
            foreach (string k in LoadedLevel.objs.Keys.ToArray())
            {
                LoadObjListModels(LoadedLevel.objs[k],k);
            }

			foreach (string l in GameModule.AutoHideList)
				if (LoadedLevel.HasList(l)) HideList(LoadedLevel.objs[l], true);

			HideGroup_CB.CheckedChanged += HideGroup_CB_CheckedChanged;
            //Populate combobox
            comboBox1.Items.AddRange(LoadedLevel.objs.Keys.ToArray());
            comboBox1.SelectedIndex = 0;
            splitContainer2.Enabled = true;
            FindMenu.Visible = true;
            saveAsToolStripMenuItem.Enabled = true;
            saveToolStripMenuItem.Enabled = true;
            LoadingForm.EndLoading();
        }

        //NewLevel
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UnloadLevel();
            SaveFileDialog sav = new SaveFileDialog();
            sav.Filter = GameModule.LevelFormatFilter;
            if (sav.ShowDialog() != DialogResult.OK) return;
            LoadedLevel =  GameModule.NewLevel(sav.FileName);
            //Populate combobox
            comboBox1.Items.AddRange(LoadedLevel.objs.Keys.ToArray());
            comboBox1.SelectedIndex = 0;
            splitContainer2.Enabled = true;
            FindMenu.Visible = true;
        }

        bool NoModels = true; //Debug only
        List<string> SkipModels = null;
        string GetModelName(string ObjName) //convert bfres to obj and cache in models folder
        {
#if DEBUG
			if (NoModels && Debugger.IsAttached)
                return null;
#endif
			if (SkipModels?.Contains(ObjName) ?? false) return null;

			string CachedModelPath = $"{ModelsFolder}\\{ObjName}.obj";
			if (render.ImportedModels.ContainsKey(CachedModelPath) || //The model has aready been loaded or has been converted
				GameModule.GetModelFile(ObjName, CachedModelPath))
				return CachedModelPath;

			SkipModels?.Add(ObjName);
            return null;
        }

        public void LoadObjListModels(IList<ILevelObj> list, string listName)
        {
            foreach (var obj in list)
            {
                AddModel(obj, listName);
            }
        }

        public void AddModel(ILevelObj obj, string listName)
        {
			if (obj is IPathObj)
			{
				render.AddPath(obj, ((IPathObj)obj).Points);
				return;
			}

            string PlaceholderModel = $"{ModelsFolder}/UnkBlue.obj";
            if (listName == "AreaList") PlaceholderModel = $"{ModelsFolder}/UnkYellow.obj";
            else if (listName == "DebugList") PlaceholderModel = $"{ModelsFolder}/UnkRed.obj";
            else if (listName == "CameraAreaInfo") PlaceholderModel = $"{ModelsFolder}/UnkGreen.obj";

            string ModelFile = GetModelName(obj.ModelName);
            if (ModelFile == null) ModelFile = PlaceholderModel;
            render.AddModel(ModelFile, obj, obj.ModelView_Pos, obj.ModelView_Scale, obj.ModelView_Rot);
        }

		public void AddModelObj(string path, object reference, Vector3D Pos, Vector3D Scale, Vector3D Rot ) =>
			render.AddModel(path, reference, Pos, Scale, Rot);

		public void UpdateModelPosition(ILevelObj o)
        {
			if (CurList is IPathObj)
				render.AddPath(CurList, ((IPathObj)CurList).Points);
			render.ChangeTransform(o, o.ModelView_Pos, o.ModelView_Scale, o.ModelView_Rot);
        }

        void PopulateListBox()
        {
            ObjectsListBox.Items.Clear();
            propertyGrid1.SelectedObject = null;
            foreach (var o in CurList) ObjectsListBox.Items.Add(o.ToString());
            ListEditingPanel.Visible = EditingList;
        }

        public void EditList(IList<dynamic> objList)
        {
            IObjList list = GameModule.CreateObjList(RendererControl.C0ListName, objList);
            ListEditingStack.Push(list);
            foreach (var o in list) ObjectsListBox.Items.Add(o.ToString());
            LoadObjListModels(list, RendererControl.C0ListName);
            PopulateListBox();
        }

        public void PreviousList()
        {
            if (!EditingList) return;
            foreach (var obj in CurList) render.RemoveModel(obj);
            ListEditingStack.Pop().ApplyChanges();
            PopulateListBox();
        }

        //Open,save find etc
#region UIControlsEvents
        private void objectByIdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string ID = "obj0";
            if (InputDialog.Show("Search by ID", "Write the ID to search for", ref ID) == DialogResult.Cancel) return;
            SearchObject(o => o.ID == ID, null, "Object ID =" + ID);
        }

        private void objectByNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string ObjName = "Coin";
            if (InputDialog.Show("Search by object name", "Write the name to search for", ref ObjName) == DialogResult.Cancel) return;
            ObjName = ObjName.ToLower();
            SearchObject(o => o.Name.ToLower() == ObjName, null, "Object name =" + ObjName);
        }

        private void objectByModelNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string ObjName = "Coin";
            if (InputDialog.Show("Search by ModelName", "Write the name to search for", ref ObjName) == DialogResult.Cancel) return;
            ObjName = ObjName.ToLower();
            SearchObject(o => o.ModelName.ToLower() == ObjName, null, "Object ModelName =" + ObjName);
        }

        private void label2_Click(object sender, EventArgs e)
        {
            new FrmCredits(GameModule,LoadedModules).ShowDialog();
        }

        private void propertyGridChange(object s, PropertyValueChangedEventArgs e)
        {
            if (SelectionCount < 0) { MessageBox.Show("No object selected in the list"); return; }
            {
                string name = e.ChangedItem.Label;				
                if (GameModule.ModelFieldPropNames.Contains(name) || name == "Name")
                {
                    string path = GetModelName(SelectedObj.ModelName);
                    if (path == null) path = $"{ModelsFolder}/UnkBlue.obj";
                    foreach (var i in SelectedObjs) render.ChangeModel(i, path);
                }
                foreach (var i in SelectedObjs)
                {
                    UpdateModelPosition(i);
                }
            }
        }

        private void Btn_AddObj_Click(object sender, EventArgs e)
        {
            string name = "";
            InputDialog.Show("", "Enter a name for the object", ref name);
            if (name.Trim() == "") return;
            var o = GameModule.NewObject();
			o.ID_int = LoadedLevel.HighestID++;
			o.Name = name;
            o.ModelView_Pos = render.GetPositionInView();
            AddObj(o, CurList);
            render.LookAt(o.ModelView_Pos);
        }
        
#region ClipBoard
        private void ClipBoardMenu_Opening(object sender, CancelEventArgs e)
        {
            ClipBoardMenu_Paste.Enabled = StoredValue != null;
            {
                bool SingleObjectSelected = SelectionCount == 1;
                ClipBoardMenu_CopyPos.Visible = SingleObjectSelected;
                ClipBoardMenu_CopyRot.Visible = SingleObjectSelected;
                ClipBoardMenu_CopyScale.Visible = SingleObjectSelected;
                ClipBoardMenu_CopyTransform.Visible = SingleObjectSelected;
            }
            if (SelectionCount > 1) ClipBoardMenu_CopyFull.Text = "Copy objects";
            else ClipBoardMenu_CopyFull.Text = "Copy object";
            ClipBoardMenu_CopyFull.Visible = SelectionCount != 0;
        }

        private void ClipBoardMenu_CopyTransform_Click(object sender, EventArgs e) =>
            StoredValue = new ClipBoardItem() { Type = ClipBoardItem.ClipboardType.Transform, transform = SelectedObj.transform };

        private void ClipBoardMenu_CopyPos_Click(object sender, EventArgs e) =>
            StoredValue = new ClipBoardItem() { Type = ClipBoardItem.ClipboardType.Position, transform = SelectedObj.transform };

        private void ClipBoardMenu_CopyRot_Click(object sender, EventArgs e) =>
            StoredValue = new ClipBoardItem() { Type = ClipBoardItem.ClipboardType.Rotation, transform = SelectedObj.transform };

        private void ClipBoardMenu_CopyScale_Click(object sender, EventArgs e) =>
            StoredValue = new ClipBoardItem() { Type = ClipBoardItem.ClipboardType.Scale, transform = SelectedObj.transform };

        private void Btn_CopyObjs_Click(object sender, EventArgs e) => ClipBoardMenu_CopyFull_Click(null, null);
        private void ClipBoardMenu_CopyFull_Click(object sender, EventArgs e)
        {
            ILevelObj[] objs = new ILevelObj[SelectionCount];
            for (int i = 0; i < objs.Length; i++) objs[i] = (ILevelObj)SelectedObjs[i].Clone();
            StoredValue = new ClipBoardItem() { Type = ClipBoardItem.ClipboardType.Objects, Objs = objs };
        }

        private void ClipBoardMenu_Paste_Click(object sender, EventArgs e)
        {
            if (StoredValue.Type == ClipBoardItem.ClipboardType.Objects)
            {
                foreach (var o in StoredValue.Objs)
                    AddObj((ILevelObj)o.Clone(), CurList);
            }
            else if (SelectionCount != 0)
            {

                Tuple<ILevelObj, Transform>[] args = new Tuple<ILevelObj, Transform>[SelectionCount];
                for (int i = 0; i < SelectionCount; i++) args[i] = new Tuple<ILevelObj, Transform>(SelectedObjs[i], SelectedObjs[i].transform);
                AddToUndo((dynamic arg) =>
               {
                   var _args = (Tuple<ILevelObj, Transform>[])arg;
                   foreach (var a in _args)
                   {
                       a.Item1.transform = a.Item2;
                       UpdateModelPosition(a.Item1);
                   }
               }, $"Pasted value to {SelectionCount} Object(s)");

                foreach (var o in SelectedObjs)
                {
                    switch (StoredValue.Type)
                    {
                        case ClipBoardItem.ClipboardType.Position:
                            o.Pos = StoredValue.transform.Pos;
                            break;
                        case ClipBoardItem.ClipboardType.Rotation:
                            o.Rot = StoredValue.transform.Rot;
                            break;
                        case ClipBoardItem.ClipboardType.Scale:
                            o.Scale = StoredValue.transform.Scale;
                            break;
                        case ClipBoardItem.ClipboardType.Transform:
                            o.transform = StoredValue.transform;
                            break;
                    }
                    UpdateModelPosition(o);
                }
            }
        }
#endregion

        private void UndoMenu_Open(object sender, EventArgs e)
        {
            UndoMenu.DropDownItems.Clear();
            List<ToolStripMenuItem> Items = new List<ToolStripMenuItem>();
            int count = 0;
            foreach (UndoAction act in UndoList.ToArray().Reverse())
            {
                ToolStripMenuItem btn = new ToolStripMenuItem();
                btn.Name = "Undo" + count.ToString();
                btn.Text = act.ToString();
                btn.Click += UndoListItem_Click;
                btn.MouseEnter += UndoListItem_MouseEnter;
                Items.Add(btn);
                count++;
            }
            UndoMenu.DropDownItems.AddRange(Items.ToArray());
        }

        private void UndoListItem_MouseEnter(object sender, EventArgs e)
        {
            string SenderName = ((ToolStripMenuItem)sender).Name;
            int index = int.Parse(SenderName.Substring("Undo".Length));
            for (int i = 0; i < UndoMenu.DropDownItems.Count; i++)
            {
                if (i < index) UndoMenu.DropDownItems[i].BackColor = Color.LightBlue;
                else UndoMenu.DropDownItems[i].BackColor = SystemColors.Control;
            }
        }

        private void UndoListItem_Click(object sender, EventArgs e)
        {
            string SenderName = ((ToolStripMenuItem)sender).Name;
            int index = int.Parse(SenderName.Substring("Undo".Length));
            for (int i = 0; i <= index; i++)
            {
                UndoList.Pop().Undo();
            }
            UndoMenu.HideDropDown();
        }

        private void OpenSzsFile_click(object sender, EventArgs e)
        {
            string name = ((ToolStripMenuItem)sender).Text;
			var stream = new MemoryStream(LoadedLevel.LevelFiles[name]);
			GameModule.OpenLevelFile(name, stream);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
#if DEBUG
			saveAsToolStripMenuItem_Click(sender, e); //Let's not risk modifing our precious dump
#else
            File.WriteAllBytes(LoadedLevel.FilePath, LoadedLevel.SaveSzs());
#endif
        }

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LoadedLevel == null) return;
            SaveFileDialog sav = new SaveFileDialog();
            sav.Filter = GameModule.LevelFormatFilter;
            sav.FileName = Path.GetFileNameWithoutExtension(LoadedLevel.FilePath);
            if (sav.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(sav.FileName, LoadedLevel.Save(sav.FileName));
            }
        }
		
        private void Btn_addType_Click(object sender, EventArgs e)
        {
			string res = GameModule.AddObjList(LoadedLevel);
			if (res == null) return;
            comboBox1.SelectedIndex = comboBox1.Items.Add(res);
        }

        private void changeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                GameFolder = dlg.SelectedPath;
                if (!GameFolder.EndsWith("\\")) GameFolder += "\\";
				Properties.Settings.Default[$"{GameModule.ModuleName}_GamePath"] = GameFolder;
                Properties.Settings.Default.Save();
                gamePathToolStripItem.Text = "Game path: " + GameFolder;
				GameModule.FormLoaded();
			}
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e) => new Settings(render).ShowDialog();

        private void DuplicateSelectedObj_btn(object sender, EventArgs e) => DuplicateObj(SelectedObj, CurList);
        private void btn_delObj_Click(object sender, EventArgs e)
        {
            var list = SelectedObjs.ToArray();
            foreach (var o in list) DeleteObj(o, CurList);
        }

#endregion

		//Property grid change, listbox, combobox, show/hide
#region EditorControlsEvents

		List<string> PropertyGridGetPath()
		{
			var item = propertyGrid1.SelectedGridItem;
			if (item == null) return null;
			List<string> SelectedPath = new List<string>();
			while (item.Parent.Parent.Parent != null) //t.Parent.Parent.Parent is properties 
			{
				SelectedPath.Add(item.Label);
				item = item.Parent;
			}
			return SelectedPath;
		}

		private void button5_Click(object sender, EventArgs e)
		{
			var SelectedPath = PropertyGridGetPath();
			if (SelectedPath.Count == 0 ||
				(SelectedPath.Count == 1 && GameModule.ReservedPropNames.Contains(SelectedPath[0])))
			{
				MessageBox.Show("Can't remove this property");
				return;
			}
			foreach (var o in SelectedObjs)
			{
				dynamic d = o.Prop;
				for (int i = SelectedPath.Count-1; i > 0; i--)
					d = d[SelectedPath[i]];
				if (d is Dictionary<string, dynamic>)
					((Dictionary<string,dynamic>)d).Remove(SelectedPath[0]);
				else if (d is List<dynamic>)
				{
					for (int i = 0; i < propertyGrid1.SelectedGridItem.Parent.GridItems.Count; i++) //Get the index of the selected item
						if (propertyGrid1.SelectedGridItem == propertyGrid1.SelectedGridItem.Parent.GridItems[i])
						{
							((List<dynamic>)d).RemoveAt(i);
							break;
						}
				}
			}
			propertyGrid1.Refresh();
		}		

		private void button4_Click(object sender, EventArgs e)
		{
			var newProp = AddPropertyDialog.newProperty(!(propertyGrid1.SelectedGridItem.Value is List<dynamic>));
			if (newProp == null) return;
			var path = PropertyGridGetPath();

			bool clone = newProp.Item2 is Dictionary<string, dynamic> || newProp.Item2 is List<dynamic>; //reference types must be manually cloned
			
			foreach (var o in SelectedObjs)
			{
				var toAdd = clone ? DeepCloneDictArr.DeepClone(newProp.Item2) : newProp.Item2;

				if (path == null)
					o.Prop.Add(newProp.Item1, toAdd);
				else
				{
					dynamic target = o.Prop;
					for (int i = path.Count - 1; i >= 0; i--)
						target = target[path[i]];
					if (target is List<dynamic>)
						((List<dynamic>)target).Add(toAdd);
					else if (target is Dictionary<string, dynamic>)
						((Dictionary<string, dynamic>)target).Add(newProp.Item1, toAdd);
				}
			}
			propertyGrid1.Refresh();
		}

		private void lnk_hideSelectedObjs_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (CurList.IsHidden) return;
            foreach (var o in SelectedObjs)
                render.RemoveModel(o);            
        }

        private void lnk_ShowHiddenObjs_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            foreach (var list in ListEditingStack)
            {
                if (list.IsHidden) continue;
                foreach (var o in list)
                    AddModel(o, list.name);
            }
            foreach (var list in LoadedLevel.objs.Values)
            {
                if (list.IsHidden) continue;
                foreach (var o in list)
                    AddModel(o, list.name);
            }
        }

        private void ListEditGoBack(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PreviousList();
        }

        private void SelectedListChanged(object sender, EventArgs e) //comboBox1
        {
            if (EditingList) return;
            ObjectsListBox.Items.Clear();
            propertyGrid1.SelectedObject = null;
            if (CurListName == null || CurListName == "" || CurList.Count == 0) return;
            HideGroup_CB.CheckedChanged -= HideGroup_CB_CheckedChanged; //Do not trigger event, unneeded (?)
            HideGroup_CB.Checked = CurList.IsHidden;
            HideGroup_CB.CheckedChanged += HideGroup_CB_CheckedChanged;
            foreach (var o in CurList) ObjectsListBox.Items.Add(o.ToString());
        }

        private void SelectedObjectChanged(object sender, EventArgs e) //ObjectsListBox
        {
            if (SelectionCount > 1)
            {
                Btn_CopyObjs.Visible = true;
                Btn_Duplicate.Visible = false;
            }
            else
            {
                Btn_CopyObjs.Visible = false;
                Btn_Duplicate.Visible = true;
            }

            if (ObjectsListBox.SelectedIndex == -1 || SelectedObj == null)
            {
                propertyGrid1.SelectedObject = null;
				render.ClearSelection();
				return;
            }

            propertyGrid1.SelectedObjects = SelectedObjs;

            if (CurList.IsHidden)
            {
                foreach (var o in CurList)
                    render.RemoveModel(o);
                foreach (var o in SelectedObjs)
                    AddModel(o, CurList.name);
            }

            render.SelectObjs(SelectedObjs.Cast<dynamic>().ToList());
        }        

        private void ObjectsList_DoubleClick(object sender, EventArgs e)
        {
            if (ObjectsListBox.SelectedIndex == -1 || SelectionCount > 1) return;
            render.LookAt(SelectedObj.ModelView_Pos);
        }

        private void HideGroup_CB_CheckedChanged(object sender, EventArgs e)
        {
            HideList(CurList, HideGroup_CB.Checked);
        }
#endregion

        //Dragging, click
#region RendererEvents
            
        DragArgs DraggingArgs = null;
        bool RenderIsDragging { get { return DraggingArgs != null && Mouse.LeftButton == MouseButtonState.Pressed && (ModifierKeys & Keys.Control) == Keys.Control; } }        
		
        private void render_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) //Render hotkeys
        {
            if (RenderIsDragging) return;
            HandleHotKey(e);
        }
        
        private void render_KeyUP(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                if (DraggingArgs != null) endDragging();
            }
        }        

        private void render_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!RenderIsDragging) return;
            int RoundTo = (ModifierKeys & Keys.Alt) == Keys.Alt ? 100 : ((ModifierKeys & Keys.Shift) == Keys.Shift ? 50 : 0);

            Vector3D NewPos = render.Drag(DraggingArgs, e, RoundTo);
            if (NewPos == null) return;
            ((ILevelObj)DraggingArgs.obj).ModelView_Pos = NewPos;
            UpdateModelPosition(DraggingArgs.obj);
            DraggingArgs.position = NewPos;
        }

        void endDragging()
        {
            AddToUndo((dynamic args) => 
            {
                var a = (DragArgs)args;
                ((ILevelObj)a.obj).ModelView_Pos = a.StartPos;
                UpdateModelPosition(a.obj);
            }, $"Moved object {SelectedObj}", DraggingArgs);
            DraggingArgs = null;
            propertyGrid1.Refresh();
        }

        private void render_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (DraggingArgs != null) endDragging();
        }

        private void render_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
			if (RenderIsDragging) return;
			var obj = render.GetOBJ(sender, e);
			if (obj == null)
			{
				return;
			}

			if ((ModifierKeys & Keys.Shift) == Keys.Shift && CurList.Contains(obj))
			{
				ObjectsListBox.SelectedIndices.Add(CurList.IndexOf(obj));
				return; // drag only a single object
			}
			else
				SelectedObj = obj;

			if (SelectedObj == obj && (ModifierKeys & Keys.Control) == Keys.Control) //User wants to drag
			{
				DraggingArgs = new DragArgs();
				DraggingArgs.obj = obj;
				DraggingArgs.StartPos = ((ILevelObj)DraggingArgs.obj).ModelView_Pos;
				DraggingArgs.position = DraggingArgs.StartPos;
			}
		}

#endregion

        void HandleHotKey(System.Windows.Input.KeyEventArgs e)
        {
            if (RenderIsDragging) return;
            if (e.Key == Key.B && EditingList) PreviousList();
            if (SelectionCount == 0) return;
			if (e.Key == Key.Space) render.LookAt(SelectedObj.ModelView_Pos);
			else if (e.Key == Key.OemPlus && Btn_AddObj.Enabled) Btn_AddObj_Click(null, null);
			else if (e.Key == Key.D && SelectionCount == 1) DuplicateObj(SelectedObj, CurList);
			else if (e.Key == Key.Delete) btn_delObj_Click(null, null);
			else if (e.Key == Key.F) FindMenu.ShowDropDown();
			else if (e.Key == Key.H) lnk_hideSelectedObjs_LinkClicked(null, null);
			else if (e.Key == Key.Z && UndoList.Count > 0) UndoList.Pop().Undo();
			else if (e.Key == Key.Q) render.CamMode = render.CamMode == HelixToolkit.Wpf.CameraMode.Inspect ? HelixToolkit.Wpf.CameraMode.WalkAround : HelixToolkit.Wpf.CameraMode.Inspect;
			else if (e.Key == Key.C && SelectionCount == 1)
			{
				GameModule.EditChildrenNode(SelectedObj);
			}
#if DEBUG
			else if (e.Key == Key.P)
				foreach (IObjList l in LoadedLevel.objs.Values)
					foreach (ILevelObj o in l) UpdateModelPosition(o);
#endif
			else return;
        }

        const int UndoMax = 30;
        public void AddToUndo(Action<dynamic> act, string desc, dynamic arg = null)
        {
            UndoList.Push(new UndoAction(desc, act, arg));
            if (UndoList.Count > UndoMax) UndoList.RemoveAt(0);
        }

        public void HideList(IObjList list, bool hide)
        {
            if (LoadedLevel == null || list.IsHidden == hide) return;

            list.IsHidden = hide;
            if (hide)
            {
                foreach (var o in list)
                    render.RemoveModel(o);
            }
            else
            {
                foreach (var o in list)
                    AddModel(o, list.name);
            }
        }

        public void AddObj(ILevelObj o, IObjList list)
        {
            AddToUndo((dynamic) => InternalDeleteObj(o, list), "Added object: " + o.ToString());
            InternalAddObj(o, list);
        }

        void InternalAddObj(ILevelObj o, IObjList list)
        {
            list.Add(o);
            if (list == CurList)
            {
                ObjectsListBox.Items.Add(o.ToString());
            }
            if (!(list.name == RendererControl.C0ListName && EditingList))
                AddModel(o, list.name);
        }

        public void DuplicateObj(ILevelObj o, IObjList list)
        {
            if (o == null) return;
            var newobj = (ILevelObj)o.Clone();
            newobj.ID_int = LoadedLevel.HighestID++;
            AddObj(newobj, list);
        }

        public void DeleteObj(ILevelObj o, IObjList list)
        {
            if (o == null) return;
            AddToUndo((dynamic) =>
            InternalAddObj(o, list), "Deleted object: " + o.ToString());
            InternalDeleteObj(o, list);
        }

        public void InternalDeleteObj(ILevelObj o, IObjList list)
        {
            ObjectsListBox.SelectedIndex = -1;
            if (list == CurList)
            {
                ObjectsListBox.Items.RemoveAt(CurList.IndexOf(o));
                render.RemoveModel(o);
            }
            list.Remove(o);
        }

        public void SearchObject(Func<ILevelObj,bool> seachFn, IObjList list = null, string QueryDescription = "")
        {
            List<Tuple<IObjList, ILevelObj>> Result = new List<Tuple<IObjList, ILevelObj>>();
            if (list != null)
            {
                foreach (var o in list)
                {
                    if (seachFn(o)) Result.Add(new Tuple<IObjList, ILevelObj>(list, o));
                }
            }
            else if (EditingList)
            {
                foreach (var o in CurList)
                {
                    if (seachFn(o)) Result.Add(new Tuple<IObjList, ILevelObj>(CurList, o));
                }
            }
            else
            {
                foreach (var k in LoadedLevel.objs.Values)
                {
                    foreach (var o in k)
                    {
                        if (seachFn(o)) Result.Add(new Tuple<IObjList, ILevelObj>(k, o));
                    }
                }
            }
            new EditorFroms.SearchResult(Result.ToArray(), QueryDescription, this).Show();
        }

        public void SelectObject(IObjList List, ILevelObj obj)
        {
            if (EditingList)
            {
                if (List != CurList) return;
            }
            else
                comboBox1.Text = List.name;
            ObjectsListBox.ClearSelected();
            ObjectsListBox.SelectedIndex = List.IndexOf(obj);
        }

        void GamePathAndModelCheck()
        {
			gamePathToolStripItem.Text = "Game path: " + GameFolder;
            if (GameFolder == "" || !Directory.Exists(GameFolder))
            {
                MessageBox.Show("Select the path of the game, it will be used to display the models from the game");
                changeToolStripMenuItem_Click(null, null);
                MessageBox.Show("You can change it from the tools menu later");
                this.Focus();
            }
			else GameModule.FormLoaded(); //GameModule.FormLoaded is also called in changeToolStripMenuItem_Click
			if (Properties.Settings.Default.FirstStart)
            {
                Properties.Settings.Default.FirstStart = false;
                Properties.Settings.Default.Save();
                MessageBox.Show("You can now custmize the settings (to open this window again click on File -> Settings)");
                new Settings(render).ShowDialog();
                this.Focus();
            }
        }
	}
}
