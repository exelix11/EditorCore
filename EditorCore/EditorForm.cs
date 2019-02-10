using EditorCore;
using EditorCore.Drawing;
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
using EditorCoreCommon;
using GL_EditorFramework.EditorDrawables;
using GL_EditorFramework.GL_Core;
using OpenTK;

namespace EditorCore
{
	public partial class EditorForm : Form, IEditorFormContext
	{
		ExtensionManifest SelectedGameModuleExtension;
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
		
        public string GameFolder { get; set; } = "";
        string ModelsFolder = null;
		string CustomModelsFolder = null;

		public RenderSceneBase render = null;

		public ILevel LoadedLevel { get; set; }
        public CustomStack<IObjList> ListEditingStack { get; set; } = new CustomStack<IObjList>();

        public static ClipBoardItem StoredValue = null;
        public CustomStack<UndoAction> UndoList = new CustomStack<UndoAction>();
		
        public string CurListName
        {
            get {
                if (EditingList) return Constants.LinkedListName;
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

		public int SelectionCount => ObjectsListBox.SelectedItems.Count;

		public ILevelObj SelectedObj
		{
			get
			{
				return SelectionCount == 0 ? null : (ILevelObj)ObjectsListBox.SelectedItem;
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
				ObjectsListBox.SelectedItem = value;
			}
		}

		public ILevelObj[] SelectedObjs
        {
            get
            {
				if (SelectionCount > 0)
				{
					return ObjectsListBox.SelectedItems.Cast<ILevelObj>().ToArray();
				}
				else return new ILevelObj[0];
            }
        }

        bool EditingList { get { return ListEditingStack.Count != 0; } }

		public EditorForm(string[] args, ExtensionManifest[] Modules, ExtensionManifest selectedModule = null)
		{
			InitializeComponent();
#if DEBUG
			splitContainer2.Enabled = Debugger.IsAttached;
#else
			splitContainer2.Enabled = false;
#endif
			KeyPreview = true;

			var renderControl = new GL_ControlModern();
			renderControl.ActiveCamera = null;
			renderControl.BackColor = System.Drawing.Color.Black;
			renderControl.CameraDistance = -10F;
			renderControl.CamRotX = 0F;
			renderControl.CamRotY = 0F;
			renderControl.CurrentShader = null;
			renderControl.DragStartPos = new System.Drawing.Point(0, 0);
			renderControl.Fov = 0.7853982F;
			renderControl.MainDrawable = null;
			renderControl.Name = "gL_ControlModern1";
			renderControl.Dock = DockStyle.Fill;
			renderControl.Stereoscopy = false;
			renderControl.TabIndex = 1;
			renderControl.VSync = false;
			renderControl.ZFar = 1000F;
			renderControl.ZNear = 0.01F;
			splitContainer2.Panel2.Controls.Add(renderControl);

			//render.MouseMove += render_MouseMove;
			//render.PreviewMouseRightButtonUp += render_MouseRightButtonUp;
			//render.PreviewMouseRightButtonDown += render_MouseRightButtonDown;
			//render.MouseEnter += render_MouseEnter;
			//render.MouseLeftButtonDown += render_MouseLeftButtonDown;
			//render.MouseLeftButtonUp += render_MouseLeftButtonUp;
			//render.KeyDown += render_KeyDown;
			//render.KeyUp += render_KeyUP;
			//render.CameraInertiaFactor = Properties.Settings.Default.CameraInertia;
			//render.ShowFps = Properties.Settings.Default.ShowFps;
			//render.ShowTriangleCount = Properties.Settings.Default.ShowTriCount;
			//render.ShowDebugInfo = Properties.Settings.Default.ShowDbgInfo;
			//render.CamMode = Properties.Settings.Default.CameraMode == 0 ? HelixToolkit.Wpf.CameraMode.Inspect : HelixToolkit.Wpf.CameraMode.WalkAround;
			//render.ZoomSensitivity = Properties.Settings.Default.ZoomSen;
			//render.RotationSensitivity = Properties.Settings.Default.RotSen;

#if DEBUG
			if (Debugger.IsAttached) this.Text += " - Debugger.IsAttached";
			else this.Text += " - DEBUG BUILD";
#endif
			List<ExtensionManifest> ExtensionsWithGameModules = new List<ExtensionManifest>();
			LoadedModules = Modules;
			foreach (var module in Modules)
			{
				if (module.HasGameModule) ExtensionsWithGameModules.Add(module);
				//if (module.ClipboardExt != null) TODO
				//{
				//	if (module.ClipboardExt.PasteExtensions != null) RegisterMenuExt(ClipBoardMenu, module.ClipboardExt.PasteExtensions);
				//	if (module.ClipboardExt.CopyExtensions != null) RegisterMenuExtIndex(ClipBoardMenu, module.ClipboardExt.PasteExtensions);
				//}
				RegisterMenuExtension(module.MenuExt);
			}

			if (selectedModule != null)
			{
				GameModule = selectedModule.GetNewGameModule();
				SelectedGameModuleExtension = selectedModule;
			}
			else
			{

				if (ExtensionsWithGameModules.Count == 0) return;
				else if (ExtensionsWithGameModules.Count == 1)
				{
					GameModule = ExtensionsWithGameModules[0].GetNewGameModule();
					SelectedGameModuleExtension = ExtensionsWithGameModules[0];
				}
				else
				{
					var dlg = new OtherForms.GameModuleSelect(ExtensionsWithGameModules);
					dlg.ShowDialog();
					if (dlg.result == null) return;
					GameModule = dlg.result.GetNewGameModule();
					SelectedGameModuleExtension = dlg.result;
				}
				if (GameModule == null) return;

				Properties.Settings.Default.Add<string>($"{GameModule.ModuleName}_GamePath", "");
			}

			GameModule.InitModule(this);
			render = GameModule.GetRenderer();
			renderControl.MainDrawable = render;
			render.ClickSelection += Render_ClickSelection;
			render.ObjectMoved += Render_ObjectMoved;

			//TODO editingOptionsModule = GameModule as IEditingOptionsModule;
			//if (editingOptionsModule != null)
			//{
			//	editingOptionsModule.InitOptionsMenu(ref ObjectRightClickMenu);
			//}

			if (GameModule is IActionButtonsModule)
				((IActionButtonsModule)GameModule).InitActionButtons(ref toolStrip1);

			ModelsFolder = GameModule.ModelsFolder;
			CustomModelsFolder = "Custom" + GameModule.ModelsFolder;
			GameFolder = (string)Properties.Settings.Default[$"{GameModule.ModuleName}_GamePath"];

			IsAddListSupported = GameModule.IsAddListSupported;
			IsPropertyEditingSupported = GameModule.IsPropertyEditingSupported;
			PropertyGridTypes.CustomClassConverter.Clear();

			if (GameModule.GetClassConverters != null)
			foreach (var t in GameModule.GetClassConverters)
				PropertyGridTypes.CustomClassConverter.Add(t.Item1, t.Item2);

			FileOpenArgs = args;

		}

		private void Render_ClickSelection(ILevelObj item, bool MultiSelect, ref bool Cancel)
		{
			if (item == null)
			{
				SelectedObj = null;
				return;
			}
			if (MultiSelect && SelectedObj != null && !ObjectsListBox.Items.Contains(item))
			{
				Cancel = true;
				return;
			}
			else if (!MultiSelect || SelectedObj == null)
				SelectedObj = item;
			else
				ObjectsListBox.SelectedItems.Add(item);
		}

		private void Render_ObjectMoved(IReadOnlyList<ILevelObj> Selected) => propertyGrid1.Refresh();

		private void render_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (ActiveForm != this) return;
			render.Focus();
		}

		public void RegisterMenuExtension(IMenuExtension ext)
		{
			if (ext != null)
			{
				if (ext.FileMenuExtensions != null)
					RegisterMenuExtIndex(FileMenu, ext.FileMenuExtensions, FileMenu.DropDownItems.Count - 2); //last items are separator and settings
				if (ext.ToolsMenuExtensions != null)
					RegisterMenuExtIndex(ToolsMenu, ext.ToolsMenuExtensions);
				if (ext.TitleBarExtensions != null)
					RegisterMenuExtIndex(menuStrip1, ext.TitleBarExtensions, menuStrip1.Items.Count );
			}
		}

		public void RegisterClipBoardExt(ToolStripMenuItem item) => ClipBoardMenu.Items.Add(item);
		public void RegisterFindMenuStripExt(ToolStripMenuItem item) => FindMenu.DropDownItems.Add(item);
		public void RegisterMenuStripExt(ToolStripMenuItem item) => menuStrip1.Items.Add(item);
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

			if (FileOpenArgs != null)
				GameModule.ParseArgs(FileOpenArgs);
			//openToolStripMenuItem_Click(null, null);		
			this.Text += $" - {GameModule.ModuleName} module";
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e) => LoadLevel();

		void UnloadLevel()
        {
			this.Text = "EditorCore - " + GameModule.ModuleName;

			List<Form> ToClose = new List<Form>();
			foreach (Form frm in Application.OpenForms)
			{
				if (frm is IEditorChild && ((IEditorChild)frm).ParentEditor == this)
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

			if ((ModifierKeys & Keys.Control) == 0) //Skip unloading the models, this is useful to overlay a Design.szs to a Map.szs
				render.Clear();

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

			ListEditingPanel.Visible = false;

			GC.Collect();
        }

        public void LoadLevel(string path = null)
        {
			LoadLevel(GameModule.LoadLevel(path));
        }

		public void LoadLevel(ILevel lev)
		{
			if (lev == null) return;
			UnloadLevel();
			LoadedLevel = lev;
			this.Text = GameModule.ModuleName + " - " + lev.FilePath;

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
				LoadObjListModels(LoadedLevel.objs[k]);
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
			var Nlevel = GameModule.NewLevel();
			LoadLevel(Nlevel);
		}

        bool NoModels = false; //Debug only
   //     string GetModelName(string ObjName) //convert bfres to obj and cache in models folder
   //     {

			//if (Properties.Settings.Default.CustomModels)
			//{
			//	string name = $"{CustomModelsFolder}\\{ObjName}.obj";
			//	if (File.Exists(name)) return name;
			//}

			//if (SkipModels?.Contains(ObjName) ?? false) return null;

			//string CachedModelPath = $"{ModelsFolder}\\{ObjName}.obj";
			//if (render.LoadedModels.ContainsKey(CachedModelPath) || //The model has aready been loaded or has been converted
			//	File.Exists(CachedModelPath) ||
			//	(GameModule.ConvertModelFile(ObjName, ModelsFolder) && File.Exists(CachedModelPath)))
			//	return CachedModelPath;

			//SkipModels?.Add(ObjName);
   //         return null;
   //     }

        public void LoadObjListModels(IObjList list)
        {
            foreach (var obj in list)
            {
                AddModel(obj, list);
			}
		}

        public void AddModel(ILevelObj obj, IObjList list)
		{
#if DEBUG
			if (NoModels && Debugger.IsAttached)
				return;
#endif
			//TODO handle path
            render.Add(obj);
        }

        void PopulateListBox()
        {
            ObjectsListBox.Items.Clear();
            propertyGrid1.SelectedObject = null;
            foreach (var o in CurList) ObjectsListBox.Items.Add(o);
            ListEditingPanel.Visible = EditingList;
        }

		public void EditPath(IPathObj path)
		{
			ListEditingStack.Push(path);
			PopulateListBox();
			LoadObjListModels(path);
		}

		public void EditList(IObjList objlist)
		{
			UndoList.Clear();
			ListEditingStack.Push(objlist);
			PopulateListBox();
			SelectedObjectChanged(null, null);
			LoadObjListModels(objlist);
		}

		public void EditList(IList<dynamic> objList)
        {
            IObjList list = GameModule.CreateObjList(Constants.LinkedListName, objList);
			EditList(list);
		}

        public void PreviousList()
        {
            if (!EditingList) return;
			UndoList.Clear();
			foreach (var obj in CurList) render.Remove(obj);
            ListEditingStack.Pop().ApplyChanges();
            SelectedObjectChanged(null, null);
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
					//foreach (var i in SelectedObjs) TODO
					//	GameModule.UpdateMesh(i,CurList);
					ObjectsListBox.Refresh();
                }
                //foreach (var i in SelectedObjs)
                //{
                //    UpdateModelPosition(i);
                //}
            }
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

        private void Btn_CopyObjs_Click(object sender, EventArgs e)
        {
            btnPaste.Enabled = true;
            ClipBoardMenu_CopyFull_Click(null, null);
        }
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
			var stream = new MemoryStream();
			stream.Write(LoadedLevel.LevelFiles[name], 0, LoadedLevel.LevelFiles[name].Length);
			stream.Position = 0;
			if (!GameModule.OpenLevelFile(name, stream))
				OpenFileHandler.OpenFile(name, stream);
			if (stream.Length != 0)
				LoadedLevel.LevelFiles[name] = stream.ToArray();
		}

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (LoadedLevel == null) return;
#if DEBUG
			saveAsToolStripMenuItem_Click(sender, e); //Let's not risk modifing our precious dump
#else
			GameModule.SaveLevel(LoadedLevel);
#endif
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e) =>
			GameModule?.SaveLevelAs(LoadedLevel);

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

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e) => new Settings().ShowDialog();

        private void DuplicateSelectedObj_btn(object sender, EventArgs e) => DuplicateObj(SelectedObj, CurList);

		private void DuplicateSelectedObjs_btn(object sender, EventArgs e) => DuplicateObjs(SelectedObjs, CurList);
		private void btn_delObj_Click(object sender, EventArgs e)
        {
            var list = SelectedObjs.ToArray();
            foreach (var o in list) DeleteObj(o, CurList);
        }

		private void ddmAdd_Click(object sender, EventArgs e)
		{
			var obj = GameModule.NewObject();
			if (obj != null)
			{
				obj.ModelView_Pos = render.GetPositionInView();
				AddObj(obj, CurList);
			}
		}


		private void EditorForm_Activated(object sender, EventArgs e)
		{
			btnPaste.Enabled = (StoredValue != null && StoredValue.Type == ClipBoardItem.ClipboardType.Objects);
		}

		private void FormDragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
		}

		private void FormDragDrop(object sender, DragEventArgs e)
		{
			var files = (string[])e.Data.GetData(DataFormats.FileDrop);
			foreach (var file in files)
			{
				using (var s = new FileStream(file, FileMode.Open))
					OpenFileHandler.OpenFile(System.IO.Path.GetFileName(file), s);
			}
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
			var newProp = GameModule.GetNewProperty(propertyGrid1.SelectedGridItem.Value);
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

        private void ListEditGoBack(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PreviousList();
        }

        private void SelectedListChanged(object sender, EventArgs e) //comboBox1
        {
            if (EditingList) return;
			PopulateListBox();
            if (CurListName == null || CurListName == "" || CurList.Count == 0) return;
            HideGroup_CB.CheckedChanged -= HideGroup_CB_CheckedChanged; //Do not trigger event, unneeded (?)
            HideGroup_CB.Checked = CurList.IsHidden;
            HideGroup_CB.CheckedChanged += HideGroup_CB_CheckedChanged;
        }

        public void SelectedObjectChanged(object sender, EventArgs e) //ObjectsListBox
        {
			if (CurList.ReadOnly && SelectionCount != 0)
			{
				propertyGrid1.SelectedObject = null;
				SelectedObj = null;
				ObjectsListBox.SelectedIndex = -1;
				Console.Beep();
				return;
			}

            if (SelectionCount >= 1)
            {
                btnDuplicate.Visible = btnCopy.Enabled = true;

            }
            else
            {
                btnDuplicate.Visible = btnCopy.Enabled = false;
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
                    render.Remove(o);
                foreach (var o in SelectedObjs)
                    AddModel(o, CurList);
            }

			var selection = SelectedObjs.Cast<ILevelObj>().ToList();
			if (CurList is IPathObj) selection.Add((IPathObj)CurList);
			render.SetSelected(selection);
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

		private void btnEditChildren_Click(object sender, EventArgs e)
		{
			if (SelectedObj != null)
				GameModule.EditChildrenNode(SelectedObj);
		}
		#endregion
		

        void HandleHotKey(System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.B && EditingList) PreviousList();

			if (SelectionCount == 0) return;
			if (e.Key == Key.Space) render.LookAt(SelectedObj.ModelView_Pos);
			else if (e.Key == Key.OemPlus && ddmAdd.Enabled) ddmAdd_Click(null, null);
			else if (e.Key == Key.D && SelectionCount != 0) DuplicateObjs(SelectedObjs, CurList);
			else if (e.Key == Key.Delete) btn_delObj_Click(null, null);
			else if (e.Key == Key.F) FindMenu.ShowDropDown();
			else if (e.Key == Key.H) btnHideSelected_Click(null, null);
			else if (e.Key == Key.S) btnShowSelected_Click(null, null);
			else if (e.Key == Key.Z && UndoList.Count > 0) UndoList.Pop().Undo();
			//else if (e.Key == Key.Q) render.CamMode = render.CamMode == HelixToolkit.Wpf.CameraMode.Inspect ? HelixToolkit.Wpf.CameraMode.WalkAround : HelixToolkit.Wpf.CameraMode.Inspect;
			else if (e.Key == Key.C && SelectionCount == 1)
			{
				GameModule.EditChildrenNode(SelectedObj);
			}
#if DEBUG
			//else if (e.Key == Key.P)
			//	foreach (IObjList l in LoadedLevel.objs.Values)
			//		foreach (ILevelObj o in l) UpdateModelPosition(o);
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
                    render.Remove(o);
            }
            else
            {
                foreach (var o in list)
                    AddModel(o, list);
            }
        }

        public void AddObj(ILevelObj o, IObjList list)
        {
			if (list.ReadOnly)
				return;
            AddToUndo((dynamic) => InternalDeleteObj(o, list), "Added object: " + o.ToString());
			o.ID_int = ++LoadedLevel.HighestID;
			InternalAddObj(o, list);
        }

        void InternalAddObj(ILevelObj o, IObjList list)
        {
            list.Add(o);
            if (list == CurList)
            {
                ObjectsListBox.Items.Add(o);
            }
            if (!(list.name == Constants.LinkedListName && !EditingList))
                AddModel(o, list);
        }

        public void DuplicateObj(ILevelObj o, IObjList list)
        {
            if (o == null || list.ReadOnly) return;
            var newobj = (ILevelObj)o.Clone();
            AddObj(newobj, list);
        }

		public void DuplicateObjs(ILevelObj[] objs, IObjList list)
		{
			if (objs == null || objs.Length==0 || list.ReadOnly) return;

			ObjectsListBox.SelectedItems.Clear();
			foreach (var obj in objs)
			{
				var newobj = (ILevelObj)obj.Clone();
				InternalAddObj(newobj, list);
				ObjectsListBox.SelectedItems.Add(newobj);
			}
		}

		public void DeleteObj(ILevelObj o, IObjList list)
        {
            if (o == null || list.ReadOnly) return;
			AddToUndo((dynamic) =>
            InternalAddObj(o, list), "Deleted object: " + o.ToString());
            InternalDeleteObj(o, list);
        }

        public void InternalDeleteObj(ILevelObj o, IObjList list)
        {
            ObjectsListBox.SelectedIndex = -1;
            if (list == CurList)
                ObjectsListBox.Items.RemoveAt(CurList.IndexOf(o));

			render.Remove(o);
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
                MessageBox.Show("Select the path of the game, it will be used to display the models and load the levels.");
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
				preferencesToolStripMenuItem_Click(null, null);
				this.Focus();
            }
        }

		private void newEditorInstanceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new EditorForm(null, LoadedModules, SelectedGameModuleExtension).Show();
		}

		private void EditorForm_Closed(object sender, FormClosedEventArgs e)
		{
			Program.CheckForExit();
		}

        private void btnPaste_Click(object sender, EventArgs e)
        {
            if (StoredValue.Type == ClipBoardItem.ClipboardType.Objects)
            {
                Vector3 offset = new Vector3();
                if (SelectedObj != null)
                    offset = StoredValue.Objs.First().ModelView_Pos - SelectedObj.ModelView_Pos;
                ObjectsListBox.SelectedIndices.Clear();
				
                foreach (var o in StoredValue.Objs)
                {
                    ILevelObj copiedObj = (ILevelObj)o.Clone();
                    copiedObj.ModelView_Pos -= offset;
                    InternalAddObj(copiedObj, CurList);
                    ObjectsListBox.SelectedIndices.Add(CurList.IndexOf(copiedObj));
                }

                AddToUndo((dynamic args) =>
                {
                    var aArray = (dynamic[])args;
                    var listName = (string)aArray[0];
                    var selObjs = (ILevelObj[])aArray[1];
					
                    foreach (ILevelObj obj in selObjs)
						InternalDeleteObj(obj, LoadedLevel.objs[listName]);
					
                }, SelectionCount > 1 ? $"Pasted in {SelectionCount} objects" : $"Pasted in object {SelectedObj}", new dynamic[] { CurListName, SelectedObjs.Clone() });
            }
        }

		public IEditorFormContext NewInstance(params string[] args)
		{
			var inst = new EditorForm(args, LoadedModules, SelectedGameModuleExtension);
			inst.Show();
			return inst;
		}

		private void ObjectRightClickMenu_EditChildren_Click(object sender, EventArgs e)
		{
			GameModule.EditChildrenNode(ObjectRightClickMenu.Tag as ILevelObj);
		}

		private void ObjectRightClickMenu_CopyTransform_Click(object sender, EventArgs e)
		{
			StoredValue = new ClipBoardItem() { Type = ClipBoardItem.ClipboardType.Transform, transform = (ObjectRightClickMenu.Tag as ILevelObj).transform};
		}

		private void ObjectRightClickMenu_CopyPos_Click(object sender, EventArgs e)
		{
			StoredValue = new ClipBoardItem() { Type = ClipBoardItem.ClipboardType.Position,  transform = (ObjectRightClickMenu.Tag as ILevelObj).transform};
		}

		private void ObjectRightClickMenu_CopyRot_Click(object sender, EventArgs e)
		{
			StoredValue = new ClipBoardItem() { Type = ClipBoardItem.ClipboardType.Rotation, transform = (ObjectRightClickMenu.Tag as ILevelObj).transform };
		}

		private void ObjectRightClickMenu_CopyScale_Click(object sender, EventArgs e)
		{
			StoredValue = new ClipBoardItem() { Type = ClipBoardItem.ClipboardType.Scale, transform = (ObjectRightClickMenu.Tag as ILevelObj).transform };
		}

		private void ObjectRightClickMenu_Hide_Click(object sender, EventArgs e)
		{
			render.Remove(ObjectRightClickMenu.Tag as ILevelObj);
		}

		private void btnHideSelected_Click(object sender, EventArgs e)
		{
			if (CurList.IsHidden) return;
			foreach (var o in SelectedObjs)
				render.Remove(o);
		}

		private void btnShowSelected_Click(object sender, EventArgs e)
		{
			if (CurList.IsHidden) return;
			foreach (var o in SelectedObjs)
				AddModel(o, CurList);
		}

		private void btnShowAll_Click(object sender, EventArgs e)
		{
			foreach (var list in ListEditingStack)
			{
				if (list.IsHidden) continue;
				foreach (var o in list)
					AddModel(o, list);
			}
			foreach (var list in LoadedLevel.objs.Values)
			{
				if (list.IsHidden) continue;
				foreach (var o in list)
					AddModel(o, list);
			}
		}

		private void ObjectRightClickMenu_Copy_Click(object sender, EventArgs e)
		{
			ILevelObj[] objs = new ILevelObj[] { (ObjectRightClickMenu.Tag as ILevelObj).Clone() as ILevelObj };
			StoredValue = new ClipBoardItem() { Type = ClipBoardItem.ClipboardType.Objects, Objs = objs };
			btnPaste.Enabled = true;
		}

		private void ObjectRightClickMenu_Delete_Click(object sender, EventArgs e)
		{
			IObjList list = CurList;
			if (!CurList.Contains(ObjectRightClickMenu.Tag as ILevelObj))
			{
				list = LoadedLevel.FindListByObj(ObjectRightClickMenu.Tag as ILevelObj);
				if (list == null) return;
			}
			DeleteObj(ObjectRightClickMenu.Tag as ILevelObj, list);
		}

		private void EditorForm_Closing(object sender, FormClosingEventArgs e)
		{
			UnloadLevel();
		}
	}
}