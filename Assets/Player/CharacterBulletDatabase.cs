using System.Collections.Generic;
using UnityEngine;
using DamageSystem;

public class CharacterBulletDatabase : MonoBehaviour
{
    private static List<PullForObjects> BulletPools = new List<PullForObjects>();

    public void Initialize()
    {
        BulletPools.Clear();
    }

    public static int InitializeAttackSample(AttackObject sample)
    {
        BulletPools.Add(new PullForObjects(sample as AttackObject));

        return BulletPools.Count - 1;
    }

    public static int InitializePullableObject(PullableObject sample)
    {
        BulletPools.Add(new PullForObjects(sample));

        return BulletPools.Count - 1;
    }

    public static AttackObject GetAttackObject(int index)
    {
        return BulletPools[index].GetGameObject().GetComponent<AttackObject>();
    }

    public static AttackObject GetForChangeAttackObject(int index)
    {
        BulletPools[index].SampleChanged();
        return BulletPools[index].Sample as AttackObject;
    }

    public static PullableObject GetPullableObject(int index)
    {
        return BulletPools[index].GetGameObject().GetComponent<PullableObject>();
    }

    public static PullableObject GetForChangePullableObject(int index)
    {
        BulletPools[index].SampleChanged();
        return BulletPools[index].Sample as PullableObject;
    }
}
