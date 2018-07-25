using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EditorCore
{
	public partial class AddBymlPropertyDialog : Form
	{
		dynamic result = null;
		AddBymlPropertyDialog()
		{
			InitializeComponent();
		}

		private void AddPropertyDialog_Load(object sender, EventArgs e)
		{
			comboBox1.Items.AddRange(ByamlTypeHelper.StringToNodeTable.Keys.ToArray());
			comboBox1.Items.Add(typeof(Dictionary<string, dynamic>));
			comboBox1.Items.Add(typeof(List<dynamic>));
		}

		public static Tuple<string, dynamic> newProperty(bool enableName)
		{
			var dialog = new AddBymlPropertyDialog();
			dialog.textBox5.Enabled = enableName;
			dialog.ShowDialog();
			return dialog.result;
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if ((Type)comboBox1.SelectedItem == typeof(List<dynamic>) ||
				(Type)comboBox1.SelectedItem == typeof(Dictionary<string, dynamic>))
				textBox1.Enabled = false;
			else
				textBox1.Enabled = true;
		}

		private void radioButton1_CheckedChanged(object sender, EventArgs e)
		{
			panel1.Enabled = !radioButton1.Checked;
			panel2.Enabled = radioButton1.Checked;
		}

		private void button2_Click(object sender, EventArgs e)
		{
			result = null;
			this.Close();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			dynamic value;
			if (radioButton1.Checked)
			{
				if ((Type)comboBox1.SelectedItem == typeof(List<dynamic>)) value = new List<dynamic>();
				else if ((Type)comboBox1.SelectedItem == typeof(Dictionary<string, dynamic>)) value = new Dictionary<string, dynamic>();
				else
				{
					value = ByamlTypeHelper.ConvertValue((Type)comboBox1.SelectedItem, textBox1.Text);
				}
			}
			else
			{
				value = new Dictionary<string, dynamic>();
				value.Add("X",float.Parse(textBox2.Text));
				value.Add("Y", float.Parse(textBox3.Text));
				value.Add("Z", float.Parse(textBox4.Text));
			}
			result = new Tuple<string, dynamic>(textBox5.Enabled ? textBox5.Text : null, value);
			this.Close();
		}
	}

	public static class ByamlTypeHelper
	{
		public delegate dynamic ConvertMethod(string inString);
		public static readonly Dictionary<Type, ConvertMethod> StringToNodeTable = new Dictionary<Type, ConvertMethod>()
		{
			{ typeof(string) , (s) => s },
			{ typeof(int) , (s) => (int.Parse(s)) },
			{ typeof(uint) , (s) =>(uint.Parse(s)) },
			{ typeof(long) , (s) => (long.Parse(s)) },
			{ typeof(ulong) , (s) => (ulong.Parse(s)) },
			{ typeof(double) , (s) =>(double.Parse(s)) },
			{ typeof(float) , (s) => (float.Parse(s)) },
		};
		public static dynamic ConvertValue(Type t, string value) => StringToNodeTable[t](value);
	}
}
