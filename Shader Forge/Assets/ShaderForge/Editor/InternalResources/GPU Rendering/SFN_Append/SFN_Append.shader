Shader "Hidden/Shader Forge/SFN_Append" {
    Properties {
    	_OutputMask ("Output Mask", Vector) = (1,1,1,1)
        _A ("A", 2D) = "black" {}
        _B ("B", 2D) = "black" {}
		_C ("C", 2D) = "black" {}
		_D ("D", 2D) = "black" {}
		_A_mask ("A mask", Vector) = (0,0,0,0)
		_B_mask ("B mask", Vector) = (0,0,0,0)
		_C_mask ("C mask", Vector) = (0,0,0,0)
		_D_mask ("D mask", Vector) = (0,0,0,0)
		_offsets ("Offsets", Vector) = (0,0,0,0)
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
            uniform sampler2D _A;
            uniform sampler2D _B;
			uniform sampler2D _C;
            uniform sampler2D _D;
			uniform float4 _A_mask;
			uniform float4 _B_mask;
			uniform float4 _C_mask;
			uniform float4 _D_mask;
			uniform float4 _offsets;

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

				// float to int
				int oa = _offsets.x;
				int ob = _offsets.y;
				int oc = _offsets.z;
				int od = _offsets.w;

            	// Read inputs
                float4 a = tex2D( _A, i.uv );// * _A_mask;
                float4 b = tex2D( _B, i.uv );// * _B_mask;
				float4 c = tex2D( _C, i.uv );// * _C_mask;
                float4 d = tex2D( _D, i.uv );// * _D_mask;

				// Offsets. A is never offset
				b = float4(
					b[max(0-ob,0)],
					b[max(1-ob,0)],
					b[max(2-ob,0)],
					b[max(3-ob,0)]
				);
				c = float4(
					c[max(0-oc,0)],
					c[max(1-oc,0)],
					c[max(2-oc,0)],
					c[max(3-oc,0)]
				);
				d = float4(
					d[max(0-od,0)],
					d[max(1-od,0)],
					d[max(2-od,0)],
					d[max(3-od,0)]
				);

				a *= _A_mask;
				b *= _B_mask;
				c *= _C_mask;
				d *= _D_mask;


                // Return
                return (a+b+c+d) * _OutputMask;
            }
            ENDCG
        }
    }
}
