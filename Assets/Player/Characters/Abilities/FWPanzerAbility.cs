using UnityEngine;

public class FWPanzerAbility : Ability
{
    [SerializeField] private float _waitTime;

    private FWTurret _fwTurret;

    public override void Load()
    {
        base.Load();
        _fwTurret = Player.PlayerObject.GetComponent<FWTurret>();
    }

    public override void Use()
    {
        PlayerShipData.BreakArmor();

        AttackModule[] ams = _fwTurret.SpawnedTurret.GetComponents<AttackModule>();

        for (int i = 0; i < ams.Length; i++)
        {
            ams[i].HandFireSeries(0, _waitTime);
        }
    }
}
