// Shader created with Shader Forge Beta 0.24 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.24;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,hqsc:True,hqlp:False,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0;n:type:ShaderForge.SFN_Final,id:1,x:32330,y:32994|diff-162-OUT,spec-165-OUT,gloss-66-OUT,normal-160-OUT,lwrap-237-OUT,disp-13-OUT,tess-8-OUT;n:type:ShaderForge.SFN_Tex2d,id:3,x:33303,y:32977,ptlb:Normals,tex:cf20bfced7e912046a9ce991a4d775ec,ntxv:3,isnm:True|UVIN-6-OUT;n:type:ShaderForge.SFN_Tex2d,id:4,x:34183,y:32755,tex:5fb7986dd6d0a8e4093ba82369dd6a4d,ntxv:0,isnm:False|UVIN-6-OUT,TEX-254-TEX;n:type:ShaderForge.SFN_TexCoord,id:5,x:35116,y:32879,uv:0;n:type:ShaderForge.SFN_Multiply,id:6,x:34917,y:32946|A-5-UVOUT,B-7-OUT;n:type:ShaderForge.SFN_Vector1,id:7,x:35116,y:33028,v1:2;n:type:ShaderForge.SFN_Vector1,id:8,x:32806,y:33467,v1:3;n:type:ShaderForge.SFN_Tex2d,id:12,x:34380,y:33242,tex:5fb7986dd6d0a8e4093ba82369dd6a4d,ntxv:0,isnm:False|UVIN-6-OUT,MIP-15-OUT,TEX-254-TEX;n:type:ShaderForge.SFN_Multiply,id:13,x:32806,y:33339|A-14-OUT,B-17-OUT;n:type:ShaderForge.SFN_NormalVector,id:14,x:32984,y:33295,pt:False;n:type:ShaderForge.SFN_Vector1,id:15,x:34551,y:33168,v1:1;n:type:ShaderForge.SFN_Slider,id:16,x:33363,y:33576,ptlb:Depth,min:0,cur:0.25,max:0.25;n:type:ShaderForge.SFN_Multiply,id:17,x:32984,y:33447|A-23-OUT,B-26-OUT;n:type:ShaderForge.SFN_OneMinus,id:23,x:33173,y:33408|IN-153-OUT;n:type:ShaderForge.SFN_Multiply,id:26,x:33173,y:33541|A-27-OUT,B-16-OUT;n:type:ShaderForge.SFN_Vector1,id:27,x:33363,y:33485,v1:-1;n:type:ShaderForge.SFN_Vector1,id:66,x:32615,y:32923,v1:10;n:type:ShaderForge.SFN_Tex2d,id:152,x:34380,y:33072,ptlb:Displacement (R),tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-161-UVOUT,MIP-15-OUT;n:type:ShaderForge.SFN_Max,id:153,x:33378,y:33314|A-152-R,B-12-A;n:type:ShaderForge.SFN_Subtract,id:154,x:34194,y:33176|A-12-A,B-152-R;n:type:ShaderForge.SFN_Clamp01,id:156,x:34008,y:33176|IN-154-OUT;n:type:ShaderForge.SFN_Lerp,id:157,x:33077,y:32994|A-159-OUT,B-3-RGB,T-156-OUT;n:type:ShaderForge.SFN_Vector3,id:159,x:33284,y:32860,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Normalize,id:160,x:32904,y:32994|IN-157-OUT;n:type:ShaderForge.SFN_Panner,id:161,x:34730,y:33141,spu:1,spv:0|UVIN-6-OUT;n:type:ShaderForge.SFN_Lerp,id:162,x:33801,y:32630|A-163-OUT,B-170-OUT,T-156-OUT;n:type:ShaderForge.SFN_Vector3,id:163,x:33995,y:32591,v1:0.4117647,v2:0.3826572,v3:0.3602941;n:type:ShaderForge.SFN_Multiply,id:165,x:33303,y:33141|A-156-OUT,B-172-OUT;n:type:ShaderForge.SFN_Multiply,id:170,x:33995,y:32687|A-321-RGB,B-4-RGB;n:type:ShaderForge.SFN_ComponentMask,id:172,x:33625,y:32703,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-162-OUT;n:type:ShaderForge.SFN_OneMinus,id:174,x:32806,y:33188|IN-156-OUT;n:type:ShaderForge.SFN_Multiply,id:237,x:32582,y:33164|A-238-OUT,B-174-OUT;n:type:ShaderForge.SFN_Vector1,id:238,x:32806,y:33133,v1:0.5;n:type:ShaderForge.SFN_Tex2dAsset,id:254,x:34674,y:32791,ptlb:AO (RGB) Height (A),tex:5fb7986dd6d0a8e4093ba82369dd6a4d;n:type:ShaderForge.SFN_Tex2d,id:321,x:34183,y:32591,ptlb:Diffuse,tex:b66bceaf0cc0ace4e9bdc92f14bba709,ntxv:0,isnm:False;proporder:321-3-254-152-16;pass:END;sub:END;*/

Shader "Shader Forge/Examples/TessellationDisplacement" {
    Properties {
        _Diffuse ("Diffuse", 2D) = "white" {}
        _Normals ("Normals", 2D) = "bump" {}
        _AORGBHeightA ("AO (RGB) Height (A)", 2D) = "white" {}
        _DisplacementR ("Displacement (R)", 2D) = "white" {}
        _Depth ("Depth", Range(0, 0.25)) = 0.25
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma hull hull
            #pragma domain domain
            #pragma vertex tessvert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers opengl gles xbox360 ps3 flash 
            #pragma target 5.0
            #pragma glsl
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            
            
            uniform sampler2D _Normals; uniform float4 _Normals_ST;
            
            
            
            
            
            
            
            
            
            
            uniform float _Depth;
            
            
            
            
            
            
            uniform sampler2D _DisplacementR; uniform float4 _DisplacementR_ST;
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            uniform sampler2D _AORGBHeightA; uniform float4 _AORGBHeightA_ST;
            
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            
            
            
            
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
                LIGHTING_COORDS(5,6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            #ifdef UNITY_CAN_COMPILE_TESSELLATION
                struct TessVertex {
                    float4 vertex : INTERNALTESSPOS;
                    float3 normal : NORMAL;
                    float4 tangent : TANGENT;
                    float4 uv0 : TEXCOORD0;
                };
                struct OutputPatchConstant {
                    float edge[3]         : SV_TessFactor;
                    float inside          : SV_InsideTessFactor;
                    float3 vTangent[4]    : TANGENT;
                    float2 vUV[4]         : TEXCOORD;
                    float3 vTanUCorner[4] : TANUCORNER;
                    float3 vTanVCorner[4] : TANVCORNER;
                    float4 vCWts          : TANWEIGHTS;
                };
                TessVertex tessvert (VertexInput v) {
                    TessVertex o;
                    o.vertex = v.vertex;
                    o.normal = v.normal;
                    o.tangent = v.tangent;
                    o.uv0 = v.uv0;
                    return o;
                }
                void displacement (inout VertexInput v){
                    float4 node_348 = _Time + _TimeEditor;
                    float2 node_6 = (v.uv0.rg*2.0);
                    float2 node_161 = (node_6+node_348.g*float2(1,0));
                    float node_15 = 1.0;
                    float4 node_152 = tex2Dlod(_DisplacementR,float4(TRANSFORM_TEX(node_161, _DisplacementR),0.0,node_15));
                    float4 node_12 = tex2Dlod(_AORGBHeightA,float4(TRANSFORM_TEX(node_6, _AORGBHeightA),0.0,node_15));
                    v.vertex.xyz +=  (v.normal*((1.0 - max(node_152.r,node_12.a))*((-1.0)*_Depth)));
                }
                float Tessellation(TessVertex v){
                    return 3.0;
                }
                OutputPatchConstant hullconst (InputPatch<TessVertex,3> v) {
                    OutputPatchConstant o;
                    float ts = Tessellation( v[0] );
                    o.edge[0] = ts;
                    o.edge[1] = ts;
                    o.edge[2] = ts;
                    o.inside = ts;
                    return o;
                }
                [domain("tri")]
                [partitioning("fractional_odd")]
                [outputtopology("triangle_cw")]
                [patchconstantfunc("hullconst")]
                [outputcontrolpoints(3)]
                TessVertex hull (InputPatch<TessVertex,3> v, uint id : SV_OutputControlPointID) {
                    return v[id];
                }
                [domain("tri")]
                VertexOutput domain (OutputPatchConstant tessFactors, const OutputPatch<TessVertex,3> vi, float3 bary : SV_DomainLocation) {
                    VertexInput v;
                    v.vertex = vi[0].vertex*bary.x + vi[1].vertex*bary.y + vi[2].vertex*bary.z;
                    v.normal = vi[0].normal*bary.x + vi[1].normal*bary.y + vi[2].normal*bary.z;
                    v.tangent = vi[0].tangent*bary.x + vi[1].tangent*bary.y + vi[2].tangent*bary.z;
                    v.uv0 = vi[0].uv0*bary.x + vi[1].uv0*bary.y + vi[2].uv0*bary.z;
                    displacement(v);
                    VertexOutput o = vert(v);
                    return o;
                }
            #endif
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float2 node_6 = (i.uv0.rg*2.0);
                float node_15 = 1.0;
                float4 node_12 = tex2Dlod(_AORGBHeightA,float4(TRANSFORM_TEX(node_6, _AORGBHeightA),0.0,node_15));
                float4 node_348 = _Time + _TimeEditor;
                float2 node_161 = (node_6+node_348.g*float2(1,0));
                float4 node_152 = tex2Dlod(_DisplacementR,float4(TRANSFORM_TEX(node_161, _DisplacementR),0.0,node_15));
                float node_156 = saturate((node_12.a-node_152.r));
                float3 normalLocal = normalize(lerp(float3(0,0,1),UnpackNormal(tex2D(_Normals,TRANSFORM_TEX(node_6, _Normals))).rgb,node_156));
                float3 normalDirection =  mul( normalLocal, tangentTransform ); // Perturbed normals
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float node_237 = (0.5*(1.0 - node_156));
                float3 w = float3(node_237,node_237,node_237)*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                float3 diffuse = forwardLight * attenColor + UNITY_LIGHTMODEL_AMBIENT.xyz;
///////// Gloss:
                float gloss = exp2(10.0*10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float2 node_349 = i.uv0;
                float3 node_162 = lerp(float3(0.4117647,0.3826572,0.3602941),(tex2D(_Diffuse,TRANSFORM_TEX(node_349.rg, _Diffuse)).rgb*tex2D(_AORGBHeightA,TRANSFORM_TEX(node_6, _AORGBHeightA)).rgb),node_156);
                float node_165 = (node_156*node_162.r);
                float3 specularColor = float3(node_165,node_165,node_165);
                float3 specular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),gloss) * specularColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                finalColor += diffuseLight * node_162;
                finalColor += specular;
/// Final Color:
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
            #pragma hull hull
            #pragma domain domain
            #pragma vertex tessvert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma exclude_renderers opengl gles xbox360 ps3 flash 
            #pragma target 5.0
            #pragma glsl
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            
            
            uniform sampler2D _Normals; uniform float4 _Normals_ST;
            
            
            
            
            
            
            
            
            
            
            uniform float _Depth;
            
            
            
            
            
            
            uniform sampler2D _DisplacementR; uniform float4 _DisplacementR_ST;
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            uniform sampler2D _AORGBHeightA; uniform float4 _AORGBHeightA_ST;
            
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            
            
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
                LIGHTING_COORDS(5,6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            #ifdef UNITY_CAN_COMPILE_TESSELLATION
                struct TessVertex {
                    float4 vertex : INTERNALTESSPOS;
                    float3 normal : NORMAL;
                    float4 tangent : TANGENT;
                    float4 uv0 : TEXCOORD0;
                };
                struct OutputPatchConstant {
                    float edge[3]         : SV_TessFactor;
                    float inside          : SV_InsideTessFactor;
                    float3 vTangent[4]    : TANGENT;
                    float2 vUV[4]         : TEXCOORD;
                    float3 vTanUCorner[4] : TANUCORNER;
                    float3 vTanVCorner[4] : TANVCORNER;
                    float4 vCWts          : TANWEIGHTS;
                };
                TessVertex tessvert (VertexInput v) {
                    TessVertex o;
                    o.vertex = v.vertex;
                    o.normal = v.normal;
                    o.tangent = v.tangent;
                    o.uv0 = v.uv0;
                    return o;
                }
                void displacement (inout VertexInput v){
                    float4 node_350 = _Time + _TimeEditor;
                    float2 node_6 = (v.uv0.rg*2.0);
                    float2 node_161 = (node_6+node_350.g*float2(1,0));
                    float node_15 = 1.0;
                    float4 node_152 = tex2Dlod(_DisplacementR,float4(TRANSFORM_TEX(node_161, _DisplacementR),0.0,node_15));
                    float4 node_12 = tex2Dlod(_AORGBHeightA,float4(TRANSFORM_TEX(node_6, _AORGBHeightA),0.0,node_15));
                    v.vertex.xyz +=  (v.normal*((1.0 - max(node_152.r,node_12.a))*((-1.0)*_Depth)));
                }
                float Tessellation(TessVertex v){
                    return 3.0;
                }
                OutputPatchConstant hullconst (InputPatch<TessVertex,3> v) {
                    OutputPatchConstant o;
                    float ts = Tessellation( v[0] );
                    o.edge[0] = ts;
                    o.edge[1] = ts;
                    o.edge[2] = ts;
                    o.inside = ts;
                    return o;
                }
                [domain("tri")]
                [partitioning("fractional_odd")]
                [outputtopology("triangle_cw")]
                [patchconstantfunc("hullconst")]
                [outputcontrolpoints(3)]
                TessVertex hull (InputPatch<TessVertex,3> v, uint id : SV_OutputControlPointID) {
                    return v[id];
                }
                [domain("tri")]
                VertexOutput domain (OutputPatchConstant tessFactors, const OutputPatch<TessVertex,3> vi, float3 bary : SV_DomainLocation) {
                    VertexInput v;
                    v.vertex = vi[0].vertex*bary.x + vi[1].vertex*bary.y + vi[2].vertex*bary.z;
                    v.normal = vi[0].normal*bary.x + vi[1].normal*bary.y + vi[2].normal*bary.z;
                    v.tangent = vi[0].tangent*bary.x + vi[1].tangent*bary.y + vi[2].tangent*bary.z;
                    v.uv0 = vi[0].uv0*bary.x + vi[1].uv0*bary.y + vi[2].uv0*bary.z;
                    displacement(v);
                    VertexOutput o = vert(v);
                    return o;
                }
            #endif
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float2 node_6 = (i.uv0.rg*2.0);
                float node_15 = 1.0;
                float4 node_12 = tex2Dlod(_AORGBHeightA,float4(TRANSFORM_TEX(node_6, _AORGBHeightA),0.0,node_15));
                float4 node_350 = _Time + _TimeEditor;
                float2 node_161 = (node_6+node_350.g*float2(1,0));
                float4 node_152 = tex2Dlod(_DisplacementR,float4(TRANSFORM_TEX(node_161, _DisplacementR),0.0,node_15));
                float node_156 = saturate((node_12.a-node_152.r));
                float3 normalLocal = normalize(lerp(float3(0,0,1),UnpackNormal(tex2D(_Normals,TRANSFORM_TEX(node_6, _Normals))).rgb,node_156));
                float3 normalDirection =  mul( normalLocal, tangentTransform ); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float node_237 = (0.5*(1.0 - node_156));
                float3 w = float3(node_237,node_237,node_237)*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                float3 diffuse = forwardLight * attenColor;
///////// Gloss:
                float gloss = exp2(10.0*10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float2 node_351 = i.uv0;
                float3 node_162 = lerp(float3(0.4117647,0.3826572,0.3602941),(tex2D(_Diffuse,TRANSFORM_TEX(node_351.rg, _Diffuse)).rgb*tex2D(_AORGBHeightA,TRANSFORM_TEX(node_6, _AORGBHeightA)).rgb),node_156);
                float node_165 = (node_156*node_162.r);
                float3 specularColor = float3(node_165,node_165,node_165);
                float3 specular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),gloss) * specularColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                finalColor += diffuseLight * node_162;
                finalColor += specular;
/// Final Color:
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
        Pass {
            Name "ShadowCollector"
            Tags {
                "LightMode"="ShadowCollector"
            }
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma hull hull
            #pragma domain domain
            #pragma vertex tessvert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCOLLECTOR
            #define SHADOW_COLLECTOR_PASS
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcollector
            #pragma exclude_renderers opengl gles xbox360 ps3 flash 
            #pragma target 5.0
            #pragma glsl
            uniform float4 _TimeEditor;
            
            
            
            
            
            
            
            
            
            
            uniform float _Depth;
            
            
            
            
            
            uniform sampler2D _DisplacementR; uniform float4 _DisplacementR_ST;
            
            
            
            uniform sampler2D _AORGBHeightA; uniform float4 _AORGBHeightA_ST;
            
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_COLLECTOR;
                float4 uv0 : TEXCOORD5;
                float3 normalDir : TEXCOORD6;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_SHADOW_COLLECTOR(o)
                return o;
            }
            #ifdef UNITY_CAN_COMPILE_TESSELLATION
                struct TessVertex {
                    float4 vertex : INTERNALTESSPOS;
                    float3 normal : NORMAL;
                    float4 tangent : TANGENT;
                    float4 uv0 : TEXCOORD0;
                };
                struct OutputPatchConstant {
                    float edge[3]         : SV_TessFactor;
                    float inside          : SV_InsideTessFactor;
                    float3 vTangent[4]    : TANGENT;
                    float2 vUV[4]         : TEXCOORD;
                    float3 vTanUCorner[4] : TANUCORNER;
                    float3 vTanVCorner[4] : TANVCORNER;
                    float4 vCWts          : TANWEIGHTS;
                };
                TessVertex tessvert (VertexInput v) {
                    TessVertex o;
                    o.vertex = v.vertex;
                    o.normal = v.normal;
                    o.tangent = v.tangent;
                    o.uv0 = v.uv0;
                    return o;
                }
                void displacement (inout VertexInput v){
                    float4 node_352 = _Time + _TimeEditor;
                    float2 node_6 = (v.uv0.rg*2.0);
                    float2 node_161 = (node_6+node_352.g*float2(1,0));
                    float node_15 = 1.0;
                    float4 node_152 = tex2Dlod(_DisplacementR,float4(TRANSFORM_TEX(node_161, _DisplacementR),0.0,node_15));
                    float4 node_12 = tex2Dlod(_AORGBHeightA,float4(TRANSFORM_TEX(node_6, _AORGBHeightA),0.0,node_15));
                    v.vertex.xyz +=  (v.normal*((1.0 - max(node_152.r,node_12.a))*((-1.0)*_Depth)));
                }
                float Tessellation(TessVertex v){
                    return 3.0;
                }
                OutputPatchConstant hullconst (InputPatch<TessVertex,3> v) {
                    OutputPatchConstant o;
                    float ts = Tessellation( v[0] );
                    o.edge[0] = ts;
                    o.edge[1] = ts;
                    o.edge[2] = ts;
                    o.inside = ts;
                    return o;
                }
                [domain("tri")]
                [partitioning("fractional_odd")]
                [outputtopology("triangle_cw")]
                [patchconstantfunc("hullconst")]
                [outputcontrolpoints(3)]
                TessVertex hull (InputPatch<TessVertex,3> v, uint id : SV_OutputControlPointID) {
                    return v[id];
                }
                [domain("tri")]
                VertexOutput domain (OutputPatchConstant tessFactors, const OutputPatch<TessVertex,3> vi, float3 bary : SV_DomainLocation) {
                    VertexInput v;
                    v.vertex = vi[0].vertex*bary.x + vi[1].vertex*bary.y + vi[2].vertex*bary.z;
                    v.normal = vi[0].normal*bary.x + vi[1].normal*bary.y + vi[2].normal*bary.z;
                    v.tangent = vi[0].tangent*bary.x + vi[1].tangent*bary.y + vi[2].tangent*bary.z;
                    v.uv0 = vi[0].uv0*bary.x + vi[1].uv0*bary.y + vi[2].uv0*bary.z;
                    displacement(v);
                    VertexOutput o = vert(v);
                    return o;
                }
            #endif
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                SHADOW_COLLECTOR_FRAGMENT(i)
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Cull Off
            Offset 1, 1
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma hull hull
            #pragma domain domain
            #pragma vertex tessvert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma exclude_renderers opengl gles xbox360 ps3 flash 
            #pragma target 5.0
            #pragma glsl
            uniform float4 _TimeEditor;
            
            
            
            
            
            
            
            
            
            
            uniform float _Depth;
            
            
            
            
            
            uniform sampler2D _DisplacementR; uniform float4 _DisplacementR_ST;
            
            
            
            uniform sampler2D _AORGBHeightA; uniform float4 _AORGBHeightA_ST;
            
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float4 uv0 : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            #ifdef UNITY_CAN_COMPILE_TESSELLATION
                struct TessVertex {
                    float4 vertex : INTERNALTESSPOS;
                    float3 normal : NORMAL;
                    float4 tangent : TANGENT;
                    float4 uv0 : TEXCOORD0;
                };
                struct OutputPatchConstant {
                    float edge[3]         : SV_TessFactor;
                    float inside          : SV_InsideTessFactor;
                    float3 vTangent[4]    : TANGENT;
                    float2 vUV[4]         : TEXCOORD;
                    float3 vTanUCorner[4] : TANUCORNER;
                    float3 vTanVCorner[4] : TANVCORNER;
                    float4 vCWts          : TANWEIGHTS;
                };
                TessVertex tessvert (VertexInput v) {
                    TessVertex o;
                    o.vertex = v.vertex;
                    o.normal = v.normal;
                    o.tangent = v.tangent;
                    o.uv0 = v.uv0;
                    return o;
                }
                void displacement (inout VertexInput v){
                    float4 node_353 = _Time + _TimeEditor;
                    float2 node_6 = (v.uv0.rg*2.0);
                    float2 node_161 = (node_6+node_353.g*float2(1,0));
                    float node_15 = 1.0;
                    float4 node_152 = tex2Dlod(_DisplacementR,float4(TRANSFORM_TEX(node_161, _DisplacementR),0.0,node_15));
                    float4 node_12 = tex2Dlod(_AORGBHeightA,float4(TRANSFORM_TEX(node_6, _AORGBHeightA),0.0,node_15));
                    v.vertex.xyz +=  (v.normal*((1.0 - max(node_152.r,node_12.a))*((-1.0)*_Depth)));
                }
                float Tessellation(TessVertex v){
                    return 3.0;
                }
                OutputPatchConstant hullconst (InputPatch<TessVertex,3> v) {
                    OutputPatchConstant o;
                    float ts = Tessellation( v[0] );
                    o.edge[0] = ts;
                    o.edge[1] = ts;
                    o.edge[2] = ts;
                    o.inside = ts;
                    return o;
                }
                [domain("tri")]
                [partitioning("fractional_odd")]
                [outputtopology("triangle_cw")]
                [patchconstantfunc("hullconst")]
                [outputcontrolpoints(3)]
                TessVertex hull (InputPatch<TessVertex,3> v, uint id : SV_OutputControlPointID) {
                    return v[id];
                }
                [domain("tri")]
                VertexOutput domain (OutputPatchConstant tessFactors, const OutputPatch<TessVertex,3> vi, float3 bary : SV_DomainLocation) {
                    VertexInput v;
                    v.vertex = vi[0].vertex*bary.x + vi[1].vertex*bary.y + vi[2].vertex*bary.z;
                    v.normal = vi[0].normal*bary.x + vi[1].normal*bary.y + vi[2].normal*bary.z;
                    v.tangent = vi[0].tangent*bary.x + vi[1].tangent*bary.y + vi[2].tangent*bary.z;
                    v.uv0 = vi[0].uv0*bary.x + vi[1].uv0*bary.y + vi[2].uv0*bary.z;
                    displacement(v);
                    VertexOutput o = vert(v);
                    return o;
                }
            #endif
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
