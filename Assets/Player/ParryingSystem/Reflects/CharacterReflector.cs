using UnityEngine;

[RequireComponent(typeof(WeaponRoot))]
public class CharacterReflector : MonoBehaviour
{
    [SerializeField] private UnlockRequire _unlockRequire;
    [Space()]
    [SerializeField] private PullableObject _triggeringObject;
    [SerializeField] private Transform _spawnPivot;

    private PullForObjects ObjectPool;
    private bool _workable;

    private void OnEnable() 
    {
        _workable = Unlocks.HasUnlock(_unlockRequire);
        print($"reflects: {_workable}");

        if (!_workable)
            return;

        ObjectPool = new PullForObjects(_triggeringObject);
        GetComponent<WeaponRoot>().WeaponCharged += OnWeaponCharged;
    }

    private void OnDisable() {
        if (_workable)
        {
            GetComponent<WeaponRoot>().WeaponCharged -= OnWeaponCharged;
        }
    }

    public void OnWeaponCharged()
    {
        if (!_workable)
            return;

        GameObject projectile = ObjectPool.GetGameObject();

        projectile.transform.position = _spawnPivot.position;
        print($"try reflect");
    }
}
