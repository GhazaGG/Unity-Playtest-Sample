using UnityEngine;

namespace PlaytestSample
{
    public class SpawnPoints : MonoBehaviour
    {
        [SerializeField] private Transform[] _points;

        public Transform For(ulong clientId) => _points[(int)(clientId % (ulong)_points.Length)];
    }
}
