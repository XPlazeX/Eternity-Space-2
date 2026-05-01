using UnityEngine;

[ExecuteAlways]
public sealed class LevelDirectionalLightSettings : MonoBehaviour
{
    [Header("Direction")]
    [Tooltip("0 = light from right, 90 = from up, 180 = from left, 270 = from down.")]
    [Range(0f, 360f)]
    public float angleDegrees = 90f;

    [Header("Lighting Multipliers")]
    [Tooltip("Brightness of flat pixels: direction index 0.")]
    [Min(0f)]
    public float flatMul = 0.8f;

    [Tooltip("Brightness of pixels facing the light.")]
    [Min(0f)]
    public float towardMul = 1.15f;

    [Tooltip("Brightness of pixels facing away from the light.")]
    [Min(0f)]
    public float awayMul = 0.25f;

    [Header("Shadow Tint")]
    public Color shadowTint = new Color(0.28f, 0.34f, 0.50f, 1f);

    [Range(0f, 1f)]
    public float shadowTintStrength = 0.5f;

    [Header("Cavity / Crevice Shadow")]
    [Tooltip("How strongly blue channel of Direction Map darkens the pixel.")]
    [Range(0f, 1f)]
    public float cavityDarkness = 0.6f;

    static readonly int GlobalLightDirId = Shader.PropertyToID("_ES_GlobalLightDir");
    static readonly int FlatMulId = Shader.PropertyToID("_ES_FlatMul");
    static readonly int TowardMulId = Shader.PropertyToID("_ES_TowardMul");
    static readonly int AwayMulId = Shader.PropertyToID("_ES_AwayMul");

    static readonly int ShadowTintId = Shader.PropertyToID("_ES_ShadowTint");
    static readonly int ShadowTintStrengthId = Shader.PropertyToID("_ES_ShadowTintStrength");
    static readonly int CavityDarknessId = Shader.PropertyToID("_ES_CavityDarkness");

    void OnEnable()
    {
        Apply();
    }

    void OnValidate()
    {
        Apply();
    }

    void Update()
    {
        Apply();
    }

    public void Apply()
    {
        float radians = angleDegrees * Mathf.Deg2Rad;

        Vector2 dir = new Vector2(
            Mathf.Cos(radians),
            Mathf.Sin(radians)
        ).normalized;

        Shader.SetGlobalVector(GlobalLightDirId, new Vector4(dir.x, dir.y, 0f, 0f));
        Shader.SetGlobalFloat(FlatMulId, flatMul);
        Shader.SetGlobalFloat(TowardMulId, towardMul);
        Shader.SetGlobalFloat(AwayMulId, awayMul);

        Shader.SetGlobalColor(ShadowTintId, shadowTint);
        Shader.SetGlobalFloat(ShadowTintStrengthId, shadowTintStrength);
        Shader.SetGlobalFloat(CavityDarknessId, cavityDarkness);
    }

    void OnDrawGizmosSelected()
    {
        float radians = angleDegrees * Mathf.Deg2Rad;

        Vector3 dir = new Vector3(
            Mathf.Cos(radians),
            Mathf.Sin(radians), 0f
        ).normalized;

        Gizmos.DrawLine(transform.position, transform.position + (dir * 10f));
        Gizmos.color = Color.yellowNice;
    }
}