using UnityEngine;

public class GravityPlayer : MonoBehaviour
{
    [SerializeField] private float _gravityPower;

    private void Update() {
        if (Player.PlayerTransform != null)
            transform.position += (Player.PlayerTransform.position - transform.position).normalized * (1f / Mathf.Pow((Player.PlayerTransform.position - transform.position).magnitude, 2f)) * Time.deltaTime * _gravityPower;
    }
}
