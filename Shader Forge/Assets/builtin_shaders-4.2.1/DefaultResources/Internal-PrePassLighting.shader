Shader "Hidden/Internal-PrePassLighting" {
Properties {
	_LightTexture0 ("", any) = "" {}
	_LightTextureB0 ("", 2D) = "" {}
	_ShadowMapTexture ("", any) = "" {}
}
SubShader {

CGINCLUDE
#include "UnityCG.cginc"
struct appdata {
	float4 vertex : POSITION;
	float3 normal : NORMAL;
};

struct v2f {
	float4 pos : SV_POSITION;
	float4 uv : TEXCOORD0;
	float3 ray : TEXCOORD1;
};

v2f vert (appdata v)
{
	v2f o;
	o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
	o.uv = ComputeScreenPos (o.pos);
	o.ray = mul (UNITY_MATRIX_MV, v.vertex).xyz * float3(-1,-1,1);
	
	// v.texcoord is equal to 0 when we are drawing 3D light shapes and
	// contains a ray pointing from the camera to one of near plane's
	// corners in camera space when we are drawing a full screen quad.
	o.ray = lerp(o.ray, v.normal, v.normal.z != 0);
	
	return o;
}
sampler2D _CameraNormalsTexture;
sampler2D _CameraDepthTexture;
float4 _LightDir;
float4 _LightPos;
float4 _LightColor;
float4 unity_LightmapFade;
CBUFFER_START(UnityPerCamera2)
float4x4 _CameraToWorld;
CBUFFER_END
float4x4 _LightMatrix0;
sampler2D _LightTextureB0;


#if defined (POINT_COOKIE)
samplerCUBE _LightTexture0;
#else
sampler2D _LightTexture0;
#endif


#if defined (SHADOWS_DEPTH)
#if defined (SPOT)
UNITY_DECLARE_SHADOWMAP(_ShadowMapTexture);
#if defined (SHADOWS_SOFT)
uniform float4 _ShadowOffsets[4];
#endif
inline half unitySampleShadow (float4 shadowCoord)
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
	shadowVals.x = UNITY_SAMPLE_DEPTH(tex2D( _ShadowMapTexture, coord + _ShadowOffsets[0].xy ));
	shadowVals.y = UNITY_SAMPLE_DEPTH(tex2D( _ShadowMapTexture, coord + _ShadowOffsets[1].xy ));
	shadowVals.z = UNITY_SAMPLE_DEPTH(tex2D( _ShadowMapTexture, coord + _ShadowOffsets[2].xy ));
	shadowVals.w = UNITY_SAMPLE_DEPTH(tex2D( _ShadowMapTexture, coord + _ShadowOffsets[3].xy ));
	half4 shadows = (shadowVals < coord.zzzz) ? _LightShadowData.rrrr : 1.0f;
	#endif
	
	// average-4 PCF
	half shadow = dot( shadows, 0.25f );
	
	#else
	
	// 1-tap shadows
	
	#if defined (SHADOWS_NATIVE)
	half shadow = UNITY_SAMPLE_SHADOW_PROJ(_ShadowMapTexture,shadowCoord);
	shadow = _LightShadowData.r + shadow * (1-_LightShadowData.r);
	#else
	half shadow = UNITY_SAMPLE_DEPTH(tex2Dproj (_ShadowMapTexture, UNITY_PROJ_COORD(shadowCoord))) < (shadowCoord.z / shadowCoord.w) ? _LightShadowData.r : 1.0;
	#endif
	
	#endif
	
	return shadow;
}
#endif //SPOT
#endif //SHADOWS_DEPTH



#if defined (SHADOWS_CUBE)
#if defined (POINT) || defined (POINT_COOKIE)
samplerCUBE _ShadowMapTexture;
inline float SampleCubeDistance (float3 vec)
{
	float4 packDist = texCUBE (_ShadowMapTexture, vec);
	return DecodeFloatRGBA( packDist );
}
inline half unitySampleShadow (float3 vec, float mydist)
{
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
#endif //POINT || POINT_COOKIE
#endif //SHADOWS_CUBE


#if defined (SHADOWS_SCREEN)
sampler2D _ShadowMapTexture;
#endif

float ComputeFadeDistance(float3 wpos, float z)
{
	float sphereDist = distance(wpos, unity_ShadowFadeCenterAndType.xyz);
	return lerp(z, sphereDist, unity_ShadowFadeCenterAndType.w);
}

half ComputeShadow(float3 vec, float fadeDist, float2 uv)
{
	#if defined(SHADOWS_DEPTH) || defined(SHADOWS_SCREEN) || defined(SHADOWS_CUBE)
	float fade = fadeDist * _LightShadowData.z + _LightShadowData.w;
	fade = saturate(fade);
	#endif
	
	#if defined(SPOT)
	#if defined(SHADOWS_DEPTH)
	float4 shadowCoord = mul (unity_World2Shadow[0], float4(vec,1));
	return saturate(unitySampleShadow (shadowCoord) + fade);
	#endif //SHADOWS_DEPTH
	#endif
	
	#if defined (DIRECTIONAL) || defined (DIRECTIONAL_COOKIE)
	#if defined(SHADOWS_SCREEN)
	return saturate(tex2D (_ShadowMapTexture, uv).r + fade);
	#endif
	#endif //DIRECTIONAL || DIRECTIONAL_COOKIE
	
	#if defined (POINT) || defined (POINT_COOKIE)
	#if defined(SHADOWS_CUBE)
	float mydist = length(vec) * _LightPositionRange.w;
	mydist *= 0.97; // bias
	return unitySampleShadow (vec, mydist);	
	#endif //SHADOWS_CUBE
	#endif
	
	return 1.0;
}

half4 CalculateLight (v2f i)
{
	i.ray = i.ray * (_ProjectionParams.z / i.ray.z);
	float2 uv = i.uv.xy / i.uv.w;
	
	half4 nspec = tex2D (_CameraNormalsTexture, uv);
	half3 normal = nspec.rgb * 2 - 1;
	normal = normalize(normal);
	
	float depth = UNITY_SAMPLE_DEPTH(tex2D (_CameraDepthTexture, uv));
	depth = Linear01Depth (depth);
	float4 vpos = float4(i.ray * depth,1);
	float3 wpos = mul (_CameraToWorld, vpos).xyz;

	float fadeDist = ComputeFadeDistance(wpos, vpos.z);
	
	#if defined (SPOT)	
	float3 tolight = _LightPos.xyz - wpos;
	half3 lightDir = normalize (tolight);
	
	float4 uvCookie = mul (_LightMatrix0, float4(wpos,1));
	float atten = tex2Dproj (_LightTexture0, UNITY_PROJ_COORD(uvCookie)).w;
	atten *= uvCookie.w < 0;
	float att = dot(tolight, tolight) * _LightPos.w;
	atten *= tex2D (_LightTextureB0, att.rr).UNITY_ATTEN_CHANNEL;
	
	atten *= ComputeShadow (wpos, fadeDist, uv);
	
	#endif //SPOT
	
	
	
	#if defined (DIRECTIONAL) || defined (DIRECTIONAL_COOKIE)
	half3 lightDir = -_LightDir.xyz;
	float atten = 1.0;
	
	atten *= ComputeShadow (wpos, fadeDist, uv);
	
	#if defined (DIRECTIONAL_COOKIE)
	atten *= tex2D (_LightTexture0, mul(_LightMatrix0, half4(wpos,1)).xy).w;
	#endif //DIRECTIONAL_COOKIE
	#endif //DIRECTIONAL || DIRECTIONAL_COOKIE
	
	
	
	#if defined (POINT) || defined (POINT_COOKIE)
	float3 tolight = wpos - _LightPos.xyz;
	half3 lightDir = -normalize (tolight);
	
	float att = dot(tolight, tolight) * _LightPos.w;
	float atten = tex2D (_LightTextureB0, att.rr).UNITY_ATTEN_CHANNEL;
	
	atten *= ComputeShadow (tolight, fadeDist, uv);
	
	#if defined (POINT_COOKIE)
	atten *= texCUBE(_LightTexture0, mul(_LightMatrix0, half4(wpos,1)).xyz).w;
	#endif //POINT_COOKIE
	
	#endif //POINT || POINT_COOKIE
	
	half diff = max (0, dot (lightDir, normal));
	half3 h = normalize (lightDir - normalize(wpos-_WorldSpaceCameraPos));
	
	float spec = pow (max (0, dot(h,normal)), nspec.a*128.0);
	spec *= saturate(atten);
	
	half4 res;
	res.xyz = _LightColor.rgb * (diff * atten);
	res.w = spec * Luminance (_LightColor.rgb);
	
	float fade = fadeDist * unity_LightmapFade.z + unity_LightmapFade.w;
	res *= saturate(1.0-fade);
	
	return res;
}
ENDCG

/*Pass 1: LDR Pass - Lighting encoded into a subtractive ARGB8 buffer*/
Pass {
	ZWrite Off Fog { Mode Off }
	Blend DstColor Zero
	
CGPROGRAM
#pragma target 3.0
#pragma vertex vert
#pragma fragment frag
#pragma exclude_renderers noprepass
#pragma glsl_no_auto_normalization
#pragma multi_compile_lightpass

fixed4 frag (v2f i) : COLOR
{
	return exp2(-CalculateLight(i));
}

ENDCG
}

/*Pass 2: HDR Pass - Lighting additively blended into floating point buffer*/
Pass {
	ZWrite Off Fog { Mode Off }
	Blend One One
	
CGPROGRAM
#pragma target 3.0
#pragma vertex vert
#pragma fragment frag
#pragma exclude_renderers noprepass
#pragma glsl_no_auto_normalization
#pragma multi_compile_lightpass

fixed4 frag (v2f i) : COLOR
{
	return CalculateLight(i);
}

ENDCG
}

/*Pass 3: Xenon HDR Specular Pass - 10-10-10-2 buffer means we need seperate specular buffer*/
Pass {
	ZWrite Off Fog { Mode Off }
	Blend One One
	
CGPROGRAM
#pragma target 3.0
#pragma vertex vert
#pragma fragment frag
#pragma exclude_renderers noprepass
#pragma glsl_no_auto_normalization
#pragma multi_compile_lightpass

fixed4 frag (v2f i) : COLOR
{
	return CalculateLight(i).argb;
}

ENDCG
}

}
Fallback Off
}
