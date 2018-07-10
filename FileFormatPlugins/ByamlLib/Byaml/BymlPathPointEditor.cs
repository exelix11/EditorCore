using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ByamlExt
{
	public partial class BymlPathPointEditor : Form
	{
		dynamic target;
		public BymlPathPointEditor(dynamic _target)
		{
			InitializeComponent();
			target = _target;
			propertyGrid1.SelectedObject = target;
		}

		private void BymlPathPointEditor_Load(object sender, EventArgs e)
		{

		}
	}
}
