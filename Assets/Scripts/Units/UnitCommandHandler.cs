using Buildings;
using Combat;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Units
{
    public class UnitCommandHandler : MonoBehaviour
    {
        [SerializeField]
        private UnitSelectionHandler _unitSelectionHandler;

        [SerializeField]
        private LayerMask _layerMask;

        private Camera mainCamera;

        private void Start()
        {
            mainCamera = Camera.main;
            GameoverHandler.ClientOnGameOver += ClientHandleGameOver;
        }

        private void Update()
        {
            if (!Mouse.current.rightButton.wasPressedThisFrame)
            {
                return;
            }
            var ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, _layerMask))
            {
                return;
            }

            if (hit.collider.TryGetComponent(out Targetable target))
            {
                if (!target.hasAuthority)
                {
                    TryTarget(target);
                    return;
                }
            }
            TryMove(hit.point);
        }

        private void TryTarget(Targetable target)
        {
            foreach (var unit in _unitSelectionHandler.selectedUnits)
            {
                unit.GetTargeter().CmdSetTarget(target.gameObject);
            }
        }

        private void TryMove(Vector3 point)
        {
            foreach (var unit in _unitSelectionHandler.selectedUnits)
            {
                unit.GetUnitMovement().CmdMove(point);
            }
        }

        private void ClientHandleGameOver(string winner)
        {
            enabled = false;
        }
    }
}
