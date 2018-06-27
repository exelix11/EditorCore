using EditorCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MK8DExt
{
	class MK8DExt : ExtensionManifest
	{
		public string ModuleName => "MK8DExt";
		public string Author => "Exelix11";
		public string ThanksTo => "KillzXGaming for the C# BFRES loader\r\ngdkchan for Bntxx";

		public Version TargetVersion => new Version(1, 0, 0, 0);

		public IMenuExtension MenuExt => null;

		public IClipboardExtension ClipboardExt => null;

		public bool HasGameModule => true;
		MK8DModule _module = null;
		public IGameModule GameModule
		{
			get
			{
				if (_module == null) _module = new MK8DModule();
				return _module;
			}
		}
	}
}
