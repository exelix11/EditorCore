using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using EveryFileExplorer;
using EditorCoreCommon;

namespace SARCExt
{
	public partial class SarcEditor : Form
	{
		public SarcData loadedSarc;
		public Stream sourceStream;
		public SarcEditor(SarcData sarc, Stream source = null)
		{
			InitializeComponent();
			loadedSarc = sarc;
			sourceStream = source;
			if (sourceStream == null)
				replaceToolStripMenuItem.Enabled = false;
		}

		private void SarcEditor_Load(object sender, EventArgs e)
		{
			if (loadedSarc == null)
			{
				MessageBox.Show("No sarc has been loaded");
				this.Close();
			}
			else
			{
				listBox1.Items.AddRange(loadedSarc.Files.Keys.ToArray());
			}
		}

		private void extractAllFilesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ExtractMultipleFiles(loadedSarc.Files.Keys.ToArray());
		}

		void ExtractMultipleFiles(IEnumerable<string> files)
		{
			var dlg = new FolderBrowserDialog();
			if (dlg.ShowDialog() != DialogResult.OK)
				return;
			foreach (string f in files)
			{
				File.WriteAllBytes(Path.Combine(dlg.SelectedPath, f), loadedSarc.Files[f]);
			}
		}

		private void extractToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (listBox1.SelectedItems.Count > 1)
				ExtractMultipleFiles(listBox1.SelectedItems.Cast<string>());
			else
			{
				var sav = new SaveFileDialog() { FileName = listBox1.SelectedItem.ToString()};
				if (sav.ShowDialog() != DialogResult.OK)
					return;
				File.WriteAllBytes(sav.FileName, loadedSarc.Files[listBox1.SelectedItem.ToString()]);
			}
		}

		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (loadedSarc.HashOnly)
			{
				MessageBox.Show("Can't remove files from a hash only sarc");
				return;
			}
			foreach (var item in listBox1.SelectedItems.Cast<string>())
				loadedSarc.Files.Remove(item);
			listBox1.Items.Clear();
			listBox1.Items.AddRange(loadedSarc.Files.Keys.ToArray());
		}

		private void addFilesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (loadedSarc.HashOnly)
			{
				MessageBox.Show("Can't add files to a hash only sarc");
				return;
			}
			var opn = new OpenFileDialog() { Multiselect = true };
			if (opn.ShowDialog() != DialogResult.OK)
				return;
			foreach (var f in opn.FileNames)
			{
				string name = Path.GetFileName(f);
				if (EditorCore.InputDialog.Show("File name", "Write the name for this file, use / to place it in a folder", ref name) != DialogResult.OK)
					return;

				if (loadedSarc.Files.ContainsKey(name))
				{
					MessageBox.Show($"File {name} already in szs");
					continue;
				}
				loadedSarc.Files.Add(name, File.ReadAllBytes(f));
			}
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var sav = new SaveFileDialog() { Filter = "szs file|*.szs|sarc file|*.sarc" };
			if (sav.ShowDialog() != DialogResult.OK)
				return;
			if (numericUpDown1.Value == 0)
				File.WriteAllBytes(sav.FileName, SARC.PackN(loadedSarc).Item2);
			else
			{
				var s = SARC.PackN(loadedSarc);
				File.WriteAllBytes(sav.FileName, YAZ0.Compress(s.Item2, (int)numericUpDown1.Value,(uint)s.Item1));
			}
		}

		private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			byte[] compressedSarc = null;
			var s = SARC.PackN(loadedSarc);
			if (numericUpDown1.Value != 0)
				compressedSarc = YAZ0.Compress(s.Item2, (int)numericUpDown1.Value,(uint)s.Item1);
			else
				compressedSarc = s.Item2;
			//sourceStream.Position = 0;
			sourceStream.Write(compressedSarc, 0, compressedSarc.Length);
		}

		private void ListBoxDoubleClick(object sender, EventArgs e)
		{
			if (listBox1.SelectedItem != null)
				OpenFileHandler.OpenFile(listBox1.SelectedItem.ToString(), 
					new MemoryStream(loadedSarc.Files[listBox1.SelectedItem.ToString()]));
		}

		private void replaceToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			if (listBox1.SelectedItem == null) return;
			var opn = new OpenFileDialog();
			if (opn.ShowDialog() != DialogResult.OK) return;
			loadedSarc.Files[listBox1.SelectedItem.ToString()] = File.ReadAllBytes(opn.FileName);
		}

		private void copyNameToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (listBox1.SelectedItem == null) return;
			Clipboard.SetText(listBox1.SelectedItem.ToString());
		}
	}
}
