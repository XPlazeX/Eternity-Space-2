public class ContagionDialogTrigger : ConditionedDialogTrigger
{
    public override bool TryTriggerDialog(int id)
    {
        if (ContagionHandler.ContagionLevel == 0)
            return false;
        base.TryTriggerDialog(id);
        return true;
    }
}
