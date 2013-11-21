
Shader "Hidden/Internal-Flare" 
{
	SubShader {

		Tags {"RenderType"="Overlay"}
		ZWrite Off ZTest Always
		Fog {Mode Off}
		Cull Off
		Blend One One
		ColorMask RGB

		Pass {	
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"

			sampler2D _FlareTexture;
			
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

			float4 _FlareTexture_ST;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord, _FlareTexture);
				return o;
			}

			fixed4 frag (v2f i) : COLOR
			{
				return tex2D(_FlareTexture, i.texcoord) * i.color;
			}
			ENDCG 
		}
	} 	

	SubShader {
		Tags {"RenderType"="Overlay"}
		ZWrite Off ZTest Always
		Fog {Mode Off}
		Cull Off
		Blend One One
		ColorMask RGB
		BindChannels { 
			Bind "vertex", vertex Bind "color", color Bind "texcoord", texcoord0 
		}
		Pass { 
			SetTexture [_FlareTexture] { combine previous * texture } 
		}
	}
}
