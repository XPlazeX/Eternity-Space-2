using UnityEngine;

public class WarpDriveFieldControllerByDelta : MonoBehaviour
{
    [SerializeField] private SpriteRenderer targetRenderer;

    [SerializeField] private float minDeltaPerSecond = 0.5f;
    [SerializeField] private float fullDeltaPerSecond = 10f;

    [SerializeField] private float appearSpeed = 6f;
    [SerializeField] private float disappearSpeed = 3f;

    [SerializeField] private string warpAmountProperty = "_WarpAmount";

    private MaterialPropertyBlock _mpb;
    private Vector3 _lastPosition;
    private float _currentWarp;

    private void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<SpriteRenderer>();

        _mpb = new MaterialPropertyBlock();
        _lastPosition = transform.position;
    }

    private void LateUpdate()
    {
        Vector3 currentPosition = transform.position;
        Vector3 delta = currentPosition - _lastPosition;
        float deltaPerSecond = delta.magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
        _lastPosition = currentPosition;

        float targetWarp = 0f;
        if (deltaPerSecond > minDeltaPerSecond)
            targetWarp = Mathf.InverseLerp(minDeltaPerSecond, fullDeltaPerSecond, deltaPerSecond) * PlayerController.RelativeRelativity;

        float speed = targetWarp > _currentWarp ? appearSpeed : disappearSpeed;
        _currentWarp = Mathf.MoveTowards(_currentWarp, targetWarp, speed * Time.deltaTime);

        targetRenderer.GetPropertyBlock(_mpb);
        _mpb.SetFloat(warpAmountProperty, _currentWarp);
        targetRenderer.SetPropertyBlock(_mpb);
        transform.up = SceneStatics.FlatVector(Vector3.RotateTowards(transform.up, delta.normalized, 3f * Time.deltaTime, 3f * Time.deltaTime));
    }
}