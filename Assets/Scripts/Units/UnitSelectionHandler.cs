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
        public List<Unit> selectedUnits { get; } = new List<Unit>();

        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                foreach (Unit selected in selectedUnits)
                {
                    selected.Deselect();
                }
                selectedUnits.Clear();
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
            selectedUnits.Add(unit);
            foreach (Unit selected in selectedUnits)
            {
                selected.Select();
            }
        }
    }
}
