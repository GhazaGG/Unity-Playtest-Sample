# Unity Playtest Sample

A tiny co-op Unity 6 project: two rooms, a locked door, a key and some coins, for one to four players over Netcode for
GameObjects. It is the test bed for [unity-playtest](https://github.com/GhazaGG/Unity-Playtest-for-Claude), a Claude Code
skill that playtests Unity games through Unity's Pipeline CLI.

It is deliberately small, and it is **not bug-free**. Try your own playtest on it before you read the skill's evals.

## Requirements

- Unity 6000.3.25f1
- Netcode for GameObjects 2.13.3, Unity Transport 2.7.4
- `com.unity.pipeline` 0.8.0-exp.1 (the CLI the skill uses; the game does not need it)

## Play

Open `Assets/Sample/Scenes/Menu.unity` and press Play, then **Host**. A second player can **Join** `127.0.0.1` from a
build.

- WASD to move, hold the right mouse button to look, E to interact.
- Take the key from the table, unlock the door, and collect the coins in both rooms. Coins count for the whole team.

Builds accept a few command-line switches for quick local tests:

| Switch | Effect |
|---|---|
| `-host` | Start hosting right away |
| `-join <address>` | Join that address right away |
| `-port <port>` | Host or join on this UDP port instead of 7777 (to run two sessions on one PC) |
| `-latency <ms>` | Delay every packet this peer sends (`LatencySimulator`). Measured with NGO's `GetCurrentRtt` on one PC: 0 → ~10 ms, 30 on both peers → ~140 ms, 60 on both → ~275 ms round trip |

## Layout

| Path | What |
|---|---|
| `Assets/Sample/Scenes/Menu.unity` | NetworkManager with `SessionMenu` (host, join, leave, HUD) and `LatencySimulator` |
| `Assets/Sample/Scenes/Level01.unity` | The level: rooms, door, key, coins, `ScoreBoard`, `SpawnPoints` |
| `Assets/Sample/Prefabs/Player.prefab` | First-person player: `PlayerMotor`, `InteractionDetector`, `PlayerInteractor`, `PlayerState` |
| `Assets/Sample/Scripts/` | All game code, assembly `PlaytestSample` |

## License

MIT
