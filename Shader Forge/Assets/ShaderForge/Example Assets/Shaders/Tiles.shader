// Shader created with Shader Forge v1.06 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.06;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:3,uamb:True,mssp:True,lmpd:False,lprd:False,rprd:False,enco:False,frtr:True,vitr:True,dbil:True,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,dith:0,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:0,x:33620,y:32843,varname:node_0,prsc:2|diff-3580-OUT,spec-145-OUT,gloss-146-OUT,normal-123-RGB,amspl-162-OUT;n:type:ShaderForge.SFN_Tex2d,id:123,x:33183,y:33122,ptovrint:False,ptlb:Normal,ptin:_Normal,varname:_Normal,prsc:2,tex:bbab0a6f7bae9cf42bf057d8ee2755f6,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Tex2d,id:138,x:32804,y:32643,ptovrint:False,ptlb:Diffuse,ptin:_Diffuse,varname:_Diffuse,prsc:2,tex:b66bceaf0cc0ace4e9bdc92f14bba709,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Slider,id:144,x:32384,y:33104,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:_Gloss,prsc:2,min:0,cur:0.3675219,max:1;n:type:ShaderForge.SFN_Slider,id:145,x:33010,y:32820,ptovrint:False,ptlb:Specular,ptin:_Specular,varname:_Specular,prsc:2,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:146,x:33183,y:32945,varname:node_146,prsc:2|A-147-OUT,B-144-OUT;n:type:ShaderForge.SFN_Power,id:147,x:33011,y:32882,varname:node_147,prsc:2|VAL-138-R,EXP-148-OUT;n:type:ShaderForge.SFN_Vector1,id:148,x:32826,y:32916,varname:node_148,prsc:2,v1:2;n:type:ShaderForge.SFN_Cubemap,id:156,x:32993,y:33373,ptovrint:False,ptlb:Specular IBL,ptin:_SpecularIBL,varname:_SpecularIBL,prsc:2,cube:f466cf7415226e046b096197eb7341aa,pvfc:0|MIP-176-OUT;n:type:ShaderForge.SFN_Multiply,id:162,x:33183,y:33373,cmnt:RGBM Decode,varname:node_162,prsc:2|A-156-RGB,B-156-A,C-163-OUT;n:type:ShaderForge.SFN_Vector1,id:163,x:32993,y:33527,varname:node_163,prsc:2,v1:4;n:type:ShaderForge.SFN_RemapRange,id:176,x:32815,y:33373,varname:node_176,prsc:2,frmn:0,frmx:1,tomn:6,tomx:0|IN-144-OUT;n:type:ShaderForge.SFN_OneMinus,id:9122,x:33390,y:32788,varname:node_9122,prsc:2|IN-145-OUT;n:type:ShaderForge.SFN_Multiply,id:3580,x:33390,y:32666,varname:node_3580,prsc:2|A-138-R,B-9122-OUT;proporder:123-138-144-145-156;pass:END;sub:END;*/

Shader "Shader Forge/Examples/Tiles" {
    Properties {
        _Normal ("Normal", 2D) = "bump" {}
        _Diffuse ("Diffuse", 2D) = "white" {}
        _Gloss ("Gloss", Range(0, 1)) = 0.3675219
        _Specular ("Specular", Range(0, 1)) = 0
        _SpecularIBL ("Specular IBL", Cube) = "_Skybox" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "ForwardBase"
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
            #pragma glsl
            uniform float4 _LightColor0;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float _Gloss;
            uniform float _Specular;
            uniform samplerCUBE _SpecularIBL;
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
                LIGHTING_COORDS(5,6)
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
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _Normal_var = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(i.uv0, _Normal)));
                float3 normalLocal = _Normal_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i)*2;
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(i.uv0, _Diffuse));
                float gloss = (pow(_Diffuse_var.r,2.0)*_Gloss);
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float4 _SpecularIBL_var = texCUBElod(_SpecularIBL,float4(viewReflectDirection,(_Gloss*-6.0+6.0)));
                float3 specularColor = float3(_Specular,_Specular,_Specular);
                float specularMonochrome = dot(specularColor,float3(0.3,0.59,0.11));
                float HdotL = max(0.0,dot(halfDirection,lightDirection));
                float3 fresnelTerm = specularColor + ( 1.0 - specularColor ) * pow((1.0 - HdotL),5);
                float NdotV = max(0.0,dot( normalDirection, viewDirection ));
                float3 fresnelTermAmb = specularColor + ( 1.0 - specularColor ) * ( pow((1.0 - NdotV),5) / (4-3*gloss) );
                float alpha = 1.0 / ( sqrt( (Pi/4.0) * specPow + Pi/2.0 ) );
                float visTerm = ( NdotL * ( 1.0 - alpha ) + alpha ) * ( NdotV * ( 1.0 - alpha ) + alpha );
                visTerm = 1.0 / visTerm;
                float normTerm = (specPow + 8.0 ) / (8.0 * Pi);
                float3 directSpecular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),specPow)*fresnelTerm*visTerm*normTerm;
                float3 indirectSpecular = (0 + (_SpecularIBL_var.rgb*_SpecularIBL_var.a*4.0)) * fresnelTermAmb;
                float3 specular = (directSpecular + indirectSpecular) * specularColor;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 indirectDiffuse = float3(0,0,0);
                float3 directDiffuse = max( 0.0, NdotL)*InvPi * attenColor;
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb*2; // Ambient Light
                float node_3580 = (_Diffuse_var.r*(1.0 - _Specular));
                float3 diffuse = (directDiffuse + indirectDiffuse) * float3(node_3580,node_3580,node_3580);
                diffuse *= 1-specularMonochrome;
/// Final Color:
                float3 finalColor = diffuse + specular;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Name "ForwardAdd"
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
            #pragma glsl
            uniform float4 _LightColor0;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float _Gloss;
            uniform float _Specular;
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
                LIGHTING_COORDS(5,6)
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
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _Normal_var = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(i.uv0, _Normal)));
                float3 normalLocal = _Normal_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i)*2;
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(i.uv0, _Diffuse));
                float gloss = (pow(_Diffuse_var.r,2.0)*_Gloss);
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float3 specularColor = float3(_Specular,_Specular,_Specular);
                float specularMonochrome = dot(specularColor,float3(0.3,0.59,0.11));
                float HdotL = max(0.0,dot(halfDirection,lightDirection));
                float3 fresnelTerm = specularColor + ( 1.0 - specularColor ) * pow((1.0 - HdotL),5);
                float NdotV = max(0.0,dot( normalDirection, viewDirection ));
                float alpha = 1.0 / ( sqrt( (Pi/4.0) * specPow + Pi/2.0 ) );
                float visTerm = ( NdotL * ( 1.0 - alpha ) + alpha ) * ( NdotV * ( 1.0 - alpha ) + alpha );
                visTerm = 1.0 / visTerm;
                float normTerm = (specPow + 8.0 ) / (8.0 * Pi);
                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*fresnelTerm*visTerm*normTerm;
                float3 specular = directSpecular * specularColor;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL)*InvPi * attenColor;
                float node_3580 = (_Diffuse_var.r*(1.0 - _Specular));
                float3 diffuse = directDiffuse * float3(node_3580,node_3580,node_3580);
                diffuse *= 1-specularMonochrome;
/// Final Color:
                float3 finalColor = diffuse + specular;
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
