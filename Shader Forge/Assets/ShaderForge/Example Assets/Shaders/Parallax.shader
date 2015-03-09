// Shader created with Shader Forge v1.06 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.06;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:True,rprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:True,tesm:0,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,dith:0,ufog:True,aust:False,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:0,x:33740,y:32624,varname:node_0,prsc:2|diff-123-RGB,spec-46-OUT,gloss-42-OUT,normal-47-RGB;n:type:ShaderForge.SFN_Multiply,id:35,x:32898,y:32736,varname:node_35,prsc:2|A-37-UVOUT,B-36-OUT;n:type:ShaderForge.SFN_Vector1,id:36,x:32701,y:32828,varname:node_36,prsc:2,v1:8;n:type:ShaderForge.SFN_TexCoord,id:37,x:32701,y:32682,varname:node_37,prsc:2,uv:0;n:type:ShaderForge.SFN_Vector1,id:42,x:33559,y:32729,varname:node_42,prsc:2,v1:0.7;n:type:ShaderForge.SFN_Vector1,id:46,x:33559,y:32673,varname:node_46,prsc:2,v1:0.8;n:type:ShaderForge.SFN_Tex2d,id:47,x:33559,y:32821,ptovrint:False,ptlb:Normal,ptin:_Normal,varname:_Normal,prsc:2,tex:cf20bfced7e912046a9ce991a4d775ec,ntxv:3,isnm:True|UVIN-110-UVOUT;n:type:ShaderForge.SFN_Parallax,id:110,x:33299,y:32714,varname:node_110,prsc:2|UVIN-35-OUT,HEI-111-A,DEP-112-OUT;n:type:ShaderForge.SFN_Tex2d,id:111,x:33095,y:32608,varname:node_574,prsc:2,tex:5fb7986dd6d0a8e4093ba82369dd6a4d,ntxv:0,isnm:False|UVIN-35-OUT,TEX-113-TEX;n:type:ShaderForge.SFN_Vector1,id:112,x:33095,y:32793,varname:node_112,prsc:2,v1:0.15;n:type:ShaderForge.SFN_Tex2dAsset,id:113,x:32898,y:32571,ptovrint:False,ptlb:AO (RGB) Height (A),ptin:_AORGBHeightA,varname:_AORGBHeightA,tex:5fb7986dd6d0a8e4093ba82369dd6a4d,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:123,x:33559,y:32544,varname:node_577,prsc:2,tex:5fb7986dd6d0a8e4093ba82369dd6a4d,ntxv:0,isnm:False|UVIN-110-UVOUT,TEX-113-TEX;proporder:113-47;pass:END;sub:END;*/

Shader "Shader Forge/Examples/Parallax" {
    Properties {
        _AORGBHeightA ("AO (RGB) Height (A)", 2D) = "white" {}
        _Normal ("Normal", 2D) = "bump" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        LOD 128
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define SHOULD_SAMPLE_SH_PROBE ( defined (LIGHTMAP_OFF) )
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers gles xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform sampler2D _AORGBHeightA; uniform float4 _AORGBHeightA_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(_Object2World, float4(v.normal,0)).xyz;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float2 node_35 = (i.uv0*8.0);
                float4 node_574 = tex2D(_AORGBHeightA,TRANSFORM_TEX(node_35, _AORGBHeightA));
                float2 node_110 = (0.15*(node_574.a - 0.5)*mul(tangentTransform, viewDirection).xy + node_35);
                float3 _Normal_var = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_110.rg, _Normal)));
                float3 normalLocal = _Normal_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = 0.7;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float node_46 = 0.8;
                float3 specularColor = float3(node_46,node_46,node_46);
                float3 directSpecular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),specPow);
                float3 specular = directSpecular * specularColor;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 indirectDiffuse = float3(0,0,0);
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                #if SHOULD_SAMPLE_SH_PROBE
                    indirectDiffuse += ShadeSH9(float4(normalDirection,1)) * 0.5; // Per-Pixel Light Probes / Spherical harmonics
                #endif
                float4 node_577 = tex2D(_AORGBHeightA,TRANSFORM_TEX(node_110.rg, _AORGBHeightA));
                float3 diffuse = (directDiffuse + indirectDiffuse) * node_577.rgb;
/// Final Color:
                float3 finalColor = diffuse + specular;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Name "ForwardAdd"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            Fog { Color (0,0,0,0) }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #define SHOULD_SAMPLE_SH_PROBE ( defined (LIGHTMAP_OFF) )
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma exclude_renderers gles xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform sampler2D _AORGBHeightA; uniform float4 _AORGBHeightA_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(_Object2World, float4(v.normal,0)).xyz;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float2 node_35 = (i.uv0*8.0);
                float4 node_574 = tex2D(_AORGBHeightA,TRANSFORM_TEX(node_35, _AORGBHeightA));
                float2 node_110 = (0.15*(node_574.a - 0.5)*mul(tangentTransform, viewDirection).xy + node_35);
                float3 _Normal_var = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_110.rg, _Normal)));
                float3 normalLocal = _Normal_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = 0.7;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float node_46 = 0.8;
                float3 specularColor = float3(node_46,node_46,node_46);
                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow);
                float3 specular = directSpecular * specularColor;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float4 node_577 = tex2D(_AORGBHeightA,TRANSFORM_TEX(node_110.rg, _AORGBHeightA));
                float3 diffuse = directDiffuse * node_577.rgb;
/// Final Color:
                float3 finalColor = diffuse + specular;
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
