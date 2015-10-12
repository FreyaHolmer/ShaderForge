Shader "Hidden/Shader Forge/SFN_Blend_Exclusion" {
    Properties {
        _OutputMask ("Output Mask", Vector) = (1,1,1,1)
        _SRC ("Src", 2D) = "black" {}
        _DST ("Dst", 2D) = "black" {}
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
            uniform sampler2D _SRC;
            uniform sampler2D _DST;

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
                float4 _src = tex2D( _SRC, i.uv );
                float4 _dst = tex2D( _DST, i.uv );

                // Operator
                float4 outputColor = (0.5 - 2.0*(_src-0.5)*(_dst-0.5));

                // Return
                return outputColor * _OutputMask;
            }
            ENDCG
        }
    }
}
