using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorCore.Interfaces
{
	public interface IPathObj : ILevelObj, IObjList
	{
		Vector3[] Points { get; }
	}
}
