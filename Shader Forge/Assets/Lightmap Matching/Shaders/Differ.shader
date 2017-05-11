Shader "Unlit/Differ"
{
	Properties
	{
		_Unity ("Unity RT", 2D) = "white" {}
		_SF ("SF RT", 2D) = "white" {}
		_iMin ("Input min", Range(0,1)) = 0.0
		_iMax ("Input max", Range(0,1)) = 1.0
		_oMin ("Output min", Range(0,1)) = 0.0
		_oMax ("Output max", Range(0,1)) = 1.0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
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

			sampler2D _Unity;
			sampler2D _SF;

			float _iMin;
			float _iMax;
			float _oMin;
			float _oMax;

			// lerp
			// v = b * t + (1-t)*a
			// v = b * t + a - ta
			// v = t * (b-a) + a

			// inverse lerp
			// t = (v-a) / (b-a)

			// remap
			// oV = ( (v-iMin) / (iMax-iMin) ) * ( oMax - oMin ) + oMin

			float4 Remap( float4 v, float iMin, float iMax, float oMin, float oMax ){
				return ( (v-iMin) / (iMax-iMin) ) * ( oMax - oMin ) + oMin;
			}
			
			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target {
				
				float4 cUnity = tex2D(_Unity, i.uv);
				float4 cSF = tex2D(_SF, i.uv);

				return Remap( abs(cUnity - cSF), _iMin, _iMax, _oMin, _oMax);
			}
			ENDCG
		}
	}
}
