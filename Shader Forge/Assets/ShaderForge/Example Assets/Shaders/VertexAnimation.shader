// Shader created with Shader Forge Beta 0.24 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.24;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:True,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,hqsc:True,hqlp:False,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0;n:type:ShaderForge.SFN_Final,id:1,x:31696,y:32397|diff-149-OUT,spec-4921-OUT,normal-4935-OUT,emission-166-OUT,transm-133-OUT,lwrap-133-OUT,voffset-140-OUT;n:type:ShaderForge.SFN_Subtract,id:18,x:33624,y:32681|A-22-OUT,B-19-OUT;n:type:ShaderForge.SFN_Vector1,id:19,x:33793,y:32752,v1:0.5;n:type:ShaderForge.SFN_Abs,id:21,x:33453,y:32681|IN-18-OUT;n:type:ShaderForge.SFN_Frac,id:22,x:33793,y:32627|IN-24-OUT;n:type:ShaderForge.SFN_Panner,id:23,x:34133,y:32627,spu:0.25,spv:0;n:type:ShaderForge.SFN_ComponentMask,id:24,x:33964,y:32627,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-23-UVOUT;n:type:ShaderForge.SFN_Multiply,id:25,x:33269,y:32681,cmnt:Triangle Wave|A-21-OUT,B-26-OUT;n:type:ShaderForge.SFN_Vector1,id:26,x:33453,y:32809,v1:2;n:type:ShaderForge.SFN_Power,id:133,x:33100,y:32681,cmnt:Panning gradient|VAL-25-OUT,EXP-8547-OUT;n:type:ShaderForge.SFN_NormalVector,id:139,x:32563,y:32954,pt:False;n:type:ShaderForge.SFN_Multiply,id:140,x:32344,y:32838|A-133-OUT,B-142-OUT,C-139-OUT;n:type:ShaderForge.SFN_ValueProperty,id:142,x:32563,y:32886,ptlb:Bulge Scale,v1:0.2;n:type:ShaderForge.SFN_Lerp,id:149,x:32439,y:32098|A-151-RGB,B-8608-OUT,T-133-OUT;n:type:ShaderForge.SFN_Lerp,id:150,x:32312,y:32379|A-267-RGB,B-265-OUT,T-133-OUT;n:type:ShaderForge.SFN_Tex2d,id:151,x:32756,y:31989,ptlb:Diffuse,tex:b66bceaf0cc0ace4e9bdc92f14bba709,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:166,x:32908,y:32498,cmnt:Glow|A-168-RGB,B-8677-OUT,C-133-OUT;n:type:ShaderForge.SFN_Color,id:168,x:33100,y:32412,ptlb:Glow Color,c1:1,c2:0.2391481,c3:0.1102941,c4:1;n:type:ShaderForge.SFN_Vector3,id:265,x:32631,y:32404,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Tex2d,id:267,x:32547,y:32244,ptlb:Normals,tex:bbab0a6f7bae9cf42bf057d8ee2755f6,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Vector1,id:4921,x:32213,y:32317,v1:1;n:type:ShaderForge.SFN_Normalize,id:4935,x:32138,y:32379|IN-150-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8547,x:33269,y:32833,ptlb:Bulge Shape,v1:5;n:type:ShaderForge.SFN_Vector1,id:8608,x:32620,y:32115,v1:0.1;n:type:ShaderForge.SFN_ValueProperty,id:8677,x:33100,y:32584,ptlb:Glow Intensity,v1:1.2;proporder:151-267-168-142-8547-8677;pass:END;sub:END;*/

Shader "Shader Forge/Examples/Vertex Animation" {
    Properties {
        _Diffuse ("Diffuse", 2D) = "white" {}
        _Normals ("Normals", 2D) = "bump" {}
        _GlowColor ("Glow Color", Color) = (1,0.2391481,0.1102941,1)
        _BulgeScale ("Bulge Scale", Float ) = 0.2
        _BulgeShape ("Bulge Shape", Float ) = 5
        _GlowIntensity ("Glow Intensity", Float ) = 1.2
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
            uniform float4 _TimeEditor;
            uniform float _BulgeScale;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float4 _GlowColor;
            uniform sampler2D _Normals; uniform float4 _Normals_ST;
            uniform float _BulgeShape;
            uniform float _GlowIntensity;
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
                float3 shLight : TEXCOORD7;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.shLight = ShadeSH9(float4(v.normal * unity_Scale.w,1)) * 0.5;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                float4 node_8699 = _Time + _TimeEditor;
                float2 node_8698 = o.uv0;
                float node_133 = pow((abs((frac((node_8698.rg+node_8699.g*float2(0.25,0)).r)-0.5))*2.0),_BulgeShape); // Panning gradient
                v.vertex.xyz += (node_133*_BulgeScale*v.normal);
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
                float2 node_8698 = i.uv0;
                float4 node_8699 = _Time + _TimeEditor;
                float node_133 = pow((abs((frac((node_8698.rg+node_8699.g*float2(0.25,0)).r)-0.5))*2.0),_BulgeShape); // Panning gradient
                float3 normalLocal = normalize(lerp(UnpackNormal(tex2D(_Normals,TRANSFORM_TEX(node_8698.rg, _Normals))).rgb,float3(0,0,1),node_133));
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 w = float3(node_133,node_133,node_133)*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                float3 backLight = max(float3(0.0,0.0,0.0), -NdotLWrap + w ) * float3(node_133,node_133,node_133);
                float3 diffuse = (forwardLight+backLight) * attenColor;
////// Emissive:
                float3 emissive = (_GlowColor.rgb*_GlowIntensity*node_133);
///////// Gloss:
                float gloss = exp2(0.5*10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float node_4921 = 1.0;
                float3 specularColor = float3(node_4921,node_4921,node_4921);
                float3 specular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),gloss) * specularColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                diffuseLight += i.shLight; // Per-Vertex Light Probes / Spherical harmonics
                float node_8608 = 0.1;
                finalColor += diffuseLight * lerp(tex2D(_Diffuse,TRANSFORM_TEX(node_8698.rg, _Diffuse)).rgb,float3(node_8608,node_8608,node_8608),node_133);
                finalColor += specular;
                finalColor += emissive;
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
            uniform float4 _TimeEditor;
            uniform float _BulgeScale;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float4 _GlowColor;
            uniform sampler2D _Normals; uniform float4 _Normals_ST;
            uniform float _BulgeShape;
            uniform float _GlowIntensity;
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
                float4 node_8701 = _Time + _TimeEditor;
                float2 node_8700 = o.uv0;
                float node_133 = pow((abs((frac((node_8700.rg+node_8701.g*float2(0.25,0)).r)-0.5))*2.0),_BulgeShape); // Panning gradient
                v.vertex.xyz += (node_133*_BulgeScale*v.normal);
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
                float2 node_8700 = i.uv0;
                float4 node_8701 = _Time + _TimeEditor;
                float node_133 = pow((abs((frac((node_8700.rg+node_8701.g*float2(0.25,0)).r)-0.5))*2.0),_BulgeShape); // Panning gradient
                float3 normalLocal = normalize(lerp(UnpackNormal(tex2D(_Normals,TRANSFORM_TEX(node_8700.rg, _Normals))).rgb,float3(0,0,1),node_133));
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 w = float3(node_133,node_133,node_133)*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                float3 backLight = max(float3(0.0,0.0,0.0), -NdotLWrap + w ) * float3(node_133,node_133,node_133);
                float3 diffuse = (forwardLight+backLight) * attenColor;
///////// Gloss:
                float gloss = exp2(0.5*10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float node_4921 = 1.0;
                float3 specularColor = float3(node_4921,node_4921,node_4921);
                float3 specular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),gloss) * specularColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                float node_8608 = 0.1;
                finalColor += diffuseLight * lerp(tex2D(_Diffuse,TRANSFORM_TEX(node_8700.rg, _Diffuse)).rgb,float3(node_8608,node_8608,node_8608),node_133);
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
            uniform float _BulgeScale;
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
                float4 node_8703 = _Time + _TimeEditor;
                float node_133 = pow((abs((frac((o.uv0.rg+node_8703.g*float2(0.25,0)).r)-0.5))*2.0),_BulgeShape); // Panning gradient
                v.vertex.xyz += (node_133*_BulgeScale*v.normal);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_SHADOW_COLLECTOR(o)
                return o;
            }
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
            uniform float _BulgeScale;
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
                float4 node_8705 = _Time + _TimeEditor;
                float node_133 = pow((abs((frac((o.uv0.rg+node_8705.g*float2(0.25,0)).r)-0.5))*2.0),_BulgeShape); // Panning gradient
                v.vertex.xyz += (node_133*_BulgeScale*v.normal);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
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
