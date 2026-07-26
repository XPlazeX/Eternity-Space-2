using UnityEngine;

public class ActiveByUnlock : MonoBehaviour
{
    [SerializeField] private UnlockRequire[] _unlockRequires;
    [SerializeField] private bool _anyRequirement = false;
    [SerializeField] private bool _disableIfHas;

    private void Start() 
    {
        if (!_disableIfHas)
            gameObject.SetActive(_anyRequirement ? Unlocks.HasAnyUnlocks(_unlockRequires) : Unlocks.HasUnlocks(_unlockRequires));
        else
            gameObject.SetActive(_anyRequirement ? !Unlocks.HasAnyUnlocks(_unlockRequires) : !Unlocks.HasUnlocks(_unlockRequires));
    }
}
