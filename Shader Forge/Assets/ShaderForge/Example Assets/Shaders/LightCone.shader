// Shader created with Shader Forge Alpha 0.15 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.15;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:0,blpr:3,bsrc:0,bdst:6,culm:0,dpts:2,wrdp:False,uamb:False,ufog:False,aust:True,igpj:True,qofs:0,lico:1,qpre:3,flbk:,rntp:2,lmpd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True;n:type:ShaderForge.SFN_Final,id:142,x:32703,y:32448|emission-200-OUT;n:type:ShaderForge.SFN_Tex2d,id:144,x:33454,y:32652,ptlb:Cone Falloff,tex:857a8e9195b715848abbbbb790d378b1|UVIN-180-OUT;n:type:ShaderForge.SFN_Append,id:180,x:33627,y:32652|A-229-OUT,B-182-OUT;n:type:ShaderForge.SFN_TexCoord,id:181,x:33979,y:32751,uv:0;n:type:ShaderForge.SFN_ComponentMask,id:182,x:33800,y:32751,cc1:1,cc2:-1,cc3:-1,cc4:-1|IN-181-UVOUT;n:type:ShaderForge.SFN_Multiply,id:200,x:33004,y:32581|A-217-OUT,B-233-OUT;n:type:ShaderForge.SFN_Color,id:215,x:33627,y:32470,ptlb:Cone Color,c1:1,c2:1,c3:1,c4:0;n:type:ShaderForge.SFN_Multiply,id:217,x:33454,y:32428|A-219-OUT,B-215-RGB;n:type:ShaderForge.SFN_Slider,id:219,x:33627,y:32368,ptlb:Cone Strength,min:0,cur:0.55,max:2;n:type:ShaderForge.SFN_Vector1,id:226,x:33979,y:32643,v1:0.5;n:type:ShaderForge.SFN_Fresnel,id:229,x:33800,y:32609|EXP-226-OUT;n:type:ShaderForge.SFN_Tex2d,id:230,x:33766,y:33109,ptlb:Smoke,tex:28c7aad1372ff114b90d330f8a2dd938|UVIN-246-OUT;n:type:ShaderForge.SFN_Multiply,id:233,x:33219,y:32764|A-144-R,B-251-OUT;n:type:ShaderForge.SFN_FragmentPosition,id:244,x:34642,y:33048;n:type:ShaderForge.SFN_Append,id:245,x:34410,y:32986|A-244-X,B-244-Z;n:type:ShaderForge.SFN_Multiply,id:246,x:33996,y:33109|A-255-UVOUT,B-252-OUT;n:type:ShaderForge.SFN_ConstantLerp,id:251,x:33522,y:33087,a:0.4,b:1|IN-230-R;n:type:ShaderForge.SFN_Vector2,id:252,x:34198,y:33207,v1:0.5,v2:0.5;n:type:ShaderForge.SFN_Panner,id:255,x:34198,y:33048,spu:1,spv:1|UVIN-245-OUT,DIST-297-TSL;n:type:ShaderForge.SFN_Time,id:297,x:34502,y:33188;proporder:144-215-219-230;pass:END;sub:END;*/

Shader "Shader Forge/Examples/LightCone" {
    Properties {
        _ConeFalloff ("Cone Falloff", 2D) = "white" {}
        _ConeColor ("Cone Color", Color) = (1,1,1,0)
        _ConeStrength ("Cone Strength", Range(0, 2)) = 0
        _Smoke ("Smoke", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One OneMinusSrcColor
            ZWrite Off
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers flash 
            #pragma target 3.0
            #pragma glsl
            uniform float4 _TimeEditor;
            uniform sampler2D _ConeFalloff;
            uniform float4 _ConeColor;
            uniform float _ConeStrength;
            uniform sampler2D _Smoke;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = normalize(i.normalDir);
////// Lighting:
                float4 node_244 = i.posWorld;
                float4 node_297 = _Time + _TimeEditor;
                float3 lightFinal = ((_ConeStrength*_ConeColor.rgb)*(tex2D(_ConeFalloff,float2(pow(1.0-max(0,dot(normalDirection, viewDirection)),0.5),i.uv0.rg.g)).r*lerp(0.4,1,tex2D(_Smoke,((float2(node_244.r,node_244.b)+node_297.r*float2(1,1))*float2(0.5,0.5))).r)));
/// Final Color:
                return fixed4(lightFinal,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
