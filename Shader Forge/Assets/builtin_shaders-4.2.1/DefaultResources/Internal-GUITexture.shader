
Shader "Hidden/Internal-GUITexture" 
{
	Properties { _MainTex ("Texture", any) = "" {} } 

	SubShader {

		Tags { "ForceSupported" = "True" "RenderType"="Overlay" } 
		
		Lighting Off 
		Blend SrcAlpha OneMinusSrcAlpha 
		Cull Off 
		ZWrite Off 
		Fog { Mode Off } 
		ZTest Always 
		
		Pass {	
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			sampler2D _MainTex;

			uniform float4 _MainTex_ST;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				return o;
			}

			fixed4 frag (v2f i) : COLOR
			{
				return 2.0f * tex2D(_MainTex, i.texcoord) * i.color;
			}
			ENDCG 
		}
	} 	
 
	
	SubShader { 

		Tags { "ForceSupported" = "True" "RenderType"="Overlay" } 

		Lighting Off 
		Blend SrcAlpha OneMinusSrcAlpha 
		Cull Off 
		ZWrite Off 
		Fog { Mode Off } 
		ZTest Always 
		
		BindChannels { 
			Bind "vertex", vertex 
			Bind "color", color 
			Bind "TexCoord", texcoord 
		} 
		
		Pass { 
			SetTexture [_MainTex] {
				combine primary * texture DOUBLE, primary * texture DOUBLE
			} 
		} 
	} 

	Fallback off 
}
