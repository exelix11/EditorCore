using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace EditorCore.Interfaces
{
	public interface IPathObj : IList<ILevelObj>, ILevelObj
	{
		Point3D[] Points { get; set; }
	}
}
