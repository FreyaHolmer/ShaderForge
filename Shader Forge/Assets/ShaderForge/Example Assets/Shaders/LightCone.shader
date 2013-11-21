// Shader created with Shader Forge Alpha 0.15 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.15;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:0,blpr:3,bsrc:0,bdst:6,culm:0,dpts:2,wrdp:False,uamb:False,ufog:False,aust:True,igpj:True,qofs:0,lico:1,qpre:3,flbk:,rntp:2,lmpd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True;n:type:ShaderForge.SFN_Final,id:142,x:33043,y:32438|emission-200-OUT;n:type:ShaderForge.SFN_Tex2d,id:144,x:33454,y:32652,ptlb:Cone Falloff,tex:857a8e9195b715848abbbbb790d378b1|UVIN-180-OUT;n:type:ShaderForge.SFN_Append,id:180,x:33627,y:32652|A-224-OUT,B-182-OUT;n:type:ShaderForge.SFN_TexCoord,id:181,x:33979,y:32751,uv:0;n:type:ShaderForge.SFN_ComponentMask,id:182,x:33800,y:32751,cc1:1,cc2:-1,cc3:-1,cc4:-1|IN-181-UVOUT;n:type:ShaderForge.SFN_Multiply,id:200,x:33285,y:32542|A-217-OUT,B-144-R;n:type:ShaderForge.SFN_Color,id:215,x:33627,y:32470,ptlb:Cone Color,c1:1,c2:1,c3:1,c4:0;n:type:ShaderForge.SFN_Multiply,id:217,x:33454,y:32428|A-219-OUT,B-215-RGB;n:type:ShaderForge.SFN_Slider,id:219,x:33627,y:32368,ptlb:Cone Strength,min:0,cur:1,max:2;n:type:ShaderForge.SFN_Dot,id:221,x:34153,y:32619,dt:0|A-222-OUT,B-223-OUT;n:type:ShaderForge.SFN_ViewVector,id:222,x:34330,y:32557;n:type:ShaderForge.SFN_NormalVector,id:223,x:34330,y:32682,pt:False;n:type:ShaderForge.SFN_OneMinus,id:224,x:33800,y:32619|IN-225-OUT;n:type:ShaderForge.SFN_Power,id:225,x:33979,y:32619|VAL-221-OUT,EXP-226-OUT;n:type:ShaderForge.SFN_Vector1,id:226,x:34153,y:32758,v1:2;proporder:144-215-219;pass:END;sub:END;*/

Shader "Shader Forge/Examples/LightCone" {
    Properties {
        _ConeFalloff ("Cone Falloff", 2D) = "white" {}
        _ConeColor ("Cone Color", Color) = (1,1,1,0)
        _ConeStrength ("Cone Strength", Range(0, 2)) = 0
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
            uniform sampler2D _ConeFalloff;
            uniform float4 _ConeColor;
            uniform float _ConeStrength;
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
//////// DEBUG - Lighting()
                float3 lightFinal = ((_ConeStrength*_ConeColor.rgb)*tex2D(_ConeFalloff,float2((1.0 - pow(dot(viewDirection,i.normalDir),2.0)),i.uv0.rg.g)).r);
//////// DEBUG - Final output color
                return fixed4(lightFinal * float3(1,1,1),1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
