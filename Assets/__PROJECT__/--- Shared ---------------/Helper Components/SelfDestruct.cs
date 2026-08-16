using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField] private float _lifetime;

    private float _timer = 0;

    public void SetTimer(float t)
    {
        _lifetime = t;
    }

    private void FixedUpdate() {
        _timer += ESTime.worldDeltaTime;

        if (_timer >= _lifetime)
            Destroy(gameObject);
    } 
}
