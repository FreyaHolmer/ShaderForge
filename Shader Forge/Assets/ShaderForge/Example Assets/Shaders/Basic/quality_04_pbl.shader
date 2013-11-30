// Shader created with Shader Forge Alpha 0.15 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.15;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:4,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,uamb:True,ufog:True,aust:True,igpj:False,qofs:0,lico:1,qpre:1,flbk:,rntp:1,lmpd:False,enco:False,frtr:True,vitr:True,dbil:True,rmgx:True;n:type:ShaderForge.SFN_Final,id:1,x:32644,y:32698|diff-11-OUT,spec-14-OUT,gloss-13-OUT;n:type:ShaderForge.SFN_Slider,id:5,x:33267,y:32755,ptlb:Diffuse Strength,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Tex2d,id:6,x:33298,y:32572,ptlb:Diffuse;n:type:ShaderForge.SFN_Multiply,id:11,x:33092,y:32640|A-6-RGB,B-5-OUT;n:type:ShaderForge.SFN_Slider,id:13,x:33027,y:32953,ptlb:Gloss,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:14,x:33027,y:32862,ptlb:Spec,min:0,cur:0,max:1;proporder:5-6-14-13;pass:END;sub:END;*/

Shader "Shader Forge/Examples/Lighting/q04_pbl" {
    Properties {
        _DiffuseStrength ("Diffuse Strength", Range(0, 1)) = 0
        _Diffuse ("Diffuse", 2D) = "white" {}
        _Spec ("Spec", Range(0, 1)) = 0
        _Gloss ("Gloss", Range(0, 1)) = 0
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
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 3.0
            #pragma glsl
            uniform float4 _LightColor0;
            uniform float _DiffuseStrength;
            uniform sampler2D _Diffuse;
            uniform float _Gloss;
            uniform float _Spec;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = normalize(i.normalDir);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i)*2;
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
                float3 diffuse = max( 0.0, dot(normalDirection,lightDirection ))*InvPi * attenColor;
///////// Gloss:
                float gloss = exp2(_Gloss*10.0+1.0);
////// Specular:
                float _Spec_var = _Spec;
                float3 specularColor = float3(_Spec_var,_Spec_var,_Spec_var);
                float HdotL = max(0.0,dot(halfDirection,lightDirection));
                float3 fresnelTerm = specularColor + ( 1.0 - specularColor ) * pow((1.0 - HdotL),5);
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float NdotV = max(0.0,dot( normalDirection, viewDirection ));
                float alpha = 1.0 / ( sqrt( (Pi/4.0) * gloss + Pi/2.0 ) );
                float visTerm = ( NdotL * ( 1.0 - alpha ) + alpha ) * ( NdotV * ( 1.0 - alpha ) + alpha );
                visTerm = 1.0 / visTerm;
                float normTerm = (gloss + 8.0 ) / (8.0 * Pi);
                float3 specular = attenColor * float3(_Spec_var,_Spec_var,_Spec_var)*NdotL * pow(max(0,dot(halfDirection,normalDirection)),gloss)*fresnelTerm*visTerm*normTerm;
                float3 lightFinal = diffuse * (tex2D(_Diffuse,i.uv0.xy).rgb*_DiffuseStrength) + specular;
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
            #pragma glsl
            uniform float4 _LightColor0;
            uniform float _DiffuseStrength;
            uniform sampler2D _Diffuse;
            uniform float _Gloss;
            uniform float _Spec;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = normalize(i.normalDir);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i)*2;
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
                float3 diffuse = max( 0.0, dot(normalDirection,lightDirection ))*InvPi * attenColor;
///////// Gloss:
                float gloss = exp2(_Gloss*10.0+1.0);
////// Specular:
                float _Spec_var = _Spec;
                float3 specularColor = float3(_Spec_var,_Spec_var,_Spec_var);
                float HdotL = max(0.0,dot(halfDirection,lightDirection));
                float3 fresnelTerm = specularColor + ( 1.0 - specularColor ) * pow((1.0 - HdotL),5);
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float NdotV = max(0.0,dot( normalDirection, viewDirection ));
                float alpha = 1.0 / ( sqrt( (Pi/4.0) * gloss + Pi/2.0 ) );
                float visTerm = ( NdotL * ( 1.0 - alpha ) + alpha ) * ( NdotV * ( 1.0 - alpha ) + alpha );
                visTerm = 1.0 / visTerm;
                float normTerm = (gloss + 8.0 ) / (8.0 * Pi);
                float3 specular = attenColor * float3(_Spec_var,_Spec_var,_Spec_var)*NdotL * pow(max(0,dot(halfDirection,normalDirection)),gloss)*fresnelTerm*visTerm*normTerm;
                float3 lightFinal = diffuse * (tex2D(_Diffuse,i.uv0.xy).rgb*_DiffuseStrength) + specular;
/// Final Color:
                return fixed4(lightFinal,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
