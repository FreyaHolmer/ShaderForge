// Shader created with Shader Forge Alpha 0.10 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.10;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:0,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,uamb:False,ufog:True,aust:True,igpj:False,qofs:0,lico:0,qpre:1,flbk:Custom/Test Shadowed;n:type:ShaderForge.SFN_Final,id:0,x:32656,y:32502|8-11-0;n:type:ShaderForge.SFN_Lerp,id:1,x:33201,y:32624|1-2-0,2-3-0,3-6-0;n:type:ShaderForge.SFN_Vector3,id:2,x:33532,y:32522,v1:1,v2:0,v3:0;n:type:ShaderForge.SFN_Vector3,id:3,x:33528,y:32722,v1:0,v2:1,v3:0;n:type:ShaderForge.SFN_VertexColor,id:4,x:33542,y:32845;n:type:ShaderForge.SFN_Tex2d,id:5,x:33542,y:33073,ptlb:blendmod,tex:66321cc856b03e245ac41ed8a53e0ecc;n:type:ShaderForge.SFN_Multiply,id:6,x:33270,y:32988|1-4-4,2-5-6;n:type:ShaderForge.SFN_OneMinus,id:7,x:33270,y:33123|1-5-3;n:type:ShaderForge.SFN_Multiply,id:8,x:33270,y:33242|1-5-3,2-9-0;n:type:ShaderForge.SFN_Vector1,id:9,x:33469,y:33275,v1:1;n:type:ShaderForge.SFN_Multiply,id:11,x:32894,y:32624|1-12-0,2-7-0;n:type:ShaderForge.SFN_Vector3,id:12,x:33058,y:32503,v1:1,v2:1,v3:1;pass:END;sub:END;*/

Shader "Shader Forge/Vertex Blend" {
    Properties {
        _blendmod ("blendmod", 2D) = "white" {}
    }
    SubShader {
        Pass {
            Tags {
                "LightMode"="ForwardBase"
            }
            
            CGPROGRAM
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma vertex vert
            #pragma fragment frag
            #pragma exclude_renderers gles xbox360 ps3 flash 
            #pragma target 3.0
            uniform sampler2D _blendmod;
            struct VertexInput {
                float4 vertex : POSITION;
                float4 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(1,2)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.uv0 = v.uv0;
                o.vertexColor = v.vertexColor;
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float4 node_5 = tex2D(_blendmod,i.uv0.xy);
                return fixed4((float3(1,1,1)*(1.0 - node_5.r)),1);
            }
            ENDCG
        }
    }
    FallBack "Specular"
}
