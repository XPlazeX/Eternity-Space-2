using UnityEngine;

public class GameContagionExecutor : MonoBehaviour
{
    [SerializeField] private Module[] _moduleList;
    [SerializeField] private float _aggroPerContagion;
    [SerializeField] private float _mobilityPerContagion;

    public void Initialize() 
    {
        ContagionHandler.Initialize();

        ShipStats.IncreaseStat("EnemyAggresionMultiplier", _aggroPerContagion * ContagionHandler.ContagionLevel);
        if (ContagionHandler.ContagionLevel >= 2)
        {
            ShipStats.IncreaseStat("EnemyMobilityMultiplier", _mobilityPerContagion * (ContagionHandler.ContagionLevel - 1));
        }

        ModuleSpawn();

        print($"<color=#6666FF>Заражение учтено, модули заспаунены. Уровень заражения: {ContagionHandler.ContagionLevel}</color>");
    }

    private void ModuleSpawn()
    {
        for (int i = 0; i < ContagionHandler.ContagionLevel; i++)
        {
            if (i >= _moduleList.Length)
                break;

            if (_moduleList[i] == null)
                continue;

            ModuleCore.SpawnModule(_moduleList[i]);
        }
    }

    public static void AddContagionWithEffect(int volume)
    {
        ContagionHandler.AddContagion(volume);
        SceneStatics.UICore.GetComponent<PlayerUI>().PlayEffect(PlayerUI.Effect.Contagion, 2.5f * volume);
    }
}
