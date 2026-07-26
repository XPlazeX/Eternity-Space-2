using UnityEngine;

public class SailfishAlt : MonoBehaviour
{
    private const int achievement_id = 903;

    private bool _startCondition = false;

    private void Start() {
        if (Unlocks.HasUnlock(achievement_id))
        {
            Destroy(this);
        }

        _startCondition = PlayerShipData.HitPoints == 0;
    }

    private void OnEnable() {
        VictoryHandler.LevelVictored += OnLevelVictoried;
    }

    private void OnDisable() {
        VictoryHandler.LevelVictored -= OnLevelVictoried;
    }

    private void OnLevelVictoried()
    {
        if (_startCondition && PlayerShipData.HitPoints >= 45)
        {
            Unlocks.NewUnlock(achievement_id);
            Destroy(this);
        }
    }
}
