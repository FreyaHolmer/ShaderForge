// Shader created with Shader Forge Alpha 0.11 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.11;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:2,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,uamb:False,ufog:True,aust:True,igpj:False,qofs:0,lico:1,qpre:1,flbk:,rntp:1,lmpd:True;n:type:ShaderForge.SFN_Final,id:1,x:32803,y:32862|0-4-0,4-5-2;n:type:ShaderForge.SFN_Color,id:2,x:33478,y:32597,ptlb:Color,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Tex2d,id:3,x:33306,y:32778,ptlb:MainTex|0-8-0;n:type:ShaderForge.SFN_Multiply,id:4,x:33063,y:32682|1-2-0,2-3-2;n:type:ShaderForge.SFN_Tex2d,id:5,x:33306,y:32968,ptlb:Normalmap,tex:bbab0a6f7bae9cf42bf057d8ee2755f6;n:type:ShaderForge.SFN_TexCoord,id:8,x:33491,y:32778,uv:0;n:type:ShaderForge.SFN_Multiply,id:11,x:33235,y:32526|1-12-0,2-2-0;n:type:ShaderForge.SFN_Vector1,id:12,x:33412,y:32410,v1:2;pass:END;sub:END;*/

Shader "Shader Forge/LightmappedBump Test" {
    Properties {
        _Color ("Color", Color) = (0.5,0.5,0.5,1)
        _MainTex ("MainTex", 2D) = "white" {}
        _Normalmap ("Normalmap", 2D) = "bump" {}
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
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma vertex vert
            #pragma fragment frag
            #pragma exclude_renderers gles xbox360 ps3 flash 
            #pragma target 3.0
            #ifndef LIGHTMAP_OFF
                sampler2D unity_Lightmap;
                float4 unity_LightmapST;
                #ifndef DIRLIGHTMAP_OFF
                    sampler2D unity_LightmapInd;
                #endif
            #endif
            uniform float4 _Color;
            uniform sampler2D _MainTex;
            uniform sampler2D _Normalmap;
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
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.uv0 = v.uv0;
                #ifndef LIGHTMAP_OFF
                    o.uvLM = v.uv1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                #endif
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
                float3 normalLocal = UnpackNormal(tex2D(_Normalmap,i.uv0.xy)).rgb;
                float3 normalDirection = normalize( mul( normalLocal, tangentTransform ) );
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
                float3 halfDirection = normalize(viewDirection+lightDirection);
                float attenuation = LIGHT_ATTENUATION(i);
                #ifndef LIGHTMAP_OFF
                    float3 lambert = lightmap;
                #else
                    float3 lambert = attenuation * max( 0.0, dot(normalDirection,lightDirection )) * _LightColor0.xyz;
                #endif
                float3 lightFinal = lambert;
                float4 node_2 = _Color;
                return fixed4(lightFinal * (node_2.rgb*tex2D(_MainTex,i.uv0.rg).rgb),1);
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
            #include "Lighting.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma vertex vert
            #pragma fragment frag
            #pragma exclude_renderers gles xbox360 ps3 flash 
            #pragma target 3.0
            #ifndef LIGHTMAP_OFF
                sampler2D unity_Lightmap;
                float4 unity_LightmapST;
                #ifndef DIRLIGHTMAP_OFF
                    sampler2D unity_LightmapInd;
                #endif
            #endif
            uniform float4 _Color;
            uniform sampler2D _MainTex;
            uniform sampler2D _Normalmap;
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
                float3 normalLocal = UnpackNormal(tex2D(_Normalmap,i.uv0.xy)).rgb;
                float3 normalDirection = normalize( mul( normalLocal, tangentTransform ) );
                float3 lightDirection;
                if (0.0 == _WorldSpaceLightPos0.w){
                    lightDirection = normalize( _WorldSpaceLightPos0.xyz );
                } else {
                    lightDirection = normalize( _WorldSpaceLightPos0 - i.posWorld.xyz );
                }
                float3 halfDirection = normalize(viewDirection+lightDirection);
                float attenuation = LIGHT_ATTENUATION(i);
                float3 lambert = attenuation * max( 0.0, dot(normalDirection,lightDirection )) * _LightColor0.xyz;
                float3 lightFinal = lambert;
                float4 node_2 = _Color;
                return fixed4(lightFinal * (node_2.rgb*tex2D(_MainTex,i.uv0.rg).rgb),1);
            }
            ENDCG
        }
    }
    FallBack "Specular"
}
