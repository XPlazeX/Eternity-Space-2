public static class DeathCountRegister
{
    private const string first_death_dialogue = "FirstDeath";
    public static void RegisterDeath()
    {
        if (!Unlocks.HasUnlock(5) && !Unlocks.HasUnlock(6))
            return;

        if (!Unlocks.HasUnlock(4))
        {
            GlobalSave gsave = GlobalSaveHandler.GetSave();
            gsave.LobbyDialogue = first_death_dialogue;
            GlobalSaveHandler.RewriteSave(gsave);

            Unlocks.NewUnlock(4);
        } 
        else
        {
            Unlocks.ProgressUnlock(4, 1);
        }
    }
}
