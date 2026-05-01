using UnityEngine;
 
public class WaveWarp : ScreenPostEffectBase
{
    public Texture noiseTexure;
    [Range(0.0f, 1.0f)]
    public float noiseTimeScale = 1.0f;
    [Range(0.0f, 1.0f)]
    public float uvScale = 0.15f;
 
 
    void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
    {
        if (_Material != null)
        {
            _Material.SetFloat("_UVScale", uvScale);
            _Material.SetFloat("_NoiseTimeScale", noiseTimeScale);
            _Material.SetTexture("_NoiseTex", noiseTexure);
 
            Graphics.Blit(sourceTexture, destTexture, _Material);
        }
        else
        {
            Graphics.Blit(sourceTexture, destTexture);
        }
    }
 
}
 
