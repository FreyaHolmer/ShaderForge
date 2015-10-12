Shader "Hidden/Shader Forge/SFN_Rotator_PIV_SPD_ANG" {
    Properties {
        _OutputMask ("Output Mask", Vector) = (1,1,1,1)
        _UVIN ("UV", 2D) = "black" {}
        _PIV ("Piv", 2D) = "black" {}
        _ANG ("Ang", 2D) = "black" {}
        _SPD ("Spd", 2D) = "black" {}
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
            uniform sampler2D _PIV;
            uniform sampler2D _ANG;
            uniform sampler2D _SPD;

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
                float4 _piv = tex2D( _PIV, i.uv );
                float4 _ang = tex2D( _ANG, i.uv );
                float4 _spd = tex2D( _SPD, i.uv );

                // Operator
float ang = _ang.x;
                float spd = _spd.x;
                float cosVal = cos(_spd.x*ang);
                float sinVal = sin(_spd.x*ang);
                float2 piv = _piv.xy;
                float4 outputColor = float4((mul(_uvin.xy-piv,float2x2( cosVal, -sinVal, sinVal, cosVal))+piv),0,0);

                // Return
                return outputColor * _OutputMask;
            }
            ENDCG
        }
    }
}
