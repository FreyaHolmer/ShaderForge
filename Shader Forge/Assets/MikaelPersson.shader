// Shader created with Shader Forge Alpha 0.14 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.14;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:3,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,uamb:True,ufog:True,aust:True,igpj:False,qofs:0,lico:1,qpre:1,flbk:,rntp:1,lmpd:False;n:type:ShaderForge.SFN_Final,id:1,x:32707,y:32788|diff-2-OUT,spec-5-OUT,gloss-8-OUT,emission-16-OUT;n:type:ShaderForge.SFN_Desaturate,id:2,x:32944,y:32640|COL-3-RGB,DES-4-OUT;n:type:ShaderForge.SFN_Tex2d,id:3,x:33117,y:32535,ptlb:Diffuse MAP;n:type:ShaderForge.SFN_ValueProperty,id:4,x:33117,y:32685,ptlb:Desaturate,v1:-0.2;n:type:ShaderForge.SFN_Multiply,id:5,x:32949,y:32800|A-6-RGB,B-7-OUT;n:type:ShaderForge.SFN_Tex2d,id:6,x:33122,y:32762,ptlb:Spec MAP;n:type:ShaderForge.SFN_ValueProperty,id:7,x:33122,y:32904,ptlb:Spec. power,v1:0.6;n:type:ShaderForge.SFN_ComponentMask,id:8,x:33124,y:32962,cc1:0,cc2:4,cc3:4,cc4:4|IN-9-OUT;n:type:ShaderForge.SFN_Add,id:9,x:33294,y:32962|A-10-OUT,B-13-OUT;n:type:ShaderForge.SFN_Fresnel,id:10,x:33483,y:32820|NRM-11-RGB,EXP-12-OUT;n:type:ShaderForge.SFN_Tex2d,id:11,x:33664,y:32739,ptlb:Normal MAP;n:type:ShaderForge.SFN_ValueProperty,id:12,x:33664,y:32882,ptlb:Fresnel Factor,v1:0.3;n:type:ShaderForge.SFN_Multiply,id:13,x:33483,y:32953|A-14-RGB,B-15-OUT;n:type:ShaderForge.SFN_Tex2d,id:14,x:33664,y:32953,ptlb:Gloss MAP;n:type:ShaderForge.SFN_ValueProperty,id:15,x:33664,y:33092,ptlb:Gloss power,v1:1;n:type:ShaderForge.SFN_Multiply,id:16,x:32980,y:33168|A-17-OUT,B-29-OUT;n:type:ShaderForge.SFN_Add,id:17,x:33191,y:33168|A-21-OUT,B-18-OUT;n:type:ShaderForge.SFN_OneMinus,id:18,x:33368,y:33299|IN-22-OUT;n:type:ShaderForge.SFN_Cubemap,id:19,x:33556,y:33459,ptlb:Sky 1;n:type:ShaderForge.SFN_ComponentMask,id:20,x:33545,y:33168,cc1:0,cc2:4,cc3:4,cc4:4|IN-23-OUT;n:type:ShaderForge.SFN_Multiply,id:21,x:33368,y:33168|A-20-OUT,B-22-OUT;n:type:ShaderForge.SFN_ValueProperty,id:22,x:33545,y:33329,ptlb:Refl. power,v1:0.4;n:type:ShaderForge.SFN_NormalVector,id:23,x:33730,y:33168,pt:False;n:type:ShaderForge.SFN_Multiply,id:24,x:33368,y:33459|A-19-RGB,B-25-OUT;n:type:ShaderForge.SFN_ValueProperty,id:25,x:33556,y:33618,ptlb:Sky 1 strength,v1:0.3;n:type:ShaderForge.SFN_Cubemap,id:26,x:33562,y:33715,ptlb:Sky 2;n:type:ShaderForge.SFN_ValueProperty,id:27,x:33562,y:33891,ptlb:Sky 2 Strength,v1:2;n:type:ShaderForge.SFN_Multiply,id:28,x:33368,y:33779|A-26-RGB,B-27-OUT;n:type:ShaderForge.SFN_Multiply,id:29,x:33005,y:33622|A-31-OUT,B-33-OUT;n:type:ShaderForge.SFN_Color,id:30,x:33368,y:33618,ptlb:Sky 1 Colour,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:31,x:33190,y:33523|A-24-OUT,B-30-RGB;n:type:ShaderForge.SFN_Color,id:32,x:33368,y:33922,ptlb:Sky 2 Colour,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:33,x:33177,y:33757|A-28-OUT,B-32-RGB;pass:END;sub:END;*/

Shader "Shader Forge/MikaelPersson" {
    Properties {
        _DiffuseMAP ("Diffuse MAP", 2D) = "white" {}
        _Desaturate ("Desaturate", Float ) = 0
        _SpecMAP ("Spec MAP", 2D) = "white" {}
        _Specpower ("Spec. power", Float ) = 0
        _NormalMAP ("Normal MAP", 2D) = "white" {}
        _FresnelFactor ("Fresnel Factor", Float ) = 0
        _GlossMAP ("Gloss MAP", 2D) = "white" {}
        _Glosspower ("Gloss power", Float ) = 0
        _Sky1 ("Sky 1", Cube) = "_Skybox" {}
        _Reflpower ("Refl. power", Float ) = 0
        _Sky1strength ("Sky 1 strength", Float ) = 0
        _Sky2 ("Sky 2", Cube) = "_Skybox" {}
        _Sky2Strength ("Sky 2 Strength", Float ) = 0
        _Sky1Colour ("Sky 1 Colour", Color) = (0.5,0.5,0.5,1)
        _Sky2Colour ("Sky 2 Colour", Color) = (0.5,0.5,0.5,1)
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
            #pragma exclude_renderers gles xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _DiffuseMAP;
            uniform float _Desaturate;
            uniform sampler2D _SpecMAP;
            uniform float _Specpower;
            uniform sampler2D _NormalMAP;
            uniform float _FresnelFactor;
            uniform sampler2D _GlossMAP;
            uniform float _Glosspower;
            uniform samplerCUBE _Sky1;
            uniform float _Reflpower;
            uniform float _Sky1strength;
            uniform samplerCUBE _Sky2;
            uniform float _Sky2Strength;
            uniform float4 _Sky1Colour;
            uniform float4 _Sky2Colour;
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
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.posWorld = mul(_Object2World, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = normalize(i.normalDir);
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float attenuation = LIGHT_ATTENUATION(i);
                float3 lambert = attenuation * max( 0.0, dot(normalDirection,lightDirection )) * _LightColor0.xyz;
                float node_22 = _Reflpower;
                float3 addLight = lambert * (tex2D(_SpecMAP,i.uv0.xy).rgb*_Specpower) * pow(max(0,dot(reflect(-lightDirection, normalDirection),viewDirection)),(pow(1.0-max(0,dot(tex2D(_NormalMAP,i.uv0.xy).rgb, viewDirection)),_FresnelFactor)+(tex2D(_GlossMAP,i.uv0.xy).rgb*_Glosspower)).r) + (((i.normalDir.r*node_22)+(1.0 - node_22))*(((texCUBE(_Sky1,viewReflectDirection).rgb*_Sky1strength)*_Sky1Colour.rgb)*((texCUBE(_Sky2,viewReflectDirection).rgb*_Sky2Strength)*_Sky2Colour.rgb)));
                float3 lightFinal = lambert + UNITY_LIGHTMODEL_AMBIENT.xyz;
                return fixed4(lightFinal * lerp(tex2D(_DiffuseMAP,i.uv0.xy).rgb,dot(tex2D(_DiffuseMAP,i.uv0.xy).rgb,float3(0.3,0.59,0.11)),_Desaturate) + addLight,1);
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
            #pragma exclude_renderers gles xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _DiffuseMAP;
            uniform float _Desaturate;
            uniform sampler2D _SpecMAP;
            uniform float _Specpower;
            uniform sampler2D _NormalMAP;
            uniform float _FresnelFactor;
            uniform sampler2D _GlossMAP;
            uniform float _Glosspower;
            uniform samplerCUBE _Sky1;
            uniform float _Reflpower;
            uniform float _Sky1strength;
            uniform samplerCUBE _Sky2;
            uniform float _Sky2Strength;
            uniform float4 _Sky1Colour;
            uniform float4 _Sky2Colour;
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
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.posWorld = mul(_Object2World, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = normalize(i.normalDir);
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection;
                if (0.0 == _WorldSpaceLightPos0.w){
                    lightDirection = normalize( _WorldSpaceLightPos0.xyz );
                } else {
                    lightDirection = normalize( _WorldSpaceLightPos0 - i.posWorld.xyz );
                }
                float attenuation = LIGHT_ATTENUATION(i);
                float3 lambert = attenuation * max( 0.0, dot(normalDirection,lightDirection )) * _LightColor0.xyz;
                float3 addLight = lambert * (tex2D(_SpecMAP,i.uv0.xy).rgb*_Specpower) * pow(max(0,dot(reflect(-lightDirection, normalDirection),viewDirection)),(pow(1.0-max(0,dot(tex2D(_NormalMAP,i.uv0.xy).rgb, viewDirection)),_FresnelFactor)+(tex2D(_GlossMAP,i.uv0.xy).rgb*_Glosspower)).r);
                float3 lightFinal = lambert;
                return fixed4(lightFinal * lerp(tex2D(_DiffuseMAP,i.uv0.xy).rgb,dot(tex2D(_DiffuseMAP,i.uv0.xy).rgb,float3(0.3,0.59,0.11)),_Desaturate) + addLight,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
