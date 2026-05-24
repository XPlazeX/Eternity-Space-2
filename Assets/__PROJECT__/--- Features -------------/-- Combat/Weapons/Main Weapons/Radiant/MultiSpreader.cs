using DamageSystem;
using UnityEngine;

public class MultiSpreader : Spreader
{
    [SerializeField] private Spreaderlayer[] spreaderLayers;

    public override void Fire()
    {
        base.Fire();

        for (int i = 0; i < spreaderLayers.Length; i++)
        {
            float startAngle = -spreaderLayers[i].angleStep * ((spreaderLayers[i].bulletCount - 1) / 2f);
        
            for (int j = 0; j < spreaderLayers[i].bulletCount; j++)
            {
                SpawnBullet(spreaderLayers[i].attackObject, _barrels[0].position, startAngle + _barrels[0].eulerAngles.z);
                startAngle += spreaderLayers[i].angleStep;
            }
        }
    }

    [System.Serializable]
    private struct Spreaderlayer
    {
        public AttackObject attackObject;
        public int bulletCount;
        public float angleStep;
    }
}
