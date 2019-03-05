Shader "Hidden/Shader Forge/SFN_Fresnel_NRM" {
    Properties {
        _OutputMask ("Output Mask", Vector) = (1,1,1,1)
        _NRM ("Nrm", 2D) = "black" {}
        _EXP ("Exp", 2D) = "black" {}
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
            uniform sampler2D _NRM;
            uniform sampler2D _EXP;

            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv = v.texcoord0;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                o.screenPos = float4( o.pos.xy / o.pos.w, 0, 0 );
                o.screenPos.y *= _ProjectionParams.x;
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {

				#if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5;



                float3 viewDirection = tex2D( _NRM, float2(0.5,0.5) );

                // Read inputs
                float4 _nrm = tex2D( _NRM, sceneUVs );
                float4 _exp = tex2D( _EXP, i.uv );

                // Operator
                float4 outputColor = 1.0-max(0,dot(_nrm.xyz, viewDirection));

                // Return
                return outputColor * _OutputMask;
            }
            ENDCG
        }
    }
}
