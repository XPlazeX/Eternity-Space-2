using UnityEngine;

public class InstantiateAbility : Ability
{
    [SerializeField] private GameObject _spawningObject;
    [SerializeField] private bool _bindObjectToPlayer;
    [SerializeField] private bool _useFieldAttackObject;
    [SerializeField] private FieldAttackModule.FieldAttackObject _attackObject;
    [SerializeField] private bool _useBarrels = false;
    [SerializeField] private int[] _barrelIndexes;

    public override void Use()
    {
        if (!_useFieldAttackObject)
            InstantiateUse();
        else
            AttackUse();
    }

    public void InstantiateUse()
    {
        if (_useBarrels)
        {
            WeaponRoot wr = Player.PlayerObject.GetComponent<WeaponRoot>();

            for (int i = 0; i < _barrelIndexes.Length; i++)
            {
                InstantiateObject(wr.PlayerBarrels[_barrelIndexes[i]].position);
            }
        }

        else{
            InstantiateObject(Player.PlayerTransform.position);
        }
    }

    private void InstantiateObject(Vector3 position)
    {
        GameObject proj = Instantiate(_spawningObject, position, Quaternion.identity);

        if (_bindObjectToPlayer)
        {
            proj.transform.SetParent(Player.PlayerTransform);
        }
    }

    public void AttackUse()
    {
        _attackObject.Fire();
    }
}
