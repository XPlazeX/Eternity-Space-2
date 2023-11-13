using UnityEngine;

public class RewardScaler : Module
{
    [SerializeField] private float _auriteBoost;
    [SerializeField] private float _cosmiliteBoost;
    [SerializeField] private int _cosmiliteAddition;
    [SerializeField] private int _positroniumAddition;

    public override void Asquiring()
    {
        GameSessionSave save = GameSessionInfoHandler.GetSessionSave();

        if (_auriteBoost != 0)
        {
            save.MoneyPerLevel = Mathf.FloorToInt(save.MoneyPerLevel * (1f +_auriteBoost));
        }
        if (_cosmiliteAddition != 0)
        {
            save.RecievedCosmilite += _cosmiliteAddition;
        }
        if (_cosmiliteBoost != 0)
        {
            save.RecievedCosmilite = Mathf.FloorToInt(save.RecievedCosmilite * (1f +_cosmiliteBoost));
        }
        if (_positroniumAddition != 0)
        {
            save.RecievedPositronium += _positroniumAddition;
        }

        GameSessionInfoHandler.RewriteSessionSave(save);

        print("Получаемая награда увеличена");
    }
}
