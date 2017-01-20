Shader "Our Town/tree_display"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
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
			sampler2D _BGTex;
			sampler2D _FinalTex;
			sampler2D _TransTex;

			fixed4 frag (v2f i) : SV_Target
			{
				float4 source = tex2D(_MainTex, i.uv);
				float rightAmount = source.r;
				rightAmount = clamp(rightAmount, 0., 1.);

				float leftAmount = source.b;
				leftAmount = clamp(leftAmount, 0., 1.);
				float rightFade = source.g;
				float leftFade = source.a;
				fixed4 paperCol = tex2D(_BGTex, i.uv);
				fixed4 finalCol = tex2D(_FinalTex, i.uv);
				fixed4 col;
				float2 transUV;
				transUV.x = i.uv.x * 32;
				transUV.y = i.uv.y * 4;
				fixed4 transVal = tex2D(_TransTex, transUV.xy);
				//change amount to be its own step value - mutate the mask in place, then lerp colors
				rightAmount = 1 - step(transVal, 1 - rightFade);
				leftAmount = 1 - step(transVal, 1 - leftFade);
				
				col = lerp(paperCol, finalCol, max(leftAmount, rightAmount));
				//return tex2D(_TransTex, transUV.xy);
				//return finalCol;
				return col;
			}
			ENDCG
		}
	}
}
