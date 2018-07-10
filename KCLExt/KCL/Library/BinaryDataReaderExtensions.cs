using Syroot.BinaryData;
using Syroot.Maths;
using Syroot.NintenTools.MarioKart8.Collisions;

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
                values[i] = new KclFace(self.ReadSingle(), self.ReadUInt16(), self.ReadUInt16(), self.ReadUInt16(),
                    self.ReadUInt16(), self.ReadUInt16(), self.ReadUInt16(), self.ReadUInt32());
            }
            return values;
        }

        /// <summary>
        /// Reads a <see cref="Vector3"/> instance from the current stream and returns it.
        /// </summary>
        /// <param name="self">The extended <see cref="BinaryDataReader"/>.</param>
        /// <returns>The <see cref="Vector3"/> instance.</returns>
        internal static Vector3 ReadVector3(this BinaryDataReader self)
        {
            return new Vector3(self.ReadInt32(), self.ReadInt32(), self.ReadInt32());
        }

        /// <summary>
        /// Reads <see cref="Vector3"/> instances from the current stream and returns them.
        /// </summary>
        /// <param name="self">The extended <see cref="BinaryDataReader"/>.</param>
        /// <param name="count">The number of instances to read.</param>
        /// <returns>The <see cref="Vector3"/> instances.</returns>
        internal static Vector3[] ReadVector3s(this BinaryDataReader self, int count)
        {
            Vector3[] values = new Vector3[count];
            for (int i = 0; i < count; i++)
            {
                values[i] = ReadVector3(self);
            }
            return values;
        }

        /// <summary>
        /// Reads a <see cref="Vector3F"/> instance from the current stream and returns it.
        /// </summary>
        /// <param name="self">The extended <see cref="BinaryDataReader"/>.</param>
        /// <returns>The <see cref="Vector3F"/> instance.</returns>
        internal static Vector3F ReadVector3F(this BinaryDataReader self)
        {
            return new Vector3F(self.ReadSingle(), self.ReadSingle(), self.ReadSingle());
        }

        /// <summary>
        /// Reads <see cref="Vector3F"/> instances from the current stream and returns them.
        /// </summary>
        /// <param name="self">The extended <see cref="BinaryDataReader"/>.</param>
        /// <param name="count">The number of instances to read.</param>
        /// <returns>The <see cref="Vector3F"/> instances.</returns>
        internal static Vector3F[] ReadVector3Fs(this BinaryDataReader self, int count)
        {
            Vector3F[] values = new Vector3F[count];
            for (int i = 0; i < count; i++)
            {
                values[i] = ReadVector3F(self);
            }
            return values;
        }
    }
}
