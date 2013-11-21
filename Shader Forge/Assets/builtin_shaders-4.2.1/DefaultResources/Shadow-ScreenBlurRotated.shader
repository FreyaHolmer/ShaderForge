Shader "Hidden/Shadow-ScreenBlurRotated" {
Properties {
	_MainTex ("Base", 2D) = "white" {}
}
SubShader {
	Pass {
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }
		
CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag
#pragma exclude_renderers noshadows
#pragma fragmentoption ARB_precision_hint_fastest
#pragma target 3.0
#include "UnityCG.cginc"


#if SHADER_API_PS3
#	define shadowPrecission float
#	define shadowPrecission4 float4
#else
#	define shadowPrecission half
#	define shadowPrecission4 half4
#endif


uniform sampler2D _MainTex;
uniform sampler2D unity_RandomRotation16;

// x,y of each - sample offset for blur
uniform float4 _BlurOffsets0;
uniform float4 _BlurOffsets1;
uniform float4 _BlurOffsets2;
uniform float4 _BlurOffsets3;
uniform float4 _BlurOffsets4;
uniform float4 _BlurOffsets5;
uniform float4 _BlurOffsets6;
uniform float4 _BlurOffsets7;

// @TODO: Should be possible to calc offset and scale in the dot products here. Should be faster on platforms with free swizzles (like.. not PS3)
inline float2 GetRotatedTexCoord(float2 offsets, float4 rotation)
{
	float2 offset;
	offset.x = dot( offsets.xy, rotation.rg );
	offset.y = dot( offsets.xy, rotation.ba );
	return offset;
}

float4 unity_ShadowBlurParams;
#define LOOP_ITERATION(i) { 	\
	shadowPrecission4 sample = tex2D( _MainTex, coord + radius * GetRotatedTexCoord(_BlurOffsets##i.xy, rotation) ); \
	shadowPrecission sampleDist = sample.b + sample.a / 255.0; \
	shadowPrecission diff = dist - sampleDist; \
	diff = saturate( diffTolerance - abs(diff) ); \
	mask.xy += diff * sample.xy; }

fixed4 frag (v2f_img i) : COLOR
{
	float4 coord = float4(i.uv,0,0);
	const float randomRotationTextureSize = 16.0f;
	
	shadowPrecission4 rotation = 2.0 * tex2D( unity_RandomRotation16, (coord.xy * _ScreenParams.xy) / randomRotationTextureSize ) - 1.0;
	shadowPrecission4 mask = tex2D( _MainTex, coord.xy );
	shadowPrecission dist = mask.b + mask.a / 255.0;
	shadowPrecission radius = saturate(unity_ShadowBlurParams.y / (1.0-dist));
	
	shadowPrecission diffTolerance = unity_ShadowBlurParams.x;
	
	mask.xy *= diffTolerance;

	// Would this code look better in a loop? You bet.
	// But, that requires using a uniform array in GLSL,
	// which means that shaderlab would need support for array parameters.
	// So, until we have that, this needs to be unrolled to work in GLSL, 
	// then we can revert to the looped version.
	
	LOOP_ITERATION (0);
	LOOP_ITERATION (1);
	LOOP_ITERATION (2);
	LOOP_ITERATION (3);
	LOOP_ITERATION (4);
	LOOP_ITERATION (5);
	LOOP_ITERATION (6);
	LOOP_ITERATION (7);

	shadowPrecission shadow = mask.x / mask.y;
	return shadow;
}
ENDCG
	}	
}

Fallback Off
}
