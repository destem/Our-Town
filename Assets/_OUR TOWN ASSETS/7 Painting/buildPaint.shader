// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Our Town/buildPaint"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Painting("The Painting Array", 2DArray) = "" {}
		level("Painting level", Vector) = (0,0,0,0)
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
			#pragma target 5.0
			
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
				return o;
			}
			
			sampler2D _MainTex;
			UNITY_DECLARE_TEX2DARRAY(_Painting);
			float4 level;

			fixed4 frag (v2f i) : SV_Target
			{
				i.uv -= 0.5; // for centering vertically
				i.uv.y /= .22222; //for scaling to 16:9 aspect ratio

				fixed4 backCol = UNITY_SAMPLE_TEX2DARRAY(_Painting, float3(i.uv.x + 0.5, i.uv.y + 0.5, level.x));
				fixed4 forwardCol = UNITY_SAMPLE_TEX2DARRAY(_Painting, float3(i.uv.x + 0.5, i.uv.y + 0.5, level.x + 1));
				
				fixed4 col = lerp(backCol, forwardCol, level.y);
				col *= step(i.uv.y, 0.5);
				col *= 1 - step(i.uv.y, -0.5);
				return col;
			}
			ENDCG
		}
	}
}
