Shader "Fragment Exploration/frag_shader_6_DS_frag_cook-torrance" {
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
			#pragma target 3.0
			
			// User vars
			uniform float4 _Color;
			uniform float4 _SpecColor;
			uniform float _SpecStrength;
			uniform float _Shininess;
			uniform float _DiffusePower;
			
			// Unity vars
			uniform float4 _LightColor0;
			uniform float PI = 3.14159265358979323846264338327;
			uniform float gaussConstant = 100;
			
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
				// Cook-Torrance
				//
				
				float NdotV = dot(normalDirection,viewDirection);
				
				float fresnel = (1+NdotV);
				float3 halfvec = normalize(viewDirection+lightDirection);
				
				float NdotH = dot(normalDirection,halfvec);
				float NdotL = dot(normalDirection,lightDirection);
				float VdotH = dot(viewDirection,halfvec);
				
				float geo = min(min(1, (2*NdotH*NdotV/VdotH)), (2*NdotH*NdotL/VdotH));
				
				float alpha = acos(NdotH);
				float distrib = gaussConstant*exp(-(alpha*alpha)/(_Shininess*_Shininess));
				
				float cookTorr = 1*(fresnel*distrib*geo)/(PI*NdotV);
				
//				
//				float alphaDistr = acos(max(0,dot(normalDirection,halfvec)));
//				float m = _Shininess;
//				float beckmann = (exp(-pow(tan(alphaDistr),2)/(m*m))) / (PI*(m*m)*pow(cos(alphaDistr),4));
//				
//				float geoAtten = min();


				float3 specularReflection = float3(cookTorr);// (d*fresnel*g)/(4*dot(v,n)*dot(n,l));
				//
				
				
				
				
				// BLINN-PHONG:
//				float3 halfVec = normalize(viewDirection+lightDirection);
//				float3 specularReflection = lambert * _SpecColor * _SpecStrength * pow(max(0,dot(halfVec,normalDirection)),_Shininess);
				
				// FINAL LIGHT
				float3 light = specularReflection;
//				float3 light = diffuseReflection + specularReflection + UNITY_LIGHTMODEL_AMBIENT;
			
				return fixed4(light*_Color,1);
			}

			ENDCG
		} 
	}
	FallBack "Diffuse"
}