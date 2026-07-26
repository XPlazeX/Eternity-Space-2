using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField] private float _lifetime;

    private float _timer = 0;

    private void FixedUpdate() {
        _timer += ESTime.worldDeltaTime;

        if (_timer >= _lifetime)
            Destroy(gameObject);
    } 
}
