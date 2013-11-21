// Shader created with Shader Forge Alpha 0.09 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.09;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:3,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,uamb:False,ufog:True,aust:True,igpj:False,qofs:0,lico:1,qpre:1;n:type:ShaderForge.SFN_Final,id:0,x:32358,y:32605|4-22-2,8-8-0;n:type:ShaderForge.SFN_Cubemap,id:2,x:33434,y:32778,ptlb:Diffuse,cube:9562af3a21060fd4c887f7253b973220,pvfc:0|0-5-0;n:type:ShaderForge.SFN_Cubemap,id:3,x:33434,y:32948,ptlb:Spec,cube:974ac51f7c9d1004b89cb94fb7009210,pvfc:0|0-6-0;n:type:ShaderForge.SFN_NormalVector,id:5,x:33617,y:32768;n:type:ShaderForge.SFN_ViewReflectionVector,id:6,x:33617,y:32938;n:type:ShaderForge.SFN_Multiply,id:7,x:33018,y:32768|1-2-2,2-19-0;n:type:ShaderForge.SFN_Add,id:8,x:32672,y:32854|1-23-0,2-24-0;n:type:ShaderForge.SFN_Multiply,id:9,x:33018,y:32938|1-3-2,2-10-0;n:type:ShaderForge.SFN_Power,id:10,x:33216,y:33013|1-3-6,2-11-0;n:type:ShaderForge.SFN_Vector1,id:11,x:33434,y:33050,v1:0.445;n:type:ShaderForge.SFN_Tex2d,id:18,x:33255,y:33203,ptlb:node_18;n:type:ShaderForge.SFN_Power,id:19,x:33202,y:32833|1-2-6,2-11-0;n:type:ShaderForge.SFN_Tex2d,id:21,x:33606,y:32344,ptlb:node_21,tex:b66bceaf0cc0ace4e9bdc92f14bba709;n:type:ShaderForge.SFN_Tex2d,id:22,x:32847,y:32503,ptlb:node_22,tex:bbab0a6f7bae9cf42bf057d8ee2755f6;n:type:ShaderForge.SFN_Multiply,id:23,x:32819,y:32738|1-21-2,2-7-0;n:type:ShaderForge.SFN_Multiply,id:24,x:32835,y:32987|1-27-0,2-9-0;n:type:ShaderForge.SFN_Power,id:25,x:33465,y:32460|1-21-3,2-26-0;n:type:ShaderForge.SFN_Vector1,id:26,x:33812,y:32534,v1:3;n:type:ShaderForge.SFN_Multiply,id:27,x:33282,y:32534|1-25-0,2-28-0;n:type:ShaderForge.SFN_Vector1,id:28,x:33550,y:32582,v1:7;pass:END;sub:END;*/

Shader "Shader Forge/IBL" {
    Properties {
        _Diffuse ("Diffuse", Cube) = "_Skybox" {}
        _Spec ("Spec", Cube) = "_Skybox" {}
        _node18 ("node_18", 2D) = "white" {}
        _node21 ("node_21", 2D) = "white" {}
        _node22 ("node_22", 2D) = "bump" {}
    }
    SubShader {
        Tags {
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
            #pragma exclude_renderers flash gles xbox360 ps3 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform samplerCUBE _Diffuse;
            uniform samplerCUBE _Spec;
            uniform sampler2D _node18;
            uniform sampler2D _node21;
            uniform sampler2D _node22;
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
                float3 normalLocal = UnpackNormal(tex2D(_node22,i.uv0.xy)).rgb;
                float3 normalDirection = normalize( mul( normalLocal, tangentTransform ) );
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float attenuation = LIGHT_ATTENUATION(i);
                float3 lambert = attenuation * max( 0.0, dot(normalDirection,lightDirection )) * _LightColor0.xyz;
                float4 node_21 = tex2D(_node21,i.uv0.xy);
                float4 node_2 = texCUBE(_Diffuse,normalDirection);
                float node_11 = 0.445;
                float4 node_3 = texCUBE(_Spec,viewReflectDirection);
                float3 addLight = ((node_21.rgb*(node_2.rgb*pow(node_2.a,node_11)))+((pow(node_21.r,3)*7)*(node_3.rgb*pow(node_3.a,node_11))));
                float3 lightFinal = lambert;
                return fixed4(lightFinal * 0 + addLight,1);
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
            #pragma exclude_renderers flash gles xbox360 ps3 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform samplerCUBE _Diffuse;
            uniform samplerCUBE _Spec;
            uniform sampler2D _node18;
            uniform sampler2D _node21;
            uniform sampler2D _node22;
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
                float3 normalLocal = UnpackNormal(tex2D(_node22,i.uv0.xy)).rgb;
                float3 normalDirection = normalize( mul( normalLocal, tangentTransform ) );
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection;
                if (0.0 == _WorldSpaceLightPos0.w){
                    lightDirection = normalize( _WorldSpaceLightPos0.xyz );
                } else {
                    lightDirection = normalize( _WorldSpaceLightPos0 - i.posWorld.xyz );
                }
                float attenuation = LIGHT_ATTENUATION(i);
                float3 lambert = attenuation * max( 0.0, dot(normalDirection,lightDirection )) * _LightColor0.xyz;
                
                float3 lightFinal = lambert;
                return fixed4(lightFinal * 0,1);
            }
            ENDCG
        }
    }
    FallBack "Specular"
}
