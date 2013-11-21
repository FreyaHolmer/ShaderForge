// Upgrade NOTE: replaced 'samplerRECT' with 'sampler2D'
// Upgrade NOTE: replaced 'texRECT' with 'tex2D'
// Upgrade NOTE: replaced 'texRECTbias' with 'tex2Dbias'
// Upgrade NOTE: replaced 'texRECTlod' with 'tex2Dlod'
// Upgrade NOTE: replaced 'texRECTproj' with 'tex2Dproj'

#ifndef HLSL_SUPPORT_INCLUDED
#define HLSL_SUPPORT_INCLUDED

#if defined(SHADER_API_D3D11) || defined(SHADER_API_XBOX360) || defined(SHADER_API_D3D11_9X)
#pragma warning (disable : 3205) // conversion of larger type to smaller
#pragma warning (disable : 3568) // unknown pragma ignored
#endif

#if !defined(SHADER_TARGET_GLSL)
#define fixed half
#define fixed2 half2
#define fixed3 half3
#define fixed4 half4
#define fixed4x4 half4x4
#define fixed3x3 half3x3
#define fixed2x2 half2x2
#define sampler2D_half sampler2D
#define sampler2D_float sampler2D
#define samplerCUBE_half samplerCUBE
#define samplerCUBE_float samplerCUBE
#endif


#if defined(SHADER_API_D3D11) || defined(SHADER_API_D3D11_9X)
#define CBUFFER_START(name) cbuffer name {
#define CBUFFER_END };
#else
#define CBUFFER_START(name)
#define CBUFFER_END
#endif

#if defined(SHADOWS_NATIVE) && (defined(SHADER_API_D3D11) || defined(SHADER_API_D3D11_9X))
	// DX11 syntax for shadow maps
	#define UNITY_DECLARE_SHADOWMAP(tex) Texture2D tex; SamplerComparisonState sampler##tex
	#define UNITY_SAMPLE_SHADOW(tex,coord) tex.SampleCmpLevelZero (sampler##tex,(coord).xy,(coord).z)
	#define UNITY_SAMPLE_SHADOW_PROJ(tex,coord) tex.SampleCmpLevelZero (sampler##tex,(coord).xy/(coord).w,(coord).z/(coord).w)
#elif defined(SHADOWS_NATIVE) && defined(SHADER_TARGET_GLSL)
	// hlsl2glsl syntax for shadow maps
	#define UNITY_DECLARE_SHADOWMAP(tex) sampler2DShadow tex
	#define UNITY_SAMPLE_SHADOW(tex,coord) shadow2D (tex,(coord).xyz)
	#define UNITY_SAMPLE_SHADOW_PROJ(tex,coord) shadow2Dproj (tex,coord)
#else
	// Cg syntax for shadow maps
	#define UNITY_DECLARE_SHADOWMAP(tex) sampler2D tex
	#define UNITY_SAMPLE_SHADOW(tex,coord) tex2D (tex,(coord).xyz).r
	#define UNITY_SAMPLE_SHADOW_PROJ(tex,coord) tex2Dproj (tex,coord).r
#endif



#define sampler2D sampler2D
#define tex2D tex2D
#define tex2Dlod tex2Dlod
#define tex2Dbias tex2Dbias
#define tex2Dproj tex2Dproj

#if (defined(SHADER_API_D3D9) || defined(SHADER_API_OPENGL) || defined(SHADER_API_PS3)) && !defined(SHADER_TARGET_GLSL)
// Cg seems to use WPOS instead of VPOS semantic?
#define VPOS WPOS
// Cg does not have tex2Dgrad and friends, but has tex2D overload that
// can take the derivatives
half4 tex2Dgrad (in sampler2D s, in float2 t, in float2 dx, in float2 dy) { return tex2D (s, t, dx, dy); }
#endif

#if !defined(SHADER_API_XBOX360) && !defined(SHADER_API_PS3) && !defined(SHADER_API_GLES) && !defined(SHADER_API_GLES3) && !defined(SHADER_TARGET_GLSL) && !defined(SHADER_API_D3D11) && !defined(SHADER_API_D3D11_9X)
#define UNITY_HAS_LIGHT_PARAMETERS 1
#endif

#if defined(SHADER_API_XBOX360)

float4 tex2Dproj(in sampler2D s, in float4 t) 
{ 
	float2 ti=t.xy / t.w;
	return tex2D( s, ti);
}

float4 tex2Dproj(in sampler2D s, in float3 t) 
{ 
	float2 ti=t.xy / t.z;
	return tex2D( s, ti);
}


#endif



#if defined(SHADER_API_XBOX360) || defined(SHADER_API_D3D11) || defined(SHADER_API_D3D11_9X) || defined (SHADER_TARGET_GLSL)
#define FOGC FOG
#endif


#if !defined(SHADER_API_D3D11) && !defined(SHADER_API_D3D11_9X)
#define SV_POSITION POSITION
#endif


#if defined(SHADER_API_D3D9) || defined(SHADER_API_XBOX360) || defined(SHADER_API_PS3) || defined(SHADER_API_D3D11) || defined(SHADER_API_D3D11_9X)
#define UNITY_ATTEN_CHANNEL r
#else
#define UNITY_ATTEN_CHANNEL a
#endif

#if defined(SHADER_API_D3D9) || defined(SHADER_API_XBOX360)
#define UNITY_HALF_TEXEL_OFFSET
#endif

#if defined(SHADER_API_D3D9) || defined(SHADER_API_XBOX360) || defined(SHADER_API_PS3) || defined(SHADER_API_FLASH) || defined(SHADER_API_D3D11) || defined(SHADER_API_D3D11_9X)
#define UNITY_UV_STARTS_AT_TOP 1
#endif

#if defined(SHADER_API_D3D9) || defined(SHADER_API_XBOX360) || defined(SHADER_API_PS3) || defined(SHADER_API_FLASH) || defined(SHADER_API_D3D11) || defined(SHADER_API_D3D11_9X)
#define UNITY_NEAR_CLIP_VALUE (0.0)
#else
#define UNITY_NEAR_CLIP_VALUE (-1.0)
#endif


#if defined(SHADER_API_D3D9) || defined (SHADER_API_FLASH)
#define UNITY_MIGHT_NOT_HAVE_DEPTH_TEXTURE
#endif


#if defined(SHADER_API_OPENGL) && !defined(SHADER_TARGET_GLSL)
#define UNITY_BUGGY_TEX2DPROJ4
#define UNITY_PROJ_COORD(a) (a).xyw
#else
#define UNITY_PROJ_COORD(a) a
#endif


#if defined(SHADER_API_D3D11) || defined(SHADER_API_XBOX360) || defined(SHADER_API_D3D11_9X)
#define UNITY_COMPILER_HLSL
#elif defined(SHADER_TARGET_GLSL)
#define UNITY_COMPILER_HLSL2GLSL
#else
#define UNITY_COMPILER_CG
#endif


#if defined(UNITY_COMPILER_HLSL)
#define UNITY_INITIALIZE_OUTPUT(type,name) name = (type)0;
#else
#define UNITY_INITIALIZE_OUTPUT(type,name)
#endif

#if defined(SHADER_API_D3D11)
#define UNITY_CAN_COMPILE_TESSELLATION 1
#endif


// Not really needed anymore, but did ship in Unity 4.0; with D3D11_9X remapping them to .r channel.
// Now that's not used.
#define UNITY_SAMPLE_1CHANNEL(x,y) tex2D(x,y).a
#define UNITY_ALPHA_CHANNEL a


#endif
