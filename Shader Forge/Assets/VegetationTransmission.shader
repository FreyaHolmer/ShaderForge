// Shader created with Shader Forge Alpha 0.01 
// Shader Forge (c) Joachim 'Acegikmo' Holmér
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.01;sub:START;n:type:ShaderForge.SFN_Final,id:0,x:32747,y:32797|0-26-0,1-24-0,3-20-0,6-1-6,7-12-0,9-10-0,10-9-0;n:type:ShaderForge.SFN_Tex2d,id:1,x:33663,y:32433,tex:66321cc856b03e245ac41ed8a53e0ecc;n:type:ShaderForge.SFN_Vector3,id:8,x:33293,y:33011,v1:0.8,v2:1,v3:0.4;n:type:ShaderForge.SFN_Vector1,id:9,x:33121,y:33182,v1:0.8;n:type:ShaderForge.SFN_Multiply,id:10,x:33121,y:33035|1-8-0,2-11-0;n:type:ShaderForge.SFN_Slider,id:11,x:33390,y:33127,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Vector1,id:12,x:33122,y:32891,v1:2;n:type:ShaderForge.SFN_Tex2d,id:19,x:33501,y:32782,tex:cb6c5165ed180c543be39ed70e72abc8;n:type:ShaderForge.SFN_Lerp,id:20,x:33291,y:32776|1-19-2,2-21-0,3-23-0;n:type:ShaderForge.SFN_Vector3,id:21,x:33501,y:32916,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Slider,id:23,x:33575,y:33022,min:0,cur:0.5263158,max:1;n:type:ShaderForge.SFN_Power,id:24,x:33425,y:32532|1-1-3,2-25-0;n:type:ShaderForge.SFN_Vector1,id:25,x:33599,y:32584,v1:2;n:type:ShaderForge.SFN_Multiply,id:26,x:33125,y:32460|1-27-0,2-1-2;n:type:ShaderForge.SFN_Vector1,id:27,x:33335,y:32372,v1:1;sub:END;*/

Shader "Shader Forge/VegetationTransmission" {
    Properties {
        _node_1 ("Texture thing", 2D) = "white" {}
        _node_11 ("", Range(0, 1)) = 0
        _node_19 ("s", 2D) = "white" {}
        _node_23 ("", Range(0, 1)) = 0
    }
    SubShader {
        Pass {
        	Name "Caster"
        	Tags { "LightMode" = "ShadowCaster" }
        	Offset 1, 1
        	Fog {Mode Off}
        	ZWrite On ZTest LEqual Cull Off
        	CGPROGRAM
        		#pragma vertex vert
        		#pragma fragment frag
        		#pragma multi_compile_shadowcaster
        		#pragma fragmentoption ARB_precision_hint_fastest
        		#include "UnityCG.cginc"
        		struct v2f {
        			V2F_SHADOW_CASTER;
        			float2  uv0 : TEXCOORD1;
        		};
        uniform sampler2D _node_1;
        uniform float4 _node_1_ST;
        		v2f vert( appdata_base v ){
        			v2f o;
        			TRANSFER_SHADOW_CASTER(o)
        			o.uv0 = v.texcoord;
        			return o;
        		}
        		float4 frag( v2f i ) : COLOR {
        float4 node_1 = tex2D(_node_1,i.uv0.xy);
        clip(node_1.a - 0.5f);
        			SHADOW_CASTER_FRAGMENT(i)
        		}
        	ENDCG
        }
        Pass {
            Tags {
                "LightMode"="ForwardBase"
            }
            
            CGPROGRAM
            #include "UnityCG.cginc"
            #pragma vertex vert
            #pragma fragment frag
            uniform float4 _LightColor0;
            uniform sampler2D _node_1;
            uniform float _node_11;
            uniform sampler2D _node_19;
            uniform float _node_23;
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
                o.tangentDir = normalize(float3(mul(_Object2World, float4(float3(v.tangent), 0.0))));
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float4 node_1 = tex2D(_node_1,i.uv0.xy);
                clip(node_1.a - 0.5f);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalLocal = lerp(UnpackNormal(tex2D(_node_19,i.uv0.xy)).rgb,float3(0,0,1),_node_23);
                float3x3 local2WorldTranspose = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 normalDirection = normalize( mul( normalLocal, local2WorldTranspose ).xyz );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float atten = 1.0;
                float NdotL = dot( normalDirection, lightDirection );
                float3 lightWrap = 0.8*0.5;
                float3 NdotLWrap = NdotL * ( 1.0 - lightWrap );
                float3 forwardLight = pow( saturate( NdotLWrap + lightWrap ), 2 );
                float3 backLight = pow( saturate( -NdotLWrap + lightWrap ), 2 ) * (float3(0.8,1,0.4)*_node_11);
                float3 lambert = forwardLight+backLight;
                float3 addLight = lambert * pow(node_1.r,2) * pow(max(0,dot(reflect(-lightDirection, normalDirection),viewDirection)),10);
                float3 lightFinal = lambert + UNITY_LIGHTMODEL_AMBIENT.xyz;
                return fixed4(lightFinal * (1*node_1.rgb) + addLight,1);
            }
            ENDCG
        }
    }
    FallBack "Specular"
}
