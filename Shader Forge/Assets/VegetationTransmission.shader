// Shader created with Shader Forge Alpha 0.15 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.15;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:2,blpr:0,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:True,uamb:True,ufog:True,aust:True,igpj:False,qofs:0,lico:1,qpre:2,flbk:,rntp:3,lmpd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True;n:type:ShaderForge.SFN_Final,id:0,x:32747,y:32797|diff-26-OUT,spec-24-OUT,normal-20-OUT,clip-1-A,transm-10-OUT,lwrap-9-OUT;n:type:ShaderForge.SFN_Tex2d,id:1,x:33663,y:32433,ptlb:Diffuse,tex:66321cc856b03e245ac41ed8a53e0ecc;n:type:ShaderForge.SFN_Vector3,id:8,x:33293,y:33011,v1:0.8,v2:1,v3:0.4;n:type:ShaderForge.SFN_Vector1,id:9,x:33121,y:33182,v1:0.8;n:type:ShaderForge.SFN_Multiply,id:10,x:33121,y:33035|A-8-OUT,B-11-OUT;n:type:ShaderForge.SFN_Slider,id:11,x:33293,y:33132,ptlb:Transmission,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Tex2d,id:19,x:33501,y:32792,ptlb:Normal,tex:cb6c5165ed180c543be39ed70e72abc8;n:type:ShaderForge.SFN_Lerp,id:20,x:33291,y:32776|A-21-OUT,B-19-RGB,T-23-OUT;n:type:ShaderForge.SFN_Vector3,id:21,x:33503,y:32674,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Slider,id:23,x:33501,y:32971,ptlb:Normal Intensity,min:0,cur:0.5263158,max:1;n:type:ShaderForge.SFN_Power,id:24,x:33425,y:32532|VAL-1-RGB,EXP-25-OUT;n:type:ShaderForge.SFN_Vector1,id:25,x:33599,y:32584,v1:2;n:type:ShaderForge.SFN_Multiply,id:26,x:33125,y:32460|A-27-OUT,B-1-RGB;n:type:ShaderForge.SFN_Vector1,id:27,x:33335,y:32372,v1:1;proporder:1-11-23-19;pass:END;sub:END;*/

Shader "Shader Forge/VegetationTransmission" {
    Properties {
        _Diffuse ("Diffuse", 2D) = "white" {}
        _Transmission ("Transmission", Range(0, 1)) = 0
        _NormalIntensity ("Normal Intensity", Range(0, 1)) = 0
        _Normal ("Normal", 2D) = "white" {}
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
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _Diffuse;
            uniform float _Transmission;
            uniform sampler2D _Normal;
            uniform float _NormalIntensity;
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
                float4 node_1 = tex2D(_Diffuse,i.uv0.xy);
                clip(node_1.a - 0.5);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalLocal = lerp(float3(0,0,1),UnpackNormal(tex2D(_Normal,i.uv0.xy)).rgb,_NormalIntensity);
                float3 normalDirection = normalize( mul( normalLocal, tangentTransform ) );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float node_9 = 0.8;
                float3 w = float3(node_9,node_9,node_9)*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = pow( max(float3(0.0,0.0,0.0), NdotLWrap + w ), 1 );
                float3 backLight = pow( max(float3(0.0,0.0,0.0), -NdotLWrap + w ), 1 ) * (float3(0.8,1,0.4)*_Transmission);
                float3 diffuse = forwardLight+backLight * attenColor;
///////// Gloss:
                float gloss = exp2(0.5*10.0+1.0);
////// Specular:
                float3 specular = attenColor * pow(node_1.rgb,2.0) * pow(max(0,dot(halfDirection,normalDirection)),gloss);
                float3 lightFinal = diffuse * (1.0*node_1.rgb) + specular;
/// Final Color:
                return fixed4(lightFinal,1);
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
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _Diffuse;
            uniform float _Transmission;
            uniform sampler2D _Normal;
            uniform float _NormalIntensity;
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
                float4 node_1 = tex2D(_Diffuse,i.uv0.xy);
                clip(node_1.a - 0.5);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalLocal = lerp(float3(0,0,1),UnpackNormal(tex2D(_Normal,i.uv0.xy)).rgb,_NormalIntensity);
                float3 normalDirection = normalize( mul( normalLocal, tangentTransform ) );
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float node_9 = 0.8;
                float3 w = float3(node_9,node_9,node_9)*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = pow( max(float3(0.0,0.0,0.0), NdotLWrap + w ), 1 );
                float3 backLight = pow( max(float3(0.0,0.0,0.0), -NdotLWrap + w ), 1 ) * (float3(0.8,1,0.4)*_Transmission);
                float3 diffuse = forwardLight+backLight * attenColor;
///////// Gloss:
                float gloss = exp2(0.5*10.0+1.0);
////// Specular:
                float3 specular = attenColor * pow(node_1.rgb,2.0) * pow(max(0,dot(halfDirection,normalDirection)),gloss);
                float3 lightFinal = diffuse * (1.0*node_1.rgb) + specular;
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
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 3.0
            uniform sampler2D _Diffuse;
            uniform float _Transmission;
            uniform sampler2D _Normal;
            uniform float _NormalIntensity;
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
                float4 node_1 = tex2D(_Diffuse,i.uv0.xy);
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
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 3.0
            uniform sampler2D _Diffuse;
            uniform float _Transmission;
            uniform sampler2D _Normal;
            uniform float _NormalIntensity;
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
                float4 node_1 = tex2D(_Diffuse,i.uv0.xy);
                clip(node_1.a - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
