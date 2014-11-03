// Shader created with Shader Forge Beta 0.37 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.37;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:True,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,arlg:0,hqsc:True,hqlp:False,tesm:0,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:0,x:33556,y:32453,varname:node_0,prsc:2|diff-270-RGB,spec-6-OUT,gloss-4-OUT,normal-9-RGB,lwrap-272-RGB;n:type:ShaderForge.SFN_Vector1,id:3,x:33060,y:32629,varname:node_3,prsc:2,v1:0.3;n:type:ShaderForge.SFN_Vector1,id:4,x:33250,y:32637,varname:node_4,prsc:2,v1:2;n:type:ShaderForge.SFN_OneMinus,id:5,x:33060,y:32486,varname:node_5,prsc:2|IN-270-RGB;n:type:ShaderForge.SFN_Multiply,id:6,x:33250,y:32486,varname:node_6,prsc:2|A-5-OUT,B-3-OUT;n:type:ShaderForge.SFN_Tex2d,id:9,x:33250,y:32717,ptovrint:False,ptlb:Normal,ptin:_Normal,varname:node_7,prsc:2,tex:80286949e259c2d44876306923857245,ntxv:3,isnm:True|UVIN-276-UVOUT;n:type:ShaderForge.SFN_Color,id:270,x:32858,y:32412,ptovrint:False,ptlb:Diffuse,ptin:_Diffuse,varname:node_10,prsc:2,glob:False,c1:0.9019608,c2:0.7019608,c3:0.3764706,c4:1;n:type:ShaderForge.SFN_Color,id:272,x:33250,y:32896,ptovrint:False,ptlb:Light Wrapping,ptin:_LightWrapping,varname:node_271,prsc:2,glob:False,c1:0.9058824,c2:0.4941176,c3:0.4901961,c4:1;n:type:ShaderForge.SFN_TexCoord,id:276,x:33510,y:32596,varname:node_276,prsc:2,uv:0;proporder:9-272-270;pass:END;sub:END;*/

Shader "Shader Forge/Examples/LightWrapping" {
    SubShader {
        Tags { "RenderType"="Opaque" }
        
        Pass {
            Tags { "LightMode"="ForwardBase" }
            Name "ForwardBase"
            Blend One One
            Fog {Mode Off}
            
            CGPROGRAM
            #define UNITY_PASS_FORWARDBASE
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
            };
            VertexOutput vert(VertexInput v) {
                VertexOutput o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput o) {
                return float3(0,0,0);
            }
            ENDCG
        }
        
        Pass {
            Tags { "LightMode"="ForwardAdd" }
            Name "ForwardAdd"
            Blend One One
            Fog {Mode Off}
            
            CGPROGRAM
            #define UNITY_PASS_FORWARDADD
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
            };
            VertexOutput vert(VertexInput v) {
                VertexOutput o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput o) {
                return float3(0,0,0);
            }
            ENDCG
        }
        
    }
}
