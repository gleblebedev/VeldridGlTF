using System;
using Veldrid;
using Veldrid.SPIRV;

namespace VeldridGlTF.Viewer.Systems.Render.Shaders.PBR
{
    public class PBRShaderKey : ShaderKey, IEquatable<PBRShaderKey>
    {
        public PBRShaderKey(IShaderFactory factory, ILayoutNameResolver layoutNameResolver, RenderVertexLayout layout, uint jointCount = 0) :
            base(factory, layoutNameResolver)
        {
            JointCount = jointCount;
            VertexLayout = layout;
            foreach (var element in VertexLayout.VertexLayoutDescription.Elements)
                switch (element.Name)
                {
                    case "NORMAL":
                        SetFlag(ShaderFlag.HAS_NORMALS);
                        break;
                    case "TANGENT":
                        SetFlag(ShaderFlag.HAS_TANGENTS);
                        break;
                    case "TARGET_POSITION0":
                        SetFlag(ShaderFlag.HAS_TARGET_POSITION0);
                        break;
                    case "TARGET_POSITION1":
                        SetFlag(ShaderFlag.HAS_TARGET_POSITION1);
                        break;
                    case "TARGET_POSITION2":
                        SetFlag(ShaderFlag.HAS_TARGET_POSITION2);
                        break;
                    case "TARGET_POSITION3":
                        SetFlag(ShaderFlag.HAS_TARGET_POSITION3);
                        break;
                    case "TARGET_POSITION4":
                        SetFlag(ShaderFlag.HAS_TARGET_POSITION4);
                        break;
                    case "TARGET_NORMAL0":
                        SetFlag(ShaderFlag.HAS_TARGET_NORMAL0);
                        break;
                    case "TARGET_NORMAL1":
                        SetFlag(ShaderFlag.HAS_TARGET_NORMAL1);
                        break;
                    case "TARGET_NORMAL2":
                        SetFlag(ShaderFlag.HAS_TARGET_NORMAL2);
                        break;
                    case "TARGET_NORMAL3":
                        SetFlag(ShaderFlag.HAS_TARGET_NORMAL3);
                        break;
                    case "TARGET_NORMAL4":
                        SetFlag(ShaderFlag.HAS_TARGET_NORMAL4);
                        break;
                    case "TARGET_TANGENT0":
                        SetFlag(ShaderFlag.HAS_TARGET_TANGENT0);
                        break;
                    case "TARGET_TANGENT1":
                        SetFlag(ShaderFlag.HAS_TARGET_TANGENT1);
                        break;
                    case "TARGET_TANGENT2":
                        SetFlag(ShaderFlag.HAS_TARGET_TANGENT2);
                        break;
                    case "TARGET_TANGENT3":
                        SetFlag(ShaderFlag.HAS_TARGET_TANGENT3);
                        break;
                    case "TARGET_TANGENT4":
                        SetFlag(ShaderFlag.HAS_TARGET_TANGENT4);
                        break;
                    case "TEXCOORD_0":
                        SetFlag(ShaderFlag.HAS_UV_SET1);
                        break;
                    case "JOINTS_0":
                        SetFlag(ShaderFlag.HAS_JOINT_SET1);
                        break;
                    case "WEIGHTS_0":
                        SetFlag(ShaderFlag.HAS_WEIGHT_SET1);
                        break;
                    case "JOINTS_1":
                        SetFlag(ShaderFlag.HAS_JOINT_SET2);
                        break;
                    case "WEIGHTS_1":
                        SetFlag(ShaderFlag.HAS_WEIGHT_SET2);
                        break;
                    case "TEXCOORD_1":
                        SetFlag(ShaderFlag.HAS_UV_SET2);
                        break;
                    case "COLOR_0":
                        if (element.Format == VertexElementFormat.Float3)
                            SetFlag(ShaderFlag.HAS_VERTEX_COLOR_VEC3);
                        else if (element.Format == VertexElementFormat.Float4)
                            SetFlag(ShaderFlag.HAS_VERTEX_COLOR_VEC4);
                        break;
                }
        }

        public RenderVertexLayout VertexLayout { get; }

        public uint JointCount { get; }

        public ulong Flags { get; set; }

        public bool Equals(PBRShaderKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Flags == other.Flags && JointCount == other.JointCount && Equals(VertexLayout, other.VertexLayout);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PBRShaderKey) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ Flags.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)JointCount;
                hashCode = (hashCode * 397) ^ (VertexLayout != null ? VertexLayout.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(PBRShaderKey left, PBRShaderKey right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PBRShaderKey left, PBRShaderKey right)
        {
            return !Equals(left, right);
        }

        public bool HasFlag(ShaderFlag flag)
        {
            var mask = 1ul << (int) flag;
            return mask == (Flags & mask);
        }

        public void SetFlag(ShaderFlag flag)
        {
            Flags |= 1ul << (int) flag;
        }

        public void DropFlag(ShaderFlag flag)
        {
            Flags &= ~(1ul << (int) flag);
        }
    }
}