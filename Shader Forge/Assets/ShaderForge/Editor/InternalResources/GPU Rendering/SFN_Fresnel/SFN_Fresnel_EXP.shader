Shader "Hidden/Shader Forge/SFN_Fresnel_EXP" {
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
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = float3(0,0,1);
                float3 normalDirection = float3( i.uv * 2 - 1, 0 );
                normalDirection.z = 1 - dot(normalDirection.xy, normalDirection.xy);


                // Read inputs
                float4 _nrm = tex2D( _NRM, i.uv );
                float4 _exp = tex2D( _EXP, i.uv );

                // Operator
                float4 outputColor = pow( 1.0-max(0,dot(normalDirection, viewDirection)), _exp.x );

                // Return
                return outputColor * _OutputMask;
            }
            ENDCG
        }
    }
}
