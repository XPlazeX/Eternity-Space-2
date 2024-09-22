using System.Collections.Generic;
using UnityEngine;

public class OrbHandler : MonoBehaviour
{
    [SerializeField] private Orb[] _orbSamples;

    private static List<PullForObjects> OrbPools = new List<PullForObjects>();

    public void Start()
    {
        OrbPools.Clear();

        for (int i = 0; i < _orbSamples.Length; i++)
        {
            OrbPools.Add(new PullForObjects(_orbSamples[i] as Orb));
        }

        SceneStatics.SceneCore.GetComponent<ExplosionHandler>().PreloadExplosion(Bullet.parryExplosionID);
    }

    public static void SpawnOrb(int id, Vector3 startPosition, Transform targetTransform)
    {
        Orb orb = OrbPools[id].GetGameObject().GetComponent<Orb>();
        orb.Set(startPosition, targetTransform);
    }
}
