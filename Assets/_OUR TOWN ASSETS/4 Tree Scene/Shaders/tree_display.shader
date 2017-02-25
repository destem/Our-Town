Shader "Our Town/tree_display"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_SecondTex("Other buffer to combine", 2D) = "white" {}
		_BGTex("Background Paper", 2D) = "white" {}
		_FinalTex("Final Image", 2D) = "white" {}
		_TransTex("Transition Texture", 2D) = "white" {}
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

			//half2 unpack(float packed) {
			//	uint packedAsUint = asuint(packed); 
			//	return half2((packedAsUint & 0xffff), (packedAsUint >> 16)) / 65535.0h;
			//}
			
			sampler2D _MainTex;
			sampler2D _SecondTex;
			sampler2D _BGTex;
			sampler2D _FinalTex;
			sampler2D _TransTex;

			fixed4 frag (v2f i) : SV_Target
			{
				float4 source = tex2D(_MainTex, i.uv);
				float4 second = tex2D(_SecondTex, i.uv);
				float maskOneAmount = source.r;
				float maskTwoAmount = source.b;
				float maskThreeAmount = second.r;
				float maskFourAmount = second.b;

				float maskOneFade = source.g;
				float maskTwoFade = source.a;
				float maskThreeFade = second.g;
				float maskFourFade = second.a;

				fixed4 paperCol = tex2D(_BGTex, i.uv);
				fixed4 finalCol = tex2D(_FinalTex, i.uv);
				fixed4 col;
				float2 transUV;
				transUV.x = i.uv.x * 32;
				transUV.y = i.uv.y * 4;
				fixed4 transVal = tex2D(_TransTex, transUV.xy);
				//change amount to be its own step value - mutate the mask in place, then lerp colors
				maskOneAmount = 1 - step(transVal, 1 - maskOneFade);
				maskTwoAmount = 1 - step(transVal, 1 - maskTwoFade);
				maskThreeAmount = 1 - step(transVal, 1 - maskThreeFade);
				maskFourAmount = 1 - step(transVal, 1 - maskFourFade);

				float finalAmount = max(max(maskTwoAmount, maskOneAmount), max(maskThreeAmount, maskFourAmount));
				float4 lumcoeff = float4(0.299, 0.587, 0.114, 0.);
				float luminence = dot(finalCol, lumcoeff);
				// only lerp to finalCol if it's luminence is low enough (ink)
				// edge, val. If less than edge, returns 1
				//finalAmount *= 1-(step(.5, luminence));

				col = lerp(paperCol, finalCol, finalAmount);
				return col;
			}
			ENDCG
		}
	}
}
