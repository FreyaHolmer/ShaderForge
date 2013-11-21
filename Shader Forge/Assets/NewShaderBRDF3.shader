// Shader created with Shader Forge Alpha 0.01 
// Shader Forge (c) Joachim 'Acegikmo' Holmér
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.01;sub:START;n:type:ShaderForge.SFN_Final,id:0,x:33522,y:32782|0-23-0,9-22-0;n:type:ShaderForge.SFN_Vector1,id:22,x:33857,y:32885,v1:1;n:type:ShaderForge.SFN_Vector1,id:23,x:33844,y:32645,v1:1;n:type:ShaderForge.SFN_Slider,id:24,x:34017,y:32751,min:0,cur:0,max:1;sub:END;*/

Shader "Shader Forge/NewShaderBRDF3" {
    Properties {
        _node_24 ("", Range(0, 1)) = 0
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
            uniform float4 _LightColor0;
            uniform float _node_24;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.uv0 = v.uv0;
                float3 normalDirection = mul(float4(v.normal,0), _World2Object).xyz;
                o.normalDir = normalDirection;
                o.posWorld = mul(_Object2World, v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float3 normalDirection = normalize(i.normalDir);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float atten = 1.0;
                float NdotL = dot( normalDirection, lightDirection );
                float3 forwardLight = pow( saturate( NdotL ), 1 );
                float3 backLight = pow( saturate( -NdotL ), 1 ) * 1;
                float3 lambert = forwardLight+backLight;
                float3 lightFinal = lambert + UNITY_LIGHTMODEL_AMBIENT.xyz;
                return fixed4(lightFinal * 1,1);
            }
            ENDCG
        }
    }
    FallBack "Specular"
}
