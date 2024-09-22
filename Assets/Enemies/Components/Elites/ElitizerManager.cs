using UnityEngine;

public class ElitizerManager : MonoBehaviour
{
    [SerializeField] private Elitizer[] _elitizers;
    [SerializeField] private float _minHardness = 1.3f;
    [SerializeField] private float _minSpawnChance = 0.01f;

    private void Start() 
    {
        float hardness = GameSessionInfoHandler.HardnessMultiplier - _minHardness;
        Debug.Log($"<color=cyan>HARDNESS: {GameSessionInfoHandler.HardnessMultiplier}</color>");
        if (hardness < 0f)
        {
            print($"<color=magenta>Elite not avaiable</color>");
            return;
        }
        print($"<color=magenta>Elite chance: {_minSpawnChance + (1f - 1f / (1 + hardness / 3f)) / 3f}</color>");
    }

    public bool WillElitize()
    {
        float hardness = GameSessionInfoHandler.HardnessMultiplier - _minHardness;

        if (hardness < 0f)
            return false;

        float resultChance = _minSpawnChance + (1f - 1f / (1 + hardness / 3f)) / 3f;

        return Random.value < resultChance;
    }

    public void Elitize(DamageBody db)
    {
        _ElitizePivot_ pivot = db.GetComponentInChildren<_ElitizePivot_>();

        if (pivot == null)
            return;

        Elitizer elitizer = Instantiate(_elitizers[Random.Range(0, _elitizers.Length)], pivot.transform.position, pivot.transform.rotation);
        elitizer.transform.SetParent(db.transform);
    }
}
