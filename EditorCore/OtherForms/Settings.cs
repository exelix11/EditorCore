using ModelViewer;
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
    public partial class Settings : Form
    {
		List<RendererControl> renderList = new List<RendererControl>();

		public Settings()
		{
			InitializeComponent();

			foreach (var f in Application.OpenForms)
			{
				if (f is EditorForm)
					renderList.Add(((EditorForm)f).render);
			}

		}

        private void Settings_Load(object sender, EventArgs e)
        {
			drawDistance.Maximum = decimal.MaxValue;
			drawDistance.Value = double.IsInfinity(Properties.Settings.Default.FarPlaneDistance) ? 0 : (decimal)Properties.Settings.Default.FarPlaneDistance;
            SettingsPanel.Visible = true;
            CamInertiaUpDown.Value = (decimal)Properties.Settings.Default.CameraInertia;
            ChbFps.Checked = Properties.Settings.Default.ShowFps;
            ChbTriCount.Checked = Properties.Settings.Default.ShowTriCount;
            ChbDebugInfo.Checked = Properties.Settings.Default.ShowDbgInfo;
            cbCameraMode.SelectedIndex = Properties.Settings.Default.CameraMode;
            ZoomSenUpDown.Value = (decimal)Properties.Settings.Default.ZoomSen;
            RotSenUpDown.Value = (decimal)Properties.Settings.Default.RotSen;
            ChbStartupUpdate.Checked = Properties.Settings.Default.CheckUpdates;
            ChbStartupDb.Checked = Properties.Settings.Default.DownloadDb;
            tbUrl.Text = Properties.Settings.Default.DownloadDbLink;
			chbCustomModels.Checked = Properties.Settings.Default.CustomModels;
            SettingsPanel.Focus();
        }

        private void Form_closing(object sender, FormClosingEventArgs e)
		{
			Properties.Settings.Default.CameraInertia = (double)CamInertiaUpDown.Value;
			Properties.Settings.Default.ShowFps = ChbFps.Checked;
			Properties.Settings.Default.ShowTriCount = ChbTriCount.Checked;
			Properties.Settings.Default.ShowDbgInfo = ChbDebugInfo.Checked;
			Properties.Settings.Default.CameraMode = cbCameraMode.SelectedIndex;
			Properties.Settings.Default.ZoomSen = (double)ZoomSenUpDown.Value;
			Properties.Settings.Default.RotSen = (double)RotSenUpDown.Value;
			Properties.Settings.Default.FarPlaneDistance = drawDistance.Value == 0 ? double.PositiveInfinity : (double)drawDistance.Value;
			Properties.Settings.Default.CheckUpdates = ChbStartupUpdate.Checked;
			Properties.Settings.Default.DownloadDb = ChbStartupDb.Checked;
			Properties.Settings.Default.DownloadDbLink = tbUrl.Text;
			Properties.Settings.Default.CustomModels = chbCustomModels.Checked;
			Properties.Settings.Default.Save();

			foreach (var render in renderList)
			{
				render.CameraInertiaFactor = (double)CamInertiaUpDown.Value;
				render.ShowFps = ChbFps.Checked;
				render.ShowTriangleCount = ChbTriCount.Checked;
				render.ShowDebugInfo = ChbDebugInfo.Checked;
				render.CamMode = cbCameraMode.SelectedIndex == 0 ? HelixToolkit.Wpf.CameraMode.Inspect : HelixToolkit.Wpf.CameraMode.WalkAround;
				render.ZoomSensitivity = (double)ZoomSenUpDown.Value;
				render.RotationSensitivity = (double)RotSenUpDown.Value;
				render.FarPlaneDistance = Properties.Settings.Default.FarPlaneDistance;
			}
        }
    }
}
