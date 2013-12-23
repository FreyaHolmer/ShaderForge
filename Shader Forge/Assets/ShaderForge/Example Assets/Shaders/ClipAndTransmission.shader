// Shader created with Shader Forge Beta 0.16 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.16;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:3,blpr:0,bsrc:3,bdst:7,culm:2,dpts:2,wrdp:True,uamb:True,ufog:True,aust:True,igpj:False,qofs:0,lico:1,qpre:2,flbk:Transparent/Cutout/Diffuse,rntp:3,lmpd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,hqsc:True,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300;n:type:ShaderForge.SFN_Final,id:0,x:32757,y:32619|diff-175-OUT,spec-3-OUT,gloss-270-OUT,normal-2-RGB,transm-7-OUT,lwrap-6-OUT,clip-1-A;n:type:ShaderForge.SFN_Tex2d,id:1,x:33797,y:32621,ptlb:Diffuse,tex:66321cc856b03e245ac41ed8a53e0ecc,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:2,x:33193,y:32797,ptlb:Normal,tex:cb6c5165ed180c543be39ed70e72abc8,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Vector1,id:3,x:33061,y:32641,v1:0.1;n:type:ShaderForge.SFN_Vector3,id:6,x:33193,y:33054,v1:0.9,v2:0.9,v3:0.8;n:type:ShaderForge.SFN_Vector3,id:7,x:33193,y:32955,v1:0.9,v2:1,v3:0.5;n:type:ShaderForge.SFN_Desaturate,id:13,x:33473,y:32607|COL-1-RGB,DES-14-OUT;n:type:ShaderForge.SFN_Vector1,id:14,x:33649,y:32661,v1:0.4;n:type:ShaderForge.SFN_Multiply,id:175,x:33267,y:32607|A-176-OUT,B-13-OUT;n:type:ShaderForge.SFN_Vector1,id:176,x:33473,y:32543,v1:0.8;n:type:ShaderForge.SFN_Vector1,id:270,x:33061,y:32701,v1:0.4;proporder:1-2;pass:END;sub:END;*/

Shader "Shader Forge/Examples/ClipAndTransmission" {
    Properties {
        _Diffuse ("Diffuse", 2D) = "white" {}
        _Normal ("Normal", 2D) = "bump" {}
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
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers gles xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
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
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float2 node_332 = i.uv0;
                float4 node_1 = tex2D(_Diffuse,TRANSFORM_TEX(node_332.rg, _Diffuse));
                clip(node_1.a - 0.5);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalLocal = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_332.rg, _Normal))).rgb;
                float3 normalDirection = normalize( mul( normalLocal, tangentTransform ) );
                
                float nSign = sign( dot( viewDirection, normalDirection ) ); // Reverse normal if this is a backface
                i.normalDir *= nSign;
                normalDirection *= nSign;
                
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 w = float3(0.9,0.9,0.8)*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = pow( max(float3(0.0,0.0,0.0), NdotLWrap + w ), 1 );
                float3 backLight = pow( max(float3(0.0,0.0,0.0), -NdotLWrap + w ), 1 ) * float3(0.9,1,0.5);
                float3 diffuse = (forwardLight+backLight) * attenColor + UNITY_LIGHTMODEL_AMBIENT.xyz;
///////// Gloss:
                float gloss = exp2(0.4*10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float node_3 = 0.1;
                float3 specularColor = float3(node_3,node_3,node_3);
                float3 specular = attenColor * pow(max(0,dot(reflect(-lightDirection, normalDirection),viewDirection)),gloss) * specularColor;
                float3 finalColor = diffuse * (0.8*lerp(node_1.rgb,dot(node_1.rgb,float3(0.3,0.59,0.11)),0.4)) + specular;
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
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
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
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float2 node_333 = i.uv0;
                float4 node_1 = tex2D(_Diffuse,TRANSFORM_TEX(node_333.rg, _Diffuse));
                clip(node_1.a - 0.5);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalLocal = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_333.rg, _Normal))).rgb;
                float3 normalDirection = normalize( mul( normalLocal, tangentTransform ) );
                
                float nSign = sign( dot( viewDirection, normalDirection ) ); // Reverse normal if this is a backface
                i.normalDir *= nSign;
                normalDirection *= nSign;
                
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 w = float3(0.9,0.9,0.8)*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = pow( max(float3(0.0,0.0,0.0), NdotLWrap + w ), 1 );
                float3 backLight = pow( max(float3(0.0,0.0,0.0), -NdotLWrap + w ), 1 ) * float3(0.9,1,0.5);
                float3 diffuse = (forwardLight+backLight) * attenColor;
///////// Gloss:
                float gloss = exp2(0.4*10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float node_3 = 0.1;
                float3 specularColor = float3(node_3,node_3,node_3);
                float3 specular = attenColor * pow(max(0,dot(reflect(-lightDirection, normalDirection),viewDirection)),gloss) * specularColor;
                float3 finalColor = diffuse * (0.8*lerp(node_1.rgb,dot(node_1.rgb,float3(0.3,0.59,0.11)),0.4)) + specular;
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
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
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
                float2 node_334 = i.uv0;
                float4 node_1 = tex2D(_Diffuse,TRANSFORM_TEX(node_334.rg, _Diffuse));
                clip(node_1.a - 0.5);
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
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
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
                float2 node_335 = i.uv0;
                float4 node_1 = tex2D(_Diffuse,TRANSFORM_TEX(node_335.rg, _Diffuse));
                clip(node_1.a - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Transparent/Cutout/Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
