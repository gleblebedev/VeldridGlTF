using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

        public class HierarchyTestOperationCollection
        {
            private readonly IList<HierarchyTestOperation> _operations;

            public HierarchyTestOperationCollection(IList<HierarchyTestOperation> operations)
            {
                _operations = operations;
            }

            public IList<HierarchyTestOperation> Operations
            {
                get { return _operations; }
            }

            public override string ToString()
            {
                return string.Join(", ", Operations);
            }
        }

        public static IEnumerable<object[]> HierachyPermunations
        {
            get
            {
                var operations = new[]
                {
                    new HierarchyTestOperation("Node[0]=Vec", _ => _.Nodes[0].Transform.Position = new Vector3(2,0,0)),
                    new HierarchyTestOperation("Node[1]=Vec", _ => _.Nodes[0].Transform.Position = new Vector3(-1,4,0)),
                    new HierarchyTestOperation("Node[2]=Vec", _ => _.Nodes[0].Transform.Position = new Vector3(5,0,1)),
                    new HierarchyTestOperation("Update", _ => _.Scene.UpdateWorldTransforms()),
                    new HierarchyTestOperation("First", null),
                    new HierarchyTestOperation("Second", null),
                };
                long permutations = 1;
                for (int i = 2; i <= operations.Length; ++i)
                {
                    permutations *= i;
                }

                var attachments = new List<Tuple<int, int>[]>();
                for (int first = 0; first <= 2; ++first)
                {
                    for (int second = 0; second <= 2; ++second)
                    {
                        for (int firstTo = 0; firstTo <= 2; ++firstTo)
                        {
                            for (int secondTo = 0; secondTo <= 2; ++secondTo)
                            {
                                //TODO
                            }
                        }
                    }
                }

                var counter = 100;
                for (int permutation = 0; permutation < permutations && counter > 0; permutation++,--counter)
                {
                    var args = new List<HierarchyTestOperation>(operations);
                    var combination = new List<HierarchyTestOperation>(operations.Length);
                    var p = permutation;
                    while (args.Count > 0)
                    {
                        var index = p % args.Count;
                        p /= args.Count;
                        combination.Add(args[index]);
                        args.RemoveAt(index);
                    }

                    yield return new object[]{ new HierarchyTestOperationCollection(combination) };
                }
            }
        }



        [Test]
        [TestCaseSource(nameof(HierachyPermunations))]
        public void NodeHierarchy_ValidWorldMatrices(HierarchyTestOperationCollection operations)
        {
            var state = new HierarchyTestState();
            foreach (var operation in operations.Operations)
            {
                operation.Execute(state);
            }

            state.Scene.UpdateWorldTransforms();

            foreach (var stateNode in state.Nodes)
            {
                Matrix4x4 evaluated;
                stateNode.EvaluateWorldTransform(out evaluated);
                Matrix4x4 actual = stateNode.GetComponent<WorldTransform>().WorldMatrix;
                foreach (var diff in GetComponents(actual).Zip(GetComponents(evaluated),(a,b)=>a-b))
                {
                    if (Math.Abs(diff) > 1e-6)
                        Assert.Fail("Expected "+evaluated+" but the value after update is "+actual);
                }
            }
        }

        public IEnumerable<float> GetComponents(Matrix4x4 m)
        {
            yield return m.M11;
            yield return m.M12;
            yield return m.M13;
            yield return m.M14;

            yield return m.M21;
            yield return m.M22;
            yield return m.M23;
            yield return m.M24;

            yield return m.M31;
            yield return m.M32;
            yield return m.M33;
            yield return m.M34;

            yield return m.M41;
            yield return m.M42;
            yield return m.M43;
            yield return m.M44;

        }

        public class HierarchyTestOperation
        {
            private readonly string _name;
            private readonly Action<HierarchyTestState> _action;

            public HierarchyTestOperation(string name, Action<HierarchyTestState> action)
            {
                _name = name;
                _action = action;
            }

            public void Execute(HierarchyTestState state)
            {
                _action(state);
            }

            public override string ToString()
            {
                return _name;
            }
        }

        public class HierarchyTestState
        {
            public Scene Scene;
            private Node[] _nodes;

            public HierarchyTestState()
            {
                Scene = new Scene();
                _nodes = new Node[] {new Node(Scene), new Node(Scene), new Node(Scene)};
            }

            public Node[] Nodes
            {
                get { return _nodes; }
            }
        }
    }
}
