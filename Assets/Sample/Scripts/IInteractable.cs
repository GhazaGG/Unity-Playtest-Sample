namespace PlaytestSample
{
    // Something the interact key can act on. The server calls Interact; the HUD shows Prompt.
    public interface IInteractable
    {
        string Prompt(PlayerState player);
        void Interact(PlayerState player);
    }
}
