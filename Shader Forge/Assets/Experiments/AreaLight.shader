// Shader created with Shader Forge Beta 0.18 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.18;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:0,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,uamb:True,mssp:True,ufog:True,aust:True,igpj:False,qofs:0,lico:1,qpre:1,flbk:,rntp:1,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,hqsc:True,hqlp:False,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.1280277,fgcg:0.1953466,fgcb:0.2352941,fgca:1,fgde:0.01,fgrn:0,fgrf:300;n:type:ShaderForge.SFN_Final,id:1,x:32351,y:32697|normal-41-RGB,custl-8-OUT;n:type:ShaderForge.SFN_ViewReflectionVector,id:2,x:35178,y:32763;n:type:ShaderForge.SFN_Dot,id:5,x:33797,y:32880,dt:1|A-35-OUT,B-29-OUT;n:type:ShaderForge.SFN_LightAttenuation,id:7,x:32968,y:33047;n:type:ShaderForge.SFN_Multiply,id:8,x:32785,y:33014|A-40-OUT,B-7-OUT,C-9-RGB;n:type:ShaderForge.SFN_LightColor,id:9,x:32968,y:33172;n:type:ShaderForge.SFN_Power,id:10,x:33386,y:32998|VAL-5-OUT,EXP-16-OUT;n:type:ShaderForge.SFN_Slider,id:11,x:34264,y:33111,ptlb:Gloss,min:0,cur:0.593985,max:1;n:type:ShaderForge.SFN_Add,id:12,x:33935,y:33177|A-14-OUT,B-13-OUT;n:type:ShaderForge.SFN_Vector1,id:13,x:34090,y:33285,v1:1;n:type:ShaderForge.SFN_Multiply,id:14,x:34090,y:33136|A-11-OUT,B-15-OUT;n:type:ShaderForge.SFN_Vector1,id:15,x:34264,y:33186,v1:10;n:type:ShaderForge.SFN_Exp,id:16,x:33747,y:33177,et:1|IN-12-OUT;n:type:ShaderForge.SFN_VectorRejection,id:17,x:34936,y:32748,cmnt:Lp r vR|A-20-OUT,B-2-OUT;n:type:ShaderForge.SFN_LightPosition,id:18,x:35178,y:32506;n:type:ShaderForge.SFN_FragmentPosition,id:19,x:35178,y:32632;n:type:ShaderForge.SFN_Subtract,id:20,x:34936,y:32574,cmnt:Lpos Relative to Wpos|A-18-XYZ,B-19-XYZ;n:type:ShaderForge.SFN_Length,id:21,x:34597,y:32765|IN-17-OUT;n:type:ShaderForge.SFN_LightColor,id:23,x:34991,y:33132;n:type:ShaderForge.SFN_Multiply,id:24,x:34813,y:33173,cmnt:Light radius|A-23-A,B-25-OUT;n:type:ShaderForge.SFN_Vector1,id:25,x:34991,y:33257,v1:8;n:type:ShaderForge.SFN_If,id:27,x:34384,y:32896,cmnt:L pos B offset|A-21-OUT,B-24-OUT,GT-34-OUT,EQ-17-OUT,LT-17-OUT;n:type:ShaderForge.SFN_ViewReflectionVector,id:29,x:34007,y:32950;n:type:ShaderForge.SFN_Subtract,id:32,x:34191,y:32821|A-20-OUT,B-27-OUT;n:type:ShaderForge.SFN_Normalize,id:33,x:34936,y:32896|IN-17-OUT;n:type:ShaderForge.SFN_Multiply,id:34,x:34731,y:32936,cmnt:Lp r vR clamped|A-33-OUT,B-24-OUT;n:type:ShaderForge.SFN_Normalize,id:35,x:34002,y:32821,cmnt:Modified Light Direction|IN-32-OUT;n:type:ShaderForge.SFN_NormalVector,id:36,x:33517,y:32698,pt:True;n:type:ShaderForge.SFN_Dot,id:39,x:33296,y:32647,dt:1|A-68-OUT,B-36-OUT;n:type:ShaderForge.SFN_Add,id:40,x:32957,y:32825|A-95-OUT,B-48-OUT;n:type:ShaderForge.SFN_Tex2d,id:41,x:32785,y:32741,ptlb:Normals,tex:bbab0a6f7bae9cf42bf057d8ee2755f6,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Slider,id:47,x:33287,y:33175,ptlb:Spec,min:0,cur:0.6278195,max:1;n:type:ShaderForge.SFN_Multiply,id:48,x:33192,y:32998|A-10-OUT,B-47-OUT;n:type:ShaderForge.SFN_VectorRejection,id:55,x:34707,y:32169,cmnt:Lp r vR|A-20-OUT,B-67-OUT;n:type:ShaderForge.SFN_Length,id:57,x:34368,y:32186|IN-55-OUT;n:type:ShaderForge.SFN_If,id:59,x:34162,y:32292,cmnt:L pos B offset|A-57-OUT,B-24-OUT,GT-65-OUT,EQ-55-OUT,LT-55-OUT;n:type:ShaderForge.SFN_Subtract,id:61,x:33962,y:32242|A-20-OUT,B-59-OUT;n:type:ShaderForge.SFN_Normalize,id:63,x:34707,y:32317|IN-55-OUT;n:type:ShaderForge.SFN_Multiply,id:65,x:34502,y:32357,cmnt:Lp r vR clamped|A-63-OUT,B-24-OUT;n:type:ShaderForge.SFN_NormalVector,id:67,x:34916,y:32225,pt:True;n:type:ShaderForge.SFN_Normalize,id:68,x:33772,y:32242|IN-61-OUT;n:type:ShaderForge.SFN_Tex2d,id:94,x:33296,y:32818,ptlb:Diffuse,tex:b66bceaf0cc0ace4e9bdc92f14bba709,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:95,x:33126,y:32738|A-39-OUT,B-94-RGB;proporder:11-41-47-94;pass:END;sub:END;*/

Shader "Shader Forge/NewShader" {
    Properties {
        _Gloss ("Gloss", Range(0, 1)) = 0
        _Normals ("Normals", 2D) = "bump" {}
        _Spec ("Spec", Range(0, 1)) = 0
        _Diffuse ("Diffuse", 2D) = "white" {}
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
            fixed4 frag(VertexOutput i) : COLOR {
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float2 node_104 = i.uv0;
                float3 normalLocal = UnpackNormal(tex2D(_Normals,TRANSFORM_TEX(node_104.rg, _Normals))).rgb;
                float3 normalDirection = normalize( mul( normalLocal, tangentTransform ) );
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float3 node_20 = (_WorldSpaceLightPos0.rgb-i.posWorld.rgb);
                float3 node_67 = normalDirection;
                float3 node_55 = (node_20 - (node_67 * dot(node_20,node_67)/dot(node_67,node_67)));
                float node_24 = (_LightColor0.a*8.0);
                float node_59_if_leA = step(length(node_55),node_24);
                float node_59_if_leB = step(node_24,length(node_55));
                float3 node_2 = viewReflectDirection;
                float3 node_17 = (node_20 - (node_2 * dot(node_20,node_2)/dot(node_2,node_2)));
                float node_27_if_leA = step(length(node_17),node_24);
                float node_27_if_leB = step(node_24,length(node_17));
                float3 finalColor = (((max(0,dot(normalize((node_20-lerp((node_59_if_leA*node_55)+(node_59_if_leB*(normalize(node_55)*node_24)),node_55,node_59_if_leA*node_59_if_leB))),normalDirection))*tex2D(_Diffuse,TRANSFORM_TEX(node_104.rg, _Diffuse)).rgb)+(pow(max(0,dot(normalize((node_20-lerp((node_27_if_leA*node_17)+(node_27_if_leB*(normalize(node_17)*node_24)),node_17,node_27_if_leA*node_27_if_leB))),viewReflectDirection)),exp2(((_Gloss*10.0)+1.0)))*_Spec))*attenuation*_LightColor0.rgb);
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
            fixed4 frag(VertexOutput i) : COLOR {
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float2 node_105 = i.uv0;
                float3 normalLocal = UnpackNormal(tex2D(_Normals,TRANSFORM_TEX(node_105.rg, _Normals))).rgb;
                float3 normalDirection = normalize( mul( normalLocal, tangentTransform ) );
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float3 node_20 = (_WorldSpaceLightPos0.rgb-i.posWorld.rgb);
                float3 node_67 = normalDirection;
                float3 node_55 = (node_20 - (node_67 * dot(node_20,node_67)/dot(node_67,node_67)));
                float node_24 = (_LightColor0.a*8.0);
                float node_59_if_leA = step(length(node_55),node_24);
                float node_59_if_leB = step(node_24,length(node_55));
                float3 node_2 = viewReflectDirection;
                float3 node_17 = (node_20 - (node_2 * dot(node_20,node_2)/dot(node_2,node_2)));
                float node_27_if_leA = step(length(node_17),node_24);
                float node_27_if_leB = step(node_24,length(node_17));
                float3 finalColor = (((max(0,dot(normalize((node_20-lerp((node_59_if_leA*node_55)+(node_59_if_leB*(normalize(node_55)*node_24)),node_55,node_59_if_leA*node_59_if_leB))),normalDirection))*tex2D(_Diffuse,TRANSFORM_TEX(node_105.rg, _Diffuse)).rgb)+(pow(max(0,dot(normalize((node_20-lerp((node_27_if_leA*node_17)+(node_27_if_leB*(normalize(node_17)*node_24)),node_17,node_27_if_leA*node_27_if_leB))),viewReflectDirection)),exp2(((_Gloss*10.0)+1.0)))*_Spec))*attenuation*_LightColor0.rgb);
/// Final Color:
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
