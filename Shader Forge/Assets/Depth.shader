Shader "Render Depth" {
Properties {
	_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
}
SubShader {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Pass {
		Fog { Mode Off }
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

sampler2D _CameraDepthTexture;
float _InvFade;

struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

struct v2f {
	float4 vertex : SV_POSITION;
	float2 depth : TEXCOORD0;
	float4 projPos : TEXCOORD1;
};

v2f vert (appdata_t v) {
	v2f o;
	o.vertex = mul (UNITY_MATRIX_MVP, v.vertex);
	o.projPos = ComputeScreenPos (o.vertex);
	COMPUTE_EYEDEPTH(o.projPos.z);
	return o;
}

half4 frag(v2f i) : COLOR {

	float sceneZ = LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos))));
	float partZ = i.projPos.z;
	return half4(_InvFade*saturate(sceneZ-partZ));
}
ENDCG
	}
}
}
