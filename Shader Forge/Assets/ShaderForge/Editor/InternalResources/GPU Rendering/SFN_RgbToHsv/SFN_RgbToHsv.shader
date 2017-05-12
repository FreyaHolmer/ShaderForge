Shader "Hidden/Shader Forge/SFN_RgbToHsv" {
    Properties {
        _OutputMask ("Output Mask", Vector) = (1,1,1,1)
        _IN ("", 2D) = "black" {}
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
            uniform sampler2D _IN;

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
                float4 _in = tex2D( _IN, i.uv );

                // Operator
float4 k = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 p = lerp(float4(_in.zy, k.wz), float4(_in.yz, k.xy), step(_in.z, _in.y));
                float4 q = lerp(float4(p.xyw, _in.x), float4(_in.x, p.yzx), step(p.x, _in.x));
                float d = q.x - min(q.w, q.y);
                float e = 1.0e-10;
                float4 outputColor = float4(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x, 0);;

                // Return
                return outputColor * _OutputMask;
            }
            ENDCG
        }
    }
}
