// Shader created with Shader Forge Alpha 0.15 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.15;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:4,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,uamb:True,ufog:True,aust:True,igpj:False,qofs:0,lico:1,qpre:1,flbk:,rntp:1,lmpd:False,enco:False,frtr:True,vitr:False,dbil:True,rmgx:True;n:type:ShaderForge.SFN_Final,id:1,x:32540,y:32732|diff-4-OUT,spec-35-OUT,gloss-41-OUT,normal-51-RGB,emission-58-OUT;n:type:ShaderForge.SFN_Slider,id:2,x:33229,y:32911,ptlb:Gloss,min:0,cur:0.3834587,max:1;n:type:ShaderForge.SFN_Color,id:3,x:33342,y:32601,ptlb:Color,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:4,x:33114,y:32537|A-5-RGB,B-3-RGB;n:type:ShaderForge.SFN_Tex2d,id:5,x:33342,y:32431,ptlb:MainTex,tex:b66bceaf0cc0ace4e9bdc92f14bba709|UVIN-47-OUT;n:type:ShaderForge.SFN_TexCoord,id:27,x:33986,y:32460,uv:0;n:type:ShaderForge.SFN_Vector4Property,id:28,x:34205,y:32656,ptlb:Tiling,v1:1,v2:1,v3:0,v4:0;n:type:ShaderForge.SFN_Cubemap,id:29,x:33752,y:33147,ptlb:Shine,cube:2f821dbbb5e173e468876ef2e4eaa490,pvfc:0;n:type:ShaderForge.SFN_ValueProperty,id:30,x:33539,y:33351,ptlb:Emission,v1:8;n:type:ShaderForge.SFN_Multiply,id:35,x:33041,y:32679|A-5-R,B-79-OUT;n:type:ShaderForge.SFN_Multiply,id:41,x:33041,y:32844|A-5-R,B-2-OUT;n:type:ShaderForge.SFN_Add,id:47,x:33598,y:32674|A-49-OUT,B-48-OUT;n:type:ShaderForge.SFN_Append,id:48,x:33986,y:32753|A-28-Z,B-28-W;n:type:ShaderForge.SFN_Multiply,id:49,x:33790,y:32534|A-27-UVOUT,B-50-OUT;n:type:ShaderForge.SFN_Append,id:50,x:33986,y:32611|A-28-X,B-28-Y;n:type:ShaderForge.SFN_Tex2d,id:51,x:33385,y:33007,ptlb:Normals,tex:bbab0a6f7bae9cf42bf057d8ee2755f6|UVIN-47-OUT;n:type:ShaderForge.SFN_Multiply,id:52,x:33041,y:33012|A-5-R,B-56-OUT;n:type:ShaderForge.SFN_Multiply,id:55,x:33539,y:33201|A-29-RGB,B-29-A;n:type:ShaderForge.SFN_Multiply,id:56,x:33359,y:33231|A-55-OUT,B-30-OUT;n:type:ShaderForge.SFN_Multiply,id:58,x:32883,y:32913|A-41-OUT,B-52-OUT;n:type:ShaderForge.SFN_Vector1,id:79,x:33386,y:32779,v1:0.7;proporder:3-5-2-30-28-51-29;pass:END;sub:END;*/

Shader "Shader Forge/NewShader" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("MainTex", 2D) = "white" {}
        _Gloss ("Gloss", Range(0, 1)) = 0
        _Emission ("Emission", Float ) = 0
        _Tiling ("Tiling", Vector) = (1,1,0,0)
        _Normals ("Normals", 2D) = "white" {}
        _Shine ("Shine", Cube) = "_Skybox" {}
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
            uniform float4 _LightColor0;
            uniform float _Gloss;
            uniform float4 _Color;
            uniform sampler2D _MainTex;
            uniform float4 _Tiling;
            uniform samplerCUBE _Shine;
            uniform float _Emission;
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
                float4 node_28 = _Tiling;
                float2 node_47 = ((i.uv0.rg*float2(node_28.r,node_28.g))+float2(node_28.b,node_28.a));
                float3 normalLocal = UnpackNormal(tex2D(_Normals,node_47)).rgb;
                float3 normalDirection = normalize( mul( normalLocal, tangentTransform ) );
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i)*2;
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
                float3 diffuse = max( 0.0, dot(normalDirection,lightDirection ))*InvPi * attenColor + UNITY_LIGHTMODEL_AMBIENT.xyz;
////// Emissive:
                float4 node_5 = tex2D(_MainTex,node_47);
                float node_41 = (node_5.r*_Gloss);
                float4 node_29 = texCUBE(_Shine,viewReflectDirection);
                float3 emissive = (node_41*(node_5.r*((node_29.rgb*node_29.a)*_Emission)));
///////// Gloss:
                float gloss = exp2(node_41*10.0+1.0);
////// Specular:
                float node_35 = (node_5.r*0.7);
                float3 specularColor = float3(node_35,node_35,node_35);
                float HdotL = max(0.0,dot(halfDirection,lightDirection));
                float3 fresnelTerm = specularColor + ( 1.0 - specularColor ) * pow((1.0 - HdotL),5);
                float normTerm = (gloss + 8.0 ) / (8.0 * Pi);
                float3 specular = attenColor * float3(node_35,node_35,node_35)*NdotL * pow(max(0,dot(halfDirection,normalDirection)),gloss)*fresnelTerm*normTerm;
                float3 lightFinal = diffuse * (node_5.rgb*_Color.rgb) + specular + emissive;
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
            uniform float _Gloss;
            uniform float4 _Color;
            uniform sampler2D _MainTex;
            uniform float4 _Tiling;
            uniform samplerCUBE _Shine;
            uniform float _Emission;
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
                float4 node_28 = _Tiling;
                float2 node_47 = ((i.uv0.rg*float2(node_28.r,node_28.g))+float2(node_28.b,node_28.a));
                float3 normalLocal = UnpackNormal(tex2D(_Normals,node_47)).rgb;
                float3 normalDirection = normalize( mul( normalLocal, tangentTransform ) );
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
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
                float4 node_5 = tex2D(_MainTex,node_47);
                float node_41 = (node_5.r*_Gloss);
                float gloss = exp2(node_41*10.0+1.0);
////// Specular:
                float node_35 = (node_5.r*0.7);
                float3 specularColor = float3(node_35,node_35,node_35);
                float HdotL = max(0.0,dot(halfDirection,lightDirection));
                float3 fresnelTerm = specularColor + ( 1.0 - specularColor ) * pow((1.0 - HdotL),5);
                float normTerm = (gloss + 8.0 ) / (8.0 * Pi);
                float3 specular = attenColor * float3(node_35,node_35,node_35)*NdotL * pow(max(0,dot(halfDirection,normalDirection)),gloss)*fresnelTerm*normTerm;
                float3 lightFinal = diffuse * (node_5.rgb*_Color.rgb) + specular;
/// Final Color:
                return fixed4(lightFinal,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
