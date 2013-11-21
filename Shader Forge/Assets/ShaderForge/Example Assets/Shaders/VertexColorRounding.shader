// Shader created with Shader Forge Alpha 0.15 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.15;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:0,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,uamb:False,ufog:False,aust:True,igpj:False,qofs:0,lico:0,qpre:1,flbk:,rntp:1,lmpd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True;n:type:ShaderForge.SFN_Final,id:0,x:33043,y:32815|emission-10-OUT;n:type:ShaderForge.SFN_VertexColor,id:4,x:33846,y:32852;n:type:ShaderForge.SFN_Round,id:7,x:33484,y:32892|IN-8-OUT;n:type:ShaderForge.SFN_Multiply,id:8,x:33659,y:32892|A-4-RGB,B-137-OUT;n:type:ShaderForge.SFN_Divide,id:10,x:33305,y:32981|A-7-OUT,B-137-OUT;n:type:ShaderForge.SFN_Slider,id:38,x:34221,y:32974,ptlb:Divisions,min:0,cur:0.4360902,max:1;n:type:ShaderForge.SFN_Power,id:42,x:34030,y:32997|VAL-38-OUT,EXP-44-OUT;n:type:ShaderForge.SFN_Vector1,id:44,x:34221,y:33041,v1:2;n:type:ShaderForge.SFN_ConstantLerp,id:137,x:33846,y:32997,a:0.5,b:12|IN-42-OUT;proporder:38;pass:END;sub:END;*/

Shader "Shader Forge/Examples/Vertex Color Rounding" {
    Properties {
        _Divisions ("Divisions", Range(0, 1)) = 0
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Tags {
                "LightMode"="ForwardBase"
            }
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers gles xbox360 ps3 flash 
            #pragma target 3.0
            #pragma glsl
            uniform float _Divisions;
            struct VertexInput {
                float4 vertex : POSITION;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.vertexColor = v.vertexColor;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
//////// DEBUG - Lighting()
                float node_137 = lerp(0.5,12,pow(_Divisions,2.0));
                float3 lightFinal = (round((i.vertexColor.rgb*node_137))/node_137);
//////// DEBUG - Final output color
                return fixed4(lightFinal * float3(1,1,1),1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
