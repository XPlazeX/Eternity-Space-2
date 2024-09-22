using UnityEngine;

public class ChildMultiBarrel : AttackPattern
{
    [SerializeField] private int[] _barrelIndexes;

    public override void Fire()
    {
        base.Fire();
        
        for (int i = 0; i < _barrelIndexes.Length; i++)
        {
            Transform t = SpawnBullet().transform;

            t.position = _barrels[_barrelIndexes[i]].position;
            t.rotation = _barrels[_barrelIndexes[i]].rotation;
            t.SetParent(transform);
        }
    }
}
