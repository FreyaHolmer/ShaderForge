// Shader created with Shader Forge Alpha 0.14 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.14;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:0,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,uamb:False,ufog:True,aust:True,igpj:False,qofs:0,lico:1,qpre:1,flbk:,rntp:1,lmpd:False;n:type:ShaderForge.SFN_Final,id:1,x:31478,y:32185|emission-95-OUT;n:type:ShaderForge.SFN_Slider,id:3,x:34245,y:31972,ptlb:Roughness,min:0,cur:0.8577143,max:1;n:type:ShaderForge.SFN_NormalVector,id:4,x:34156,y:32539,pt:False;n:type:ShaderForge.SFN_LightVector,id:5,x:34156,y:32682;n:type:ShaderForge.SFN_ViewVector,id:6,x:34156,y:32420;n:type:ShaderForge.SFN_Dot,id:7,x:33897,y:32453,dt:0|A-6-OUT,B-4-OUT;n:type:ShaderForge.SFN_Dot,id:8,x:33897,y:32630,dt:0|A-4-OUT,B-5-OUT;n:type:ShaderForge.SFN_Subtract,id:9,x:33545,y:32398|A-6-OUT,B-10-OUT;n:type:ShaderForge.SFN_Multiply,id:10,x:33723,y:32475|A-7-OUT,B-4-OUT;n:type:ShaderForge.SFN_Multiply,id:14,x:33723,y:32608|A-4-OUT,B-8-OUT;n:type:ShaderForge.SFN_Subtract,id:15,x:33545,y:32653|A-5-OUT,B-14-OUT;n:type:ShaderForge.SFN_Dot,id:16,x:33352,y:32508,dt:0|A-9-OUT,B-15-OUT;n:type:ShaderForge.SFN_ValueProperty,id:18,x:33352,y:32461,ptlb:Gamma,v1:0;n:type:ShaderForge.SFN_Multiply,id:19,x:34066,y:31962|A-3-OUT,B-3-OUT;n:type:ShaderForge.SFN_ValueProperty,id:21,x:34066,y:31915,ptlb:Rough Sq,v1:0;n:type:ShaderForge.SFN_OneMinus,id:22,x:33347,y:31820|IN-24-OUT;n:type:ShaderForge.SFN_Vector1,id:23,x:33711,y:31800,v1:0.5;n:type:ShaderForge.SFN_Multiply,id:24,x:33532,y:31820|A-23-OUT,B-27-OUT;n:type:ShaderForge.SFN_Add,id:25,x:33874,y:31816|A-26-OUT,B-19-OUT;n:type:ShaderForge.SFN_Vector1,id:26,x:34066,y:31784,v1:0.33;n:type:ShaderForge.SFN_Divide,id:27,x:33711,y:31868|A-19-OUT,B-25-OUT;n:type:ShaderForge.SFN_ValueProperty,id:29,x:33347,y:31760,ptlb:C1,v1:0;n:type:ShaderForge.SFN_Multiply,id:30,x:33532,y:32075|A-35-OUT,B-31-OUT;n:type:ShaderForge.SFN_Vector1,id:31,x:33711,y:32189,v1:0.45;n:type:ShaderForge.SFN_Add,id:32,x:33874,y:32072|A-19-OUT,B-33-OUT;n:type:ShaderForge.SFN_Vector1,id:33,x:34066,y:32134,v1:0.09;n:type:ShaderForge.SFN_Divide,id:35,x:33711,y:32037|A-19-OUT,B-32-OUT;n:type:ShaderForge.SFN_ValueProperty,id:36,x:33532,y:32022,ptlb:C2,v1:0;n:type:ShaderForge.SFN_ArcCos,id:38,x:33653,y:33173|IN-7-OUT;n:type:ShaderForge.SFN_ArcCos,id:39,x:33653,y:33387|IN-8-OUT;n:type:ShaderForge.SFN_Min,id:40,x:33469,y:33387|A-38-OUT,B-39-OUT;n:type:ShaderForge.SFN_Max,id:41,x:33469,y:33173|A-38-OUT,B-39-OUT;n:type:ShaderForge.SFN_Max,id:47,x:33328,y:32833|A-8-OUT,B-48-OUT;n:type:ShaderForge.SFN_Vector1,id:48,x:33509,y:32867,v1:0;n:type:ShaderForge.SFN_Color,id:49,x:32943,y:32629,ptlb:Diffuse,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:50,x:32765,y:32695|A-49-RGB,B-59-OUT;n:type:ShaderForge.SFN_LightAttenuation,id:57,x:33328,y:32709;n:type:ShaderForge.SFN_Multiply,id:58,x:33141,y:32779|A-57-OUT,B-47-OUT;n:type:ShaderForge.SFN_Multiply,id:59,x:32943,y:32837|A-58-OUT,B-61-RGB;n:type:ShaderForge.SFN_LightColor,id:61,x:33141,y:32902;n:type:ShaderForge.SFN_Vector1,id:62,x:33272,y:33041,v1:0.125;n:type:ShaderForge.SFN_Multiply,id:63,x:33093,y:33079|A-62-OUT,B-35-OUT;n:type:ShaderForge.SFN_Multiply,id:64,x:32838,y:33105|A-63-OUT,B-65-OUT;n:type:ShaderForge.SFN_Power,id:65,x:32711,y:33506|VAL-73-OUT,EXP-77-OUT;n:type:ShaderForge.SFN_ValueProperty,id:67,x:33469,y:33123,ptlb:Alpha,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:68,x:33469,y:33332,ptlb:Beta,v1:0;n:type:ShaderForge.SFN_Pi,id:69,x:33274,y:33526;n:type:ShaderForge.SFN_Multiply,id:70,x:33102,y:33526|A-69-OUT,B-69-OUT;n:type:ShaderForge.SFN_Divide,id:73,x:32916,y:33480|A-75-OUT,B-70-OUT;n:type:ShaderForge.SFN_Vector1,id:74,x:33272,y:33387,v1:4;n:type:ShaderForge.SFN_Multiply,id:75,x:33102,y:33299|A-76-OUT,B-74-OUT;n:type:ShaderForge.SFN_Multiply,id:76,x:33272,y:33246|A-41-OUT,B-40-OUT;n:type:ShaderForge.SFN_Vector1,id:77,x:32916,y:33608,v1:2;n:type:ShaderForge.SFN_ValueProperty,id:79,x:32838,y:33059,ptlb:C3,v1:0;n:type:ShaderForge.SFN_Multiply,id:80,x:32482,y:32724|A-16-OUT,B-81-OUT;n:type:ShaderForge.SFN_Tan,id:81,x:32648,y:32816|IN-40-OUT;n:type:ShaderForge.SFN_Multiply,id:82,x:32297,y:32659|A-30-OUT,B-80-OUT;n:type:ShaderForge.SFN_ValueProperty,id:83,x:32297,y:32604,ptlb:A,v1:0;n:type:ShaderForge.SFN_OneMinus,id:84,x:32628,y:32330|IN-85-OUT;n:type:ShaderForge.SFN_Abs,id:85,x:32810,y:32330|IN-16-OUT;n:type:ShaderForge.SFN_Multiply,id:86,x:32446,y:33091|A-84-OUT,B-64-OUT;n:type:ShaderForge.SFN_Multiply,id:87,x:32133,y:33143|A-86-OUT,B-88-OUT;n:type:ShaderForge.SFN_Tan,id:88,x:32332,y:33248|IN-91-OUT;n:type:ShaderForge.SFN_Add,id:89,x:32684,y:33248|A-41-OUT,B-40-OUT;n:type:ShaderForge.SFN_Subtract,id:91,x:32512,y:33248|A-89-OUT,B-92-OUT;n:type:ShaderForge.SFN_Vector1,id:92,x:32684,y:33376,v1:2;n:type:ShaderForge.SFN_ValueProperty,id:94,x:32133,y:33083,ptlb:B,v1:0;n:type:ShaderForge.SFN_Multiply,id:95,x:31754,y:32309|A-50-OUT,B-98-OUT;n:type:ShaderForge.SFN_Add,id:97,x:31960,y:32991|A-82-OUT,B-87-OUT;n:type:ShaderForge.SFN_Add,id:98,x:31945,y:32490|A-22-OUT,B-97-OUT;pass:END;sub:END;*/

Shader "Shader Forge/Oren Nayar Complex" {
    Properties {
        _Roughness ("Roughness", Range(0, 1)) = 0
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
                float3 node_4 = i.normalDir;
                float3 node_5 = lightDirection;
                float node_8 = dot(node_4,node_5);
                float node_3 = _Roughness;
                float node_19 = (node_3*node_3);
                float node_35 = (node_19/(node_19+0.09));
                float3 node_6 = viewDirection;
                float node_7 = dot(node_6,node_4);
                float node_16 = dot((node_6-(node_7*node_4)),(node_5-(node_4*node_8)));
                float node_38 = acos(node_7);
                float node_39 = acos(node_8);
                float node_40 = min(node_38,node_39);
                float node_41 = max(node_38,node_39);
                float node_69 = 3.141592654;
                return fixed4(((_Diffuse.rgb*((attenuation*max(node_8,0.0))*_LightColor0.rgb))*((1.0 - (0.5*(node_19/(0.33+node_19))))+(((node_35*0.45)*(node_16*tan(node_40)))+(((1.0 - abs(node_16))*((0.125*node_35)*pow((((node_41*node_40)*4.0)/(node_69*node_69)),2.0)))*tan(((node_41+node_40)-2.0)))))),1);
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
                float3 node_4 = i.normalDir;
                float3 node_5 = lightDirection;
                float node_8 = dot(node_4,node_5);
                float node_3 = _Roughness;
                float node_19 = (node_3*node_3);
                float node_35 = (node_19/(node_19+0.09));
                float3 node_6 = viewDirection;
                float node_7 = dot(node_6,node_4);
                float node_16 = dot((node_6-(node_7*node_4)),(node_5-(node_4*node_8)));
                float node_38 = acos(node_7);
                float node_39 = acos(node_8);
                float node_40 = min(node_38,node_39);
                float node_41 = max(node_38,node_39);
                float node_69 = 3.141592654;
                return fixed4(((_Diffuse.rgb*((attenuation*max(node_8,0.0))*_LightColor0.rgb))*((1.0 - (0.5*(node_19/(0.33+node_19))))+(((node_35*0.45)*(node_16*tan(node_40)))+(((1.0 - abs(node_16))*((0.125*node_35)*pow((((node_41*node_40)*4.0)/(node_69*node_69)),2.0)))*tan(((node_41+node_40)-2.0)))))),1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
