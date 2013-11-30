// Shader created with Shader Forge Alpha 0.15 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.15;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:1,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,uamb:False,ufog:True,aust:True,igpj:False,qofs:0,lico:0,qpre:2,flbk:,rntp:3,lmpd:True,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True;n:type:ShaderForge.SFN_Final,id:1,x:32487,y:32427|diff-75-OUT,clip-49-A;n:type:ShaderForge.SFN_Tex2d,id:2,x:33366,y:32313,tex:28c7aad1372ff114b90d330f8a2dd938|UVIN-9-UVOUT,TEX-3-TEX;n:type:ShaderForge.SFN_Tex2dAsset,id:3,x:33533,y:32477,ptlb:Caustics,tex:28c7aad1372ff114b90d330f8a2dd938;n:type:ShaderForge.SFN_Tex2d,id:4,x:33363,y:32597,tex:28c7aad1372ff114b90d330f8a2dd938|UVIN-10-UVOUT,TEX-3-TEX;n:type:ShaderForge.SFN_Multiply,id:5,x:33199,y:32475|A-2-RGB,B-4-RGB;n:type:ShaderForge.SFN_Panner,id:9,x:33533,y:32313,spu:0.03,spv:0.07|UVIN-24-OUT,DIST-774-T;n:type:ShaderForge.SFN_Panner,id:10,x:33533,y:32597,spu:-0.04,spv:-0.05|UVIN-26-OUT,DIST-774-T;n:type:ShaderForge.SFN_TexCoord,id:23,x:33870,y:32428,uv:0;n:type:ShaderForge.SFN_Multiply,id:24,x:33701,y:32313|A-25-OUT,B-23-UVOUT;n:type:ShaderForge.SFN_ValueProperty,id:25,x:33870,y:32326,ptlb:Caustic Tile 1,v1:1.3;n:type:ShaderForge.SFN_Multiply,id:26,x:33695,y:32597|A-23-UVOUT,B-27-OUT;n:type:ShaderForge.SFN_ValueProperty,id:27,x:33870,y:32614,ptlb:Caustic Tile 2,v1:1;n:type:ShaderForge.SFN_Tex2d,id:49,x:33036,y:32570,ptlb:MainTex,tex:66321cc856b03e245ac41ed8a53e0ecc;n:type:ShaderForge.SFN_Multiply,id:72,x:33036,y:32427|A-73-OUT,B-5-OUT;n:type:ShaderForge.SFN_ValueProperty,id:73,x:33199,y:32406,ptlb:Caustic Power,v1:1;n:type:ShaderForge.SFN_Multiply,id:74,x:32871,y:32427|A-72-OUT,B-49-RGB;n:type:ShaderForge.SFN_Add,id:75,x:32710,y:32427|A-74-OUT,B-49-RGB;n:type:ShaderForge.SFN_Time,id:774,x:33695,y:32736;proporder:49-3-27-25-73;pass:END;sub:END;*/

Shader "Shader Forge/Cave M5 Alpha" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _Caustics ("Caustics", 2D) = "white" {}
        _CausticTile2 ("Caustic Tile 2", Float ) = 0
        _CausticTile1 ("Caustic Tile 1", Float ) = 0
        _CausticPower ("Caustic Power", Float ) = 0
    }
    SubShader {
        Tags {
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
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
            #include "Lighting.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers gles xbox360 ps3 flash 
            #pragma target 3.0
            #pragma glsl
            uniform float4 _TimeEditor;
            #ifndef LIGHTMAP_OFF
                sampler2D unity_Lightmap;
                float4 unity_LightmapST;
                #ifndef DIRLIGHTMAP_OFF
                    sampler2D unity_LightmapInd;
                #endif
            #endif
            uniform sampler2D _Caustics;
            uniform float _CausticTile1;
            uniform float _CausticTile2;
            uniform sampler2D _MainTex;
            uniform float _CausticPower;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
                #ifndef LIGHTMAP_OFF
                    float2 uvLM : TEXCOORD7;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                #ifndef LIGHTMAP_OFF
                    o.uvLM = v.uv1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                #endif
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float4 node_49 = tex2D(_MainTex,i.uv0.xy);
                clip(node_49.a - 0.5);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 normalDirection = normalize(i.normalDir);
                #ifndef LIGHTMAP_OFF
                    float4 lmtex = tex2D(unity_Lightmap,i.uvLM);
                    #ifndef DIRLIGHTMAP_OFF
                        float3 lightmap = DecodeLightmap(lmtex);
                        float3 scalePerBasisVector = DecodeLightmap(tex2D(unity_LightmapInd,i.uvLM));
                        UNITY_DIRBASIS
                        half3 normalInRnmBasis = saturate (mul (unity_DirBasis, float3(0,0,1)));
                        lightmap *= dot (normalInRnmBasis, scalePerBasisVector);
                    #else
                        float3 lightmap = DecodeLightmap(tex2D(unity_Lightmap,i.uvLM));
                    #endif
                #endif
                #ifndef LIGHTMAP_OFF
                    #ifdef DIRLIGHTMAP_OFF
                        float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                    #else
                        float3 lightDirection = normalize (scalePerBasisVector.x * unity_DirBasis[0] + scalePerBasisVector.y * unity_DirBasis[1] + scalePerBasisVector.z * unity_DirBasis[2]);
                        lightDirection = mul(lightDirection,tangentTransform); // Tangent to world
                    #endif
                #else
                    float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                #endif
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                #ifndef LIGHTMAP_OFF
                    float3 diffuse = lightmap;
                #else
                    float3 diffuse = max( 0.0, dot(normalDirection,lightDirection )) * attenColor;
                #endif
                float2 node_23 = i.uv0;
                float4 node_774 = _Time + _TimeEditor;
                float3 lightFinal = diffuse * (((_CausticPower*(tex2D(_Caustics,((_CausticTile1*node_23.rg)+node_774.g*float2(0.03,0.07))).rgb*tex2D(_Caustics,((node_23.rg*_CausticTile2)+node_774.g*float2(-0.04,-0.05))).rgb))*node_49.rgb)+node_49.rgb);
/// Final Color:
                return fixed4(lightFinal,1);
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
            #ifndef LIGHTMAP_OFF
                sampler2D unity_Lightmap;
                float4 unity_LightmapST;
                #ifndef DIRLIGHTMAP_OFF
                    sampler2D unity_LightmapInd;
                #endif
            #endif
            uniform sampler2D _Caustics;
            uniform float _CausticTile1;
            uniform float _CausticTile2;
            uniform sampler2D _MainTex;
            uniform float _CausticPower;
            struct VertexInput {
                float4 vertex : POSITION;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_COLLECTOR;
                float4 uv0 : TEXCOORD5;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_SHADOW_COLLECTOR(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float4 node_49 = tex2D(_MainTex,i.uv0.xy);
                clip(node_49.a - 0.5);
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
            #ifndef LIGHTMAP_OFF
                sampler2D unity_Lightmap;
                float4 unity_LightmapST;
                #ifndef DIRLIGHTMAP_OFF
                    sampler2D unity_LightmapInd;
                #endif
            #endif
            uniform sampler2D _Caustics;
            uniform float _CausticTile1;
            uniform float _CausticTile2;
            uniform sampler2D _MainTex;
            uniform float _CausticPower;
            struct VertexInput {
                float4 vertex : POSITION;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float4 uv0 : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float4 node_49 = tex2D(_MainTex,i.uv0.xy);
                clip(node_49.a - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
