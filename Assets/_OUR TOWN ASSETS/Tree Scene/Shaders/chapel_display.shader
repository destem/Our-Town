﻿Shader "Our Town/chapel_display"
{
	Properties
	{
		_MainTex("Mask", 2D) = "white" {}
		_Paper("Background", 2D) = "white" {}
		_Trees("Image to Mask", 2D) = "white" {}
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
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _Paper;
			sampler2D _MainTex;
			sampler2D _Trees;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 mask = tex2D(_MainTex, i.uv);
				fixed4 col = tex2D(_Trees, i.uv);
				fixed4 paper = tex2D(_Paper, i.uv);
				if (mask.r < 1.)
					col = paper;
				return col;
			}
			ENDCG
		}
	}
}
