#ifndef AUTOLIGHT_INCLUDED
#define AUTOLIGHT_INCLUDED

#include "HLSLSupport.cginc"

// ------------ Shadow helpers --------


// ---- Screen space shadows
#if defined (SHADOWS_SCREEN)

uniform float4 _ShadowOffsets[4];

#if defined(SHADOWS_NATIVE)
UNITY_DECLARE_SHADOWMAP(_ShadowMapTexture);
#else
uniform sampler2D _ShadowMapTexture;
#endif

#define SHADOW_COORDS(idx1) float4 _ShadowCoord : TEXCOORD##idx1;

#if (defined(SHADER_API_GLES) || defined(SHADER_API_GLES3)) && defined(SHADER_API_MOBILE)
#define TRANSFER_SHADOW(a) a._ShadowCoord = mul( unity_World2Shadow[0], mul( _Object2World, v.vertex ) );

inline fixed unitySampleShadow (float4 shadowCoord)
{
	#if defined(SHADOWS_NATIVE)

	fixed shadow = UNITY_SAMPLE_SHADOW(_ShadowMapTexture, shadowCoord.xyz);
	shadow = _LightShadowData.r + shadow * (1-_LightShadowData.r);
	return shadow;

	#else

	float dist = tex2Dproj( _ShadowMapTexture, UNITY_PROJ_COORD(shadowCoord) ).x;

	// tegra is confused if we use _LightShadowData.x directly
	// with "ambiguous overloaded function reference max(mediump float, float)"
	half lightShadowDataX = _LightShadowData.x;
	return max(dist > (shadowCoord.z/shadowCoord.w), lightShadowDataX);

	#endif
}

#else // !((defined(SHADER_API_GLES) || defined(SHADER_API_GLES3)) && defined(SHADER_API_MOBILE))

#define TRANSFER_SHADOW(a) a._ShadowCoord = ComputeScreenPos(a.pos);

inline fixed unitySampleShadow (float4 shadowCoord)
{
	fixed shadow = tex2Dproj( _ShadowMapTexture, UNITY_PROJ_COORD(shadowCoord) ).r;
	return shadow;
}

#endif

#define SHADOW_ATTENUATION(a) unitySampleShadow(a._ShadowCoord)

#endif


// ---- Depth map shadows

#if defined (SHADOWS_DEPTH) && defined (SPOT)

#if !defined(SHADOWMAPSAMPLER_DEFINED)
UNITY_DECLARE_SHADOWMAP(_ShadowMapTexture);
#endif
#if defined (SHADOWS_SOFT)
uniform float4 _ShadowOffsets[4];
#endif

inline fixed unitySampleShadow (float4 shadowCoord)
{
	#if defined (SHADOWS_SOFT)

	// 4-tap shadows

	float3 coord = shadowCoord.xyz / shadowCoord.w;
	#if defined (SHADOWS_NATIVE)
	half4 shadows;
	shadows.x = UNITY_SAMPLE_SHADOW(_ShadowMapTexture, coord + _ShadowOffsets[0]);
	shadows.y = UNITY_SAMPLE_SHADOW(_ShadowMapTexture, coord + _ShadowOffsets[1]);
	shadows.z = UNITY_SAMPLE_SHADOW(_ShadowMapTexture, coord + _ShadowOffsets[2]);
	shadows.w = UNITY_SAMPLE_SHADOW(_ShadowMapTexture, coord + _ShadowOffsets[3]);
	shadows = _LightShadowData.rrrr + shadows * (1-_LightShadowData.rrrr);
	#else
	float4 shadowVals;
	shadowVals.x = UNITY_SAMPLE_DEPTH (tex2D( _ShadowMapTexture, coord + _ShadowOffsets[0].xy ));
	shadowVals.y = UNITY_SAMPLE_DEPTH (tex2D( _ShadowMapTexture, coord + _ShadowOffsets[1].xy ));
	shadowVals.z = UNITY_SAMPLE_DEPTH (tex2D( _ShadowMapTexture, coord + _ShadowOffsets[2].xy ));
	shadowVals.w = UNITY_SAMPLE_DEPTH (tex2D( _ShadowMapTexture, coord + _ShadowOffsets[3].xy ));
	half4 shadows = (shadowVals < coord.zzzz) ? _LightShadowData.rrrr : 1.0f;
	#endif

	// average-4 PCF
	half shadow = dot (shadows, 0.25f);

	#else

	// 1-tap shadows

	#if defined (SHADOWS_NATIVE)
	half shadow = UNITY_SAMPLE_SHADOW_PROJ(_ShadowMapTexture, shadowCoord);
	shadow = _LightShadowData.r + shadow * (1-_LightShadowData.r);
	#else
	half shadow = UNITY_SAMPLE_DEPTH(tex2Dproj (_ShadowMapTexture, UNITY_PROJ_COORD(shadowCoord))) < (shadowCoord.z / shadowCoord.w) ? _LightShadowData.r : 1.0;
	#endif

	#endif

	return shadow;
}
#define SHADOW_COORDS(idx1) float4 _ShadowCoord : TEXCOORD##idx1;
#define TRANSFER_SHADOW(a) a._ShadowCoord = mul (unity_World2Shadow[0], mul(_Object2World,v.vertex));
#define SHADOW_ATTENUATION(a) unitySampleShadow(a._ShadowCoord)

#endif


// ---- Point light shadows

#if defined (SHADOWS_CUBE)

uniform samplerCUBE _ShadowMapTexture;
inline float SampleCubeDistance (float3 vec)
{
	float4 packDist = texCUBE (_ShadowMapTexture, vec);
	return DecodeFloatRGBA( packDist );
}
inline float unityCubeShadow (float3 vec)
{
	float mydist = length(vec) * _LightPositionRange.w;
	mydist *= 0.97; // bias

	#if defined (SHADOWS_SOFT)
	float z = 1.0/128.0;
	float4 shadowVals;
	shadowVals.x = SampleCubeDistance (vec+float3( z, z, z));
	shadowVals.y = SampleCubeDistance (vec+float3(-z,-z, z));
	shadowVals.z = SampleCubeDistance (vec+float3(-z, z,-z));
	shadowVals.w = SampleCubeDistance (vec+float3( z,-z,-z));
	half4 shadows = (shadowVals < mydist.xxxx) ? _LightShadowData.rrrr : 1.0f;
	return dot(shadows,0.25);
	#else
	float dist = SampleCubeDistance (vec);
	return dist < mydist ? _LightShadowData.r : 1.0;
	#endif
}
#define SHADOW_COORDS(idx1) float3 _ShadowCoord : TEXCOORD##idx1;
#define TRANSFER_SHADOW(a) a._ShadowCoord = mul(_Object2World, v.vertex).xyz - _LightPositionRange.xyz;
#define SHADOW_ATTENUATION(a) unityCubeShadow(a._ShadowCoord)

#endif



// ---- Shadows off
#if !defined (SHADOWS_SCREEN) && !defined (SHADOWS_DEPTH) && !defined (SHADOWS_CUBE)

#define SHADOW_COORDS(idx1)
#define TRANSFER_SHADOW(a)
#define SHADOW_ATTENUATION(a) 1.0

#endif



// ------------ Light helpers --------

#ifdef POINT
#define LIGHTING_COORDS(idx1,idx2) float3 _LightCoord : TEXCOORD##idx1; SHADOW_COORDS(idx2)
uniform sampler2D _LightTexture0;
uniform float4x4 _LightMatrix0;
#define TRANSFER_VERTEX_TO_FRAGMENT(a) a._LightCoord = mul(_LightMatrix0, mul(_Object2World, v.vertex)).xyz; TRANSFER_SHADOW(a)
#define LIGHT_ATTENUATION(a)	(tex2D(_LightTexture0, dot(a._LightCoord,a._LightCoord).rr).UNITY_ATTEN_CHANNEL * SHADOW_ATTENUATION(a))
#endif

#ifdef SPOT
#define LIGHTING_COORDS(idx1,idx2) float4 _LightCoord : TEXCOORD##idx1; SHADOW_COORDS(idx2)
uniform sampler2D _LightTexture0;
uniform float4x4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
#define TRANSFER_VERTEX_TO_FRAGMENT(a) a._LightCoord = mul(_LightMatrix0, mul(_Object2World, v.vertex)); TRANSFER_SHADOW(a)
inline fixed UnitySpotCookie(float4 LightCoord)
{
	return tex2D(_LightTexture0, LightCoord.xy / LightCoord.w + 0.5).w;
}
inline fixed UnitySpotAttenuate(float3 LightCoord)
{
	return tex2D(_LightTextureB0, dot(LightCoord, LightCoord).xx).UNITY_ATTEN_CHANNEL;
}
#define LIGHT_ATTENUATION(a)	( (a._LightCoord.z > 0) * UnitySpotCookie(a._LightCoord) * UnitySpotAttenuate(a._LightCoord.xyz) * SHADOW_ATTENUATION(a) )
#endif


#ifdef DIRECTIONAL
	#define LIGHTING_COORDS(idx1,idx2) SHADOW_COORDS(idx1)
	#define TRANSFER_VERTEX_TO_FRAGMENT(a) TRANSFER_SHADOW(a)
	#define LIGHT_ATTENUATION(a)	SHADOW_ATTENUATION(a)
#endif


#ifdef POINT_COOKIE
#define LIGHTING_COORDS(idx1,idx2) float3 _LightCoord : TEXCOORD##idx1; SHADOW_COORDS(idx2)
uniform samplerCUBE _LightTexture0;
uniform float4x4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
#define TRANSFER_VERTEX_TO_FRAGMENT(a) a._LightCoord = mul(_LightMatrix0, mul(_Object2World, v.vertex)).xyz; TRANSFER_SHADOW(a)
#define LIGHT_ATTENUATION(a)	(tex2D(_LightTextureB0, dot(a._LightCoord,a._LightCoord).rr).UNITY_ATTEN_CHANNEL * texCUBE(_LightTexture0, a._LightCoord).w * SHADOW_ATTENUATION(a))
#endif

#ifdef DIRECTIONAL_COOKIE
#define LIGHTING_COORDS(idx1,idx2) float2 _LightCoord : TEXCOORD##idx1; SHADOW_COORDS(idx2)
uniform sampler2D _LightTexture0;
uniform float4x4 _LightMatrix0;
#define TRANSFER_VERTEX_TO_FRAGMENT(a) a._LightCoord = mul(_LightMatrix0, mul(_Object2World, v.vertex)).xy; TRANSFER_SHADOW(a)
#define LIGHT_ATTENUATION(a)	(tex2D(_LightTexture0, a._LightCoord).w * SHADOW_ATTENUATION(a))
#endif


#endif
