// Shader created with Shader Forge Beta 0.34 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.34;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:3,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,blpr:0,bsrc:0,bdst:1,culm:0,dpts:2,wrdp:True,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32865,y:31801|emission-748-OUT;n:type:ShaderForge.SFN_TexCoord,id:544,x:35289,y:31645,uv:0;n:type:ShaderForge.SFN_Multiply,id:545,x:35081,y:31645|A-544-UVOUT,B-546-OUT;n:type:ShaderForge.SFN_Vector1,id:546,x:35289,y:31797,v1:10;n:type:ShaderForge.SFN_Frac,id:556,x:34760,y:31954|IN-545-OUT;n:type:ShaderForge.SFN_RemapRange,id:559,x:34596,y:31954,frmn:0,frmx:1,tomn:-1,tomx:1|IN-556-OUT;n:type:ShaderForge.SFN_Length,id:561,x:33849,y:31960|IN-585-OUT;n:type:ShaderForge.SFN_Floor,id:562,x:33519,y:31960|IN-607-OUT;n:type:ShaderForge.SFN_Rotator,id:563,x:34206,y:32020|UVIN-764-OUT,PIV-574-OUT;n:type:ShaderForge.SFN_Vector2,id:574,x:34380,y:32141,v1:0,v2:0;n:type:ShaderForge.SFN_Multiply,id:585,x:34010,y:31960|A-586-OUT,B-563-UVOUT;n:type:ShaderForge.SFN_Vector2,id:586,x:34206,y:31913,v1:1,v2:3;n:type:ShaderForge.SFN_Clamp01,id:607,x:33682,y:31960|IN-561-OUT;n:type:ShaderForge.SFN_Vector1,id:641,x:34596,y:32116,v1:8;n:type:ShaderForge.SFN_Floor,id:736,x:34596,y:31564|IN-545-OUT;n:type:ShaderForge.SFN_Noise,id:737,x:34380,y:31564|XY-736-OUT;n:type:ShaderForge.SFN_Noise,id:739,x:34380,y:31693|XY-743-OUT;n:type:ShaderForge.SFN_Noise,id:741,x:34380,y:31817|XY-744-OUT;n:type:ShaderForge.SFN_Add,id:743,x:34596,y:31693|A-736-OUT,B-546-OUT;n:type:ShaderForge.SFN_Add,id:744,x:34596,y:31817|A-743-OUT,B-546-OUT;n:type:ShaderForge.SFN_Append,id:745,x:34206,y:31616|A-737-OUT,B-739-OUT;n:type:ShaderForge.SFN_Append,id:746,x:34010,y:31716|A-745-OUT,B-741-OUT;n:type:ShaderForge.SFN_OneMinus,id:747,x:33350,y:31960|IN-562-OUT;n:type:ShaderForge.SFN_Multiply,id:748,x:33158,y:31898|A-753-OUT,B-747-OUT;n:type:ShaderForge.SFN_Power,id:753,x:33846,y:31764|VAL-746-OUT,EXP-754-OUT;n:type:ShaderForge.SFN_Vector1,id:754,x:34010,y:31849,v1:2;n:type:ShaderForge.SFN_Posterize,id:764,x:34380,y:32020|IN-559-OUT,STPS-641-OUT;pass:END;sub:END;*/

Shader "Shader Forge/Examples/Pixel Rotator" {
    Properties {
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
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float node_546 = 10.0;
                float2 node_545 = (i.uv0.rg*node_546);
                float2 node_736 = floor(node_545);
                float2 node_737_skew = node_736 + 0.2127+node_736.x*0.3713*node_736.y;
                float2 node_737_rnd = 4.789*sin(489.123*(node_737_skew));
                float node_737 = frac(node_737_rnd.x*node_737_rnd.y*(1+node_737_skew.x));
                float2 node_743 = (node_736+node_546);
                float2 node_739_skew = node_743 + 0.2127+node_743.x*0.3713*node_743.y;
                float2 node_739_rnd = 4.789*sin(489.123*(node_739_skew));
                float node_739 = frac(node_739_rnd.x*node_739_rnd.y*(1+node_739_skew.x));
                float2 node_744 = (node_743+node_546);
                float2 node_741_skew = node_744 + 0.2127+node_744.x*0.3713*node_744.y;
                float2 node_741_rnd = 4.789*sin(489.123*(node_741_skew));
                float node_741 = frac(node_741_rnd.x*node_741_rnd.y*(1+node_741_skew.x));
                float4 node_9153 = _Time + _TimeEditor;
                float node_563_ang = node_9153.g;
                float node_563_spd = 1.0;
                float node_563_cos = cos(node_563_spd*node_563_ang);
                float node_563_sin = sin(node_563_spd*node_563_ang);
                float2 node_563_piv = float2(0,0);
                float node_641 = 8.0;
                float2 node_563 = (mul(floor((frac(node_545)*2.0+-1.0) * node_641) / (node_641 - 1)-node_563_piv,float2x2( node_563_cos, -node_563_sin, node_563_sin, node_563_cos))+node_563_piv);
                float3 emissive = (pow(float3(float2(node_737,node_739),node_741),2.0)*(1.0 - floor(saturate(length((float2(1,3)*node_563))))));
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
