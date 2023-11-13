using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField] private float _lifetime;

    private float _timer = 0;

    private void FixedUpdate() {
        _timer += Time.deltaTime;

        if (_timer >= _lifetime)
            Destroy(gameObject);
    } 
}
