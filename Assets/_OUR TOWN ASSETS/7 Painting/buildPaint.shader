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
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			UNITY_DECLARE_TEX2DARRAY(_Painting);
			float4 level;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 backCol = UNITY_SAMPLE_TEX2DARRAY(_Painting, float3(i.uv.x, i.uv.y, level.x));
				fixed4 forwardCol = UNITY_SAMPLE_TEX2DARRAY(_Painting, float3(i.uv.x, i.uv.y, level.x + 1));
				
				return lerp(backCol, forwardCol, level.y);
			}
			ENDCG
		}
	}
}
