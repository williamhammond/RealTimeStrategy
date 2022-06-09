using Networking;
using TMPro;
using UnityEngine;

namespace Resources
{
    public class ResourcesDisplay : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text resourcesText = null;

        private RTSPlayer player;

        private void Update()
        {
            if (player == null)
            {
                if (player != null)
                {
                    ClientHandleResourcesUpdated(player.GetResources());
                    player.ClientOnResourcesUpdated += ClientHandleResourcesUpdated;
                }
            }
        }

        private void OnDestroy()
        {
            player.ClientOnResourcesUpdated -= ClientHandleResourcesUpdated;
        }

        private void ClientHandleResourcesUpdated(int resources)
        {
            resourcesText.text = $"Resources: {resources}";
        }
    }
}
