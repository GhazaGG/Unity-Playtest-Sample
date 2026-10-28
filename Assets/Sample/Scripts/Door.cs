using Unity.Netcode;
using UnityEngine;

namespace PlaytestSample
{
    // Opens for a player who holds the key. The server decides; every peer swings the hinge.
    public class Door : NetworkBehaviour, IInteractable
    {
        [SerializeField] private Transform _hinge;
        [SerializeField] private float _openAngle = 90f;

        public readonly NetworkVariable<bool> IsOpen = new();

        public override void OnNetworkSpawn() => IsOpen.OnValueChanged += OnOpenChanged;

        public override void OnNetworkDespawn() => IsOpen.OnValueChanged -= OnOpenChanged;

        public string Prompt(PlayerState player) => IsOpen.Value ? "" : player.HasKey.Value ? "E  Unlock" : "Locked";

        public void Interact(PlayerState player)
        {
            if (IsServer && !IsOpen.Value && player.HasKey.Value) IsOpen.Value = true;
        }

        private void OnOpenChanged(bool previous, bool current) => Swing();

        private void Swing() => _hinge.localRotation = Quaternion.Euler(0f, IsOpen.Value ? _openAngle : 0f, 0f);
    }
}
