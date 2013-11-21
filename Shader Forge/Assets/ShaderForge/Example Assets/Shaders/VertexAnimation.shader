// Shader created with Shader Forge Alpha 0.15 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.15;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:2,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,uamb:False,ufog:False,aust:True,igpj:False,qofs:0,lico:1,qpre:1,flbk:,rntp:1,lmpd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True;n:type:ShaderForge.SFN_Final,id:1,x:32464,y:31708|diff-85-RGB,spec-128-OUT,gloss-127-OUT,normal-75-OUT,emission-129-OUT,voffset-107-OUT;n:type:ShaderForge.SFN_NormalVector,id:18,x:32841,y:32697,pt:False;n:type:ShaderForge.SFN_Time,id:19,x:35085,y:32114;n:type:ShaderForge.SFN_Add,id:21,x:34719,y:32035|A-116-OUT,B-88-OUT;n:type:ShaderForge.SFN_Frac,id:22,x:34561,y:32134|IN-21-OUT;n:type:ShaderForge.SFN_Multiply,id:24,x:32657,y:32618|A-28-OUT,B-18-OUT;n:type:ShaderForge.SFN_Slider,id:27,x:33013,y:32532,ptlb:Magnitude,min:0,cur:0.1405983,max:1.4;n:type:ShaderForge.SFN_Multiply,id:28,x:32841,y:32561|A-27-OUT,B-55-OUT;n:type:ShaderForge.SFN_Subtract,id:43,x:34384,y:32134|A-22-OUT,B-44-OUT;n:type:ShaderForge.SFN_Vector1,id:44,x:34561,y:32266,v1:0.5;n:type:ShaderForge.SFN_Abs,id:49,x:34016,y:32134|IN-50-OUT;n:type:ShaderForge.SFN_Multiply,id:50,x:34193,y:32134|A-43-OUT,B-51-OUT;n:type:ShaderForge.SFN_Vector1,id:51,x:34384,y:32266,v1:2;n:type:ShaderForge.SFN_Subtract,id:55,x:33133,y:32745|A-56-OUT,B-60-OUT;n:type:ShaderForge.SFN_Multiply,id:56,x:33331,y:32697|A-58-OUT,B-57-OUT;n:type:ShaderForge.SFN_Power,id:57,x:33540,y:32822|VAL-64-OUT,EXP-59-OUT;n:type:ShaderForge.SFN_Vector1,id:58,x:33700,y:32690,v1:3;n:type:ShaderForge.SFN_Vector1,id:59,x:33674,y:32967,v1:2;n:type:ShaderForge.SFN_Multiply,id:60,x:33331,y:32822|A-59-OUT,B-61-OUT;n:type:ShaderForge.SFN_Power,id:61,x:33540,y:32697|VAL-64-OUT,EXP-58-OUT;n:type:ShaderForge.SFN_Power,id:64,x:33806,y:32195|VAL-49-OUT,EXP-86-OUT;n:type:ShaderForge.SFN_Tex2d,id:72,x:33266,y:31871,ptlb:Normals,tex:bbab0a6f7bae9cf42bf057d8ee2755f6|UVIN-121-OUT;n:type:ShaderForge.SFN_Lerp,id:73,x:33072,y:31957|A-72-RGB,B-108-RGB,T-79-OUT;n:type:ShaderForge.SFN_Normalize,id:75,x:32894,y:31945|IN-73-OUT;n:type:ShaderForge.SFN_Multiply,id:76,x:33439,y:32191|A-77-OUT,B-64-OUT;n:type:ShaderForge.SFN_Vector1,id:77,x:33611,y:32180,v1:1.5;n:type:ShaderForge.SFN_Clamp01,id:79,x:33266,y:32191|IN-76-OUT;n:type:ShaderForge.SFN_Tex2d,id:85,x:32894,y:31499,ptlb:Diffuse,tex:b66bceaf0cc0ace4e9bdc92f14bba709|UVIN-121-OUT;n:type:ShaderForge.SFN_Slider,id:86,x:34156,y:32412,ptlb:Sharpness,min:1,cur:3.052632,max:8;n:type:ShaderForge.SFN_Vector1,id:87,x:35085,y:32238,v1:0.25;n:type:ShaderForge.SFN_Multiply,id:88,x:34884,y:32114|A-19-T,B-87-OUT;n:type:ShaderForge.SFN_Vector3,id:95,x:32841,y:32439,v1:0,v2:1,v3:0;n:type:ShaderForge.SFN_Multiply,id:96,x:32657,y:32472|A-95-OUT,B-27-OUT;n:type:ShaderForge.SFN_Abs,id:99,x:33949,y:32314|IN-50-OUT;n:type:ShaderForge.SFN_Power,id:100,x:33753,y:32364|VAL-99-OUT,EXP-101-OUT;n:type:ShaderForge.SFN_Vector1,id:101,x:33960,y:32580,v1:2;n:type:ShaderForge.SFN_Add,id:104,x:32475,y:32472|A-96-OUT,B-24-OUT;n:type:ShaderForge.SFN_Multiply,id:107,x:32293,y:32408|A-100-OUT,B-104-OUT;n:type:ShaderForge.SFN_Tex2d,id:108,x:33266,y:32016,ptlb:Inner Normals,tex:cf20bfced7e912046a9ce991a4d775ec|UVIN-110-OUT;n:type:ShaderForge.SFN_TexCoord,id:109,x:35272,y:31835,uv:0;n:type:ShaderForge.SFN_Multiply,id:110,x:33564,y:31782|A-123-OUT,B-113-OUT;n:type:ShaderForge.SFN_Append,id:113,x:33736,y:31838|A-21-OUT,B-117-OUT;n:type:ShaderForge.SFN_Multiply,id:114,x:35085,y:31878|A-109-UVOUT,B-118-OUT;n:type:ShaderForge.SFN_ComponentMask,id:116,x:34884,y:31973,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-114-OUT;n:type:ShaderForge.SFN_ComponentMask,id:117,x:34884,y:31813,cc1:1,cc2:-1,cc3:-1,cc4:-1|IN-114-OUT;n:type:ShaderForge.SFN_Vector2,id:118,x:35272,y:31979,v1:2,v2:1;n:type:ShaderForge.SFN_TexCoord,id:120,x:33736,y:31602,uv:0;n:type:ShaderForge.SFN_Multiply,id:121,x:33564,y:31644|A-120-UVOUT,B-123-OUT;n:type:ShaderForge.SFN_Vector2,id:123,x:33736,y:31749,v1:16,v2:1;n:type:ShaderForge.SFN_ConstantLerp,id:127,x:32894,y:31796,a:4,b:300|IN-64-OUT;n:type:ShaderForge.SFN_ConstantLerp,id:128,x:32894,y:31647,a:0.1,b:1|IN-64-OUT;n:type:ShaderForge.SFN_Multiply,id:129,x:32894,y:32136|A-130-OUT,B-64-OUT;n:type:ShaderForge.SFN_Vector3,id:130,x:33072,y:32082,v1:2,v2:0.5,v3:0.1;proporder:85-72-108-27-86;pass:END;sub:END;*/

Shader "Shader Forge/Examples/VertexAnimation" {
    Properties {
        _Diffuse ("Diffuse", 2D) = "white" {}
        _Normals ("Normals", 2D) = "white" {}
        _InnerNormals ("Inner Normals", 2D) = "white" {}
        _Magnitude ("Magnitude", Range(0, 1.4)) = 0
        _Sharpness ("Sharpness", Range(1, 8)) = 1
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
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
            #pragma glsl
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            uniform float _Magnitude;
            uniform sampler2D _Normals;
            uniform sampler2D _Diffuse;
            uniform float _Sharpness;
            uniform sampler2D _InnerNormals;
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
                float2 node_114 = (o.uv0.rg*float2(2,1));
                float4 node_19 = _Time + _TimeEditor;
                float node_21 = (node_114.r+(node_19.g*0.25));
                float node_50 = ((frac(node_21)-0.5)*2.0);
                float node_27 = _Magnitude;
                float node_58 = 3.0;
                float node_64 = pow(abs(node_50),_Sharpness);
                float node_59 = 2.0;
                v.vertex.xyz += (pow(abs(node_50),2.0)*((float3(0,1,0)*node_27)+((node_27*((node_58*pow(node_64,node_59))-(node_59*pow(node_64,node_58))))*v.normal)));
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float2 node_123 = float2(16,1);
                float2 node_121 = (i.uv0.rg*node_123);
                float2 node_114 = (i.uv0.rg*float2(2,1));
                float4 node_19 = _Time + _TimeEditor;
                float node_21 = (node_114.r+(node_19.g*0.25));
                float node_50 = ((frac(node_21)-0.5)*2.0);
                float node_64 = pow(abs(node_50),_Sharpness);
                float3 normalLocal = normalize(lerp(UnpackNormal(tex2D(_Normals,node_121)).rgb,UnpackNormal(tex2D(_InnerNormals,(node_123*float2(node_21,node_114.g)))).rgb,saturate((1.5*node_64))));
                float3 normalDirection = normalize( mul( normalLocal, tangentTransform ) );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 halfDirection = normalize(viewDirection+lightDirection);
//////// DEBUG - Lighting()
                float3 attenuation = LIGHT_ATTENUATION(i) * _LightColor0.xyz;
//////// DEBUG - CalcDiffuse()
                float3 diffuse = max( 0.0, dot(normalDirection,lightDirection )) * attenuation;
//////// DEBUG - CalcEmissive()
                float3 emissive = (float3(2,0.5,0.1)*node_64);
                float gloss = exp2(lerp(4,300,node_64)*10.0+1.0);
//////// DEBUG - CalcSpecular()
                float node_128 = lerp(0.1,1,node_64);
                float3 specular = attenuation * float3(node_128,node_128,node_128) * pow(max(0,dot(halfDirection,normalDirection)),gloss);
                float3 lightFinal = diffuse;
                return fixed4(lightFinal * tex2D(_Diffuse,node_121).rgb + specular + emissive,1);
                return fixed4(lightFinal * tex2D(_Diffuse,node_121).rgb,1);
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
            #pragma glsl
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            uniform float _Magnitude;
            uniform sampler2D _Normals;
            uniform sampler2D _Diffuse;
            uniform float _Sharpness;
            uniform sampler2D _InnerNormals;
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
                float2 node_114 = (o.uv0.rg*float2(2,1));
                float4 node_19 = _Time + _TimeEditor;
                float node_21 = (node_114.r+(node_19.g*0.25));
                float node_50 = ((frac(node_21)-0.5)*2.0);
                float node_27 = _Magnitude;
                float node_58 = 3.0;
                float node_64 = pow(abs(node_50),_Sharpness);
                float node_59 = 2.0;
                v.vertex.xyz += (pow(abs(node_50),2.0)*((float3(0,1,0)*node_27)+((node_27*((node_58*pow(node_64,node_59))-(node_59*pow(node_64,node_58))))*v.normal)));
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float2 node_123 = float2(16,1);
                float2 node_121 = (i.uv0.rg*node_123);
                float2 node_114 = (i.uv0.rg*float2(2,1));
                float4 node_19 = _Time + _TimeEditor;
                float node_21 = (node_114.r+(node_19.g*0.25));
                float node_50 = ((frac(node_21)-0.5)*2.0);
                float node_64 = pow(abs(node_50),_Sharpness);
                float3 normalLocal = normalize(lerp(UnpackNormal(tex2D(_Normals,node_121)).rgb,UnpackNormal(tex2D(_InnerNormals,(node_123*float2(node_21,node_114.g)))).rgb,saturate((1.5*node_64))));
                float3 normalDirection = normalize( mul( normalLocal, tangentTransform ) );
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 halfDirection = normalize(viewDirection+lightDirection);
//////// DEBUG - Lighting()
                float3 attenuation = LIGHT_ATTENUATION(i) * _LightColor0.xyz;
//////// DEBUG - CalcDiffuse()
                float3 diffuse = max( 0.0, dot(normalDirection,lightDirection )) * attenuation;
                float gloss = exp2(lerp(4,300,node_64)*10.0+1.0);
//////// DEBUG - CalcSpecular()
                float node_128 = lerp(0.1,1,node_64);
                float3 specular = attenuation * float3(node_128,node_128,node_128) * pow(max(0,dot(halfDirection,normalDirection)),gloss);
                float3 lightFinal = diffuse;
                return fixed4(lightFinal * tex2D(_Diffuse,node_121).rgb + specular,1);
                return fixed4(lightFinal * tex2D(_Diffuse,node_121).rgb,1);
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
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCOLLECTOR
            #define SHADOW_COLLECTOR_PASS
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcollector
            #pragma exclude_renderers gles xbox360 ps3 flash 
            #pragma target 3.0
            #pragma glsl
            uniform float4 _TimeEditor;
            uniform float _Magnitude;
            uniform sampler2D _Normals;
            uniform sampler2D _Diffuse;
            uniform float _Sharpness;
            uniform sampler2D _InnerNormals;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
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
                float2 node_114 = (o.uv0.rg*float2(2,1));
                float4 node_19 = _Time + _TimeEditor;
                float node_21 = (node_114.r+(node_19.g*0.25));
                float node_50 = ((frac(node_21)-0.5)*2.0);
                float node_27 = _Magnitude;
                float node_58 = 3.0;
                float node_64 = pow(abs(node_50),_Sharpness);
                float node_59 = 2.0;
                v.vertex.xyz += (pow(abs(node_50),2.0)*((float3(0,1,0)*node_27)+((node_27*((node_58*pow(node_64,node_59))-(node_59*pow(node_64,node_58))))*v.normal)));
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_SHADOW_COLLECTOR(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
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
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma exclude_renderers gles xbox360 ps3 flash 
            #pragma target 3.0
            #pragma glsl
            uniform float4 _TimeEditor;
            uniform float _Magnitude;
            uniform sampler2D _Normals;
            uniform sampler2D _Diffuse;
            uniform float _Sharpness;
            uniform sampler2D _InnerNormals;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
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
                float2 node_114 = (o.uv0.rg*float2(2,1));
                float4 node_19 = _Time + _TimeEditor;
                float node_21 = (node_114.r+(node_19.g*0.25));
                float node_50 = ((frac(node_21)-0.5)*2.0);
                float node_27 = _Magnitude;
                float node_58 = 3.0;
                float node_64 = pow(abs(node_50),_Sharpness);
                float node_59 = 2.0;
                v.vertex.xyz += (pow(abs(node_50),2.0)*((float3(0,1,0)*node_27)+((node_27*((node_58*pow(node_64,node_59))-(node_59*pow(node_64,node_58))))*v.normal)));
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
