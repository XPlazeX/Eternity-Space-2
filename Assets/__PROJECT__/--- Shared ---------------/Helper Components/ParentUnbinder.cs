using UnityEngine;

public class ParentUnbinder : MonoBehaviour
{
    private void OnEnable() {
        transform.parent = null;
    }
}
