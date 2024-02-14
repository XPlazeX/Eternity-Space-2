using UnityEngine;

[RequireComponent(typeof(DamageBody))]
public class DamageBodyHpMultiplier : MonoBehaviour
{
    [SerializeField] private float _multiplier = 1f;

    private void Start() {
        GetComponent<DamageBody>().MultiplyHP(_multiplier);
    }
}
