Shader "Our Town/tree_growth"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {} //serves as input
		_MaskOneTex("Mask One", 2D) = "white" {}
		_MaskTwoTex("Mask Two", 2D) = "white" {}
		_Noise("Noise", 2D) = "white" {}
		_MaskOneCoords("Mask One Coordinates", Vector) = (-1., -1., -1., -1.)
		_MaskTwoCoords("Mask Two Coordinates", Vector) = (-1., -1., -1., -1.)
		_Speeds("Values for slow, medium, and fast growth rates", Vector) = (0., 0., 0., 0.)
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
				// screen space has UV origin in upper right!
				//o.uv.y = 1 - o.uv.y;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _MaskOneTex;
			sampler2D _MaskTwoTex;
			sampler2D _Noise;
			float4 _MaskTwoCoords;
			float4 _MaskOneCoords;
			float4 _Speeds;

			float4 when_eq(float4 x, float4 y) {
				return 1.0 - abs(sign(x - y));
			}

			float4 when_neq(float4 x, float4 y) {
				return abs(sign(x - y));
			}

			float4 when_gt(float4 x, float4 y) {
				return max(sign(x - y), 0.0);
			}

			float4 when_lt(float4 x, float4 y) {
				return max(sign(y - x), 0.0);
			}

			float4 when_ge(float4 x, float4 y) {
				return 1.0 - when_lt(x, y);
			}

			float4 when_le(float4 x, float4 y) {
				return 1.0 - when_gt(x, y);
			}

			float4 and(float4 a, float4 b) {
				return a * b;
			}

			float4 or(float4 a, float4 b) {
				return min(a + b, 1.0);
			}

			float4 xor(float4 a, float4 b) {
				return (a + b) % 2.0;
			}

			float4 not(float4 a) {
				return 1.0 - a;
			}

			float maprange(float s, float a1, float a2, float b1, float b2)
			{
				return b1 + (s - a1)*(b2 - b1) / (a2 - a1);
			}

			
			fixed4 frag (v2f i) : SV_Target
			{
				float4 sourceVal = tex2D(_MainTex, i.uv);
				half sourceOne = sourceVal.r;
				half sourceTwo = sourceVal.b;

				float4 oneVal = tex2D(_MaskOneTex, i.uv);
				float2 noiseUV;
				noiseUV.x = i.uv.x * 8;
				noiseUV.y = i.uv.y;
				float noise = tex2D(_Noise, noiseUV).r;
				_Speeds *= maprange(noise, 0., 1., .01, 2.);

				float oneSlow = oneVal.r;
				float oneMed = oneVal.g;
				float oneFast = oneVal.b;
				float4 twoVal = tex2D(_MaskTwoTex, i.uv);
				float twoSlow = twoVal.r;
				float twoMed = twoVal.g;
				float twoFast = twoVal.b;

				float threshhold = _Speeds.w;

				float step_w = 1. / 8192;
				float step_h = 1. / 1024;
				float kernel[9] = { .05, .2, .05, .2, -1, .2, .05, .2, .05 }; //laplacian approximation
				float2 offset[9] = { float2(-step_w, -step_h), float2(0.0, -step_h), float2(step_w, -step_h),
					float2(-step_w, 0.0), float2(0.0, 0.0), float2(step_w, 0.0),
					float2(-step_w, step_h), float2(0.0, step_h), float2(step_w, step_h) };

				half sumOne = 0;
				half sumTwo = 0;
				half sumThree = 0;
				half sumFour = 0;
				float4 tap;

				for (int j = 0; j<9; j++)
				{
					tap = tex2D(_MainTex, i.uv + offset[j]);
					sumOne += tap.r;
					sumTwo += tap.b;
				}

				// grow masks
				half oneAmount = clamp(sourceOne + (_Speeds.z * step(threshhold, sumOne) * step(0.5, oneFast)) + (_Speeds.y * step(threshhold, sumOne) * step(0.5, oneMed)) + (_Speeds.x * step(threshhold, sumOne) * step(0.5, oneSlow)), 0., 1.);
				half twoAmount = clamp(sourceTwo + (_Speeds.z * step(threshhold, sumTwo) * step(0.5, twoFast)) + (_Speeds.y * step(threshhold, sumTwo) * step(0.5, twoMed)) + (_Speeds.x * step(threshhold, sumTwo) * step(0.5, twoSlow)), 0., 1.);

				// increment "fade" of masks and clamp
				half oneFade = saturate(sourceVal.g + .003) * step(.001, oneAmount);
				half twoFade = saturate(sourceVal.a + .003) * step(.001, twoAmount);

				//adding new start position on mask one
				float aspectX = _ScreenParams.x / _ScreenParams.y;
				float oneRange = 0.001 * _MaskOneCoords.z;
				//need to correct for aspect ratio
				float oneDist = distance(float2(_MaskOneCoords.x * aspectX, _MaskOneCoords.y), float2(i.uv.x * aspectX, i.uv.y));
				// add to output if we have a coordinate from mask one, we're in range of brush size, and there's color in the mask
				half oneAppend = fixed4(1, 1, 1, cos(oneDist / oneRange) * .2) * step(0.01, _MaskOneCoords.x) *  step(oneDist, oneRange) * min(step(.01, oneFast) + step(.01, oneMed) + step(.01, oneSlow), 1.);
				oneAmount += oneAppend;
				oneFade += .001 * step(.1, oneAppend);
				saturate(oneAmount);

				//adding to mask two
				float twoRange = 0.001 * _MaskTwoCoords.z;
				float twoDist = distance(float2(_MaskTwoCoords.x * aspectX, _MaskTwoCoords.y), float2(i.uv.x * aspectX, i.uv.y));
				half twoAppend = fixed4(1, 1, 1, cos(twoDist / twoRange) * .2) * step(0.01, _MaskTwoCoords.x) * step(twoDist, twoRange) * min(step(.01, twoFast) + step(.01, twoMed) + step(.01, twoSlow), 1.);
				twoAmount += twoAppend;
				twoFade += .001 * step(.1, twoAppend);
				saturate(twoAmount);	

				
				float4 col = float4(oneAmount, oneFade, twoAmount, twoFade );
				return col;
			}
			ENDCG
		}
	}
}
