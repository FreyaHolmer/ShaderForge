// Upgrade NOTE: commented out 'float3 _WorldSpaceCameraPos', a built-in variable
// Upgrade NOTE: commented out 'float4 unity_ShadowFadeCenterAndType', a built-in variable
// Upgrade NOTE: commented out 'float4x4 _Object2World', a built-in variable
// Upgrade NOTE: commented out 'float4x4 _World2Object', a built-in variable
// Upgrade NOTE: replaced 'glstate.lightmodel.ambient' with 'UNITY_LIGHTMODEL_AMBIENT'
// Upgrade NOTE: replaced 'glstate.matrix.invtrans.modelview[0]' with 'UNITY_MATRIX_IT_MV'
// Upgrade NOTE: replaced 'glstate.matrix.modelview[0]' with 'UNITY_MATRIX_MV'
// Upgrade NOTE: replaced 'glstate.matrix.mvp' with 'UNITY_MATRIX_MVP'
// Upgrade NOTE: replaced 'glstate.matrix.projection' with 'UNITY_MATRIX_P'
// Upgrade NOTE: replaced 'glstate.matrix.texture' with 'UNITY_MATRIX_TEXTURE'
// Upgrade NOTE: replaced 'glstate.matrix.texture[0]' with 'UNITY_MATRIX_TEXTURE0'
// Upgrade NOTE: replaced 'glstate.matrix.texture[1]' with 'UNITY_MATRIX_TEXTURE1'
// Upgrade NOTE: replaced 'glstate.matrix.texture[2]' with 'UNITY_MATRIX_TEXTURE2'
// Upgrade NOTE: replaced 'glstate.matrix.texture[3]' with 'UNITY_MATRIX_TEXTURE3'
// Upgrade NOTE: replaced 'glstate.matrix.transpose.modelview[0]' with 'UNITY_MATRIX_T_MV'

#ifndef UNITY_SHADER_VARIABLES_INCLUDED
#define UNITY_SHADER_VARIABLES_INCLUDED

#include "HLSLSupport.cginc"

#if defined (DIRECTIONAL_COOKIE) || defined (DIRECTIONAL)
#define USING_DIRECTIONAL_LIGHT
#endif



// ----------------------------------------------------------------------------

CBUFFER_START(UnityPerCamera)
	// Time values from Unity
	uniform float4 _Time;
	uniform float4 _SinTime;
	uniform float4 _CosTime;
	uniform float4 unity_DeltaTime; // dt, 1/dt, smoothdt, 1/smoothdt
	
	// uniform float3 _WorldSpaceCameraPos;
	
	// x = 1 or -1 (-1 if projection is flipped)
	// y = near plane
	// z = far plane
	// w = 1/far plane
	uniform float4 _ProjectionParams;
	
	// x = width
	// y = height
	// z = 1 + 1.0/width
	// w = 1 + 1.0/height
	uniform float4 _ScreenParams;
	
	uniform float4 _ZBufferParams;
CBUFFER_END


CBUFFER_START(UnityPerCameraRare)
	uniform float4 unity_CameraWorldClipPlanes[6];

CBUFFER_END



// ----------------------------------------------------------------------------

CBUFFER_START(UnityLighting)

	#ifdef USING_DIRECTIONAL_LIGHT
	uniform fixed4 _WorldSpaceLightPos0;
	#else
	uniform float4 _WorldSpaceLightPos0;
	#endif

	uniform float4 _LightPositionRange; // xyz = pos, w = 1/range

	float4 unity_4LightPosX0;
	float4 unity_4LightPosY0;
	float4 unity_4LightPosZ0;
	float4 unity_4LightAtten0;

	float4 unity_LightColor[4];
	float4 unity_LightPosition[4];
	// x = -1
	// y = 1
	// z = quadratic attenuation
	// w = range^2
	float4 unity_LightAtten[4];

	// SH lighting environment
	float4 unity_SHAr;
	float4 unity_SHAg;
	float4 unity_SHAb;
	float4 unity_SHBr;
	float4 unity_SHBg;
	float4 unity_SHBb;
	float4 unity_SHC;
CBUFFER_END

CBUFFER_START(UnityLightingOld)
	float3 unity_LightColor0, unity_LightColor1, unity_LightColor2, unity_LightColor3; // keeping those only for any existing shaders; remove in 4.0
CBUFFER_END


// ----------------------------------------------------------------------------

CBUFFER_START(UnityShadows)
	float4 unity_ShadowSplitSpheres[4];
	float4 unity_ShadowSplitSqRadii;
	float4 unity_LightShadowBias;
	float4 _LightSplitsNear;
	float4 _LightSplitsFar;
	float4x4 unity_World2Shadow[4];
	float4 _LightShadowData;
	// float4 unity_ShadowFadeCenterAndType;
CBUFFER_END

#define _World2Shadow unity_World2Shadow[0]
#define _World2Shadow1 unity_World2Shadow[1]
#define _World2Shadow2 unity_World2Shadow[2]
#define _World2Shadow3 unity_World2Shadow[3]


// ----------------------------------------------------------------------------

CBUFFER_START(UnityPerDraw)
	#if defined(SHADER_API_XBOX360) || defined(SHADER_API_D3D11) || defined (SHADER_TARGET_GLSL) || defined(SHADER_API_D3D11_9X)
		float4x4 glstate_matrix_mvp;
		float4x4 glstate_matrix_modelview0;
		float4x4 glstate_matrix_invtrans_modelview0;
		#define UNITY_MATRIX_MVP glstate_matrix_mvp
		#define UNITY_MATRIX_MV glstate_matrix_modelview0
		#define UNITY_MATRIX_IT_MV glstate_matrix_invtrans_modelview0
	#else
		#define UNITY_MATRIX_MVP UNITY_MATRIX_MVP
		#define UNITY_MATRIX_MV UNITY_MATRIX_MV
		#define UNITY_MATRIX_IT_MV UNITY_MATRIX_IT_MV
	#endif
	
	// uniform float4x4 _Object2World;
	// uniform float4x4 _World2Object;
		
	uniform float4 unity_Scale; // w = 1 / uniform scale
CBUFFER_END




CBUFFER_START(UnityPerDrawRare)
	#if defined(SHADER_API_XBOX360) || defined(SHADER_API_D3D11) || defined (SHADER_TARGET_GLSL) || defined(SHADER_API_D3D11_9X)
		float4x4 glstate_matrix_transpose_modelview0;
		#define UNITY_MATRIX_T_MV glstate_matrix_transpose_modelview0
	#else
		#define UNITY_MATRIX_T_MV UNITY_MATRIX_T_MV
	#endif
CBUFFER_END



// ----------------------------------------------------------------------------

CBUFFER_START(UnityPerDrawTexMatrices)
	#if defined(SHADER_API_XBOX360) || defined(SHADER_API_D3D11) || defined (SHADER_TARGET_GLSL) || defined(SHADER_API_D3D11_9X)
		#ifndef SHADER_TARGET_GLSL
		float4x4 glstate_matrix_texture[8];
		#endif
		float4x4 glstate_matrix_texture0;
		float4x4 glstate_matrix_texture1;
		float4x4 glstate_matrix_texture2;
		float4x4 glstate_matrix_texture3;
		#define UNITY_MATRIX_TEXTURE glstate_matrix_texture
		#define UNITY_MATRIX_TEXTURE0 glstate_matrix_texture0
		#define UNITY_MATRIX_TEXTURE1 glstate_matrix_texture1
		#define UNITY_MATRIX_TEXTURE2 glstate_matrix_texture2
		#define UNITY_MATRIX_TEXTURE3 glstate_matrix_texture3
	#else
		#define UNITY_MATRIX_TEXTURE UNITY_MATRIX_TEXTURE
		#define UNITY_MATRIX_TEXTURE0 UNITY_MATRIX_TEXTURE0
		#define UNITY_MATRIX_TEXTURE1 UNITY_MATRIX_TEXTURE1
		#define UNITY_MATRIX_TEXTURE2 UNITY_MATRIX_TEXTURE2
		#define UNITY_MATRIX_TEXTURE3 UNITY_MATRIX_TEXTURE3
	#endif
CBUFFER_END


// ----------------------------------------------------------------------------

CBUFFER_START(UnityPerFrame)

	#if defined(SHADER_API_XBOX360) || defined(SHADER_API_D3D11) || defined (SHADER_TARGET_GLSL) || defined(SHADER_API_D3D11_9X)
		float4x4 glstate_matrix_projection;
		float4	 glstate_lightmodel_ambient;
		#define UNITY_MATRIX_P glstate_matrix_projection
		#define UNITY_LIGHTMODEL_AMBIENT glstate_lightmodel_ambient
	#else
		#define UNITY_MATRIX_P UNITY_MATRIX_P
		#define UNITY_LIGHTMODEL_AMBIENT UNITY_LIGHTMODEL_AMBIENT
	#endif
	
	float4x4 unity_MatrixV;
	float4x4 unity_MatrixVP;
	#define UNITY_MATRIX_V unity_MatrixV
	#define UNITY_MATRIX_VP unity_MatrixVP

CBUFFER_END


#endif
