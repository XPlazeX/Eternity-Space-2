using UnityEngine;

public class ActiveByUnlock : MonoBehaviour
{
    [SerializeField] private UnlockRequire[] _unlockRequires;

    private void Start() {
        gameObject.SetActive(Unlocks.HasUnlocks(_unlockRequires));
    }
}
