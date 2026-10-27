using Unity.Netcode;
using UnityEngine;

namespace PlaytestSample
{
    // The interact key (E). The owner sends what its detector sees; the server acts on it.
    public class PlayerInteractor : NetworkBehaviour
    {
        [SerializeField] private InteractionDetector _detector;
        [SerializeField] private PlayerState _state;

        public override void OnNetworkSpawn() => _detector.enabled = IsOwner;

        private void Update()
        {
            if (IsOwner && Input.GetKeyDown(KeyCode.E)) Interact();
        }

        public string Interact()
        {
            if (!(_detector.CurrentTarget is NetworkBehaviour target)) return "nothing in reach";
            InteractServerRpc(target.NetworkObject);
            return "sent " + target.name;
        }

        [Rpc(SendTo.Server)]
        private void InteractServerRpc(NetworkObjectReference targetRef)
        {
            // The client already raycast from its own camera, so the server takes the target as sent.
            if (targetRef.TryGet(out var target) && target.TryGetComponent(out IInteractable interactable))
                interactable.Interact(_state);
        }
    }
}
