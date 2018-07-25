using Syroot.BinaryData;
using Syroot.NintenTools.MarioKart8.Collisions;
using System.Windows.Media.Media3D;

namespace Syroot.NintenTools.MarioKart8.IO
{
    /// <summary>
    /// Represents extension methods for <see cref="BinaryDataReader"/> instances.
    /// </summary>
    internal static class BinaryDataReaderExtensions
    {
        // ---- METHODS (INTERNAL) -------------------------------------------------------------------------------------
        
        /// <summary>
        /// Reads <see cref="KclFace"/> instances from the current stream and returns them.
        /// </summary>
        /// <param name="self">The extended <see cref="BinaryDataReader"/>.</param>
        /// <param name="count">The number of instances to read.</param>
        /// <returns>The <see cref="KclFace"/> instances.</returns>
        internal static KclFace[] ReadTriangles(this BinaryDataReader self, int count)
        {
            KclFace[] values = new KclFace[count];
            for (int i = 0; i < count; i++)
            {
				values[i] = new KclFace()
				{
					Length = self.ReadSingle(),
					PositionIndex = self.ReadUInt16(),
					DirectionIndex = self.ReadUInt16(),
					Normal1Index = self.ReadUInt16(),
					Normal2Index = self.ReadUInt16(),
					Normal3Index = self.ReadUInt16(),
					CollisionFlags = self.ReadUInt16(),
					GlobalIndex = self.ReadUInt32()
				};
			}
            return values;
        }

        /// <summary>
        /// Reads a <see cref="Vector3D"/> instance from the current stream and returns it.
        /// </summary>
        /// <param name="self">The extended <see cref="BinaryDataReader"/>.</param>
        /// <returns>The <see cref="Vector3D"/> instance.</returns>
        internal static Vector3D ReadVector3D(this BinaryDataReader self)
        {
            return new Vector3D(self.ReadSingle(), self.ReadSingle(), self.ReadSingle());
        }

        /// <summary>
        /// Reads <see cref="Vector3D"/> instances from the current stream and returns them.
        /// </summary>
        /// <param name="self">The extended <see cref="BinaryDataReader"/>.</param>
        /// <param name="count">The number of instances to read.</param>
        /// <returns>The <see cref="Vector3D"/> instances.</returns>
        internal static Vector3D[] ReadVector3Ds(this BinaryDataReader self, int count)
        {
            Vector3D[] values = new Vector3D[count];
            for (int i = 0; i < count; i++)
            {
                values[i] = ReadVector3D(self);
            }
            return values;
        }
    }
}
