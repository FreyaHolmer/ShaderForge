// Shader created with Shader Forge Alpha 0.14 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.14;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:1,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,uamb:False,ufog:True,aust:True,igpj:False,qofs:0,lico:1,qpre:1,flbk:,rntp:1,lmpd:False;n:type:ShaderForge.SFN_Final,id:1,x:32719,y:32712|diff-2-OUT,diffpow-3-OUT;n:type:ShaderForge.SFN_Vector1,id:2,x:33057,y:32702,v1:0.5;n:type:ShaderForge.SFN_Vector1,id:3,x:32988,y:32765,v1:0.8;pass:END;sub:END;*/

Shader "Shader Forge/OrenNayarFake" {
    Properties {
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Tags {
                "LightMode"="ForwardBase"
            }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers gles xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _LightColor0;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                LIGHTING_COORDS(2,3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.posWorld = mul(_Object2World, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float3 normalDirection = normalize(i.normalDir);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float attenuation = LIGHT_ATTENUATION(i);
                float3 lambert = attenuation * pow(max( 0.0, dot(normalDirection,lightDirection )),0.8) * _LightColor0.xyz;
                float3 lightFinal = lambert;
                float node_2 = 0.5;
                return fixed4(lightFinal * float3(node_2,node_2,node_2),1);
            }
            ENDCG
        }
        Pass {
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma exclude_renderers gles xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _LightColor0;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                LIGHTING_COORDS(2,3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.posWorld = mul(_Object2World, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float3 normalDirection = normalize(i.normalDir);
                float3 lightDirection;
                if (0.0 == _WorldSpaceLightPos0.w){
                    lightDirection = normalize( _WorldSpaceLightPos0.xyz );
                } else {
                    lightDirection = normalize( _WorldSpaceLightPos0 - i.posWorld.xyz );
                }
                float attenuation = LIGHT_ATTENUATION(i);
                float3 lambert = attenuation * pow(max( 0.0, dot(normalDirection,lightDirection )),0.8) * _LightColor0.xyz;
                float3 lightFinal = lambert;
                float node_2 = 0.5;
                return fixed4(lightFinal * float3(node_2,node_2,node_2),1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
