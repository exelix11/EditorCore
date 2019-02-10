using OpenTK;
using Syroot.BinaryData;
using Syroot.NintenTools.MarioKart8.Collisions;
using System.Windows.Media.Media3D;

namespace Syroot.NintenTools.MarioKart8.IO
{
    /// <summary>
    /// Represents extension methods for <see cref="BinaryDataWriter"/> instances.
    /// </summary>
    internal static class BinaryDataWriterExtensions
    {
        // ---- METHODS (INTERNAL) -------------------------------------------------------------------------------------

        /// <summary>
        /// Writes <see cref="KclFace"/> instances into the current stream.
        /// </summary>
        /// <param name="self">The extended <see cref="BinaryDataWriter"/>.</param>
        /// <param name="values">The <see cref="KclFace"/> instances.</param>
        internal static void Write(this BinaryDataWriter self, KclFace[] values)
        {
            foreach (KclFace value in values)
            {
                self.Write(value.Length);
                self.Write(value.PositionIndex);
                self.Write(value.DirectionIndex);
                self.Write(value.Normal1Index);
                self.Write(value.Normal2Index);
                self.Write(value.Normal3Index);
                self.Write(value.CollisionFlags);
                self.Write(value.GlobalIndex);
            }
        }

        /// <summary>
        /// Writes a <see cref="Vector3"/> instance into the current stream.
        /// </summary>
        /// <param name="self">The extended <see cref="BinaryDataWriter"/>.</param>
        /// <param name="value">The <see cref="Vector3"/> instance.</param>
        internal static void Write(this BinaryDataWriter self, Vector3 value)
        {
            self.Write((float)value.X);
            self.Write((float)value.Y);
            self.Write((float)value.Z);
        }

        /// <summary>
        /// Writes <see cref="Vector3"/> instances into the current stream.
        /// </summary>
        /// <param name="self">The extended <see cref="BinaryDataWriter"/>.</param>
        /// <param name="values">The <see cref="Vector3"/> instances.</param>
        internal static void Write(this BinaryDataWriter self, Vector3[] values)
        {
            foreach (Vector3 value in values)
            {
                Write(self, value);
            }
		}
		
    }
}
