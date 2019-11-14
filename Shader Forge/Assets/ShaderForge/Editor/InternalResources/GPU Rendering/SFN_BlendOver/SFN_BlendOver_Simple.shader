Shader "Hidden/Shader Forge/SFN_BlendOver_Simple" {
    Properties {
        _OutputMask ("Output Mask", Vector) = (1,1,1,1)
        _SRC ("SRC", 2D) = "black" {}
        _DST ("DST", 2D) = "black" {}
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
            uniform sampler2D _SRC;
            uniform sampler2D _DST;

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
                float4 src = tex2D( _SRC, i.uv );
                float4 dst = tex2D( _DST, i.uv );

				float alpha = src.a + dst.a * (1.0-src.a);
				float3 rgb = lerp( dst.rgb * dst.a, src.rgb, src.a );
				float4 outputColor = float4( rgb, alpha );

                // Return
                return outputColor * _OutputMask;
            }
            ENDCG
        }
    }
}
