Shader "Hidden/BlitCopy" {
	Properties { _MainTex ("", any) = "" {} }
	SubShader { 
		Pass {
 			ZTest Always Cull Off ZWrite Off Fog { Mode Off }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			
			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.texcoord = v.texcoord.xy;
				return o;
			}

			fixed4 frag (v2f i) : COLOR
			{
				return tex2D(_MainTex, i.texcoord);
			}
			ENDCG 

		}
	}
	SubShader { 
		Pass {
 			ZTest Always Cull Off ZWrite Off Fog { Mode Off }
			SetTexture [_MainTex] { combine texture }
		}
	}
	Fallback Off 
}
