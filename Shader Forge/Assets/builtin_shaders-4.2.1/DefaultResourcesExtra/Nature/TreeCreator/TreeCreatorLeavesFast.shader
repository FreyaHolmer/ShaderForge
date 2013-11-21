Shader "Nature/Tree Creator Leaves Fast" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_TranslucencyColor ("Translucency Color", Color) = (0.73,0.85,0.41,1) // (187,219,106,255)
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.3
	_TranslucencyViewDependency ("View dependency", Range(0,1)) = 0.7
	_ShadowStrength("Shadow Strength", Range(0,1)) = 1.0
	
	_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
	
	// These are here only to provide default values
	_Scale ("Scale", Vector) = (1,1,1,1)
	_SquashAmount ("Squash", Float) = 1
}

SubShader { 
	Tags {
		"IgnoreProjector"="True"
		"RenderType" = "TreeLeaf"
	}
	LOD 200
		
	Pass {
		Tags { "LightMode" = "ForwardBase" }
		Name "ForwardBase"

	CGPROGRAM
		#include "TreeVertexLit.cginc"

		#pragma vertex VertexLeaf
		#pragma fragment FragmentLeaf
		#pragma exclude_renderers flash
		#pragma multi_compile_fwdbase nolightmap
		
		sampler2D _MainTex;
		float4 _MainTex_ST;

		fixed _Cutoff;
		sampler2D _ShadowMapTexture;

		struct v2f_leaf {
			float4 pos : SV_POSITION;
			fixed4 diffuse : COLOR0;
		#if defined(SHADOWS_SCREEN)
			fixed4 mainLight : COLOR1;
		#endif
			float2 uv : TEXCOORD0;
		#if defined(SHADOWS_SCREEN)
			float4 screenPos : TEXCOORD1;
		#endif
		};

		v2f_leaf VertexLeaf (appdata_full v)
		{
			v2f_leaf o;
			TreeVertLeaf(v);
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

			fixed ao = v.color.a;
			ao += 0.1; ao = saturate(ao * ao * ao); // emphasize AO
						
			fixed3 color = v.color.rgb * _Color.rgb * ao;
			
			float3 worldN = mul ((float3x3)_Object2World, SCALED_NORMAL);

			fixed4 mainLight;
			mainLight.rgb = ShadeTranslucentMainLight (v.vertex, worldN) * color;
			mainLight.a = v.color.a;
			o.diffuse.rgb = ShadeTranslucentLights (v.vertex, worldN) * color;
			o.diffuse.a = 1;
		#if defined(SHADOWS_SCREEN)
			o.mainLight = mainLight;
			o.screenPos = ComputeScreenPos (o.pos);
		#else
			o.diffuse *= 0.5;
			o.diffuse += mainLight;
		#endif			
			o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
			return o;
		}

		fixed4 FragmentLeaf (v2f_leaf IN) : COLOR
		{
			fixed4 albedo = tex2D(_MainTex, IN.uv);
			fixed alpha = albedo.a;
			clip (alpha - _Cutoff);

		#if defined(SHADOWS_SCREEN)
			half4 light = IN.mainLight;
			half atten = tex2Dproj(_ShadowMapTexture, UNITY_PROJ_COORD(IN.screenPos)).r;
			light.rgb *= lerp(2, 2*atten, _ShadowStrength);
			light.rgb += IN.diffuse.rgb;
		#else
			half4 light = IN.diffuse;
			light.rgb *= 2.0;
		#endif

			return fixed4 (albedo.rgb * light, 0.0);
		}

	ENDCG
	}
}

Dependency "OptimizedShader" = "Hidden/Nature/Tree Creator Leaves Fast Optimized"
FallBack "Diffuse"
}
