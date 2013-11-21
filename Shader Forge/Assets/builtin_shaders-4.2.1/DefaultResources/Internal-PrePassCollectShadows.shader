// Collects cascaded shadows into screen space buffer ready for blurring
Shader "Hidden/Internal-PrePassCollectShadows" {
Properties {
	_ShadowMapTexture ("", any) = "" {}
}
SubShader {
Pass {
	ZWrite Off ZTest Always Cull Off Fog { Mode Off }

CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma exclude_renderers noshadows flash
#pragma glsl_no_auto_normalization
#pragma multi_compile_shadowcollector

#include "UnityCG.cginc"
struct appdata {
	float4 vertex : POSITION;
	float2 texcoord : TEXCOORD0;
	float3 normal : NORMAL;
};

struct v2f {
	float4 pos : SV_POSITION;
	float2 uv : TEXCOORD0;
	float3 ray : TEXCOORD1;
};

v2f vert (appdata v)
{
	v2f o;
	o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
	o.uv = v.texcoord;
	o.ray = v.normal;
	return o;
}
sampler2D _CameraDepthTexture;
float4 unity_LightmapFade;

CBUFFER_START(UnityPerCamera2)
float4x4 _CameraToWorld;
CBUFFER_END

UNITY_DECLARE_SHADOWMAP(_ShadowMapTexture);

inline float SquareLength(float3 vec)
{
	return dot(vec, vec);
}

inline half unitySampleShadow (float4 wpos, float z)
{
	float3 sc0 = mul (unity_World2Shadow[0], wpos).xyz;
	float3 sc1 = mul (unity_World2Shadow[1], wpos).xyz;
	float3 sc2 = mul (unity_World2Shadow[2], wpos).xyz;
	float3 sc3 = mul (unity_World2Shadow[3], wpos).xyz;

#if defined (SHADOWS_SPLIT_SPHERES)
	float3 fromCenter0 = wpos.xyz - unity_ShadowSplitSpheres[0].xyz;
	float3 fromCenter1 = wpos.xyz - unity_ShadowSplitSpheres[1].xyz;
	float3 fromCenter2 = wpos.xyz - unity_ShadowSplitSpheres[2].xyz;
	float3 fromCenter3 = wpos.xyz - unity_ShadowSplitSpheres[3].xyz;
	float4 distances2 = float4(dot(fromCenter0,fromCenter0), dot(fromCenter1,fromCenter1), dot(fromCenter2,fromCenter2), dot(fromCenter3,fromCenter3));
	float4 weights = float4(distances2 < unity_ShadowSplitSqRadii);
	weights.yzw = saturate(weights.yzw - weights.xyz);
#else
	float4 zNear = float4( z >= _LightSplitsNear );
	float4 zFar = float4( z < _LightSplitsFar );
	float4 weights = zNear * zFar;
#endif
	
	float4 coord = float4(sc0 * weights[0] + sc1 * weights[1] + sc2 * weights[2] + sc3 * weights[3], 1);
#if defined (SHADOWS_NATIVE)
	half shadow = UNITY_SAMPLE_SHADOW(_ShadowMapTexture,coord);
	shadow = lerp(_LightShadowData.r, 1.0, shadow);
#else
	half shadow = UNITY_SAMPLE_DEPTH(tex2D (_ShadowMapTexture, coord.xy)) < coord.z ? _LightShadowData.r : 1.0;
#endif
	//shadow = dot(weights, float4(0,0.33,0.66,1)*0.33);
	return shadow;
}

fixed4 frag (v2f i) : COLOR
{
	float depth = UNITY_SAMPLE_DEPTH(tex2D (_CameraDepthTexture, i.uv));
	depth = Linear01Depth (depth);
	float4 vpos = float4(i.ray * depth,1);
	float4 wpos = mul (_CameraToWorld, vpos);	
	half shadow = unitySampleShadow (wpos, vpos.z);
	float4 res;
	res.x = shadow;
	res.y = 1.0;
	res.zw = EncodeFloatRG (1 - depth);
	return res;	
}

ENDCG
}

}
Fallback Off
}
