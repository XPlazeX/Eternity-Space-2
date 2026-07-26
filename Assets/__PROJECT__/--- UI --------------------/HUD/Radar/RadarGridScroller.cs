using UnityEngine;
using UnityEngine.UI;

public class RadarGridScroller : MonoBehaviour
{
    private static readonly int AlbedoTexId = Shader.PropertyToID("_AlbedoTex");

    [Header("References")]
    [SerializeField] private Image targetImage;
    [SerializeField] private Transform playerTransform;

    [Header("Scroll")]
    [SerializeField] private Vector2 worldOffsetMultiplier = Vector2.one * 0.01f;
    [SerializeField] private Vector2 baseOffset;

    private Material _runtimeMaterial;
    private Material _sourceMaterial;
    private Vector2 _accumulatedLocalPosition;
    private Vector3 _previousPlayerPosition;
    private bool _hasPreviousPlayerPosition;

    private void Reset()
    {
        targetImage = GetComponent<Image>();
    }

    private void Awake()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();

        ResolvePlayer();
        EnsureRuntimeMaterial();
        ResetScrollTracking();
        UpdateOffset();
    }

    private void OnEnable()
    {
        ResolvePlayer();
        EnsureRuntimeMaterial();
        ResetScrollTracking();
        UpdateOffset();
    }

    private void OnDisable()
    {
        if (targetImage != null && targetImage.material == _runtimeMaterial)
            targetImage.material = _sourceMaterial;

        if (_runtimeMaterial != null)
            Destroy(_runtimeMaterial);

        _runtimeMaterial = null;
        _sourceMaterial = null;
        _hasPreviousPlayerPosition = false;
    }

    private void Update()
    {
        ResolvePlayer();
        EnsureRuntimeMaterial();
        UpdateOffset();
    }

    private void ResolvePlayer()
    {
        if (playerTransform != null)
            return;

        if (Player.PlayerTransform != null)
            playerTransform = Player.PlayerTransform;
    }

    private void EnsureRuntimeMaterial()
    {
        if (targetImage == null)
            return;

        Material currentMaterial = targetImage.material;

        if (_runtimeMaterial != null && currentMaterial == _runtimeMaterial)
            return;

        if (_runtimeMaterial != null)
            Destroy(_runtimeMaterial);

        _sourceMaterial = currentMaterial;

        if (_sourceMaterial == null)
            return;

        _runtimeMaterial = new Material(_sourceMaterial)
        {
            name = $"{_sourceMaterial.name} (Radar Grid Instance)"
        };

        targetImage.material = _runtimeMaterial;
    }

    private void UpdateOffset()
    {
        if (_runtimeMaterial == null || playerTransform == null)
            return;

        if (!_hasPreviousPlayerPosition)
            ResetScrollTracking();

        Vector3 playerPosition = playerTransform.position;
        Vector3 worldDelta = playerPosition - _previousPlayerPosition;
        _previousPlayerPosition = playerPosition;

        Vector3 playerRelativeDelta = Quaternion.Inverse(playerTransform.rotation) * worldDelta;
        _accumulatedLocalPosition += new Vector2(playerRelativeDelta.x, playerRelativeDelta.y);

        Vector2 offset = baseOffset + Vector2.Scale(_accumulatedLocalPosition, worldOffsetMultiplier);
        _runtimeMaterial.SetTextureOffset(AlbedoTexId, offset);
    }

    private void ResetScrollTracking()
    {
        if (playerTransform == null)
            return;

        _previousPlayerPosition = playerTransform.position;
        _hasPreviousPlayerPosition = true;
    }
}
