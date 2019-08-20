using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace VeldridGlTF.Viewer
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix3x3
    {
        private static readonly Matrix3x3 _identity = new Matrix3x3(1f, 0.0f, 0.0f, 0.0f, 1f, 0.0f, 0.0f, 0.0f, 1f);
        public float M11;
        public float M12;
        public float M13;
        public float p0;
        public float M21;
        public float M22;
        public float M23;
        public float p1;
        public float M31;
        public float M32;
        public float M33;
        public float p2;

        public static Matrix3x3 Identity
        {
            get
            {
                return Matrix3x3._identity;
            }
        }


        public Matrix3x3(float m11,
            float m12,
            float m13,
            float m21,
            float m22,
            float m23,
            float m31,
            float m32,
            float m33)
        {
            this.M11 = m11;
            this.M12 = m12;
            this.M13 = m13;
            this.M21 = m21;
            this.M22 = m22;
            this.M23 = m23;
            this.M31 = m31;
            this.M32 = m32;
            this.M33 = m33;
            p0 = p1 = p2 = 0;
        }

        public Matrix3x3(Matrix3x2 value)
        {
            this.M11 = value.M11;
            this.M12 = value.M12;
            this.M13 = 0.0f;
            this.M21 = value.M21;
            this.M22 = value.M22;
            this.M23 = 0.0f;
            this.M31 = 0.0f;
            this.M32 = 0.0f;
            this.M33 = 1f;
            p0 = p1 = p2 = 0;
        }

        public static Matrix3x3 CreateScale(float xScale, float yScale, float zScale)
        {
            Matrix3x3 matrix4x4 = Identity;
            matrix4x4.M11 = xScale;
            matrix4x4.M12 = 0.0f;
            matrix4x4.M13 = 0.0f;
            matrix4x4.M21 = 0.0f;
            matrix4x4.M22 = yScale;
            matrix4x4.M23 = 0.0f;
            matrix4x4.M31 = 0.0f;
            matrix4x4.M32 = 0.0f;
            matrix4x4.M33 = zScale;
            return matrix4x4;
        }

        public static Matrix3x3 CreateScale(Vector3 scales)
        {
            Matrix3x3 matrix4x4 = Identity;
            matrix4x4.M11 = scales.X;
            matrix4x4.M12 = 0.0f;
            matrix4x4.M13 = 0.0f;
            matrix4x4.M21 = 0.0f;
            matrix4x4.M22 = scales.Y;
            matrix4x4.M23 = 0.0f;
            matrix4x4.M31 = 0.0f;
            matrix4x4.M32 = 0.0f;
            matrix4x4.M33 = scales.Z;
            return matrix4x4;
        }

        public static Matrix3x3 CreateScale(float scale)
        {
            Matrix3x3 matrix4x4 = Identity;
            matrix4x4.M11 = scale;
            matrix4x4.M12 = 0.0f;
            matrix4x4.M13 = 0.0f;
            matrix4x4.M21 = 0.0f;
            matrix4x4.M22 = scale;
            matrix4x4.M23 = 0.0f;
            matrix4x4.M31 = 0.0f;
            matrix4x4.M32 = 0.0f;
            matrix4x4.M33 = scale;
            return matrix4x4;
        }


        public static Matrix3x3 CreateRotationX(float radians)
        {
            float num1 = (float)Math.Cos(radians);
            float num2 = (float)Math.Sin(radians);
            Matrix3x3 matrix4x4 = Matrix3x3.Identity;
            matrix4x4.M11 = 1f;
            matrix4x4.M12 = 0.0f;
            matrix4x4.M13 = 0.0f;
            matrix4x4.M21 = 0.0f;
            matrix4x4.M22 = num1;
            matrix4x4.M23 = num2;
            matrix4x4.M31 = 0.0f;
            matrix4x4.M32 = -num2;
            matrix4x4.M33 = num1;
            return matrix4x4;
        }

      
        public static Matrix3x3 CreateRotationY(float radians)
        {
            float num1 = (float)Math.Cos(radians);
            float num2 = (float)Math.Sin(radians);
            Matrix3x3 matrix4x4 = Matrix3x3.Identity;
            matrix4x4.M11 = num1;
            matrix4x4.M12 = 0.0f;
            matrix4x4.M13 = -num2;
            matrix4x4.M21 = 0.0f;
            matrix4x4.M22 = 1f;
            matrix4x4.M23 = 0.0f;
            matrix4x4.M31 = num2;
            matrix4x4.M32 = 0.0f;
            matrix4x4.M33 = num1;
            return matrix4x4;
        }



        public static Matrix3x3 CreateRotationZ(float radians)
        {
            float num1 = (float)Math.Cos(radians);
            float num2 = (float)Math.Sin(radians);
            Matrix3x3 matrix4x4 = Identity;
            matrix4x4.M11 = num1;
            matrix4x4.M12 = num2;
            matrix4x4.M13 = 0.0f;
            matrix4x4.M21 = -num2;
            matrix4x4.M22 = num1;
            matrix4x4.M23 = 0.0f;
            matrix4x4.M31 = 0.0f;
            matrix4x4.M32 = 0.0f;
            matrix4x4.M33 = 1f;
            return matrix4x4;
        }

      

        public static Matrix3x3 CreateFromAxisAngle(Vector3 axis, float angle)
        {
            float x = axis.X;
            float y = axis.Y;
            float z = axis.Z;
            float num1 = (float)Math.Sin(angle);
            float num2 = (float)Math.Cos(angle);
            float num3 = x * x;
            float num4 = y * y;
            float num5 = z * z;
            float num6 = x * y;
            float num7 = x * z;
            float num8 = y * z;
            Matrix3x3 matrix4x4 = Identity;
            matrix4x4.M11 = num3 + num2 * (1f - num3);
            matrix4x4.M12 = (float)((double)num6 - (double)num2 * (double)num6 + (double)num1 * (double)z);
            matrix4x4.M13 = (float)((double)num7 - (double)num2 * (double)num7 - (double)num1 * (double)y);
            matrix4x4.M21 = (float)((double)num6 - (double)num2 * (double)num6 - (double)num1 * (double)z);
            matrix4x4.M22 = num4 + num2 * (1f - num4);
            matrix4x4.M23 = (float)((double)num8 - (double)num2 * (double)num8 + (double)num1 * (double)x);
            matrix4x4.M31 = (float)((double)num7 - (double)num2 * (double)num7 + (double)num1 * (double)y);
            matrix4x4.M32 = (float)((double)num8 - (double)num2 * (double)num8 - (double)num1 * (double)x);
            matrix4x4.M33 = num5 + num2 * (1f - num5);
            return matrix4x4;
        }


        public static Matrix3x3 CreateLookAt(
          Vector3 cameraPosition,
          Vector3 cameraTarget,
          Vector3 cameraUpVector)
        {
            Vector3 vector3_1 = Vector3.Normalize(cameraPosition - cameraTarget);
            Vector3 vector3_2 = Vector3.Normalize(Vector3.Cross(cameraUpVector, vector3_1));
            Vector3 vector1 = Vector3.Cross(vector3_1, vector3_2);
            Matrix3x3 matrix4x4 = Identity;
            matrix4x4.M11 = vector3_2.X;
            matrix4x4.M12 = vector1.X;
            matrix4x4.M13 = vector3_1.X;
            matrix4x4.M21 = vector3_2.Y;
            matrix4x4.M22 = vector1.Y;
            matrix4x4.M23 = vector3_1.Y;
            matrix4x4.M31 = vector3_2.Z;
            matrix4x4.M32 = vector1.Z;
            matrix4x4.M33 = vector3_1.Z;
            return matrix4x4;
        }

        public static Matrix3x3 CreateWorld(Vector3 forward, Vector3 up)
        {
            Vector3 vector3_1 = Vector3.Normalize(-forward);
            Vector3 vector2 = Vector3.Normalize(Vector3.Cross(up, vector3_1));
            Vector3 vector3_2 = Vector3.Cross(vector3_1, vector2);
            Matrix3x3 matrix4x4 = Identity;
            matrix4x4.M11 = vector2.X;
            matrix4x4.M12 = vector2.Y;
            matrix4x4.M13 = vector2.Z;
            matrix4x4.M21 = vector3_2.X;
            matrix4x4.M22 = vector3_2.Y;
            matrix4x4.M23 = vector3_2.Z;
            matrix4x4.M31 = vector3_1.X;
            matrix4x4.M32 = vector3_1.Y;
            matrix4x4.M33 = vector3_1.Z;
            return matrix4x4;
        }

        public static Matrix3x3 CreateFromQuaternion(Quaternion quaternion)
        {
            float num1 = quaternion.X * quaternion.X;
            float num2 = quaternion.Y * quaternion.Y;
            float num3 = quaternion.Z * quaternion.Z;
            float num4 = quaternion.X * quaternion.Y;
            float num5 = quaternion.Z * quaternion.W;
            float num6 = quaternion.Z * quaternion.X;
            float num7 = quaternion.Y * quaternion.W;
            float num8 = quaternion.Y * quaternion.Z;
            float num9 = quaternion.X * quaternion.W;
            Matrix3x3 matrix4x4 = Identity;
            matrix4x4.M11 = (float)(1.0 - 2.0 * ((double)num2 + (double)num3));
            matrix4x4.M12 = (float)(2.0 * ((double)num4 + (double)num5));
            matrix4x4.M13 = (float)(2.0 * ((double)num6 - (double)num7));
            matrix4x4.M21 = (float)(2.0 * ((double)num4 - (double)num5));
            matrix4x4.M22 = (float)(1.0 - 2.0 * ((double)num3 + (double)num1));
            matrix4x4.M23 = (float)(2.0 * ((double)num8 + (double)num9));
            matrix4x4.M31 = (float)(2.0 * ((double)num6 + (double)num7));
            matrix4x4.M32 = (float)(2.0 * ((double)num8 - (double)num9));
            matrix4x4.M33 = (float)(1.0 - 2.0 * ((double)num2 + (double)num1));
            return matrix4x4;
        }

        public static Matrix3x3 CreateFromYawPitchRoll(float yaw, float pitch, float roll)
        {
            return Matrix3x3.CreateFromQuaternion(Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll));
        }
    }

}
