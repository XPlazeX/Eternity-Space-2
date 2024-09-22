using System.Collections.Generic;

[System.Serializable]
public class ReviveSave
{
    public List<string> ReviveOrder = new List<string>();

    public bool CanRevive => ReviveOrder.Count > 0;

    public void RegisterRevive(string code)
    {
        ReviveOrder.Add(code);
    }

    public string ExtractNearestRevive()
    {
        if (!CanRevive)
            return null;

        string reviveCode = ReviveOrder[0];
        ReviveOrder.RemoveAt(0);

        return reviveCode;
    }
}
