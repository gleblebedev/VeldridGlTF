using System;
using System.Collections.Generic;
using System.Text;
using SharpGLTF.Schema2;
using Veldrid;

namespace VeldridGlTF.Viewer.Systems.Render.ElementAccessors
{
    public class AbstractElementAccessor
    {
        private readonly string _key;
        private readonly VertexElementFormat _format;
        private readonly Accessor _accessor;

        public AbstractElementAccessor(string key, VertexElementFormat format, Accessor accessor)
        {
            _key = key;
            _format = format;
            _accessor = accessor;
        }

        public VertexElementDescription VertexElementDescription
        {
            get
            {
                return new VertexElementDescription(_key, Format, VertexElementSemantic.TextureCoordinate);
            }
        }

        public VertexElementFormat Format
        {
            get { return _format; }
        }

        public static AbstractElementAccessor GetElementAccessor(string key, Accessor accessor)
        {
            switch (accessor.Dimensions)
            {
                case DimensionType.SCALAR:
                    switch (accessor.Encoding)
                    {
                        case EncodingType.BYTE:
                            throw FormatNotSupportedException(accessor);
                        case EncodingType.UNSIGNED_BYTE:
                            throw FormatNotSupportedException(accessor);
                        case EncodingType.SHORT:
                            throw FormatNotSupportedException(accessor);
                        case EncodingType.UNSIGNED_SHORT:
                            throw FormatNotSupportedException(accessor);
                        case EncodingType.UNSIGNED_INT:
                            return new IntElementAccessor(key, VertexElementFormat.Int1, 1, accessor);
                        case EncodingType.FLOAT:
                            return new FloatElementAccessor(key, VertexElementFormat.Float1, 1, accessor);
                        default:
                            throw FormatNotSupportedException(accessor);
                    }
                    break;
                case DimensionType.VEC2:
                    switch (accessor.Encoding)
                    {
                        case EncodingType.BYTE:
                            return new AbstractElementAccessor(key, accessor.Normalized ? VertexElementFormat.SByte2 : VertexElementFormat.SByte2_Norm, accessor);
                        case EncodingType.UNSIGNED_BYTE:
                            return new AbstractElementAccessor(key, accessor.Normalized ? VertexElementFormat.Byte2 : VertexElementFormat.Byte2_Norm, accessor);
                        case EncodingType.SHORT:
                            return new AbstractElementAccessor(key, accessor.Normalized ? VertexElementFormat.Short2 : VertexElementFormat.Short2_Norm, accessor);
                        case EncodingType.UNSIGNED_SHORT:
                            return new AbstractElementAccessor(key, accessor.Normalized ? VertexElementFormat.UShort2 : VertexElementFormat.UShort2_Norm, accessor);
                        case EncodingType.UNSIGNED_INT:
                            return new IntElementAccessor(key, VertexElementFormat.Int2, 2, accessor);
                        case EncodingType.FLOAT:
                            return new FloatElementAccessor(key, VertexElementFormat.Float2, 2, accessor);
                        default:
                            throw FormatNotSupportedException(accessor);
                    }
                    break;
                case DimensionType.VEC3:
                    switch (accessor.Encoding)
                    {
                        case EncodingType.BYTE:
                            throw FormatNotSupportedException(accessor);
                        case EncodingType.UNSIGNED_BYTE:
                            throw FormatNotSupportedException(accessor);
                        case EncodingType.SHORT:
                            throw FormatNotSupportedException(accessor);
                        case EncodingType.UNSIGNED_SHORT:
                            throw FormatNotSupportedException(accessor);
                        case EncodingType.UNSIGNED_INT:
                            return new IntElementAccessor(key, VertexElementFormat.Int3, 3, accessor);
                        case EncodingType.FLOAT:
                            return new FloatElementAccessor(key, VertexElementFormat.Float3, 3, accessor);
                        default:
                            throw FormatNotSupportedException(accessor);
                    }
                    break;
                case DimensionType.VEC4:
                    switch (accessor.Encoding)
                    {
                        case EncodingType.BYTE:
                            return new AbstractElementAccessor(key, accessor.Normalized ? VertexElementFormat.SByte4 : VertexElementFormat.SByte4_Norm, accessor);
                        case EncodingType.UNSIGNED_BYTE:
                            return new AbstractElementAccessor(key, accessor.Normalized ? VertexElementFormat.Byte4 : VertexElementFormat.Byte4_Norm, accessor);
                        case EncodingType.SHORT:
                            return new AbstractElementAccessor(key, accessor.Normalized ? VertexElementFormat.Short4 : VertexElementFormat.Short4_Norm, accessor);
                        case EncodingType.UNSIGNED_SHORT:
                            return new AbstractElementAccessor(key, accessor.Normalized ? VertexElementFormat.UShort4 : VertexElementFormat.UShort4_Norm, accessor);
                        case EncodingType.UNSIGNED_INT:
                            return new IntElementAccessor(key, VertexElementFormat.Int4, 3, accessor);
                        case EncodingType.FLOAT:
                            return new FloatElementAccessor(key, VertexElementFormat.Float4, 4, accessor);
                        default:
                            throw FormatNotSupportedException(accessor);
                    }
                case DimensionType.MAT2:
                    throw FormatNotSupportedException(accessor);
                case DimensionType.MAT3:
                    throw FormatNotSupportedException(accessor);
                case DimensionType.MAT4:
                    throw FormatNotSupportedException(accessor);
                default:
                    throw FormatNotSupportedException(accessor);
            }
        }

        private static FormatException FormatNotSupportedException(Accessor accessorValue)
        {
            return new FormatException(accessorValue.Name + " " + accessorValue.Dimensions + " " + accessorValue.Encoding + " " + accessorValue.Normalized + " is not supported");
        }
    }
}
