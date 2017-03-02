﻿Shader "Our Town/wipe"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_From ("From Texture", 2D) = "white" {}
		_To("To Texture", 2D) = "white" {}
		_Trans("Transition Texture", 2D) = "white" {}
		_Value("Lerp Value", float) = 0
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
			
			sampler2D _MainTex;
			sampler2D _From;
			sampler2D _To;
			sampler2D _Trans;
			float _Value;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 from = tex2D(_From, i.uv);
				fixed4 to = tex2D(_To, i.uv);
				float2 transUV;
				transUV.x = i.uv.x * 32;
				transUV.y = i.uv.y * 4;
				fixed4 trans = tex2D(_Trans, transUV.xy);
				float value = 1 - smoothstep(_Value - .05, _Value + .05, abs(i.uv.x - 0.5)*2);
				value = 1 - step(trans, 1 - value);
				//return fixed4(i.uv.x, i.uv.y, 0., 1.);
				return lerp(from, to, value);
			}
			ENDCG
		}
	}
}
