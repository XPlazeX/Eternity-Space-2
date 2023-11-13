using GearCompatibility;
using UnityEngine;

[CreateAssetMenu(fileName = "CompatCatalog", menuName = "Teleplane2/CompatCatalog", order = 0)]
public class CompatCatalog : ScriptableObject {
    [SerializeField] private CompatObject[] _catalog;

    public CompatObject[] Catalog => _catalog;
}
