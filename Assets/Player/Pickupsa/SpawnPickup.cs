using UnityEngine;

public class SpawnPickup : Pickup
{
    [SerializeField] private GameObject _spawningObject;
    [SerializeField] private bool _bindToPlayer;
    [SerializeField] private float _objectLifeTime = -1f;
    [SerializeField] private bool _playShield;

    private GameObject _spawnedObject;

    protected override void Picked()
    {
        _spawnedObject = Instantiate(_spawningObject, Player.PlayerTransform.position, Quaternion.identity);

        if (_bindToPlayer)
            _spawnedObject.transform.SetParent(Player.PlayerTransform);

        if (_objectLifeTime > 0f)
            GameObject.FindWithTag("BetweenScenes").GetComponent<TimedDelegator>().FuseAction(NegativeEffect, _objectLifeTime);

        if (_playShield)
            SceneStatics.UICore.GetComponent<PlayerUI>().PlayEffect(PlayerUI.Effect.Shielding, _objectLifeTime);

        base.Picked();
    }

    private void NegativeEffect()
    {
        Destroy(_spawnedObject);
    }
}
