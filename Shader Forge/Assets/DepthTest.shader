// Shader created with Shader Forge Alpha 0.10 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.10;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:0,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,uamb:True,ufog:True,aust:True,igpj:False,qofs:0,lico:1,qpre:1,flbk:,rntp:1;n:type:ShaderForge.SFN_Final,id:1,x:32291,y:32806|8-12-0;n:type:ShaderForge.SFN_FragmentPosition,id:4,x:33137,y:32806;n:type:ShaderForge.SFN_ComponentMask,id:5,x:32784,y:32816,cc1:2,cc2:4,cc3:4,cc4:4|1-7-0;n:type:ShaderForge.SFN_Transform,id:7,x:32963,y:32816,s:0|1-4-0;n:type:ShaderForge.SFN_ViewVector,id:8,x:32912,y:32631;n:type:ShaderForge.SFN_Multiply,id:10,x:32564,y:32726|1-11-0,2-5-0;n:type:ShaderForge.SFN_Vector1,id:11,x:32730,y:32643,v1:-1;n:type:ShaderForge.SFN_ScreenPos,id:12,x:32720,y:32969;pass:END;sub:END;*/

Shader "Shader Forge/DepthTest" {
    Properties {
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
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma vertex vert
            #pragma fragment frag
            #pragma exclude_renderers gles xbox360 ps3 flash 
            #pragma target 3.0
            struct VertexInput {
                float4 vertex : POSITION;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                LIGHTING_COORDS(2,3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.uv0 = v.uv0;
                o.screenPos = float4( o.pos.xyz / o.pos.w, 0 );
                o.screenPos.y *= _ProjectionParams.x;
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                return fixed4(i.screenPos.b+UNITY_LIGHTMODEL_AMBIENT.xyz,1);
            }
            ENDCG
        }
    }
    FallBack "Specular"
}
