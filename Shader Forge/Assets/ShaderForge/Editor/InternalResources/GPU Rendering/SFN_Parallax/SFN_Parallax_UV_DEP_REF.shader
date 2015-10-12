Shader "Hidden/Shader Forge/SFN_Parallax_UV_DEP_REF" {
    Properties {
        _OutputMask ("Output Mask", Vector) = (1,1,1,1)
        _UVIN ("UV", 2D) = "black" {}
        _HEI ("Hei", 2D) = "black" {}
        _DEP ("Dep", 2D) = "black" {}
        _REF ("Ref", 2D) = "black" {}
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
            uniform sampler2D _UVIN;
            uniform sampler2D _HEI;
            uniform sampler2D _DEP;
            uniform sampler2D _REF;

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
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {

                // Read inputs
                float4 _uvin = tex2D( _UVIN, i.uv );
                float4 _hei = tex2D( _HEI, i.uv );
                float4 _dep = tex2D( _DEP, i.uv );
                float4 _ref = tex2D( _REF, i.uv );

                // Operator
                float4 outputColor = (_dep.x*(_hei - _ref.x)*mul(tangentTransform, viewDirection).xy + _uv.xy);

                // Return
                return outputColor * _OutputMask;
            }
            ENDCG
        }
    }
}
