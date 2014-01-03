// Shader created with Shader Forge Beta 0.17 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.17;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:1,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,uamb:True,mssp:True,ufog:True,aust:True,igpj:False,qofs:0,lico:1,qpre:1,flbk:,rntp:1,lmpd:False,lprd:True,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,hqsc:True,hqlp:False,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300;n:type:ShaderForge.SFN_Final,id:1,x:32594,y:32645|spec-4-OUT,amspl-15-OUT;n:type:ShaderForge.SFN_Cubemap,id:3,x:33984,y:32840,ptlb:Cubemap,cube:974ac51f7c9d1004b89cb94fb7009210,pvfc:0;n:type:ShaderForge.SFN_Vector1,id:4,x:32932,y:32628,v1:1;n:type:ShaderForge.SFN_Vector1,id:6,x:33984,y:32985,v1:8;n:type:ShaderForge.SFN_Multiply,id:7,x:33407,y:32834|A-3-RGB,B-10-OUT;n:type:ShaderForge.SFN_Desaturate,id:8,x:33221,y:32898|COL-7-OUT,DES-9-OUT;n:type:ShaderForge.SFN_Vector1,id:9,x:33407,y:32978,v1:0.7;n:type:ShaderForge.SFN_Power,id:10,x:33598,y:32920|VAL-14-OUT,EXP-11-OUT;n:type:ShaderForge.SFN_Slider,id:11,x:33794,y:33075,ptlb:Cubemap Power,min:1,cur:1,max:4;n:type:ShaderForge.SFN_Slider,id:13,x:33221,y:33061,ptlb:Cubemap Strength,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Multiply,id:14,x:33794,y:32920|A-3-A,B-6-OUT;n:type:ShaderForge.SFN_Multiply,id:15,x:33054,y:32943|A-8-OUT,B-13-OUT;proporder:3-11-13;pass:END;sub:END;*/

Shader "Shader Forge/Cubemap" {
    Properties {
        _Cubemap ("Cubemap", Cube) = "_Skybox" {}
        _CubemapPower ("Cubemap Power", Range(1, 4)) = 1
        _CubemapStrength ("Cubemap Strength", Range(0, 1)) = 0
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
            uniform float4 _LightColor0;
            uniform samplerCUBE _Cubemap;
            uniform float _CubemapPower;
            uniform float _CubemapStrength;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float3 shLight : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.shLight = ShadeSH9(float4(v.normal * unity_Scale.w,1)) * 0.5;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = normalize(i.normalDir);
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = exp2(0.5*10.0+1.0);
////// Specular:
                float NdotL = dot( normalDirection, lightDirection );
                NdotL = max(0.0, NdotL);
                float4 node_3 = texCUBE(_Cubemap,viewReflectDirection);
                float node_4 = 1.0;
                float3 specularColor = float3(node_4,node_4,node_4);
                float3 specularAmb = (lerp((node_3.rgb*pow((node_3.a*8.0),_CubemapPower)),dot((node_3.rgb*pow((node_3.a*8.0),_CubemapPower)),float3(0.3,0.59,0.11)),0.7)*_CubemapStrength) * specularColor;
                float3 specular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),gloss) * specularColor + specularAmb;
                float3 finalColor = specular;
/// Final Color:
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            Fog { Color (0,0,0,0) }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform samplerCUBE _Cubemap;
            uniform float _CubemapPower;
            uniform float _CubemapStrength;
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
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = normalize(i.normalDir);
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = exp2(0.5*10.0+1.0);
////// Specular:
                float NdotL = dot( normalDirection, lightDirection );
                NdotL = max(0.0, NdotL);
                float node_4 = 1.0;
                float3 specularColor = float3(node_4,node_4,node_4);
                float3 specular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),gloss) * specularColor;
                float3 finalColor = specular;
/// Final Color:
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
