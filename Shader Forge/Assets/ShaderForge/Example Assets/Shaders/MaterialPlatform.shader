// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D
// Upgrade NOTE: replaced tex2D unity_LightmapInd with UNITY_SAMPLE_TEX2D_SAMPLER

// Shader created with Shader Forge v1.06 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.06;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:False,mssp:False,lmpd:True,lprd:False,rprd:False,enco:False,frtr:True,vitr:True,dbil:True,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,dith:0,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.1280277,fgcg:0.1953466,fgcb:0.2352941,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:34911,y:32561,varname:node_1,prsc:2|diff-128-OUT,spec-146-OUT,gloss-453-OUT,normal-397-OUT,amspl-140-OUT;n:type:ShaderForge.SFN_Tex2d,id:2,x:33146,y:32587,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,tex:3d403fe3184a448fa8bc190c7f07f28c,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:128,x:34648,y:32395,varname:node_128,prsc:2|A-290-OUT,B-524-OUT,T-132-B;n:type:ShaderForge.SFN_Cubemap,id:129,x:34237,y:33058,ptovrint:False,ptlb:Cubemap,ptin:_Cubemap,varname:_Cubemap,prsc:2,cube:f466cf7415226e046b096197eb7341aa,pvfc:0;n:type:ShaderForge.SFN_Multiply,id:130,x:34432,y:33103,cmnt:RGBM Decode,varname:node_130,prsc:2|A-129-RGB,B-129-A,C-419-OUT;n:type:ShaderForge.SFN_VertexColor,id:132,x:34457,y:32111,varname:node_132,prsc:2;n:type:ShaderForge.SFN_Multiply,id:140,x:34648,y:32930,varname:node_140,prsc:2|A-132-B,B-130-OUT;n:type:ShaderForge.SFN_ConstantLerp,id:146,x:34648,y:32529,varname:node_146,prsc:2,a:0.1,b:0.8|IN-132-B;n:type:ShaderForge.SFN_Tex2d,id:288,x:33221,y:32145,ptovrint:False,ptlb:Dirt Mask,ptin:_DirtMask,varname:_DirtMask,prsc:2,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-350-OUT;n:type:ShaderForge.SFN_Multiply,id:289,x:33744,y:32200,varname:node_289,prsc:2|A-343-OUT,B-365-OUT;n:type:ShaderForge.SFN_Lerp,id:290,x:34434,y:32304,varname:node_290,prsc:2|A-2-RGB,B-297-OUT,T-467-OUT;n:type:ShaderForge.SFN_Multiply,id:297,x:34229,y:32413,varname:node_297,prsc:2|A-2-RGB,B-488-RGB;n:type:ShaderForge.SFN_Multiply,id:337,x:33395,y:32145,varname:node_337,prsc:2|A-288-R,B-288-R;n:type:ShaderForge.SFN_OneMinus,id:343,x:33559,y:32145,varname:node_343,prsc:2|IN-337-OUT;n:type:ShaderForge.SFN_TexCoord,id:349,x:32878,y:32071,varname:node_349,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:350,x:33043,y:32145,varname:node_350,prsc:2|A-349-UVOUT,B-351-OUT;n:type:ShaderForge.SFN_Vector1,id:351,x:32878,y:32224,varname:node_351,prsc:2,v1:2;n:type:ShaderForge.SFN_Multiply,id:364,x:33395,y:32279,varname:node_364,prsc:2|A-2-A,B-2-A;n:type:ShaderForge.SFN_OneMinus,id:365,x:33559,y:32279,varname:node_365,prsc:2|IN-364-OUT;n:type:ShaderForge.SFN_Tex2d,id:391,x:34237,y:32717,ptovrint:False,ptlb:Floor Normal,ptin:_FloorNormal,varname:_FloorNormal,prsc:2,tex:839587738573a48e28a59f1905941428,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Lerp,id:397,x:34648,y:32801,varname:node_397,prsc:2|A-391-RGB,B-442-OUT,T-132-B;n:type:ShaderForge.SFN_Tex2d,id:398,x:33864,y:32832,varname:node_398,prsc:2,tex:80286949e259c2d44876306923857245,ntxv:3,isnm:True|UVIN-400-OUT,TEX-430-TEX;n:type:ShaderForge.SFN_TexCoord,id:399,x:33456,y:32832,varname:node_399,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:400,x:33652,y:32832,varname:node_400,prsc:2|A-399-UVOUT,B-407-OUT;n:type:ShaderForge.SFN_ValueProperty,id:407,x:33456,y:33003,ptovrint:False,ptlb:Edge Normal Scale,ptin:_EdgeNormalScale,varname:_EdgeNormalScale,prsc:2,glob:False,v1:4;n:type:ShaderForge.SFN_Vector1,id:419,x:34237,y:33207,varname:node_419,prsc:2,v1:5;n:type:ShaderForge.SFN_Tex2dAsset,id:430,x:33652,y:33121,ptovrint:False,ptlb:Edge Normal,ptin:_EdgeNormal,varname:_EdgeNormal,tex:80286949e259c2d44876306923857245,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Tex2d,id:437,x:33864,y:32972,varname:node_431,prsc:2,tex:80286949e259c2d44876306923857245,ntxv:3,isnm:True|UVIN-438-OUT,TEX-430-TEX;n:type:ShaderForge.SFN_Multiply,id:438,x:33652,y:32969,varname:node_438,prsc:2|A-399-UVOUT,B-407-OUT,C-439-OUT;n:type:ShaderForge.SFN_Vector1,id:439,x:33456,y:33060,varname:node_439,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Lerp,id:440,x:34062,y:32908,varname:node_440,prsc:2|A-398-RGB,B-437-RGB,T-441-OUT;n:type:ShaderForge.SFN_Vector1,id:441,x:33864,y:33107,varname:node_441,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Normalize,id:442,x:34237,y:32908,cmnt:Normal Metal,varname:node_442,prsc:2|IN-440-OUT;n:type:ShaderForge.SFN_Lerp,id:453,x:34648,y:32670,varname:node_453,prsc:2|A-546-OUT,B-454-OUT,T-132-B;n:type:ShaderForge.SFN_Vector1,id:454,x:34462,y:32717,varname:node_454,prsc:2,v1:0.8;n:type:ShaderForge.SFN_VertexColor,id:461,x:33744,y:32068,varname:node_461,prsc:2;n:type:ShaderForge.SFN_Multiply,id:467,x:33921,y:32161,cmnt:Dirt mask,varname:node_467,prsc:2|A-461-R,B-289-OUT;n:type:ShaderForge.SFN_Color,id:488,x:34039,y:32504,ptovrint:False,ptlb:Dirt Color,ptin:_DirtColor,varname:_DirtColor,prsc:2,glob:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Vector1,id:524,x:34434,y:32429,varname:node_524,prsc:2,v1:0;n:type:ShaderForge.SFN_Multiply,id:546,x:34462,y:32580,varname:node_546,prsc:2|A-547-OUT,B-2-A;n:type:ShaderForge.SFN_Vector1,id:547,x:34262,y:32565,varname:node_547,prsc:2,v1:0.4;proporder:2-391-430-407-129-288-488;pass:END;sub:END;*/

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
                #ifndef DIRLIGHTMAP_OFF
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
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
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
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = mul(_Object2World, float4(v.normal,0)).xyz;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                #ifndef LIGHTMAP_OFF
                    o.uvLM = v.texcoord1 * unity_LightmapST.xy + unity_LightmapST.zw;
                #endif
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _FloorNormal_var = UnpackNormal(tex2D(_FloorNormal,TRANSFORM_TEX(i.uv0, _FloorNormal)));
                float2 node_400 = (i.uv0*_EdgeNormalScale);
                float3 node_398 = UnpackNormal(tex2D(_EdgeNormal,TRANSFORM_TEX(node_400, _EdgeNormal)));
                float2 node_438 = (i.uv0*_EdgeNormalScale*0.5);
                float3 node_431 = UnpackNormal(tex2D(_EdgeNormal,TRANSFORM_TEX(node_438, _EdgeNormal)));
                float3 normalLocal = lerp(_FloorNormal_var.rgb,normalize(lerp(node_398.rgb,node_431.rgb,0.5)),i.vertexColor.b);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                #ifndef LIGHTMAP_OFF
                    float4 lmtex = UNITY_SAMPLE_TEX2D(unity_Lightmap,i.uvLM);
                    #ifndef DIRLIGHTMAP_OFF
                        float3 lightmap = DecodeLightmap(lmtex);
                        float3 scalePerBasisVector = DecodeLightmap(UNITY_SAMPLE_TEX2D_SAMPLER(unity_LightmapInd,unity_Lightmap,i.uvLM));
                        UNITY_DIRBASIS
                        half3 normalInRnmBasis = saturate (mul (unity_DirBasis, normalLocal));
                        lightmap *= dot (normalInRnmBasis, scalePerBasisVector);
                    #else
                        float3 lightmap = DecodeLightmap(lmtex);
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
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i)*2;
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float gloss = lerp((0.4*_MainTex_var.a),0.8,i.vertexColor.b);
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float4 _Cubemap_var = texCUBE(_Cubemap,viewReflectDirection);
                float node_146 = lerp(0.1,0.8,i.vertexColor.b);
                float3 specularColor = float3(node_146,node_146,node_146);
                #if !defined(LIGHTMAP_OFF) && defined(DIRLIGHTMAP_OFF)
                    float3 directSpecular = float3(0,0,0);
                #else
                    float3 directSpecular = 1 * pow(max(0,dot(halfDirection,normalDirection)),specPow);
                #endif
                float3 indirectSpecular = (0 + (i.vertexColor.b*(_Cubemap_var.rgb*_Cubemap_var.a*5.0)));
                float3 specular = (directSpecular + indirectSpecular) * specularColor;
                #ifndef LIGHTMAP_OFF
                    #ifndef DIRLIGHTMAP_OFF
                        specular *= lightmap;
                    #else
                        specular *= attenColor;
                    #endif
                #else
                    specular *= attenColor;
                #endif
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                #ifndef LIGHTMAP_OFF
                    float3 directDiffuse = float3(0,0,0);
                #else
                    float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                #endif
                #ifndef LIGHTMAP_OFF
                    #ifdef SHADOWS_SCREEN
                        #if (defined(SHADER_API_GLES) || defined(SHADER_API_GLES3)) && defined(SHADER_API_MOBILE)
                            directDiffuse += min(lightmap.rgb, attenuation);
                        #else
                            directDiffuse += max(min(lightmap.rgb,attenuation*lmtex.rgb), lightmap.rgb*attenuation*0.5);
                        #endif
                    #else
                        directDiffuse += lightmap.rgb;
                    #endif
                #endif
                float2 node_350 = (i.uv0*2.0);
                float4 _DirtMask_var = tex2D(_DirtMask,TRANSFORM_TEX(node_350, _DirtMask));
                float node_524 = 0.0;
                float3 diffuse = directDiffuse * lerp(lerp(_MainTex_var.rgb,(_MainTex_var.rgb*_DirtColor.rgb),(i.vertexColor.r*((1.0 - (_DirtMask_var.r*_DirtMask_var.r))*(1.0 - (_MainTex_var.a*_MainTex_var.a))))),float3(node_524,node_524,node_524),i.vertexColor.b);
/// Final Color:
                float3 finalColor = diffuse + specular;
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
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(5,6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = mul(_Object2World, float4(v.normal,0)).xyz;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _FloorNormal_var = UnpackNormal(tex2D(_FloorNormal,TRANSFORM_TEX(i.uv0, _FloorNormal)));
                float2 node_400 = (i.uv0*_EdgeNormalScale);
                float3 node_398 = UnpackNormal(tex2D(_EdgeNormal,TRANSFORM_TEX(node_400, _EdgeNormal)));
                float2 node_438 = (i.uv0*_EdgeNormalScale*0.5);
                float3 node_431 = UnpackNormal(tex2D(_EdgeNormal,TRANSFORM_TEX(node_438, _EdgeNormal)));
                float3 normalLocal = lerp(_FloorNormal_var.rgb,normalize(lerp(node_398.rgb,node_431.rgb,0.5)),i.vertexColor.b);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i)*2;
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float gloss = lerp((0.4*_MainTex_var.a),0.8,i.vertexColor.b);
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float node_146 = lerp(0.1,0.8,i.vertexColor.b);
                float3 specularColor = float3(node_146,node_146,node_146);
                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow);
                float3 specular = directSpecular * specularColor;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float2 node_350 = (i.uv0*2.0);
                float4 _DirtMask_var = tex2D(_DirtMask,TRANSFORM_TEX(node_350, _DirtMask));
                float node_524 = 0.0;
                float3 diffuse = directDiffuse * lerp(lerp(_MainTex_var.rgb,(_MainTex_var.rgb*_DirtColor.rgb),(i.vertexColor.r*((1.0 - (_DirtMask_var.r*_DirtMask_var.r))*(1.0 - (_MainTex_var.a*_MainTex_var.a))))),float3(node_524,node_524,node_524),i.vertexColor.b);
/// Final Color:
                float3 finalColor = diffuse + specular;
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
