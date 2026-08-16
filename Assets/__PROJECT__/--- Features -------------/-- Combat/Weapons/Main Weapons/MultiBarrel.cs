using UnityEngine;

public class MultiBarrel : AttackPattern
{
    [SerializeField] private int[] _barrelIndexes;
    [SerializeField] private MultiBarrelMode multiBarrelMode;

    int _stepIndex;

    public override void Fire()
    {
        base.Fire();
        
        if (multiBarrelMode == MultiBarrelMode.RandomBarrels)
        {
            int id = Random.Range(0, _barrelIndexes.Length);
            SpawnBullet(_barrels[_barrelIndexes[id]].position, _barrels[_barrelIndexes[id]].eulerAngles.z);

            MuzzleFlash(_barrels[_barrelIndexes[id]].position);
        } 
        else if (multiBarrelMode == MultiBarrelMode.StepBarrels)
        {
            SpawnBullet(_barrels[_barrelIndexes[_stepIndex]].position, _barrels[_barrelIndexes[_stepIndex]].eulerAngles.z);
            MuzzleFlash(_barrels[_barrelIndexes[_stepIndex]].position);

            _stepIndex ++;
            if (_stepIndex >= _barrelIndexes.Length)
                _stepIndex = 0;
        }
        else
        {
            for (int i = 0; i < _barrelIndexes.Length; i++)
            {
                SpawnBullet(_barrels[_barrelIndexes[i]].position, _barrels[_barrelIndexes[i]].eulerAngles.z);
                MuzzleFlash(_barrels[_barrelIndexes[i]].position);
            }
        }
    }

    public enum MultiBarrelMode
    {
        AllBarrels,
        StepBarrels,
        RandomBarrels
    }
}
