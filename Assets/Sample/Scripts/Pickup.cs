using System.Collections;
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
        [SerializeField] private float _collectTime = 0.25f;

        public string Prompt(PlayerState player) => _kind == Kind.Key ? "E  Take key" : $"E  Take coin ({_value})";

        public void Interact(PlayerState player)
        {
            if (!IsServer || !IsSpawned) return;
            if (_kind == Kind.Key) player.HasKey.Value = true;
            else _scoreBoard.Add(_value);
            CollectRpc();
        }

        // Every peer plays a short shrink, then the server despawns the pickup.
        [Rpc(SendTo.Everyone)]
        private void CollectRpc() => StartCoroutine(Collect());

        private IEnumerator Collect()
        {
            var size = transform.localScale;
            for (var t = 0f; t < _collectTime; t += Time.deltaTime)
            {
                transform.localScale = size * (1f - t / _collectTime);
                yield return null;
            }
            // Placed in the scene: keep the object and hide it, so late joiners agree.
            if (IsServer && IsSpawned) NetworkObject.Despawn(false);
        }

        public override void OnNetworkDespawn() => gameObject.SetActive(false);
    }
}
