Shader "Hidden/Shader Forge/SFN_ComponentMask_CC1" {
    Properties {
        _OutputMask ("Output Mask", Vector) = (1,1,1,1)
        _IN ("In", 2D) = "black" {}
        _chr ("ChR", Float) = 0 
        _chg ("ChG", Float) = 0 
        _chb ("ChB", Float) = 0 
        _cha ("ChA", Float) = 0 
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
            uniform float _chr;
            uniform float _chg;
            uniform float _chb;
            uniform float _cha;

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
                float4 outputColor = _in[_chr].xxxx;

                // Return
                return outputColor * _OutputMask;
            }
            ENDCG
        }
    }
}
