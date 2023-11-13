// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
 
// public class DistortEffect : PostEffectBase {
 
//          // Искаженный фактор времени
//     [Range(0.0f, 1.0f)]
//     public float DistortTimeFactor = 0.15f;
//          // Сила твист
//     [Range(0.0f, 0.2f)]
//     public float DistortStrength = 0.01f;
//          // График шума
//     public Texture NoiseTexture = null;
 
//     public void OnRenderImage(RenderTexture source, RenderTexture destination)
//     {
//         if (_Material)
//         {
//             _Material.SetTexture("_NoiseTex", NoiseTexture);
//             _Material.SetFloat("_DistortTimeFactor", DistortTimeFactor);
//             _Material.SetFloat("_DistortStrength", DistortStrength);
//             Graphics.Blit(source, destination, _Material);
//         }
//         else
//         {
//             Graphics.Blit(source, destination);
//         }
//     }
// }