using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using EditorCore.Interfaces;
using EditorCoreCommon;

namespace EditorCore
{
    static class Program
    {
        /// <summary>
        /// Punto di ingresso principale dell'applicazione.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            System.IO.Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			if (Properties.Settings.Default.UpgradeSettings)
            { 
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpgradeSettings = false;
                Properties.Settings.Default.Save();
            }

			string[] ExtDlls = Directory.GetFiles("Ext", "*Ext.dll");
			List<ExtensionManifest> extensions = new List<ExtensionManifest>();
			foreach (string file in ExtDlls)
			{
				System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFrom(file);
				foreach (Type type in assembly.GetTypes())
				{
					Type typeExample = type.GetInterface("ExtensionManifest");
					if (typeExample == null) continue;
					
					var ext = assembly.CreateInstance(type.FullName) as ExtensionManifest;
					if (ext != null)
					{
						extensions.Add(ext);
						if (ext.Handlers != null)
							OpenFileHandler.handlers.AddRange(ext.Handlers);
					}
				}
			}
			
			if (Properties.Settings.Default.CheckUpdates)
				foreach (var m in extensions) m.CheckForUpdates();

			var firstForm = new EditorForm(args, extensions.ToArray());
			firstForm.Show();

			Timer ApplicationExitCheck = new Timer()
			{
				Enabled = true,
				Interval = 30000,
			};
			ApplicationExitCheck.Tick += delegate (object sender, EventArgs e) { CheckForExit(); };
			ApplicationExitCheck.Start(); //This is needed because closing every form won't close the application

			Application.Run();            
        }

		public static void CheckForExit()
		{
			if (Application.OpenForms.Count == 0)
				Environment.Exit(0);
		}
    }	
}
