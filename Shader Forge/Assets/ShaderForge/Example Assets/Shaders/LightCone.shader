// Shader created with Shader Forge v1.06 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.06;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:0,uamb:False,mssp:True,lmpd:False,lprd:True,rprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:2,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:False,dith:0,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.1280277,fgcg:0.1953466,fgcb:0.2352941,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:142,x:34662,y:32396,varname:node_142,prsc:2|emission-217-OUT;n:type:ShaderForge.SFN_Tex2d,id:144,x:34132,y:32516,ptovrint:False,ptlb:Cone Falloff,ptin:_ConeFalloff,varname:_ConeFalloff,prsc:2,tex:857a8e9195b715848abbbbb790d378b1,ntxv:0,isnm:False|UVIN-180-OUT;n:type:ShaderForge.SFN_Append,id:180,x:33923,y:32452,varname:node_180,prsc:2|A-229-OUT,B-181-V;n:type:ShaderForge.SFN_TexCoord,id:181,x:33738,y:32523,varname:node_181,prsc:2,uv:0;n:type:ShaderForge.SFN_Color,id:215,x:34132,y:32338,ptovrint:False,ptlb:Cone Color,ptin:_ConeColor,varname:_ConeColor,prsc:2,glob:False,c1:1,c2:1,c3:1,c4:0;n:type:ShaderForge.SFN_Multiply,id:217,x:34464,y:32495,varname:node_217,prsc:2|A-219-OUT,B-215-RGB,C-144-R,D-251-OUT;n:type:ShaderForge.SFN_Slider,id:219,x:33975,y:32230,ptovrint:False,ptlb:Cone Strength,ptin:_ConeStrength,varname:_ConeStrength,prsc:2,min:0,cur:0.55,max:2;n:type:ShaderForge.SFN_Vector1,id:226,x:33559,y:32412,varname:node_226,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Fresnel,id:229,x:33738,y:32391,varname:node_229,prsc:2|EXP-226-OUT;n:type:ShaderForge.SFN_Tex2d,id:230,x:33959,y:32713,ptovrint:False,ptlb:Smoke,ptin:_Smoke,varname:_Smoke,prsc:2,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-246-OUT;n:type:ShaderForge.SFN_FragmentPosition,id:244,x:33219,y:32719,varname:node_244,prsc:2;n:type:ShaderForge.SFN_Append,id:245,x:33403,y:32719,varname:node_245,prsc:2|A-244-X,B-244-Z;n:type:ShaderForge.SFN_Multiply,id:246,x:33786,y:32713,varname:node_246,prsc:2|A-318-OUT,B-255-UVOUT;n:type:ShaderForge.SFN_ConstantLerp,id:251,x:34132,y:32713,varname:node_251,prsc:2,a:0.4,b:1|IN-230-R;n:type:ShaderForge.SFN_Panner,id:255,x:33589,y:32774,varname:node_255,prsc:2,spu:1,spv:1|UVIN-245-OUT,DIST-339-TSL;n:type:ShaderForge.SFN_ValueProperty,id:318,x:33589,y:32713,ptovrint:False,ptlb:Smoke Scale,ptin:_SmokeScale,varname:_SmokeScale,prsc:2,glob:False,v1:0.5;n:type:ShaderForge.SFN_Time,id:339,x:33403,y:32845,varname:node_339,prsc:2;proporder:144-215-219-230-318;pass:END;sub:END;*/

Shader "Shader Forge/Examples/LightCone" {
    Properties {
        _ConeFalloff ("Cone Falloff", 2D) = "white" {}
        _ConeColor ("Cone Color", Color) = (1,1,1,0)
        _ConeStrength ("Cone Strength", Range(0, 2)) = 0.55
        _Smoke ("Smoke", 2D) = "white" {}
        _SmokeScale ("Smoke Scale", Float ) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            ZWrite Off
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define SHOULD_SAMPLE_SH_PROBE ( defined (LIGHTMAP_OFF) )
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _ConeFalloff; uniform float4 _ConeFalloff_ST;
            uniform float4 _ConeColor;
            uniform float _ConeStrength;
            uniform sampler2D _Smoke; uniform float4 _Smoke_ST;
            uniform float _SmokeScale;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(_Object2World, float4(v.normal,0)).xyz;
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float2 node_180 = float2(pow(1.0-max(0,dot(normalDirection, viewDirection)),0.5),i.uv0.g);
                float4 _ConeFalloff_var = tex2D(_ConeFalloff,TRANSFORM_TEX(node_180, _ConeFalloff));
                float4 node_339 = _Time + _TimeEditor;
                float2 node_246 = (_SmokeScale*(float2(i.posWorld.r,i.posWorld.b)+node_339.r*float2(1,1)));
                float4 _Smoke_var = tex2D(_Smoke,TRANSFORM_TEX(node_246, _Smoke));
                float3 emissive = (_ConeStrength*_ConeColor.rgb*_ConeFalloff_var.r*lerp(0.4,1,_Smoke_var.r));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
