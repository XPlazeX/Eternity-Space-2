using UnityEngine;

public class _DisableTimeControl : MonoBehaviour
{
    private void OnEnable() {
        TimeHandler.Workable = false;
    }

    private void OnDisable() {
        TimeHandler.Workable = true;
    }
}
