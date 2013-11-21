Shader "Fragment Exploration/sss_3" {
	Properties {
		node_1_smp ("", 2D) = "white" {}
		node_2_smp ("", 2D) = "white" {}
		_Color ("Color", Color) = (0.5, 0.5, 0.5, 1)
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
			float3 _Color;
			uniform float4 _LightColor0;
			uniform sampler2D node_1_smp;
			uniform sampler2D node_2_smp;
			struct VertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 uv0 : TEXCOORD0;
			};
			struct VertexOutput {
				float4 pos : SV_POSITION;
				float4 col : COLOR;
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
				
				o.tangentDir = mul(_Object2World, float4(v.tangent.xyz, 0.0));
				o.tangentDir = normalize(o.tangentDir);
				o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
				o.posWorld = mul(_Object2World, v.vertex);
				o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
				
				return o;
			}
			fixed4 frag(VertexOutput i) : COLOR {
			
				float3 normalLocal = UnpackNormal(tex2D(node_2_smp,i.uv0.xy)).rgb;
				float3x3 local2WorldTranspose = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
				float3 normalDirection = i.normalDir;//normalize( mul( normalLocal, local2WorldTranspose ) );
				float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
				float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				float atten = 1.0;
				float lambertFalloff = max(0.0,dot(normalDirection,lightDirection));
				float halflambert = dot(normalDirection,lightDirection)*0.5f+0.5f;
				float3 sss = halflambert*float3(1.0,0.15,0.1);
				//float lambert = atten * (lambertFalloff) * _LightColor0.xyz;
				//float3 lightFinal = lambert + UNITY_LIGHTMODEL_AMBIENT.xyz;
				return fixed4(sss*0.5,1);
			}
			ENDCG
		}
	}
	FallBack "Specular"
}
