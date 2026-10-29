using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlaytestSample
{
    // Host, join and leave, and a small HUD. Sits on the NetworkManager in the Menu scene.
    [RequireComponent(typeof(NetworkManager))]
    public class SessionMenu : MonoBehaviour
    {
        [SerializeField] private string _levelScene = "Level01";
        [SerializeField] private string _menuScene = "Menu";
        [SerializeField] private int _maxPlayers = 4;
        [SerializeField] private ushort _port = 7777;

        private static bool s_commandLineUsed;

        private NetworkManager _network;
        private ScoreBoard _scoreBoard;
        private string _address = "127.0.0.1";
        private bool _menuOpen;

        private void Awake()
        {
            _network = GetComponent<NetworkManager>();
            _network.ConnectionApprovalCallback = Approve;
            _network.OnClientStopped += OnStopped;
            var args = System.Environment.GetCommandLineArgs();
            var port = System.Array.IndexOf(args, "-port");
            if (port >= 0 && port + 1 < args.Length && ushort.TryParse(args[port + 1], out var value)) _port = value;
        }

        private void Start()
        {
            // Quick local tests: start a build with -host, or with -join <address>. Only once, not again after leaving.
            if (s_commandLineUsed) return;
            s_commandLineUsed = true;
            var args = System.Environment.GetCommandLineArgs();
            var join = System.Array.IndexOf(args, "-join");
            if (System.Array.IndexOf(args, "-host") >= 0) Host();
            else if (join >= 0 && join + 1 < args.Length) Join(args[join + 1]);
        }

        public void Host()
        {
            GetComponent<UnityTransport>().ConnectionData.Port = _port;
            if (_network.StartHost()) _network.SceneManager.LoadScene(_levelScene, LoadSceneMode.Single);
        }

        public void Join(string address)
        {
            GetComponent<UnityTransport>().SetConnectionData(address, _port);
            _network.StartClient();
        }

        public void Leave() => _network.Shutdown();

        // Esc opens the in-game menu and back. While it is open the player's own input is off.
        private void Update()
        {
            if (!_network.IsListening || !GameInput.ToggleMenu.WasPressedThisFrame()) return;
            _menuOpen = !_menuOpen;
            if (_menuOpen) GameInput.Player.Disable();
        }

        public void Resume()
        {
            _menuOpen = false;
            GameInput.Player.Enable();
        }

        public string Hud()
        {
            var player = _network.LocalClient?.PlayerObject;
            if (player == null) return "Joining...";
            var state = player.GetComponent<PlayerState>();
            var target = player.GetComponentInChildren<InteractionDetector>().CurrentTarget;
            if (_scoreBoard == null) _scoreBoard = FindAnyObjectByType<ScoreBoard>();
            var coins = _scoreBoard != null ? _scoreBoard.Score.Value : 0;
            return $"Coins: {coins}   Key: {(state.HasKey.Value ? "yes" : "no")}\n{target?.Prompt(state)}";
        }

        private void Approve(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            response.Approved = _network.ConnectedClientsIds.Count < _maxPlayers;
            response.CreatePlayerObject = response.Approved;
            if (!response.Approved) response.Reason = "The session is full.";
        }

        private void OnStopped(bool wasHost)
        {
            // Start over from a fresh menu, which brings its own NetworkManager.
            Resume();
            Destroy(gameObject);
            SceneManager.LoadScene(_menuScene);
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 120), GUI.skin.box);
            if (!_network.IsListening)
            {
                if (GUILayout.Button("Host")) Host();
                _address = GUILayout.TextField(_address);
                if (GUILayout.Button("Join")) Join(_address);
            }
            else if (_menuOpen)
            {
                GUILayout.Label("Menu");
                if (GUILayout.Button("Resume")) Resume();
                if (GUILayout.Button("Leave")) Leave();
            }
            else
            {
                GUILayout.Label(Hud());
                GUILayout.Label("WASD move, hold right mouse to look, E interact, Esc menu");
            }
            GUILayout.EndArea();
        }
    }
}
