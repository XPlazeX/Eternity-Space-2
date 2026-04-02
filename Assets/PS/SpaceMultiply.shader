Shader "Custom/SpriteBackgroundOverlay"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Mask", 2D) = "white" {}

        _Intensity ("Intensity", Range(0, 8)) = 1
        _BrightnessPower ("Brightness Response", Range(0.1, 4)) = 1
        _MaskAlpha ("Mask Alpha", Range(0, 1)) = 1

        // 0 = Add, 1 = Screen, 2 = SoftAdd
        [KeywordEnum(Add, Screen, SoftAdd)] _BlendMode ("Blend Mode", Float) = 0

        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        GrabPass
        {
            "_BackgroundTex"
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ PIXELSNAP_ON
            #pragma shader_feature_local _BLENDMODE_ADD _BLENDMODE_SCREEN _BLENDMODE_SOFTADD

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;      // <- SpriteRenderer.color приходит сюда
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex    : SV_POSITION;
                float4 color     : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 grabPos   : TEXCOORD1;
            };

            sampler2D _MainTex;
            sampler2D _BackgroundTex;

            float _Intensity;
            float _BrightnessPower;
            float _MaskAlpha;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color;
                OUT.grabPos = ComputeGrabScreenPos(OUT.vertex);

                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap(OUT.vertex);
                #endif

                return OUT;
            }

            float3 ApplyOverlay(float3 bg, float3 overlay)
            {
                #if defined(_BLENDMODE_SCREEN)
                    // Screen: ярко, мягко, меньше "выжигает"
                    return 1.0 - (1.0 - bg) * (1.0 - overlay);

                #elif defined(_BLENDMODE_SOFTADD)
                    // SoftAdd: промежуточный вариант, мягче обычного add
                    return bg + overlay * (1.0 - bg);

                #else
                    // Add
                    return bg + overlay;
                #endif
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 maskTex = tex2D(_MainTex, IN.texcoord);
                fixed4 bgTex = tex2Dproj(_BackgroundTex, UNITY_PROJ_COORD(IN.grabPos));

                float3 bg = bgTex.rgb;

                // Яркость фона под оверлеем
                float luminance = dot(bg, float3(0.299, 0.587, 0.114));
                luminance = pow(saturate(luminance), _BrightnessPower);

                // Цвет только из SpriteRenderer.color
                float3 spriteColor = IN.color.rgb;
                float spriteAlpha = IN.color.a;

                // Маска действия спрайта
                float mask = maskTex.a * spriteAlpha * _MaskAlpha;

                // Сила именно от яркости фона
                float3 overlay = spriteColor * (luminance * _Intensity);

                // Вычисляем конечный цвет "как если бы пиксель был полностью покрыт"
                float3 composed = ApplyOverlay(bg, overlay);

                // Альфой регулируем, насколько мы заменяем исходный фон этим composed
                return fixed4(composed, mask);
            }
            ENDCG
        }
    }
}