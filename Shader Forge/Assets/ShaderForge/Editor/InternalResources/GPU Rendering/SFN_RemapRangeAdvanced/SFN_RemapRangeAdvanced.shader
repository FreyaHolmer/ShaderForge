Shader "Hidden/Shader Forge/SFN_RemapRangeAdvanced" {
    Properties {
        _OutputMask ("Output Mask", Vector) = (1,1,1,1)
        _IN ("Val", 2D) = "black" {}
        _IMIN ("iMin", 2D) = "black" {}
        _IMAX ("iMax", 2D) = "black" {}
        _OMIN ("oMin", 2D) = "black" {}
        _OMAX ("oMax", 2D) = "black" {}
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
            uniform sampler2D _IMIN;
            uniform sampler2D _IMAX;
            uniform sampler2D _OMIN;
            uniform sampler2D _OMAX;

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
                float4 _imin = tex2D( _IMIN, i.uv );
                float4 _imax = tex2D( _IMAX, i.uv );
                float4 _omin = tex2D( _OMIN, i.uv );
                float4 _omax = tex2D( _OMAX, i.uv );

                // Operator
                float4 outputColor = (_omin + ( (_in - _imin) * (_omax - _omin) ) / (_imax - _imin));

                // Return
                return outputColor * _OutputMask;
            }
            ENDCG
        }
    }
}
