// Shader created with Shader Forge Alpha 0.15 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.15;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:4,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,uamb:True,ufog:True,aust:True,igpj:False,qofs:0,lico:1,qpre:1,flbk:,rntp:1,lmpd:False,enco:False,frtr:True,vitr:True,dbil:True,rmgx:True;n:type:ShaderForge.SFN_Final,id:0,x:32149,y:32542|diff-138-RGB,spec-145-OUT,gloss-146-OUT,normal-123-RGB;n:type:ShaderForge.SFN_TexCoord,id:117,x:33276,y:32885,uv:0;n:type:ShaderForge.SFN_Vector1,id:119,x:33276,y:32822,v1:2;n:type:ShaderForge.SFN_Tex2d,id:123,x:32520,y:32881,ptlb:Normal,tex:bbab0a6f7bae9cf42bf057d8ee2755f6|UVIN-124-OUT;n:type:ShaderForge.SFN_Multiply,id:124,x:33109,y:32843|A-119-OUT,B-117-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:138,x:32869,y:32484,ptlb:Diffuse,tex:b66bceaf0cc0ace4e9bdc92f14bba709|UVIN-124-OUT;n:type:ShaderForge.SFN_Slider,id:144,x:32679,y:32783,ptlb:Gloss,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Slider,id:145,x:32536,y:32579,ptlb:Specular,min:0,cur:0.3458647,max:1;n:type:ShaderForge.SFN_Multiply,id:146,x:32520,y:32704|A-147-OUT,B-144-OUT;n:type:ShaderForge.SFN_Power,id:147,x:32693,y:32641|VAL-138-R,EXP-148-OUT;n:type:ShaderForge.SFN_Vector1,id:148,x:32869,y:32689,v1:2;proporder:123-138-144-145;pass:END;sub:END;*/

Shader "Shader Forge/Examples/Tiles" {
    Properties {
        _Normal ("Normal", 2D) = "white" {}
        _Diffuse ("Diffuse", 2D) = "white" {}
        _Gloss ("Gloss", Range(0, 1)) = 0
        _Specular ("Specular", Range(0, 1)) = 0
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
            #pragma glsl
            uniform float4 _LightColor0;
            uniform sampler2D _Normal;
            uniform sampler2D _Diffuse;
            uniform float _Gloss;
            uniform float _Specular;
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
                float2 node_124 = (2.0*i.uv0.rg);
                float3 normalLocal = UnpackNormal(tex2D(_Normal,node_124)).rgb;
                float3 normalDirection = normalize( mul( normalLocal, tangentTransform ) );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 halfDirection = normalize(viewDirection+lightDirection);
//////// DEBUG - Lighting()
                float attenuation = LIGHT_ATTENUATION(i)*2;
                float3 attenColor = attenuation * _LightColor0.xyz;
//////// DEBUG - CalcDiffuse()
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
                float3 diffuse = max( 0.0, dot(normalDirection,lightDirection ))*InvPi * attenColor;
                float4 node_138 = tex2D(_Diffuse,node_124);
                float gloss = exp2((pow(node_138.r,2.0)*_Gloss)*10.0+1.0);
//////// DEBUG - CalcSpecular()
                float _Specular_var = _Specular;
                float3 specularColor = float3(_Specular_var,_Specular_var,_Specular_var);
                float HdotL = max(0.0,dot(halfDirection,lightDirection));
                float3 fresnelTerm = specularColor + ( 1.0 - specularColor ) * pow((1.0 - HdotL),5);
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float NdotV = max(0.0,dot( normalDirection, viewDirection ));
                float alpha = 1.0 / ( sqrt( (Pi/4.0) * gloss + Pi/2.0 ) );
                float visTerm = ( NdotL * ( 1.0 - alpha ) + alpha ) * ( NdotV * ( 1.0 - alpha ) + alpha );
                visTerm = 1.0 / visTerm;
                float normTerm = (gloss + 8.0 ) / (8.0 * Pi);
                float3 specular = attenColor * float3(_Specular_var,_Specular_var,_Specular_var)*NdotL * pow(max(0,dot(halfDirection,normalDirection)),gloss)*fresnelTerm*visTerm*normTerm;
                float3 lightFinal = diffuse + specular;
//////// DEBUG - Final output color
                return fixed4(lightFinal * node_138.rgb,1);
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
            uniform sampler2D _Normal;
            uniform sampler2D _Diffuse;
            uniform float _Gloss;
            uniform float _Specular;
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
                float2 node_124 = (2.0*i.uv0.rg);
                float3 normalLocal = UnpackNormal(tex2D(_Normal,node_124)).rgb;
                float3 normalDirection = normalize( mul( normalLocal, tangentTransform ) );
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 halfDirection = normalize(viewDirection+lightDirection);
//////// DEBUG - Lighting()
                float attenuation = LIGHT_ATTENUATION(i)*2;
                float3 attenColor = attenuation * _LightColor0.xyz;
//////// DEBUG - CalcDiffuse()
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
                float3 diffuse = max( 0.0, dot(normalDirection,lightDirection ))*InvPi * attenColor;
                float4 node_138 = tex2D(_Diffuse,node_124);
                float gloss = exp2((pow(node_138.r,2.0)*_Gloss)*10.0+1.0);
//////// DEBUG - CalcSpecular()
                float _Specular_var = _Specular;
                float3 specularColor = float3(_Specular_var,_Specular_var,_Specular_var);
                float HdotL = max(0.0,dot(halfDirection,lightDirection));
                float3 fresnelTerm = specularColor + ( 1.0 - specularColor ) * pow((1.0 - HdotL),5);
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float NdotV = max(0.0,dot( normalDirection, viewDirection ));
                float alpha = 1.0 / ( sqrt( (Pi/4.0) * gloss + Pi/2.0 ) );
                float visTerm = ( NdotL * ( 1.0 - alpha ) + alpha ) * ( NdotV * ( 1.0 - alpha ) + alpha );
                visTerm = 1.0 / visTerm;
                float normTerm = (gloss + 8.0 ) / (8.0 * Pi);
                float3 specular = attenColor * float3(_Specular_var,_Specular_var,_Specular_var)*NdotL * pow(max(0,dot(halfDirection,normalDirection)),gloss)*fresnelTerm*visTerm*normTerm;
                float3 lightFinal = diffuse + specular;
//////// DEBUG - Final output color
                return fixed4(lightFinal * node_138.rgb,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
