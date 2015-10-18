Shader "Hidden/Shader Forge/SFN_FaceSign" {
    Properties {
        _OutputMask ("Output Mask", Vector) = (1,1,1,1)
        _BackfaceValue ("Backface Value", Float) = 0
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
            uniform float _BackfaceValue;

 
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float faceSign = ( facing >= 0 ? 1 : _BackfaceValue );

                // Operator
                float4 outputColor = float4(faceSign,faceSign,faceSign,faceSign);

                // Return
                return outputColor * _OutputMask;
            }
            ENDCG
        }
    }
}
