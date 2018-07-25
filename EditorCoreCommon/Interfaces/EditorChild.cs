using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorCore.Interfaces
{
	public interface IEditorChild
	{
		IEditorFormContext ParentEditor { get; set; }
	}
}
