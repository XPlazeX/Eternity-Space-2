using System;
using UnityEngine;

/// Спаунер цепляет хендл на DamageBody и возвращает в EncounterParser
/// EncounterParser подписывается на коллбек Died у EnemyHandle
public class EnemyHandle : MonoBehaviour
{
    public event Action<EnemyHandle> DiedCallback;

    public float weight;
    [NonSerialized] public bool isBoss;
    [NonSerialized] public bool isElite;

    public void Bind(DamageBody damageBody)
    {
        damageBody.Deathed += OnDeathed;
    }

    public void OnDeathed()
    {
        DiedCallback?.Invoke(this);
    }
}
