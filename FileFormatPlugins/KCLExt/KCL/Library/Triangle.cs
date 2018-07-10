﻿using Syroot.Maths;

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

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the face normal of this triangle.
        /// </summary>
        internal Vector3F Normal
        {
            get
            {
                return (Vertices[1] - Vertices[0]).Cross(Vertices[2] - Vertices[0]).Normalized();
            }
        }
    }
}