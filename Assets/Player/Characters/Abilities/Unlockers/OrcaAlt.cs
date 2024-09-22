using UnityEngine;

public class OrcaAlt : MonoBehaviour
{
    private const int achievement_id = 901;

    private void Start() {
        if (Unlocks.HasUnlock(achievement_id))
        {
            Destroy(this);
        }
    }

    private void OnEnable() {
        VictoryHandler.LevelVictored += OnLevelVictoried;
    }

    private void OnDisable() {
        VictoryHandler.LevelVictored -= OnLevelVictoried;
    }

    private void OnLevelVictoried()
    {
        if (GameSessionInfoHandler.GetSessionSave().LocationID == 22)
        {
            Unlocks.NewUnlock(achievement_id);
            Destroy(this);
        }
    }
}
