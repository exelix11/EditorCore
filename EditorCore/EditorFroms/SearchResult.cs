using EditorCore.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EditorCore.EditorFroms
{
    public partial class SearchResult : Form, IEditorChild
    {
        public Tuple<IObjList, ILevelObj>[] SearchResultArr;
		public IEditorFormContext ParentEditor { get; set; }

		public SearchResult(Tuple<IObjList,ILevelObj>[] _sr, string title, IEditorFormContext _owner)
        {
            InitializeComponent();
            title = "Search result: " + title;
            SearchResultArr = _sr;
			ParentEditor = _owner;
        }

        private void SearchResult_Load(object sender, EventArgs e)
        {
            foreach (var res in SearchResultArr)
            {
                listBox1.Items.Add(res.Item2.ToString() + " in " + res.Item1.name);
            }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
				ParentEditor.SelectObject(SearchResultArr[listBox1.SelectedIndex].Item1, SearchResultArr[listBox1.SelectedIndex].Item2);
            } 
        }
    }
}
