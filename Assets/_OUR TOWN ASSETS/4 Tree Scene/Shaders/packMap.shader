﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Our Town/packMap"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
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
			
			float pack(half low, half high) {
				uint lowScaled = low * 0xFFFF;
				uint highScaled = high * 0xFFFF;
				uint packed = (lowScaled & 0xffff) | (highScaled << 16);
				return asfloat(packed);
			}

			
			sampler2D _MainTex;

			fixed4 frag (v2f i) : SV_Target
			{
				float4 col = tex2D(_MainTex, i.uv);
				// the packing algorithm fails when the "high" value is < 0.002h, so we set that as our minimum
				return float4(pack(col.r, .002h), pack(0.0, .002h), pack(0.0, .002h), pack(0.0, .002h));
			}
			ENDCG
		}
	}
}
