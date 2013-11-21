Shader "Fragment Exploration/BRDF_textured" {
	Properties {
		_specular ("Specular", Range(0, 1)) = 0
		//_roughness ("Specular Power", Range(0, 1)) = 0
		_node_4 ("Normals", 2D) = "bump" {}
		_albedo ("Albedo", 2D) = "white" {}
		_roughness ("Roughness", 2D) = "white" {}
	}
	SubShader {
		Tags {
			"LightMode"="ForwardBase"
		}
		Pass {
			
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma exclude_renderers flash
			uniform float4 _LightColor0;
			uniform float _specular;
		   // uniform float _roughness;
			uniform sampler2D _node_4;
			uniform sampler2D _albedo;
			uniform sampler2D _roughness;


			struct VertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 uv0 : TEXCOORD0;
			};
			struct VertexOutput {
				float4 pos : SV_POSITION;
				float4 uv0 : TEXCOORD0;
				float4 posWorld : TEXCOORD1;
				float3 normalDir : TEXCOORD2;
				float3 tangentDir : TEXCOORD3;
				float3 binormalDir : TEXCOORD4;
			};
			VertexOutput vert (VertexInput v) {
				VertexOutput o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv0 = v.uv0;
				float3 normalDirection = mul(float4(v.normal,0), _World2Object).xyz;
				o.normalDir = normalDirection;
				o.tangentDir = normalize(mul(_Object2World, float4(v.tangent.xyz, 0.0)).xyz);
				o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
				o.posWorld = mul(_Object2World, v.vertex);
				return o;
			}
			fixed4 frag(VertexOutput i) : COLOR {
				float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
				float3 normalLocal = UnpackNormal(tex2D(_node_4,i.uv0.xy)).rgb;
				float3x3 local2WorldTranspose = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
				float3 normalDirection = normalize( mul( normalLocal, local2WorldTranspose ) );
				float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);

				float3 halfDirection = normalize(viewDirection+lightDirection);
				


				// ALBEDO
				float3 albedo = (tex2D(_albedo, i.uv0.xy)+UNITY_LIGHTMODEL_AMBIENT)/3.14159;
				
				// FRESNEL
				float a = pow((1.0-_specular)/(1.0+_specular), 2);
				float fresnel = a+(1-a)*pow(1-dot(halfDirection, viewDirection),5);

				// ROUGHNESS
				float specPow = exp2(10 * tex2D(_roughness, i.uv0.xy) + 1);
				float roughnessContrib = (specPow+2)/(8*3.14159);

				// NORMALS
				float normalContrib = pow(saturate(dot(normalDirection, halfDirection)), specPow);

				float3 lightFinal = (albedo+fresnel*roughnessContrib*normalContrib)*dot(lightDirection,normalDirection)*_LightColor0.xyz;

				//lightFinal = float3(normalContrib);


				return fixed4(lightFinal,1);
			}
			ENDCG
		}
	}
	FallBack "Specular"
}
