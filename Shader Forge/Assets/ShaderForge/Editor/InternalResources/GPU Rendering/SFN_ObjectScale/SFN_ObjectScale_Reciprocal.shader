Shader "Hidden/Shader Forge/SFN_ObjectScale_Reciprocal" {
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
            #pragma target 3.0
            uniform float4 _OutputMask;


            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float3 scale : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
               	o.scale = float3( length(_World2Object[0].xyz), length(_World2Object[1].xyz), length(_World2Object[2].xyz) );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                // Operator
                float4 outputColor = float4(i.scale,0);
                // Return
                return outputColor * _OutputMask;
            }
            ENDCG
        }
    }
}
