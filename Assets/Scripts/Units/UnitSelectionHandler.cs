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

        private Vector2 _startPosition;

        private RTSPlayer _player;
        private Camera _mainCamera;
        public List<Unit> selectedUnits { get; } = new List<Unit>();

        private void Start()
        {
            _mainCamera = Camera.main;

            Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
            GameoverHandler.ClientOnGameOver += ClientHandleGameOver;

            _player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
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
                foreach (Unit selected in selectedUnits)
                {
                    selected.Deselect();
                }

                selectedUnits.Clear();
            }

            unitSelectionArea.gameObject.SetActive(true);
            _startPosition = Mouse.current.position.ReadValue();
            UpdateSelectionArea();
        }

        private void UpdateSelectionArea()
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            float areaWidth = mousePosition.x - _startPosition.x;
            float areaHeight = mousePosition.y - _startPosition.y;

            unitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));
            unitSelectionArea.anchoredPosition =
                _startPosition + new Vector2(areaWidth / 2, areaHeight / 2);
        }

        private void ClearSelectionArea()
        {
            unitSelectionArea.gameObject.SetActive(false);
            if (unitSelectionArea.sizeDelta.magnitude == 0)
            {
                Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
                {
                    return;
                }

                if (!hit.collider.TryGetComponent<Unit>(out Unit unit))
                {
                    return;
                }

                if (!unit.hasAuthority)
                {
                    return;
                }

                selectedUnits.Add(unit);
                foreach (Unit selected in selectedUnits)
                {
                    selected.Select();
                }

                return;
            }

            Vector2 min = unitSelectionArea.anchoredPosition - (unitSelectionArea.sizeDelta / 2);
            Vector2 max = unitSelectionArea.anchoredPosition + (unitSelectionArea.sizeDelta / 2);
            foreach (Unit unit in _player.GetUnits())
            {
                if (selectedUnits.Contains(unit))
                {
                    continue;
                }

                Vector3 screenPosition = _mainCamera.WorldToScreenPoint(unit.transform.position);
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
