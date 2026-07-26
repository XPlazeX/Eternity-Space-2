using UnityEngine;

[CreateAssetMenu(fileName = "Pack", menuName = "Teleplane2/EnemyPack", order = 1)]
public class EnemyPackObject : ScriptableObject // содержит в себе наборы моб-количество-вес
{
    [SerializeField] private EnemyPack[] _enemyPacks;

    public EnemyPack[] EnemyPacks => _enemyPacks;
}
