// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Our Town/wipe"
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
				o.vertex = UnityObjectToClipPos(v.vertex);
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
				i.uv -= 0.5; // for centering vertically
				i.uv.y /= .22222; //for scaling to 16:9 aspect ratio

				fixed4 from = tex2D(_From, i.uv + 0.5);
				fixed4 to = tex2D(_To, i.uv + 0.5);
				float2 transUV;
				transUV.x = i.uv.x * 32;
				transUV.y = i.uv.y * 4;
				fixed4 trans = tex2D(_Trans, transUV.xy + 0.5);
				float value = 1 - smoothstep(_Value - .05, _Value + .05, abs(i.uv.x - 0.5)*2);
				value = 1 - step(trans, 1 - value);
				//return fixed4(i.uv.x, i.uv.y, 0., 1.);
				fixed4 col = lerp(from, to, value);
				col *= step(i.uv.y, 0.5);
				col *= 1 - step(i.uv.y, -0.5);
				return col;
			}
			ENDCG
		}
	}
}
