Shader "Hidden/Shader Forge/SFN_Transform" {
    Properties {
        _OutputMask ("Output Mask", Vector) = (1,1,1,1)
        _IN ("In", 2D) = "black" {}
        _FromSpace ("From space", Float) = 0
        _ToSpace ("To space", Float) = 0
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
            uniform float _FromSpace;
            uniform float _ToSpace;

            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float3 tangentDir : TEXCOORD2;
                float3 bitangentDir : TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);

                // Read inputs
                float4 _in = tex2D( _IN, i.uv0 );

                // Operator
                float4 outputColor;

                if(_FromSpace == _ToSpace){
                	return _in;
                }

                // 0, World
                // 1, Local
                // 2, Tangent
                // 3, View

                if(_FromSpace == 0){
                	if(_ToSpace == 1){ 
                		outputColor = mul( unity_WorldToObject, _in );		// World To Local
                	} else if(_ToSpace == 2){ 
                		outputColor.xyz = mul( tangentTransform, _in.xyz );	// World To Tangent
                	} else if(_ToSpace == 3){ 
                		outputColor = mul( UNITY_MATRIX_V, _in );		// World To View
                	}
                } else if( _FromSpace = 1 ){
                	if(_ToSpace == 0){ 
                		outputColor = mul( unity_ObjectToWorld, _in );								// Local To World
                	} else if(_ToSpace == 2){ 
                		outputColor.xyz = mul( tangentTransform, mul( unity_ObjectToWorld, _in ).xyz );	// Local To Tangent
                	} else if(_ToSpace == 3){ 
                		outputColor = UnityObjectToViewPos( _in ).xyzz;								// Local To View
                	}
                } else if( _FromSpace = 2 ){
                	if(_ToSpace == 0){ 
                		outputColor.xyz = mul( _in.xyz, tangentTransform );										// Tangent To World
                	} else if(_ToSpace == 1){ 
                		outputColor = mul( unity_WorldToObject, float4(mul( _in.xyz, tangentTransform ), 0) );	// Tangent To Local
                	} else if(_ToSpace == 3){ 
                		outputColor = mul( UNITY_MATRIX_V, float4(mul( _in.xyz, tangentTransform ), 0) );	// Tangent To View
                	}
                } else if( _FromSpace = 3 ){
                	if(_ToSpace == 0){ 
                		outputColor = mul( _in, UNITY_MATRIX_V );									// View To World
                	} else if(_ToSpace == 1){ 
                		outputColor = mul( _in, UNITY_MATRIX_MV );									// View To Local
                	} else if(_ToSpace == 2){ 
                		outputColor.xyz = mul( tangentTransform, mul( _in, UNITY_MATRIX_V ).xyz );		// View To Tangent
                	}
                }




                outputColor.w = 0;

                // Return
                return outputColor * _OutputMask;
            }
            ENDCG
        }
    }
}
