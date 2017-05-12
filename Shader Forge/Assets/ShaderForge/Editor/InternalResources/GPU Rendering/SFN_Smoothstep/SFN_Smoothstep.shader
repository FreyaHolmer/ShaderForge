Shader "Hidden/Shader Forge/SFN_Smoothstep" {
    Properties {
        _OutputMask ("Output Mask", Vector) = (1,1,1,1)
        _A ("Min", 2D) = "black" {}
        _B ("Max", 2D) = "black" {}
        _V ("Val", 2D) = "black" {}
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
            uniform sampler2D _V;

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
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {

                // Read inputs
                float4 _a = tex2D( _A, i.uv );
                float4 _b = tex2D( _B, i.uv );
                float4 _v = tex2D( _V, i.uv );

                // Operator
                float4 outputColor = smoothstep(_a, _b, _v);

                // Return
                return outputColor * _OutputMask;
            }
            ENDCG
        }
    }
}
