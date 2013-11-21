Shader "Hidden/Shadow-ScreenBlur" {
Properties {
	_MainTex ("Base", 2D) = "white" {}
}
SubShader {
	Pass {
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }
		
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma exclude_renderers noshadows
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

v2f_img vert (appdata_img v)
{
	v2f_img o;
	o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
	o.uv = v.texcoord.xy;
	#if SHADER_API_FLASH
	o.uv.xy *= unity_NPOTScale.xy;
	#endif
	return o;
}


uniform sampler2D _MainTex;

// x,y of each - sample offset for blur
uniform float4 _BlurOffsets0;
uniform float4 _BlurOffsets1;
uniform float4 _BlurOffsets2;
uniform float4 _BlurOffsets3;
uniform float4 _BlurOffsets4;
uniform float4 _BlurOffsets5;
uniform float4 _BlurOffsets6;
uniform float4 _BlurOffsets7;

float4 unity_ShadowBlurParams;



#define LOOP_ITERATION(i) { 	\
	float4 sample = tex2D( _MainTex, (coord + radius * _BlurOffsets##i).xy ); \
	float sampleDist = sample.b + sample.a / 255.0; \
	float diff = dist - sampleDist; \
	diff = saturate( diffTolerance - abs(diff) ); \
	mask.xy += diff * sample.xy; }

fixed4 frag (v2f_img i) : COLOR
{
	float4 coord = float4(i.uv,0,0);
	float4 mask = tex2D( _MainTex, coord.xy );
	float dist = mask.b + mask.a / 255.0;
	float radius = saturate(unity_ShadowBlurParams.y / (1.0-dist));
	
	float diffTolerance = unity_ShadowBlurParams.x;
	
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
	
	// In Flash, due to very limited register count we can't do more samples :(
	#ifndef SHADER_API_FLASH
	LOOP_ITERATION (4);
	LOOP_ITERATION (5);
	LOOP_ITERATION (6);
	LOOP_ITERATION (7);
	#endif

	float shadow = mask.x / mask.y;
	return shadow;
}
ENDCG
	}	
}

Fallback Off
} 