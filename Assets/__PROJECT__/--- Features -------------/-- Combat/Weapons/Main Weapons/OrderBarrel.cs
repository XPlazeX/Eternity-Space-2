using UnityEngine;

public class OrderBarrel : AttackPattern
{
    [SerializeField] private int[] _barrelIndexesOrder;

    private int _lastID;

    public override void Fire()
    {
        base.Fire();
        
        SpawnBullet(_barrels[_barrelIndexesOrder[_lastID]].position, _barrels[_barrelIndexesOrder[_lastID]].eulerAngles.z);

        _lastID ++;
        if (_lastID == _barrelIndexesOrder.Length)
            _lastID = 0;
    }
}
