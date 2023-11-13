Shader "Custom/OnWaveHeat"
{
	Properties
	{
		[PerRendererData]
		_MainTex ("Main Texture", 2D) = "white" {}
		_Color ("Color" , Color) = (1,1,1,1)

		_DistortStrength("DistortStrength", Range(0,1)) = 0.2
		_DistortTimeFactor("DistortTimeFactor", Range(0,1)) = 1
		_NoiseTex("NoiseTexture", 2D) = "white" {}

		[NoScaleOffset]
		_DisplacementTex ("Displacement Texture", 2D) = "white" {}
		[PerRendererData]
		_DisplacementPower ("Displacement Power", Float) = 0
	}

	SubShader
	{
		Tags 
		{ 
			"RenderType" = "Transparent" 
			"Queue" = "Transparent"
		}

		Cull Off
        //ZWrite Off 
		Blend Off 
		//SrcAlpha OneMinusSrcAlpha
		GrabPass {}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
		
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
				float4 grabPos : TEXCOORD1;
			};

			fixed4 _Color;
			sampler2D _MainTex;
			sampler2D _DisplacementTex;
			sampler2D _GrabTexture;

			sampler2D _NoiseTex;
			float4 _NoiseTex_ST;
			float _DistortStrength;
			float _DistortTimeFactor;
			
			float _DisplacementPower;

			// v2f vert (appdata v)
			// {
			// 	v2f o;
			// 	o.uv = v.uv;
			// 	o.color = v.color;
			// 	o.vertex = UnityObjectToClipPos(v.vertex);	

			// 	o.grabPos = ComputeScreenPos (o.vertex);
			// 	//o.grabPos /= o.grabPos.w;
				
			// 	return o;
			// }
			v2f vert(appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.grabPos = ComputeGrabScreenPos(o.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _NoiseTex);
				return o;
			}
						
			fixed4 frag (v2f i) : SV_Target
			{									

				float4 offset = tex2D(_NoiseTex, i.uv - _Time.xy * _DistortTimeFactor);
				i.grabPos.xy -= offset.xy * _DistortStrength;

				fixed4 color = tex2Dproj(_GrabTexture, i.grabPos);
				
				return color;			
			}
			ENDCG
		}
	}
}
