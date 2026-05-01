using UnityEngine;

public class TrailReloader : MonoBehaviour
{
    private TrailRenderer _trailRenderer;

    private void Awake() {
        _trailRenderer = GetComponent<TrailRenderer>();
    }

    private void OnDisable() {
        _trailRenderer.Clear();
    }
}
