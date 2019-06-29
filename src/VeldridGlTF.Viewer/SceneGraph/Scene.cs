using Leopotam.Ecs;

namespace VeldridGlTF.Viewer.SceneGraph
{
    public class Scene: NodeContainer
    {
        private Node.WorldMatrixUpdateQueue _worldMatrixUpdateQueue = new Node.WorldMatrixUpdateQueue();
        private EcsWorld _world;
        private EcsSystems _systems;

        public Scene()
        {
            _world = new EcsWorld();
            _systems = new EcsSystems(_world, "main");
        }

        public EcsWorld World
        {
            get { return _world; }
            set { _world = value; }
        }

        public EcsSystems Systems
        {
            get { return _systems; }
            set { _systems = value; }
        }

        public WorldMatrixToken EnqueueWorldTransformUpdate(Node node)
        {
            return _worldMatrixUpdateQueue.Add(node);
        }

        public void UpdateWorldTransforms()
        {
            _worldMatrixUpdateQueue.Update();
        }

        public void Dispose()
        {
            _systems?.Dispose();
            _systems = null;
            _world.Dispose();
            _world = null;
        }
    }
}