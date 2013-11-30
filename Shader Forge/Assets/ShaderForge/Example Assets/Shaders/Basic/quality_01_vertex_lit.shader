// Shader created with Shader Forge Alpha 0.15 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.15;sub:START;pass:START;ps:lgpr:0,nrmq:1,limd:2,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,uamb:True,ufog:True,aust:True,igpj:False,qofs:0,lico:1,qpre:1,flbk:,rntp:1,lmpd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True;n:type:ShaderForge.SFN_Final,id:1,x:32719,y:32712|diff-4-RGB,spec-2-OUT;n:type:ShaderForge.SFN_Vector1,id:2,x:33009,y:32809,v1:1;n:type:ShaderForge.SFN_Tex2d,id:4,x:33009,y:32669,ptlb:Diffuse,tex:b66bceaf0cc0ace4e9bdc92f14bba709;proporder:4;pass:END;sub:END;*/

Shader "Shader Forge/Examples/Lighting/q01_vertex_lit" {
    Properties {
        _Diffuse ("Diffuse", 2D) = "white" {}
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
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 3.0
            #pragma glsl
            uniform float4 _LightColor0;
            uniform sampler2D _Diffuse;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float3 vtxLight : COLOR;
                float4 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.posWorld = mul(_Object2World, v.vertex);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - mul(_Object2World, v.vertex).xyz);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 halfDirection = normalize(viewDirection+lightDirection);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
////// Lighting:
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                float attenuation = LIGHT_ATTENUATION(o);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float3 diffuse = max( 0.0, dot(o.normalDir,lightDirection )) * attenColor;
///////// Gloss:
                float gloss = exp2(0.5*10.0+1.0);
////// Specular:
                float node_2 = 1.0;
                float3 specular = attenColor * float3(node_2,node_2,node_2) * pow(max(0,dot(halfDirection,o.normalDir)),gloss);
                o.vtxLight = diffuse + specular;
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = normalize(i.normalDir);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float3 lightFinal = i.vtxLight * tex2D(_Diffuse,i.uv0.xy).rgb;
/// Final Color:
                return fixed4(lightFinal,1);
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
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 3.0
            #pragma glsl
            uniform float4 _LightColor0;
            uniform sampler2D _Diffuse;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float3 vtxLight : COLOR;
                float4 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.posWorld = mul(_Object2World, v.vertex);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - mul(_Object2World, v.vertex).xyz);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - o.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 halfDirection = normalize(viewDirection+lightDirection);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
////// Lighting:
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                float attenuation = LIGHT_ATTENUATION(o);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float3 diffuse = max( 0.0, dot(o.normalDir,lightDirection )) * attenColor;
///////// Gloss:
                float gloss = exp2(0.5*10.0+1.0);
////// Specular:
                float node_2 = 1.0;
                float3 specular = attenColor * float3(node_2,node_2,node_2) * pow(max(0,dot(halfDirection,o.normalDir)),gloss);
                o.vtxLight = diffuse + specular;
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = normalize(i.normalDir);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float3 lightFinal = i.vtxLight * tex2D(_Diffuse,i.uv0.xy).rgb;
/// Final Color:
                return fixed4(lightFinal,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
