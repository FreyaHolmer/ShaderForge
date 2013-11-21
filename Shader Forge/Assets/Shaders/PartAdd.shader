// Upgrade NOTE: replaced 'glstate.matrix.mvp' with 'UNITY_MATRIX_MVP'

Shader "ADD TEST" {
Properties {
	_FogColor ("Fog Color", Color) = (0.26,0.19,0.16,1.0)
}

Category {
	

	// ---- Fragment program cards
	SubShader {
	
		GrabPass { } 
	
		Pass {
			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
			Blend One Zero
			ColorMask RGBA
			Cull front
			Lighting Off
			ZWrite Off
			ZTest LEqual
			Fog { Color (0,0,0,0) }
			BindChannels {
				Bind "Color", color
				Bind "Vertex", vertex
				Bind "TexCoord", texcoord
			} 
			
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			sampler2D OriginalColor;
			struct v2f {
				float4 pos : SV_POSITION;
				float4 col : COLOR0;
			};
			
			struct appdata {
				float4 vertex : POSITION;
			};
			
			v2f vert (appdata v){
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.col = fixed4(v.vertex.rgb,1);
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR0 {
				return i.col;
			}
			
			ENDCG
		}
		
		// PASS #2
		
		GrabPass { "BackfacePositions" } 
//		
		Pass {
			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
			Blend One Zero
			ColorMask RGBA
			Cull back
			Lighting Off
			ZWrite On
			ZTest LEqual
			Fog { Color (0,0,0,0) }
			BindChannels {
				Bind "Color", color
				Bind "Vertex", vertex
				Bind "TexCoord", texcoord
			}
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			struct appdata {
				float4 vertex : POSITION;
			};
			
			struct v2f {
				float4 position : SV_POSITION;
				float4 screenPos : TEXCOORD0;
				float4 col : COLOR0;
//				float2 uvgrab : TEXCOORD;
			};
			
			
			
			v2f vert (appdata i){
				v2f o;
//				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.col = i.vertex;
				o.position = mul(UNITY_MATRIX_MVP, i.vertex);
				o.screenPos = o.position;
//				o.uvgrab.xy = float2(0.5,-0.5) + 0.5*(o.pos.xy/o.pos.w);
//   				o.uvgrab.y = -o.uvgrab.y;
				return o;
			}
			
			sampler2D BackfacePositions;
			sampler2D _GrabTexture;
			float4 _FogColor;
			
			fixed4 frag (v2f i) : COLOR0 {
			
				fixed3 finalColor;
				
				float2 screenPos = i.screenPos.xy / i.screenPos.w;
			    screenPos.x = (screenPos.x + 1) * 0.5;
			    screenPos.y = 1-(screenPos.y + 1) * 0.5;
				
				
				float4 colBehind = tex2D(_GrabTexture,screenPos);
				float4 sPos = tex2D(BackfacePositions,screenPos);
//				return (i.col-sPos);
//				finalColor = i.col;
//				finalColor = sPos.rgb;
				float dist = length(i.col.rgb-sPos.rgb)*_FogColor.a;
				i.col = fixed4(1.0,1.0,1.0,1.0);
//				finalColor = fixed3(dist,dist,dist)*_FogColor.rgb;
//				return fixed4(finalColor,1);
				return fixed4(lerp(colBehind.rgb,_FogColor.rgb,saturate(dist)),1);
//				return tex2D(_GrabTexture,i.uvgrab.xy);
			}
			
			ENDCG
		}
	} 	
}
}