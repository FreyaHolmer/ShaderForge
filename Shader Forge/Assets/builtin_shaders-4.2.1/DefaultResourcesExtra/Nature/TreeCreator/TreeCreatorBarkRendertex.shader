Shader "Hidden/Nature/Tree Creator Bark Rendertex" {
Properties {
	_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
	_BumpSpecMap ("Normalmap (GA) Spec (R)", 2D) = "bump" {}
	_TranslucencyMap ("Trans (RGB) Gloss(A)", 2D) = "white" {}
	
	// These are here only to provide default values
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	_Scale ("Scale", Vector) = (1,1,1,1)
	_SquashAmount ("Squash", Float) = 1
}

SubShader {  
	Fog { Mode Off }	
	Pass {
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

struct v2f {
	float4 pos : SV_POSITION;
	float2 uv : TEXCOORD0;
	float3 color : TEXCOORD1;
	float2 params[3]: TEXCOORD2;
};

CBUFFER_START(UnityTerrainImposter)
	float3 _TerrainTreeLightDirections[4];
	float4 _TerrainTreeLightColors[4];
CBUFFER_END

v2f vert (appdata_full v) {
	v2f o;
	o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
	o.uv = v.texcoord.xy;
	float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
	
	for (int j = 0; j < 3; j++)
	{
		float3 lightDir = _TerrainTreeLightDirections[j];
	
		half nl = dot (v.normal, lightDir);
		o.params[j].r = max (0, nl);
		
		half3 h = normalize (lightDir + viewDir);
		float nh = max (0, dot (v.normal, h));
		o.params[j].g = nh;
	}
	
	o.color = v.color.a;
	return o;
}

sampler2D _MainTex;
sampler2D _BumpSpecMap;
sampler2D _TranslucencyMap;
fixed4 _SpecColor;

fixed4 frag (v2f i) : COLOR
{
	fixed3 albedo = tex2D (_MainTex, i.uv).rgb * i.color;
	half gloss = tex2D(_TranslucencyMap, i.uv).a;
	half specular = tex2D (_BumpSpecMap, i.uv).r * 128.0;
	
	half3 light = UNITY_LIGHTMODEL_AMBIENT * albedo;
	
	for (int j = 0; j < 3; j++)
	{
		half3 lightColor = _TerrainTreeLightColors[j].rgb;
		
		half nl = i.params[j].r;
		light += albedo * lightColor * nl;
		
		float nh = i.params[j].g;
		float spec = pow (nh, specular) * gloss;
		light += lightColor * _SpecColor.rgb * spec;
	}
	
	fixed4 c;
	c.rgb = light * 2.0;
	c.a = 1.0;
	return c;
}
ENDCG
	}
}

SubShader {
	Pass {		
		Fog { Mode Off }
		
		CGPROGRAM
		#pragma vertex vert
		#pragma exclude_renderers shaderonly
		#include "UnityCG.cginc"

		struct v2f {
			float4 pos : SV_POSITION;
			fixed4 color : COLOR;
			float2 uv : TEXCOORD0;
		};

		CBUFFER_START(UnityTerrainImposter)
			float3 _TerrainTreeLightDirections[4];
			float4 _TerrainTreeLightColors[4];
		CBUFFER_END

		v2f vert (appdata_full v) {
			v2f o;
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			o.uv = v.texcoord.xy;

			float3 light = UNITY_LIGHTMODEL_AMBIENT.rgb;
			
			for (int j = 0; j < 3; j++)
			{
				half3 lightColor = _TerrainTreeLightColors[j].rgb;
				float3 lightDir = _TerrainTreeLightDirections[j];
			
				half nl = dot (v.normal, lightDir);
				light += lightColor * nl;
			}
			
			// lighting * AO
			o.color.rgb = light * v.color.a;
			o.color.a = 1;
			return o;
		}
		ENDCG
		
		SetTexture [_MainTex] {
			Combine texture * primary DOUBLE, primary
		} 
	}
}

FallBack Off
}
