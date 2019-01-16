Shader "Hidden/Shader Forge/SFN_HsvToRgb" {
    Properties {
        _OutputMask ("Output Mask", Vector) = (1,1,1,1)
        _H ("Hue", 2D) = "black" {}
        _S ("Sat", 2D) = "black" {}
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
            uniform sampler2D _H;
            uniform sampler2D _S;
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
                float4 _h = tex2D( _H, i.uv );
                float4 _s = tex2D( _S, i.uv );
                float4 _v = tex2D( _V, i.uv );

                // Operator
                float4 outputColor = float4((lerp(float3(1,1,1),saturate(3.0*abs(1.0-2.0*frac(_h.x+float3(0.0,-1.0/3.0,1.0/3.0)))-1),_s.x)*_v.x),0);

                // Return
                return outputColor * _OutputMask;
            }
            ENDCG
        }
    }
}
