Shader "Reflective/VertexLit" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_SpecColor ("Spec Color", Color) = (1,1,1,1)
	_Shininess ("Shininess", Range (0.03, 1)) = 0.7
	_ReflectColor ("Reflection Color", Color) = (1,1,1,0.5)
	_MainTex ("Base (RGB) RefStrength (A)", 2D) = "white" {} 
	_Cube ("Reflection Cubemap", Cube) = "_Skybox" { TexGen CubeReflect }
}

Category {
	Tags { "RenderType"="Opaque" }
	LOD 150

	// ------------------------------------------------------------------
	// Pixel shader cards
	
	SubShader {
	
		// First pass does reflection cubemap
		Pass { 
			Name "BASE"
			Tags {"LightMode" = "Always"}
CGPROGRAM
#pragma exclude_renderers gles xbox360 ps3 gles3
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

struct v2f {
	float4 pos : SV_POSITION;
	float2 uv : TEXCOORD0;
	float3 I : TEXCOORD1;
};

uniform float4 _MainTex_ST;

v2f vert(appdata_tan v)
{
	v2f o;
	o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
	o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);

	// calculate world space reflection vector	
	float3 viewDir = WorldSpaceViewDir( v.vertex );
	float3 worldN = mul((float3x3)_Object2World, v.normal * unity_Scale.w);
	o.I = reflect( -viewDir, worldN );
	
	return o; 
}

uniform sampler2D _MainTex;
uniform samplerCUBE _Cube;
uniform fixed4 _ReflectColor;

fixed4 frag (v2f i) : COLOR
{
	fixed4 texcol = tex2D (_MainTex, i.uv);
	fixed4 reflcol = texCUBE( _Cube, i.I );
	reflcol *= texcol.a;
	return reflcol * _ReflectColor;
} 
ENDCG
		}
		
		// Vertex Lit
		Pass {
			Tags { "LightMode" = "Vertex" }
			Blend One One ZWrite Off Fog { Color (0,0,0,0) }
			Lighting On
			Material {
				Diffuse [_Color]
				Emission [_PPLAmbient]
				Specular [_SpecColor]
				Shininess [_Shininess]
			}
			SeparateSpecular On
CGPROGRAM
#pragma exclude_renderers shaderonly
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest

#include "UnityCG.cginc"

struct v2f {
	float2 uv : TEXCOORD0;
	fixed4 diff : COLOR0;
	fixed4 spec : COLOR1;
};

uniform sampler2D _MainTex : register(s0);
uniform fixed4 _ReflectColor;
uniform fixed4 _SpecColor;

fixed4 frag (v2f i) : COLOR
{
	fixed4 temp = tex2D (_MainTex, i.uv);	
	fixed4 c;
	c.xyz = (temp.xyz * i.diff.xyz + temp.w * i.spec.xyz ) * 2;
	c.w = temp.w * (i.diff.w + Luminance(i.spec.xyz) * _SpecColor.a);
	return c;
} 
ENDCG
			SetTexture[_MainTex] {}
		}
		
		// Lightmapped
		Pass {
			Tags { "LightMode" = "VertexLM" }
			Blend One One ZWrite Off Fog { Color (0,0,0,0) }
			ColorMask RGB
			
			BindChannels {
				Bind "Vertex", vertex
				Bind "normal", normal
				Bind "texcoord1", texcoord0 // lightmap uses 2nd uv
				Bind "texcoord", texcoord1 // main uses 1st uv
			}
			
			SetTexture [unity_Lightmap] {
				matrix [unity_LightmapMatrix]
				constantColor [_Color]
				combine texture * constant
			}
			SetTexture [_MainTex] {
				combine texture * previous DOUBLE, texture * primary
			}
		}
		
		// Lightmapped, encoded as RGBM
		Pass {
			Tags { "LightMode" = "VertexLMRGBM" }
			Blend One One ZWrite Off Fog { Color (0,0,0,0) }
			ColorMask RGB
			
			BindChannels {
				Bind "Vertex", vertex
				Bind "normal", normal
				Bind "texcoord1", texcoord0 // lightmap uses 2nd uv
				Bind "texcoord1", texcoord1 // unused
				Bind "texcoord", texcoord2 // main uses 1st uv
			}
			
			SetTexture [unity_Lightmap] {
				matrix [unity_LightmapMatrix]
				combine texture * texture alpha DOUBLE
			}
			SetTexture [unity_Lightmap] {
				constantColor [_Color]
				combine previous * constant
			}
			SetTexture [_MainTex] {
				combine texture * previous QUAD, texture * primary
			}
		}
	}
	
	// ------------------------------------------------------------------
	// Old cards
	
	SubShader {
		Pass { 
			Name "BASE"
			Tags { "LightMode" = "Vertex" }
			Material {
				Diffuse [_Color]
				Ambient (1,1,1,1)
				Shininess [_Shininess]
				Specular [_SpecColor]
			}
			Lighting On
			SeparateSpecular on
			SetTexture [_MainTex] {
				combine texture * primary DOUBLE, texture * primary
			}
			SetTexture [_Cube] {
				combine texture * previous alpha + previous, previous
			}
		}
	}
}

// Fallback for cards that don't do cubemapping
FallBack "VertexLit"
}
