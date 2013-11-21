// Only used on D3D11 for non-fullscreen clears
Shader "Hidden/InternalClear" {

	CGINCLUDE
	#pragma vertex vert
	#pragma fragment frag
	#include "UnityShaderVariables.cginc"

	struct appdata_t {
		float4 vertex : POSITION;
		fixed4 color : COLOR;
	};

	struct v2f {
		float4 vertex : SV_POSITION;
		fixed4 color : COLOR;
	};

	v2f vert (appdata_t v)
	{
		v2f o;
		o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
		o.color = v.color;
		return o;
	}

	fixed4 frag (v2f i) : COLOR
	{
		return i.color;
	}
	ENDCG 

	SubShader { 
		ZTest Always Cull Off Fog { Mode Off }
		Pass {
			ColorMask 0 ZWrite Off
			CGPROGRAM
			ENDCG
		}
		Pass {
			ZWrite Off
			CGPROGRAM
			ENDCG
		}
		Pass {
			ColorMask 0
			CGPROGRAM
			ENDCG
		}
		Pass {
			CGPROGRAM
			ENDCG
		}
	}
	Fallback Off 
}
