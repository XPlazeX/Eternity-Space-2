Shader "Custom/SoftUIMaskColored"
{
    Properties
    {
        [PerRendererData] _MainTex ("Graphic Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _MaskTex ("Soft Mask Texture", 2D) = "white" {}
        _MaskAlpha ("Mask Alpha", Range(0, 1)) = 1
        _MaskPower ("Mask Softness Power", Range(0.1, 8)) = 1
        [MaterialToggle] _InvertMask ("Invert Mask", Float) = 0

        _MultiplyColor ("Multiply Color", Color) = (1,1,1,1)
        _MultiplyStrength ("Multiply Strength", Range(0, 1)) = 1

        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
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

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex        : SV_POSITION;
                fixed4 color         : COLOR;
                float2 uv            : TEXCOORD0;
                float2 maskUV        : TEXCOORD1;
                float4 worldPosition : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            sampler2D _MaskTex;
            sampler2D _AlphaTex;

            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            fixed4 _MultiplyColor;
            float4 _MainTex_ST;
            float4 _MaskTex_ST;
            float4 _ClipRect;
            float _AlphaSplitEnabled;
            float _MaskAlpha;
            float _MaskPower;
            float _InvertMask;
            float _MultiplyStrength;

            fixed4 SampleGraphic(float2 uv)
            {
                fixed4 color = tex2D(_MainTex, uv) + _TextureSampleAdd;

                #if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
                if (_AlphaSplitEnabled)
                    color.a = tex2D(_AlphaTex, uv).r;
                #endif

                return color;
            }

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.worldPosition = IN.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.uv = TRANSFORM_TEX(IN.texcoord, _MainTex);
                OUT.maskUV = TRANSFORM_TEX(IN.texcoord, _MaskTex);
                OUT.color = IN.color * _Color;

                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 graphic = SampleGraphic(IN.uv) * IN.color;
                fixed4 mask = tex2D(_MaskTex, IN.maskUV);

                float maskAlpha = saturate(mask.a * _MaskAlpha);
                maskAlpha = lerp(maskAlpha, 1.0 - maskAlpha, step(0.5, _InvertMask));
                maskAlpha = pow(saturate(maskAlpha), max(_MaskPower, 0.0001));

                float3 multiplyColor = mask.rgb * _MultiplyColor.rgb;
                graphic.rgb *= lerp(float3(1.0, 1.0, 1.0), multiplyColor, _MultiplyStrength);
                graphic.a *= maskAlpha;

                #ifdef UNITY_UI_CLIP_RECT
                graphic.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip(graphic.a - 0.001);
                #endif

                return graphic;
            }
            ENDCG
        }
    }
}
