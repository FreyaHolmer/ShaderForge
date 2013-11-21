// Shader created with Shader Forge Alpha 0.14 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.14;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:0,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,uamb:False,ufog:True,aust:True,igpj:False,qofs:0,lico:0,qpre:1,flbk:,rntp:1,lmpd:False;n:type:ShaderForge.SFN_Final,id:1,x:32661,y:32323|emission-14-0;n:type:ShaderForge.SFN_Tex2d,id:3,x:33995,y:32950,ptlb:Texture 1,tex:5fb7986dd6d0a8e4093ba82369dd6a4d;n:type:ShaderForge.SFN_Subtract,id:4,x:34217,y:32522|1-35-4,2-5-0;n:type:ShaderForge.SFN_Vector1,id:5,x:34431,y:32611,v1:0.5;n:type:ShaderForge.SFN_ConstantClamp,id:6,x:34044,y:32522,min:0,max:50|1-4-0;n:type:ShaderForge.SFN_Multiply,id:7,x:33854,y:32594|1-6-0,2-8-0;n:type:ShaderForge.SFN_Vector1,id:8,x:34044,y:32669,v1:2;n:type:ShaderForge.SFN_OneMinus,id:9,x:33693,y:32594|1-7-0;n:type:ShaderForge.SFN_Multiply,id:10,x:33532,y:32594|1-9-0,2-12-0;n:type:ShaderForge.SFN_OneMinus,id:12,x:33693,y:32735|1-3-6;n:type:ShaderForge.SFN_OneMinus,id:13,x:33368,y:32594|1-10-0;n:type:ShaderForge.SFN_If,id:14,x:33131,y:32559|1-35-4,2-31-0,3-13-0,4-24-0,5-24-0;n:type:ShaderForge.SFN_Multiply,id:22,x:34044,y:32778|1-35-4,2-23-0;n:type:ShaderForge.SFN_Vector1,id:23,x:34232,y:32858,v1:2;n:type:ShaderForge.SFN_Multiply,id:24,x:33395,y:32965|1-22-0,2-3-6;n:type:ShaderForge.SFN_Vector1,id:31,x:33338,y:32513,v1:0.5;n:type:ShaderForge.SFN_VertexColor,id:35,x:34463,y:32375;n:type:ShaderForge.SFN_VertexColor,id:36,x:33131,y:32775;n:type:ShaderForge.SFN_Lerp,id:37,x:32927,y:32401|1-39-0,2-40-0;n:type:ShaderForge.SFN_Vector3,id:39,x:33134,y:32369,v1:0,v2:1,v3:0;n:type:ShaderForge.SFN_Vector3,id:40,x:33134,y:32457,v1:1,v2:0,v3:0;n:type:ShaderForge.SFN_Clamp01,id:72,x:34044,y:32395|1-4-0;pass:END;sub:END;*/

Shader "Shader Forge/Blendmod Test" {
    Properties {
        _Texture1 ("Texture 1", 2D) = "white" {}
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
            uniform sampler2D _Texture1;
            struct VertexInput {
                float4 vertex : POSITION;
                float4 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(1,2)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.vertexColor = v.vertexColor;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float4 node_35 = i.vertexColor;
                float node_14_if_leA = step(node_35.a,0.5);
                float node_14_if_leB = step(0.5,node_35.a);
                float4 node_3 = tex2D(_Texture1,i.uv0.xy);
                float node_24 = ((node_35.a*2.0)*node_3.a);
                float node_4 = (node_35.a-0.5);
                float node_14 = lerp((node_14_if_leA*node_24)+(node_14_if_leB*(1.0 - ((1.0 - (clamp(node_4,0,50)*2.0))*(1.0 - node_3.a)))),node_24,node_14_if_leA*node_14_if_leB);
                return fixed4(float3(node_14,node_14,node_14),1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
