using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SharpGLTF.Schema2;
using Veldrid;

namespace VeldridGlTF.Viewer.Systems.Render.ElementAccessors
{
    public abstract class AbstractElementAccessor
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

        public abstract int Size { get; }

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
                            return new ScalarElementAccessor(key, accessor);
                        case EncodingType.UNSIGNED_BYTE:
                            return new ScalarElementAccessor(key, accessor);
                        case EncodingType.SHORT:
                            return new ScalarElementAccessor(key, accessor);
                        case EncodingType.UNSIGNED_SHORT:
                            return new ScalarElementAccessor(key, accessor);
                        case EncodingType.UNSIGNED_INT:
                            return new ScalarElementAccessor(key, accessor);
                        case EncodingType.FLOAT:
                            return new ScalarElementAccessor(key, accessor);
                        default:
                            throw FormatNotSupportedException(accessor);
                    }
                    break;
                case DimensionType.VEC2:
                    switch (accessor.Encoding)
                    {
                        case EncodingType.BYTE:
                            return new Vec2ElementAccessor(key, accessor);
                        case EncodingType.UNSIGNED_BYTE:
                            return new Vec2ElementAccessor(key, accessor);
                        case EncodingType.SHORT:
                            return new Vec2ElementAccessor(key, accessor);
                        case EncodingType.UNSIGNED_SHORT:
                            return new Vec2ElementAccessor(key, accessor);
                        case EncodingType.UNSIGNED_INT:
                            return new Vec2ElementAccessor(key, accessor);
                        case EncodingType.FLOAT:
                            return new Vec2ElementAccessor(key, accessor);
                        default:
                            throw FormatNotSupportedException(accessor);
                    }
                    break;
                case DimensionType.VEC3:
                    switch (accessor.Encoding)
                    {
                        case EncodingType.BYTE:
                            return new Vec3ElementAccessor(key, accessor);
                        case EncodingType.UNSIGNED_BYTE:
                            return new Vec3ElementAccessor(key, accessor);
                        case EncodingType.SHORT:
                            return new Vec3ElementAccessor(key, accessor);
                        case EncodingType.UNSIGNED_SHORT:
                            return new Vec3ElementAccessor(key, accessor);
                        case EncodingType.UNSIGNED_INT:
                            return new Vec3ElementAccessor(key, accessor);
                        case EncodingType.FLOAT:
                            return new Vec3ElementAccessor(key, accessor);
                        default:
                            throw FormatNotSupportedException(accessor);
                    }
                    break;
                case DimensionType.VEC4:
                    switch (accessor.Encoding)
                    {
                        case EncodingType.BYTE:
                            return new Vec4ElementAccessor(key, accessor);
                        case EncodingType.UNSIGNED_BYTE:
                            return new Vec4ElementAccessor(key, accessor);
                        case EncodingType.SHORT:
                            return new Vec4ElementAccessor(key, accessor);
                        case EncodingType.UNSIGNED_SHORT:
                            return new Vec4ElementAccessor(key, accessor);
                        case EncodingType.UNSIGNED_INT:
                            return new Vec4ElementAccessor(key, accessor);
                        case EncodingType.FLOAT:
                            return new Vec4ElementAccessor(key, accessor);
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

        public abstract void Write(int index, VertexBufferStream vertexBuffer);
    }
}
