using Unity.Netcode;
using UnityEngine;

namespace PlaytestSample
{
    // A coin or the key, placed in the level. The server decides who takes it.
    public class Pickup : NetworkBehaviour, IInteractable
    {
        public enum Kind { Coin, Key }

        [SerializeField] private Kind _kind;
        [SerializeField] private int _value = 10;
        [SerializeField] private ScoreBoard _scoreBoard;

        public string Prompt(PlayerState player) => _kind == Kind.Key ? "E  Take key" : $"E  Take coin ({_value})";

        public void Interact(PlayerState player)
        {
            if (!IsServer || !IsSpawned) return;
            if (_kind == Kind.Key) player.HasKey.Value = true;
            else _scoreBoard.Add(_value);
            NetworkObject.Despawn(false); // placed in the scene: keep the object and hide it, so late joiners agree
        }

        public override void OnNetworkDespawn() => gameObject.SetActive(false);
    }
}
