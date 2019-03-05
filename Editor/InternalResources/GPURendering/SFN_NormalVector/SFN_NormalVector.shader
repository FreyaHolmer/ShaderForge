Shader "Hidden/Shader Forge/SFN_NormalVector" {
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
                float2 texcoord0 : TEXCOORD0;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex );
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {

                // Read inputs


                // Operator
                float4 outputColor = float4( normalize(i.normalDir), 0 );

                // Return
                return outputColor * _OutputMask;
            }
            ENDCG
        }
    }
}
