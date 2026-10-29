using UnityEngine.InputSystem;

namespace PlaytestSample
{
    // The game's input actions. Player holds what a player does in the level; Menu stays on so Esc always works.
    public static class GameInput
    {
        public static readonly InputActionMap Player = new("Player");
        public static readonly InputAction Interact = Player.AddAction("Interact", binding: "<Keyboard>/e");

        public static readonly InputActionMap Menu = new("Menu");
        public static readonly InputAction ToggleMenu = Menu.AddAction("ToggleMenu", binding: "<Keyboard>/escape");

        static GameInput()
        {
            Player.Enable();
            Menu.Enable();
        }
    }
}
