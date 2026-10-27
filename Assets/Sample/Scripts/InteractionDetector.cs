using UnityEngine;

namespace PlaytestSample
{
    // Finds what the local player is looking at. PlayerInteractor acts on CurrentTarget.
    public class InteractionDetector : MonoBehaviour
    {
        [SerializeField] private Transform _eye;
        [SerializeField] private float _range = 3f;

        public IInteractable CurrentTarget { get; private set; }

        private void Update() => Detect();

        public void Detect()
        {
            CurrentTarget = Physics.Raycast(_eye.position, _eye.forward, out var hit, _range, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide)
                ? hit.collider.GetComponentInParent<IInteractable>()
                : null;
        }
    }
}
