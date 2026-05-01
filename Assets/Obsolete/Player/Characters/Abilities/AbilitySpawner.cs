using UnityEngine;

public class AbilitySpawner : MonoBehaviour
{
    [SerializeField] private Ability[] _abilityVariations;

    private void Start() 
    {
        if (_abilityVariations.Length == 0)
            return;

        Ability ability = Instantiate(_abilityVariations[Mathf.Clamp(GameSessionInfoHandler.GetSessionSave().AbilityID, 0, _abilityVariations.Length)]);
        ability.Load();
    }
}
