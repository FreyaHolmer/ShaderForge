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
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                
                // Read inputs
                float4 _uvin = tex2D( _UVIN, i.uv0 );
                float4 _hei = tex2D( _HEI, i.uv0 );
                float4 _dep = tex2D( _DEP, i.uv0 );
                float4 _ref = tex2D( _REF, i.uv0 );

                // Operator
                float4 outputColor = float4((_dep.x*(_hei.x - _ref.x)*mul(tangentTransform, viewDirection).xy + _uvin.xy),0,0);

                // Return
                return outputColor * _OutputMask;
            }
            ENDCG
        }
    }
}
