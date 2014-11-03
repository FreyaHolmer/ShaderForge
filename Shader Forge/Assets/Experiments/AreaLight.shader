// Shader created with Shader Forge Beta 0.35 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.35;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:0,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.1280277,fgcg:0.1953466,fgcb:0.2352941,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32308,y:32601|normal-181-OUT,custl-8-OUT;n:type:ShaderForge.SFN_ViewReflectionVector,id:2,x:35363,y:32798;n:type:ShaderForge.SFN_Dot,id:5,x:33675,y:32919,dt:1|A-110-OUT,B-118-OUT;n:type:ShaderForge.SFN_LightAttenuation,id:7,x:32941,y:33188;n:type:ShaderForge.SFN_Multiply,id:8,x:32736,y:32990|A-40-OUT,B-9-RGB,C-7-OUT;n:type:ShaderForge.SFN_LightColor,id:9,x:32941,y:33061;n:type:ShaderForge.SFN_Power,id:10,x:33386,y:32998|VAL-5-OUT,EXP-16-OUT;n:type:ShaderForge.SFN_Slider,id:11,x:34762,y:33109,ptlb:Gloss,ptin:_Gloss,min:0,cur:0.593985,max:1;n:type:ShaderForge.SFN_Add,id:12,x:33935,y:33177|A-14-OUT,B-13-OUT;n:type:ShaderForge.SFN_Vector1,id:13,x:34090,y:33285,v1:1;n:type:ShaderForge.SFN_Multiply,id:14,x:34090,y:33136|A-419-OUT,B-15-OUT;n:type:ShaderForge.SFN_Vector1,id:15,x:34264,y:33209,v1:10;n:type:ShaderForge.SFN_Exp,id:16,x:33747,y:33177,et:1|IN-12-OUT;n:type:ShaderForge.SFN_VectorRejection,id:17,x:35132,y:32739,cmnt:Lp r vR|A-20-OUT,B-2-OUT;n:type:ShaderForge.SFN_LightPosition,id:18,x:35451,y:32101;n:type:ShaderForge.SFN_FragmentPosition,id:19,x:35451,y:32227;n:type:ShaderForge.SFN_Subtract,id:20,x:35178,y:32205,cmnt:Lpos Relative to Wpos|A-18-XYZ,B-19-XYZ;n:type:ShaderForge.SFN_Length,id:21,x:34932,y:32761|IN-17-OUT;n:type:ShaderForge.SFN_LightColor,id:23,x:35451,y:32352;n:type:ShaderForge.SFN_Multiply,id:24,x:35178,y:32371,cmnt:Light radius|A-23-A,B-25-OUT;n:type:ShaderForge.SFN_Vector1,id:25,x:35451,y:32477,v1:8;n:type:ShaderForge.SFN_Subtract,id:32,x:34191,y:32821|A-20-OUT,B-227-OUT;n:type:ShaderForge.SFN_Multiply,id:34,x:34727,y:32937,cmnt:Lp r vR clamped|A-433-OUT,B-24-OUT;n:type:ShaderForge.SFN_Normalize,id:35,x:34002,y:32821,cmnt:Modified Light Direction|IN-32-OUT;n:type:ShaderForge.SFN_NormalVector,id:36,x:33578,y:32707,pt:True;n:type:ShaderForge.SFN_Dot,id:39,x:33357,y:32656,dt:1|A-451-OUT,B-36-OUT;n:type:ShaderForge.SFN_Add,id:40,x:32941,y:32916|A-95-OUT,B-48-OUT;n:type:ShaderForge.SFN_Tex2d,id:41,x:32964,y:32467,ptlb:Normals,ptin:_Normals,tex:bbab0a6f7bae9cf42bf057d8ee2755f6,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Slider,id:47,x:33287,y:33175,ptlb:Spec,ptin:_Spec,min:0,cur:0.6278195,max:1;n:type:ShaderForge.SFN_Multiply,id:48,x:33192,y:32998|A-10-OUT,B-47-OUT;n:type:ShaderForge.SFN_VectorRejection,id:55,x:34899,y:32166,cmnt:Lp r vR|A-20-OUT,B-67-OUT;n:type:ShaderForge.SFN_Length,id:57,x:34748,y:32024|IN-55-OUT;n:type:ShaderForge.SFN_Subtract,id:61,x:33929,y:32334|A-20-OUT,B-219-OUT;n:type:ShaderForge.SFN_Multiply,id:65,x:34351,y:32171,cmnt:Lp r vR clamped|A-439-OUT,B-24-OUT;n:type:ShaderForge.SFN_NormalVector,id:67,x:35178,y:32029,pt:True;n:type:ShaderForge.SFN_Normalize,id:68,x:33695,y:32092|IN-61-OUT;n:type:ShaderForge.SFN_Tex2d,id:94,x:33357,y:32827,ptlb:Diffuse,ptin:_Diffuse,tex:b66bceaf0cc0ace4e9bdc92f14bba709,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:95,x:33126,y:32738|A-143-OUT,B-39-OUT,C-94-RGB;n:type:ShaderForge.SFN_Add,id:107,x:34002,y:32635|A-108-OUT,B-35-OUT;n:type:ShaderForge.SFN_ViewVector,id:108,x:34186,y:32563;n:type:ShaderForge.SFN_Normalize,id:110,x:33839,y:32657,cmnt:Half Dir|IN-107-OUT;n:type:ShaderForge.SFN_NormalVector,id:118,x:33906,y:32936,pt:True;n:type:ShaderForge.SFN_Slider,id:143,x:33357,y:32463,ptlb:Diffuse Contribution,ptin:_DiffuseContribution,min:0,cur:0.6278195,max:2;n:type:ShaderForge.SFN_Slider,id:173,x:32964,y:32665,ptlb:Normal Intensity,ptin:_NormalIntensity,min:0,cur:0.6278195,max:1;n:type:ShaderForge.SFN_Lerp,id:174,x:32767,y:32467|A-175-OUT,B-41-RGB,T-173-OUT;n:type:ShaderForge.SFN_Vector3,id:175,x:32964,y:32361,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Normalize,id:181,x:32597,y:32467|IN-174-OUT;n:type:ShaderForge.SFN_Step,id:218,x:34351,y:32021|A-57-OUT,B-24-OUT;n:type:ShaderForge.SFN_Lerp,id:219,x:34115,y:32107,cmnt:L pos B offset|A-65-OUT,B-55-OUT,T-218-OUT;n:type:ShaderForge.SFN_Step,id:226,x:34524,y:32769|A-21-OUT,B-24-OUT;n:type:ShaderForge.SFN_Lerp,id:227,x:34383,y:32877,cmnt:L pos B offset|A-34-OUT,B-17-OUT,T-226-OUT;n:type:ShaderForge.SFN_Multiply,id:259,x:34572,y:33149|A-11-OUT,B-260-R;n:type:ShaderForge.SFN_Tex2d,id:260,x:34762,y:33199,ptlb:Gloss map,ptin:_Glossmap,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Slider,id:418,x:34613,y:33472,ptlb:Gloss Override,ptin:_GlossOverride,min:0,cur:0.593985,max:1;n:type:ShaderForge.SFN_Lerp,id:419,x:34364,y:33311|A-259-OUT,B-420-OUT,T-418-OUT;n:type:ShaderForge.SFN_Vector1,id:420,x:34566,y:33364,v1:1;n:type:ShaderForge.SFN_Divide,id:433,x:34762,y:32649|A-17-OUT,B-21-OUT;n:type:ShaderForge.SFN_Divide,id:439,x:34552,y:31915|A-55-OUT,B-57-OUT;n:type:ShaderForge.SFN_LightVector,id:451,x:33632,y:32495;proporder:11-41-47-94-143-173-260-418;pass:END;sub:END;*/

Shader "Shader Forge/NewShader" {
    Properties {
        _Gloss ("Gloss", Range(0, 1)) = 0.593985
        _Normals ("Normals", 2D) = "bump" {}
        _Spec ("Spec", Range(0, 1)) = 0.6278195
        _Diffuse ("Diffuse", 2D) = "white" {}
        _DiffuseContribution ("Diffuse Contribution", Range(0, 2)) = 0.6278195
        _NormalIntensity ("Normal Intensity", Range(0, 1)) = 0.6278195
        _Glossmap ("Gloss map", 2D) = "white" {}
        _GlossOverride ("Gloss Override", Range(0, 1)) = 0.593985
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
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float _Gloss;
            uniform sampler2D _Normals; uniform float4 _Normals_ST;
            uniform float _Spec;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float _DiffuseContribution;
            uniform float _NormalIntensity;
            uniform sampler2D _Glossmap; uniform float4 _Glossmap_ST;
            uniform float _GlossOverride;
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
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float2 node_460 = i.uv0;
                float3 normalLocal = normalize(lerp(float3(0,0,1),UnpackNormal(tex2D(_Normals,TRANSFORM_TEX(node_460.rg, _Normals))).rgb,_NormalIntensity));
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 node_20 = (_WorldSpaceLightPos0.rgb-i.posWorld.rgb); // Lpos Relative to Wpos
                float3 node_2 = viewReflectDirection;
                float3 node_17 = (node_20 - (node_2 * dot(node_20,node_2)/dot(node_2,node_2))); // Lp r vR
                float node_21 = length(node_17);
                float node_24 = (_LightColor0.a*8.0); // Light radius
                float3 node_35 = normalize((node_20-lerp(((node_17/node_21)*node_24),node_17,step(node_21,node_24)))); // Modified Light Direction
                float3 finalColor = (((_DiffuseContribution*max(0,dot(lightDirection,normalDirection))*tex2D(_Diffuse,TRANSFORM_TEX(node_460.rg, _Diffuse)).rgb)+(pow(max(0,dot(normalize((viewDirection+node_35)),normalDirection)),exp2(((lerp((_Gloss*tex2D(_Glossmap,TRANSFORM_TEX(node_460.rg, _Glossmap)).r),1.0,_GlossOverride)*10.0)+1.0)))*_Spec))*_LightColor0.rgb*attenuation);
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
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float _Gloss;
            uniform sampler2D _Normals; uniform float4 _Normals_ST;
            uniform float _Spec;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float _DiffuseContribution;
            uniform float _NormalIntensity;
            uniform sampler2D _Glossmap; uniform float4 _Glossmap_ST;
            uniform float _GlossOverride;
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
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float2 node_461 = i.uv0;
                float3 normalLocal = normalize(lerp(float3(0,0,1),UnpackNormal(tex2D(_Normals,TRANSFORM_TEX(node_461.rg, _Normals))).rgb,_NormalIntensity));
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 node_20 = (_WorldSpaceLightPos0.rgb-i.posWorld.rgb); // Lpos Relative to Wpos
                float3 node_2 = viewReflectDirection;
                float3 node_17 = (node_20 - (node_2 * dot(node_20,node_2)/dot(node_2,node_2))); // Lp r vR
                float node_21 = length(node_17);
                float node_24 = (_LightColor0.a*8.0); // Light radius
                float3 node_35 = normalize((node_20-lerp(((node_17/node_21)*node_24),node_17,step(node_21,node_24)))); // Modified Light Direction
                float3 finalColor = (((_DiffuseContribution*max(0,dot(lightDirection,normalDirection))*tex2D(_Diffuse,TRANSFORM_TEX(node_461.rg, _Diffuse)).rgb)+(pow(max(0,dot(normalize((viewDirection+node_35)),normalDirection)),exp2(((lerp((_Gloss*tex2D(_Glossmap,TRANSFORM_TEX(node_461.rg, _Glossmap)).r),1.0,_GlossOverride)*10.0)+1.0)))*_Spec))*_LightColor0.rgb*attenuation);
/// Final Color:
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
