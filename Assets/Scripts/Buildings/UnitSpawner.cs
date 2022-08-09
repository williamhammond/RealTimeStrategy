using Combat;
using Mirror;
using Networking;
using TMPro;
using Units;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Buildings
{
    public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private Unit unitPrefab;

        [SerializeField]
        private Transform unitSpawnPoint;

        [SerializeField]
        private Health health;

        [SerializeField]
        private TMP_Text remainingUnitsText;

        [SerializeField]
        private Image unitProgressImage;

        [SerializeField]
        private int maxUnitQueue = 5;

        [SerializeField]
        private float spawnMoveRange = 10f;

        [SerializeField]
        private float unitSpawnDuration = 5f;

        [SyncVar(hook = nameof(ClientHandleQueuedUnitsUpdated))]
        private int _queuedUnits;

        [SyncVar]
        private float _unitTimer;

        private RTSPlayer _player;
        private float _progressImageVelocity;

        private void Update()
        {
            if (_player == null)
            {
                _player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
            }
            if (isServer)
            {
                ProduceUnits();
            }
            if (isClient)
            {
                UpdateTimerDisplay();
            }
        }

        #region Server

        public override void OnStartServer()
        {
            health.ServerOnDie += ServerHandleDie;
        }

        public override void OnStopServer()
        {
            health.ServerOnDie -= ServerHandleDie;
        }

        [Server]
        private void ProduceUnits()
        {
            if (_queuedUnits == 0)
                return;

            _unitTimer += Time.deltaTime;
            if (_unitTimer > unitSpawnDuration)
            {
                var unitInstance = Instantiate(
                    unitPrefab,
                    unitSpawnPoint.position,
                    unitSpawnPoint.rotation
                );
                NetworkServer.Spawn(unitInstance.gameObject, connectionToClient);

                Vector3 spawnOffset = Random.insideUnitCircle * spawnMoveRange;
                spawnOffset.y = unitSpawnPoint.position.y;
                var unitMovement = unitInstance.GetUnitMovement();
                unitMovement.ServerMove(unitSpawnPoint.position + spawnOffset);

                _queuedUnits--;
                _unitTimer = 0f;
            }
        }

        [Command]
        private void CmdSpawnUnit()
        {
            if (_queuedUnits == maxUnitQueue)
            {
                return;
            }

            var resources = connectionToClient.identity.GetComponent<RTSPlayer>().GetResources();
            if (resources >= unitPrefab.GetCost())
            {
                _queuedUnits++;
                _player.SetResources(_player.GetResources() - unitPrefab.GetCost());
            }
        }

        [Server]
        private void ServerHandleDie()
        {
            NetworkServer.Destroy(gameObject);
        }

        #endregion

        #region Client
        private void ClientHandleQueuedUnitsUpdated(int oldQueuedUnits, int newQueuedUnits)
        {
            remainingUnitsText.text = $"{newQueuedUnits}";
        }

        private void UpdateTimerDisplay()
        {
            var newProgress = _unitTimer / unitSpawnDuration;
            if (newProgress < unitProgressImage.fillAmount)
            {
                unitProgressImage.fillAmount = newProgress;
            }
            else
            {
                unitProgressImage.fillAmount = Mathf.SmoothDamp(
                    unitProgressImage.fillAmount,
                    newProgress,
                    ref _progressImageVelocity,
                    0.1f
                );
            }
        }

        #endregion

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            if (!hasAuthority)
            {
                return;
            }
            CmdSpawnUnit();
        }
    }
}
