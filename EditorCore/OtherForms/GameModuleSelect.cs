using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EditorCore.OtherForms
{
	public partial class GameModuleSelect : Form
	{
		public Interfaces.ExtensionManifest result = null;
		public GameModuleSelect(List<Interfaces.ExtensionManifest> Modules)
		{
			InitializeComponent();
			listBox1.Items.AddRange(Modules.ToArray());
		}

		private void button2_Click(object sender, EventArgs e)
		{
			result = (Interfaces.ExtensionManifest)listBox1.SelectedItem;
			this.Close();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			result = null;
			this.Close();
		}

		private void listBox1_DoubleClick(object sender, EventArgs e)
		{
			if (listBox1.SelectedItem != null)
				button2_Click(null, null);
		}
	}
}
