using UnityEngine;

public class SpawnPickup : Pickup
{
    [SerializeField] private GameObject _spawningObject;
    [SerializeField] private bool _bindToPlayer;

    private GameObject _spawnedObject;

    protected override void Picked()
    {
        _spawnedObject = Instantiate(_spawningObject, Player.PlayerTransform.position, Quaternion.identity);

        if (_bindToPlayer)
            _spawnedObject.transform.SetParent(Player.PlayerTransform);

        if (_effectDuration > 0f)
            GameObject.FindWithTag("BetweenScenes").GetComponent<TimedDelegator>().FuseAction(NegativeEffect, _effectDuration);

        base.Picked();
    }

    private void NegativeEffect()
    {
        Destroy(_spawnedObject);
    }
}
