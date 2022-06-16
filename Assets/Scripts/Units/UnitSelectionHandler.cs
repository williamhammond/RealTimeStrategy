using System.Collections.Generic;
using Buildings;
using Mirror;
using Networking;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Units
{
    public class UnitSelectionHandler : MonoBehaviour
    {
        [SerializeField]
        private LayerMask layerMask;

        [SerializeField]
        private RectTransform unitSelectionArea;

        private Vector2 startPosition;

        private RTSPlayer player;
        private Camera mainCamera;
        public List<Unit> selectedUnits { get; } = new();

        private void Start()
        {
            mainCamera = Camera.main;

            Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
            GameoverHandler.ClientOnGameOver += ClientHandleGameOver;

            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        }

        private void OnDestroy()
        {
            Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
            GameoverHandler.ClientOnGameOver -= ClientHandleGameOver;
        }

        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                StartSelectionArea();
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                ClearSelectionArea();
            }
            else if (Mouse.current.leftButton.isPressed)
            {
                UpdateSelectionArea();
            }
        }

        private void StartSelectionArea()
        {
            if (!Keyboard.current.leftShiftKey.isPressed)
            {
                foreach (var selected in selectedUnits)
                {
                    selected.Deselect();
                }

                selectedUnits.Clear();
            }

            unitSelectionArea.gameObject.SetActive(true);
            startPosition = Mouse.current.position.ReadValue();
            UpdateSelectionArea();
        }

        private void UpdateSelectionArea()
        {
            var mousePosition = Mouse.current.position.ReadValue();

            var areaWidth = mousePosition.x - startPosition.x;
            var areaHeight = mousePosition.y - startPosition.y;

            unitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));
            unitSelectionArea.anchoredPosition =
                startPosition + new Vector2(areaWidth / 2, areaHeight / 2);
        }

        private void ClearSelectionArea()
        {
            unitSelectionArea.gameObject.SetActive(false);
            if (unitSelectionArea.sizeDelta.magnitude == 0)
            {
                var ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (!Physics.Raycast(ray, out var hit, Mathf.Infinity))
                {
                    return;
                }

                if (!hit.collider.TryGetComponent(out Unit unit))
                {
                    return;
                }

                if (!unit.hasAuthority)
                {
                    return;
                }

                selectedUnits.Add(unit);
                foreach (var selected in selectedUnits)
                {
                    selected.Select();
                }

                return;
            }

            var min = unitSelectionArea.anchoredPosition - unitSelectionArea.sizeDelta / 2;
            var max = unitSelectionArea.anchoredPosition + unitSelectionArea.sizeDelta / 2;
            foreach (var unit in player.GetUnits())
            {
                if (selectedUnits.Contains(unit))
                {
                    continue;
                }

                var screenPosition = mainCamera.WorldToScreenPoint(unit.transform.position);
                if (
                    screenPosition.x > min.x
                    && screenPosition.y > min.y
                    && screenPosition.x < max.x
                    && screenPosition.y < max.y
                )
                {
                    selectedUnits.Add(unit);
                    unit.Select();
                }
            }
        }

        private void AuthorityHandleUnitDespawned(Unit unit)
        {
            selectedUnits.Remove(unit);
        }

        private void ClientHandleGameOver(string winner)
        {
            enabled = false;
        }
    }
}
