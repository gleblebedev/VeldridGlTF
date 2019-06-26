using System;
using System.Numerics;
using NUnit.Framework;

namespace VeldridGlTF.Viewer
{
    [TestFixture]
    public class TransformTestFixture
    {
        [Test]
        public void EvaluateWorldMatrix_OperationsAppliedInOrder()
        {
            var t = new Transform(
                Vector3.UnitX, 
                Quaternion.CreateFromAxisAngle(Vector3.UnitY, 1.5707963267948966192313216916398f),
                new Vector3(2,3,4));
            Matrix4x4 m;

            t.EvaluateMatrix(out m);

            var newVector = Vector3.Transform(new Vector3(1, 2, 3), m);
            Assert.AreEqual(3.0f*4+1, newVector.X, 1e-6f);
            Assert.AreEqual(2.0f * 3, newVector.Y, 1e-6f);
            Assert.AreEqual(-1.0f * 2, newVector.Z, 1e-6f);
        }
    }
}
