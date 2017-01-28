Shader "Our Town/tree_growth"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {} //serves as input
		_RightHandMask("Right Hand Mask", 2D) = "white" {}
		_LeftHandMask("Left Hand Mask", 2D) = "white" {}
		_LeftHand("Left Hand Coordinates", Vector) = (-1., -1., -1., -1.)
		_RightHand("Right Hand Coordinates", Vector) = (-1., -1., -1., -1.)
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
			sampler2D _RightHandMask;
			sampler2D _LeftHandMask;
			float4 _LeftHand;
			float4 _RightHand;
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
			
			fixed4 frag (v2f i) : SV_Target
			{
				float4 sourceVal = tex2D(_MainTex, i.uv);
				float sourceRight = sourceVal.r;
				float sourceLeft = sourceVal.b;
				float4 rightVal = tex2D(_RightHandMask, i.uv);
				float rightSlowVal = rightVal.r;
				float rightMedVal = rightVal.g;
				float rightFastVal = rightVal.b;
				float4 leftVal = tex2D(_LeftHandMask, i.uv);
				float leftSlowVal = leftVal.r;
				float leftMedVal = leftVal.g;
				float leftFastVal = leftVal.b;
				float threshhold = _Speeds.w;

				float step_w = 1. / 8192;
				float step_h = 1. / 1024;
				float kernel[9] = { .05, .2, .05, .2, -1, .2, .05, .2, .05 }; //laplacian approximation
				float2 offset[9] = { float2(-step_w, -step_h), float2(0.0, -step_h), float2(step_w, -step_h),
					float2(-step_w, 0.0), float2(0.0, 0.0), float2(step_w, 0.0),
					float2(-step_w, step_h), float2(0.0, step_h), float2(step_w, step_h) };

				float sumRight = 0;
				float sumLeft = 0;
				fixed4 tap;

				for (int j = 0; j<9; j++)
				{
					sumRight += tex2D(_MainTex, i.uv + offset[j]).r;// *kernel[j];
					sumLeft += tex2D(_MainTex, i.uv + offset[j]).b; 
				}

				//float outVal = sourceRight;
				 //sumRight is now weighted sum of neighbor values. If this val is above a threshold, AND rightFastVal is high enough, then we add white to our source for the output
				//if ((sumRight >= 0.04) && (rightFastVal > 0.01)) {
				//	outVal = sourceRight + 0.001;
				//}
				//clamp(outVal, 0., 1.);

				//float outVal = clamp(sourceRight + (0.004 * step(0.01, sumRight) * step(0.5, rightFastVal)) + (0.002 * step(0.01, sumRight) * step(0.5, rightSlowVal)), 0., 1.);
				// grow right hand
				//float rightOut = clamp(sourceRight + (1 * step(3, sumRight) * step(0.5, rightFastVal)) + (0.5 * step(3, sumRight) * step(0.5, rightSlowVal)), 0., 1.);
				float rightOut = clamp(sourceRight + (_Speeds.z * step(threshhold, sumRight) * step(0.5, rightFastVal)) + (_Speeds.y * step(threshhold, sumRight) * step(0.5, rightMedVal)) + (_Speeds.x * step(threshhold, sumRight) * step(0.5, rightSlowVal)), 0., 1.);
				//grow left hand
				//float leftOut = clamp(sourceLeft + (1 * step(3, sumLeft) * step(0.5, leftFastVal)) + (0.5 * step(3, sumLeft) * step(0.5, leftSlowVal)), 0., 1.);
				float leftOut = clamp(sourceLeft + (_Speeds.z * step(threshhold, sumLeft) * step(0.5, leftFastVal)) + (_Speeds.y * step(threshhold, sumLeft) * step(0.5, leftMedVal)) + (_Speeds.x * step(threshhold, sumLeft) * step(0.5, leftSlowVal)), 0., 1.);

				// increment "fade" of left and right values, then just clamp it!
				float rightFade = clamp(sourceVal.g + .003, 0., 1.) * step(.001, rightOut);
				float leftFade = clamp(sourceVal.a + .003, 0., 1.) * step(.001, leftOut);

				//adding new start position with the right hand?
				float aspectX = _ScreenParams.x / _ScreenParams.y;
				float rrange = 0.001 * _RightHand.z;
				//need to correct for aspect ratio
				float rdist = distance(float2(_RightHand.x * aspectX, _RightHand.y), float2(i.uv.x * aspectX, i.uv.y));
				// add to outVal if we have a right hand coordinate, we're in range of brush size, and there's either red or green in right mask
				float rightAppend = fixed4(1, 1, 1, cos(rdist / rrange) * .2) * step(0.01, _RightHand.x) *  step(rdist, rrange) * min(step(.01, rightFastVal) + step(.01, rightMedVal) + step(.01, rightSlowVal), 1.);
				rightOut += rightAppend;
				rightFade += .1 * step(.1, rightAppend);
				clamp(rightOut, 0, 1);

				//left hand
				float lrange = 0.001 * _LeftHand.z;
				//need to correct for aspect ratio
				float ldist = distance(float2(_LeftHand.x * aspectX, _LeftHand.y), float2(i.uv.x * aspectX, i.uv.y));
				// add to outVal if we have a left hand coordinat, we're in range of brush size, and there's either red or green in left mask
				float leftAppend = fixed4(1, 1, 1, cos(ldist / lrange) * .2) * step(0.01, _LeftHand.x) * step(ldist, lrange) * min(step(.01, leftFastVal) + step(.01, leftMedVal) + step(.01, leftSlowVal), 1.);
				leftOut += leftAppend;
				leftFade += .1 * step(.1, leftAppend);
				clamp(leftOut, 0, 1);

				
				
				fixed4 col = fixed4(rightOut, rightFade, leftOut, leftFade);
				return col;
			}
			ENDCG
		}
	}
}
