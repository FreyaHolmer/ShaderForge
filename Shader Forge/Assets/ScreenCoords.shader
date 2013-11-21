// Shader created with Shader Forge Alpha 0.13 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.13;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:3,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,uamb:True,ufog:True,aust:True,igpj:False,qofs:0,lico:1,qpre:1,flbk:,rntp:1,lmpd:False;n:type:ShaderForge.SFN_Final,id:1,x:32728,y:32655|8-7-0;n:type:ShaderForge.SFN_ScreenPos,id:2,x:33556,y:32687;n:type:ShaderForge.SFN_Round,id:4,x:33161,y:32700|1-5-0;n:type:ShaderForge.SFN_Multiply,id:5,x:33354,y:32700|1-2-0,2-6-0;n:type:ShaderForge.SFN_Vector1,id:6,x:33556,y:32815,v1:8;n:type:ShaderForge.SFN_Divide,id:7,x:32990,y:32815|1-4-0,2-6-0;pass:END;sub:END;*/

Shader "Shader Forge/ScreenCoords" {
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
                float4 screenPos : TEXCOORD2;
                LIGHTING_COORDS(3,4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.screenPos = float4( o.pos.xy / o.pos.w, 0, 0 );
                o.screenPos.y *= _ProjectionParams.x;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.posWorld = mul(_Object2World, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float3 normalDirection = normalize(i.normalDir);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float attenuation = LIGHT_ATTENUATION(i);
                float3 lambert = attenuation * max( 0.0, dot(normalDirection,lightDirection )) * _LightColor0.xyz;
                float node_6 = 8;
                float3 addLight = float3((round((i.screenPos.xy.rg*node_6))/node_6),0.0);
                float3 lightFinal = lambert + UNITY_LIGHTMODEL_AMBIENT.xyz;
                return fixed4(lightFinal * float3(0,0,0) + addLight,1);
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
                float4 screenPos : TEXCOORD2;
                LIGHTING_COORDS(3,4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.screenPos = float4( o.pos.xy / o.pos.w, 0, 0 );
                o.screenPos.y *= _ProjectionParams.x;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
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
                float3 lambert = attenuation * max( 0.0, dot(normalDirection,lightDirection )) * _LightColor0.xyz;
                
                float3 lightFinal = lambert;
                return fixed4(lightFinal * float3(0,0,0),1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
