using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Units
{
    public class UnitSelectionHandler : MonoBehaviour
    {
        [SerializeField]
        private LayerMask layerMask;

        private Camera _mainCamera;
        private readonly List<Unit> _selectedUnits = new List<Unit>();

        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                foreach (Unit selected in _selectedUnits)
                {
                    selected.Deselect();
                }
                _selectedUnits.Clear();
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                ClearSelectionArea();
            }
        }

        private void ClearSelectionArea()
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
            _selectedUnits.Add(unit);
            foreach (Unit selected in _selectedUnits)
            {
                selected.Select();
            }
        }
    }
}
