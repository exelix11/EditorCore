using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KCLExt
{
	public partial class MaterialSetForm : Form
	{
		Dictionary<string, ushort> Result;

		private MaterialSetForm(string[] mats)
		{
			InitializeComponent();
			foreach (string s in mats)
				dataGridView1.Rows.Add(s, 0);
		}

		public static Dictionary<string, ushort> ShowForm(string[] Materials)
		{
			MaterialSetForm f = new MaterialSetForm(Materials);
			f.ShowDialog();
			return f.Result;
		}

		private void FClosing(object sender, FormClosingEventArgs e)
		{
			Result = new Dictionary<string, ushort>();
			for (int i = 0; i < dataGridView1.Rows.Count; i++)
			{
				var v = dataGridView1[1, i].Value.ToString();
				Result.Add(dataGridView1[0, i].Value.ToString(), v == "-1" ? ushort.MaxValue : ushort.Parse(v));
			}
		}
	}
}
