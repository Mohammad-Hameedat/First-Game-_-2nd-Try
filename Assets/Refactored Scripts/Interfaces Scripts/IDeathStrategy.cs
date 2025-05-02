public interface IDeathStrategy
{
    // Returns true if the object is currently in its death state, false otherwise.
    bool IsDead { get; }

    // Puts the object into its death state.
    void TriggerDeathState();

    // Puts the object back into its revival state.
    void TriggerRevivalState();

    // Sets the object to be dead or alive.
    void ToggleAttachedScripts(bool toggleScriptState);

    // Called when the object is destroyed.
    void ClearFromAllLists();
}
