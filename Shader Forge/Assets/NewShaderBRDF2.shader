// Shader created with Shader Forge Alpha 0.10 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.10;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:3,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,uamb:True,ufog:True,aust:True,igpj:False,qofs:0,lico:1,qpre:1,flbk:FULL TEMPLATE,rntp:1;n:type:ShaderForge.SFN_Final,id:0,x:32653,y:32551|0-17-0,2-23-0,4-22-0;n:type:ShaderForge.SFN_Tex2d,id:1,x:33875,y:32576,ptlb:node_1,tex:5fb7986dd6d0a8e4093ba82369dd6a4d|0-14-0;n:type:ShaderForge.SFN_Vector1,id:3,x:33537,y:32797,v1:1;n:type:ShaderForge.SFN_Parallax,id:9,x:33613,y:32596|1-14-0,2-1-6,3-16-0,4-13-0;n:type:ShaderForge.SFN_Tex2d,id:10,x:33349,y:32531,ptlb:node_10,tex:5fb7986dd6d0a8e4093ba82369dd6a4d|0-9-0;n:type:ShaderForge.SFN_Tex2d,id:11,x:33349,y:32700,ptlb:node_11,tex:cf20bfced7e912046a9ce991a4d775ec|0-9-0;n:type:ShaderForge.SFN_TexCoord,id:12,x:34292,y:32354,uv:0;n:type:ShaderForge.SFN_Slider,id:13,x:33953,y:32789,ptlb:node_13,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:14,x:34090,y:32436|1-12-0,2-15-0;n:type:ShaderForge.SFN_Vector1,id:15,x:34256,y:32576,v1:8;n:type:ShaderForge.SFN_Slider,id:16,x:33953,y:32698,ptlb:node_16,min:0,cur:0.1150376,max:1;n:type:ShaderForge.SFN_Multiply,id:17,x:33150,y:32445|1-18-2,2-10-2;n:type:ShaderForge.SFN_Tex2d,id:18,x:33363,y:32354,ptlb:node_18,tex:b66bceaf0cc0ace4e9bdc92f14bba709|0-27-0;n:type:ShaderForge.SFN_Lerp,id:19,x:33147,y:32754|1-11-2,2-20-2,3-21-0;n:type:ShaderForge.SFN_Tex2d,id:20,x:33349,y:32842,ptlb:node_20,tex:bbab0a6f7bae9cf42bf057d8ee2755f6|0-27-0;n:type:ShaderForge.SFN_Vector1,id:21,x:33349,y:32946,v1:0.5;n:type:ShaderForge.SFN_Normalize,id:22,x:32950,y:32727|1-19-0;n:type:ShaderForge.SFN_Vector1,id:23,x:32980,y:32491,v1:0.5;n:type:ShaderForge.SFN_Multiply,id:27,x:33537,y:32408|1-28-0,2-9-0;n:type:ShaderForge.SFN_Vector1,id:28,x:33737,y:32354,v1:0.25;pass:END;sub:END;*/

Shader "Shader Forge/NewShaderBRDF2" {
    Properties {
        _node1 ("node_1", 2D) = "white" {}
        _node10 ("node_10", 2D) = "white" {}
        _node11 ("node_11", 2D) = "bump" {}
        _node13 ("node_13", Range(0, 1)) = 0
        _node16 ("node_16", Range(0, 1)) = 0
        _node18 ("node_18", 2D) = "white" {}
        _node20 ("node_20", 2D) = "bump" {}
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
            #pragma multi_compile_fwdbase_fullshadows
            #pragma vertex vert
            #pragma fragment frag
            #pragma exclude_renderers gles xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _node1;
            uniform sampler2D _node10;
            uniform sampler2D _node11;
            uniform float _node13;
            uniform float _node16;
            uniform sampler2D _node18;
            uniform sampler2D _node20;
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
                float2 node_14 = (i.uv0.rg*8);
                float2 node_9 = (_node16*(tex2D(_node1,node_14).a - _node13)*mul(tangentTransform, viewDirection).xy + node_14);
                float2 node_27 = (0.25*node_9.rg);
                float3 normalLocal = normalize(lerp(UnpackNormal(tex2D(_node11,node_9.rg)).rgb,UnpackNormal(tex2D(_node20,node_27)).rgb,0.5));
                float3 normalDirection = normalize( mul( normalLocal, tangentTransform ) );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float attenuation = LIGHT_ATTENUATION(i);
                float3 lambert = attenuation * max( 0.0, dot(normalDirection,lightDirection )) * _LightColor0.xyz;
                float3 addLight = lambert * 0.5 * pow(max(0,dot(reflect(-lightDirection, normalDirection),viewDirection)),10);
                float3 lightFinal = lambert + UNITY_LIGHTMODEL_AMBIENT.xyz;
                return fixed4(lightFinal * (tex2D(_node18,node_27).rgb*tex2D(_node10,node_9.rg).rgb) + addLight,1);
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
            #pragma multi_compile_fwdadd_fullshadows
            #pragma vertex vert
            #pragma fragment frag
            #pragma exclude_renderers gles xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _node1;
            uniform sampler2D _node10;
            uniform sampler2D _node11;
            uniform float _node13;
            uniform float _node16;
            uniform sampler2D _node18;
            uniform sampler2D _node20;
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
                float2 node_14 = (i.uv0.rg*8);
                float2 node_9 = (_node16*(tex2D(_node1,node_14).a - _node13)*mul(tangentTransform, viewDirection).xy + node_14);
                float2 node_27 = (0.25*node_9.rg);
                float3 normalLocal = normalize(lerp(UnpackNormal(tex2D(_node11,node_9.rg)).rgb,UnpackNormal(tex2D(_node20,node_27)).rgb,0.5));
                float3 normalDirection = normalize( mul( normalLocal, tangentTransform ) );
                float3 lightDirection;
                if (0.0 == _WorldSpaceLightPos0.w){
                    lightDirection = normalize( _WorldSpaceLightPos0.xyz );
                } else {
                    lightDirection = normalize( _WorldSpaceLightPos0 - i.posWorld.xyz );
                }
                float attenuation = LIGHT_ATTENUATION(i);
                float3 lambert = attenuation * max( 0.0, dot(normalDirection,lightDirection )) * _LightColor0.xyz;
                float3 addLight = lambert * 0.5 * pow(max(0,dot(reflect(-lightDirection, normalDirection),viewDirection)),10);
                float3 lightFinal = lambert;
                return fixed4(lightFinal * (tex2D(_node18,node_27).rgb*tex2D(_node10,node_9.rg).rgb) + addLight,1);
            }
            ENDCG
        }
    }
    FallBack "Specular"
}
