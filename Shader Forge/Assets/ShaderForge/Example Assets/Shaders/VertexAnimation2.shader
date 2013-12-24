// Shader created with Shader Forge Beta 0.16 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.16;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:2,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,uamb:True,mssp:True,ufog:True,aust:True,igpj:False,qofs:0,lico:1,qpre:1,flbk:,rntp:1,lmpd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,hqsc:True,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300;n:type:ShaderForge.SFN_Final,id:1,x:31696,y:32397|diff-149-OUT,spec-4921-OUT,normal-4935-OUT,transm-133-OUT,lwrap-133-OUT,voffset-140-OUT;n:type:ShaderForge.SFN_Subtract,id:18,x:33131,y:32978|A-22-OUT,B-19-OUT;n:type:ShaderForge.SFN_Vector1,id:19,x:33300,y:33049,v1:0.5;n:type:ShaderForge.SFN_Abs,id:21,x:33315,y:32623|IN-18-OUT;n:type:ShaderForge.SFN_Frac,id:22,x:33300,y:32924|IN-24-OUT;n:type:ShaderForge.SFN_Panner,id:23,x:33640,y:32924,spu:0.25,spv:0;n:type:ShaderForge.SFN_ComponentMask,id:24,x:33471,y:32924,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-23-UVOUT;n:type:ShaderForge.SFN_Multiply,id:25,x:33131,y:32623|A-21-OUT,B-26-OUT;n:type:ShaderForge.SFN_Vector1,id:26,x:33315,y:32751,v1:2;n:type:ShaderForge.SFN_Power,id:133,x:32784,y:32590,cmnt:Panning gradient|VAL-25-OUT,EXP-8547-OUT;n:type:ShaderForge.SFN_NormalVector,id:139,x:32519,y:32863,pt:False;n:type:ShaderForge.SFN_Multiply,id:140,x:32350,y:32814|A-143-OUT,B-139-OUT;n:type:ShaderForge.SFN_ValueProperty,id:142,x:32745,y:32828,ptlb:Scale,v1:0.2;n:type:ShaderForge.SFN_Multiply,id:143,x:32519,y:32735|A-133-OUT,B-142-OUT;n:type:ShaderForge.SFN_Lerp,id:149,x:32453,y:32056|A-151-RGB,B-166-OUT,T-133-OUT;n:type:ShaderForge.SFN_Lerp,id:150,x:32313,y:32421|A-267-RGB,B-265-OUT,T-133-OUT;n:type:ShaderForge.SFN_Tex2d,id:151,x:32756,y:31989,ptlb:Diffuse,tex:b66bceaf0cc0ace4e9bdc92f14bba709,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:166,x:32756,y:32148|A-213-OUT,B-133-OUT;n:type:ShaderForge.SFN_Color,id:168,x:33134,y:32027,ptlb:Glow Color,c1:1,c2:0.2391481,c3:0.1102941,c4:1;n:type:ShaderForge.SFN_Multiply,id:213,x:32942,y:32107|A-168-RGB,B-214-OUT;n:type:ShaderForge.SFN_ValueProperty,id:214,x:33134,y:32197,ptlb:Glow Intensity,v1:1.8;n:type:ShaderForge.SFN_Vector3,id:265,x:32512,y:32407,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Tex2d,id:267,x:32512,y:32254,ptlb:Normals,tex:bbab0a6f7bae9cf42bf057d8ee2755f6,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Vector1,id:4921,x:32139,y:32363,v1:1;n:type:ShaderForge.SFN_Normalize,id:4935,x:32139,y:32421|IN-150-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8547,x:33131,y:32775,ptlb:Bulge Shape,v1:5;proporder:142-151-168-214-267-8547;pass:END;sub:END;*/

Shader "Shader Forge/NewShader" {
    Properties {
        _Scale ("Scale", Float ) = 0.2
        _Diffuse ("Diffuse", 2D) = "white" {}
        _GlowColor ("Glow Color", Color) = (1,0.2391481,0.1102941,1)
        _GlowIntensity ("Glow Intensity", Float ) = 1.8
        _Normals ("Normals", 2D) = "bump" {}
        _BulgeShape ("Bulge Shape", Float ) = 5
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
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            uniform float _Scale;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float4 _GlowColor;
            uniform float _GlowIntensity;
            uniform sampler2D _Normals; uniform float4 _Normals_ST;
            uniform float _BulgeShape;
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
                float4 node_8652 = _Time + _TimeEditor;
                float2 node_8651 = o.uv0;
                float node_133 = pow((abs((frac((node_8651.rg+node_8652.g*float2(0.25,0)).r)-0.5))*2.0),_BulgeShape);
                v.vertex.xyz += ((node_133*_Scale)*v.normal);
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float2 node_8651 = i.uv0;
                float4 node_8652 = _Time + _TimeEditor;
                float node_133 = pow((abs((frac((node_8651.rg+node_8652.g*float2(0.25,0)).r)-0.5))*2.0),_BulgeShape);
                float3 normalLocal = normalize(lerp(UnpackNormal(tex2D(_Normals,TRANSFORM_TEX(node_8651.rg, _Normals))).rgb,float3(0,0,1),node_133));
                float3 normalDirection = normalize( mul( normalLocal, tangentTransform ) );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 w = float3(node_133,node_133,node_133)*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = pow( max(float3(0.0,0.0,0.0), NdotLWrap + w ), 1 );
                float3 backLight = pow( max(float3(0.0,0.0,0.0), -NdotLWrap + w ), 1 ) * float3(node_133,node_133,node_133);
                float3 diffuse = (forwardLight+backLight) * attenColor + UNITY_LIGHTMODEL_AMBIENT.xyz;
///////// Gloss:
                float gloss = exp2(0.5*10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float node_4921 = 1.0;
                float3 specularColor = float3(node_4921,node_4921,node_4921);
                float3 specular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),gloss) * specularColor;
                float3 finalColor = diffuse * lerp(tex2D(_Diffuse,TRANSFORM_TEX(node_8651.rg, _Diffuse)).rgb,((_GlowColor.rgb*_GlowIntensity)*node_133),node_133) + specular;
/// Final Color:
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
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
            uniform float4 _TimeEditor;
            uniform float _Scale;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float4 _GlowColor;
            uniform float _GlowIntensity;
            uniform sampler2D _Normals; uniform float4 _Normals_ST;
            uniform float _BulgeShape;
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
                float4 node_8654 = _Time + _TimeEditor;
                float2 node_8653 = o.uv0;
                float node_133 = pow((abs((frac((node_8653.rg+node_8654.g*float2(0.25,0)).r)-0.5))*2.0),_BulgeShape);
                v.vertex.xyz += ((node_133*_Scale)*v.normal);
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float2 node_8653 = i.uv0;
                float4 node_8654 = _Time + _TimeEditor;
                float node_133 = pow((abs((frac((node_8653.rg+node_8654.g*float2(0.25,0)).r)-0.5))*2.0),_BulgeShape);
                float3 normalLocal = normalize(lerp(UnpackNormal(tex2D(_Normals,TRANSFORM_TEX(node_8653.rg, _Normals))).rgb,float3(0,0,1),node_133));
                float3 normalDirection = normalize( mul( normalLocal, tangentTransform ) );
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 w = float3(node_133,node_133,node_133)*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = pow( max(float3(0.0,0.0,0.0), NdotLWrap + w ), 1 );
                float3 backLight = pow( max(float3(0.0,0.0,0.0), -NdotLWrap + w ), 1 ) * float3(node_133,node_133,node_133);
                float3 diffuse = (forwardLight+backLight) * attenColor;
///////// Gloss:
                float gloss = exp2(0.5*10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float node_4921 = 1.0;
                float3 specularColor = float3(node_4921,node_4921,node_4921);
                float3 specular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),gloss) * specularColor;
                float3 finalColor = diffuse * lerp(tex2D(_Diffuse,TRANSFORM_TEX(node_8653.rg, _Diffuse)).rgb,((_GlowColor.rgb*_GlowIntensity)*node_133),node_133) + specular;
/// Final Color:
                return fixed4(finalColor,1);
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
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform float _Scale;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float4 _GlowColor;
            uniform float _GlowIntensity;
            uniform sampler2D _Normals; uniform float4 _Normals_ST;
            uniform float _BulgeShape;
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
                float4 node_8656 = _Time + _TimeEditor;
                float2 node_8655 = o.uv0;
                float node_133 = pow((abs((frac((node_8655.rg+node_8656.g*float2(0.25,0)).r)-0.5))*2.0),_BulgeShape);
                v.vertex.xyz += ((node_133*_Scale)*v.normal);
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
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform float _Scale;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float4 _GlowColor;
            uniform float _GlowIntensity;
            uniform sampler2D _Normals; uniform float4 _Normals_ST;
            uniform float _BulgeShape;
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
                float4 node_8658 = _Time + _TimeEditor;
                float2 node_8657 = o.uv0;
                float node_133 = pow((abs((frac((node_8657.rg+node_8658.g*float2(0.25,0)).r)-0.5))*2.0),_BulgeShape);
                v.vertex.xyz += ((node_133*_Scale)*v.normal);
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
    CustomEditor "ShaderForgeMaterialInspector"
}
