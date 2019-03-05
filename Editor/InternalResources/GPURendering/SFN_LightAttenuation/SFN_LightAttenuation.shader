Shader "Hidden/Shader Forge/SFN_LightAttenuation" {
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
            #include "Lighting.cginc"
            #pragma target 3.0
            uniform float4 _OutputMask;


            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                LIGHTING_COORDS(0,1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos(v.vertex );
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {

                // Read inputs
				float att = LIGHT_ATTENUATION(i);

                // Operator
                float4 outputColor = float4(att,att,att,att);

                // Return
                return outputColor * _OutputMask;
            }
            ENDCG
        }
    }
}
