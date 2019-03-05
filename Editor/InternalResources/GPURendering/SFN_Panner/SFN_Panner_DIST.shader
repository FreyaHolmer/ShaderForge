Shader "Hidden/Shader Forge/SFN_Panner_DIST" {
    Properties {
        _OutputMask ("Output Mask", Vector) = (1,1,1,1)
        _UVIN ("UV", 2D) = "black" {}
        _DIST ("Dist", 2D) = "black" {}
        _uspeed ("Uspeed", Float) = 0 
        _vspeed ("Vspeed", Float) = 0 
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
            uniform sampler2D _DIST;
            uniform float _uspeed;
            uniform float _vspeed;

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
                float4 _uvin = tex2D( _UVIN, i.uv );
                float4 _dist = tex2D( _DIST, i.uv );

                // Operator
                float4 outputColor = float4((_uvin.xy+_dist.x*float2(_uspeed,_vspeed)),0,0);

                // Return
                return outputColor * _OutputMask;
            }
            ENDCG
        }
    }
}
