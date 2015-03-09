// Shader created with Shader Forge v1.06 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.06;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:0,limd:1,uamb:False,mssp:True,lmpd:False,lprd:False,rprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:1,bsrc:3,bdst:7,culm:2,dpts:2,wrdp:True,dith:0,ufog:False,aust:False,igpj:False,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:0,x:34414,y:32442,varname:node_0,prsc:2|diff-219-OUT,spec-75-OUT,gloss-76-OUT,normal-215-OUT,transm-29-OUT,lwrap-29-OUT,alpha-22-OUT,refract-14-OUT;n:type:ShaderForge.SFN_Slider,id:13,x:33580,y:32752,ptovrint:False,ptlb:Refraction Intensity,ptin:_RefractionIntensity,varname:_RefractionIntensity,prsc:2,min:0,cur:0.1,max:1;n:type:ShaderForge.SFN_Multiply,id:14,x:34145,y:32724,varname:node_14,prsc:2|A-16-OUT,B-220-OUT;n:type:ShaderForge.SFN_ComponentMask,id:16,x:33975,y:32651,varname:node_16,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-25-RGB;n:type:ShaderForge.SFN_Vector1,id:22,x:34145,y:32651,varname:node_22,prsc:2,v1:0.3;n:type:ShaderForge.SFN_Tex2d,id:25,x:33737,y:32566,ptovrint:False,ptlb:Refraction,ptin:_Refraction,varname:_Refraction,prsc:2,tex:bbab0a6f7bae9cf42bf057d8ee2755f6,ntxv:3,isnm:True|UVIN-27-OUT;n:type:ShaderForge.SFN_TexCoord,id:26,x:33379,y:32505,varname:node_26,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:27,x:33550,y:32566,varname:node_27,prsc:2|A-26-UVOUT,B-28-OUT;n:type:ShaderForge.SFN_Vector1,id:28,x:33379,y:32662,varname:node_28,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:29,x:34145,y:32577,varname:node_29,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:75,x:34145,y:32430,varname:node_75,prsc:2,v1:6;n:type:ShaderForge.SFN_Vector1,id:76,x:34145,y:32486,varname:node_76,prsc:2,v1:0.8;n:type:ShaderForge.SFN_Lerp,id:215,x:33975,y:32524,varname:node_215,prsc:2|A-216-OUT,B-25-RGB,T-13-OUT;n:type:ShaderForge.SFN_Vector3,id:216,x:33737,y:32445,varname:node_216,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Fresnel,id:217,x:33975,y:32284,varname:node_217,prsc:2;n:type:ShaderForge.SFN_ConstantLerp,id:219,x:34145,y:32284,varname:node_219,prsc:2,a:0.02,b:0.2|IN-217-OUT;n:type:ShaderForge.SFN_Multiply,id:220,x:33975,y:32803,varname:node_220,prsc:2|A-13-OUT,B-221-OUT;n:type:ShaderForge.SFN_Vector1,id:221,x:33737,y:32831,varname:node_221,prsc:2,v1:0.2;proporder:13-25;pass:END;sub:END;*/

Shader "Shader Forge/Examples/Refraction" {
    Properties {
        _RefractionIntensity ("Refraction Intensity", Range(0, 1)) = 0.1
        _Refraction ("Refraction", 2D) = "bump" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        GrabPass{ }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            
            
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
            uniform float4 _LightColor0;
            uniform sampler2D _GrabTexture;
            uniform float _RefractionIntensity;
            uniform sampler2D _Refraction; uniform float4 _Refraction_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                float4 screenPos : TEXCOORD5;
                LIGHTING_COORDS(6,7)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(_Object2World, float4(v.normal,0)).xyz;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.screenPos = o.pos;
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float2 node_27 = (i.uv0*1.0);
                float3 _Refraction_var = UnpackNormal(tex2D(_Refraction,TRANSFORM_TEX(node_27, _Refraction)));
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + (_Refraction_var.rgb.rg*(_RefractionIntensity*0.2));
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalLocal = lerp(float3(0,0,1),_Refraction_var.rgb,_RefractionIntensity);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                
                float nSign = sign( dot( viewDirection, i.normalDir ) ); // Reverse normal if this is a backface
                i.normalDir *= nSign;
                normalDirection *= nSign;
                
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = 0.8;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float node_75 = 6.0;
                float3 specularColor = float3(node_75,node_75,node_75);
                float3 directSpecular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),specPow);
                float3 specular = directSpecular * specularColor;
/////// Diffuse:
                NdotL = dot( normalDirection, lightDirection );
                float node_29 = 1.0;
                float3 w = float3(node_29,node_29,node_29)*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                float3 backLight = max(float3(0.0,0.0,0.0), -NdotLWrap + w ) * float3(node_29,node_29,node_29);
                float3 directDiffuse = (forwardLight+backLight) * attenColor;
                float node_219 = lerp(0.02,0.2,(1.0-max(0,dot(normalDirection, viewDirection))));
                float3 diffuse = directDiffuse * float3(node_219,node_219,node_219);
/// Final Color:
                float3 finalColor = diffuse + specular;
                return fixed4(lerp(sceneColor.rgb, finalColor,0.3),1);
            }
            ENDCG
        }
        Pass {
            Name "ForwardAdd"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            Cull Off
            
            
            Fog { Color (0,0,0,0) }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma exclude_renderers gles xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _GrabTexture;
            uniform float _RefractionIntensity;
            uniform sampler2D _Refraction; uniform float4 _Refraction_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                float4 screenPos : TEXCOORD5;
                LIGHTING_COORDS(6,7)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(_Object2World, float4(v.normal,0)).xyz;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.screenPos = o.pos;
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float2 node_27 = (i.uv0*1.0);
                float3 _Refraction_var = UnpackNormal(tex2D(_Refraction,TRANSFORM_TEX(node_27, _Refraction)));
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + (_Refraction_var.rgb.rg*(_RefractionIntensity*0.2));
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalLocal = lerp(float3(0,0,1),_Refraction_var.rgb,_RefractionIntensity);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                
                float nSign = sign( dot( viewDirection, i.normalDir ) ); // Reverse normal if this is a backface
                i.normalDir *= nSign;
                normalDirection *= nSign;
                
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = 0.8;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float node_75 = 6.0;
                float3 specularColor = float3(node_75,node_75,node_75);
                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow);
                float3 specular = directSpecular * specularColor;
/////// Diffuse:
                NdotL = dot( normalDirection, lightDirection );
                float node_29 = 1.0;
                float3 w = float3(node_29,node_29,node_29)*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                float3 backLight = max(float3(0.0,0.0,0.0), -NdotLWrap + w ) * float3(node_29,node_29,node_29);
                float3 directDiffuse = (forwardLight+backLight) * attenColor;
                float node_219 = lerp(0.02,0.2,(1.0-max(0,dot(normalDirection, viewDirection))));
                float3 diffuse = directDiffuse * float3(node_219,node_219,node_219);
/// Final Color:
                float3 finalColor = diffuse + specular;
                return fixed4(finalColor * 0.3,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
