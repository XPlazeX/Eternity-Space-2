Shader "Custom/Ark Water"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Sprite Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0

        _SurfaceColor ("Surface Color", Color) = (0.12, 0.55, 0.75, 0.58)
        _UnderwaterColor ("Underwater Color", Color) = (0.04, 0.34, 0.48, 0.38)
        _RelativeHeight ("Relative Height", Range(0, 1)) = 1
        _Opacity ("Opacity", Range(0, 1)) = 1

        _WaveAStrength ("Procedural Wave A Strength", Range(0, 2)) = 0.75
        _WaveAScale ("Procedural Wave A Scale", Float) = 3.5
        _WaveASpeed ("Procedural Wave A Speed", Vector) = (0.04, 0.018, 0, 0)
        _WaveADetail ("Procedural Wave A Detail", Range(0, 1)) = 0.55
        _WaveASharpness ("Procedural Wave A Sharpness", Range(0.2, 4)) = 1.2
        _WaveAWarpStrength ("Procedural Wave A Warp", Range(0, 3)) = 0.55
        _WaveAWarpScale ("Procedural Wave A Warp Scale", Float) = 7.5

        _WaveBStrength ("Procedural Wave B Strength", Range(0, 2)) = 0.45
        _WaveBScale ("Procedural Wave B Scale", Float) = 8.0
        _WaveBSpeed ("Procedural Wave B Speed", Vector) = (-0.025, 0.045, 0, 0)
        _WaveBDetail ("Procedural Wave B Detail", Range(0, 1)) = 0.75
        _WaveBSharpness ("Procedural Wave B Sharpness", Range(0.2, 4)) = 1.6
        _WaveBWarpStrength ("Procedural Wave B Warp", Range(0, 3)) = 0.85
        _WaveBWarpScale ("Procedural Wave B Warp Scale", Float) = 4.0

        _RefractionStrength ("Surface Refraction", Range(0, 0.08)) = 0.018
        _RefractionVisibility ("Refraction Visibility", Range(0, 1)) = 1
        _UnderwaterFlowStrength ("Underwater Flow", Range(0, 0.16)) = 0.055
        _FlowScale ("Underwater Flow Scale", Float) = 1.8
        _FlowSpeed ("Underwater Flow Speed", Vector) = (0.015, 0.028, 0, 0)

        _WaveLightColor ("Wave Light Color", Color) = (0.85, 1.0, 1.0, 1)
        _WaveLightIntensity ("Wave Light Intensity", Range(0, 2)) = 0.35
        _WaveLightPower ("Wave Light Power", Range(0.5, 12)) = 5.0
        _WaveDirectionalColor ("Wave Directional Color", Color) = (0.1, 0.72, 1.0, 1)
        _WaveTintDirection ("Wave Tint Direction", Vector) = (-0.35, 0.65, 0, 0)
        _WaveDirectionalIntensity ("Wave Directional Intensity", Range(0, 2)) = 0.28
        _WaveDirectionalPower ("Wave Directional Power", Range(0.5, 12)) = 2.6

        _CameraCircleCenter ("Camera Circle Center", Vector) = (0.5, 0.5, 0, 0)
        _DiveCircleRadius ("Dive Circle Radius", Range(0, 1)) = 0.24
        _DiveCircleSoftness ("Dive Circle Softness", Range(0.001, 0.5)) = 0.08
        _DiveCircleStrength ("Dive Circle Distortion", Range(0, 1)) = 1
        _DiveCircleOpacity ("Dive Circle Opacity", Range(0, 1)) = 0.18
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

        GrabPass
        {
            "_ArkWaterGrab"
        }

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
                float4 vertex    : SV_POSITION;
                fixed4 color     : COLOR;
                float2 uv        : TEXCOORD0;
                float2 worldPos  : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
                float4 grabPos   : TEXCOORD3;
            };

            sampler2D _MainTex;
            sampler2D _AlphaTex;
            float _AlphaSplitEnabled;
            sampler2D _ArkWaterGrab;

            fixed4 _Color;
            fixed4 _SurfaceColor;
            fixed4 _UnderwaterColor;
            float _RelativeHeight;
            float _Opacity;

            float _WaveAStrength;
            float _WaveAScale;
            float4 _WaveASpeed;
            float _WaveADetail;
            float _WaveASharpness;
            float _WaveAWarpStrength;
            float _WaveAWarpScale;

            float _WaveBStrength;
            float _WaveBScale;
            float4 _WaveBSpeed;
            float _WaveBDetail;
            float _WaveBSharpness;
            float _WaveBWarpStrength;
            float _WaveBWarpScale;

            float _RefractionStrength;
            float _RefractionVisibility;
            float _UnderwaterFlowStrength;
            float _FlowScale;
            float4 _FlowSpeed;

            fixed4 _WaveLightColor;
            float _WaveLightIntensity;
            float _WaveLightPower;
            fixed4 _WaveDirectionalColor;
            float4 _WaveTintDirection;
            float _WaveDirectionalIntensity;
            float _WaveDirectionalPower;

            float4 _CameraCircleCenter;
            float _DiveCircleRadius;
            float _DiveCircleSoftness;
            float _DiveCircleStrength;
            float _DiveCircleOpacity;
            float _ESArkTime;

            fixed4 SampleSpriteTexture(float2 uv)
            {
                fixed4 color = tex2D(_MainTex, uv);

            #if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
                if (_AlphaSplitEnabled)
                    color.a = tex2D(_AlphaTex, uv).r;
            #endif

                return color;
            }

            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            float valueNoise(float2 uv)
            {
                float2 i = floor(uv);
                float2 f = frac(uv);
                float2 u = f * f * (3.0 - 2.0 * f);

                float a = hash21(i);
                float b = hash21(i + float2(1.0, 0.0));
                float c = hash21(i + float2(0.0, 1.0));
                float d = hash21(i + float2(1.0, 1.0));

                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            float fbm(float2 uv, float detail)
            {
                float v = 0.0;
                float a = 0.5;
                float gain = lerp(0.28, 0.62, saturate(detail));

                v += valueNoise(uv) * a; uv *= 2.03; a *= gain;
                v += valueNoise(uv) * a; uv *= 2.01; a *= gain;
                v += valueNoise(uv) * a; uv *= 2.02; a *= gain;
                v += valueNoise(uv) * a;

                return v;
            }

            float waveHeight(float2 uv, float detail, float sharpness)
            {
                float n = saturate(fbm(uv, detail));
                float centered = n * 2.0 - 1.0;
                float shaped = sign(centered) * pow(abs(centered), sharpness);
                return shaped * 0.5 + 0.5;
            }

            float2 proceduralWaveNormal(float2 worldPos, float scale, float2 speed, float strength, float detail, float sharpness, float warpStrength, float warpScale, float time, float seed)
            {
                float safeScale = max(scale, 0.0001);
                float safeWarpScale = max(warpScale, 0.0001);

                float2 warpUV = worldPos / safeWarpScale + speed.yx * time * 0.35 + seed;
                float2 warp = float2(
                    fbm(warpUV + float2(7.1, 2.3), detail),
                    fbm(warpUV + float2(1.9, 8.7), detail)
                ) * 2.0 - 1.0;

                float2 uv = worldPos / safeScale + speed * time + warp * warpStrength;
                float e = 0.035;
                float c = waveHeight(uv, detail, sharpness);
                float dx = waveHeight(uv + float2(e, 0.0), detail, sharpness) - c;
                float dy = waveHeight(uv + float2(0.0, e), detail, sharpness) - c;

                return float2(dx, dy) / e * strength;
            }

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.uv = IN.texcoord;
                OUT.color = IN.color * _Color;
                OUT.worldPos = mul(unity_ObjectToWorld, IN.vertex).xy;
                OUT.screenPos = ComputeScreenPos(OUT.vertex);
                OUT.grabPos = ComputeGrabScreenPos(OUT.vertex);

            #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap(OUT.vertex);
            #endif

                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 sprite = SampleSpriteTexture(IN.uv) * IN.color;
                if (sprite.a <= 0.001)
                    discard;

                float height = saturate(_RelativeHeight);
                float underwater = 1.0 - height;
                float time = _ESArkTime;

                float2 normalXY = proceduralWaveNormal(
                    IN.worldPos,
                    _WaveAScale,
                    _WaveASpeed.xy,
                    _WaveAStrength,
                    _WaveADetail,
                    _WaveASharpness,
                    _WaveAWarpStrength,
                    _WaveAWarpScale,
                    time,
                    0.0
                );

                normalXY += proceduralWaveNormal(
                    IN.worldPos,
                    _WaveBScale,
                    _WaveBSpeed.xy,
                    _WaveBStrength,
                    _WaveBDetail,
                    _WaveBSharpness,
                    _WaveBWarpStrength,
                    _WaveBWarpScale,
                    time,
                    19.37
                );

                float2 flowUV = IN.worldPos / max(_FlowScale, 0.0001) + _FlowSpeed.xy * time;
                float2 flow = float2(
                    fbm(flowUV + float2(2.7, 8.1), 0.85),
                    fbm(flowUV + float2(6.4, 1.9), 0.85)
                ) * 2.0 - 1.0;

                float2 screenUV = IN.screenPos.xy / max(IN.screenPos.w, 0.0001);
                float2 circleDelta = screenUV - _CameraCircleCenter.xy;
                circleDelta.x *= _ScreenParams.x / max(_ScreenParams.y, 0.0001);

                float circleRadius = _DiveCircleRadius * underwater;
                float circleEdge = max(_DiveCircleSoftness, 0.001);
                float circleDistance = length(circleDelta);
                float circleOutside = smoothstep(circleRadius, circleRadius + circleEdge, circleDistance);
                float circleMask = (1.0 - circleOutside) * underwater;
                float circleDistortionScale = lerp(1.0, saturate(_DiveCircleStrength), circleMask);

                float2 distortion = normalXY * _RefractionStrength + flow * _UnderwaterFlowStrength * underwater;
                distortion *= circleDistortionScale;

                float4 grabPos = IN.grabPos;
                grabPos.xy += distortion * grabPos.w;
                fixed3 refracted = tex2Dproj(_ArkWaterGrab, UNITY_PROJ_COORD(grabPos)).rgb;

                fixed4 waterColor = lerp(_UnderwaterColor, _SurfaceColor, height);
                float baseAlpha = sprite.a * _Opacity;
                float waterAlpha = waterColor.a * baseAlpha;
                float circleOpacity = lerp(1.0, saturate(_DiveCircleOpacity), circleMask);
                float visualAlpha = waterAlpha * circleOpacity;
                float refractionAlpha = max(visualAlpha, baseAlpha * _RefractionVisibility);

                float3 normal3 = normalize(float3(normalXY, 0.35));
                float lightMask = saturate(dot(normal3, normalize(float3(-0.35, 0.45, 0.8))));
                lightMask = pow(lightMask, _WaveLightPower) * _WaveLightIntensity * height;

                float2 tintDir = _WaveTintDirection.xy;
                tintDir = tintDir / max(length(tintDir), 0.0001);
                float2 normalDir = normalXY / max(length(normalXY), 0.0001);
                float directionMask = saturate(dot(normalDir, tintDir));
                directionMask = pow(directionMask, _WaveDirectionalPower) * _WaveDirectionalIntensity;
                directionMask *= saturate(length(normalXY));

                fixed3 waveColor = _WaveLightColor.rgb * lightMask;
                waveColor += _WaveDirectionalColor.rgb * directionMask;

                fixed3 tintedWater = refracted * refractionAlpha + (waterColor.rgb + waveColor) * visualAlpha;
                fixed3 spriteTint = lerp(fixed3(1.0, 1.0, 1.0), sprite.rgb, sprite.a);
                fixed3 finalRgb = tintedWater * spriteTint;

                return fixed4(finalRgb, refractionAlpha);
            }
            ENDCG
        }
    }
}
