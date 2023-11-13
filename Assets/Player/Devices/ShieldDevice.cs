using System.Collections;
using UnityEngine;

public class ShieldDevice : Device
{
    [SerializeField] private float _scaleMultiplier = 1f;
    [SerializeField] private int _damage = 10;

    public override void Load()
    {
        base.Load();

        //var shield = ParryingHandler.GetForChangeParringObject(_bulletSampleIndex);
        var shield = CharacterBulletDatabase.GetForChangePullableObject(_bulletIndex).gameObject;
        shield.transform.localScale = Vector3.one * _scaleMultiplier;

        Player.StartPlayerReturn += SetParryingArea;
        SetParryingArea();
    }

    public override void Fire()
    {
        SpawnParryingObject();
    }

    public virtual void SpawnParryingObject()
    {
        ParringObject parringObject = (ParringObject)CharacterBulletDatabase.GetPullableObject(_bulletIndex);

        parringObject.transform.position = Player.PlayerTransform.position;
        parringObject.GetComponent<StaticBullet>().ModdedDamage = _damage; //* ShipStats.GetValue("PlayerFirerateMultiplier")
    }

    protected virtual void SetParryingArea()
    {
        SceneStatics.CharacterCore.GetComponent<ParryingHandler>().SetParryCircle(_scaleMultiplier);
    }

}
