using UnityEngine;

public class ActiveByUnlock : MonoBehaviour
{
    [SerializeField] private UnlockRequire[] _unlockRequires;
    [SerializeField] private bool _disableIfHas;

    private void Start() 
    {
        if (!_disableIfHas)
            gameObject.SetActive(Unlocks.HasUnlocks(_unlockRequires));
        else
            gameObject.SetActive(!Unlocks.HasUnlocks(_unlockRequires));
    }
}
