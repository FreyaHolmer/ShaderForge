// Shader created with Shader Forge Beta 0.34 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.34;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:0,uamb:False,mssp:True,lmpd:False,lprd:True,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,blpr:2,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:False,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.1280277,fgcg:0.1953466,fgcb:0.2352941,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:142,x:32976,y:32396|emission-217-OUT;n:type:ShaderForge.SFN_Tex2d,id:144,x:33576,y:32516,ptlb:Cone Falloff,ptin:_ConeFalloff,tex:857a8e9195b715848abbbbb790d378b1,ntxv:0,isnm:False|UVIN-180-OUT;n:type:ShaderForge.SFN_Append,id:180,x:33785,y:32452|A-229-OUT,B-181-V;n:type:ShaderForge.SFN_TexCoord,id:181,x:33970,y:32523,uv:0;n:type:ShaderForge.SFN_Color,id:215,x:33576,y:32338,ptlb:Cone Color,ptin:_ConeColor,glob:False,c1:1,c2:1,c3:1,c4:0;n:type:ShaderForge.SFN_Multiply,id:217,x:33244,y:32495|A-219-OUT,B-215-RGB,C-144-R,D-251-OUT;n:type:ShaderForge.SFN_Slider,id:219,x:33576,y:32230,ptlb:Cone Strength,ptin:_ConeStrength,min:0,cur:0.55,max:2;n:type:ShaderForge.SFN_Vector1,id:226,x:34149,y:32412,v1:0.5;n:type:ShaderForge.SFN_Fresnel,id:229,x:33970,y:32391|EXP-226-OUT;n:type:ShaderForge.SFN_Tex2d,id:230,x:33749,y:32713,ptlb:Smoke,ptin:_Smoke,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-246-OUT;n:type:ShaderForge.SFN_FragmentPosition,id:244,x:34489,y:32719;n:type:ShaderForge.SFN_Append,id:245,x:34305,y:32719|A-244-X,B-244-Z;n:type:ShaderForge.SFN_Multiply,id:246,x:33922,y:32713|A-318-OUT,B-255-UVOUT;n:type:ShaderForge.SFN_ConstantLerp,id:251,x:33576,y:32713,a:0.4,b:1|IN-230-R;n:type:ShaderForge.SFN_Panner,id:255,x:34119,y:32774,spu:1,spv:1|UVIN-245-OUT,DIST-339-TSL;n:type:ShaderForge.SFN_ValueProperty,id:318,x:34119,y:32713,ptlb:Smoke Scale,ptin:_SmokeScale,glob:False,v1:0.5;n:type:ShaderForge.SFN_Time,id:339,x:34305,y:32845;proporder:144-215-219-230-318;pass:END;sub:END;*/

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
                float3 shLight : TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.shLight = ShadeSH9(float4(mul(_Object2World, float4(v.normal,0)).xyz * unity_Scale.w,1)) * 0.5;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float3 normalDirection =  i.normalDir;
////// Lighting:
////// Emissive:
                float2 node_180 = float2(pow(1.0-max(0,dot(normalDirection, viewDirection)),0.5),i.uv0.g);
                float4 node_339 = _Time + _TimeEditor;
                float4 node_244 = i.posWorld;
                float2 node_246 = (_SmokeScale*(float2(node_244.r,node_244.b)+node_339.r*float2(1,1)));
                float3 emissive = (_ConeStrength*_ConeColor.rgb*tex2D(_ConeFalloff,TRANSFORM_TEX(node_180, _ConeFalloff)).r*lerp(0.4,1,tex2D(_Smoke,TRANSFORM_TEX(node_246, _Smoke)).r));
                float3 finalColor = emissive;
/// Final Color:
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
