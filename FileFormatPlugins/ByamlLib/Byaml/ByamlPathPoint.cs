using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Syroot.Maths;

namespace ByamlExt.Byaml
{
    /// <summary>
    /// Represents a point in a BYAML path.
    /// </summary>
    public class ByamlPathPoint : IEquatable<ByamlPathPoint>
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        /// <summary>
        /// The size of a single point in bytes when serialized as BYAML data.
        /// </summary>
        internal const int SizeInBytes = 28;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="ByamlPathPoint"/> class.
        /// </summary>
        public ByamlPathPoint()
        {
            Normal = new Vector3F(0, 1, 0);
        }

		// ---- PROPERTIES ---------------------------------------------------------------------------------------------

		/// <summary>
		/// Gets or sets the location.
		/// </summary>
		[TypeConverter(typeof(Vector3FConverter))]
		public Vector3F Position { get; set; }

		/// <summary>
		/// Gets or sets the normal.
		/// </summary>
		[TypeConverter(typeof(Vector3FConverter))]
		public Vector3F Normal { get; set; }

        /// <summary>
        /// Gets or sets an unknown value.
        /// </summary>
        public uint Unknown { get; set; }

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(ByamlPathPoint other)
        {
            return Position == other.Position && Normal == other.Normal && Unknown == other.Unknown;
        }

		public override string ToString()
		{
			return $"ByamlPathPoint Pos:{Position} Norm:{Normal} Unk:{Unknown}";
		}

		class Vector3FConverter : System.ComponentModel.TypeConverter
		{
			public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, Type sourceType)
			{
				return sourceType == typeof(string);
			}

			public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
			{
				Vector3F res;
				string[] tokens = ((string)value).Split(';');
				res.X = Single.Parse(tokens[0]);
				res.Y = Single.Parse(tokens[1]);
				res.Z = Single.Parse(tokens[2]);
				return res;
			}

			public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
			{
				Vector3F val = (Vector3F)value;
				return $"{val.X};{val.Y};{val.Z}";
			}
		}
	}
}
