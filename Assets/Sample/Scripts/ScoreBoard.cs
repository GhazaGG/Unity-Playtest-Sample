using Unity.Netcode;
using UnityEngine;

namespace PlaytestSample
{
    // The team's coin total. Only the server adds to it.
    public class ScoreBoard : NetworkBehaviour
    {
        [SerializeField] private AudioClip _coinClip;

        public readonly NetworkVariable<int> Score = new();

        public override void OnNetworkSpawn() => Score.OnValueChanged += OnScoreChanged;

        public override void OnNetworkDespawn() => Score.OnValueChanged -= OnScoreChanged;

        public void Add(int value)
        {
            if (IsServer) Score.Value += value;
        }

        private void OnScoreChanged(int previous, int current)
        {
            if (current > previous && _coinClip != null) AudioSource.PlayClipAtPoint(_coinClip, transform.position);
        }
    }
}
