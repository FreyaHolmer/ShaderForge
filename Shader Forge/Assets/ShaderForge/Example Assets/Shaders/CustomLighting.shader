// Shader created with Shader Forge Alpha 0.15 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.15;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:0,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,uamb:True,ufog:False,aust:True,igpj:False,qofs:0,lico:1,qpre:1,flbk:,rntp:1,lmpd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True;n:type:ShaderForge.SFN_Final,id:0,x:32153,y:31966|normal-83-RGB,emission-64-OUT;n:type:ShaderForge.SFN_LightAttenuation,id:37,x:32735,y:32052;n:type:ShaderForge.SFN_Multiply,id:38,x:32567,y:32099|A-37-OUT,B-55-OUT;n:type:ShaderForge.SFN_Dot,id:40,x:33071,y:32255,dt:1|A-42-OUT,B-41-OUT;n:type:ShaderForge.SFN_NormalVector,id:41,x:33239,y:32323,pt:True;n:type:ShaderForge.SFN_LightVector,id:42,x:33239,y:32199;n:type:ShaderForge.SFN_Dot,id:52,x:33071,y:32397,dt:1|A-41-OUT,B-62-OUT;n:type:ShaderForge.SFN_Add,id:55,x:32735,y:32172|A-81-OUT,B-58-OUT;n:type:ShaderForge.SFN_Power,id:58,x:32905,y:32418|VAL-52-OUT,EXP-59-OUT;n:type:ShaderForge.SFN_Vector1,id:59,x:33071,y:32539,v1:50;n:type:ShaderForge.SFN_HalfVector,id:62,x:33239,y:32462;n:type:ShaderForge.SFN_LightColor,id:63,x:32567,y:32221;n:type:ShaderForge.SFN_Multiply,id:64,x:32399,y:32155|A-38-OUT,B-63-RGB;n:type:ShaderForge.SFN_Color,id:80,x:33239,y:32062,ptlb:Color,c1:0.6544118,c2:0.8426978,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:81,x:32904,y:32015|A-84-OUT,B-40-OUT;n:type:ShaderForge.SFN_Tex2d,id:82,x:33239,y:31906,ptlb:Diffuse,tex:b66bceaf0cc0ace4e9bdc92f14bba709;n:type:ShaderForge.SFN_Tex2d,id:83,x:32399,y:32025,ptlb:Normals,tex:bbab0a6f7bae9cf42bf057d8ee2755f6;n:type:ShaderForge.SFN_Multiply,id:84,x:33071,y:31968|A-82-RGB,B-80-RGB;proporder:80-82-83;pass:END;sub:END;*/

Shader "Shader Forge/Examples/Custom Lighting" {
    Properties {
        _Color ("Color", Color) = (0.6544118,0.8426978,1,1)
        _Diffuse ("Diffuse", 2D) = "white" {}
        _Normals ("Normals", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Tags {
                "LightMode"="ForwardBase"
            }
            
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
            #pragma glsl
            uniform float4 _LightColor0;
            uniform float4 _Color;
            uniform sampler2D _Diffuse;
            uniform sampler2D _Normals;
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
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalLocal = UnpackNormal(tex2D(_Normals,i.uv0.xy)).rgb;
                float3 normalDirection = normalize( mul( normalLocal, tangentTransform ) );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 halfDirection = normalize(viewDirection+lightDirection);
//////// DEBUG - Lighting()
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float3 node_41 = normalDirection;
                float3 lightFinal = ((attenuation*(((tex2D(_Diffuse,i.uv0.xy).rgb*_Color.rgb)*max(0,dot(lightDirection,node_41)))+pow(max(0,dot(node_41,halfDirection)),50.0)))*_LightColor0.rgb)+UNITY_LIGHTMODEL_AMBIENT.xyz;
//////// DEBUG - Final output color
                return fixed4(lightFinal * float3(1,1,1),1);
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
            #pragma glsl
            uniform float4 _LightColor0;
            uniform float4 _Color;
            uniform sampler2D _Diffuse;
            uniform sampler2D _Normals;
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
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalLocal = UnpackNormal(tex2D(_Normals,i.uv0.xy)).rgb;
                float3 normalDirection = normalize( mul( normalLocal, tangentTransform ) );
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 halfDirection = normalize(viewDirection+lightDirection);
//////// DEBUG - Lighting()
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float3 node_41 = normalDirection;
                float3 lightFinal = ((attenuation*(((tex2D(_Diffuse,i.uv0.xy).rgb*_Color.rgb)*max(0,dot(lightDirection,node_41)))+pow(max(0,dot(node_41,halfDirection)),50.0)))*_LightColor0.rgb);
//////// DEBUG - Final output color
                return fixed4(lightFinal * float3(1,1,1),1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
