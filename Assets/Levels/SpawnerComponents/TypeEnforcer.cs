using UnityEngine;

public class TypeEnforcer : MonoBehaviour
{
    [SerializeField] private SpawnerModifier _spawnerModifier;
    [SerializeField] private ShipStatModifier[] _statsModifiers;

    public void Start()
    {
        _spawnerModifier.Enforce();

        for (int i = 0; i < _statsModifiers.Length; i++)
        {
            _statsModifiers[i].Enforce();
        }
    }
}
