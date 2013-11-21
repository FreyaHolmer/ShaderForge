Shader "Fragment Exploration/frag_shader_4_DS_frag" {
	Properties {
		_Color ("Color", Color) = (1.0,1.0,1.0,1.0)
		_SpecColor ("Specular color", Color) = (1.0,1.0,1.0,1.0)
		_SpecStrength ("Specular Strength", Float) = 1.0
		_Shininess ("Specular Power", Float) = 10
		_DiffusePower ("Diffuse Power", Float) = 1
	}
	SubShader {
		Pass {
			Tags { "LightMode"="ForwardBase" }
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			// User vars
			uniform float4 _Color;
			uniform float4 _SpecColor;
			uniform float _SpecStrength;
			uniform float _Shininess;
			uniform float _DiffusePower;
			
			// Unity vars
			uniform float4 _LightColor0;
			
			
			struct VertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};
			
			struct VertexOutput {
				float4 pos : SV_POSITION;
				float4 posWorld : TEXCOORD0;
				float3 normalDir : TEXCOORD1;
			};
			
			VertexOutput vert (VertexInput v) {
				VertexOutput o;
				
				o.posWorld = mul(_Object2World, v.vertex);
				o.normalDir = normalize(mul(float4(v.normal,0), _World2Object).xyz);
				
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				return o;
			}
			
			fixed4 frag(VertexOutput i) : COLOR {
			
			
			  
			
				// Vectors
				float3 normalDirection = normalize(i.normalDir);
				float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
				float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				float atten = 1.0;
				
				// DIFFUSE
				float3 lambert = atten * pow( max( 0.0, dot(normalDirection,lightDirection )),_DiffusePower) * _LightColor0.xyz;
				float3 diffuseReflection = lambert;
				
				// SPEC
				// PHONG:
				float3 specularReflection = lambert * _SpecColor * _SpecStrength * pow(max(0,dot(reflect(-lightDirection, normalDirection),viewDirection)),_Shininess);
				
				
				// FINAL LIGHT
				float3 light = diffuseReflection + specularReflection + UNITY_LIGHTMODEL_AMBIENT;
			
				return fixed4(light*_Color,1);
			}

			ENDCG
		} 
	}
	FallBack "Diffuse"
}