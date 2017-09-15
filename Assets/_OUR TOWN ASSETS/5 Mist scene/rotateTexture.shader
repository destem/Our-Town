// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Our Town/rotateTexture"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_offset ("Texture offset", float) = 1
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
				return o;
			}
			
			sampler2D _MainTex;
			float _offset;

			fixed4 frag (v2f i) : SV_Target
			{
				i.uv -= 0.5; // for centering vertically
				i.uv.y /= .22222; //for scaling to 16:9 aspect ratio

				fixed4 col = tex2D(_MainTex, float2(i.uv.x + _offset + 0.5, i.uv.y + 0.5));
				col *= step(i.uv.y, 0.5);
				col *= 1 - step(i.uv.y, -0.5);
				return col;
			}
			ENDCG
		}
	}
}
