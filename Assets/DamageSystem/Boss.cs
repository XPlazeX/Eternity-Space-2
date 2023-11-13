using UnityEngine;

[RequireComponent(typeof(DamageBody))]
public class Boss : MonoBehaviour
{
    private DamageBody _damageBody;
    private BossBar _bindedBossBar;

    private void OnEnable() {
        _damageBody = GetComponent<DamageBody>();
        _bindedBossBar = SceneStatics.SceneCore.GetComponent<BossDistributor>().InitializeBossBar(_damageBody.HitPoints);

        _damageBody.DamageTaking += DamageTaken;
        _damageBody.Deathed += Death;
    }

    private void OnDisable() {
        _damageBody.DamageTaking -= DamageTaken;
        _damageBody.Deathed -= Death;
    }

    private void DamageTaken(int hp)
    {
        _bindedBossBar.UpdateLabel(hp);
    }

    private void Death()
    {
        _bindedBossBar.Death();
        SceneStatics.SceneCore.GetComponent<BossDistributor>().UnregisterBossBar();
    }
}
