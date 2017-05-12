Shader "Hidden/Shader Forge/SFN_ArcTan2_NPTP" {
    Properties {
        _OutputMask ("Output Mask", Vector) = (1,1,1,1)
        _A ("y", 2D) = "black" {}
        _B ("x", 2D) = "black" {}
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

                // Operator
                float4 outputColor = atan2(_a,_b);

                // Return
                return outputColor * _OutputMask;
            }
            ENDCG
        }
    }
}
