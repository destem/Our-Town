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
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _OverTex;
			float4 _Value;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 black = fixed4(0., 0., 0., 1.);
				fixed4 main = lerp(tex2D(_MainTex, i.uv), black, _Value.x);
				fixed4 over = tex2D(_OverTex, i.uv);
			    fixed4 overlerped = lerp(over, black, _Value.y);
				fixed4 col = lerp(main, overlerped, over.a);
				return col;
			}
			ENDCG
		}
	}
}
