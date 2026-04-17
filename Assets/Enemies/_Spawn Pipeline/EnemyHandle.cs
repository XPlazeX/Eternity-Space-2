using System;
using UnityEngine;

/// Спаунер цепляет хендл на DamageBody и возвращает в EncounterParser
/// EncounterParser подписывается на коллбек Died у EnemyHandle
public class EnemyHandle : MonoBehaviour
{
    public event Action<EnemyHandle> DiedCallback;

    [NonSerialized] public float weight;
    [NonSerialized] public bool isBoss;

    public void Bind(DamageBody damageBody)
    {
        damageBody.Deathed += OnDeathed;
    }

    public void OnDeathed()
    {
        DiedCallback?.Invoke(this);
    }
}
