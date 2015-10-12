Shader "Hidden/Shader Forge/SFN_Add_5" {
    Properties {
        _OutputMask ("Output Mask", Vector) = (1,1,1,1)
        _A ("A", 2D) = "black" {}
        _B ("B", 2D) = "black" {}
        _C ("C", 2D) = "black" {}
        _D ("D", 2D) = "black" {}
        _E ("E", 2D) = "black" {}
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
            uniform sampler2D _C;
            uniform sampler2D _D;
            uniform sampler2D _E;

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
                float4 _c = tex2D( _C, i.uv );
                float4 _d = tex2D( _D, i.uv );
                float4 _e = tex2D( _E, i.uv );

                // Operator
                float4 outputColor = _a + _b + _c + _d + _e;

                // Return
                return outputColor * _OutputMask;
            }
            ENDCG
        }
    }
}
