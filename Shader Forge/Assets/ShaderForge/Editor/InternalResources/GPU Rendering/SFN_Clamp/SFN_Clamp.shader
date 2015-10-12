Shader "Hidden/Shader Forge/SFN_Clamp" {
    Properties {
        _OutputMask ("Output Mask", Vector) = (1,1,1,1)
        _IN ("", 2D) = "black" {}
        _MIN ("Min", 2D) = "black" {}
        _MAX ("Max", 2D) = "black" {}
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
            uniform sampler2D _IN;
            uniform sampler2D _MIN;
            uniform sampler2D _MAX;

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
                float4 _in = tex2D( _IN, i.uv );
                float4 _min = tex2D( _MIN, i.uv );
                float4 _max = tex2D( _MAX, i.uv );

                // Operator
                float4 outputColor = clamp(_in, _min, _max);

                // Return
                return outputColor * _OutputMask;
            }
            ENDCG
        }
    }
}
