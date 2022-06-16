using Mirror;
using Networking;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Cameras
{
    public class Minimap : MonoBehaviour, IPointerDownHandler, IDragHandler
    {
        [SerializeField]
        private RectTransform minimapRect;

        [SerializeField]
        private float mapScale = 20f;

        [SerializeField]
        private float offset = -6f;

        private Transform playerCameraTransform;

        private void Update()
        {
            if (playerCameraTransform || !NetworkClient.connection.identity)
            {
                return;
            }

            playerCameraTransform = NetworkClient.connection.identity
                .GetComponent<RTSPlayer>()
                .GetCameraTransform();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            MoveCamera();
        }

        public void OnDrag(PointerEventData eventData)
        {
            MoveCamera();
        }

        private void MoveCamera()
        {
            var mousePos = Mouse.current.position.ReadValue();

            if (
                !RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    minimapRect,
                    mousePos,
                    null,
                    out var localPoint
                )
            )
            {
                return;
            }

            var rect = minimapRect.rect;
            var lerp = new Vector2(
                (localPoint.x - rect.x) / rect.width,
                (localPoint.y - rect.y) / rect.height
            );
            var newCameraPos = new Vector3(
                Mathf.Lerp(-mapScale, mapScale, lerp.x),
                playerCameraTransform.position.y,
                Mathf.Lerp(-mapScale, mapScale, lerp.y)
            );

            playerCameraTransform.position = newCameraPos + new Vector3(0f, 0f, offset);
        }
    }
}
