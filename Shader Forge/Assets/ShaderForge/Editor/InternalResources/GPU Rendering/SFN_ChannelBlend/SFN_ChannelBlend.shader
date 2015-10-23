Shader "Hidden/Shader Forge/SFN_ChannelBlend" {
    Properties {
        _OutputMask ("Output Mask", Vector) = (1,1,1,1)
        _Type ("Type", Float) = 0 // 0 = Summed, 1 = Layered
        _M ("Mask", 2D) = "black" {}
        _R ("Rcol", 2D) = "black" {}
        _G ("Gcol", 2D) = "black" {}
        _B ("Bcol", 2D) = "black" {}
        _A ("Acol", 2D) = "black" {}
        _BTM ("Btm", 2D) = "black" {}
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
            uniform sampler2D _M;
            uniform sampler2D _R;
            uniform sampler2D _G;
            uniform sampler2D _B;
            uniform sampler2D _A;
            uniform sampler2D _BTM;

            uniform float _Type;

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
                float4 _m = tex2D( _M, i.uv );
                float4 _r = tex2D( _R, i.uv );
                float4 _g = tex2D( _G, i.uv );
                float4 _b = tex2D( _B, i.uv );
                float4 _a = tex2D( _A, i.uv );
                float4 _btm = tex2D( _BTM, i.uv );


				float4 outputColor;

                if(_Type == 0){ // Summed
                	float4 sum = float4(0,0,0,0);
                	sum += _m[0] * _r;
                	sum += _m[1] * _g;
                	sum += _m[2] * _b;
                	sum += _m[3] * _a;
                	outputColor = sum;

                } else { // Layered
                	outputColor = _btm;
					outputColor = lerp(outputColor, _r, _m[0] );
					outputColor = lerp(outputColor, _g, _m[1] );
					outputColor = lerp(outputColor, _b, _m[2] );
					outputColor = lerp(outputColor, _a, _m[3] );
                }

                // Return
                return outputColor * _OutputMask;
            }
            ENDCG
        }
    }
}
