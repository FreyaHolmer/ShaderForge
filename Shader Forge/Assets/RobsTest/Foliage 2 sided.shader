// Shader created with Shader Forge Alpha 0.13 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.13;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:1,blpr:0,bsrc:3,bdst:7,culm:2,dpts:2,wrdp:True,uamb:False,ufog:True,aust:True,igpj:False,qofs:0,lico:0,qpre:2,flbk:,rntp:3,lmpd:True;n:type:ShaderForge.SFN_Final,id:1,x:32803,y:32862|0-2-2,4-14-0,6-4-0,12-24-0;n:type:ShaderForge.SFN_Tex2d,id:2,x:33221,y:32866,ptlb:Diffuse;n:type:ShaderForge.SFN_Multiply,id:4,x:33021,y:32982|1-2-6,2-7-0;n:type:ShaderForge.SFN_Slider,id:7,x:33197,y:33020,ptlb:Alpha Clip,min:1,cur:1,max:2;n:type:ShaderForge.SFN_Vector3,id:14,x:33021,y:32898,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Sin,id:16,x:33438,y:33091|1-23-0;n:type:ShaderForge.SFN_FragmentPosition,id:17,x:34390,y:33010;n:type:ShaderForge.SFN_Multiply,id:18,x:34172,y:33076|1-17-0,2-19-0;n:type:ShaderForge.SFN_Vector1,id:19,x:34390,y:33151,v1:0.5;n:type:ShaderForge.SFN_Rotator,id:20,x:33804,y:33081|1-22-0,3-21-0;n:type:ShaderForge.SFN_Vector1,id:21,x:33987,y:33243,v1:4.5;n:type:ShaderForge.SFN_ComponentMask,id:22,x:33987,y:33081,cc1:0,cc2:2,cc3:4,cc4:4|1-18-0;n:type:ShaderForge.SFN_Panner,id:23,x:33621,y:33081,spu:1.26,spv:0|1-20-0;n:type:ShaderForge.SFN_ComponentMask,id:24,x:33236,y:33151,cc1:0,cc2:4,cc3:4,cc4:4|1-16-0;pass:END;sub:END;*/

Shader "Transparent/Foliage 2 Sided" {
    Properties {
        _Diffuse ("Diffuse", 2D) = "white" {}
        _AlphaClip ("Alpha Clip", Range(1, 2)) = 1
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
            Cull Off
            
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
            uniform float4 _TimeEditor;
            #ifndef LIGHTMAP_OFF
                sampler2D unity_Lightmap;
                float4 unity_LightmapST;
                #ifndef DIRLIGHTMAP_OFF
                    sampler2D unity_LightmapInd;
                #endif
            #endif
            uniform sampler2D _Diffuse;
            uniform float _AlphaClip;
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
                float node_20_ang = 4.5;
                float node_20_spd = 1.0;
                float node_20_cos = cos(node_20_spd*node_20_ang);
                float node_20_sin = sin(node_20_spd*node_20_ang);
                float2 node_20_piv = float2(0.5,0.5);
                float2 node_20 = (mul((mul(UNITY_MATRIX_MVP, v.vertex).rgb*0.5).rb-node_20_piv,float2x2( node_20_cos, -node_20_sin, node_20_sin, node_20_cos))+node_20_piv);
                v.vertex.xyz += float3(sin((node_20+_TimeEditor.y*float2(1.26,0))).r);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.uv0 = v.uv0;
                #ifndef LIGHTMAP_OFF
                    o.uvLM = v.uv1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                #endif
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float4 node_2 = tex2D(_Diffuse,i.uv0.xy);
                clip((node_2.a*_AlphaClip) - 0.5);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalLocal = float3(0,0,1);
                float3 normalDirection = normalize( mul( normalLocal, tangentTransform ) );
                
                float nSign = sign( dot( viewDirection, normalDirection ) ); // Reverse normal if this is a backface
                i.normalDir *= nSign;
                normalDirection *= nSign;
                
                #ifndef LIGHTMAP_OFF
                    float4 lmtex = tex2D(unity_Lightmap,i.uvLM);
                    #ifndef DIRLIGHTMAP_OFF
                        float3 lightmap = DecodeLightmap(lmtex);
                        float3 scalePerBasisVector = DecodeLightmap(tex2D(unity_LightmapInd,i.uvLM));
                        UNITY_DIRBASIS
                        half3 normalInRnmBasis = saturate (mul (unity_DirBasis, normalLocal));
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
                float attenuation = LIGHT_ATTENUATION(i);
                #ifndef LIGHTMAP_OFF
                    float3 lambert = lightmap;
                #else
                    float3 lambert = attenuation * max( 0.0, dot(normalDirection,lightDirection )) * _LightColor0.xyz;
                #endif
                float3 lightFinal = lambert;
                return fixed4(lightFinal * node_2.rgb,1);
            }
            ENDCG
        }
        Pass {
            Name "ShadowCollector"
            Tags {
                "LightMode"="ShadowCollector"
            }
            Cull Off
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
            uniform float4 _TimeEditor;
            #ifndef LIGHTMAP_OFF
                sampler2D unity_Lightmap;
                float4 unity_LightmapST;
                #ifndef DIRLIGHTMAP_OFF
                    sampler2D unity_LightmapInd;
                #endif
            #endif
            uniform sampler2D _Diffuse;
            uniform float _AlphaClip;
            struct VertexInput {
                float4 vertex : POSITION;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_COLLECTOR;
                float4 uv0 : TEXCOORD5;
                float4 posWorld : TEXCOORD6;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                float node_20_ang = 4.5;
                float node_20_spd = 1.0;
                float node_20_cos = cos(node_20_spd*node_20_ang);
                float node_20_sin = sin(node_20_spd*node_20_ang);
                float2 node_20_piv = float2(0.5,0.5);
                float2 node_20 = (mul((mul(UNITY_MATRIX_MVP, v.vertex).rgb*0.5).rb-node_20_piv,float2x2( node_20_cos, -node_20_sin, node_20_sin, node_20_cos))+node_20_piv);
                v.vertex.xyz += float3(sin((node_20+_TimeEditor.y*float2(1.26,0))).r);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.uv0 = v.uv0;
                TRANSFER_SHADOW_COLLECTOR(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float4 node_2 = tex2D(_Diffuse,i.uv0.xy);
                clip((node_2.a*_AlphaClip) - 0.5);
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
            uniform float4 _TimeEditor;
            #ifndef LIGHTMAP_OFF
                sampler2D unity_Lightmap;
                float4 unity_LightmapST;
                #ifndef DIRLIGHTMAP_OFF
                    sampler2D unity_LightmapInd;
                #endif
            #endif
            uniform sampler2D _Diffuse;
            uniform float _AlphaClip;
            struct VertexInput {
                float4 vertex : POSITION;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float4 uv0 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                float node_20_ang = 4.5;
                float node_20_spd = 1.0;
                float node_20_cos = cos(node_20_spd*node_20_ang);
                float node_20_sin = sin(node_20_spd*node_20_ang);
                float2 node_20_piv = float2(0.5,0.5);
                float2 node_20 = (mul((mul(UNITY_MATRIX_MVP, v.vertex).rgb*0.5).rb-node_20_piv,float2x2( node_20_cos, -node_20_sin, node_20_sin, node_20_cos))+node_20_piv);
                v.vertex.xyz += float3(sin((node_20+_TimeEditor.y*float2(1.26,0))).r);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.uv0 = v.uv0;
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float4 node_2 = tex2D(_Diffuse,i.uv0.xy);
                clip((node_2.a*_AlphaClip) - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
