Shader "Fragment Exploration/frag_shader_2_D_vert" {
	Properties {
		_Color ("Color", Color) = (1.0,1.0,1.0,1.0)
		_SpecColor ("Specular color", Color) = (1.0,1.0,1.0,1.0)
		_SpecStrength ("Specular Strength", Float) = 1.0
		_Shininess ("Gloss", Float) = 10
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
			
			// Unity vars
			uniform float4 _LightColor0;
			
			
			struct VertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};
			
			struct VertexOutput {
				float4 pos : SV_POSITION;
				float4 col : COLOR;
			};
			
			VertexOutput vert (VertexInput v) {
				VertexOutput o;
				  
				// Vectors
				float3 normalDirection = normalize(mul(float4(v.normal,0), _World2Object).xyz);
				float3 viewDirection = normalize(float3( float4(_WorldSpaceCameraPos.xyz,1) - mul(_Object2World, v.vertex).xyz));
				float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				float atten = 1.0;
				
				// DIFFUSE
				float lambert = atten * max( 0.0, dot(normalDirection,lightDirection ));
				float3 diffuseReflection = lambert * _LightColor0.xyz;
				
				// FINAL LIGHT
				float3 light = diffuseReflection + UNITY_LIGHTMODEL_AMBIENT;
				
				
				o.col = fixed4(light*_Color,1);
				
				
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				return o;
			}
			
			fixed4 frag(VertexOutput i) : COLOR {
				return i.col;
			}

			ENDCG
		} 
	}
	FallBack "Diffuse"
}