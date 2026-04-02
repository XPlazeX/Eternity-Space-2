Shader "Custom/SpriteWarpDriveAdvanced"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Mask", 2D) = "white" {}

        _WarpAmount ("Warp Amount", Range(0, 1)) = 0

        _InnerRadius ("Inner Radius", Range(0, 1)) = 0.18
        _PeakRadius  ("Peak Radius",  Range(0, 1)) = 0.52
        _OuterRadius ("Outer Radius", Range(0, 1)) = 1.00

        _EllipseX ("Ellipse X", Range(0.1, 4)) = 1.4
        _EllipseY ("Ellipse Y", Range(0.1, 4)) = 1.0

        _AxialStrengthPos ("Axial Strength Positive Hemisphere", Range(-0.2, 0.2)) = 0.025
        _AxialStrengthNeg ("Axial Strength Negative Hemisphere", Range(-0.2, 0.2)) = 0.025

        _RadialStrengthPos ("Radial Lens Strength Positive Hemisphere", Range(-0.2, 0.2)) = 0.01
        _RadialStrengthNeg ("Radial Lens Strength Negative Hemisphere", Range(-0.2, 0.2)) = 0.01

        _PoleSharpness ("Pole Sharpness", Range(0.5, 8)) = 2.5
        _EquatorSoftness ("Equator Softness", Range(0.1, 4)) = 1.0

        _MaskMultiplier ("Mask Multiplier", Range(0, 2)) = 1.0

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
            "_WarpBackground"
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
                float4 vertex      : SV_POSITION;
                float4 color       : COLOR;
                float2 uv          : TEXCOORD0;
                float4 grabPos     : TEXCOORD1;

                float2 screenAxisX : TEXCOORD2;
                float2 screenAxisY : TEXCOORD3;
            };

            sampler2D _MainTex;
            sampler2D _WarpBackground;

            float _WarpAmount;

            float _InnerRadius;
            float _PeakRadius;
            float _OuterRadius;

            float _EllipseX;
            float _EllipseY;

            float _AxialStrengthPos;
            float _AxialStrengthNeg;

            float _RadialStrengthPos;
            float _RadialStrengthNeg;

            float _PoleSharpness;
            float _EquatorSoftness;

            float _MaskMultiplier;

            float2 ClipToScreenUV(float4 clipPos)
            {
                float2 uv = clipPos.xy / clipPos.w;
                uv = uv * 0.5 + 0.5;

                #if UNITY_UV_STARTS_AT_TOP
                    uv.y = 1.0 - uv.y;
                #endif

                return uv;
            }

            v2f vert(appdata_t IN)
            {
                v2f OUT;

                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.uv = IN.texcoord;
                OUT.color = IN.color;
                OUT.grabPos = ComputeGrabScreenPos(OUT.vertex);

                float4 clipCenter = UnityObjectToClipPos(float4(0, 0, 0, 1));
                float4 clipX      = UnityObjectToClipPos(float4(1, 0, 0, 1));
                float4 clipY      = UnityObjectToClipPos(float4(0, 1, 0, 1));

                float2 centerUV = ClipToScreenUV(clipCenter);
                float2 xUV      = ClipToScreenUV(clipX);
                float2 yUV      = ClipToScreenUV(clipY);

                float2 axisX = xUV - centerUV;
                float2 axisY = yUV - centerUV;

                float lenX = max(length(axisX), 1e-6);
                float lenY = max(length(axisY), 1e-6);

                OUT.screenAxisX = axisX / lenX;
                OUT.screenAxisY = axisY / lenY;

                #ifdef PIXELSNAP_ON
                    OUT.vertex = UnityPixelSnap(OUT.vertex);
                #endif

                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 maskTex = tex2D(_MainTex, IN.uv);

                // uv в диапазон [-1..1]
                float2 p = IN.uv * 2.0 - 1.0;

                // Эллиптическое поле
                float2 ep = float2(p.x / max(_EllipseX, 1e-5), p.y / max(_EllipseY, 1e-5));
                float r = length(ep);

                float2 dir = (r > 1e-6) ? (ep / r) : float2(0.0, 0.0);

                // Кольцевая зона: ноль в центре, пик, потом спад к краю
                float rise = smoothstep(_InnerRadius, _PeakRadius, r);
                float fall = 1.0 - smoothstep(_PeakRadius, _OuterRadius, r);
                float radialBand = saturate(rise * fall);

                // К полюсам максимум, к экватору ноль
                float pole01 = saturate(abs(dir.x));
                float poleFactor = pow(pole01, _PoleSharpness);

                // Мягкость спада к экватору
                poleFactor = pow(poleFactor, _EquatorSoftness);

                float hemiSign = (dir.x >= 0.0) ? 1.0 : -1.0;
                bool positiveHemisphere = (dir.x >= 0.0);

                float axialStrength = positiveHemisphere ? _AxialStrengthPos : _AxialStrengthNeg;
                float radialStrength = positiveHemisphere ? _RadialStrengthPos : _RadialStrengthNeg;

                float spriteMask = maskTex.a * IN.color.a * _MaskMultiplier;
                float totalMask = spriteMask * saturate(_WarpAmount);

                // 1. Осевое двуполярное искажение
                float axialDistortion = radialBand * poleFactor * hemiSign * axialStrength * totalMask;

                // 2. Радиальная линза
                float radialFactor = radialBand * lerp(0.35, 1.0, poleFactor) * totalMask;
                float radialDistortion = radialFactor * radialStrength;

                float2 offsetScreenUV = 0.0;

                // Вдоль локальной X оси спрайта
                offsetScreenUV += IN.screenAxisX * axialDistortion;

                // От центра наружу/внутрь в экранном пространстве
                float2 radialScreenDir = normalize(IN.screenAxisX * dir.x + IN.screenAxisY * dir.y + 1e-8);
                offsetScreenUV += radialScreenDir * radialDistortion;

                float4 grabPos = IN.grabPos;
                grabPos.xy += offsetScreenUV * grabPos.w;

                fixed4 warped = tex2Dproj(_WarpBackground, UNITY_PROJ_COORD(grabPos));

                return fixed4(warped.rgb, saturate(totalMask));
            }
            ENDCG
        }
    }
}