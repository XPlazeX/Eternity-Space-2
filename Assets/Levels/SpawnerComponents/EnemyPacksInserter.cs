using System.Collections.Generic;
using UnityEngine;

public class EnemyPacksInserter : WaveEvents
{
    [Space()]
    [SerializeField] private List<PacksInserter> _enemyPacksList = new List<PacksInserter>();

    protected override void Trigger(int conditionID)
    {
        _enemyPacksList[conditionID].InsertPacks(_edbSpawner);
        print($"wave trigger insert packs: {conditionID}");
    }

    [System.Serializable]
    private class PacksInserter
    {
        [SerializeField] private EnemyPackObject _enemyPackObject;

        public void InsertPacks(EnemyDBSpawner spawner) => spawner.InsertLEDBPacks(_enemyPackObject.EnemyPacks);
    }
}
