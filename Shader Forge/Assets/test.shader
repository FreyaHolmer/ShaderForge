// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.02;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:3,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,uamb:True,ufog:True,mksh:False;n:type:ShaderForge.SFN_Final,id:0,x:32106,y:32898|0-35-0,8-21-0;n:type:ShaderForge.SFN_Multiply,id:1,x:33229,y:32906|1-8-0,2-5-0;n:type:ShaderForge.SFN_NormalVector,id:2,x:33671,y:32951;n:type:ShaderForge.SFN_ViewVector,id:3,x:33670,y:33368;n:type:ShaderForge.SFN_ComponentMask,id:5,x:33475,y:33171,cc1:0,cc2:4,cc3:4,cc4:4|1-3-0;n:type:ShaderForge.SFN_ComponentMask,id:6,x:33486,y:33331,cc1:1,cc2:4,cc3:4,cc4:4|1-3-0;n:type:ShaderForge.SFN_ComponentMask,id:7,x:33494,y:33494,cc1:2,cc2:4,cc3:4,cc4:4|1-3-0;n:type:ShaderForge.SFN_ComponentMask,id:8,x:33474,y:32741,cc1:0,cc2:4,cc3:4,cc4:4|1-2-0;n:type:ShaderForge.SFN_ComponentMask,id:9,x:33474,y:32882,cc1:1,cc2:4,cc3:4,cc4:4|1-2-0;n:type:ShaderForge.SFN_ComponentMask,id:10,x:33477,y:33020,cc1:2,cc2:4,cc3:4,cc4:4|1-2-0;n:type:ShaderForge.SFN_Multiply,id:11,x:33239,y:33109|1-9-0,2-6-0;n:type:ShaderForge.SFN_Multiply,id:12,x:33226,y:33303|1-10-0,2-7-0;n:type:ShaderForge.SFN_Add,id:13,x:33088,y:32986|1-1-0,2-11-0;n:type:ShaderForge.SFN_Add,id:14,x:33070,y:33151|1-13-0,2-12-0;n:type:ShaderForge.SFN_OneMinus,id:15,x:32891,y:33109|1-14-0;n:type:ShaderForge.SFN_Vector3,id:16,x:33163,y:32538,v1:0.9,v2:0.6,v3:0.3;n:type:ShaderForge.SFN_Multiply,id:18,x:32586,y:33392|1-19-0,2-36-0;n:type:ShaderForge.SFN_Power,id:19,x:32692,y:33168|1-15-0,2-46-0;n:type:ShaderForge.SFN_Multiply,id:21,x:32332,y:33439|1-18-0,2-23-0;n:type:ShaderForge.SFN_Slider,id:23,x:32491,y:34006,min:0,cur:0.6466165,max:1;n:type:ShaderForge.SFN_Slider,id:24,x:33245,y:33408,min:0,cur:6.390977,max:10;n:type:ShaderForge.SFN_Tex2d,id:26,x:32333,y:32846,tex:a068f95a1cdfbec46b7c6aee5b78c0ba|0-33-0;n:type:ShaderForge.SFN_Append,id:27,x:32655,y:32564|1-29-0,2-30-0;n:type:ShaderForge.SFN_Append,id:28,x:32517,y:32651|1-27-0,2-32-0;n:type:ShaderForge.SFN_Slider,id:29,x:32919,y:32510,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Slider,id:30,x:32912,y:32602,min:0,cur:0.2406015,max:1;n:type:ShaderForge.SFN_Transform,id:31,x:33504,y:33147,s:0;n:type:ShaderForge.SFN_Slider,id:32,x:32745,y:32703,min:0,cur:0.06015039,max:1;n:type:ShaderForge.SFN_Append,id:33,x:32494,y:32886|1-34-0,2-19-0;n:type:ShaderForge.SFN_Slider,id:34,x:32752,y:32782,min:0,cur:0.3759398,max:1;n:type:ShaderForge.SFN_Multiply,id:35,x:32358,y:32681|1-28-0,2-26-2;n:type:ShaderForge.SFN_Append,id:36,x:32902,y:33946|1-37-0,2-39-0;n:type:ShaderForge.SFN_Append,id:37,x:33079,y:33916|1-38-0,2-40-0;n:type:ShaderForge.SFN_Slider,id:38,x:33341,y:33853,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Slider,id:39,x:33139,y:34048,min:0,cur:0.2631579,max:1;n:type:ShaderForge.SFN_Slider,id:40,x:33347,y:33949,min:0,cur:0.1052631,max:1;n:type:ShaderForge.SFN_Time,id:41,x:33486,y:33681;n:type:ShaderForge.SFN_Tex2d,id:42,x:33056,y:33506,tex:39f86a4edef13da4792b534bb27a5c4b|0-43-0;n:type:ShaderForge.SFN_Append,id:43,x:33152,y:33682|1-41-1,2-44-0;n:type:ShaderForge.SFN_Vector1,id:44,x:33489,y:33805,v1:0;n:type:ShaderForge.SFN_Multiply,id:45,x:32886,y:33322|1-42-3,2-24-0;n:type:ShaderForge.SFN_Add,id:46,x:33019,y:33264|1-47-0,2-45-0;n:type:ShaderForge.SFN_Slider,id:47,x:33281,y:33206,min:0,cur:0,max:5;pass:END;sub:END;*/

Shader "Shader Forge/test" {
    Properties {
        _node_23 ("", Range(0, 1)) = 0
        _node_24 ("", Range(0, 10)) = 0
        _node_26 ("", 2D) = "white" {}
        _node_29 ("", Range(0, 1)) = 0
        _node_30 ("", Range(0, 1)) = 0
        _node_32 ("", Range(0, 1)) = 0
        _node_34 ("", Range(0, 1)) = 0
        _node_38 ("", Range(0, 1)) = 0
        _node_39 ("", Range(0, 1)) = 0
        _node_40 ("", Range(0, 1)) = 0
        _node_42 ("", 2D) = "white" {}
        _node_47 ("", Range(0, 5)) = 0
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
            uniform float4 _TimeEditor;
            uniform float _node_23;
            uniform float _node_24;
            uniform sampler2D _node_26;
            uniform float _node_29;
            uniform float _node_30;
            uniform float _node_32;
            uniform float _node_34;
            uniform float _node_38;
            uniform float _node_39;
            uniform float _node_40;
            uniform sampler2D _node_42;
            uniform float _node_47;
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
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = normalize(i.normalDir);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float atten = 1.0;
                float3 lambert = atten * max( 0.0, dot(normalDirection,lightDirection )) * _LightColor0.xyz;
                float3 node_2 = normalDirection;
                float3 node_3 = viewDirection;
                float node_19 = pow((1.0 - (((node_2.r*node_3.r)+(node_2.g*node_3.g))+(node_2.b*node_3.b))),(_node_47+(tex2D(_node_42,float2(_TimeEditor.g,0)).r*_node_24)));
                float3 addLight = ((node_19*float3(float2(_node_38,_node_40),_node_39))*_node_23);
                float3 lightFinal = lambert + UNITY_LIGHTMODEL_AMBIENT.xyz;
                return fixed4(lightFinal * (float3(float2(_node_29,_node_30),_node_32)*tex2D(_node_26,float2(_node_34,node_19)).rgb) + addLight,1);
            }
            ENDCG
        }
    }
    FallBack "Specular"
}