Shader "Nature/Tree Soft Occlusion Leaves" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Main Texture", 2D) = "white" {  }
		_Cutoff ("Alpha cutoff", Range(0.25,0.9)) = 0.5
		_BaseLight ("Base Light", Range(0, 1)) = 0.35
		_AO ("Amb. Occlusion", Range(0, 10)) = 2.4
		_Occlusion ("Dir Occlusion", Range(0, 20)) = 7.5
		
		// These are here only to provide default values
		_Scale ("Scale", Vector) = (1,1,1,1)
		_SquashAmount ("Squash", Float) = 1
	}
	
	SubShader {
		Tags {
			"Queue" = "Transparent-99"
			"IgnoreProjector"="True"
			"RenderType" = "TreeTransparentCutout"
		}
		Cull Off
		ColorMask RGB
		
		Pass {
			Lighting On
		
			CGPROGRAM
			#pragma vertex leaves
			#pragma fragment frag 
			#pragma glsl_no_auto_normalization
			#include "SH_Vertex.cginc"
			
			sampler2D _MainTex;
			fixed _Cutoff;
			
			fixed4 frag(v2f input) : COLOR
			{
				fixed4 c = tex2D( _MainTex, input.uv.xy);
				c.rgb *= 2.0f * input.color.rgb;
				
				clip (c.a - _Cutoff);
				
				return c;
			}
			ENDCG
		}
		
		Pass {
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }
			
			Fog {Mode Off}
			ZWrite On ZTest LEqual Cull Off
			Offset 1, 1
	
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma glsl_no_auto_normalization
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_shadowcaster
			#include "UnityCG.cginc"
			#include "TerrainEngine.cginc"
			
			struct v2f { 
				V2F_SHADOW_CASTER;
				float2 uv : TEXCOORD1;
			};
			
			struct appdata {
			    float4 vertex : POSITION;
			    fixed4 color : COLOR;
			    float4 texcoord : TEXCOORD0;
			};
			v2f vert( appdata v )
			{
				v2f o;
				TerrainAnimateTree(v.vertex, v.color.w);
				TRANSFER_SHADOW_CASTER(o)
				o.uv = v.texcoord;
				return o;
			}
			
			sampler2D _MainTex;
			fixed _Cutoff;
					
			float4 frag( v2f i ) : COLOR
			{
				fixed4 texcol = tex2D( _MainTex, i.uv );
				clip( texcol.a - _Cutoff );
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG	
		}
	}
	
	SubShader {
		Tags {
			"Queue" = "Transparent-99"
			"IgnoreProjector"="True"
			"RenderType" = "TreeTransparentCutout"
		}
		Cull Off
		ColorMask RGB
		
		Pass {
			CGPROGRAM
			#pragma exclude_renderers shaderonly
			#pragma vertex leaves
			#include "SH_Vertex.cginc"
			ENDCG

			Lighting On
			AlphaTest GEqual [_Cutoff]
			ZWrite On
			
			SetTexture [_MainTex] { combine primary * texture DOUBLE, texture }
		}
	}
	
	SubShader {
		Tags {
			"Queue" = "Transparent-99"
			"IgnoreProjector"="True"
			"RenderType" = "TransparentCutout"
		}
		Cull Off
		ColorMask RGB
		Pass {
			Tags { "LightMode" = "Vertex" }
			AlphaTest GEqual [_Cutoff]
			Lighting On
			Material {
				Diffuse [_Color]
				Ambient [_Color]
			}
			SetTexture [_MainTex] { combine primary * texture DOUBLE, texture }
		}		
	}

	Dependency "BillboardShader" = "Hidden/Nature/Tree Soft Occlusion Leaves Rendertex"
	Fallback Off
}
