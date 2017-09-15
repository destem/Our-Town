// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Our Town/star_display"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_OverTex ("Texture", 2D) = "white" {}
		_Value ("Background fade lerp value", Vector) = (0,0,0,0)
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
			sampler2D _OverTex;
			float4 _Value;

			fixed4 frag (v2f i) : SV_Target
			{
				i.uv -= 0.5; // for centering vertically
				i.uv.y /= .22222; //for scaling to 16:9 aspect ratio
				fixed4 black = fixed4(0., 0., 0., 1.);
				fixed4 main = lerp(tex2D(_MainTex, i.uv + 0.5), black, _Value.x);
				fixed4 over = tex2D(_OverTex, i.uv + 0.5);
			    fixed4 overlerped = lerp(over, black, _Value.y);
				fixed4 col = lerp(main, overlerped, over.a);
				col *= step(i.uv.y, 0.5);
				col *= 1 - step(i.uv.y, -0.5);
				return col;
				return col;
			}
			ENDCG
		}
	}
}
