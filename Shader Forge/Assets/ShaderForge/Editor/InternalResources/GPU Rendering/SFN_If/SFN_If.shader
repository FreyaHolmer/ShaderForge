Shader "Hidden/Shader Forge/SFN_If" {
    Properties {
        _OutputMask ("Output Mask", Vector) = (1,1,1,1)
        _A ("A", 2D) = "black" {}
        _B ("B", 2D) = "black" {}
        _GT ("A>B", 2D) = "black" {}
        _EQ ("A=B", 2D) = "black" {}
        _LT ("A<B", 2D) = "black" {}
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
            uniform sampler2D _A;
            uniform sampler2D _B;
            uniform sampler2D _GT;
            uniform sampler2D _EQ;
            uniform sampler2D _LT;

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
                float4 _a = tex2D( _A, i.uv );
                float4 _b = tex2D( _B, i.uv );
                float4 _gt = tex2D( _GT, i.uv );
                float4 _eq = tex2D( _EQ, i.uv );
                float4 _lt = tex2D( _LT, i.uv );

                // Operator
float sta = step(_a,_b);
                float stb = step(_b,_a);
                float4 outputColor = lerp((sta*_lt)+(stb*_gt),_eq,sta*stb);

                // Return
                return outputColor * _OutputMask;
            }
            ENDCG
        }
    }
}
