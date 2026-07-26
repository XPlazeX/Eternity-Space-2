using UnityEngine;

public class SpawnOnAsquiring : Module
{
    [SerializeField] private SpawnObject[] _spawningObjects;

    public override void Asquiring()
    {
        for (int i = 0; i < _spawningObjects.Length; i++)
        {
            _spawningObjects[i].Spawn();
        }
    }
}
