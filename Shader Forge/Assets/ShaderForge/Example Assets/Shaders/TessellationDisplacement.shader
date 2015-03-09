// Shader created with Shader Forge v1.06 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.06;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,rprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,dith:0,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:34362,y:32994,varname:node_1,prsc:2|diff-162-OUT,spec-165-OUT,gloss-66-OUT,normal-160-OUT,lwrap-237-OUT,disp-13-OUT,tess-8-OUT;n:type:ShaderForge.SFN_Tex2d,id:3,x:33597,y:33194,ptovrint:False,ptlb:Normals,ptin:_Normals,varname:_Normals,prsc:2,tex:cf20bfced7e912046a9ce991a4d775ec,ntxv:3,isnm:True|UVIN-6-OUT;n:type:ShaderForge.SFN_Tex2d,id:4,x:32986,y:33006,varname:node_798,prsc:2,tex:5fb7986dd6d0a8e4093ba82369dd6a4d,ntxv:0,isnm:False|UVIN-6-OUT,TEX-254-TEX;n:type:ShaderForge.SFN_TexCoord,id:5,x:32078,y:33020,varname:node_5,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:6,x:32307,y:33100,varname:node_6,prsc:2|A-5-UVOUT,B-7-OUT;n:type:ShaderForge.SFN_Vector1,id:7,x:32078,y:33232,varname:node_7,prsc:2,v1:2;n:type:ShaderForge.SFN_Vector1,id:8,x:34051,y:33620,varname:node_8,prsc:2,v1:3;n:type:ShaderForge.SFN_Tex2d,id:12,x:32759,y:33284,varname:node_803,prsc:2,tex:5fb7986dd6d0a8e4093ba82369dd6a4d,ntxv:0,isnm:False|UVIN-6-OUT,MIP-15-OUT,TEX-254-TEX;n:type:ShaderForge.SFN_Multiply,id:13,x:34051,y:33418,varname:node_13,prsc:2|A-14-OUT,B-17-OUT;n:type:ShaderForge.SFN_NormalVector,id:14,x:33824,y:33442,prsc:2,pt:False;n:type:ShaderForge.SFN_Vector1,id:15,x:32532,y:33364,varname:node_15,prsc:2,v1:1;n:type:ShaderForge.SFN_Slider,id:16,x:33213,y:33639,ptovrint:False,ptlb:Depth,ptin:_Depth,varname:_Depth,prsc:2,min:0,cur:0.25,max:0.25;n:type:ShaderForge.SFN_Multiply,id:17,x:33824,y:33620,varname:node_17,prsc:2|A-23-OUT,B-26-OUT;n:type:ShaderForge.SFN_OneMinus,id:23,x:33597,y:33379,varname:node_23,prsc:2|IN-153-OUT;n:type:ShaderForge.SFN_Multiply,id:26,x:33597,y:33537,varname:node_26,prsc:2|A-27-OUT,B-16-OUT;n:type:ShaderForge.SFN_Vector1,id:27,x:33370,y:33478,varname:node_27,prsc:2,v1:-1;n:type:ShaderForge.SFN_Vector1,id:66,x:34051,y:32948,varname:node_66,prsc:2,v1:10;n:type:ShaderForge.SFN_Tex2d,id:152,x:32759,y:33099,ptovrint:False,ptlb:Displacement (R),ptin:_DisplacementR,varname:_DisplacementR,prsc:2,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-161-UVOUT,MIP-15-OUT;n:type:ShaderForge.SFN_Max,id:153,x:33370,y:33276,varname:node_153,prsc:2|A-152-R,B-12-A;n:type:ShaderForge.SFN_Subtract,id:154,x:32986,y:33174,varname:node_154,prsc:2|A-12-A,B-152-R;n:type:ShaderForge.SFN_Clamp01,id:156,x:33370,y:33108,varname:node_156,prsc:2|IN-154-OUT;n:type:ShaderForge.SFN_Lerp,id:157,x:33824,y:32948,varname:node_157,prsc:2|A-159-OUT,B-3-RGB,T-156-OUT;n:type:ShaderForge.SFN_Vector3,id:159,x:33597,y:33061,varname:node_159,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Normalize,id:160,x:34051,y:33082,varname:node_160,prsc:2|IN-157-OUT;n:type:ShaderForge.SFN_Panner,id:161,x:32532,y:32984,varname:node_161,prsc:2,spu:1,spv:0|UVIN-6-OUT;n:type:ShaderForge.SFN_Lerp,id:162,x:33597,y:32875,varname:node_162,prsc:2|A-163-OUT,B-170-OUT,T-156-OUT;n:type:ShaderForge.SFN_Vector3,id:163,x:33370,y:32790,varname:node_163,prsc:2,v1:0.4117647,v2:0.3826572,v3:0.3602941;n:type:ShaderForge.SFN_Multiply,id:165,x:34051,y:32746,varname:node_165,prsc:2|A-156-OUT,B-172-OUT;n:type:ShaderForge.SFN_Multiply,id:170,x:33370,y:32940,varname:node_170,prsc:2|A-321-RGB,B-4-RGB;n:type:ShaderForge.SFN_ComponentMask,id:172,x:33824,y:32770,varname:node_172,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-162-OUT;n:type:ShaderForge.SFN_OneMinus,id:174,x:33824,y:33284,varname:node_174,prsc:2|IN-156-OUT;n:type:ShaderForge.SFN_Multiply,id:237,x:34051,y:33250,varname:node_237,prsc:2|A-238-OUT,B-174-OUT;n:type:ShaderForge.SFN_Vector1,id:238,x:33824,y:33150,varname:node_238,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Tex2dAsset,id:254,x:32532,y:33145,ptovrint:False,ptlb:AO (RGB) Height (A),ptin:_AORGBHeightA,varname:_AORGBHeightA,tex:5fb7986dd6d0a8e4093ba82369dd6a4d,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:321,x:32986,y:32821,ptovrint:False,ptlb:Diffuse,ptin:_Diffuse,varname:_Diffuse,prsc:2,tex:b66bceaf0cc0ace4e9bdc92f14bba709,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Add,id:354,x:32182,y:32491,varname:node_354,prsc:2;n:type:ShaderForge.SFN_Add,id:355,x:32397,y:32572,varname:node_355,prsc:2|A-354-OUT;n:type:ShaderForge.SFN_Add,id:356,x:32636,y:32651,varname:node_356,prsc:2|A-355-OUT;n:type:ShaderForge.SFN_Add,id:357,x:32822,y:32688,varname:node_357,prsc:2|A-356-OUT;n:type:ShaderForge.SFN_Tex2d,id:358,x:31995,y:32709,ptovrint:False,ptlb:node_358,ptin:_node_358,varname:_node_358,prsc:2,ntxv:0,isnm:False|UVIN-362-OUT;n:type:ShaderForge.SFN_Add,id:359,x:32242,y:32765,varname:node_359,prsc:2|A-358-RGB;n:type:ShaderForge.SFN_Add,id:360,x:32441,y:32803,varname:node_360,prsc:2|A-359-OUT;n:type:ShaderForge.SFN_Add,id:361,x:32657,y:32822,varname:node_361,prsc:2|A-360-OUT;n:type:ShaderForge.SFN_Multiply,id:362,x:31802,y:32804,varname:node_362,prsc:2|A-363-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:363,x:31598,y:32700,varname:node_363,prsc:2,uv:0;proporder:321-3-254-152-16;pass:END;sub:END;*/

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
            #include "Tessellation.cginc"
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
            #ifdef UNITY_CAN_COMPILE_TESSELLATION
                struct TessVertex {
                    float4 vertex : INTERNALTESSPOS;
                    float3 normal : NORMAL;
                    float4 tangent : TANGENT;
                    float2 texcoord0 : TEXCOORD0;
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
                    o.texcoord0 = v.texcoord0;
                    return o;
                }
                void displacement (inout VertexInput v){
                    float4 node_3781 = _Time + _TimeEditor;
                    float2 node_6 = (v.texcoord0*2.0);
                    float2 node_161 = (node_6+node_3781.g*float2(1,0));
                    float node_15 = 1.0;
                    float4 _DisplacementR_var = tex2Dlod(_DisplacementR,float4(TRANSFORM_TEX(node_161, _DisplacementR),0.0,node_15));
                    float4 node_803 = tex2Dlod(_AORGBHeightA,float4(TRANSFORM_TEX(node_6, _AORGBHeightA),0.0,node_15));
                    v.vertex.xyz += (v.normal*((1.0 - max(_DisplacementR_var.r,node_803.a))*((-1.0)*_Depth)));
                }
                float Tessellation(TessVertex v){
                    return 3.0;
                }
                float4 Tessellation(TessVertex v, TessVertex v1, TessVertex v2){
                    float tv = Tessellation(v);
                    float tv1 = Tessellation(v1);
                    float tv2 = Tessellation(v2);
                    return float4( tv1+tv2, tv2+tv, tv+tv1, tv+tv1+tv2 ) / float4(2,2,2,3);
                }
                OutputPatchConstant hullconst (InputPatch<TessVertex,3> v) {
                    OutputPatchConstant o;
                    float4 ts = Tessellation( v[0], v[1], v[2] );
                    o.edge[0] = ts.x;
                    o.edge[1] = ts.y;
                    o.edge[2] = ts.z;
                    o.inside = ts.w;
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
                    v.texcoord0 = vi[0].texcoord0*bary.x + vi[1].texcoord0*bary.y + vi[2].texcoord0*bary.z;
                    displacement(v);
                    VertexOutput o = vert(v);
                    return o;
                }
            #endif
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float2 node_6 = (i.uv0*2.0);
                float3 _Normals_var = UnpackNormal(tex2D(_Normals,TRANSFORM_TEX(node_6, _Normals)));
                float node_15 = 1.0;
                float4 node_803 = tex2Dlod(_AORGBHeightA,float4(TRANSFORM_TEX(node_6, _AORGBHeightA),0.0,node_15));
                float4 node_3781 = _Time + _TimeEditor;
                float2 node_161 = (node_6+node_3781.g*float2(1,0));
                float4 _DisplacementR_var = tex2Dlod(_DisplacementR,float4(TRANSFORM_TEX(node_161, _DisplacementR),0.0,node_15));
                float node_156 = saturate((node_803.a-_DisplacementR_var.r));
                float3 normalLocal = normalize(lerp(float3(0,0,1),_Normals_var.rgb,node_156));
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = 10.0;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(i.uv0, _Diffuse));
                float4 node_798 = tex2D(_AORGBHeightA,TRANSFORM_TEX(node_6, _AORGBHeightA));
                float3 node_162 = lerp(float3(0.4117647,0.3826572,0.3602941),(_Diffuse_var.rgb*node_798.rgb),node_156);
                float node_165 = (node_156*node_162.r);
                float3 specularColor = float3(node_165,node_165,node_165);
                float3 directSpecular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),specPow);
                float3 specular = directSpecular * specularColor;
/////// Diffuse:
                NdotL = dot( normalDirection, lightDirection );
                float node_237 = (0.5*(1.0 - node_156));
                float3 w = float3(node_237,node_237,node_237)*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                float3 indirectDiffuse = float3(0,0,0);
                float3 directDiffuse = forwardLight * attenColor;
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float3 diffuse = (directDiffuse + indirectDiffuse) * node_162;
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
            #pragma hull hull
            #pragma domain domain
            #pragma vertex tessvert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Tessellation.cginc"
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
            #ifdef UNITY_CAN_COMPILE_TESSELLATION
                struct TessVertex {
                    float4 vertex : INTERNALTESSPOS;
                    float3 normal : NORMAL;
                    float4 tangent : TANGENT;
                    float2 texcoord0 : TEXCOORD0;
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
                    o.texcoord0 = v.texcoord0;
                    return o;
                }
                void displacement (inout VertexInput v){
                    float4 node_4086 = _Time + _TimeEditor;
                    float2 node_6 = (v.texcoord0*2.0);
                    float2 node_161 = (node_6+node_4086.g*float2(1,0));
                    float node_15 = 1.0;
                    float4 _DisplacementR_var = tex2Dlod(_DisplacementR,float4(TRANSFORM_TEX(node_161, _DisplacementR),0.0,node_15));
                    float4 node_803 = tex2Dlod(_AORGBHeightA,float4(TRANSFORM_TEX(node_6, _AORGBHeightA),0.0,node_15));
                    v.vertex.xyz += (v.normal*((1.0 - max(_DisplacementR_var.r,node_803.a))*((-1.0)*_Depth)));
                }
                float Tessellation(TessVertex v){
                    return 3.0;
                }
                float4 Tessellation(TessVertex v, TessVertex v1, TessVertex v2){
                    float tv = Tessellation(v);
                    float tv1 = Tessellation(v1);
                    float tv2 = Tessellation(v2);
                    return float4( tv1+tv2, tv2+tv, tv+tv1, tv+tv1+tv2 ) / float4(2,2,2,3);
                }
                OutputPatchConstant hullconst (InputPatch<TessVertex,3> v) {
                    OutputPatchConstant o;
                    float4 ts = Tessellation( v[0], v[1], v[2] );
                    o.edge[0] = ts.x;
                    o.edge[1] = ts.y;
                    o.edge[2] = ts.z;
                    o.inside = ts.w;
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
                    v.texcoord0 = vi[0].texcoord0*bary.x + vi[1].texcoord0*bary.y + vi[2].texcoord0*bary.z;
                    displacement(v);
                    VertexOutput o = vert(v);
                    return o;
                }
            #endif
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float2 node_6 = (i.uv0*2.0);
                float3 _Normals_var = UnpackNormal(tex2D(_Normals,TRANSFORM_TEX(node_6, _Normals)));
                float node_15 = 1.0;
                float4 node_803 = tex2Dlod(_AORGBHeightA,float4(TRANSFORM_TEX(node_6, _AORGBHeightA),0.0,node_15));
                float4 node_4086 = _Time + _TimeEditor;
                float2 node_161 = (node_6+node_4086.g*float2(1,0));
                float4 _DisplacementR_var = tex2Dlod(_DisplacementR,float4(TRANSFORM_TEX(node_161, _DisplacementR),0.0,node_15));
                float node_156 = saturate((node_803.a-_DisplacementR_var.r));
                float3 normalLocal = normalize(lerp(float3(0,0,1),_Normals_var.rgb,node_156));
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = 10.0;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(i.uv0, _Diffuse));
                float4 node_798 = tex2D(_AORGBHeightA,TRANSFORM_TEX(node_6, _AORGBHeightA));
                float3 node_162 = lerp(float3(0.4117647,0.3826572,0.3602941),(_Diffuse_var.rgb*node_798.rgb),node_156);
                float node_165 = (node_156*node_162.r);
                float3 specularColor = float3(node_165,node_165,node_165);
                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow);
                float3 specular = directSpecular * specularColor;
/////// Diffuse:
                NdotL = dot( normalDirection, lightDirection );
                float node_237 = (0.5*(1.0 - node_156));
                float3 w = float3(node_237,node_237,node_237)*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                float3 directDiffuse = forwardLight * attenColor;
                float3 diffuse = directDiffuse * node_162;
/// Final Color:
                float3 finalColor = diffuse + specular;
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
            #include "Tessellation.cginc"
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
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_COLLECTOR;
                float2 uv0 : TEXCOORD5;
                float3 normalDir : TEXCOORD6;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(_Object2World, float4(v.normal,0)).xyz;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_SHADOW_COLLECTOR(o)
                return o;
            }
            #ifdef UNITY_CAN_COMPILE_TESSELLATION
                struct TessVertex {
                    float4 vertex : INTERNALTESSPOS;
                    float3 normal : NORMAL;
                    float4 tangent : TANGENT;
                    float2 texcoord0 : TEXCOORD0;
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
                    o.texcoord0 = v.texcoord0;
                    return o;
                }
                void displacement (inout VertexInput v){
                    float4 node_1826 = _Time + _TimeEditor;
                    float2 node_6 = (v.texcoord0*2.0);
                    float2 node_161 = (node_6+node_1826.g*float2(1,0));
                    float node_15 = 1.0;
                    float4 _DisplacementR_var = tex2Dlod(_DisplacementR,float4(TRANSFORM_TEX(node_161, _DisplacementR),0.0,node_15));
                    float4 node_803 = tex2Dlod(_AORGBHeightA,float4(TRANSFORM_TEX(node_6, _AORGBHeightA),0.0,node_15));
                    v.vertex.xyz += (v.normal*((1.0 - max(_DisplacementR_var.r,node_803.a))*((-1.0)*_Depth)));
                }
                float Tessellation(TessVertex v){
                    return 3.0;
                }
                float4 Tessellation(TessVertex v, TessVertex v1, TessVertex v2){
                    float tv = Tessellation(v);
                    float tv1 = Tessellation(v1);
                    float tv2 = Tessellation(v2);
                    return float4( tv1+tv2, tv2+tv, tv+tv1, tv+tv1+tv2 ) / float4(2,2,2,3);
                }
                OutputPatchConstant hullconst (InputPatch<TessVertex,3> v) {
                    OutputPatchConstant o;
                    float4 ts = Tessellation( v[0], v[1], v[2] );
                    o.edge[0] = ts.x;
                    o.edge[1] = ts.y;
                    o.edge[2] = ts.z;
                    o.inside = ts.w;
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
                    v.texcoord0 = vi[0].texcoord0*bary.x + vi[1].texcoord0*bary.y + vi[2].texcoord0*bary.z;
                    displacement(v);
                    VertexOutput o = vert(v);
                    return o;
                }
            #endif
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
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
            #include "Tessellation.cginc"
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
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(_Object2World, float4(v.normal,0)).xyz;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            #ifdef UNITY_CAN_COMPILE_TESSELLATION
                struct TessVertex {
                    float4 vertex : INTERNALTESSPOS;
                    float3 normal : NORMAL;
                    float4 tangent : TANGENT;
                    float2 texcoord0 : TEXCOORD0;
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
                    o.texcoord0 = v.texcoord0;
                    return o;
                }
                void displacement (inout VertexInput v){
                    float4 node_6947 = _Time + _TimeEditor;
                    float2 node_6 = (v.texcoord0*2.0);
                    float2 node_161 = (node_6+node_6947.g*float2(1,0));
                    float node_15 = 1.0;
                    float4 _DisplacementR_var = tex2Dlod(_DisplacementR,float4(TRANSFORM_TEX(node_161, _DisplacementR),0.0,node_15));
                    float4 node_803 = tex2Dlod(_AORGBHeightA,float4(TRANSFORM_TEX(node_6, _AORGBHeightA),0.0,node_15));
                    v.vertex.xyz += (v.normal*((1.0 - max(_DisplacementR_var.r,node_803.a))*((-1.0)*_Depth)));
                }
                float Tessellation(TessVertex v){
                    return 3.0;
                }
                float4 Tessellation(TessVertex v, TessVertex v1, TessVertex v2){
                    float tv = Tessellation(v);
                    float tv1 = Tessellation(v1);
                    float tv2 = Tessellation(v2);
                    return float4( tv1+tv2, tv2+tv, tv+tv1, tv+tv1+tv2 ) / float4(2,2,2,3);
                }
                OutputPatchConstant hullconst (InputPatch<TessVertex,3> v) {
                    OutputPatchConstant o;
                    float4 ts = Tessellation( v[0], v[1], v[2] );
                    o.edge[0] = ts.x;
                    o.edge[1] = ts.y;
                    o.edge[2] = ts.z;
                    o.inside = ts.w;
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
                    v.texcoord0 = vi[0].texcoord0*bary.x + vi[1].texcoord0*bary.y + vi[2].texcoord0*bary.z;
                    displacement(v);
                    VertexOutput o = vert(v);
                    return o;
                }
            #endif
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
