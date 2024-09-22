using UnityEngine;

public class MultiBarrel : AttackPattern
{
    [SerializeField] private int[] _barrelIndexes;
    [SerializeField] private bool _randomBarrel;

    public override void Fire()
    {
        base.Fire();
        
        if (_randomBarrel)
        {
            int id = Random.Range(0, _barrelIndexes.Length);
            SpawnBullet(_barrels[_barrelIndexes[id]].position, _barrels[_barrelIndexes[id]].eulerAngles.z);
        } 
        else
        {
            for (int i = 0; i < _barrelIndexes.Length; i++)
            {
                SpawnBullet(_barrels[_barrelIndexes[i]].position, _barrels[_barrelIndexes[i]].eulerAngles.z);
            }
        }
    }
}
