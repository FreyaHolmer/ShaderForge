Shader "Hidden/Internal-Halo" { 
	SubShader {
		Tags {"RenderType"="Overlay"}
		ZWrite off Cull off	// NOTE: 'Cull off' is important as the halo meshes flip handedness each time... BUG: #1220
		Fog { Color (0,0,0,0) } 
		Blend OneMinusDstColor One
		AlphaTest Greater 0
		ColorMask RGB
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			sampler2D _HaloFalloff;
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
			float4 _HaloFalloff_ST;
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_HaloFalloff);
				return o;
			}
			fixed4 frag (v2f i) : COLOR
			{
				half a = tex2D(_HaloFalloff, i.texcoord).a;
				return half4 (i.color.rgb * a, a);
			}
			ENDCG  
		}  
	}  
	SubShader {  
		Tags {"RenderType"="Overlay"}  
		ZWrite off Cull off  	// NOTE: 'Cull off' is important as the halo meshes flip handedness each time... BUG: #1220
		Fog { Color (0,0,0,0) }   
		Blend OneMinusDstColor One  
		AlphaTest Greater 0  
		ColorMask RGB  
		Pass {
			BindChannels {
				Bind "Vertex", vertex
				Bind "Color", color
				// caveat: because _HaloFalloff is a global texture prop,
				// can't bind to texcoord; need explicit texcoord0
				Bind "TexCoord", texcoord0
			}
			SetTexture [_HaloFalloff] { combine primary * texture alpha, texture alpha }
		}
	}
}
