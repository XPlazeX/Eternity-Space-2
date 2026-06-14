Shader "ES/Sprite Direction Map Lit"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _DirTex ("Direction Map", 2D) = "black" {}

        // Material tint left for manual use if needed.
        _Color ("Material Tint", Color) = (0,0,0,1)

        _ES_Flash ("Flash", Range(0, 1)) = 0
        _ES_FlashColor ("Flash Color", Color) = (1, 0.75, 0.35, 1)
        _ES_FlashLift ("Flash Shadow Lift", Range(0, 2)) = 1
        _ES_FlashAdd ("Flash Add", Range(0, 2)) = 0.25
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
            "DisableBatching"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ PIXELSNAP_ON

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _DirTex;

            float4 _MainTex_ST;
            fixed4 _Color;

            // Global level light.
            float4 _ES_GlobalLightDir; // xy = normalized 2D direction
            float _ES_FlatMul;
            float _ES_TowardMul;
            float _ES_AwayMul;

            // Global shadow tint / cavity settings.
            fixed4 _ES_ShadowTint;
            float _ES_ShadowTintStrength;
            float _ES_CavityDarkness;

            // Per-renderer flash.
            float _ES_Flash;
            fixed4 _ES_FlashColor;
            float _ES_FlashLift;
            float _ES_FlashAdd;

            struct appdata_t
            {
                float4 vertex : POSITION;
                fixed4 color : COLOR;      // SpriteRenderer.color
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;

                fixed3 spriteAdd : COLOR0; // additive SpriteRenderer rgb
                fixed  spriteAlpha : COLOR1; // SpriteRenderer alpha
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

                // IMPORTANT:
                // SpriteRenderer RGB is treated as additive color, not multiplicative tint.
                o.spriteAdd = v.color.rgb;
                o.spriteAlpha = v.color.a;

                #ifdef PIXELSNAP_ON
                o.vertex = UnityPixelSnap(o.vertex);
                #endif

                return o;
            }

            float2 DecodeDirection2D(float index)
            {
                // 0 = Flat
                // 1 = Up
                // 2 = UpRight
                // 3 = Right
                // 4 = DownRight
                // 5 = Down
                // 6 = DownLeft
                // 7 = Left
                // 8 = UpLeft

                if (index < 0.5)
                    return float2(0.0, 0.0);

                const float HALF_PI = 1.57079632679;
                const float QUARTER_PI = 0.78539816339;

                float angle = HALF_PI - (index - 1.0) * QUARTER_PI;
                return float2(cos(angle), sin(angle));
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 baseCol = tex2D(_MainTex, i.uv);
                float4 dirSample = tex2D(_DirTex, i.uv);

                // Final alpha:
                // texture alpha * SpriteRenderer alpha * material alpha
                float finalAlpha = baseCol.a * i.spriteAlpha * _Color.a;

                // R = direction index 0..8
                float index = floor(dirSample.r * 8.0 + 0.5);

                // B = cavity / crevice shadow strength 0..1
                float cavity = saturate(dirSample.b);

                bool isFlat = index < 0.5;

                float2 normalDir = DecodeDirection2D(index);
                float3 worldLight3 = float3(_ES_GlobalLightDir.x, _ES_GlobalLightDir.y, 0.0);
                float3 localLight3 = mul((float3x3)unity_WorldToObject, worldLight3);
                float2 lightDir = normalize(localLight3.xy + float2(0.0001, 0.0001));

                float lightMul;

                if (isFlat)
                {
                    lightMul = _ES_FlatMul;
                }
                else
                {
                    float d = dot(normalDir, lightDir);

                    if (d >= 0.0)
                    {
                        lightMul = lerp(_ES_FlatMul, _ES_TowardMul, saturate(d));
                    }
                    else
                    {
                        lightMul = lerp(_ES_FlatMul, _ES_AwayMul, saturate(-d));
                    }
                }

                // Permanent cavity darkening from blue channel.
                float cavityMul = 1.0 - cavity * saturate(_ES_CavityDarkness);
                lightMul *= cavityMul;
                lightMul = max(0.0, lightMul);

                // Cheap explosion/flash.
                float flash = saturate(_ES_Flash);
                float shadowLift = saturate(1.0 - lightMul) * flash * _ES_FlashLift;

                float finalMul = lightMul + shadowLift;

                // Base lit sprite.
                fixed3 rgb = baseCol.rgb * finalMul;

                // Optional material tint stays multiplicative.
                // rgb *= _Color.rgb;

                // Shadow tint.
                float shadowAmount = saturate(1.0 - finalMul) * saturate(_ES_ShadowTintStrength);
                rgb = lerp(rgb, rgb * _ES_ShadowTint.rgb, shadowAmount);

                // Small colored additive kick from explosion.
                rgb += _ES_FlashColor.rgb * baseCol.a * flash * _ES_FlashAdd * saturate(1.0 - lightMul);

                // NEW:
                // SpriteRenderer color adds on top instead of multiplying.
                // Color.black => no change
                // Color.red   => red added
                rgb += i.spriteAdd;

                return fixed4(rgb, finalAlpha);
            }

            ENDCG
        }
    }

    Fallback "Sprites/Default"
}
