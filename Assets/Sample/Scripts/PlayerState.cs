using Unity.Netcode;
using UnityEngine;

namespace PlaytestSample
{
    public class PlayerState : NetworkBehaviour
    {
        private static readonly Color[] Colors = { new(0.25f, 0.5f, 1f), new(0.3f, 0.85f, 0.4f), new(1f, 0.45f, 0.3f), new(0.8f, 0.4f, 0.9f) };

        [SerializeField] private Renderer _body;

        public readonly NetworkVariable<bool> HasKey = new();

        public override void OnNetworkSpawn()
        {
            _body.material.color = Colors[OwnerClientId % (ulong)Colors.Length];
            // The owner's camera sits inside the body, so only other players see it.
            foreach (var part in _body.GetComponentsInChildren<Renderer>()) part.enabled = !IsOwner;
        }
    }
}
