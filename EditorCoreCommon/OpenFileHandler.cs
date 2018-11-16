using EditorCore.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorCoreCommon
{
	public static class OpenFileHandler
	{
		public static List<IFileHander> handlers = new List<IFileHander>();

		public static void OpenFile(string filename, Stream FileStream, int BasePositionInStream = 0)
		{
			foreach (var h in handlers)
			{
				FileStream.Position = BasePositionInStream;
				if (h.IsFormatSupported(filename, FileStream))
				{
					FileStream.Position = BasePositionInStream;
					h.OpenFile(filename, FileStream);
					break;
				}
			}
		}

		public static void OpenFileEditable(string filename, Stream FileStream, Stream SaveStream, int BasePositionInStream = 0)
		{
			foreach (var h in handlers)
			{
				FileStream.Position = BasePositionInStream;
				if (h.IsFormatSupported(filename, FileStream))
				{
					FileStream.Position = BasePositionInStream;
					if (h is IEditableFileHandler)
						((IEditableFileHandler)h).OpenFileEdit(filename, FileStream, SaveStream);
					else
						h.OpenFile(filename, FileStream);
					break;
				}
			}
		}
	}
}
