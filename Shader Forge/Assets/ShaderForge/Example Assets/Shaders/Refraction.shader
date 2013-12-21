// Shader created with Shader Forge Beta 0.16 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.16;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:2,blpr:1,bsrc:3,bdst:7,culm:2,dpts:2,wrdp:False,uamb:False,ufog:False,aust:True,igpj:True,qofs:0,lico:1,qpre:3,flbk:,rntp:2,lmpd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,hqsc:True,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300;n:type:ShaderForge.SFN_Final,id:0,x:33249,y:32514|diff-56-OUT,spec-75-OUT,gloss-76-OUT,normal-25-RGB,transm-29-OUT,lwrap-29-OUT,alpha-22-OUT,refract-14-OUT;n:type:ShaderForge.SFN_Slider,id:13,x:33847,y:32829,ptlb:Refraction Intensity,min:0,cur:0.1,max:0.2;n:type:ShaderForge.SFN_Multiply,id:14,x:33620,y:32661|A-16-OUT,B-13-OUT;n:type:ShaderForge.SFN_ComponentMask,id:16,x:33847,y:32651,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-25-RGB;n:type:ShaderForge.SFN_Vector1,id:22,x:33620,y:32568,v1:0.3;n:type:ShaderForge.SFN_Tex2d,id:25,x:34117,y:32580,ptlb:Refraction,tex:bbab0a6f7bae9cf42bf057d8ee2755f6,ntxv:3,isnm:True|UVIN-27-OUT;n:type:ShaderForge.SFN_TexCoord,id:26,x:35343,y:32338,uv:0;n:type:ShaderForge.SFN_Multiply,id:27,x:35120,y:32359|A-26-UVOUT,B-28-OUT;n:type:ShaderForge.SFN_Vector1,id:28,x:35343,y:32440,v1:1;n:type:ShaderForge.SFN_Vector1,id:29,x:33479,y:32704,v1:1;n:type:ShaderForge.SFN_Multiply,id:31,x:34928,y:32259|A-32-OUT,B-27-OUT;n:type:ShaderForge.SFN_Vector1,id:32,x:35120,y:32241,v1:2;n:type:ShaderForge.SFN_Color,id:54,x:33887,y:31920,ptlb:Tile A,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Color,id:55,x:33887,y:32083,ptlb:Tile B,c1:0.4264706,c2:0.4264706,c3:0.4264706,c4:1;n:type:ShaderForge.SFN_Lerp,id:56,x:33681,y:32093|A-54-RGB,B-55-RGB,T-74-OUT;n:type:ShaderForge.SFN_Frac,id:57,x:34753,y:32259|IN-31-OUT;n:type:ShaderForge.SFN_Round,id:66,x:34569,y:32259|IN-57-OUT;n:type:ShaderForge.SFN_ComponentMask,id:68,x:34398,y:32223,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-66-OUT;n:type:ShaderForge.SFN_ComponentMask,id:70,x:34398,y:32364,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-66-OUT;n:type:ShaderForge.SFN_Add,id:71,x:34238,y:32238|A-68-OUT,B-70-OUT;n:type:ShaderForge.SFN_OneMinus,id:73,x:34066,y:32238|IN-71-OUT;n:type:ShaderForge.SFN_Abs,id:74,x:33887,y:32238|IN-73-OUT;n:type:ShaderForge.SFN_Vector1,id:75,x:33620,y:32452,v1:3;n:type:ShaderForge.SFN_Vector1,id:76,x:33620,y:32511,v1:0.5;proporder:13-25-55-54;pass:END;sub:END;*/

Shader "Shader Forge/Examples/Refraction" {
    Properties {
        _RefractionIntensity ("Refraction Intensity", Range(0, 0.2)) = 0
        _Refraction ("Refraction", 2D) = "bump" {}
        _TileB ("Tile B", Color) = (0.4264706,0.4264706,0.4264706,1)
        _TileA ("Tile A", Color) = (1,1,1,1)
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        GrabPass{ }
        Pass {
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers gles xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _GrabTexture;
            uniform float _RefractionIntensity;
            uniform sampler2D _Refraction; uniform float4 _Refraction_ST;
            uniform float4 _TileA;
            uniform float4 _TileB;
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
                float4 screenPos : TEXCOORD5;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.screenPos = o.pos;
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float2 node_27 = (i.uv0.rg*1.0);
                float3 node_25 = UnpackNormal(tex2D(_Refraction,TRANSFORM_TEX(node_27, _Refraction)));
                float3 normalLocal = node_25.rgb;
                float3 normalDirection = normalize( mul( normalLocal, tangentTransform ) );
                
                float nSign = sign( dot( viewDirection, normalDirection ) ); // Reverse normal if this is a backface
                i.normalDir *= nSign;
                normalDirection *= nSign;
                
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float node_29 = 1.0;
                float3 w = float3(node_29,node_29,node_29)*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = pow( max(float3(0.0,0.0,0.0), NdotLWrap + w ), 1 );
                float3 backLight = pow( max(float3(0.0,0.0,0.0), -NdotLWrap + w ), 1 ) * float3(node_29,node_29,node_29);
                float3 diffuse = (forwardLight+backLight) * attenColor;
///////// Gloss:
                float gloss = exp2(0.5*10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float node_75 = 3.0;
                float3 specularColor = float3(node_75,node_75,node_75);
                float3 specular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),gloss) * specularColor;
                float2 node_66 = round(frac((2.0*node_27)));
                float3 lightFinal = diffuse * lerp(_TileA.rgb,_TileB.rgb,abs((1.0 - (node_66.r+node_66.r)))) + specular;
/// Final Color:
                return fixed4(lerp(tex2D(_GrabTexture, float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + (node_25.rgb.rg*_RefractionIntensity)).rgb, lightFinal,0.3),1);
            }
            ENDCG
        }
        Pass {
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            Cull Off
            ZWrite Off
            Fog { Color (0,0,0,0) }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd
            #pragma exclude_renderers gles xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _GrabTexture;
            uniform float _RefractionIntensity;
            uniform sampler2D _Refraction; uniform float4 _Refraction_ST;
            uniform float4 _TileA;
            uniform float4 _TileB;
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
                float4 screenPos : TEXCOORD5;
                LIGHTING_COORDS(6,7)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.screenPos = o.pos;
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float2 node_27 = (i.uv0.rg*1.0);
                float3 node_25 = UnpackNormal(tex2D(_Refraction,TRANSFORM_TEX(node_27, _Refraction)));
                float3 normalLocal = node_25.rgb;
                float3 normalDirection = normalize( mul( normalLocal, tangentTransform ) );
                
                float nSign = sign( dot( viewDirection, normalDirection ) ); // Reverse normal if this is a backface
                i.normalDir *= nSign;
                normalDirection *= nSign;
                
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float node_29 = 1.0;
                float3 w = float3(node_29,node_29,node_29)*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = pow( max(float3(0.0,0.0,0.0), NdotLWrap + w ), 1 );
                float3 backLight = pow( max(float3(0.0,0.0,0.0), -NdotLWrap + w ), 1 ) * float3(node_29,node_29,node_29);
                float3 diffuse = (forwardLight+backLight) * attenColor;
///////// Gloss:
                float gloss = exp2(0.5*10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float node_75 = 3.0;
                float3 specularColor = float3(node_75,node_75,node_75);
                float3 specular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),gloss) * specularColor;
                float2 node_66 = round(frac((2.0*node_27)));
                float3 lightFinal = diffuse * lerp(_TileA.rgb,_TileB.rgb,abs((1.0 - (node_66.r+node_66.r)))) + specular;
/// Final Color:
                return fixed4(lightFinal,0.3);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
