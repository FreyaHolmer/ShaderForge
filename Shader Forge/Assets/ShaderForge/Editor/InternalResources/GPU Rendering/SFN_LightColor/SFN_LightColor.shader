Shader "Hidden/Shader Forge/SFN_LightColor" {
    Properties {
        _OutputMask ("Output Mask", Vector) = (1,1,1,1)

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
       		#include "AutoLight.cginc"
            #pragma target 3.0
            uniform float4 _OutputMask;

            uniform float4 _LightColor0;

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
            float4 frag(VertexOutput i) : COLOR {
            	
                // Operator
                float4 outputColor = float4(_LightColor0.rgb,0);

                // Return
                return outputColor * _OutputMask;
            }
            ENDCG
        }
    }
}
