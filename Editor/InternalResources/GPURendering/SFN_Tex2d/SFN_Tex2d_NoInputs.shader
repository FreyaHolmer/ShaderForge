Shader "Hidden/Shader Forge/SFN_Tex2d_NoInputs" {
    Properties {
    	_OutputMask ("Output Mask", Vector) = (1,1,1,1)
        _MainTex ("Main Texture", 2D) = "white" {}
        _IsNormal ("Is Normal", Float) = 0.0
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
            uniform sampler2D _MainTex;
            uniform float _IsNormal;

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
                float4 a = tex2D( _MainTex, i.uv );
				float4 n = float4(a.a, a.g, 0, 0 ) * 2 - 1;
				n.z = sqrt( 1.0 - n.x*n.x - n.y*n.y );
				float4 result = _IsNormal ? n : a;

                // Return
                return result * _OutputMask;
            }
            ENDCG
        }
    }
}
