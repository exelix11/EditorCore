using ByamlExt.Byaml;
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
using Syroot.BinaryData;
using EditorCore;

namespace ByamlExt
{
    public partial class ByamlViewer : Form
    {
		public ByteOrder byteOrder;
		public dynamic byml;
		public string FileName = "";
		bool pathSupport;
		ushort bymlVer;
		public ByamlViewer(System.Collections.IEnumerable by, bool _pathSupport, ushort _ver, ByteOrder defaultOrder = ByteOrder.LittleEndian, string name = "")
        {
            InitializeComponent();
			byteOrder = defaultOrder;
			FileName = name;
			byml = by;
			pathSupport = _pathSupport;
			bymlVer = _ver;

			if (byml == null) return;
			//the first node should always be a dictionary node
			if (byml is Dictionary<string, dynamic>)
			{
				parseDictNode(byml, treeView1.Nodes);
			}
			else if (byml is List<dynamic>)
			{
				if (((List<dynamic>)byml).Count == 0)
				{
					MessageBox.Show("This byml is empty");
				}
				parseArrayNode(byml, treeView1.Nodes);
			}
			else if (byml is List<ByamlPathPoint>)
			{
				MessageBox.Show("Unsupported root node");
			}
			else throw new Exception($"Unsupported root node type {by.GetType()}");
        }

		Stream saveStream = null;
		public ByamlViewer(System.Collections.IEnumerable by, bool _pathSupport, Stream saveTo, ushort _ver , ByteOrder defaultOrder = ByteOrder.LittleEndian) : this(by, _pathSupport,_ver,defaultOrder)
		{
			saveStream = saveTo;
			saveToolStripMenuItem.Visible = true;
		}

		//get a reference to the value to change
		class EditableNode
		{
			public Type type { get => Node[Index].GetType(); }
			dynamic Node;
			dynamic Index;

			public dynamic Get() => Node[Index];
			public void Set(dynamic value) => Node[Index] = value;
			public string GetTreeViewString()
			{
				if (Index is int)
					return Node[Index].ToString();
				else
					return Index +" : " + Node[Index].ToString();
			}

			public EditableNode(dynamic _node, dynamic _index)
			{
				Node = _node;
				Index = _index;
			}
		}
        
        void parseDictNode(IDictionary<string, dynamic> node, TreeNodeCollection addto)
        {
            foreach (string k in node.Keys)
            {
                TreeNode current = addto.Add(k);
                if (node[k] is IDictionary<string, dynamic>)
                {
                    current.Text += " : <Dictionary>";
                    current.Tag = node[k]; 
                    current.Nodes.Add("✯✯dummy✯✯"); //a text that can't be in a byml
                }
                else if (node[k] is IList<dynamic>)
                {
                    current.Text += " : <Array>";
                    current.Tag = ((IList<dynamic>)node[k]);
                    current.Nodes.Add("✯✯dummy✯✯");
				}
				else if (node[k] is IList<ByamlPathPoint>)
				{
					current.Text += " : <PathPointArray>";
					current.Tag = ((IList<ByamlPathPoint>)node[k]);
					parsePathPointArray(node[k], current.Nodes);
				}
				else
                {
                    current.Text = current.Text + " : " + (node[k] == null  ? "<NULL>" : node[k].ToString());
					if (node[k] != null) current.Tag = new EditableNode(node,k);
				}
            }
        }

		void parsePathPointArray(IList<ByamlPathPoint> list, TreeNodeCollection addto)
		{
			int index = 0;
			foreach (var k in list)
			{
				index++;
				var n = addto.Add(k == null ? "<NULL>" : k.ToString());
				if (k != null) n.Tag = new EditableNode(list, index);
			}
		}

        void parseArrayNode(IList<dynamic> list, TreeNodeCollection addto)
        {
			int index = 0;
            foreach (dynamic k in list)
            {
				if (k is IDictionary<string, dynamic>)
                {
                    TreeNode current = addto.Add("<Dictionary>");
                    current.Tag = ((IDictionary<string, dynamic>)k);
                    current.Nodes.Add("✯✯dummy✯✯");
                }
                else if (k is IList<dynamic>)
                {
                    TreeNode current = addto.Add("<Array>");
                    current.Tag = ((IList<dynamic>)k);
                    current.Nodes.Add("✯✯dummy✯✯");
				}
				else if (k is IList<ByamlPathPoint>)
				{
					TreeNode current = addto.Add("<PathPointArray>");
					current.Tag = ((IList<ByamlPathPoint>)k);
					parsePathPointArray(k, current.Nodes);
				}
				else
                {
					var n = addto.Add(k == null ? "<NULL>" : k.ToString());
					if (k != null) n.Tag = new EditableNode(list, index);
				}
				index++;
			}
        }

        private void BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Tag != null && e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Text == "✯✯dummy✯✯")
            {
                e.Node.Nodes.Clear();
                if (((dynamic)e.Node.Tag).Count == 0)
                {
                    e.Node.Nodes.Add("<Empty>");
                    return;
                }
                if (e.Node.Tag is IList<dynamic>) parseArrayNode((IList<dynamic>)e.Node.Tag, e.Node.Nodes);
                else if (e.Node.Tag is IDictionary<string, dynamic>) parseDictNode((IDictionary<string, dynamic>)e.Node.Tag, e.Node.Nodes);
                else throw new Exception("WTF");
            }
        }

        private void ContextMenuOpening(object sender, CancelEventArgs e)
        {
            CopyNode.Enabled = treeView1.SelectedNode != null;
			editValueNodeMenuItem.Enabled = treeView1.SelectedNode != null && treeView1.SelectedNode.Tag is EditableNode;
		}

        private void CopyNode_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(treeView1.SelectedNode.Text);
        }

        private void ByamlViewer_Load(object sender, EventArgs e)
        {

        }

        private void exportJsonToolStripMenuItem_Click(object sender, EventArgs e)
        {
			SaveFileDialog sav = new SaveFileDialog() {	Filter = "Xml file | *.xml"	};
			if (sav.ShowDialog() != DialogResult.OK) return;
			File.WriteAllText(sav.FileName, 
				XmlConverter.ToXml(new BymlFileData { Version = bymlVer, byteOrder = byteOrder, SupportPaths = pathSupport, RootNode = byml }));
        }

        public static void ImportFromJson()
        {
			
        }

        public static void OpenByml()
        {
            OpenFileDialog opn = new OpenFileDialog();
            opn.Filter = "byml file | *.byml";
            if (opn.ShowDialog() == DialogResult.OK)
            {
                OpenByml(opn.FileName);
            }
        }

		static bool SupportPaths()
		{
			return MessageBox.Show("Does this game support paths ?", "", MessageBoxButtons.YesNo) == DialogResult.Yes;
		}

		public static void OpenByml(string Filename) =>
			OpenByml(new FileStream(Filename, FileMode.Open), Filename);

		public static void OpenByml(Stream file, string FileName = "") =>
			OpenByml(file, FileName, SupportPaths());

		public static void OpenByml(Stream file, string FileName, bool paths)
		{
			var byml = ByamlFile.LoadN(file, paths);
			new ByamlViewer(byml.RootNode, paths, byml.Version, byml.byteOrder, FileName).Show();
		}

		public static void OpenByml(BymlFileData data, string filename) =>
			new ByamlViewer(data.RootNode, data.SupportPaths, data.Version, data.byteOrder, filename).Show();

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sav = new SaveFileDialog() { FileName = FileName, Filter = "byml file | *.byml" };
            if (sav.ShowDialog() == DialogResult.OK)
            {
                ByamlFile.SaveN(sav.FileName, 
					new BymlFileData {  Version = bymlVer, byteOrder = byteOrder, SupportPaths = pathSupport, RootNode = byml });
            }
        }

		private void editValueNodeMenuItem_Click(object sender, EventArgs e)
		{
			var node = treeView1.SelectedNode.Tag as EditableNode;
			if (node == null) return;

			if (node.Get() is ByamlPathPoint)
			{
				new BymlPathPointEditor(node.Get()).ShowDialog(); //ByamlPathPoint is a reference type
			}
			else
			{
				string value = node.Get().ToString();
				var dRes = InputDialog.Show("Enter value", $"Enter new value for the node, the value must be of type {node.type}", ref value);
				if (dRes != DialogResult.OK) return;
				if (value.Trim() == "") return;
				node.Set(ByamlTypeHelper.ConvertValue(node.type, value));
			}
			treeView1.SelectedNode.Text = node.GetTreeViewString();
		}

		private void addNodeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			dynamic target = treeView1.SelectedNode == null ? byml : treeView1.SelectedNode.Tag;

			var targetNodeCollection = treeView1.SelectedNode == null ? treeView1.Nodes : treeView1.SelectedNode.Nodes;
			
			if (target is EditableNode)
			{
				if (treeView1.SelectedNode.Parent == null)
				{
					target = byml;
					targetNodeCollection = treeView1.Nodes;
				}
				else
				{
					target = treeView1.SelectedNode.Parent.Tag;
					targetNodeCollection = treeView1.SelectedNode.Parent.Nodes;
				}
			}

			var newProp = AddBymlPropertyDialog.newProperty(!(target is IList<dynamic>));
			if (newProp == null) return;
			bool clone = newProp.Item2 is IDictionary<string, dynamic> || newProp.Item2 is IList<dynamic>; //reference types must be manually cloned
			var toAdd = clone ? DeepCloneDictArr.DeepClone(newProp.Item2) : newProp.Item2;

			targetNodeCollection.Clear();

			if (target is IList<dynamic>)
			{
				((IList<dynamic>)target).Insert(((IList<dynamic>)target).Count,toAdd);
				parseArrayNode((IList<dynamic>)target, targetNodeCollection);
			}
			else if (target is IDictionary<string, dynamic>)
			{
				((IDictionary<string, dynamic>)target).Add(newProp.Item1, toAdd);
				parseDictNode((IDictionary<string, dynamic>)target, targetNodeCollection);
			}
			else throw new Exception();

		}

		private void deleteNodeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode == null)
			{
				MessageBox.Show("Select a node first");
				return;
			}

			dynamic target;
			TreeNodeCollection targetNode;
			if (treeView1.SelectedNode.Parent == null)
			{
				target = byml;
				targetNode = treeView1.Nodes;
			}
			else
			{
				target = treeView1.SelectedNode.Parent.Tag;
				targetNode = treeView1.SelectedNode.Parent.Nodes;
			}
			int index = targetNode.IndexOf(treeView1.SelectedNode);
			if (target is Dictionary<string, dynamic>)
			{
				target.Remove(((Dictionary<string, dynamic>)target).Keys.ToArray()[index]);
			}
			else
				target.RemoveAt(targetNode.IndexOf(treeView1.SelectedNode));
			targetNode.RemoveAt(index);
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			saveStream.Position = 0;
			saveStream.SetLength(0);
			ByamlFile.SaveN(saveStream, new BymlFileData { Version = bymlVer, byteOrder = byteOrder, SupportPaths = pathSupport, RootNode = byml });
		}
	}
}
