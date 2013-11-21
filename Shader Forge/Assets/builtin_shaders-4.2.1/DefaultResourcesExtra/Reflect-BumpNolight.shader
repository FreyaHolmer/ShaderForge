Shader "Reflective/Bumped Unlit" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_ReflectColor ("Reflection Color", Color) = (1,1,1,0.5)
	_MainTex ("Base (RGB), RefStrength (A)", 2D) = "white" {}
	_Cube ("Reflection Cubemap", Cube) = "" { TexGen CubeReflect }
	_BumpMap ("Normalmap", 2D) = "bump" {}
}

Category {
	Tags { "RenderType"="Opaque" }
	LOD 250
	
	// ------------------------------------------------------------------
	// Shaders

	SubShader {
		// Always drawn reflective pass
		Pass {
			Name "BASE"
			Tags {"LightMode" = "Always"}
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest

#include "UnityCG.cginc"

struct v2f {
	float4 pos : SV_POSITION;
	float2	uv		: TEXCOORD0;
	float2	uv2		: TEXCOORD1;
	float3	I		: TEXCOORD2;
	float3	TtoW0 	: TEXCOORD3;
	float3	TtoW1	: TEXCOORD4;
	float3	TtoW2	: TEXCOORD5;
};

uniform float4 _MainTex_ST, _BumpMap_ST;

v2f vert(appdata_tan v)
{
	v2f o;
	o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
	o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
	o.uv2 = TRANSFORM_TEX(v.texcoord,_BumpMap);
	
	o.I = -WorldSpaceViewDir( v.vertex );
	
	TANGENT_SPACE_ROTATION;
	o.TtoW0 = mul(rotation, _Object2World[0].xyz * unity_Scale.w);
	o.TtoW1 = mul(rotation, _Object2World[1].xyz * unity_Scale.w);
	o.TtoW2 = mul(rotation, _Object2World[2].xyz * unity_Scale.w);
	
	return o; 
}

uniform sampler2D _BumpMap;
uniform sampler2D _MainTex;
uniform samplerCUBE _Cube;
uniform fixed4 _ReflectColor;
uniform fixed4 _Color;

fixed4 frag (v2f i) : COLOR
{
	// Sample and expand the normal map texture	
	fixed3 normal = UnpackNormal(tex2D(_BumpMap, i.uv2));
	
	fixed4 texcol = tex2D(_MainTex,i.uv);
	
	// transform normal to world space
	half3 wn;
	wn.x = dot(i.TtoW0, normal);
	wn.y = dot(i.TtoW1, normal);
	wn.z = dot(i.TtoW2, normal);
	
	// calculate reflection vector in world space
	half3 r = reflect(i.I, wn);
	
	fixed4 c = UNITY_LIGHTMODEL_AMBIENT * texcol;
	c.rgb *= 2;
	fixed4 reflcolor = texCUBE(_Cube, r) * _ReflectColor * texcol.a;
	return c + reflcolor;
}
ENDCG  
		} 
	}
	
	// ------------------------------------------------------------------
	//  No vertex or fragment programs
	
	SubShader {
		Pass { 
			Tags {"LightMode" = "Always"}
			Name "BASE"
			BindChannels {
				Bind "Vertex", vertex
				Bind "Normal", normal
			}
			SetTexture [_Cube] {
				constantColor [_ReflectColor]
				combine texture * constant
			}
		}
	}
}
	
FallBack "VertexLit", 1

}
