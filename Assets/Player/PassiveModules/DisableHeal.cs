public class DisableHeal : Module
{
    public override void Load()
    {
        ModulasSave msave = ModulasSaveHandler.GetSave();
        msave.BlockHeal = true;
        ModulasSaveHandler.RewriteSave(msave);
    }
}
