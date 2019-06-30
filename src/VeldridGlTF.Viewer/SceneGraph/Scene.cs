using Leopotam.Ecs;

namespace VeldridGlTF.Viewer.SceneGraph
{
    public class Scene : NodeContainer
    {
        private readonly Node.WorldMatrixUpdateQueue _worldMatrixUpdateQueue = new Node.WorldMatrixUpdateQueue();

        public Scene()
        {
            World = new EcsWorld();
            Systems = new EcsSystems(World, "main");
        }

        public EcsWorld World { get; set; }

        public EcsSystems Systems { get; set; }

        public WorldMatrixToken EnqueueWorldTransformUpdate(Node node)
        {
            return _worldMatrixUpdateQueue.Add(node);
        }

        public void UpdateWorldTransforms()
        {
            _worldMatrixUpdateQueue.Update();
        }

        public Node CreateNode(Node parent)
        {
            return new Node(this, true) {Parent =  parent};
        }
        public Node CreateNodeWithNoTransform(Node parent)
        {
            return new Node(this, false) { Parent = parent };
        }
        public void Dispose()
        {
            Systems?.Dispose();
            Systems = null;
            World.Dispose();
            World = null;
        }
    }
}