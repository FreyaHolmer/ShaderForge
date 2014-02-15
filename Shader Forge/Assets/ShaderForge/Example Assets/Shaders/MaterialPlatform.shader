// Shader created with Shader Forge Beta 0.24 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.24;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:False,mssp:False,lmpd:True,lprd:False,enco:False,frtr:True,vitr:True,dbil:True,rmgx:True,hqsc:True,hqlp:False,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.1280277,fgcg:0.1953466,fgcb:0.2352941,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0;n:type:ShaderForge.SFN_Final,id:1,x:32912,y:32561|diff-128-OUT,spec-146-OUT,gloss-453-OUT,normal-397-OUT,amspl-140-OUT;n:type:ShaderForge.SFN_Tex2d,id:2,x:34747,y:32587,ptlb:MainTex,tex:3d403fe3184a448fa8bc190c7f07f28c,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:128,x:33245,y:32395|A-290-OUT,B-524-OUT,T-132-B;n:type:ShaderForge.SFN_Cubemap,id:129,x:33656,y:33058,ptlb:Cubemap,cube:f466cf7415226e046b096197eb7341aa,pvfc:0;n:type:ShaderForge.SFN_Multiply,id:130,x:33461,y:33103,cmnt:RGBM Decode|A-129-RGB,B-129-A,C-419-OUT;n:type:ShaderForge.SFN_VertexColor,id:132,x:33436,y:32111;n:type:ShaderForge.SFN_Multiply,id:140,x:33245,y:32930|A-132-B,B-130-OUT;n:type:ShaderForge.SFN_ConstantLerp,id:146,x:33245,y:32529,a:0.1,b:0.8|IN-132-B;n:type:ShaderForge.SFN_Tex2d,id:288,x:34672,y:32145,ptlb:Dirt Mask,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-350-OUT;n:type:ShaderForge.SFN_Multiply,id:289,x:34149,y:32200|A-343-OUT,B-365-OUT;n:type:ShaderForge.SFN_Lerp,id:290,x:33459,y:32304|A-2-RGB,B-297-OUT,T-467-OUT;n:type:ShaderForge.SFN_Multiply,id:297,x:33664,y:32413|A-2-RGB,B-488-RGB;n:type:ShaderForge.SFN_Multiply,id:337,x:34498,y:32145|A-288-R,B-288-R;n:type:ShaderForge.SFN_OneMinus,id:343,x:34334,y:32145|IN-337-OUT;n:type:ShaderForge.SFN_TexCoord,id:349,x:35015,y:32071,uv:0;n:type:ShaderForge.SFN_Multiply,id:350,x:34850,y:32145|A-349-UVOUT,B-351-OUT;n:type:ShaderForge.SFN_Vector1,id:351,x:35015,y:32224,v1:2;n:type:ShaderForge.SFN_Multiply,id:364,x:34498,y:32279|A-2-A,B-2-A;n:type:ShaderForge.SFN_OneMinus,id:365,x:34334,y:32279|IN-364-OUT;n:type:ShaderForge.SFN_Tex2d,id:391,x:33656,y:32717,ptlb:Floor Normal,tex:839587738573a48e28a59f1905941428,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Lerp,id:397,x:33245,y:32801|A-391-RGB,B-442-OUT,T-132-B;n:type:ShaderForge.SFN_Tex2d,id:398,x:34029,y:32832,tex:80286949e259c2d44876306923857245,ntxv:3,isnm:True|UVIN-400-OUT,TEX-430-TEX;n:type:ShaderForge.SFN_TexCoord,id:399,x:34437,y:32832,uv:0;n:type:ShaderForge.SFN_Multiply,id:400,x:34241,y:32832|A-399-UVOUT,B-407-OUT;n:type:ShaderForge.SFN_ValueProperty,id:407,x:34437,y:33003,ptlb:Edge Normal Scale,v1:4;n:type:ShaderForge.SFN_Vector1,id:419,x:33656,y:33207,v1:5;n:type:ShaderForge.SFN_Tex2dAsset,id:430,x:34241,y:33121,ptlb:Edge Normal,tex:80286949e259c2d44876306923857245;n:type:ShaderForge.SFN_Tex2d,id:437,x:34029,y:32972,tex:80286949e259c2d44876306923857245,ntxv:3,isnm:True|UVIN-438-OUT,TEX-430-TEX;n:type:ShaderForge.SFN_Multiply,id:438,x:34241,y:32969|A-399-UVOUT,B-407-OUT,C-439-OUT;n:type:ShaderForge.SFN_Vector1,id:439,x:34437,y:33060,v1:0.5;n:type:ShaderForge.SFN_Lerp,id:440,x:33831,y:32908|A-398-RGB,B-437-RGB,T-441-OUT;n:type:ShaderForge.SFN_Vector1,id:441,x:34029,y:33107,v1:0.5;n:type:ShaderForge.SFN_Normalize,id:442,x:33656,y:32908,cmnt:Normal Metal|IN-440-OUT;n:type:ShaderForge.SFN_Lerp,id:453,x:33245,y:32670|A-546-OUT,B-454-OUT,T-132-B;n:type:ShaderForge.SFN_Vector1,id:454,x:33431,y:32717,v1:0.8;n:type:ShaderForge.SFN_VertexColor,id:461,x:34149,y:32068;n:type:ShaderForge.SFN_Multiply,id:467,x:33972,y:32161,cmnt:Dirt mask|A-461-R,B-289-OUT;n:type:ShaderForge.SFN_Color,id:488,x:33854,y:32504,ptlb:Dirt Color,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Vector1,id:524,x:33459,y:32429,v1:0;n:type:ShaderForge.SFN_Multiply,id:546,x:33431,y:32580|A-547-OUT,B-2-A;n:type:ShaderForge.SFN_Vector1,id:547,x:33631,y:32565,v1:0.4;proporder:2-391-430-407-129-288-488;pass:END;sub:END;*/

Shader "Shader Forge/Examples/MaterialPlatform" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _FloorNormal ("Floor Normal", 2D) = "bump" {}
        _EdgeNormal ("Edge Normal", 2D) = "bump" {}
        _EdgeNormalScale ("Edge Normal Scale", Float ) = 4
        _Cubemap ("Cubemap", Cube) = "_Skybox" {}
        _DirtMask ("Dirt Mask", 2D) = "white" {}
        _DirtColor ("Dirt Color", Color) = (0.5,0.5,0.5,1)
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
            #include "Lighting.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 3.0
            #ifndef LIGHTMAP_OFF
                sampler2D unity_Lightmap;
                float4 unity_LightmapST;
                #ifndef DIRLIGHTMAP_OFF
                    sampler2D unity_LightmapInd;
                #endif
            #endif
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform samplerCUBE _Cubemap;
            uniform sampler2D _DirtMask; uniform float4 _DirtMask_ST;
            uniform sampler2D _FloorNormal; uniform float4 _FloorNormal_ST;
            uniform float _EdgeNormalScale;
            uniform sampler2D _EdgeNormal; uniform float4 _EdgeNormal_ST;
            uniform float4 _DirtColor;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(5,6)
                #ifndef LIGHTMAP_OFF
                    float2 uvLM : TEXCOORD7;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.vertexColor = v.vertexColor;
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
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float2 node_563 = i.uv0;
                float2 node_399 = i.uv0;
                float node_407 = _EdgeNormalScale;
                float2 node_400 = (node_399.rg*node_407);
                float2 node_438 = (node_399.rg*node_407*0.5);
                float4 node_132 = i.vertexColor;
                float3 normalLocal = lerp(UnpackNormal(tex2D(_FloorNormal,TRANSFORM_TEX(node_563.rg, _FloorNormal))).rgb,normalize(lerp(UnpackNormal(tex2D(_EdgeNormal,TRANSFORM_TEX(node_400, _EdgeNormal))).rgb,UnpackNormal(tex2D(_EdgeNormal,TRANSFORM_TEX(node_438, _EdgeNormal))).rgb,0.5)),node_132.b);
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
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
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i)*2;
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                #ifndef LIGHTMAP_OFF
                    float3 diffuse = lightmap;
                #else
                    float3 diffuse = max( 0.0, NdotL) * attenColor;
                #endif
///////// Gloss:
                float4 node_2 = tex2D(_MainTex,TRANSFORM_TEX(node_563.rg, _MainTex));
                float gloss = exp2(lerp((0.4*node_2.a),0.8,node_132.b)*10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float4 node_129 = texCUBE(_Cubemap,viewReflectDirection);
                float node_146 = lerp(0.1,0.8,node_132.b);
                float3 specularColor = float3(node_146,node_146,node_146);
                float3 specularAmb = (node_132.b*(node_129.rgb*node_129.a*5.0)) * specularColor;
                float3 specular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),gloss) * specularColor + specularAmb;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                float2 node_350 = (i.uv0.rg*2.0);
                float4 node_288 = tex2D(_DirtMask,TRANSFORM_TEX(node_350, _DirtMask));
                float node_524 = 0.0;
                finalColor += diffuseLight * lerp(lerp(node_2.rgb,(node_2.rgb*_DirtColor.rgb),(i.vertexColor.r*((1.0 - (node_288.r*node_288.r))*(1.0 - (node_2.a*node_2.a))))),float3(node_524,node_524,node_524),node_132.b);
                finalColor += specular;
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
            #include "Lighting.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 3.0
            #ifndef LIGHTMAP_OFF
                sampler2D unity_Lightmap;
                float4 unity_LightmapST;
                #ifndef DIRLIGHTMAP_OFF
                    sampler2D unity_LightmapInd;
                #endif
            #endif
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _DirtMask; uniform float4 _DirtMask_ST;
            uniform sampler2D _FloorNormal; uniform float4 _FloorNormal_ST;
            uniform float _EdgeNormalScale;
            uniform sampler2D _EdgeNormal; uniform float4 _EdgeNormal_ST;
            uniform float4 _DirtColor;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(5,6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.vertexColor = v.vertexColor;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
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
                float2 node_564 = i.uv0;
                float2 node_399 = i.uv0;
                float node_407 = _EdgeNormalScale;
                float2 node_400 = (node_399.rg*node_407);
                float2 node_438 = (node_399.rg*node_407*0.5);
                float4 node_132 = i.vertexColor;
                float3 normalLocal = lerp(UnpackNormal(tex2D(_FloorNormal,TRANSFORM_TEX(node_564.rg, _FloorNormal))).rgb,normalize(lerp(UnpackNormal(tex2D(_EdgeNormal,TRANSFORM_TEX(node_400, _EdgeNormal))).rgb,UnpackNormal(tex2D(_EdgeNormal,TRANSFORM_TEX(node_438, _EdgeNormal))).rgb,0.5)),node_132.b);
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i)*2;
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor;
///////// Gloss:
                float4 node_2 = tex2D(_MainTex,TRANSFORM_TEX(node_564.rg, _MainTex));
                float gloss = exp2(lerp((0.4*node_2.a),0.8,node_132.b)*10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float node_146 = lerp(0.1,0.8,node_132.b);
                float3 specularColor = float3(node_146,node_146,node_146);
                float3 specular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),gloss) * specularColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                float2 node_350 = (i.uv0.rg*2.0);
                float4 node_288 = tex2D(_DirtMask,TRANSFORM_TEX(node_350, _DirtMask));
                float node_524 = 0.0;
                finalColor += diffuseLight * lerp(lerp(node_2.rgb,(node_2.rgb*_DirtColor.rgb),(i.vertexColor.r*((1.0 - (node_288.r*node_288.r))*(1.0 - (node_2.a*node_2.a))))),float3(node_524,node_524,node_524),node_132.b);
                finalColor += specular;
/// Final Color:
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
