using Syroot.BinaryData;
using Syroot.Maths;
using Syroot.NintenTools.MarioKart8.Collisions;

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
            self.Write(value.X);
            self.Write(value.Y);
            self.Write(value.Z);
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

        /// <summary>
        /// Writes a <see cref="Vector3F"/> instance into the current stream.
        /// </summary>
        /// <param name="self">The extended <see cref="BinaryDataWriter"/>.</param>
        /// <param name="value">The <see cref="Vector3F"/> instance.</param>
        internal static void Write(this BinaryDataWriter self, Vector3F value)
        {
            self.Write(value.X);
            self.Write(value.Y);
            self.Write(value.Z);
        }

        /// <summary>
        /// Writes <see cref="Vector3F"/> instances into the current stream.
        /// </summary>
        /// <param name="self">The extended <see cref="BinaryDataWriter"/>.</param>
        /// <param name="values">The <see cref="Vector3F"/> instances.</param>
        internal static void Write(this BinaryDataWriter self, Vector3F[] values)
        {
            foreach (Vector3F value in values)
            {
                Write(self, value);
            }
        }
    }
}
