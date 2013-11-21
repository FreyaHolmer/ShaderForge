// Shader created with Shader Forge Alpha 0.11 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.11;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:3,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:True,uamb:True,ufog:True,aust:False,igpj:True,qofs:0,lico:1,qpre:3,flbk:,rntp:2,lmpd:True;n:type:ShaderForge.SFN_Final,id:1,x:32803,y:32862|0-110-0,2-127-0,4-126-0,5-39-0;n:type:ShaderForge.SFN_Tex2d,id:3,x:34036,y:32565,ptlb:TextureX|0-85-0;n:type:ShaderForge.SFN_Tex2d,id:4,x:34033,y:33198,ptlb:TextureY|0-80-0;n:type:ShaderForge.SFN_Tex2d,id:5,x:34035,y:33814,ptlb:TextureZ|0-77-0;n:type:ShaderForge.SFN_FragmentPosition,id:11,x:34847,y:33072;n:type:ShaderForge.SFN_Slider,id:39,x:33163,y:32565,ptlb:Transparency,min:0,cur:1,max:1;n:type:ShaderForge.SFN_NormalVector,id:51,x:35298,y:32793,pt:False;n:type:ShaderForge.SFN_Abs,id:52,x:35094,y:32723|1-51-0;n:type:ShaderForge.SFN_Power,id:53,x:34904,y:32723|1-52-0,2-54-0;n:type:ShaderForge.SFN_Vector1,id:54,x:35094,y:32888,v1:2;n:type:ShaderForge.SFN_ComponentMask,id:55,x:34036,y:32387,cc1:0,cc2:4,cc3:4,cc4:4|1-53-0;n:type:ShaderForge.SFN_ComponentMask,id:56,x:34033,y:33044,cc1:1,cc2:4,cc3:4,cc4:4|1-53-0;n:type:ShaderForge.SFN_ComponentMask,id:57,x:34035,y:33648,cc1:2,cc2:4,cc3:4,cc4:4|1-53-0;n:type:ShaderForge.SFN_Multiply,id:58,x:33819,y:32505|1-55-0,2-3-2;n:type:ShaderForge.SFN_Slider,id:64,x:34501,y:32792,ptlb:ScaleX,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Slider,id:66,x:34516,y:33422,ptlb:ScaleY,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Slider,id:69,x:34530,y:33991,ptlb:ScaleZ,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Tex2d,id:71,x:34035,y:34132,ptlb:NormalZ|0-77-0;n:type:ShaderForge.SFN_Multiply,id:77,x:34250,y:33904|1-122-0,2-69-0;n:type:ShaderForge.SFN_Tex2d,id:78,x:34033,y:33499,ptlb:NormalY|0-80-0;n:type:ShaderForge.SFN_Multiply,id:80,x:34260,y:33291|1-120-0,2-66-0;n:type:ShaderForge.SFN_Tex2d,id:83,x:34036,y:32893,ptlb:NormalX|0-85-0;n:type:ShaderForge.SFN_Multiply,id:85,x:34237,y:32668|1-124-0,2-64-0;n:type:ShaderForge.SFN_Multiply,id:86,x:33819,y:32833|1-55-0,2-83-2;n:type:ShaderForge.SFN_Tex2d,id:89,x:34036,y:32728,ptlb:SpecularX|0-85-0;n:type:ShaderForge.SFN_Multiply,id:91,x:33819,y:32668|1-55-0,2-89-2;n:type:ShaderForge.SFN_Tex2d,id:92,x:34033,y:33351,ptlb:SpecularY|0-80-0;n:type:ShaderForge.SFN_Color,id:94,x:33819,y:32387,ptlb:ColourX,c1:1,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Color,id:95,x:33830,y:33044,ptlb:ColourY,c1:0,c2:1,c3:0,c4:1;n:type:ShaderForge.SFN_Multiply,id:96,x:33830,y:33138|1-56-0,2-4-2;n:type:ShaderForge.SFN_Multiply,id:97,x:33830,y:33291|1-56-0,2-92-2;n:type:ShaderForge.SFN_Multiply,id:98,x:33830,y:33439|1-56-0,2-78-2;n:type:ShaderForge.SFN_Multiply,id:99,x:33606,y:32407|1-94-0,2-58-0;n:type:ShaderForge.SFN_Multiply,id:100,x:33596,y:33078|1-95-0,2-96-0;n:type:ShaderForge.SFN_Color,id:101,x:33830,y:33648,ptlb:ColourZ,c1:0.01176471,c2:0,c3:1,c4:1;n:type:ShaderForge.SFN_Tex2d,id:102,x:34035,y:33964,ptlb:SpecularZ|0-77-0;n:type:ShaderForge.SFN_Multiply,id:105,x:33830,y:33754|1-57-0,2-5-2;n:type:ShaderForge.SFN_Multiply,id:106,x:33830,y:33912|1-57-0,2-102-2;n:type:ShaderForge.SFN_Multiply,id:107,x:33830,y:34072|1-57-0,2-71-2;n:type:ShaderForge.SFN_Multiply,id:108,x:33632,y:33651|1-101-0,2-105-0;n:type:ShaderForge.SFN_Add,id:109,x:33394,y:32600|1-99-0,2-100-0;n:type:ShaderForge.SFN_Add,id:110,x:33211,y:32668|1-109-0,2-108-0;n:type:ShaderForge.SFN_Add,id:114,x:33394,y:32792|1-91-0,2-97-0;n:type:ShaderForge.SFN_Add,id:115,x:33211,y:32872|1-114-0,2-106-0;n:type:ShaderForge.SFN_Append,id:120,x:34446,y:33249|1-11-1,2-11-3;n:type:ShaderForge.SFN_Append,id:122,x:34447,y:33814|1-11-1,2-11-2;n:type:ShaderForge.SFN_Append,id:124,x:34428,y:32592|1-11-2,2-11-3;n:type:ShaderForge.SFN_Add,id:125,x:33409,y:33065|1-86-0,2-98-0;n:type:ShaderForge.SFN_Add,id:126,x:33226,y:33138|1-125-0,2-107-0;n:type:ShaderForge.SFN_Multiply,id:127,x:33025,y:32772|1-39-0,2-115-0;pass:END;sub:END;*/

Shader "Shader Forge/ThriKreen" {
    Properties {
        _TextureX ("TextureX", 2D) = "white" {}
        _TextureY ("TextureY", 2D) = "white" {}
        _TextureZ ("TextureZ", 2D) = "white" {}
        _Transparency ("Transparency", Range(0, 1)) = 0
        _ScaleX ("ScaleX", Range(0, 1)) = 0
        _ScaleY ("ScaleY", Range(0, 1)) = 0
        _ScaleZ ("ScaleZ", Range(0, 1)) = 0
        _NormalZ ("NormalZ", 2D) = "white" {}
        _NormalY ("NormalY", 2D) = "white" {}
        _NormalX ("NormalX", 2D) = "white" {}
        _SpecularX ("SpecularX", 2D) = "white" {}
        _SpecularY ("SpecularY", 2D) = "white" {}
        _ColourX ("ColourX", Color) = (1,0,0,1)
        _ColourY ("ColourY", Color) = (0,1,0,1)
        _ColourZ ("ColourZ", Color) = (0.01176471,0,1,1)
        _SpecularZ ("SpecularZ", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            
            CGPROGRAM
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdbase
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
            uniform sampler2D _TextureX;
            uniform sampler2D _TextureY;
            uniform sampler2D _TextureZ;
            uniform float _Transparency;
            uniform float _ScaleX;
            uniform float _ScaleY;
            uniform float _ScaleZ;
            uniform sampler2D _NormalZ;
            uniform sampler2D _NormalY;
            uniform sampler2D _NormalX;
            uniform sampler2D _SpecularX;
            uniform sampler2D _SpecularY;
            uniform float4 _ColourX;
            uniform float4 _ColourY;
            uniform float4 _ColourZ;
            uniform sampler2D _SpecularZ;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 uv1 : TEXCOORD1;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float3 tangentDir : TEXCOORD2;
                float3 binormalDir : TEXCOORD3;
                #ifndef LIGHTMAP_OFF
                    float2 uvLM : TEXCOORD4;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                #ifndef LIGHTMAP_OFF
                    o.uvLM = v.uv1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                #endif
                float3 normalDirection = mul(float4(v.normal,0), _World2Object).xyz;
                o.normalDir = normalDirection;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 node_53 = pow(abs(i.normalDir),2);
                float node_55 = node_53.r;
                float4 node_11 = i.posWorld;
                float2 node_85 = (float2(node_11.g,node_11.b)*_ScaleX);
                float node_56 = node_53.g;
                float2 node_80 = (float2(node_11.r,node_11.b)*_ScaleY);
                float node_57 = node_53.b;
                float2 node_77 = (float2(node_11.r,node_11.g)*_ScaleZ);
                float3 normalLocal = (((node_55*tex2D(_NormalX,node_85).rgb)+(node_56*tex2D(_NormalY,node_80).rgb))+(node_57*tex2D(_NormalZ,node_77).rgb));
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
                float attenuation = 1;
                #ifndef LIGHTMAP_OFF
                    float3 lambert = lightmap;
                #else
                    float3 lambert = attenuation * max( 0.0, dot(normalDirection,lightDirection )) * _LightColor0.xyz;
                #endif
                float node_39 = _Transparency;
                float3 addLight = lambert * (node_39*(((node_55*tex2D(_SpecularX,node_85).rgb)+(node_56*tex2D(_SpecularY,node_80).rgb))+(node_57*tex2D(_SpecularZ,node_77).rgb))) * pow(max(0,dot(reflect(-lightDirection, normalDirection),viewDirection)),10);
                float3 lightFinal = lambert + UNITY_LIGHTMODEL_AMBIENT.xyz;
                return fixed4(lightFinal * (((_ColourX.rgb*(node_55*tex2D(_TextureX,node_85).rgb))+(_ColourY.rgb*(node_56*tex2D(_TextureY,node_80).rgb)))+(_ColourZ.rgb*(node_57*tex2D(_TextureZ,node_77).rgb))) + addLight,node_39);
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
            #pragma multi_compile_fwdadd
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
            uniform sampler2D _TextureX;
            uniform sampler2D _TextureY;
            uniform sampler2D _TextureZ;
            uniform float _Transparency;
            uniform float _ScaleX;
            uniform float _ScaleY;
            uniform float _ScaleZ;
            uniform sampler2D _NormalZ;
            uniform sampler2D _NormalY;
            uniform sampler2D _NormalX;
            uniform sampler2D _SpecularX;
            uniform sampler2D _SpecularY;
            uniform float4 _ColourX;
            uniform float4 _ColourY;
            uniform float4 _ColourZ;
            uniform sampler2D _SpecularZ;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 uv1 : TEXCOORD1;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float3 tangentDir : TEXCOORD2;
                float3 binormalDir : TEXCOORD3;
                LIGHTING_COORDS(4,5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
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
                float3 node_53 = pow(abs(i.normalDir),2);
                float node_55 = node_53.r;
                float4 node_11 = i.posWorld;
                float2 node_85 = (float2(node_11.g,node_11.b)*_ScaleX);
                float node_56 = node_53.g;
                float2 node_80 = (float2(node_11.r,node_11.b)*_ScaleY);
                float node_57 = node_53.b;
                float2 node_77 = (float2(node_11.r,node_11.g)*_ScaleZ);
                float3 normalLocal = (((node_55*tex2D(_NormalX,node_85).rgb)+(node_56*tex2D(_NormalY,node_80).rgb))+(node_57*tex2D(_NormalZ,node_77).rgb));
                float3 normalDirection = normalize( mul( normalLocal, tangentTransform ) );
                float3 lightDirection;
                if (0.0 == _WorldSpaceLightPos0.w){
                    lightDirection = normalize( _WorldSpaceLightPos0.xyz );
                } else {
                    lightDirection = normalize( _WorldSpaceLightPos0 - i.posWorld.xyz );
                }
                float attenuation = LIGHT_ATTENUATION(i);
                float3 lambert = attenuation * max( 0.0, dot(normalDirection,lightDirection )) * _LightColor0.xyz;
                float node_39 = _Transparency;
                float3 addLight = lambert * (node_39*(((node_55*tex2D(_SpecularX,node_85).rgb)+(node_56*tex2D(_SpecularY,node_80).rgb))+(node_57*tex2D(_SpecularZ,node_77).rgb))) * pow(max(0,dot(reflect(-lightDirection, normalDirection),viewDirection)),10);
                float3 lightFinal = lambert;
                return fixed4(lightFinal * (((_ColourX.rgb*(node_55*tex2D(_TextureX,node_85).rgb))+(_ColourY.rgb*(node_56*tex2D(_TextureY,node_80).rgb)))+(_ColourZ.rgb*(node_57*tex2D(_TextureZ,node_77).rgb))) + addLight,node_39);
            }
            ENDCG
        }
    }
    FallBack "Specular"
}
