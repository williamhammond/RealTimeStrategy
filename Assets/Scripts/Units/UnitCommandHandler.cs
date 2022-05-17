using System;
using Combat;
using Units;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommandHandler : MonoBehaviour
{
    [SerializeField]
    private UnitSelectionHandler _unitSelectionHandler = null;

    [SerializeField]
    private LayerMask _layerMask = new LayerMask();

    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
        GameoverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    private void OnDestroy()
    {
        throw new NotImplementedException();
    }

    private void Update()
    {
        if (!Mouse.current.rightButton.wasPressedThisFrame)
        {
            return;
        }
        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _layerMask))
        {
            return;
        }

        if (hit.collider.TryGetComponent<Targetable>(out Targetable target))
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
        foreach (Unit unit in _unitSelectionHandler.selectedUnits)
        {
            unit.GetTargeter().CmdSetTarget(target.gameObject);
        }
    }

    private void TryMove(Vector3 point)
    {
        foreach (Unit unit in _unitSelectionHandler.selectedUnits)
        {
            unit.GetUnitMovement().CmdMove(point);
        }
    }

    private void ClientHandleGameOver(string winner)
    {
        enabled = false;
    }
}
