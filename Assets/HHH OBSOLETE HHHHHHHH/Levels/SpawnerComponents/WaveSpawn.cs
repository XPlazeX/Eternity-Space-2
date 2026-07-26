using UnityEngine;

public class WaveSpawn : WaveEvents
{
    [Space()]
    [SerializeField] private SpawnObject[] _spawnObjects;

    protected override void Trigger(int conditionID)
    {
        _spawnObjects[conditionID].Spawn();
        
        print($"wave trigger spawn: {conditionID}");
    }
}
