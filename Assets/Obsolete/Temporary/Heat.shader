// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Heat" 
{
	Properties
	{
		_DistortStrength("DistortStrength", Range(0,1)) = 0.2
		_DistortTimeFactor("DistortTimeFactor", Range(0,1)) = 1
		_NoiseTex("NoiseTexture", 2D) = "white" {}
	}
	SubShader
	{
		ZWrite Off
		Cull Off
		//GrabPass
		GrabPass
		{
			// Здесь дается имя текстуры захвата экрана, и через эту текстуру можно получить текстуру захвата, и независимо от того, используют ли шейдер несколько объектов в каждом кадре, только один будет выполнять операцию захвата экрана
			 // Если он пуст, снимок экрана будет по умолчанию записан в _GrabTexture, но говорят, что каждый пользователь, использующий этот шейдер, сделает снимок экрана!
			"_GrabTempTex"
		}
 
		Pass
		{
			Tags
			{ 
				"RenderType" = "Transparent"
				"Queue" = "Transparent + 100"
			}
 
			CGPROGRAM
			sampler2D _GrabTempTex;
			float4 _GrabTempTex_ST;
			sampler2D _NoiseTex;
			float4 _NoiseTex_ST;
			float _DistortStrength;
			float _DistortTimeFactor;
			#include "UnityCG.cginc"
			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 grabPos : TEXCOORD1;
			};
 
			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.grabPos = ComputeGrabScreenPos(o.pos);
				o.uv = TRANSFORM_TEX(v.texcoord, _NoiseTex);
				return o;
			}
 
			fixed4 frag(v2f i) : SV_Target
			{
				 // Сначала сэмплируем карту шума, сэмплированное значение uv непрерывно преобразуется со временем, и выводим случайное значение на карту шума, умноженное на коэффициент искажения
				float4 offset = tex2D(_NoiseTex, i.uv - _Time.xy * _DistortTimeFactor);
				 // Использовать выходные данные сэмплированной карты шума в качестве значения смещения следующей сэмплированной карты захвата, здесь умноженной на коэффициент интенсивности искажения
				i.grabPos.xy -= offset.xy * _DistortStrength;
				 // После смещения ультрафиолета, образец текстуры, чтобы получить эффект искажения
				fixed4 color = tex2Dproj(_GrabTempTex, i.grabPos);
				return color;
			}
 
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}
}