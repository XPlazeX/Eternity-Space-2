Shader "Custom/SpriteProceduralWreck"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _BreachCenter ("Breach Center UV", Vector) = (0.5, 0.5, 0, 0)
        _BreachRadius ("Breach Radius", Range(0, 1)) = 0.2
        _EdgeWidth ("Edge Width", Range(0.001, 0.25)) = 0.04

        _NoiseScale ("Noise Scale", Float) = 12.0
        _NoiseStrength ("Noise Strength", Range(0, 0.5)) = 0.08
        _NoiseSeed ("Noise Seed", Float) = 0.0

        _BurnAmount ("Burn Amount", Range(0, 1)) = 0.75
        _HeatAmount ("Heat Amount", Range(0, 3)) = 1.2
        _AshDarkness ("Ash Darkness", Range(0, 1)) = 0.15
        _GlobalAlpha ("Global Alpha", Range(0, 1)) = 1.0

        // x,y = min uv в атласе; z,w = size uv в атласе
        _SpriteUVRect ("Sprite UV Rect", Vector) = (0,0,1,1)

        _AngularNoiseStrength ("Angular Noise Strength", Range(0, 0.5)) = 0.16
        _AngularNoiseScale ("Angular Noise Scale", Float) = 6.0
        _WarpStrength ("Warp Strength", Range(0, 0.25)) = 0.04
        _WarpScale ("Warp Scale", Float) = 10.0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 uv       : TEXCOORD0;
            };

            sampler2D _MainTex;
            fixed4 _Color;

            float4 _BreachCenter;
            float _BreachRadius;
            float _EdgeWidth;

            float _NoiseScale;
            float _NoiseStrength;
            float _NoiseSeed;

            float _BurnAmount;
            float _HeatAmount;
            float _AshDarkness;
            float _GlobalAlpha;

            float4 _SpriteUVRect;

            float _AngularNoiseStrength;
            float _AngularNoiseScale;
            float _WarpStrength;
            float _WarpScale;

            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32 + _NoiseSeed);
                return frac(p.x * p.y);
            }

            float valueNoise(float2 uv)
            {
                float2 i = floor(uv);
                float2 f = frac(uv);

                float a = hash21(i);
                float b = hash21(i + float2(1, 0));
                float c = hash21(i + float2(0, 1));
                float d = hash21(i + float2(1, 1));

                float2 u = f * f * (3.0 - 2.0 * f);

                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            float fbm(float2 uv)
            {
                float v = 0.0;
                float a = 0.5;

                v += valueNoise(uv) * a; uv *= 2.03; a *= 0.5;
                v += valueNoise(uv) * a; uv *= 2.01; a *= 0.5;
                v += valueNoise(uv) * a; uv *= 2.02; a *= 0.5;
                v += valueNoise(uv) * a;

                return v;
            }

            float2 hash22(float2 p)
            {
                float n1 = hash21(p);
                float n2 = hash21(p + 17.13);
                return float2(n1, n2);
            }

            float2 domainWarp(float2 uv, float scale, float strength)
            {
                float2 q = float2(
                    fbm(uv * scale + float2(3.1, 7.2)),
                    fbm(uv * scale + float2(8.3, 2.8))
                );

                q = q * 2.0 - 1.0;
                return uv + q * strength;
            }

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.uv = IN.texcoord;
                OUT.color = IN.color * _Color;

                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap(OUT.vertex);
                #endif

                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 baseCol = tex2D(_MainTex, IN.uv) * IN.color;
                if (baseCol.a <= 0.001)
                    discard;

                // Переводим UV всей текстуры в локальные UV текущего спрайта [0..1]
                float2 localUV = (IN.uv - _SpriteUVRect.xy) / max(_SpriteUVRect.zw, float2(1e-5, 1e-5));

                // На всякий случай
                if (localUV.x < 0.0 || localUV.x > 1.0 || localUV.y < 0.0 || localUV.y > 1.0)
                    discard;

                float2 warpedUV = domainWarp(localUV, _WarpScale, _WarpStrength);

                float2 toCenter = warpedUV - _BreachCenter.xy;
                float dist = length(toCenter);

                // Угол вокруг центра
                float angle = atan2(toCenter.y, toCenter.x);   // [-pi, pi]
                float angle01 = angle / 6.2831853 + 0.5;       // [0, 1]

                // Шум по углу: именно он ломает круговую идеальность
                float angularNoise = fbm(float2(angle01 * _AngularNoiseScale, _NoiseSeed * 0.017));
                angularNoise = angularNoise * 2.0 - 1.0;

                // Обычный локальный шум тоже оставляем, но как вторичный
                float localNoise = fbm(warpedUV * _NoiseScale + _NoiseSeed * 0.013);
                localNoise = localNoise * 2.0 - 1.0;

                // Комбинируем оба
                float noisyRadius = _BreachRadius
                                + angularNoise * _AngularNoiseStrength
                                + localNoise * _NoiseStrength;

                float sdf = dist - noisyRadius;

                if (sdf < 0.0)
                    discard;

                float edgeMask = 1.0 - saturate(sdf / max(_EdgeWidth, 0.0001));
                edgeMask = smoothstep(0.0, 1.0, edgeMask);

                float luminance = dot(baseCol.rgb, float3(0.299, 0.587, 0.114));
                float3 ashColor = lerp(baseCol.rgb, luminance.xxx * _AshDarkness, _BurnAmount);

                float3 heatColor = float3(1.0, 0.45, 0.08) * _HeatAmount;

                float3 finalRgb = ashColor;
                finalRgb = lerp(finalRgb, finalRgb * 0.2 + heatColor, edgeMask);

                fixed4 finalCol;
                finalCol.rgb = finalRgb;
                finalCol.a = baseCol.a * _GlobalAlpha;
                return finalCol;
            }
            ENDCG
        }
    }
}