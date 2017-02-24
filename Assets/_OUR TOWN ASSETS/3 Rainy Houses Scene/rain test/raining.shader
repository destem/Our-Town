Shader "Our Town/Raining"
{
	Properties
	{
		_MainTex("Main", 2D) = "white" {}
	    _Paper("Paper", 2D) = "white" {}
		_Rain("Rain", 2D) = "white" {}
	    _Noise1("Noise 1", 2D) = "white" {}
		_Noise2("Noise 2", 2D) = "white" {}

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
			sampler2D _Noise1;
			sampler2D _Noise2;
			sampler2D _Rain;
			sampler2D _Paper;

			float map(float s, float a1, float a2, float b1, float b2)
			{
				return b1 + (s - a1)*(b2 - b1) / (a2 - a1);
			}

			fixed4 frag (v2f i) : SV_Target
			{
				
				fixed4 paper = tex2D(_Paper, i.uv);
				//float noise1y = i.uv.y - _Time.y * .2;
				float noise1 = tex2D(_Noise1, i.uv).r;
				fixed4 rain = tex2D(_Rain, float2(i.uv.x, (i.uv.y - _Time.y * map(noise1, 0., 1., .1, .5))));
				//float noise2y = i.uv.y - _Time.y * .1;
				//float noise2 = tex2D(_Noise2, float2(i.uv.x, noise2y)).r;

				//return lerp(paper, rain, noise1*noise2);
				return rain;
			}
			ENDCG
		}
	}
}
