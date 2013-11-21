// Shader created with Shader Forge Alpha 0.09 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.09;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:0,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,uamb:False,ufog:True,aust:True,igpj:False,qofs:0,lico:1,qpre:1;n:type:ShaderForge.SFN_Final,id:0,x:30542,y:32427|4-44-2,8-46-0;n:type:ShaderForge.SFN_LightVector,id:2,x:33518,y:32656;n:type:ShaderForge.SFN_LightAttenuation,id:4,x:33156,y:32384;n:type:ShaderForge.SFN_LightColor,id:5,x:32934,y:32384;n:type:ShaderForge.SFN_NormalVector,id:6,x:33514,y:32792;n:type:ShaderForge.SFN_Dot,id:10,x:33256,y:32566,dt:0|1-2-0,2-6-0;n:type:ShaderForge.SFN_Clamp01,id:11,x:33077,y:32486|1-10-0;n:type:ShaderForge.SFN_Multiply,id:13,x:32716,y:32391|1-5-0,2-19-0;n:type:ShaderForge.SFN_Multiply,id:19,x:32892,y:32455|1-4-0,2-11-0;n:type:ShaderForge.SFN_HalfVector,id:22,x:33518,y:33040;n:type:ShaderForge.SFN_Dot,id:23,x:33318,y:32866,dt:0|1-6-0,2-22-0;n:type:ShaderForge.SFN_Clamp01,id:24,x:33114,y:32858|1-23-0;n:type:ShaderForge.SFN_Power,id:25,x:32907,y:32858|1-24-0,2-74-0;n:type:ShaderForge.SFN_Vector1,id:27,x:33134,y:33067,v1:2;n:type:ShaderForge.SFN_Divide,id:28,x:32716,y:33067|1-30-0,2-29-0;n:type:ShaderForge.SFN_Vector1,id:29,x:32907,y:33171,v1:8;n:type:ShaderForge.SFN_Add,id:30,x:32907,y:33067|1-74-0,2-27-0;n:type:ShaderForge.SFN_Multiply,id:31,x:32509,y:32871|1-25-0,2-28-0;n:type:ShaderForge.SFN_Dot,id:33,x:32922,y:32762,dt:0|1-2-0,2-22-0;n:type:ShaderForge.SFN_Vector1,id:34,x:32806,y:32663,v1:1;n:type:ShaderForge.SFN_Subtract,id:35,x:32627,y:32627|1-34-0,2-33-0;n:type:ShaderForge.SFN_Power,id:36,x:32160,y:32682|1-35-0,2-37-0;n:type:ShaderForge.SFN_Vector1,id:37,x:32627,y:32792,v1:5;n:type:ShaderForge.SFN_Color,id:38,x:32408,y:32556,ptlb:Specular color,c1:0.1176471,c2:0.1176471,c3:0.1176471,c4:1;n:type:ShaderForge.SFN_Subtract,id:39,x:32160,y:32605|1-40-0,2-38-0;n:type:ShaderForge.SFN_Vector1,id:40,x:32408,y:32427,v1:1;n:type:ShaderForge.SFN_Add,id:41,x:31777,y:32455|1-38-0,2-42-0;n:type:ShaderForge.SFN_Multiply,id:42,x:31965,y:32525|1-39-0,2-36-0;n:type:ShaderForge.SFN_Multiply,id:43,x:31598,y:32558|1-41-0,2-61-0;n:type:ShaderForge.SFN_Tex2d,id:44,x:31432,y:32280,ptlb:Normal,tex:bbab0a6f7bae9cf42bf057d8ee2755f6;n:type:ShaderForge.SFN_Add,id:46,x:30880,y:32455|1-48-0,2-73-0;n:type:ShaderForge.SFN_Sqrt,id:47,x:32939,y:33630|1-58-0;n:type:ShaderForge.SFN_Multiply,id:48,x:32265,y:32269|1-76-2,2-13-0;n:type:ShaderForge.SFN_LightColor,id:49,x:31382,y:32762;n:type:ShaderForge.SFN_Multiply,id:50,x:31382,y:32558|1-43-0,2-72-0;n:type:ShaderForge.SFN_Pi,id:51,x:33805,y:33915;n:type:ShaderForge.SFN_Pi,id:52,x:33518,y:33447;n:type:ShaderForge.SFN_Vector1,id:53,x:33535,y:33482,v1:2;n:type:ShaderForge.SFN_Vector1,id:54,x:33821,y:33950,v1:4;n:type:ShaderForge.SFN_Divide,id:55,x:33343,y:33396|1-52-0,2-53-0;n:type:ShaderForge.SFN_Divide,id:56,x:33630,y:33864|1-51-0,2-54-0;n:type:ShaderForge.SFN_Multiply,id:57,x:33438,y:33797|1-74-0,2-56-0;n:type:ShaderForge.SFN_Add,id:58,x:33125,y:33584|1-55-0,2-57-0;n:type:ShaderForge.SFN_Vector1,id:59,x:32939,y:33516,v1:1;n:type:ShaderForge.SFN_Divide,id:60,x:32739,y:33615|1-59-0,2-47-0;n:type:ShaderForge.SFN_Multiply,id:61,x:32001,y:32830|1-19-0,2-31-0;n:type:ShaderForge.SFN_Multiply,id:62,x:32010,y:33114|1-19-0,2-67-0;n:type:ShaderForge.SFN_Multiply,id:63,x:32010,y:33295|1-65-0,2-67-0;n:type:ShaderForge.SFN_ViewVector,id:64,x:32604,y:33304;n:type:ShaderForge.SFN_Dot,id:65,x:32375,y:33261,dt:0|1-6-0,2-64-0;n:type:ShaderForge.SFN_Vector1,id:66,x:32739,y:33498,v1:1;n:type:ShaderForge.SFN_Subtract,id:67,x:32477,y:33675|1-66-0,2-60-0;n:type:ShaderForge.SFN_Add,id:68,x:31819,y:33221|1-62-0,2-60-0;n:type:ShaderForge.SFN_Multiply,id:69,x:31600,y:33280|1-68-0,2-70-0;n:type:ShaderForge.SFN_Add,id:70,x:31819,y:33383|1-63-0,2-60-0;n:type:ShaderForge.SFN_Vector1,id:71,x:31617,y:33203,v1:1;n:type:ShaderForge.SFN_Divide,id:72,x:31412,y:33234|1-71-0,2-69-0;n:type:ShaderForge.SFN_Multiply,id:73,x:31099,y:32525|1-50-0,2-49-0;n:type:ShaderForge.SFN_Slider,id:74,x:33900,y:33259,ptlb:Specular Power,min:0,cur:28.94737,max:350;n:type:ShaderForge.SFN_Tex2d,id:76,x:32611,y:32126,ptlb:Base;pass:END;sub:END;*/

Shader "Shader Forge/PBRTexture" {
    Properties {
        _Specularcolor ("Specular color", Color) = (0.1176471,0.1176471,0.1176471,1)
        _Normal ("Normal", 2D) = "bump" {}
        _SpecularPower ("Specular Power", Range(0, 350)) = 0
        _Base ("Base", 2D) = "white" {}
    }
    SubShader {
        Tags {
        }
        Pass {
            Tags {
                "LightMode"="ForwardBase"
            }
            
            CGPROGRAM
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma vertex vert
            #pragma fragment frag
            #pragma exclude_renderers flash gles xbox360 ps3 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _Specularcolor;
            uniform sampler2D _Normal;
            uniform float _SpecularPower;
            uniform sampler2D _Base;
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
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.uv0 = v.uv0;
                float3 normalDirection = mul(float4(v.normal,0), _World2Object).xyz;
                o.normalDir = normalDirection;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalLocal = UnpackNormal(tex2D(_Normal,i.uv0.xy)).rgb;
                float3 normalDirection = normalize( mul( normalLocal, tangentTransform ) );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 halfDirection = normalize(viewDirection+lightDirection);
                float attenuation = LIGHT_ATTENUATION(i);
                float3 node_2 = lightDirection;
                float3 node_6 = normalDirection;
                float node_19 = (attenuation*saturate(dot(node_2,node_6)));
                float4 node_38 = _Specularcolor;
                float3 node_22 = halfDirection;
                float node_74 = _SpecularPower;
                float node_60 = (1/sqrt(((3.141592654/2)+(node_74*(3.141592654/4)))));
                float node_67 = (1-node_60);
                return fixed4(((tex2D(_Base,i.uv0.xy).rgb*(_LightColor0.xyz*node_19))+((((node_38.rgb+((1-node_38.rgb)*pow((1-dot(node_2,node_22)),5)))*(node_19*(pow(saturate(dot(node_6,node_22)),node_74)*((node_74+2)/8))))*(1/(((node_19*node_67)+node_60)*((dot(node_6,viewDirection)*node_67)+node_60))))*_LightColor0.xyz)),1);
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
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma vertex vert
            #pragma fragment frag
            #pragma exclude_renderers flash gles xbox360 ps3 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _Specularcolor;
            uniform sampler2D _Normal;
            uniform float _SpecularPower;
            uniform sampler2D _Base;
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
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.uv0 = v.uv0;
                float3 normalDirection = mul(float4(v.normal,0), _World2Object).xyz;
                o.normalDir = normalDirection;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalLocal = UnpackNormal(tex2D(_Normal,i.uv0.xy)).rgb;
                float3 normalDirection = normalize( mul( normalLocal, tangentTransform ) );
                float3 lightDirection;
                if (0.0 == _WorldSpaceLightPos0.w){
                    lightDirection = normalize( _WorldSpaceLightPos0.xyz );
                } else {
                    lightDirection = normalize( _WorldSpaceLightPos0 - i.posWorld.xyz );
                }
                float3 halfDirection = normalize(viewDirection+lightDirection);
                float attenuation = LIGHT_ATTENUATION(i);
                float3 node_2 = lightDirection;
                float3 node_6 = normalDirection;
                float node_19 = (attenuation*saturate(dot(node_2,node_6)));
                float4 node_38 = _Specularcolor;
                float3 node_22 = halfDirection;
                float node_74 = _SpecularPower;
                float node_60 = (1/sqrt(((3.141592654/2)+(node_74*(3.141592654/4)))));
                float node_67 = (1-node_60);
                return fixed4(((tex2D(_Base,i.uv0.xy).rgb*(_LightColor0.xyz*node_19))+((((node_38.rgb+((1-node_38.rgb)*pow((1-dot(node_2,node_22)),5)))*(node_19*(pow(saturate(dot(node_6,node_22)),node_74)*((node_74+2)/8))))*(1/(((node_19*node_67)+node_60)*((dot(node_6,viewDirection)*node_67)+node_60))))*_LightColor0.xyz)),1);
            }
            ENDCG
        }
    }
    FallBack "Specular"
}
