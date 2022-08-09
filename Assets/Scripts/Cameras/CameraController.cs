using Inputs;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cameras
{
    public class CameraController : NetworkBehaviour
    {
        [SerializeField]
        private Transform playerCameraTransform;

        [SerializeField]
        private float speed = 20f;

        [SerializeField]
        private float screenBorderThickness = 10f;

        [SerializeField]
        private Vector2 screenXLimits = Vector2.zero;

        [SerializeField]
        private Vector2 screenZLimits = Vector2.zero;

        private Controls _controls;

        private Vector2 _previousInput;

        [ClientCallback]
        private void Update()
        {
            if (!hasAuthority || !Application.isFocused)
                return;

            UpdateCameraPosition();
        }

        public override void OnStartAuthority()
        {
            playerCameraTransform.gameObject.SetActive(true);

            _controls = new Controls();

            _controls.Player.MoveCamera.performed += SetPreviousInput;
            _controls.Player.MoveCamera.canceled += SetPreviousInput;

            _controls.Enable();
        }

        private void UpdateCameraPosition()
        {
            var position = playerCameraTransform.position;

            if (_previousInput == Vector2.zero)
            {
                var cursorMovement = Vector3.zero;

                var cursorPosition = Mouse.current.position.ReadValue();

                if (cursorPosition.y >= Screen.height - screenBorderThickness)
                {
                    cursorMovement.z += 1;
                }
                else if (cursorPosition.y <= screenBorderThickness)
                {
                    cursorMovement.z -= 1;
                }

                if (cursorPosition.x >= Screen.width - screenBorderThickness)
                {
                    cursorMovement.x += 1;
                }
                else if (cursorPosition.x <= screenBorderThickness)
                {
                    cursorMovement.x -= 1;
                }

                position += cursorMovement.normalized * (speed * Time.deltaTime);
            }
            else
            {
                position +=
                    new Vector3(_previousInput.x, 0f, _previousInput.y) * (speed * Time.deltaTime);
            }

            position.x = Mathf.Clamp(position.x, screenXLimits.x, screenXLimits.y);
            position.z = Mathf.Clamp(position.z, screenXLimits.x, screenXLimits.y);

            playerCameraTransform.position = position;
        }

        private void SetPreviousInput(InputAction.CallbackContext ctx)
        {
            _previousInput = ctx.ReadValue<Vector2>();
        }
    }
}
