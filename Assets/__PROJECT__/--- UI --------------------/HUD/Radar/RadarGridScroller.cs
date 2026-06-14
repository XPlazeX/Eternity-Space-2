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
    [SerializeField] private bool usePlayerRotation;

    private Material _runtimeMaterial;
    private Material _sourceMaterial;

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
        UpdateOffset();
    }

    private void OnEnable()
    {
        ResolvePlayer();
        EnsureRuntimeMaterial();
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

        Vector3 playerPosition = playerTransform.position;
        Vector2 worldPosition = new Vector2(playerPosition.x, playerPosition.y);

        if (usePlayerRotation)
        {
            Vector3 rotatedPosition = Quaternion.Inverse(Player.Orientation) * new Vector3(worldPosition.x, worldPosition.y, 0f);
            worldPosition = new Vector2(rotatedPosition.x, rotatedPosition.y);
        }

        Vector2 offset = baseOffset + Vector2.Scale(worldPosition, worldOffsetMultiplier);
        _runtimeMaterial.SetTextureOffset(AlbedoTexId, offset);
    }
}
