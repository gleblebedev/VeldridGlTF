namespace VeldridGlTF.Viewer.Components
{
    public class LocalTransform
    {
        public Transform Transform;

        public LocalTransform Parent;

        public LocalTransform()
        {
            Transform = Transform.Identity;
        }

        public void EvaluateWorldTransform(out Transform transform)
        {
            if (Parent != null)
                transform = Transform * Parent.Transform;
            else
                transform = Transform;
        }
    }
}