// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Our Town/chapel_display"
{
	Properties
	{
		_MainTex("Mask", 2D) = "white" {}
		_Paper("Background", 2D) = "white" {}
		_Chapel("Image to Fade to", 2D) = "white" {}
		_Value("Value to lerp in Chapel", Vector) = (0., 0., 0., 0.)
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
//#if UNITY_UV_STARTS_AT_TOP				
//				o.uv.y = 1 - v.uv.y;
//#endif
				return o;
			}
			
			sampler2D _Paper;
			sampler2D _MainTex;
			sampler2D _Chapel;
			float4 _Value;

			fixed4 frag (v2f i) : SV_Target
			{
				//fixed4 mask = tex2D(_MainTex, i.uv);
				float2 offset = float2(_Value.y, 0.);
				fixed4 col = tex2D(_Chapel, i.uv + offset);
				fixed4 paper = tex2D(_Paper, i.uv + offset);
				
				return lerp(paper, col, _Value.x);
			}
			ENDCG
		}
	}
}
