using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit;
using NUnit.Framework;
using VeldridGlTF.Viewer.Components;

namespace VeldridGlTF.Viewer.SceneGraph
{
    [TestFixture]
    public class NodeTestFixture
    {
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void NewNode_AddedToScene_UpdateWorldTransformsDoesntThrow(bool hasTransform)
        {
            var scene = new Scene();
            var node = new Node(scene, hasTransform);

            Assert.AreEqual(1, scene.Children.Count);
            Assert.AreEqual(node, scene.Children.First());

            var worldTransform = node.GetComponent<WorldTransform>();

            if (hasTransform)
            {
                Assert.NotNull(node.Transform);
                Assert.NotNull(worldTransform);
            }
            else
            {
                Assert.IsNull(node.Transform);
                Assert.IsNull(worldTransform);
            }

            scene.UpdateWorldTransforms();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void NewNode_AddedToParentWithTransform_UpdateWorldTransformsDoesntThrow(bool hasTransform)
        {
            var scene = new Scene();
            var parent = new Node(scene, true);
            var node = new Node(scene, hasTransform) {Parent = parent};

            scene.UpdateWorldTransforms();
        }
    }
}
