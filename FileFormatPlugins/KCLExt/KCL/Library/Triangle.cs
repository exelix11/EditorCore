using Syroot.Maths;

namespace Syroot.NintenTools.MarioKart8
{
    /// <summary>
    /// Represents a polygon in 3-dimensional space, defined by 3 vertices storing their positions.
    /// </summary>
    internal class Triangle
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the vertices which store the corner positions of the triangle.
        /// </summary>
        internal Vector3F[] Vertices;

		// ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance of the <see cref="Triangle"/> class.
		/// </summary>
		internal Triangle()
		{
			Vertices = new Vector3F[3];
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Triangle"/> class.
		/// </summary>
		internal Triangle(params Vector3F[] vec)
        {
			Vertices = vec;
		}

		// ---- PROPERTIES ---------------------------------------------------------------------------------------------

		/// <summary>
		/// Gets the face normal of this triangle.
		/// </summary>
		public Vector3F Normal
		{
			get
			{
				Vector3F a = (Vertices[1] - Vertices[0]).Cross(Vertices[2] - Vertices[0]);
				return a / (float)System.Math.Sqrt(a.X * a.X + a.Y * a.Y + a.Z * a.Z);
			}
		}
	}
}
