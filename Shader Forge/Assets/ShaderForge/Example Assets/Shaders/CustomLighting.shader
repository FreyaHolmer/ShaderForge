// Shader created with Shader Forge Beta 0.34 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.34;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:0,uamb:True,mssp:True,lmpd:False,lprd:True,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:0,x:32136,y:31953|normal-83-RGB,custl-64-OUT,olwid-255-OUT;n:type:ShaderForge.SFN_LightAttenuation,id:37,x:32583,y:32026;n:type:ShaderForge.SFN_Dot,id:40,x:33524,y:32250,dt:1|A-42-OUT,B-41-OUT;n:type:ShaderForge.SFN_NormalVector,id:41,x:33733,y:32344,pt:True;n:type:ShaderForge.SFN_LightVector,id:42,x:33733,y:32223;n:type:ShaderForge.SFN_Dot,id:52,x:33524,y:32423,dt:1|A-41-OUT,B-62-OUT;n:type:ShaderForge.SFN_Add,id:55,x:32583,y:32288|A-84-OUT,B-187-RGB,C-265-OUT;n:type:ShaderForge.SFN_Power,id:58,x:33322,y:32523,cmnt:Specular Light|VAL-52-OUT,EXP-244-OUT;n:type:ShaderForge.SFN_HalfVector,id:62,x:33733,y:32483;n:type:ShaderForge.SFN_LightColor,id:63,x:32583,y:32155;n:type:ShaderForge.SFN_Multiply,id:64,x:32399,y:32155|A-37-OUT,B-63-RGB,C-55-OUT;n:type:ShaderForge.SFN_Color,id:80,x:33087,y:32178,ptlb:Color,ptin:_Color,glob:False,c1:0.6544118,c2:0.8426978,c3:1,c4:1;n:type:ShaderForge.SFN_Tex2d,id:82,x:33087,y:32002,ptlb:Diffuse,ptin:_Diffuse,tex:8993b617f08498f43adcbd90697f1c5d,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:83,x:32399,y:31966,ptlb:Normals,ptin:_Normals,tex:c6dfb00dbee6bc044a8a3bb22e56e064,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Multiply,id:84,x:32882,y:32160,cmnt:Diffuse Light|A-82-RGB,B-80-RGB,C-264-OUT;n:type:ShaderForge.SFN_AmbientLight,id:187,x:32882,y:32280;n:type:ShaderForge.SFN_ValueProperty,id:216,x:33322,y:32423,ptlb:Bands,ptin:_Bands,glob:False,v1:8;n:type:ShaderForge.SFN_Slider,id:239,x:34314,y:32591,ptlb:Gloss,ptin:_Gloss,min:0,cur:0.4511278,max:1;n:type:ShaderForge.SFN_Add,id:240,x:33733,y:32640|A-242-OUT,B-241-OUT;n:type:ShaderForge.SFN_Vector1,id:241,x:33901,y:32728,v1:1;n:type:ShaderForge.SFN_Multiply,id:242,x:33901,y:32578|A-239-OUT,B-243-OUT;n:type:ShaderForge.SFN_Vector1,id:243,x:34314,y:32661,v1:10;n:type:ShaderForge.SFN_Exp,id:244,x:33562,y:32640,et:1|IN-240-OUT;n:type:ShaderForge.SFN_Vector1,id:255,x:32399,y:32288,v1:0.05;n:type:ShaderForge.SFN_Posterize,id:264,x:33087,y:32344|IN-40-OUT,STPS-216-OUT;n:type:ShaderForge.SFN_Posterize,id:265,x:33087,y:32475|IN-58-OUT,STPS-216-OUT;proporder:80-82-83-216-239;pass:END;sub:END;*/

Shader "Shader Forge/Examples/Custom Lighting" {
    Properties {
        _Color ("Color", Color) = (0.6544118,0.8426978,1,1)
        _Diffuse ("Diffuse", 2D) = "white" {}
        _Normals ("Normals", 2D) = "bump" {}
        _Bands ("Bands", Float ) = 8
        _Gloss ("Gloss", Range(0, 1)) = 0.4511278
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "Outline"
            Tags {
            }
            Cull Front
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma exclude_renderers gles xbox360 ps3 flash 
            #pragma target 3.0
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.pos = mul(UNITY_MATRIX_MVP, float4(v.vertex.xyz + v.normal*0.05,1));
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                return fixed4(float3(0,0,0),0);
            }
            ENDCG
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            Fog {Mode Off}
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
            uniform float4 _Color;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform sampler2D _Normals; uniform float4 _Normals_ST;
            uniform float _Bands;
            uniform float _Gloss;
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
                float3 shLight : TEXCOORD7;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.shLight = ShadeSH9(float4(mul(_Object2World, float4(v.normal,0)).xyz * unity_Scale.w,1)) * 0.5;
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
                float2 node_9091 = i.uv0;
                float3 normalLocal = UnpackNormal(tex2D(_Normals,TRANSFORM_TEX(node_9091.rg, _Normals))).rgb;
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 node_41 = normalDirection;
                float3 finalColor = (attenuation*_LightColor0.rgb*((tex2D(_Diffuse,TRANSFORM_TEX(node_9091.rg, _Diffuse)).rgb*_Color.rgb*floor(max(0,dot(lightDirection,node_41)) * _Bands) / (_Bands - 1))+UNITY_LIGHTMODEL_AMBIENT.rgb+floor(pow(max(0,dot(node_41,halfDirection)),exp2(((_Gloss*10.0)+1.0))) * _Bands) / (_Bands - 1)));
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
            #pragma exclude_renderers gles xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _Color;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform sampler2D _Normals; uniform float4 _Normals_ST;
            uniform float _Bands;
            uniform float _Gloss;
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
                float2 node_9092 = i.uv0;
                float3 normalLocal = UnpackNormal(tex2D(_Normals,TRANSFORM_TEX(node_9092.rg, _Normals))).rgb;
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 node_41 = normalDirection;
                float3 finalColor = (attenuation*_LightColor0.rgb*((tex2D(_Diffuse,TRANSFORM_TEX(node_9092.rg, _Diffuse)).rgb*_Color.rgb*floor(max(0,dot(lightDirection,node_41)) * _Bands) / (_Bands - 1))+UNITY_LIGHTMODEL_AMBIENT.rgb+floor(pow(max(0,dot(node_41,halfDirection)),exp2(((_Gloss*10.0)+1.0))) * _Bands) / (_Bands - 1)));
/// Final Color:
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
