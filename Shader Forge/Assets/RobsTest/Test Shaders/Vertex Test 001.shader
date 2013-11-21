// Shader created with Shader Forge Alpha 0.09 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.09;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:0,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:False,uamb:False,ufog:True,aust:True,igpj:True,qofs:0,lico:0,qpre:3;n:type:ShaderForge.SFN_Final,id:0,x:32803,y:32862|8-1-0;n:type:ShaderForge.SFN_VertexColor,id:1,x:33211,y:32869;pass:END;sub:END;*/

Shader "Shader Forge/Vertex Test 001" {
    Properties {
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
        }
        Pass {
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            CGPROGRAM
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma vertex vert
            #pragma fragment frag
            #pragma exclude_renderers flash gles xbox360 ps3 
            #pragma target 3.0
            struct VertexInput {
                float4 vertex : POSITION;
                float4 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.uv0 = v.uv0;
                o.vertexColor = v.vertexColor;
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                return fixed4(i.vertexColor.rgb,1);
            }
            ENDCG
        }
    }
    FallBack "Specular"
}
