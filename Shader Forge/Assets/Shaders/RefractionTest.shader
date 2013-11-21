// Upgrade NOTE: commented out 'float3 _WorldSpaceCameraPos', a built-in variable
// Upgrade NOTE: commented out 'float4x4 _Object2World', a built-in variable
// Upgrade NOTE: commented out 'float4x4 _World2Object', a built-in variable

// Upgrade NOTE: commented out 'float3 _WorldSpaceCameraPos', a built-in variable
// Upgrade NOTE: commented out 'float4x4 _Object2World', a built-in variable
// Upgrade NOTE: commented out 'float4x4 _World2Object', a built-in variable

// Upgrade NOTE: replaced 'glstate.matrix.mvp' with 'UNITY_MATRIX_MVP'

Shader "REFRACTION TEST" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_RefractStrength ("Refraction Strength", Range (0, 1)) = 1
	_Opacity ("Opacity", Range (0, 1)) = 1
	_Color ("Overall Diffuse Color Filter", Color) = (1,1,1,1)
	_SpecColor ("Specular Material Color", Color) = (1,1,1,1) 
	_Shininess ("Shininess", Float) = 10
}

Category {
	

	// ---- Fragment program cards
	SubShader {
	
		GrabPass { } // To get screen content
		
		Pass {
			Tags { "LightMode" = "PrepassBase" "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
			Blend One Zero
			ColorMask RGBA
			Cull back
			Lighting Off
			ZWrite On
			ZTest LEqual
		
			CGPROGRAM 
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
//			#include "Lighting.cginc"
			
			struct appdata {
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD;
				float3 normal : NORMAL;
			};
			
			struct v2f {
				float4 position : SV_POSITION;
				float4 texcoord : TEXCOORD;
				float3 diffuseColor : TEXCOORD1;
            	float3 specularColor : TEXCOORD2;
            	float4 screenPos : TEXCOORD3;
				float4 col : COLOR0;
			};
			
			uniform float4 _Color; 
			uniform float4 _SpecColor; 
			uniform float _Shininess;
			
			

			uniform float4 _LightColor0; 
			
			
			v2f vert (appdata i){
				v2f o;
				o.texcoord = i.texcoord;
//				o.position = mul(UNITY_MATRIX_MVP, i.vertex);
				o.screenPos = o.position;
				
				 
				
				
				// VERTEX LIGHTING START
				//
				float4x4 modelMatrix = _Object2World;
				float4x4 modelMatrixInverse = _World2Object; 
				float3 normalDirection = normalize(float3(mul(float4(i.normal, 0.0), modelMatrixInverse)));
				float3 viewDirection = normalize(float3(float4(_WorldSpaceCameraPos, 1.0) - mul(modelMatrix, i.vertex)));
				float3 lightDirection;
				float attenuation;
				
				if (0.0 == _WorldSpaceLightPos0.w){ // directional light?
					attenuation = 1.0; // no attenuation
					lightDirection = normalize(float3(_WorldSpaceLightPos0));
				} else { // point or spot light
					float3 vertexToLightSource = float3(_WorldSpaceLightPos0 - mul(modelMatrix, i.vertex));
					float distance = length(vertexToLightSource);
					attenuation = 1.0 / distance; // linear attenuation 
					lightDirection = normalize(vertexToLightSource);
				}
				
				float3 ambientLighting = float3(UNITY_LIGHTMODEL_AMBIENT) * float3(_Color);
				float3 diffuseReflection = attenuation * float3(_LightColor0) * float3(_Color) * max(0.0, dot(normalDirection, lightDirection));
				float3 specularReflection;
				if (dot(normalDirection, lightDirection) < 0.0){ // light source on the wrong side?
					specularReflection = float3(0.0, 0.0, 0.0); // no specular reflection
				} else { // light source on the right side
					specularReflection = attenuation * float3(_LightColor0) * float3(_SpecColor) * pow(max(0.0, dot( reflect(-lightDirection, normalDirection), viewDirection)), _Shininess);
				}
				
				o.diffuseColor = ambientLighting + diffuseReflection;
				o.specularColor = specularReflection;
				o.texcoord = i.texcoord;
				o.position = mul(UNITY_MATRIX_MVP, i.vertex);
					
				// VERTEX LIGHTING END
				
				
				
				
				
				
				
				return o;
			}
			 

			uniform sampler2D _GrabTexture;
			uniform sampler2D _BumpMap;
			uniform sampler2D _MainTex;
			uniform float _RefractStrength;
			uniform float _Opacity;
//			float4 _Time;
 
			
			fixed4 frag (v2f i) : COLOR0 {
			
				fixed3 finalColor;
				float2 screenPos = i.screenPos.xy / i.screenPos.w;
			    screenPos.x = (screenPos.x + 1) * 0.5;
			    screenPos.y = 1-(screenPos.y + 1) * 0.5;
			    
			    float2 panUV = i.texcoord.xy+_Time.xy*-0.1;
			    float3 diff = tex2D(_MainTex,panUV);
				float3 nrm = UnpackNormal(tex2D(_BumpMap, panUV));
				finalColor = tex2D(_GrabTexture,screenPos.xy+nrm.xy*_RefractStrength);
				finalColor = lerp(finalColor,diff,_Opacity);
				
				return fixed4(i.specularColor+i.diffuseColor*finalColor,1);
			}
			
			ENDCG
		}
		
		
		// ADDITIONAL LIGHT SOURCES, VERTEX LIT:
		Pass {    
         Tags { "LightMode" = "PrepassFinal" } 
            // pass for additional light sources
         Blend One One // additive blending 
 
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
 
         // User-specified properties
         uniform sampler2D _MainTex;    
         uniform float4 _Color; 
         uniform float4 _SpecColor; 
         uniform float _Shininess;
 
         // The following built-in uniforms (apart from _LightColor0) 
         // are defined in "UnityCG.cginc", which could be #included 
//         uniform float4 unity_Scale; // w = 1/scale; see _World2Object
         // uniform float3 _WorldSpaceCameraPos;
         // uniform float4x4 _Object2World; // model matrix
         // uniform float4x4 _World2Object; // inverse model matrix 
            // (all but the bottom-right element have to be scaled 
            // with unity_Scale.w if scaling is important) 
//         uniform float4 _WorldSpaceLightPos0; 
            // position or direction of light source
         uniform float4 _LightColor0; 
            // color of light source (from "Lighting.cginc")
 
         struct vertexInput {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float4 texcoord : TEXCOORD0;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 tex : TEXCOORD0;
            float3 diffuseColor : TEXCOORD1;
            float3 specularColor : TEXCOORD2;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 
            float4x4 modelMatrix = _Object2World;
            float4x4 modelMatrixInverse = _World2Object; 
               // multiplication with unity_Scale.w is unnecessary 
               // because we normalize transformed vectors
 
            float3 normalDirection = normalize(float3(
               mul(float4(input.normal, 0.0), modelMatrixInverse)));
            float3 viewDirection = normalize(float3(
               float4(_WorldSpaceCameraPos, 1.0) 
               - mul(modelMatrix, input.vertex)));
            float3 lightDirection;
            float attenuation;
 
            if (0.0 == _WorldSpaceLightPos0.w) // directional light?
            {
               attenuation = 1.0; // no attenuation
               lightDirection = 
                  normalize(float3(_WorldSpaceLightPos0));
            } 
            else // point or spot light
            {
               float3 vertexToLightSource = float3(_WorldSpaceLightPos0
                  - mul(modelMatrix, input.vertex));
               float distance = length(vertexToLightSource);
               attenuation = 1.0 / distance; // linear attenuation 
               lightDirection = normalize(vertexToLightSource);
            }
 
            float3 diffuseReflection = 
               attenuation * float3(_LightColor0) * float3(_Color)
               * max(0.0, dot(normalDirection, lightDirection));
 
            float3 specularReflection;
            if (dot(normalDirection, lightDirection) < 0.0) 
               // light source on the wrong side?
            {
               specularReflection = float3(0.0, 0.0, 0.0); 
                  // no specular reflection
            }
            else // light source on the right side
            {
               specularReflection = attenuation * float3(_LightColor0) 
                  * float3(_SpecColor) * pow(max(0.0, dot(
                  reflect(-lightDirection, normalDirection), 
                  viewDirection)), _Shininess);
            }
 
            output.diffuseColor = diffuseReflection; // no ambient
            output.specularColor = specularReflection;
            output.tex = input.texcoord;
            output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
            return float4(input.specularColor +
               input.diffuseColor * tex2D(_MainTex, float2(input.tex)),
               1.0);
         }
 
         ENDCG
      }
		
		
		
		
	} 	
}
}