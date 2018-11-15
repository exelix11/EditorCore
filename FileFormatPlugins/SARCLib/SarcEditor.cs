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
			string[] Targets = listBox1.SelectedItems.Cast<string>().ToArray();
			foreach (var item in Targets) 
			{
				loadedSarc.Files.Remove(item);
				listBox1.Items.Remove(item);
			}
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
				listBox1.Items.Add(name);
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
			if (listBox1.SelectedItem == null)
				return;
			var Fname = listBox1.SelectedItem.ToString();
			var stream = new EditableStream(loadedSarc.Files[Fname]); //This should work with just a memoryStream but a custom class lets us know if something has been written to it.
			OpenFileHandler.OpenFile(Fname, stream);
			if (stream.HasBeenEdited)
				loadedSarc.Files[Fname] = stream.ToArray();
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

		private void renameToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (listBox1.SelectedItem == null) return;
			string originalName = listBox1.SelectedItem.ToString();
			string name = Path.GetFileName(originalName);
			if (EditorCore.InputDialog.Show("File name", "Write the name for this file, use / to place it in a folder", ref name) != DialogResult.OK)
				return;

			if (loadedSarc.Files.ContainsKey(name))
			{
				MessageBox.Show($"File {name} already in szs");
				return;
			}
			loadedSarc.Files.Add(name, loadedSarc.Files[originalName]);
			loadedSarc.Files.Remove(originalName);
			listBox1.Items.Add(name);
			listBox1.Items.Remove(originalName);
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{

		}
	}

	class EditableStream : Stream
	{
		MemoryStream _mem = new MemoryStream();
		public bool HasBeenEdited;

		public byte[] ToArray() => _mem.ToArray();

		public EditableStream(byte[] data)
		{
			_mem.Write(data, 0, data.Length);
		}

		public override bool CanRead => _mem.CanRead;
		public override bool CanSeek => _mem.CanSeek;
		public override bool CanWrite => _mem.CanWrite;
		public override long Length => _mem.Length;
		public override long Position { get => _mem.Position; set => _mem.Position = value; }
		public override void Flush() => _mem.Flush();
		public override int Read(byte[] buffer, int offset, int count) =>
			_mem.Read(buffer, offset, count);

		public override long Seek(long offset, SeekOrigin origin) =>
			_mem.Seek(offset, origin);

		public override void SetLength(long value) =>
			_mem.SetLength(value);

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (count != 0) HasBeenEdited = true;
			_mem.Write(buffer, offset, count);
		}
	}
}
