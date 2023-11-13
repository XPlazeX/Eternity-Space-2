using UnityEngine;

[RequireComponent(typeof(AttackPattern))]
public class ShakeOnFire : MonoBehaviour
{
    [SerializeField] private float _shakePower = 1f;
    [SerializeField] private float _shakeDurationMultiplier = 1;

    private void Start() {
        GetComponent<AttackPattern>().Fired += OnFire;
    }

    private void OnFire()
    {
        CameraController.Shake(_shakePower, _shakeDurationMultiplier);
    }

}
