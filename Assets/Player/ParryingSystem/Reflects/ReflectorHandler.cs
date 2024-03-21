using System.Collections.Generic;
using UnityEngine;
using DamageSystem;

public class ReflectorHandler : MonoBehaviour
{
    public delegate void reflectHandler();
    public static event reflectHandler PlayerReflected;
    public static event reflectHandler EnemyReflected;

    [SerializeField] private AttackObject[] _reflectedProjectiles;

    private static List<PullForObjects> ReflectingPools = new List<PullForObjects>();

    public void Initialize()
    {
        ReflectingPools.Clear();

        for (int i = 0; i < _reflectedProjectiles.Length; i++)
        {
            ReflectingPools.Add(new PullForObjects(_reflectedProjectiles[i]));
        }
    }

    public static AttackObject GetReflectedProjectile(int id, DamageKey newDamageKey, bool isPlayerReflected = true)
    {
        if (isPlayerReflected)
        {
            PlayerReflected?.Invoke();
        } else
        {
            EnemyReflected?.Invoke();
        }

        AttackObject result = ReflectingPools[id].GetGameObject().GetComponent<AttackObject>();

        result.ChangeDamageKey(newDamageKey);

        return result;
    }
}
