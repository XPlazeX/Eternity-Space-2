using UnityEngine;

public class HealOnVictory : Module
{
    [Header("Если использовать ifThisAlive, то ставьте autoLoad")]
    [SerializeField] private bool _ifThisAlive = false;
    [SerializeField] private Vector2Int _gainRange;
    [SerializeField] private bool _useArray;
    [SerializeField] private int[] _gainVariants;

    public override void Load()
    {
        if (_ifThisAlive)
        {
            VictoryHandler.LevelVictored += Heal;
        } else
        {
            int addValue = Random.Range(_gainRange.x, _gainRange.y + 1);

            if (_useArray)
                addValue = _gainVariants[Random.Range(0, _gainVariants.Length)];

            SceneStatics.CharacterCore.GetComponent<VictoryHandler>().AddHealOnVictory(addValue);
        }
    }

    private void OnDisable() 
    {
        if (_ifThisAlive)
        {
            VictoryHandler.LevelVictored -= Heal;
        }
    }

    public void Heal()
    {
        int addValue = Random.Range(_gainRange.x, _gainRange.y + 1);

        if (_useArray)
            addValue = _gainVariants[Random.Range(0, _gainVariants.Length)];

        PlayerShipData.RegenerateHP(addValue);
    }
}
