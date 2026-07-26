Shader "ES/Tilemap Direction Map Lit"
{
    Properties
    {
        // Both textures are supplied by TilemapRenderer/SpriteAtlas.
        // Add the direction sheet as a Sprite Secondary Texture named _DirTex.
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        [PerRendererData] _DirTex ("Direction Map", 2D) = "black" {}

        _Color ("Material Tint", Color) = (0,0,0,1)
        _VertexColorAddStrength ("Tilemap Color Add Strength", Range(0, 1)) = 0

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
            #pragma target 3.0
            #pragma multi_compile _ PIXELSNAP_ON

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _DirTex;

            fixed4 _Color;
            float _VertexColorAddStrength;

            float4 _ES_GlobalLightDir;
            float _ES_FlatMul;
            float _ES_TowardMul;
            float _ES_AwayMul;

            fixed4 _ES_ShadowTint;
            float _ES_ShadowTintStrength;
            float _ES_CavityDarkness;

            float _ES_Flash;
            fixed4 _ES_FlashColor;
            float _ES_FlashLift;
            float _ES_FlashAdd;

            struct appdata_t
            {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 objectPos : TEXCOORD1;
                fixed3 tileColor : COLOR0;
                fixed tileAlpha : COLOR1;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.objectPos = v.vertex.xy;
                o.tileColor = v.color.rgb;
                o.tileAlpha = v.color.a;

                #ifdef PIXELSNAP_ON
                o.vertex = UnityPixelSnap(o.vertex);
                #endif

                return o;
            }

            float2 DecodeDirection2D(float index)
            {
                if (index < 0.5)
                    return float2(0.0, 0.0);

                const float HALF_PI = 1.57079632679;
                const float QUARTER_PI = 0.78539816339;
                float angle = HALF_PI - (index - 1.0) * QUARTER_PI;
                return float2(cos(angle), sin(angle));
            }

            // Reconstruct the tile's +U and +V axes from the UV-to-geometry mapping.
            // RuleTile rotation and mirroring are baked into that mapping, so this also
            // transforms the decoded direction without requiring map variants.
            float2 DirectionFromTileToObject(float2 tileDirection, float2 uv, float2 objectPos)
            {
                float2 uvDx = ddx(uv);
                float2 uvDy = ddy(uv);
                float2 posDx = ddx(objectPos);
                float2 posDy = ddy(objectPos);

                float determinant = uvDx.x * uvDy.y - uvDy.x * uvDx.y;
                float safeDeterminant = (abs(determinant) < 1e-8)
                    ? (determinant < 0.0 ? -1e-8 : 1e-8)
                    : determinant;

                float2 axisU = (posDx * uvDy.y - posDy * uvDx.y) / safeDeterminant;
                float2 axisV = (-posDx * uvDy.x + posDy * uvDx.x) / safeDeterminant;

                axisU = normalize(axisU + float2(1e-8, 0.0));
                axisV = normalize(axisV + float2(0.0, 1e-8));
                return normalize(axisU * tileDirection.x + axisV * tileDirection.y + float2(1e-8, 1e-8));
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 baseCol = tex2D(_MainTex, i.uv);
                float4 dirSample = tex2D(_DirTex, i.uv);

                float finalAlpha = baseCol.a * i.tileAlpha * _Color.a;
                float index = floor(dirSample.r * 8.0 + 0.5);
                float cavity = saturate(dirSample.b);
                bool isFlat = index < 0.5;

                float2 normalDir = DecodeDirection2D(index);
                if (!isFlat)
                    normalDir = DirectionFromTileToObject(normalDir, i.uv, i.objectPos);

                float3 worldLight3 = float3(_ES_GlobalLightDir.x, _ES_GlobalLightDir.y, 0.0);
                float3 objectLight3 = mul((float3x3)unity_WorldToObject, worldLight3);
                float2 lightDir = normalize(objectLight3.xy + float2(0.0001, 0.0001));

                float lightMul;
                if (isFlat)
                {
                    lightMul = _ES_FlatMul;
                }
                else
                {
                    float d = dot(normalDir, lightDir);
                    lightMul = d >= 0.0
                        ? lerp(_ES_FlatMul, _ES_TowardMul, saturate(d))
                        : lerp(_ES_FlatMul, _ES_AwayMul, saturate(-d));
                }

                float cavityMul = 1.0 - cavity * saturate(_ES_CavityDarkness);
                lightMul = max(0.0, lightMul * cavityMul);

                float flash = saturate(_ES_Flash);
                float shadowLift = saturate(1.0 - lightMul) * flash * _ES_FlashLift;
                float finalMul = lightMul + shadowLift;

                fixed3 rgb = baseCol.rgb * finalMul;
                float shadowAmount = saturate(1.0 - finalMul) * saturate(_ES_ShadowTintStrength);
                rgb = lerp(rgb, rgb * _ES_ShadowTint.rgb, shadowAmount);
                rgb += _ES_FlashColor.rgb * baseCol.a * flash * _ES_FlashAdd * saturate(1.0 - lightMul);

                // Tilemap color is white by default, unlike the black additive color used
                // by the sprite shader. Keep addition opt-in to avoid whitening the map.
                rgb += i.tileColor * saturate(_VertexColorAddStrength);

                return fixed4(rgb, finalAlpha);
            }
            ENDCG
        }
    }

    Fallback "Sprites/Default"
}
