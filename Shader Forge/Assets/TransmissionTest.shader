// Shader created with Shader Forge Alpha 0.06 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.06;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:3,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,uamb:False,ufog:True,mksh:False;n:type:ShaderForge.SFN_Final,id:0,x:32803,y:32872|0-15-0,2-21-0,3-23-0,4-24-2,10-17-0;n:type:ShaderForge.SFN_Vector3,id:1,x:33733,y:32575,v1:1,v2:0.5,v3:0.4;n:type:ShaderForge.SFN_Slider,id:10,x:33753,y:32857,ptlb:node_2,min:0,cur:0.5488722,max:1;n:type:ShaderForge.SFN_Append,id:11,x:33499,y:32929|1-10-0,2-12-0;n:type:ShaderForge.SFN_Slider,id:12,x:33751,y:32958,ptlb:node_12,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:13,x:33750,y:33034,ptlb:node_13,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Append,id:14,x:33350,y:33043|1-11-0,2-13-0;n:type:ShaderForge.SFN_Multiply,id:15,x:33499,y:32495|1-16-0,2-1-0;n:type:ShaderForge.SFN_Vector1,id:16,x:33733,y:32430,v1:1;n:type:ShaderForge.SFN_Multiply,id:17,x:33167,y:33101|1-14-0,2-18-0;n:type:ShaderForge.SFN_Vector1,id:18,x:33352,y:33185,v1:1;n:type:ShaderForge.SFN_OneMinus,id:20,x:33499,y:32638|1-1-0;n:type:ShaderForge.SFN_Multiply,id:21,x:33306,y:32703|1-20-0,2-22-0;n:type:ShaderForge.SFN_Vector1,id:22,x:33499,y:32781,v1:0.1;n:type:ShaderForge.SFN_Vector1,id:23,x:33176,y:32776,v1:3;n:type:ShaderForge.SFN_Tex2d,id:24,x:33176,y:32885,ptlb:node_24,tex:91730f8ee98c48e42aff09048fbbfaaa;pass:END;sub:END;*/

Shader "Shader Forge/TransmissionTest" {
    Properties {
        _node2 ("node_2", Range(0, 1)) = 0
        _node12 ("node_12", Range(0, 1)) = 0
        _node13 ("node_13", Range(0, 1)) = 0
        _node24 ("node_24", 2D) = "white" {}
    }
    SubShader {
        Pass {
            Tags {
                "LightMode"="ForwardBase"
            }
            
            CGPROGRAM
            #include "UnityCG.cginc"
            #pragma vertex vert
            #pragma fragment frag
            #pragma exclude_renderers flash 
            uniform float4 _LightColor0;
            uniform float _node2;
            uniform float _node12;
            uniform float _node13;
            uniform sampler2D _node24;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.uv0 = v.uv0;
                float3 normalDirection = mul(float4(v.normal,0), _World2Object).xyz;
                o.normalDir = normalDirection;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalLocal = UnpackNormal(tex2D(_node24,i.uv0.xy)).rgb;
                float3x3 local2WorldTranspose = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 normalDirection = normalize( mul( normalLocal, local2WorldTranspose ) );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float atten = 1.0;
                float NdotL = dot( normalDirection, lightDirection );
                float3 lightWrap = (float3(float2(_node2,_node12),_node13)*1)*0.5;
                float3 NdotLWrap = NdotL * ( 1.0 - lightWrap );
                float3 forwardLight = pow( max(float3(0.0,0.0,0.0), NdotLWrap + lightWrap ), 1 );
                float3 lambert = forwardLight;
                lambert *= _LightColor0.xyz;
                float3 addLight = lambert * ((1.0 - float3(1,0.5,0.4))*0.1) * pow(max(0,dot(reflect(-lightDirection, normalDirection),viewDirection)),3);
                float3 lightFinal = lambert;
                return fixed4(lightFinal * (1*float3(1,0.5,0.4)) + addLight,1);
            }
            ENDCG
        }
    }
    FallBack "Specular"
}
