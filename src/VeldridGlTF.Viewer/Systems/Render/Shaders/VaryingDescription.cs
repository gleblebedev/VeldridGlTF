namespace VeldridGlTF.Viewer.Systems.Render.Shaders
{
    public class VaryingDescription
    {
        /// <summary>
        ///     The format of the element.
        /// </summary>
        public VaryingFormat Format;

        public int Location;

        /// <summary>
        ///     The name of the element.
        /// </summary>
        public string Name;

        public VaryingDescription(string name, VaryingFormat format)
        {
            Name = name;
            Format = format;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    /// <summary>
    ///     The format of an individual vertex element.
    /// </summary>
    public enum VaryingFormat : byte
    {
        /// <summary>
        ///     One 32-bit floating point value.
        /// </summary>
        Float1,

        /// <summary>
        ///     Two 32-bit floating point values.
        /// </summary>
        Float2,

        /// <summary>
        ///     Three 32-bit floating point values.
        /// </summary>
        Float3,

        /// <summary>
        ///     Four 32-bit floating point values.
        /// </summary>
        Float4,

        Mat3,

        Mat4
    }
}