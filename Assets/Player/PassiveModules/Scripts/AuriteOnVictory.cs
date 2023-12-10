using UnityEngine;

public class AuriteOnVictory : Module
{
    [SerializeField] private Vector2Int _gainRange;
    [SerializeField] private bool _useArray;
    [SerializeField] private int[] _gainVariants;

    public override void Load()
    {
        int addValue = Random.Range(_gainRange.x, _gainRange.y + 1);

        if (_useArray)
            addValue = _gainRange[Random.Range(0, _gainVariants.Length)];

        SceneStatics.CharacterCore.GetComponent<VictoryHandler>().AddAurite(addValue);
    }
}
