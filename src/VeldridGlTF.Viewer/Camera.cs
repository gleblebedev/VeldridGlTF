using System;
using System.Numerics;
using VeldridGlTF.Viewer.Systems;

namespace VeldridGlTF.Viewer
{
    public class Camera
    {
        private float _far = 1000f;
        private Vector3 _lookDirection = new Vector3(0, -.3f, -1f);
        private float _moveSpeed = 10.0f;
        private float _near = 1f;
        private float _pitch;

        private Vector3 _position = new Vector3(0, -.3f, -1f) * -800.0f;

        private Vector2 _previousMousePos;

        private float _windowHeight;
        private float _windowWidth;

        private float _yaw;

        public Camera(float width, float height)
        {
            _windowWidth = width;
            _windowHeight = height;
            UpdatePerspectiveMatrix();
            UpdateViewMatrix();
        }

        public Matrix4x4 ViewMatrix { get; private set; }

        public Matrix4x4 ProjectionMatrix { get; private set; }

        public Vector3 Position
        {
            get => _position;
            set
            {
                _position = value;
                UpdateViewMatrix();
            }
        }

        public float FarDistance
        {
            get => _far;
            set
            {
                _far = value;
                UpdatePerspectiveMatrix();
            }
        }

        public float FieldOfView { get; } = 1f;

        public float NearDistance
        {
            get => _near;
            set
            {
                _near = value;
                UpdatePerspectiveMatrix();
            }
        }

        public float AspectRatio => _windowWidth / _windowHeight;

        public float Yaw
        {
            get => _yaw;
            set
            {
                _yaw = value;
                UpdateViewMatrix();
            }
        }

        public float Pitch
        {
            get => _pitch;
            set
            {
                _pitch = value;
                UpdateViewMatrix();
            }
        }

        public Vector3 Forward => GetLookDir();

        public event Action<Matrix4x4> ProjectionChanged;
        public event Action<Matrix4x4> ViewChanged;


        private float Clamp(float value, float min, float max)
        {
            return value > max
                ? max
                : value < min
                    ? min
                    : value;
        }

        public void WindowResized(float width, float height)
        {
            _windowWidth = width;
            _windowHeight = height;
            UpdatePerspectiveMatrix();
        }

        private void UpdatePerspectiveMatrix()
        {
            ProjectionMatrix =
                Matrix4x4.CreatePerspectiveFieldOfView(FieldOfView, _windowWidth / _windowHeight, _near, _far);
            ProjectionChanged?.Invoke(ProjectionMatrix);
        }

        private void UpdateViewMatrix()
        {
            var lookDir = GetLookDir();
            _lookDirection = lookDir;
            ViewMatrix = Matrix4x4.CreateLookAt(_position, _position + _lookDirection, Vector3.UnitY);
            ViewChanged?.Invoke(ViewMatrix);
        }

        private Vector3 GetLookDir()
        {
            var lookRotation = Quaternion.CreateFromYawPitchRoll(Yaw, Pitch, 0f);
            var lookDir = Vector3.Transform(-Vector3.UnitZ, lookRotation);
            return lookDir;
        }

        public CameraInfo GetCameraInfo()
        {
            return new CameraInfo
            {
                CameraPosition_WorldSpace = _position,
                CameraLookDirection = _lookDirection
            };
        }
    }
}