Shader "Hidden/Shader Forge/SFN_Desaturate_REQONLY" {
    Properties {
        _OutputMask ("Output Mask", Vector) = (1,1,1,1)
        _COL ("Col", 2D) = "black" {}
        _DES ("Des", 2D) = "black" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma target 3.0
            uniform float4 _OutputMask;
            uniform sampler2D _COL;
            uniform sampler2D _DES;

            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv = v.texcoord0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {

                // Read inputs
                float4 _col = tex2D( _COL, i.uv );
                float4 _des = tex2D( _DES, i.uv );

                // Operator
                float4 outputColor = dot(_col,float3(0.3,0.59,0.11));

                // Return
                return outputColor * _OutputMask;
            }
            ENDCG
        }
    }
}
