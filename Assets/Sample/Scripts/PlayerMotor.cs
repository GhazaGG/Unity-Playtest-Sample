using Unity.Netcode;
using UnityEngine;

namespace PlaytestSample
{
    // First-person walking and looking for the owner. NetworkTransform (owner authority) sends the result.
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMotor : NetworkBehaviour
    {
        [SerializeField] private Transform _cameraPivot;
        [SerializeField] private float _speed = 4f;
        [SerializeField] private float _lookSpeed = 2f;

        private CharacterController _controller;
        private float _pitch;
        private float _verticalSpeed;
        private bool _placed;

        public Transform CameraPivot => _cameraPivot;

        private void Awake() => _controller = GetComponent<CharacterController>();

        public override void OnNetworkSpawn() => _cameraPivot.gameObject.SetActive(IsOwner);

        private void Update()
        {
            if (!IsOwner) return;
            // Players are created in the menu and carried into the level; wait for its spawn points.
            if (!_placed)
            {
                _placed = TryPlaceAtSpawn();
                return;
            }

            if (Input.GetMouseButton(1)) // the cursor stays free for the menu buttons
            {
                transform.Rotate(0f, Input.GetAxis("Mouse X") * _lookSpeed, 0f);
                _pitch = Mathf.Clamp(_pitch - Input.GetAxis("Mouse Y") * _lookSpeed, -80f, 80f);
                _cameraPivot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
            }

            var input = Vector3.ClampMagnitude(new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")), 1f);
            var move = transform.TransformDirection(input) * _speed;
            _verticalSpeed = _controller.isGrounded ? -1f : _verticalSpeed + Physics.gravity.y * Time.deltaTime;
            move.y = _verticalSpeed;
            _controller.Move(move * Time.deltaTime);
        }

        private bool TryPlaceAtSpawn()
        {
            var spawns = FindAnyObjectByType<SpawnPoints>();
            if (spawns == null) return false;
            var point = spawns.For(OwnerClientId);
            _controller.enabled = false;
            transform.SetPositionAndRotation(point.position, point.rotation);
            _controller.enabled = true;
            return true;
        }
    }
}
