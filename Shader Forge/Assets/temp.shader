// Shader created with Shader Forge Alpha 0.01 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.01;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:3,blpr:0,bsrc:3,bdst:7,culm:2,dpts:2,wrdp:True,uamb:True,ufog:False,mksh:False;n:type:ShaderForge.SFN_Final,id:0,x:32935,y:32958|0-5-0,1-5-0,2-5-0,6-9-0;n:type:ShaderForge.SFN_Vector3,id:3,x:33395,y:32740,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Tex2d,id:4,x:33579,y:32960,tex:28c7aad1372ff114b90d330f8a2dd938;n:type:ShaderForge.SFN_Lerp,id:5,x:33203,y:32869|1-3-0,2-6-0,3-9-0;n:type:ShaderForge.SFN_Vector3,id:6,x:33393,y:32882,v1:1,v2:0,v3:0;n:type:ShaderForge.SFN_Slider,id:8,x:33649,y:33060,min:-0.5,cur:0.131579,max:0.5;n:type:ShaderForge.SFN_Add,id:9,x:33393,y:33011|1-4-3,2-8-0;pass:END;sub:END;*/

Shader "Shader Forge/temp" {
    Properties {
        _node_4 ("", 2D) = "white" {}
        _node_8 ("", Range(-0.5, 0.5)) = -0.5
    }
    SubShader {
        Pass {
            Tags {
                "LightMode"="ForwardBase"
            }
            Cull Off
            
            CGPROGRAM
            #include "UnityCG.cginc"
            #pragma vertex vert
            #pragma fragment frag
            uniform float4 _LightColor0;
            uniform sampler2D _node_4;
            uniform float _node_8;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.uv0 = v.uv0;
                float3 normalDirection = mul(float4(v.normal,0), _World2Object).xyz;
                o.normalDir = normalDirection;
                o.posWorld = mul(_Object2World, v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float node_9 = (tex2D(_node_4,i.uv0.xy).r+_node_8);
                clip(node_9 - 0.5f);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = normalize(i.normalDir);
                if( dot( viewDirection, normalDirection ) < 0 ){ // Reverse normal if this is a backface
                    i.normalDir *= -1;
                    normalDirection *= -1;
                }
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float atten = 1.0;
                float3 node_5 = lerp(float3(0,0,1),float3(1,0,0),node_9);
                float3 lambert = atten * pow(max( 0.0, dot(normalDirection,lightDirection )),node_5) * _LightColor0.xyz;
                float3 addLight = lambert * node_5 * pow(max(0,dot(reflect(-lightDirection, normalDirection),viewDirection)),10);
                float3 lightFinal = lambert + UNITY_LIGHTMODEL_AMBIENT.xyz;
                return fixed4(lightFinal * node_5 + addLight,1);
            }
            ENDCG
        }
    }
    FallBack "Specular"
}
