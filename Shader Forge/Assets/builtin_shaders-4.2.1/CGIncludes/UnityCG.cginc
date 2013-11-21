#ifndef UNITY_CG_INCLUDED
#define UNITY_CG_INCLUDED

#include "UnityShaderVariables.cginc"




#if SHADER_API_FLASH
uniform float4 unity_NPOTScale;
#endif
#if defined(SHADER_API_PS3)
#	define UNITY_SAMPLE_DEPTH(value) (dot((value).wxy, float3(0.996093809371817670572857294849, 0.0038909914428586627756752238080039, 1.5199185323666651467481343000015e-5)))
#elif defined(SHADER_API_FLASH)
#	define UNITY_SAMPLE_DEPTH(value) (DecodeFloatRGBA(value))
#else
#	define UNITY_SAMPLE_DEPTH(value) (value).r
#endif

uniform fixed4 unity_ColorSpaceGrey;

// -------------------------------------------------------------------
//  helper functions and macros used in many standard shaders


#if defined (DIRECTIONAL) || defined (DIRECTIONAL_COOKIE) || defined (POINT) || defined (SPOT) || defined (POINT_NOATT) || defined (POINT_COOKIE)
#define USING_LIGHT_MULTI_COMPILE
#endif

#define SCALED_NORMAL (v.normal * unity_Scale.w)

struct appdata_base {
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    float4 texcoord : TEXCOORD0;
};

struct appdata_tan {
    float4 vertex : POSITION;
    float4 tangent : TANGENT;
    float3 normal : NORMAL;
    float4 texcoord : TEXCOORD0;
};

struct appdata_full {
    float4 vertex : POSITION;
    float4 tangent : TANGENT;
    float3 normal : NORMAL;
    float4 texcoord : TEXCOORD0;
    float4 texcoord1 : TEXCOORD1;
    fixed4 color : COLOR;
#if defined(SHADER_API_XBOX360)
	half4 texcoord2 : TEXCOORD2;
	half4 texcoord3 : TEXCOORD3;
	half4 texcoord4 : TEXCOORD4;
	half4 texcoord5 : TEXCOORD5;
#endif
};

// Computes world space light direction
inline float3 WorldSpaceLightDir( in float4 v )
{
	float3 worldPos = mul(_Object2World, v).xyz;
	#ifndef USING_LIGHT_MULTI_COMPILE
		return _WorldSpaceLightPos0.xyz - worldPos * _WorldSpaceLightPos0.w;
	#else
		#ifndef USING_DIRECTIONAL_LIGHT
		return _WorldSpaceLightPos0.xyz - worldPos;
		#else
		return _WorldSpaceLightPos0.xyz;
		#endif
	#endif
}

// Computes object space light direction
inline float3 ObjSpaceLightDir( in float4 v )
{
	float3 objSpaceLightPos = mul(_World2Object, _WorldSpaceLightPos0).xyz;
	#ifndef USING_LIGHT_MULTI_COMPILE
		return objSpaceLightPos.xyz - v.xyz * _WorldSpaceLightPos0.w;
	#else
		#ifndef USING_DIRECTIONAL_LIGHT
		return objSpaceLightPos.xyz * unity_Scale.w - v.xyz;
		#else
		return objSpaceLightPos.xyz;
		#endif
	#endif
}

// Computes world space view direction
inline float3 WorldSpaceViewDir( in float4 v )
{
	return _WorldSpaceCameraPos.xyz - mul(_Object2World, v).xyz;
}

// Computes object space view direction
inline float3 ObjSpaceViewDir( in float4 v )
{
	float3 objSpaceCameraPos = mul(_World2Object, float4(_WorldSpaceCameraPos.xyz, 1)).xyz * unity_Scale.w;
	return objSpaceCameraPos - v.xyz;
}

// Declares 3x3 matrix 'rotation', filled with tangent space basis
#define TANGENT_SPACE_ROTATION \
	float3 binormal = cross( v.normal, v.tangent.xyz ) * v.tangent.w; \
	float3x3 rotation = float3x3( v.tangent.xyz, binormal, v.normal )




float3 Shade4PointLights (
	float4 lightPosX, float4 lightPosY, float4 lightPosZ,
	float3 lightColor0, float3 lightColor1, float3 lightColor2, float3 lightColor3,
	float4 lightAttenSq,
	float3 pos, float3 normal)
{
	// to light vectors
	float4 toLightX = lightPosX - pos.x;
	float4 toLightY = lightPosY - pos.y;
	float4 toLightZ = lightPosZ - pos.z;
	// squared lengths
	float4 lengthSq = 0;
	lengthSq += toLightX * toLightX;
	lengthSq += toLightY * toLightY;
	lengthSq += toLightZ * toLightZ;
	// NdotL
	float4 ndotl = 0;
	ndotl += toLightX * normal.x;
	ndotl += toLightY * normal.y;
	ndotl += toLightZ * normal.z;
	// correct NdotL
	float4 corr = rsqrt(lengthSq);
	ndotl = max (float4(0,0,0,0), ndotl * corr);
	// attenuation
	float4 atten = 1.0 / (1.0 + lengthSq * lightAttenSq);
	float4 diff = ndotl * atten;
	// final color
	float3 col = 0;
	col += lightColor0 * diff.x;
	col += lightColor1 * diff.y;
	col += lightColor2 * diff.z;
	col += lightColor3 * diff.w;
	return col;
}


float3 ShadeVertexLights (float4 vertex, float3 normal)
{
	float3 viewpos = mul (UNITY_MATRIX_MV, vertex).xyz;
	float3 viewN = mul ((float3x3)UNITY_MATRIX_IT_MV, normal);
	float3 lightColor = UNITY_LIGHTMODEL_AMBIENT.xyz;
	for (int i = 0; i < 4; i++) {
		float3 toLight = unity_LightPosition[i].xyz - viewpos.xyz * unity_LightPosition[i].w;
		float lengthSq = dot(toLight, toLight);
		float atten = 1.0 / (1.0 + lengthSq * unity_LightAtten[i].z);
		float diff = max (0, dot (viewN, normalize(toLight)));
		lightColor += unity_LightColor[i].rgb * (diff * atten);
	}
	return lightColor;
}


// normal should be normalized, w=1.0
half3 ShadeSH9 (half4 normal)
{
	half3 x1, x2, x3;
	
	// Linear + constant polynomial terms
	x1.r = dot(unity_SHAr,normal);
	x1.g = dot(unity_SHAg,normal);
	x1.b = dot(unity_SHAb,normal);
	
	// 4 of the quadratic polynomials
	half4 vB = normal.xyzz * normal.yzzx;
	x2.r = dot(unity_SHBr,vB);
	x2.g = dot(unity_SHBg,vB);
	x2.b = dot(unity_SHBb,vB);
	
	// Final quadratic polynomial
	float vC = normal.x*normal.x - normal.y*normal.y;
	x3 = unity_SHC.rgb * vC;
    return x1 + x2 + x3;
} 


// Transforms 2D UV by scale/bias property
#define TRANSFORM_TEX(tex,name) (tex.xy * name##_ST.xy + name##_ST.zw)
// Transforms 4D UV by a texture matrix (use only if you know exactly which matrix you need)
#define TRANSFORM_UV(idx) mul (UNITY_MATRIX_TEXTURE##idx, v.texcoord).xy



struct v2f_vertex_lit {
	float2 uv	: TEXCOORD0;
	fixed4 diff	: COLOR0;
	fixed4 spec	: COLOR1;
};  

inline fixed4 VertexLight( v2f_vertex_lit i, sampler2D mainTex )
{
	fixed4 texcol = tex2D( mainTex, i.uv );
	fixed4 c;
	c.xyz = ( texcol.xyz * i.diff.xyz + i.spec.xyz * texcol.a ) * 2;
	c.w = texcol.w * i.diff.w;
	return c;
}


// Calculates UV offset for parallax bump mapping
inline float2 ParallaxOffset( half h, half height, half3 viewDir )
{
	h = h * height - height/2.0;
	float3 v = normalize(viewDir);
	v.z += 0.42;
	return h * (v.xy / v.z);
}


// Converts color to luminance (grayscale)
inline fixed Luminance( fixed3 c )
{
	return dot( c, fixed3(0.22, 0.707, 0.071) );
}

// Decodes lightmaps:
// - doubleLDR encoded on GLES
// - RGBM encoded with range [0;8] on other platforms using surface shaders
inline fixed3 DecodeLightmap( fixed4 color )
{
#if (defined(SHADER_API_GLES) || defined(SHADER_API_GLES3)) && defined(SHADER_API_MOBILE)
	return 2.0 * color.rgb;
#else
	// potentially faster to do the scalar multiplication
	// in parenthesis for scalar GPUs
	return (8.0 * color.a) * color.rgb;
#endif
}


// Helpers used in image effects. Most image effects use the same
// minimal vertex shader (vert_img).

struct appdata_img {
    float4 vertex : POSITION;
    half2 texcoord : TEXCOORD0;
};
struct v2f_img {
	float4 pos : SV_POSITION;
	half2 uv : TEXCOORD0;
};

float2 MultiplyUV (float4x4 mat, float2 inUV) {
	float4 temp = float4 (inUV.x, inUV.y, 0, 0);
	temp = mul (mat, temp);
	return temp.xy;
}

v2f_img vert_img( appdata_img v )
{
	v2f_img o;
	o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
	o.uv = MultiplyUV( UNITY_MATRIX_TEXTURE0, v.texcoord );
	return o;
}


// Encoding/decoding [0..1) floats into 8 bit/channel RGBA. Note that 1.0 will not be encoded properly.
inline float4 EncodeFloatRGBA( float v )
{
	float4 kEncodeMul = float4(1.0, 255.0, 65025.0, 160581375.0);
	float kEncodeBit = 1.0/255.0;
	float4 enc = kEncodeMul * v;
	enc = frac (enc);
	enc -= enc.yzww * kEncodeBit;
	return enc;
}
inline float DecodeFloatRGBA( float4 enc )
{
	float4 kDecodeDot = float4(1.0, 1/255.0, 1/65025.0, 1/160581375.0);
	return dot( enc, kDecodeDot );
}

// Encoding/decoding [0..1) floats into 8 bit/channel RG. Note that 1.0 will not be encoded properly.
inline float2 EncodeFloatRG( float v )
{
	float2 kEncodeMul = float2(1.0, 255.0);
	float kEncodeBit = 1.0/255.0;
	float2 enc = kEncodeMul * v;
	enc = frac (enc);
	enc.x -= enc.y * kEncodeBit;
	return enc;
}
inline float DecodeFloatRG( float2 enc )
{
	float2 kDecodeDot = float2(1.0, 1/255.0);
	return dot( enc, kDecodeDot );
}


// Encoding/decoding view space normals into 2D 0..1 vector
inline float2 EncodeViewNormalStereo( float3 n )
{
	float kScale = 1.7777;
	float2 enc;
	enc = n.xy / (n.z+1);
	enc /= kScale;
	enc = enc*0.5+0.5;
	return enc;
}
inline float3 DecodeViewNormalStereo( float4 enc4 )
{
	float kScale = 1.7777;
	float3 nn = enc4.xyz*float3(2*kScale,2*kScale,0) + float3(-kScale,-kScale,1);
	float g = 2.0 / dot(nn.xyz,nn.xyz);
	float3 n;
	n.xy = g*nn.xy;
	n.z = g-1;
	return n;
}

inline float4 EncodeDepthNormal( float depth, float3 normal )
{
	float4 enc;
	enc.xy = EncodeViewNormalStereo (normal);
	enc.zw = EncodeFloatRG (depth);
	return enc;
}

inline void DecodeDepthNormal( float4 enc, out float depth, out float3 normal )
{
	depth = DecodeFloatRG (enc.zw);
	normal = DecodeViewNormalStereo (enc);
}

inline fixed3 UnpackNormalDXT5nm (fixed4 packednormal)
{
	fixed3 normal;
	normal.xy = packednormal.wy * 2 - 1;
#if defined(SHADER_API_FLASH)
	// Flash does not have efficient saturate(), and dot() seems to require an extra register.
	normal.z = sqrt(1 - normal.x*normal.x - normal.y*normal.y);
#else
	normal.z = sqrt(1 - saturate(dot(normal.xy, normal.xy)));
#endif
	return normal;
}

inline fixed3 UnpackNormal(fixed4 packednormal)
{
#if (defined(SHADER_API_GLES) || defined(SHADER_API_GLES3)) && defined(SHADER_API_MOBILE)
	return packednormal.xyz * 2 - 1;
#else
	return UnpackNormalDXT5nm(packednormal);
#endif
}


// Z buffer to linear 0..1 depth (0 at eye, 1 at far plane)
inline float Linear01Depth( float z )
{
	return 1.0 / (_ZBufferParams.x * z + _ZBufferParams.y);
}
// Z buffer to linear depth
inline float LinearEyeDepth( float z )
{
	return 1.0 / (_ZBufferParams.z * z + _ZBufferParams.w);
}


// Depth render texture helpers
#if defined(UNITY_MIGHT_NOT_HAVE_DEPTH_TEXTURE)
	#define UNITY_TRANSFER_DEPTH(oo) oo = o.pos.zw
	#if SHADER_API_FLASH
	#define UNITY_OUTPUT_DEPTH(i) return EncodeFloatRGBA(i.x/i.y)
	#else
	#define UNITY_OUTPUT_DEPTH(i) return i.x/i.y
	#endif
#else
	#define UNITY_TRANSFER_DEPTH(oo) 
	#define UNITY_OUTPUT_DEPTH(i) return 0
#endif
#define DECODE_EYEDEPTH(i) LinearEyeDepth(i)
#define COMPUTE_EYEDEPTH(o) o = -mul( UNITY_MATRIX_MV, v.vertex ).z
#define COMPUTE_DEPTH_01 -(mul( UNITY_MATRIX_MV, v.vertex ).z * _ProjectionParams.w)
#define COMPUTE_VIEW_NORMAL mul((float3x3)UNITY_MATRIX_IT_MV, v.normal)


// Projected screen position helpers
#define V2F_SCREEN_TYPE float4
inline float4 ComputeScreenPos (float4 pos) {
	float4 o = pos * 0.5f;
	#if defined(UNITY_HALF_TEXEL_OFFSET)
	o.xy = float2(o.x, o.y*_ProjectionParams.x) + o.w * _ScreenParams.zw;
	#else
	o.xy = float2(o.x, o.y*_ProjectionParams.x) + o.w;
	#endif
	
	#if defined(SHADER_API_FLASH)
	o.xy *= unity_NPOTScale.xy;
	#endif
	
	o.zw = pos.zw;
	return o;
}

inline float4 ComputeGrabScreenPos (float4 pos) {
	#if UNITY_UV_STARTS_AT_TOP
	float scale = -1.0;
	#else
	float scale = 1.0;
	#endif
	float4 o = pos * 0.5f;
	o.xy = float2(o.x, o.y*scale) + o.w;
	o.zw = pos.zw;
	return o;
}


inline float2 TransformViewToProjection (float2 v) {
	return float2(v.x*UNITY_MATRIX_P[0][0], v.y*UNITY_MATRIX_P[1][1]);
}

inline float3 TransformViewToProjection (float3 v) {
	return float3(v.x*UNITY_MATRIX_P[0][0], v.y*UNITY_MATRIX_P[1][1], v.z*UNITY_MATRIX_P[2][2]);
}




// Shadow caster pass helpers


#ifdef SHADOWS_CUBE
	#define V2F_SHADOW_CASTER float4 pos : SV_POSITION; float3 vec : TEXCOORD0
	#define TRANSFER_SHADOW_CASTER(o) o.vec = mul( _Object2World, v.vertex ).xyz - _LightPositionRange.xyz; o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
	#define SHADOW_CASTER_FRAGMENT(i) return EncodeFloatRGBA( min(length(i.vec) * _LightPositionRange.w, 0.999) );
#else
	#if defined(UNITY_MIGHT_NOT_HAVE_DEPTH_TEXTURE)
	#define V2F_SHADOW_CASTER float4 pos : SV_POSITION; float4 hpos : TEXCOORD0
	#define TRANSFER_SHADOW_CASTER(o) o.pos = mul(UNITY_MATRIX_MVP, v.vertex); o.pos.z += unity_LightShadowBias.x; \
	float clamped = max(o.pos.z, o.pos.w*UNITY_NEAR_CLIP_VALUE); o.pos.z = lerp(o.pos.z, clamped, unity_LightShadowBias.y); o.hpos = o.pos;
	#else
	#define V2F_SHADOW_CASTER float4 pos : SV_POSITION
	#define TRANSFER_SHADOW_CASTER(o) o.pos = mul(UNITY_MATRIX_MVP, v.vertex); o.pos.z += unity_LightShadowBias.x; \
	float clamped = max(o.pos.z, o.pos.w*UNITY_NEAR_CLIP_VALUE); o.pos.z = lerp(o.pos.z, clamped, unity_LightShadowBias.y);
	#endif
	#define SHADOW_CASTER_FRAGMENT(i) UNITY_OUTPUT_DEPTH(i.hpos.zw);
#endif

// Shadow collector pass helpers
#ifdef SHADOW_COLLECTOR_PASS


#if !defined(SHADOWMAPSAMPLER_DEFINED)
UNITY_DECLARE_SHADOWMAP(_ShadowMapTexture);
#endif


#define V2F_SHADOW_COLLECTOR float4 pos : SV_POSITION; float3 _ShadowCoord0 : TEXCOORD0; float3 _ShadowCoord1 : TEXCOORD1; float3 _ShadowCoord2 : TEXCOORD2; float3 _ShadowCoord3 : TEXCOORD3; float4 _WorldPosViewZ : TEXCOORD4
#define TRANSFER_SHADOW_COLLECTOR(o)	\
	o.pos = mul(UNITY_MATRIX_MVP, v.vertex); \
	float4 wpos = mul(_Object2World, v.vertex); \
	o._WorldPosViewZ.xyz = wpos; \
	o._WorldPosViewZ.w = -mul( UNITY_MATRIX_MV, v.vertex ).z; \
	o._ShadowCoord0 = mul(unity_World2Shadow[0], wpos).xyz; \
	o._ShadowCoord1 = mul(unity_World2Shadow[1], wpos).xyz; \
	o._ShadowCoord2 = mul(unity_World2Shadow[2], wpos).xyz; \
	o._ShadowCoord3 = mul(unity_World2Shadow[3], wpos).xyz;


#if defined (SHADOWS_NATIVE)
	#define SAMPLE_SHADOW_COLLECTOR_SHADOW(coord) \
	half shadow = UNITY_SAMPLE_SHADOW(_ShadowMapTexture,coord); \
	shadow = _LightShadowData.r + shadow * (1-_LightShadowData.r);
#else
	#define SAMPLE_SHADOW_COLLECTOR_SHADOW(coord) \
	float shadow = UNITY_SAMPLE_DEPTH(tex2D( _ShadowMapTexture, coord.xy )) < coord.z ? _LightShadowData.r : 1.0;
#endif

#define COMPUTE_SHADOW_COLLECTOR_SHADOW(i, weights, shadowFade) \
	float4 coord = float4(i._ShadowCoord0 * weights[0] + i._ShadowCoord1 * weights[1] + i._ShadowCoord2 * weights[2] + i._ShadowCoord3 * weights[3], 1); \
	SAMPLE_SHADOW_COLLECTOR_SHADOW(coord) \
	float4 res; \
	res.x = saturate(shadow + shadowFade); \
	res.y = 1.0; \
	res.zw = EncodeFloatRG (1 - i._WorldPosViewZ.w * _ProjectionParams.w); \
	return res;	

#if defined (SHADOWS_SPLIT_SPHERES)
#define SHADOW_COLLECTOR_FRAGMENT(i) \
	float3 fromCenter0 = i._WorldPosViewZ.xyz - unity_ShadowSplitSpheres[0].xyz; \
	float3 fromCenter1 = i._WorldPosViewZ.xyz - unity_ShadowSplitSpheres[1].xyz; \
	float3 fromCenter2 = i._WorldPosViewZ.xyz - unity_ShadowSplitSpheres[2].xyz; \
	float3 fromCenter3 = i._WorldPosViewZ.xyz - unity_ShadowSplitSpheres[3].xyz; \
	float4 distances2 = float4(dot(fromCenter0,fromCenter0), dot(fromCenter1,fromCenter1), dot(fromCenter2,fromCenter2), dot(fromCenter3,fromCenter3)); \
	float4 cascadeWeights = float4(distances2 < unity_ShadowSplitSqRadii); \
	cascadeWeights.yzw = saturate(cascadeWeights.yzw - cascadeWeights.xyz); \
	float sphereDist = distance(i._WorldPosViewZ.xyz, unity_ShadowFadeCenterAndType.xyz); \
	float shadowFade = saturate(sphereDist * _LightShadowData.z + _LightShadowData.w); \
	COMPUTE_SHADOW_COLLECTOR_SHADOW(i, cascadeWeights, shadowFade)
#else
#define SHADOW_COLLECTOR_FRAGMENT(i) \
	float4 viewZ = i._WorldPosViewZ.w; \
	float4 zNear = float4( viewZ >= _LightSplitsNear ); \
	float4 zFar = float4( viewZ < _LightSplitsFar ); \
	float4 cascadeWeights = zNear * zFar; \
	float shadowFade = saturate(i._WorldPosViewZ.w * _LightShadowData.z + _LightShadowData.w); \
	COMPUTE_SHADOW_COLLECTOR_SHADOW(i, cascadeWeights, shadowFade)
#endif
	
#endif

#endif
