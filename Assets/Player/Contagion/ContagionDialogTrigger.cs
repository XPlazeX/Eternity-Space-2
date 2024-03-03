public class ContagionDialogTrigger : ConditionedDialogTrigger
{
    public override void TryTriggerDialog(int id)
    {
        if (ContagionHandler.ContagionLevel == 0)
            return;
        base.TryTriggerDialog(id);
    }
}
