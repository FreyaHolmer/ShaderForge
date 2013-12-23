// Shader created with Shader Forge Beta 0.16 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.16;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:3,blpr:0,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:True,uamb:True,ufog:True,aust:True,igpj:False,qofs:0,lico:1,qpre:1,flbk:,rntp:1,lmpd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,hqsc:True,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300;n:type:ShaderForge.SFN_Final,id:0,x:32953,y:32692|diff-286-OUT,spec-2-R,normal-4-RGB,emission-5-OUT;n:type:ShaderForge.SFN_Cubemap,id:1,x:34231,y:32817,ptlb:Cubemap,cube:f466cf7415226e046b096197eb7341aa,pvfc:1;n:type:ShaderForge.SFN_Tex2d,id:2,x:33221,y:32649,ptlb:Specular,tex:26c22711225093d47bd4f1294ca52131,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:4,x:33221,y:32796,ptlb:Normal,tex:80286949e259c2d44876306923857245,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Multiply,id:5,x:33221,y:32926|A-224-OUT,B-10-OUT;n:type:ShaderForge.SFN_NormalVector,id:6,x:33992,y:33100,pt:False;n:type:ShaderForge.SFN_ComponentMask,id:8,x:33799,y:33100,cc1:1,cc2:-1,cc3:-1,cc4:-1|IN-6-OUT;n:type:ShaderForge.SFN_Add,id:10,x:33429,y:33171|A-12-OUT,B-13-OUT;n:type:ShaderForge.SFN_Vector1,id:11,x:33799,y:33237,v1:0.4;n:type:ShaderForge.SFN_Multiply,id:12,x:33612,y:33110|A-8-OUT,B-11-OUT;n:type:ShaderForge.SFN_OneMinus,id:13,x:33612,y:33237|IN-11-OUT;n:type:ShaderForge.SFN_Vector1,id:150,x:33131,y:32535,v1:0;n:type:ShaderForge.SFN_Multiply,id:213,x:34051,y:32895|A-1-A,B-214-OUT;n:type:ShaderForge.SFN_Vector1,id:214,x:34231,y:32960,v1:8;n:type:ShaderForge.SFN_Multiply,id:215,x:33883,y:32823|A-1-RGB,B-213-OUT;n:type:ShaderForge.SFN_Fresnel,id:223,x:33708,y:32577|EXP-1080-OUT;n:type:ShaderForge.SFN_Lerp,id:224,x:33493,y:32783|A-225-OUT,B-215-OUT,T-223-OUT;n:type:ShaderForge.SFN_Multiply,id:225,x:33708,y:32712|A-226-OUT,B-215-OUT;n:type:ShaderForge.SFN_Vector1,id:226,x:33904,y:32712,v1:0.1;n:type:ShaderForge.SFN_ConstantLerp,id:286,x:33412,y:32511,a:0.4,b:0|IN-223-OUT;n:type:ShaderForge.SFN_Slider,id:1080,x:33886,y:32540,ptlb:Fresnel Exponent,min:1,cur:2.526316,max:8;proporder:1-2-4-1080;pass:END;sub:END;*/

Shader "Shader Forge/Examples/Cubemaps" {
    Properties {
        _Cubemap ("Cubemap", Cube) = "_Skybox" {}
        _Specular ("Specular", 2D) = "white" {}
        _Normal ("Normal", 2D) = "bump" {}
        _FresnelExponent ("Fresnel Exponent", Range(1, 8)) = 1
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
            uniform samplerCUBE _Cubemap;
            uniform sampler2D _Specular; uniform float4 _Specular_ST;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform float _FresnelExponent;
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
                float2 node_1357 = i.uv0;
                float3 normalLocal = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_1357.rg, _Normal))).rgb;
                float3 normalDirection = normalize( mul( normalLocal, tangentTransform ) );
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor + UNITY_LIGHTMODEL_AMBIENT.xyz;
////// Emissive:
                float4 node_1 = texCUBE(_Cubemap,viewReflectDirection);
                float3 node_215 = (node_1.rgb*(node_1.a*8.0));
                float node_223 = pow(1.0-max(0,dot(normalDirection, viewDirection)),_FresnelExponent);
                float node_11 = 0.4;
                float3 emissive = (lerp((0.1*node_215),node_215,node_223)*((i.normalDir.g*node_11)+(1.0 - node_11)));
///////// Gloss:
                float gloss = exp2(0.5*10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float4 _Specular_var = tex2D(_Specular,TRANSFORM_TEX(node_1357.rg, _Specular));
                float3 specularColor = float3(_Specular_var.r,_Specular_var.r,_Specular_var.r);
                float3 specular = attenColor * pow(max(0,dot(reflect(-lightDirection, normalDirection),viewDirection)),gloss) * specularColor;
                float node_286 = lerp(0.4,0,node_223);
                float3 finalColor = diffuse * float3(node_286,node_286,node_286) + specular + emissive;
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
            uniform samplerCUBE _Cubemap;
            uniform sampler2D _Specular; uniform float4 _Specular_ST;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform float _FresnelExponent;
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
                float2 node_1358 = i.uv0;
                float3 normalLocal = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_1358.rg, _Normal))).rgb;
                float3 normalDirection = normalize( mul( normalLocal, tangentTransform ) );
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor;
///////// Gloss:
                float gloss = exp2(0.5*10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float4 _Specular_var = tex2D(_Specular,TRANSFORM_TEX(node_1358.rg, _Specular));
                float3 specularColor = float3(_Specular_var.r,_Specular_var.r,_Specular_var.r);
                float3 specular = attenColor * pow(max(0,dot(reflect(-lightDirection, normalDirection),viewDirection)),gloss) * specularColor;
                float node_223 = pow(1.0-max(0,dot(normalDirection, viewDirection)),_FresnelExponent);
                float node_286 = lerp(0.4,0,node_223);
                float3 finalColor = diffuse * float3(node_286,node_286,node_286) + specular;
/// Final Color:
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
