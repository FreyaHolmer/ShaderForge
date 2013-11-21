// Shader created with Shader Forge Alpha 0.14 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.14;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:0,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,uamb:False,ufog:True,aust:True,igpj:False,qofs:0,lico:1,qpre:1,flbk:,rntp:1,lmpd:False;n:type:ShaderForge.SFN_Final,id:1,x:32327,y:32342|emission-56-OUT;n:type:ShaderForge.SFN_Slider,id:3,x:34245,y:31972,ptlb:Roughness,min:0.001,cur:0.8577143,max:1;n:type:ShaderForge.SFN_NormalVector,id:4,x:34156,y:32539,pt:False;n:type:ShaderForge.SFN_LightVector,id:5,x:34156,y:32682;n:type:ShaderForge.SFN_ViewVector,id:6,x:34156,y:32420;n:type:ShaderForge.SFN_Dot,id:7,x:33897,y:32453,dt:0|A-6-OUT,B-4-OUT;n:type:ShaderForge.SFN_Dot,id:8,x:33897,y:32630,dt:0|A-4-OUT,B-5-OUT;n:type:ShaderForge.SFN_Subtract,id:9,x:33545,y:32398|A-6-OUT,B-10-OUT;n:type:ShaderForge.SFN_Multiply,id:10,x:33723,y:32475|A-7-OUT,B-4-OUT;n:type:ShaderForge.SFN_Multiply,id:14,x:33723,y:32608|A-4-OUT,B-8-OUT;n:type:ShaderForge.SFN_Subtract,id:15,x:33545,y:32653|A-5-OUT,B-14-OUT;n:type:ShaderForge.SFN_Dot,id:16,x:33352,y:32508,dt:1|A-9-OUT,B-15-OUT;n:type:ShaderForge.SFN_ValueProperty,id:18,x:33352,y:32461,ptlb:Gamma,v1:0;n:type:ShaderForge.SFN_Multiply,id:19,x:34066,y:31962|A-3-OUT,B-3-OUT;n:type:ShaderForge.SFN_ValueProperty,id:21,x:34066,y:31915,ptlb:Rough Sq,v1:0;n:type:ShaderForge.SFN_OneMinus,id:22,x:33347,y:31820|IN-24-OUT;n:type:ShaderForge.SFN_Vector1,id:23,x:33711,y:31800,v1:0.5;n:type:ShaderForge.SFN_Multiply,id:24,x:33532,y:31820|A-23-OUT,B-27-OUT;n:type:ShaderForge.SFN_Add,id:25,x:33874,y:31816|A-26-OUT,B-19-OUT;n:type:ShaderForge.SFN_Vector1,id:26,x:34066,y:31784,v1:0.57;n:type:ShaderForge.SFN_Divide,id:27,x:33711,y:31868|A-19-OUT,B-25-OUT;n:type:ShaderForge.SFN_ValueProperty,id:29,x:33347,y:31760,ptlb:A,v1:0;n:type:ShaderForge.SFN_Multiply,id:30,x:33532,y:32075|A-35-OUT,B-31-OUT;n:type:ShaderForge.SFN_Vector1,id:31,x:33711,y:32189,v1:0.45;n:type:ShaderForge.SFN_Add,id:32,x:33874,y:32072|A-19-OUT,B-33-OUT;n:type:ShaderForge.SFN_Vector1,id:33,x:34066,y:32134,v1:0.09;n:type:ShaderForge.SFN_Divide,id:35,x:33711,y:32037|A-19-OUT,B-32-OUT;n:type:ShaderForge.SFN_ValueProperty,id:36,x:33532,y:32022,ptlb:B,v1:0;n:type:ShaderForge.SFN_ArcCos,id:38,x:33693,y:33001|IN-7-OUT;n:type:ShaderForge.SFN_ArcCos,id:39,x:33693,y:33209|IN-8-OUT;n:type:ShaderForge.SFN_Min,id:40,x:33503,y:33209|A-38-OUT,B-39-OUT;n:type:ShaderForge.SFN_Max,id:41,x:33503,y:33001|A-38-OUT,B-39-OUT;n:type:ShaderForge.SFN_ValueProperty,id:42,x:33160,y:33049,ptlb:C,v1:0;n:type:ShaderForge.SFN_Sin,id:44,x:33328,y:33001|IN-41-OUT;n:type:ShaderForge.SFN_Multiply,id:45,x:33160,y:33103|A-44-OUT,B-46-OUT;n:type:ShaderForge.SFN_Tan,id:46,x:33328,y:33209|IN-40-OUT;n:type:ShaderForge.SFN_Max,id:47,x:33328,y:32833|A-8-OUT,B-48-OUT;n:type:ShaderForge.SFN_Vector1,id:48,x:33509,y:32894,v1:0;n:type:ShaderForge.SFN_Color,id:49,x:32943,y:32629,ptlb:Diffuse,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:50,x:32765,y:32695|A-49-RGB,B-59-OUT;n:type:ShaderForge.SFN_Multiply,id:53,x:33143,y:32339|A-30-OUT,B-16-OUT;n:type:ShaderForge.SFN_Multiply,id:54,x:32963,y:32465|A-53-OUT,B-45-OUT;n:type:ShaderForge.SFN_Add,id:55,x:32769,y:32336|A-22-OUT,B-54-OUT;n:type:ShaderForge.SFN_Multiply,id:56,x:32601,y:32468|A-55-OUT,B-50-OUT;n:type:ShaderForge.SFN_LightAttenuation,id:57,x:33328,y:32709;n:type:ShaderForge.SFN_Multiply,id:58,x:33141,y:32779|A-57-OUT,B-47-OUT;n:type:ShaderForge.SFN_Multiply,id:59,x:32943,y:32837|A-58-OUT,B-61-RGB;n:type:ShaderForge.SFN_LightColor,id:61,x:33141,y:32902;pass:END;sub:END;*/

Shader "Shader Forge/Oren Nayar Approx" {
    Properties {
        _Roughness ("Roughness", Range(0.001, 1)) = 0.001
        _Diffuse ("Diffuse", Color) = (0.5,0.5,0.5,1)
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
            uniform float _Roughness;
            uniform float4 _Diffuse;
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
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = normalize(i.normalDir);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float attenuation = LIGHT_ATTENUATION(i);
                float node_3 = _Roughness;
                float node_19 = (node_3*node_3);
                float3 node_6 = viewDirection;
                float3 node_4 = i.normalDir;
                float node_7 = dot(node_6,node_4);
                float3 node_5 = lightDirection;
                float node_8 = dot(node_4,node_5);
                float node_38 = acos(node_7);
                float node_39 = acos(node_8);
                return fixed4((((1.0 - (0.5*(node_19/(0.57+node_19))))+((((node_19/(node_19+0.09))*0.45)*max(0,dot((node_6-(node_7*node_4)),(node_5-(node_4*node_8)))))*(sin(max(node_38,node_39))*tan(min(node_38,node_39)))))*(_Diffuse.rgb*((attenuation*max(node_8,0.0))*_LightColor0.rgb))),1);
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
            uniform float _Roughness;
            uniform float4 _Diffuse;
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
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = normalize(i.normalDir);
                float3 lightDirection;
                if (0.0 == _WorldSpaceLightPos0.w){
                    lightDirection = normalize( _WorldSpaceLightPos0.xyz );
                } else {
                    lightDirection = normalize( _WorldSpaceLightPos0 - i.posWorld.xyz );
                }
                float attenuation = LIGHT_ATTENUATION(i);
                float node_3 = _Roughness;
                float node_19 = (node_3*node_3);
                float3 node_6 = viewDirection;
                float3 node_4 = i.normalDir;
                float node_7 = dot(node_6,node_4);
                float3 node_5 = lightDirection;
                float node_8 = dot(node_4,node_5);
                float node_38 = acos(node_7);
                float node_39 = acos(node_8);
                return fixed4((((1.0 - (0.5*(node_19/(0.57+node_19))))+((((node_19/(node_19+0.09))*0.45)*max(0,dot((node_6-(node_7*node_4)),(node_5-(node_4*node_8)))))*(sin(max(node_38,node_39))*tan(min(node_38,node_39)))))*(_Diffuse.rgb*((attenuation*max(node_8,0.0))*_LightColor0.rgb))),1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
