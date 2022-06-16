using UnityEngine;

namespace Cameras
{
    public class FaceCamera : MonoBehaviour
    {
        private Transform mainCameraTransform;

        private void Start()
        {
            if (UnityEngine.Camera.main != null)
            {
                mainCameraTransform = UnityEngine.Camera.main.transform;
            }
        }

        private void LateUpdate()
        {
            var rotation = mainCameraTransform.rotation;
            transform.LookAt(
                transform.position + rotation * Vector3.forward,
                rotation * Vector3.up
            );
        }
    }
}
