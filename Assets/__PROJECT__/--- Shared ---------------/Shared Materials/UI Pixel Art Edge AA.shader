Shader "UI/Pixel Art Edge AA"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _EdgeThreshold ("Alpha Threshold", Range(0.01, 0.99)) = 0.5
        _EdgeSoftness ("Edge Softness", Range(0.25, 2.0)) = 1.0
        _SampleSpread ("Sample Spread", Range(0.25, 1.5)) = 0.75

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
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
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
            #pragma target 3.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float _EdgeThreshold;
            float _EdgeSoftness;
            float _SampleSpread;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.worldPosition = IN.vertex;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                return OUT;
            }

            fixed SampleAlpha(float2 uv)
            {
                return (tex2D(_MainTex, uv) + _TextureSampleAdd).a;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // Keep the visible pixel colour untouched. Only alpha coverage is
                // supersampled, so rotated edges become smooth without applying a
                // full-screen filter or blurring the pixel-art interior.
                fixed4 center = tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd;

                float2 dx = ddx(IN.texcoord) * _SampleSpread;
                float2 dy = ddy(IN.texcoord) * _SampleSpread;

                float coverage = SampleAlpha(IN.texcoord) * 0.2;
                coverage += SampleAlpha(IN.texcoord + dx * 0.5 + dy * 0.5) * 0.2;
                coverage += SampleAlpha(IN.texcoord + dx * 0.5 - dy * 0.5) * 0.2;
                coverage += SampleAlpha(IN.texcoord - dx * 0.5 + dy * 0.5) * 0.2;
                coverage += SampleAlpha(IN.texcoord - dx * 0.5 - dy * 0.5) * 0.2;

                float halfWidth = max(fwidth(coverage) * _EdgeSoftness, 1.0 / 255.0);
                coverage = smoothstep(_EdgeThreshold - halfWidth,
                                      _EdgeThreshold + halfWidth,
                                      coverage);

                fixed4 color = fixed4(center.rgb, coverage) * IN.color;

                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip(color.a - 0.001);
                #endif

                return color;
            }
            ENDCG
        }
    }
}
