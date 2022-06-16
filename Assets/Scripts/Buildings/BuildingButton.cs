using Mirror;
using Networking;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Buildings
{
    public class BuildingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private Building _building;

        [SerializeField]
        private Image iconImage;

        [SerializeField]
        private TMP_Text priceText;

        [SerializeField]
        private LayerMask floorMask;

        private Camera mainCamera;
        private RTSPlayer player;
        private GameObject buildingPreviewInstance;
        private Renderer buildingRendererInstance;
        private BoxCollider buildingCollider;

        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

        private void Start()
        {
            mainCamera = Camera.main;

            iconImage.sprite = _building.GetIcon();
            priceText.text = _building.GetPrice().ToString();

            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();

            buildingCollider = _building.GetComponent<BoxCollider>();
        }

        private void Update()
        {
            if (buildingPreviewInstance)
            {
                UpdateBuildingPreview();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            if (player.GetResources() < _building.GetPrice())
            {
                return;
            }

            buildingPreviewInstance = Instantiate(_building.GetBuildingPreview());
            buildingRendererInstance = buildingPreviewInstance.GetComponentInChildren<Renderer>();

            buildingPreviewInstance.SetActive(false);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (buildingPreviewInstance == null)
            {
                return;
            }

            var ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, floorMask))
            {
                player.CmdTryPlaceBuilding(_building.GetId(), hit.point);
            }

            Destroy(buildingPreviewInstance);
        }

        private void UpdateBuildingPreview()
        {
            var ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, floorMask))
            {
                return;
            }

            buildingPreviewInstance.transform.position = hit.point;

            if (!buildingPreviewInstance.activeSelf)
            {
                buildingPreviewInstance.SetActive(true);
            }

            var color = player.CanPlaceBuilding(buildingCollider, hit.point)
              ? Color.green
              : Color.red;
            buildingRendererInstance.material.SetColor(BaseColor, color);
        }
    }
}
