using UnityEngine;

public class WaveSpawn : WaveEvents
{
    [Space()]
    [SerializeField] private SpawnObject[] _spawnObjects;

    protected override void Trigger(int conditionID)
    {
        GameObject go = Instantiate(_spawnObjects[conditionID].gameObject, _spawnObjects[conditionID].spawnPosition, Quaternion.Euler(0, 0, _spawnObjects[conditionID].spawnAngles));
        if (go.GetComponent<DamageBody>())
        {
            Spawner.InitializeHPBar(go.GetComponent<DamageBody>());
        }
        print($"wave trigger spawn: {conditionID}");
    }
}
