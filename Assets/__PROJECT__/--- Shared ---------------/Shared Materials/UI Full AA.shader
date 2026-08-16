Shader "UI/Full AA"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _AAStrength ("AA Strength", Range(0.0, 1.0)) = 1.0
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
            float _AAStrength;
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

            fixed4 SampleSprite(float2 uv)
            {
                return tex2D(_MainTex, uv) + _TextureSampleAdd;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 dx = ddx(IN.texcoord) * _SampleSpread;
                float2 dy = ddy(IN.texcoord) * _SampleSpread;

                fixed4 center = SampleSprite(IN.texcoord);
                fixed4 s0 = SampleSprite(IN.texcoord + dx * 0.5 + dy * 0.5);
                fixed4 s1 = SampleSprite(IN.texcoord + dx * 0.5 - dy * 0.5);
                fixed4 s2 = SampleSprite(IN.texcoord - dx * 0.5 + dy * 0.5);
                fixed4 s3 = SampleSprite(IN.texcoord - dx * 0.5 - dy * 0.5);

                // Average in premultiplied-alpha space to prevent dark or coloured
                // fringes around transparent sprite pixels, then return to the
                // straight-alpha format expected by the standard UI blend mode.
                float alpha = (center.a + s0.a + s1.a + s2.a + s3.a) * 0.2;
                float3 premultiplied =
                    (center.rgb * center.a +
                     s0.rgb * s0.a + s1.rgb * s1.a +
                     s2.rgb * s2.a + s3.rgb * s3.a) * 0.2;

                fixed4 filtered;
                filtered.a = alpha;
                filtered.rgb = premultiplied / max(alpha, 1.0 / 255.0);

                fixed4 color = lerp(center, filtered, _AAStrength) * IN.color;

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
